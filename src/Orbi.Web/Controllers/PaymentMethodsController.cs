using Microsoft.AspNetCore.Mvc;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class PaymentMethodsController : Controller
{
    private readonly PaymentMethodService _methodService;

    public PaymentMethodsController(PaymentMethodService methodService)
    {
        _methodService = methodService;
    }

    public async Task<IActionResult> Index(
        string? searchField, string? searchTerm, int page = 1)
    {
        var result = await _methodService.GetPaginatedAsync(searchField, searchTerm, page, 5);
        ViewBag.SearchField = searchField;
        ViewBag.SearchTerm = searchTerm;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var method = await _methodService.GetByIdAsync(id);
        if (method == null)
            return NotFound();

        return View(method);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentMethodViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            await _methodService.CreateAsync(viewModel);
            TempData["Success"] = "Payment method created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var method = await _methodService.GetByIdAsync(id);
        if (method == null)
            return NotFound();

        return View(method);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PaymentMethodViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _methodService.UpdateAsync(viewModel);
                TempData["Success"] = "Payment method updated successfully.";
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
            await _methodService.SoftDeleteAsync(id);
            TempData["Success"] = "Payment method deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
