using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class ReviewsController : Controller
{
    private readonly ReviewService _reviewService;
    private readonly AppDbContext _context;

    public ReviewsController(ReviewService reviewService, AppDbContext context)
    {
        _reviewService = reviewService;
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? searchField, string? searchTerm, int page = 1)
    {
        var result = await _reviewService.GetPaginatedAsync(searchField, searchTerm, page, 5);
        ViewBag.SearchField = searchField;
        ViewBag.SearchTerm = searchTerm;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        if (review == null)
            return NotFound();

        return View(review);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCustomersDropDownListAsync();
        await PopulateStoresDropDownListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            await _reviewService.CreateAsync(viewModel);
            TempData["Success"] = "Review created successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateCustomersDropDownListAsync(viewModel.CustomerId);
        await PopulateStoresDropDownListAsync(viewModel.StoreId);
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        if (review == null)
            return NotFound();

        await PopulateCustomersDropDownListAsync(review.CustomerId);
        await PopulateStoresDropDownListAsync(review.StoreId);
        return View(review);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReviewViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _reviewService.UpdateAsync(viewModel);
                TempData["Success"] = "Review updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        await PopulateCustomersDropDownListAsync(viewModel.CustomerId);
        await PopulateStoresDropDownListAsync(viewModel.StoreId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _reviewService.SoftDeleteAsync(id);
            TempData["Success"] = "Review deleted successfully.";
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
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Take(200)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FirstName + " " + c.LastName
            })
            .ToListAsync();

        ViewBag.CustomerId = new SelectList(customers, "Value", "Text", selectedCustomer);
    }

    private async Task PopulateStoresDropDownListAsync(object? selectedStore = null)
    {
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

        ViewBag.StoreId = new SelectList(stores, "Value", "Text", selectedStore);
    }
}
