using System.Diagnostics;
using System.Data.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var model = new HomeDashboardViewModel(await GetTableCountsAsync());
            return View(model);
        }
        catch (Exception ex) when (IsDatabaseAccessError(ex))
        {
            _logger.LogError(ex, "Unable to load home record counts.");
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = StatusCodes.Status503ServiceUnavailable,
                Message = "The service is temporarily unavailable. Please try again later."
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> RecordCounts()
    {
        try
        {
            var model = new HomeDashboardViewModel(await GetTableCountsAsync());

            return Json(new
            {
                totalRecords = model.TotalRecords,
                maxRecords = model.MaxRecords,
                tableCounts = model.TableCounts.Select(item => new
                {
                    tableName = item.TableName,
                    count = item.Count
                })
            });
        }
        catch (Exception ex) when (IsDatabaseAccessError(ex))
        {
            _logger.LogError(ex, "Unable to refresh home record counts.");

            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                error = "Service temporarily unavailable"
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> DbStatus()
    {
        try
        {
            await _context.Database.CanConnectAsync();
            return Ok(new { available = true });
        }
        catch (Exception ex) when (IsDatabaseAccessError(ex))
        {
            _logger.LogWarning(ex, "Database health check failed.");

            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                available = false
            });
        }
    }

    private async Task<List<TableCountViewModel>> GetTableCountsAsync()
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

        return counts;
    }

    private static bool IsDatabaseAccessError(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is DbException or TimeoutException or InvalidOperationException)
            {
                return true;
            }
        }

        return false;
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
        var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        var isDatabaseError = exception is not null && IsDatabaseAccessError(exception);
        var statusCode = isDatabaseError
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status500InternalServerError;

        Response.StatusCode = statusCode;

        if (exception is not null)
        {
            _logger.LogError(exception, "Unhandled request error.");
        }

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = statusCode,
            Message = isDatabaseError
                ? "The service is temporarily unavailable. Please try again later."
                : "An error occurred while processing your request."
        });
    }
}
