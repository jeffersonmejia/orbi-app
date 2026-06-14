using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class StoresController : Controller
{
    private readonly StoreService _storeService;
    private readonly AppDbContext _context;

    public StoresController(StoreService storeService, AppDbContext context)
    {
        _storeService = storeService;
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? searchField, string? searchTerm, int page = 1)
    {
        var result = await _storeService.GetPaginatedAsync(searchField, searchTerm, page, 5);
        ViewBag.SearchField = searchField;
        ViewBag.SearchTerm = searchTerm;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var store = await _storeService.GetByIdAsync(id);
        if (store == null)
            return NotFound();

        return View(store);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesDropDownListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StoreViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            await _storeService.CreateAsync(viewModel);
            TempData["Success"] = "Store created successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateCategoriesDropDownListAsync(viewModel.CategoryId);
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var store = await _storeService.GetByIdAsync(id);
        if (store == null)
            return NotFound();

        await PopulateCategoriesDropDownListAsync(store.CategoryId);
        return View(store);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StoreViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _storeService.UpdateAsync(viewModel);
                TempData["Success"] = "Store updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        await PopulateCategoriesDropDownListAsync(viewModel.CategoryId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _storeService.SoftDeleteAsync(id);
            TempData["Success"] = "Store deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesDropDownListAsync(object? selectedCategory = null)
    {
        var categories = await _context.StoreCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToListAsync();

        ViewBag.CategoryId = new SelectList(categories, "Value", "Text", selectedCategory);
    }
}
