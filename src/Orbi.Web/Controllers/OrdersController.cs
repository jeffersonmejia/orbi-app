using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class OrdersController : Controller
{
    private readonly OrderService _orderService;
    private readonly AppDbContext _context;

    public OrdersController(OrderService orderService, AppDbContext context)
    {
        _orderService = orderService;
        _context = context;
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
            .Where(c => c.IsActive)
            .OrderBy(c => c.FirstName)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FirstName + " " + c.LastName
            })
            .ToListAsync();

        var stores = await _context.Stores
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToListAsync();

        var drivers = await _context.DeliveryDrivers
            .Where(d => d.IsActive && d.IsAvailable)
            .OrderBy(d => d.FirstName)
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.FirstName + " " + d.LastName
            })
            .ToListAsync();
        drivers.Insert(0, new SelectListItem { Value = "", Text = "-- No Driver --" });

        var orderStatuses = await _context.OrderStatuses
            .OrderBy(os => os.Name)
            .Select(os => new SelectListItem
            {
                Value = os.Id.ToString(),
                Text = os.Name
            })
            .ToListAsync();

        var addresses = await _context.Addresses
            .Where(a => a.IsActive)
            .OrderBy(a => a.Street)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Street + ", " + a.City
            })
            .ToListAsync();

        var products = await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
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
