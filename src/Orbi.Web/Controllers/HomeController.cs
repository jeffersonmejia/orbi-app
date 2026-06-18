using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orbi.Web.Models;

namespace Orbi.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Status(int code)
    {
        return code switch
        {
            StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden => RedirectToAction(nameof(NotAuthorized)),
            StatusCodes.Status404NotFound => RedirectToAction(nameof(NotFoundPage)),
            _ => RedirectToAction(nameof(Error))
        };
    }

    public IActionResult NotAuthorized()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }

    public IActionResult NotFoundPage()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("NotFound");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
