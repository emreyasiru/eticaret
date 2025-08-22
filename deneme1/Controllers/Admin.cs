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
        [HttpPost]
        public IActionResult KategoriEkle(string Kategori_Adi,string altkategori)
        {
            if (altkategori == "0")
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
    }
}
