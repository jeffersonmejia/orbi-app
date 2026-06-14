using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class OrderStatusService : IEntityService<OrderStatus, OrderStatusViewModel>
{
    private readonly AppDbContext _context;

    public OrderStatusService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderStatusViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<OrderStatusViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "name" => query.Where(s => s.Name.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<OrderStatusViewModel>(items, count, page, pageSize);
    }

    private IQueryable<OrderStatusViewModel> GetAllQuery()
    {
        return _context.OrderStatuses
            .OrderBy(s => s.Name)
            .Select(s => new OrderStatusViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                UpdatedAt = s.UpdatedAt,
                IsActive = s.IsActive
            });
    }

    public async Task<OrderStatusViewModel?> GetByIdAsync(int id)
    {
        var status = await _context.OrderStatuses
            .FirstOrDefaultAsync(s => s.Id == id);

        if (status == null) return null;

        return new OrderStatusViewModel
        {
            Id = status.Id,
            Name = status.Name,
            Description = status.Description,
            UpdatedAt = status.UpdatedAt,
            IsActive = status.IsActive
        };
    }

    public async Task CreateAsync(OrderStatusViewModel viewModel)
    {
        var status = new OrderStatus
        {
            Name = viewModel.Name,
            Description = viewModel.Description,
            IsActive = true
        };

        _context.OrderStatuses.Add(status);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrderStatusViewModel viewModel)
    {
        var status = await _context.OrderStatuses
            .FirstOrDefaultAsync(s => s.Id == viewModel.Id)
            ?? throw new KeyNotFoundException($"OrderStatus with Id {viewModel.Id} not found.");

        status.Name = viewModel.Name;
        status.Description = viewModel.Description;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var status = await _context.OrderStatuses
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new KeyNotFoundException($"OrderStatus with Id {id} not found.");

        status.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
