using eticaret.Models;
using Microsoft.AspNetCore.Mvc;
using eticaret.Modeller;
namespace eticaret.Controllers
{
    public class Admin : Controller
    {
        private readonly EticaretContext _db;
        public Admin(EticaretContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Giris", "Admin");
            }
            return View();
        }
        public IActionResult Giris()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Kategoriler() 
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Giris", "Admin");
            }
            var anaktg = _db.AnaKategoris.ToList();
            var altktg=_db.AltKategoris.ToList();
            var ktg = new Kategori
            {
                AnaKategoriList = anaktg,
                AltKategoriList = altktg
            };
            return View(ktg);
        }
        [HttpGet]
        public IActionResult Urunler()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Giris", "Admin");
            }

            try
            {
                // Önce AltKategoris ile join dene
                var urunlerAltKategori = (from u in _db.Urunlers
                                          join a in _db.AltKategoris on u.KategoriId equals a.Id
                                          select new UrunListeView
                                          {
                                              Id = u.Id,
                                              UrunAdi = u.UrunAdi,
                                              Stok = u.Stok,
                                              KategoriId = u.KategoriId,
                                              KategoriAdi = a.KategoriAdi,
                                              Alis = u.Alis,
                                              Satis = u.Satis,
                                              IndirimliFiyat = u.IndirimliFiyat,
                                              Aciklama = u.Aciklama
                                          }).ToList();

                // Eğer AltKategoris'ten sonuç gelmezse, AnaKategoris'ten dene
                if (!urunlerAltKategori.Any())
                {
                    var urunlerAnaKategori = (from u in _db.Urunlers
                                              join ak in _db.AnaKategoris on u.KategoriId equals ak.Id
                                              select new UrunListeView
                                              {
                                                  Id = u.Id,
                                                  UrunAdi = u.UrunAdi,
                                                  Stok = u.Stok,
                                                  KategoriId = u.KategoriId,
                                                  KategoriAdi = ak.KategoriAdi,
                                                  Alis = u.Alis,
                                                  Satis = u.Satis,
                                                  IndirimliFiyat = u.IndirimliFiyat,
                                                  Aciklama = u.Aciklama
                                              }).ToList();

                    if (urunlerAnaKategori.Any())
                    {
                        return View(urunlerAnaKategori);
                    }
                }
                else
                {
                    return View(urunlerAltKategori);
                }

                // Her iki durumda da sonuç yoksa, left join ile tüm ürünleri getir
                var tumUrunler = (from u in _db.Urunlers
                                  from ak in _db.AnaKategoris.Where(x => x.Id == u.KategoriId).DefaultIfEmpty()
                                  from alt in _db.AltKategoris.Where(x => x.Id == u.KategoriId).DefaultIfEmpty()
                                  select new UrunListeView
                                  {
                                      Id = u.Id,
                                      UrunAdi = u.UrunAdi,
                                      Stok = u.Stok,
                                      KategoriId = u.KategoriId,
                                      KategoriAdi = alt != null ? alt.KategoriAdi :
                                                   (ak != null ? ak.KategoriAdi : "Kategori Bulunamadı"),
                                      Alis = u.Alis,
                                      Satis = u.Satis,
                                      IndirimliFiyat = u.IndirimliFiyat,
                                      Aciklama = u.Aciklama
                                  }).ToList();

                return View(tumUrunler);
            }
            catch (Exception ex)
            {
                ViewBag.Hata = $"Ürünler yüklenirken hata oluştu: {ex.Message}";
                return View(new List<UrunListeView>());
            }
        }
        [HttpGet]
        public IActionResult UrunEkle()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Giris", "Admin");
            }
            var veriler = new UrunKayit
            {
                Vergilerim = _db.Vergis.ToList(),
                Kategorilerim = _db.AnaKategoris.ToList()
            };
            return View(veriler);
        }
        [HttpGet]
        public IActionResult KategoriGetir(int id)
        {
            var gelen = _db.AltKategoris.Where(x => x.AnaKategoriId == id && x.UstKategoriId == null).ToList();

            // Seçilen kategorinin adını al
            var secilenKategori = _db.AnaKategoris.FirstOrDefault(x => x.Id == id);

            var html = "";

            // Önce seçilen kategori adını göster
            if (secilenKategori != null)
            {
                html += "<div class='alert alert-info mt-2 secilen-kategori'>";
                html += "<strong>Seçilen:</strong> " + secilenKategori.KategoriAdi;
                html += "</div>";
            }

            // Eğer alt kategoriler varsa select oluştur
            if (gelen.Count > 0)
            {
                html += "<select class='form-select mt-2' onchange='secimim(this)' data-seviye='2'>";
                html += "<option value=''>Alt kategori seçiniz</option>";
                foreach (var item in gelen)
                {
                    html += "<option value='" + item.Id + "'>" + item.KategoriAdi + "</option>";
                }
                html += "</select>";
            }

            return Content(html, "text/html");
        }

        [HttpGet]
        public IActionResult AltKategoriGetir(int id)
        {
            var gelen = _db.AltKategoris.Where(x => x.UstKategoriId == id).ToList();

            // Seçilen alt kategorinin adını al
            var secilenAltKategori = _db.AltKategoris.FirstOrDefault(x => x.Id == id);

            var html = "";

            // Önce seçilen kategori adını göster
            if (secilenAltKategori != null)
            {
                html += "<div class='alert alert-success mt-2 secilen-kategori'>";
                html += "<strong>Seçilen:</strong> " + secilenAltKategori.KategoriAdi;
                html += "</div>";
            }

            // Eğer alt kategoriler varsa select oluştur
            if (gelen.Count > 0)
            {
                html += "<select class='form-select mt-2' onchange='secimim(this)' data-seviye='3'>";
                html += "<option value=''>Alt kategori seçiniz</option>";
                foreach (var item in gelen)
                {
                    html += "<option value='" + item.Id + "'>" + item.KategoriAdi + "</option>";
                }
                html += "</select>";
            }

            return Content(html, "text/html");
        }


        [HttpPost]
        public IActionResult KategoriEkle(string Kategori_Adi,string? altkategori, string anakategori)
        {
            if (anakategori == "0")
            {
                var ktgekle = new AnaKategori
                {
                    KategoriAdi = Kategori_Adi
                };
                _db.AnaKategoris.Add(ktgekle);
                _db.SaveChanges();
                ViewBag.hata = "kategori ekleme işlemi tamamlanmıştır";
            }
            else
            {
                int anaktgid=Convert.ToInt32(anakategori);
                if (altkategori == "0")
                {
                    var altktg = new AltKategori
                    {
                        AnaKategoriId = anaktgid,
                        KategoriAdi = Kategori_Adi,
                        Durum = true,
                        UstKategoriId=null
                    };
                    _db.AltKategoris.Add(altktg);
                }
                else
                {
                    int ustktgid=Convert.ToInt32(altkategori);
                    var altktg = new AltKategori
                    {
                        AnaKategoriId = anaktgid,
                        KategoriAdi = Kategori_Adi,
                        Durum = true,
                        UstKategoriId=ustktgid,
                    };
                    _db.AltKategoris.Add(altktg);
                }

                   
                _db.SaveChanges();

            }
                return RedirectToAction("Kategoriler","Admin");
        }
        [HttpPost]
        public IActionResult Giris(string username, string password)
        {
            var dogrula=_db.Kullanicis.FirstOrDefault(x => x.Username == username && x.Password==password);
            if(dogrula!=null)
            {
                if (dogrula.Durum == false)
                {
                    ViewBag.ErrorMessage = "kullanıcı hesabınız devre dışı bırakılmış";
                    return View();
                }

                HttpContext.Session.SetInt32("UserId", dogrula.Id);
                HttpContext.Session.SetString("username", username);
                return RedirectToAction("Index","Admin");
            }
            else
            {
                ViewBag.ErrorMessage = "kullanıcı adı veya şifre yanlış";
                return View();
            }
        }
        public IActionResult deneme()
        {
            return View();
        }
    }
}
