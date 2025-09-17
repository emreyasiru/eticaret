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
        public IActionResult magaza()
        {
            return View();
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
            // Mail g�nderme
            try
            {
                // Mail ayarlar�
                string gondericiMail = "emreyasiru@gmail.com";
                string gondericiSifre = "rcwv waeh xnum hpjj"; // Gmail uygulama �ifresi

                // SMTP client olu�tur
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(gondericiMail, gondericiSifre);
                smtp.EnableSsl = true;

                // Mail mesaj� olu�tur
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(gondericiMail, "E Ticaret");
                mailMessage.To.Add(mail);
                mailMessage.Subject = "Hesap Do�rulama Kodu";
                mailMessage.Body = $"Merhaba {isim},\n\nHesab�n�z� do�rulamak i�in kod: {dogrulamakodu}\n\n�yi g�nler!";
                mailMessage.IsBodyHtml = false;

                // Mail g�nder
                smtp.Send(mailMessage);

                // Ba�ar�l� mesaj
                TempData["Mesaj"] = "Do�rulama kodu mail adresinize g�nderildi.";
            }
            catch (Exception ex)
            {
                // Hata durumunda
                TempData["Hata"] = "Mail g�nderilirken hata olu�tu: " + ex.Message;
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
                TempData["Mesaj"] = "Hesab�n�z ba�ar�yla do�ruland�. Giri� yapabilirsiniz.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Hata"] = "Ge�ersiz do�rulama kodu. L�tfen tekrar deneyin.";
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
