using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class CustomerService : IEntityService<Customer, CustomerViewModel>
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<CustomerViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "name" => query.Where(c => (c.FirstName + " " + c.LastName).ToLower().Contains(term)),
                "email" => query.Where(c => c.Email.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<CustomerViewModel>(items, count, page, pageSize);
    }

    private IQueryable<CustomerViewModel> GetAllQuery()
    {
        return _context.Customers
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new CustomerViewModel
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                UpdatedAt = c.UpdatedAt,
                IsActive = c.IsActive
            });
    }

    public async Task<CustomerViewModel?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (customer == null) return null;

        return new CustomerViewModel
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            UpdatedAt = customer.UpdatedAt,
            IsActive = customer.IsActive
        };
    }

    public async Task CreateAsync(CustomerViewModel viewModel)
    {
        var customer = new Customer
        {
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName,
            Email = viewModel.Email,
            Phone = viewModel.Phone,
            IsActive = true
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CustomerViewModel viewModel)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == viewModel.Id && c.IsActive)
            ?? throw new KeyNotFoundException($"Customer with Id {viewModel.Id} not found.");

        customer.FirstName = viewModel.FirstName;
        customer.LastName = viewModel.LastName;
        customer.Email = viewModel.Email;
        customer.Phone = viewModel.Phone;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive)
            ?? throw new KeyNotFoundException($"Customer with Id {id} not found.");

        customer.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
