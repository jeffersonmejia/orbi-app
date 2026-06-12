using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class ProductService : IEntityService<Product, ProductViewModel>
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<ProductViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "name" => query.Where(p => p.Name.ToLower().Contains(term)),
                "store" => query.Where(p => p.StoreName!.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<ProductViewModel>(items, count, page, pageSize);
    }

    private IQueryable<ProductViewModel> GetAllQuery()
    {
        return _context.Products
            .Include(p => p.Store)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                StoreId = p.StoreId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                StoreName = p.Store.Name,
                IsActive = p.IsActive
            });
    }

    public async Task<ProductViewModel?> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Store)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product == null) return null;

        return new ProductViewModel
        {
            Id = product.Id,
            StoreId = product.StoreId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            StoreName = product.Store.Name,
            IsActive = product.IsActive
        };
    }

    public async Task CreateAsync(ProductViewModel viewModel)
    {
        var product = new Product
        {
            StoreId = viewModel.StoreId,
            Name = viewModel.Name,
            Description = viewModel.Description,
            Price = viewModel.Price,
            Stock = viewModel.Stock,
            ImageUrl = viewModel.ImageUrl,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductViewModel viewModel)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == viewModel.Id && p.IsActive)
            ?? throw new KeyNotFoundException($"Product with Id {viewModel.Id} not found.");

        product.Name = viewModel.Name;
        product.Description = viewModel.Description;
        product.Price = viewModel.Price;
        product.Stock = viewModel.Stock;
        product.ImageUrl = viewModel.ImageUrl;

        if (viewModel.StoreId > 0)
            product.StoreId = viewModel.StoreId;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive)
            ?? throw new KeyNotFoundException($"Product with Id {id} not found.");

        product.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
