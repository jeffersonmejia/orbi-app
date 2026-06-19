using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var counts = new List<TableCountViewModel>
        {
            new("Customers", await _context.Customers.IgnoreQueryFilters().CountAsync(), "Customers", "bi-people-fill", "Directory"),
            new("Addresses", await _context.Addresses.IgnoreQueryFilters().CountAsync(), "Addresses", "bi-geo-alt-fill", "Directory"),
            new("StoreCategories", await _context.StoreCategories.IgnoreQueryFilters().CountAsync(), "StoreCategories", "bi-tags-fill", "Catalog"),
            new("Stores", await _context.Stores.IgnoreQueryFilters().CountAsync(), "Stores", "bi-shop", "Catalog"),
            new("Products", await _context.Products.IgnoreQueryFilters().CountAsync(), "Products", "bi-box-seam-fill", "Catalog"),
            new("Orders", await _context.Orders.IgnoreQueryFilters().CountAsync(), "Orders", "bi-bag-fill", "Orders"),
            new("OrderDetails", await _context.OrderDetails.IgnoreQueryFilters().CountAsync(), null, "bi-list-check", "Orders"),
            new("OrderStatuses", await _context.OrderStatuses.IgnoreQueryFilters().CountAsync(), "OrderStatuses", "bi-signpost-2-fill", "Orders"),
            new("DeliveryDrivers", await _context.DeliveryDrivers.IgnoreQueryFilters().CountAsync(), "DeliveryDrivers", "bi-truck", "Directory"),
            new("PaymentMethods", await _context.PaymentMethods.IgnoreQueryFilters().CountAsync(), "PaymentMethods", "bi-credit-card-2-front-fill", "Orders"),
            new("Payments", await _context.Payments.IgnoreQueryFilters().CountAsync(), "Payments", "bi-cash-stack", "Orders"),
            new("Reviews", await _context.Reviews.IgnoreQueryFilters().CountAsync(), "Reviews", "bi-chat-square-text-fill", "Catalog")
        };

        var model = new HomeDashboardViewModel(counts);
        return View(model);
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
