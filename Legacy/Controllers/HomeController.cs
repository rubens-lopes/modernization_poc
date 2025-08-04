using System;
using System.Linq;
using System.Web.Mvc;

using ModernizationPoC.Legacy.DAL;

namespace ModernizationPoC.Legacy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        
        public ActionResult Index()
        {
            Console.WriteLine(_db.Toggles.Count());
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
