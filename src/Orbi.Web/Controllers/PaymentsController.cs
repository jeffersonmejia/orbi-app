using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Orbi.Web.Data;
using Orbi.Web.Security;
using Orbi.Web.Services;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Controllers;

public class PaymentsController : Controller
{
    private readonly PaymentService _paymentService;
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly CurrentUserAccess _access;

    public PaymentsController(PaymentService paymentService, AppDbContext context, IMemoryCache cache, CurrentUserAccess access)
    {
        _paymentService = paymentService;
        _context = context;
        _cache = cache;
        _access = access;
    }

    public async Task<IActionResult> Index(
        string? searchField, string? searchTerm, int page = 1)
    {
        var result = await _paymentService.GetPaginatedAsync(searchField, searchTerm, page, 5);
        ViewBag.SearchField = searchField;
        ViewBag.SearchTerm = searchTerm;
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
            return NotFound();

        return View(payment);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateOrdersDropDownListAsync();
        await PopulatePaymentMethodsDropDownListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            await _paymentService.CreateAsync(viewModel);
            TempData["Success"] = "Payment created successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateOrdersDropDownListAsync(viewModel.OrderId);
        await PopulatePaymentMethodsDropDownListAsync(viewModel.PaymentMethodId);
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
            return NotFound();

        await PopulateOrdersDropDownListAsync(payment.OrderId);
        await PopulatePaymentMethodsDropDownListAsync(payment.PaymentMethodId);
        return View(payment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PaymentViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                await _paymentService.UpdateAsync(viewModel);
                TempData["Success"] = "Payment updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        await PopulateOrdersDropDownListAsync(viewModel.OrderId);
        await PopulatePaymentMethodsDropDownListAsync(viewModel.PaymentMethodId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _paymentService.SoftDeleteAsync(id);
            TempData["Success"] = "Payment deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateOrdersDropDownListAsync(object? selectedOrder = null)
    {
        var orders = await _access.ScopeOrders(_context.Orders)
            .AsNoTracking()
            .Where(o => o.IsActive)
            .OrderByDescending(o => o.OrderDate)
            .Take(200)
            .Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = "Order #" + o.Id + " - " + o.TotalAmount.ToString("C")
            })
            .ToListAsync();

        ViewBag.OrderId = new SelectList(orders, "Value", "Text", selectedOrder);
    }

    private async Task PopulatePaymentMethodsDropDownListAsync(object? selectedMethod = null)
    {
        var methods = await _cache.GetOrCreateAsync("dropdown:payment-methods", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return await _context.PaymentMethods
            .AsNoTracking()
            .Where(m => m.IsActive)
            .OrderBy(m => m.Name)
            .Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            })
            .ToListAsync();
        });

        ViewBag.PaymentMethodId = new SelectList(methods, "Value", "Text", selectedMethod);
    }
}
