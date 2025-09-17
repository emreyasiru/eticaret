using deneme1.Models;
using eticaret.Models;
using eticaret.Modeller;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NuGet.Versioning;

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
        public IActionResult Uye()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
