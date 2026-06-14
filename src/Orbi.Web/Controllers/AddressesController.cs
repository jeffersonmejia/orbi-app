using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class AddressesController : Controller
{
    private readonly AddressService _addressService;
    private readonly AppDbContext _context;

    public AddressesController(AddressService addressService, AppDbContext context)
    {
        _addressService = addressService;
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? searchField, string? searchTerm, int page = 1)
    {
        var result = await _addressService.GetPaginatedAsync(searchField, searchTerm, page, 5);
        ViewBag.SearchField = searchField;
        ViewBag.SearchTerm = searchTerm;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var address = await _addressService.GetByIdAsync(id);
        if (address == null)
            return NotFound();

        return View(address);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCustomersDropDownListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddressViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            await _addressService.CreateAsync(viewModel);
            TempData["Success"] = "Address created successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateCustomersDropDownListAsync(viewModel.CustomerId);
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var address = await _addressService.GetByIdAsync(id);
        if (address == null)
            return NotFound();

        await PopulateCustomersDropDownListAsync(address.CustomerId);
        return View(address);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AddressViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _addressService.UpdateAsync(viewModel);
                TempData["Success"] = "Address updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        await PopulateCustomersDropDownListAsync(viewModel.CustomerId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _addressService.SoftDeleteAsync(id);
            TempData["Success"] = "Address deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCustomersDropDownListAsync(object? selectedCustomer = null)
    {
        var customers = await _context.Customers
            .Where(c => c.IsActive)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FirstName + " " + c.LastName
            })
            .ToListAsync();

        ViewBag.CustomerId = new SelectList(customers, "Value", "Text", selectedCustomer);
    }
}
