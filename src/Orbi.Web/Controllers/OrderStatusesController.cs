using Microsoft.AspNetCore.Mvc;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class OrderStatusesController : Controller
{
    private readonly OrderStatusService _statusService;

    public OrderStatusesController(OrderStatusService statusService)
    {
        _statusService = statusService;
    }

    public async Task<IActionResult> Index(
        string? searchField, string? searchTerm, int page = 1)
    {
        var result = await _statusService.GetPaginatedAsync(searchField, searchTerm, page, 5);
        ViewBag.SearchField = searchField;
        ViewBag.SearchTerm = searchTerm;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var status = await _statusService.GetByIdAsync(id);
        if (status == null)
            return NotFound();

        return View(status);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderStatusViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            await _statusService.CreateAsync(viewModel);
            TempData["Success"] = "Order status created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var status = await _statusService.GetByIdAsync(id);
        if (status == null)
            return NotFound();

        return View(status);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrderStatusViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _statusService.UpdateAsync(viewModel);
                TempData["Success"] = "Order status updated successfully.";
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
            await _statusService.SoftDeleteAsync(id);
            TempData["Success"] = "Order status deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
