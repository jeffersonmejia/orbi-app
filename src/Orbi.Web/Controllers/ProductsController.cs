using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Security;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class ProductsController : Controller
{
    private readonly ProductService _productService;
    private readonly AppDbContext _context;
    private readonly CurrentUserAccess _access;

    public ProductsController(ProductService productService, AppDbContext context, CurrentUserAccess access)
    {
        _productService = productService;
        _context = context;
        _access = access;
    }

    public async Task<IActionResult> Index(
        string? searchField, string? searchTerm, int page = 1)
    {
        var result = await _productService.GetPaginatedAsync(searchField, searchTerm, page, 5);
        ViewBag.SearchField = searchField;
        ViewBag.SearchTerm = searchTerm;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateStoresDropDownListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            await _productService.CreateAsync(viewModel);
            TempData["Success"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateStoresDropDownListAsync(viewModel.StoreId);
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        await PopulateStoresDropDownListAsync(product.StoreId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _productService.UpdateAsync(viewModel);
                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        await PopulateStoresDropDownListAsync(viewModel.StoreId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.SoftDeleteAsync(id);
            TempData["Success"] = "Product deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateStoresDropDownListAsync(object? selectedStore = null)
    {
        var stores = await _access.ScopeStores(_context.Stores)
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
