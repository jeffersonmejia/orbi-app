using Microsoft.AspNetCore.Mvc;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class DeliveryDriversController : Controller
{
    private readonly DeliveryDriverService _driverService;

    public DeliveryDriversController(DeliveryDriverService driverService)
    {
        _driverService = driverService;
    }

    public async Task<IActionResult> Index(
        string? searchField, string? searchTerm, int page = 1)
    {
        var result = await _driverService.GetPaginatedAsync(searchField, searchTerm, page, 5);
        ViewBag.SearchField = searchField;
        ViewBag.SearchTerm = searchTerm;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var driver = await _driverService.GetByIdAsync(id);
        if (driver == null)
            return NotFound();

        return View(driver);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DeliveryDriverViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            await _driverService.CreateAsync(viewModel);
            TempData["Success"] = "Delivery driver created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var driver = await _driverService.GetByIdAsync(id);
        if (driver == null)
            return NotFound();

        return View(driver);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DeliveryDriverViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _driverService.UpdateAsync(viewModel);
                TempData["Success"] = "Delivery driver updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _driverService.SoftDeleteAsync(id);
            TempData["Success"] = "Delivery driver deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
