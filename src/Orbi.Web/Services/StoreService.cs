using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class StoreService : IEntityService<Store, StoreViewModel>
{
    private readonly AppDbContext _context;

    public StoreService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StoreViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<StoreViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "name" => query.Where(s => s.Name.ToLower().Contains(term)),
                "category" => query.Where(s => s.CategoryName!.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<StoreViewModel>(items, count, page, pageSize);
    }

    private IQueryable<StoreViewModel> GetAllQuery()
    {
        return _context.Stores
            .Include(s => s.Category)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new StoreViewModel
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                CategoryName = s.Category.Name,
                Name = s.Name,
                Description = s.Description,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                UpdatedAt = s.UpdatedAt,
                IsActive = s.IsActive
            });
    }

    public async Task<StoreViewModel?> GetByIdAsync(int id)
    {
        var store = await _context.Stores
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

        if (store == null) return null;

        return new StoreViewModel
        {
            Id = store.Id,
            CategoryId = store.CategoryId,
            CategoryName = store.Category.Name,
            Name = store.Name,
            Description = store.Description,
            Phone = store.Phone,
            Email = store.Email,
            Address = store.Address,
            Latitude = store.Latitude,
            Longitude = store.Longitude,
            UpdatedAt = store.UpdatedAt,
            IsActive = store.IsActive
        };
    }

    public async Task CreateAsync(StoreViewModel viewModel)
    {
        var store = new Store
        {
            CategoryId = viewModel.CategoryId,
            Name = viewModel.Name,
            Description = viewModel.Description,
            Phone = viewModel.Phone,
            Email = viewModel.Email,
            Address = viewModel.Address,
            Latitude = viewModel.Latitude,
            Longitude = viewModel.Longitude,
            IsActive = true
        };

        _context.Stores.Add(store);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StoreViewModel viewModel)
    {
        var store = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == viewModel.Id && s.IsActive)
            ?? throw new KeyNotFoundException($"Store with Id {viewModel.Id} not found.");

        store.CategoryId = viewModel.CategoryId;
        store.Name = viewModel.Name;
        store.Description = viewModel.Description;
        store.Phone = viewModel.Phone;
        store.Email = viewModel.Email;
        store.Address = viewModel.Address;
        store.Latitude = viewModel.Latitude;
        store.Longitude = viewModel.Longitude;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var store = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == id && s.IsActive)
            ?? throw new KeyNotFoundException($"Store with Id {id} not found.");

        store.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
