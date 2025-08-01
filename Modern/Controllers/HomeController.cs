using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using ModernizationPoc.Modern.Models;

namespace ModernizationPoc.Modern.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    // public IActionResult Index()
    // {
    //     return View();
    // }
    //
    // public IActionResult Privacy()
    // {
    //     return View();
    // }

    public ActionResult About()
    {
        ViewBag.Message = "Your <b>modern</b> application description page.";
        logger.LogInformation("About");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        logger.LogError("Error");
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}