using deneme1.Models;
using eticaret.Modeller;
using eticaret.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace deneme1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EticaretContext _db;

        public HomeController(ILogger<HomeController> logger, EticaretContext db)
        {
            _db = db;
            _logger = logger;
        }

       

        public IActionResult Index()
        {
            var urunler = new UrunListesi
            {
                Kategorilerim = _db.AnaKategoris.ToList(),
                Urunlerim=_db.Urunlers.ToList(),
                UrunGorsellerim = _db.UrunGorsels.ToList(),
                Altkategorilerim=_db.AltKategoris.ToList()
            };
            return View(urunler);
        }

        public IActionResult Iletisim()
        {
            return View();
        }
        [HttpGet]
        public IActionResult magaza()
        {
            var urunler = new UrunListesi
            {
                Kategorilerim = _db.AnaKategoris.ToList(),
                Urunlerim = _db.Urunlers.ToList(),
                UrunGorsellerim = _db.UrunGorsels.ToList(),
                Altkategorilerim = _db.AltKategoris.ToList(),
                UrunDetaylarým = _db.UrunDetays.ToList()
            };

            return View(urunler);
        }
        [HttpPost]
        public IActionResult Giriss(string mail, string sifre)
        {
            var kullanici = _db.Misafirs.FirstOrDefault(x => x.Mail == mail && x.Sifre == sifre &&x.Durum==true);
            if (kullanici != null)
            {
               
                    HttpContext.Session.SetInt32("UserId", kullanici.Id);
                    HttpContext.Session.SetString("username", kullanici.Isim);
                    return RedirectToAction("Index", "Home");
               
            }
            else
            {
                TempData["Hata"] = "Geçersiz mail veya þifre.";
                return RedirectToAction("Uye", "Home");
            }
        }
        public IActionResult Cikis()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Uye()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Uye(string isim,string mail,string sifre)
        {
            var dogrulamakodu=new Random().Next(1000,9999).ToString();
            var misafir = new Misafir
            {
                Isim = isim,
                Mail = mail,
                Sifre = sifre,
                Kod = dogrulamakodu,
                Durum= false
            };
            _db.Misafirs.Add(misafir);
            _db.SaveChanges();
            // Mail gönderme
            try
            {
                // Mail ayarlarý
                string gondericiMail = "emreyasiru@gmail.com";
                string gondericiSifre = "rcwv waeh xnum hpjj"; // Gmail uygulama þifresi

                // SMTP client oluþtur
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(gondericiMail, gondericiSifre);
                smtp.EnableSsl = true;

                // Mail mesajý oluþtur
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(gondericiMail, "E Ticaret");
                mailMessage.To.Add(mail);
                mailMessage.Subject = "Hesap Doðrulama Kodu";
                mailMessage.Body = $"Merhaba {isim},\n\nHesabýnýzý doðrulamak için kod: {dogrulamakodu}\n\nÝyi günler!";
                mailMessage.IsBodyHtml = false;

                // Mail gönder
                smtp.Send(mailMessage);

                // Baþarýlý mesaj
                TempData["Mesaj"] = "Doðrulama kodu mail adresinize gönderildi.";
            }
            catch (Exception ex)
            {
                // Hata durumunda
                TempData["Hata"] = "Mail gönderilirken hata oluþtu: " + ex.Message;
            }

            return RedirectToAction("Dogrula", "Home");
        }
        [HttpGet]
        public IActionResult Dogrula()
        {
          
            return View();
        }
        [HttpPost]
        public IActionResult Dogrula(string kod)
        {
            var onay = _db.Misafirs.FirstOrDefault(x => x.Kod == kod);
            if (onay != null)
            {
                onay.Durum = true;
                _db.SaveChanges();
                HttpContext.Session.SetInt32("UserId", onay.Id);
                HttpContext.Session.SetString("username", onay.Isim);
               
                TempData["Mesaj"] = "Hesabýnýz baþarýyla doðrulandý. Giriþ yapabilirsiniz.";
                TempData["Basarili"] = "true"; // Baþarýlý doðrulama için flag
                return View(); // Önce view'ý döndür, JavaScript ile yönlendirme yapacaðýz
            }
            else
            {
                TempData["Hata"] = "Hatalý kod girdiniz. Lütfen tekrar deneyin.";
                return View();
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
