using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using ModernizationPoC.Modern;
using ModernizationPoC.Modern.DAL;

using ModernizationPoc.Modern.Models;

using ModernizationPoC.Shared;

using Yarp.ReverseProxy.Configuration;

namespace ModernizationPoc.Modern.Controllers;

public class HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, IProxyConfigProvider configProvider) : Controller
{
    public ActionResult About()
    {
        ViewBag.Message = "Your <b>modern</b> application description page.";

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        logger.LogError("Error");
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<ActionResult> UseLegacyAbout()
    {
        await UseAboutAsync(useNet80: false);
        ViewBag.Message = "About page is using net48";

        return View("About");
    }

    public async Task<ActionResult> UseModernAbout()
    {
        await UseAboutAsync(useNet80: true);
        ViewBag.Message = "About page is using net80";
            
        return View("About");
    }

    private async Task UseAboutAsync(bool useNet80)
    {
        var toggle = dbContext.Toggles.FirstOrDefault();
        if (toggle is null)
        {
            toggle = new ModernizationToggle { AboutPage = useNet80 };
            dbContext.Toggles.Add(toggle);
        }
        else
        {
            toggle.AboutPage = useNet80;
        }

        await dbContext.SaveChangesAsync();

        if (configProvider is DatabaseConfigProvider databaseConfigProvider) await databaseConfigProvider.ReloadAsync();
    }
}
