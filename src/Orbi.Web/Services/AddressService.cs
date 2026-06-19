using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.Security;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class AddressService : IEntityService<Address, AddressViewModel>
{
    private readonly AppDbContext _context;
    private readonly CurrentUserAccess _access;

    public AddressService(AppDbContext context, CurrentUserAccess access)
    {
        _context = context;
        _access = access;
    }

    public async Task<IEnumerable<AddressViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<AddressViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "customer" => query.Where(a => a.CustomerName!.ToLower().Contains(term)),
                "city" => query.Where(a => a.City.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<AddressViewModel>(items, count, page, pageSize);
    }

    private IQueryable<AddressViewModel> GetAllQuery()
    {
        return _access.ScopeAddresses(_context.Addresses)
            .AsNoTracking()
            .Include(a => a.Customer)
            .Where(a => a.IsActive)
            .OrderBy(a => a.City)
            .ThenBy(a => a.Street)
            .Select(a => new AddressViewModel
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                CustomerName = a.Customer.FirstName + " " + a.Customer.LastName,
                Street = a.Street,
                City = a.City,
                State = a.State,
                ZipCode = a.ZipCode,
                Country = a.Country,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                UpdatedAt = a.UpdatedAt,
                IsActive = a.IsActive
            });
    }

    public async Task<AddressViewModel?> GetByIdAsync(int id)
    {
        var address = await _access.ScopeAddresses(_context.Addresses)
            .AsNoTracking()
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

        if (address == null) return null;

        return new AddressViewModel
        {
            Id = address.Id,
            CustomerId = address.CustomerId,
            CustomerName = address.Customer.FirstName + " " + address.Customer.LastName,
            Street = address.Street,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            UpdatedAt = address.UpdatedAt,
            IsActive = address.IsActive
        };
    }

    public async Task CreateAsync(AddressViewModel viewModel)
    {
        if (_access.IsCustomer)
        {
            viewModel.CustomerId = await _access.RequireOwnCustomerIdAsync(_context);
        }
        else if (!_access.IsAdmin)
        {
            throw new UnauthorizedAccessException();
        }

        var address = new Address
        {
            CustomerId = viewModel.CustomerId,
            Street = viewModel.Street,
            City = viewModel.City,
            State = viewModel.State,
            ZipCode = viewModel.ZipCode,
            Country = viewModel.Country,
            Latitude = viewModel.Latitude,
            Longitude = viewModel.Longitude,
            IsActive = true
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AddressViewModel viewModel)
    {
        var address = await _access.ScopeAddresses(_context.Addresses)
            .FirstOrDefaultAsync(a => a.Id == viewModel.Id && a.IsActive)
            ?? throw new KeyNotFoundException($"Address with Id {viewModel.Id} not found.");

        address.CustomerId = _access.IsCustomer
            ? await _access.RequireOwnCustomerIdAsync(_context)
            : viewModel.CustomerId;
        address.Street = viewModel.Street;
        address.City = viewModel.City;
        address.State = viewModel.State;
        address.ZipCode = viewModel.ZipCode;
        address.Country = viewModel.Country;
        address.Latitude = viewModel.Latitude;
        address.Longitude = viewModel.Longitude;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var address = await _access.ScopeAddresses(_context.Addresses)
            .FirstOrDefaultAsync(a => a.Id == id && a.IsActive)
            ?? throw new KeyNotFoundException($"Address with Id {id} not found.");

        address.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
