using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.Security;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class ReviewService : IEntityService<Review, ReviewViewModel>
{
    private readonly AppDbContext _context;
    private readonly CurrentUserAccess _access;

    public ReviewService(AppDbContext context, CurrentUserAccess access)
    {
        _context = context;
        _access = access;
    }

    public async Task<IEnumerable<ReviewViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<ReviewViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "customer" => query.Where(r => r.CustomerName!.ToLower().Contains(term)),
                "store" => query.Where(r => r.StoreName!.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<ReviewViewModel>(items, count, page, pageSize);
    }

    private IQueryable<ReviewViewModel> GetAllQuery()
    {
        return _access.ScopeReviews(_context.Reviews)
            .AsNoTracking()
            .Include(r => r.Customer)
            .Include(r => r.Store)
            .Where(r => r.IsActive)
            .Select(r => new ReviewViewModel
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                StoreId = r.StoreId,
                StoreName = r.Store.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                UpdatedAt = r.UpdatedAt,
                IsActive = r.IsActive
            });
    }

    public async Task<ReviewViewModel?> GetByIdAsync(int id)
    {
        var review = await _access.ScopeReviews(_context.Reviews)
            .AsNoTracking()
            .Include(r => r.Customer)
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

        if (review == null) return null;

        return new ReviewViewModel
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            CustomerName = review.Customer.FirstName + " " + review.Customer.LastName,
            StoreId = review.StoreId,
            StoreName = review.Store.Name,
            Rating = review.Rating,
            Comment = review.Comment,
            UpdatedAt = review.UpdatedAt,
            IsActive = review.IsActive
        };
    }

    public async Task CreateAsync(ReviewViewModel viewModel)
    {
        if (_access.IsCustomer)
        {
            viewModel.CustomerId = await _access.RequireOwnCustomerIdAsync(_context);
        }
        else if (!_access.IsAdmin)
        {
            throw new UnauthorizedAccessException();
        }

        var review = new Review
        {
            CustomerId = viewModel.CustomerId,
            StoreId = viewModel.StoreId,
            Rating = viewModel.Rating,
            Comment = viewModel.Comment,
            IsActive = true
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ReviewViewModel viewModel)
    {
        var review = await _access.ScopeReviews(_context.Reviews)
            .FirstOrDefaultAsync(r => r.Id == viewModel.Id && r.IsActive)
            ?? throw new KeyNotFoundException($"Review with Id {viewModel.Id} not found.");

        review.CustomerId = _access.IsCustomer
            ? await _access.RequireOwnCustomerIdAsync(_context)
            : viewModel.CustomerId;
        review.StoreId = viewModel.StoreId;
        review.Rating = viewModel.Rating;
        review.Comment = viewModel.Comment;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var review = await _access.ScopeReviews(_context.Reviews)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive)
            ?? throw new KeyNotFoundException($"Review with Id {id} not found.");

        review.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
