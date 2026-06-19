using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.Security;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class PaymentMethodService : IEntityService<PaymentMethod, PaymentMethodViewModel>
{
    private readonly AppDbContext _context;
    private readonly CurrentUserAccess _access;

    public PaymentMethodService(AppDbContext context, CurrentUserAccess access)
    {
        _context = context;
        _access = access;
    }

    public async Task<IEnumerable<PaymentMethodViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<PaymentMethodViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "name" => query.Where(p => p.Name.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<PaymentMethodViewModel>(items, count, page, pageSize);
    }

    private IQueryable<PaymentMethodViewModel> GetAllQuery()
    {
        return _context.PaymentMethods
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new PaymentMethodViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                UpdatedAt = p.UpdatedAt,
                IsActive = p.IsActive
            });
    }

    public async Task<PaymentMethodViewModel?> GetByIdAsync(int id)
    {
        var method = await _context.PaymentMethods
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (method == null) return null;

        return new PaymentMethodViewModel
        {
            Id = method.Id,
            Name = method.Name,
            Description = method.Description,
            UpdatedAt = method.UpdatedAt,
            IsActive = method.IsActive
        };
    }

    public async Task CreateAsync(PaymentMethodViewModel viewModel)
    {
        if (!_access.IsAdmin)
            throw new UnauthorizedAccessException();

        var method = new PaymentMethod
        {
            Name = viewModel.Name,
            Description = viewModel.Description,
            IsActive = true
        };

        _context.PaymentMethods.Add(method);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PaymentMethodViewModel viewModel)
    {
        if (!_access.IsAdmin)
            throw new UnauthorizedAccessException();

        var method = await _context.PaymentMethods
            .FirstOrDefaultAsync(p => p.Id == viewModel.Id && p.IsActive)
            ?? throw new KeyNotFoundException($"PaymentMethod with Id {viewModel.Id} not found.");

        method.Name = viewModel.Name;
        method.Description = viewModel.Description;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        if (!_access.IsAdmin)
            throw new UnauthorizedAccessException();

        var method = await _context.PaymentMethods
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive)
            ?? throw new KeyNotFoundException($"PaymentMethod with Id {id} not found.");

        method.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
