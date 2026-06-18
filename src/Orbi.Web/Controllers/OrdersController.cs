using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Orbi.Web.Data;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class OrdersController : Controller
{
    private readonly OrderService _orderService;
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    public OrdersController(OrderService orderService, AppDbContext context, IMemoryCache cache)
    {
        _orderService = orderService;
        _context = context;
        _cache = cache;
    }

    public async Task<IActionResult> Index(
        string? searchField, string? searchTerm, int page = 1)
    {
        var result = await _orderService.GetPaginatedAsync(searchField, searchTerm, page, 5);
        ViewBag.SearchField = searchField;
        ViewBag.SearchTerm = searchTerm;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null)
            return NotFound();

        return View(order);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropDownListsAsync();
        return View(new OrderViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            await _orderService.CreateAsync(viewModel);
            TempData["Success"] = "Order created successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropDownListsAsync(viewModel);
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null)
            return NotFound();

        await PopulateDropDownListsAsync(order);
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrderViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _orderService.UpdateAsync(viewModel);
                TempData["Success"] = "Order updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        await PopulateDropDownListsAsync(viewModel);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _orderService.SoftDeleteAsync(id);
            TempData["Success"] = "Order deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropDownListsAsync(object? selectedValues = null)
    {
        var customers = await _context.Customers
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.FirstName)
            .Take(200)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FirstName + " " + c.LastName
            })
            .ToListAsync();

        var stores = await _context.Stores
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Take(200)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToListAsync();

        var drivers = await _context.DeliveryDrivers
            .AsNoTracking()
            .Where(d => d.IsActive && d.IsAvailable)
            .OrderBy(d => d.FirstName)
            .Take(200)
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.FirstName + " " + d.LastName
            })
            .ToListAsync();
        drivers.Insert(0, new SelectListItem { Value = "", Text = "-- No Driver --" });

        var orderStatuses = await _cache.GetOrCreateAsync("dropdown:order-statuses", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return await _context.OrderStatuses
            .AsNoTracking()
            .OrderBy(os => os.Name)
            .Select(os => new SelectListItem
            {
                Value = os.Id.ToString(),
                Text = os.Name
            })
            .ToListAsync();
        });

        var addresses = await _context.Addresses
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.Street)
            .Take(200)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Street + ", " + a.City
            })
            .ToListAsync();

        var products = await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Take(200)
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name + " ($" + p.Price + ")"
            })
            .ToListAsync();

        ViewBag.CustomerId = new SelectList(customers, "Value", "Text", GetSelectedValue(selectedValues, "CustomerId"));
        ViewBag.StoreId = new SelectList(stores, "Value", "Text", GetSelectedValue(selectedValues, "StoreId"));
        ViewBag.DeliveryDriverId = new SelectList(drivers, "Value", "Text", GetSelectedValue(selectedValues, "DeliveryDriverId"));
        ViewBag.OrderStatusId = new SelectList(orderStatuses, "Value", "Text", GetSelectedValue(selectedValues, "OrderStatusId"));
        ViewBag.AddressId = new SelectList(addresses, "Value", "Text", GetSelectedValue(selectedValues, "AddressId"));
        ViewBag.ProductId = new SelectList(products, "Value", "Text");
    }

    private static object? GetSelectedValue(object? obj, string propertyName)
    {
        if (obj == null) return null;

        var prop = obj.GetType().GetProperty(propertyName);
        return prop?.GetValue(obj);
    }
}
