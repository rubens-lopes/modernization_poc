using System.Linq;
using System.Web.Mvc;

using ModernizationPoC.Legacy.DAL;
using ModernizationPoC.Shared;

namespace ModernizationPoC.Legacy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UseLegacyAbout()
        {
            UseAbout(useNet80: false);
            ViewBag.Message = "About page is using net48";

            return View("About");
        }

        public ActionResult UseModernAbout()
        {
            UseAbout(useNet80: true);
            ViewBag.Message = "About page is using net80";
            
            return View("About");
        }

        public ActionResult About()
        {
            ViewBag.Title = "About";
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        private void UseAbout(bool useNet80)
        {
            var toggle = _db.Toggles.FirstOrDefault();
            if (toggle is null)
            {
                toggle = new ModernizationToggle { AboutPage = useNet80 };
                _db.Toggles.Add(toggle);
            }
            else
            {
                toggle.AboutPage = useNet80;
            }

            _db.SaveChanges();
        }
    }
}
