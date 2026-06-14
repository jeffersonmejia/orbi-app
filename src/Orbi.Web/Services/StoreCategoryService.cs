using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class StoreCategoryService : IEntityService<StoreCategory, StoreCategoryViewModel>
{
    private readonly AppDbContext _context;

    public StoreCategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StoreCategoryViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<StoreCategoryViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "name" => query.Where(c => c.Name.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<StoreCategoryViewModel>(items, count, page, pageSize);
    }

    private IQueryable<StoreCategoryViewModel> GetAllQuery()
    {
        return _context.StoreCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new StoreCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                UpdatedAt = c.UpdatedAt,
                IsActive = c.IsActive
            });
    }

    public async Task<StoreCategoryViewModel?> GetByIdAsync(int id)
    {
        var category = await _context.StoreCategories
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (category == null) return null;

        return new StoreCategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            UpdatedAt = category.UpdatedAt,
            IsActive = category.IsActive
        };
    }

    public async Task CreateAsync(StoreCategoryViewModel viewModel)
    {
        var category = new StoreCategory
        {
            Name = viewModel.Name,
            Description = viewModel.Description,
            IsActive = true
        };

        _context.StoreCategories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StoreCategoryViewModel viewModel)
    {
        var category = await _context.StoreCategories
            .FirstOrDefaultAsync(c => c.Id == viewModel.Id && c.IsActive)
            ?? throw new KeyNotFoundException($"StoreCategory with Id {viewModel.Id} not found.");

        category.Name = viewModel.Name;
        category.Description = viewModel.Description;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var category = await _context.StoreCategories
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive)
            ?? throw new KeyNotFoundException($"StoreCategory with Id {id} not found.");

        category.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
