using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class PaymentService : IEntityService<Payment, PaymentViewModel>
{
    private readonly AppDbContext _context;

    public PaymentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<PaymentViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "order" => query.Where(p => p.OrderInfo!.ToLower().Contains(term)),
                "method" => query.Where(p => p.PaymentMethodName!.ToLower().Contains(term)),
                "status" => query.Where(p => p.Status.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.PaymentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<PaymentViewModel>(items, count, page, pageSize);
    }

    private IQueryable<PaymentViewModel> GetAllQuery()
    {
        return _context.Payments
            .Include(p => p.Order)
            .Include(p => p.PaymentMethod)
            .Where(p => p.IsActive)
            .Select(p => new PaymentViewModel
            {
                Id = p.Id,
                OrderId = p.OrderId,
                OrderInfo = "Order #" + p.OrderId + " - " + p.Order.TotalAmount.ToString("C"),
                PaymentMethodId = p.PaymentMethodId,
                PaymentMethodName = p.PaymentMethod.Name,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                TransactionId = p.TransactionId,
                Status = p.Status,
                UpdatedAt = p.UpdatedAt,
                IsActive = p.IsActive
            });
    }

    public async Task<PaymentViewModel?> GetByIdAsync(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .Include(p => p.PaymentMethod)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (payment == null) return null;

        return new PaymentViewModel
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            OrderInfo = "Order #" + payment.OrderId + " - " + payment.Order.TotalAmount.ToString("C"),
            PaymentMethodId = payment.PaymentMethodId,
            PaymentMethodName = payment.PaymentMethod.Name,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            TransactionId = payment.TransactionId,
            Status = payment.Status,
            UpdatedAt = payment.UpdatedAt,
            IsActive = payment.IsActive
        };
    }

    public async Task CreateAsync(PaymentViewModel viewModel)
    {
        var payment = new Payment
        {
            OrderId = viewModel.OrderId,
            PaymentMethodId = viewModel.PaymentMethodId,
            Amount = viewModel.Amount,
            PaymentDate = DateTime.UtcNow,
            TransactionId = viewModel.TransactionId,
            Status = viewModel.Status,
            IsActive = true
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PaymentViewModel viewModel)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == viewModel.Id && p.IsActive)
            ?? throw new KeyNotFoundException($"Payment with Id {viewModel.Id} not found.");

        payment.OrderId = viewModel.OrderId;
        payment.PaymentMethodId = viewModel.PaymentMethodId;
        payment.Amount = viewModel.Amount;
        payment.TransactionId = viewModel.TransactionId;
        payment.Status = viewModel.Status;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive)
            ?? throw new KeyNotFoundException($"Payment with Id {id} not found.");

        payment.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
