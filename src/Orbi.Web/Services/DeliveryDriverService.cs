using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.Security;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class DeliveryDriverService : IEntityService<DeliveryDriver, DeliveryDriverViewModel>
{
    private readonly AppDbContext _context;
    private readonly CurrentUserAccess _access;

    public DeliveryDriverService(AppDbContext context, CurrentUserAccess access)
    {
        _context = context;
        _access = access;
    }

    public async Task<IEnumerable<DeliveryDriverViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<DeliveryDriverViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "name" => query.Where(d => (d.FirstName + " " + d.LastName).ToLower().Contains(term)),
                "email" => query.Where(d => d.Email.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<DeliveryDriverViewModel>(items, count, page, pageSize);
    }

    private IQueryable<DeliveryDriverViewModel> GetAllQuery()
    {
        return _access.ScopeDeliveryDrivers(_context.DeliveryDrivers)
            .AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .Select(d => new DeliveryDriverViewModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email,
                Phone = d.Phone,
                CurrentLatitude = d.CurrentLatitude,
                CurrentLongitude = d.CurrentLongitude,
                LastLocationUpdate = d.LastLocationUpdate,
                IsAvailable = d.IsAvailable,
                UpdatedAt = d.UpdatedAt,
                IsActive = d.IsActive
            });
    }

    public async Task<DeliveryDriverViewModel?> GetByIdAsync(int id)
    {
        var driver = await _access.ScopeDeliveryDrivers(_context.DeliveryDrivers)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        if (driver == null) return null;

        return new DeliveryDriverViewModel
        {
            Id = driver.Id,
            FirstName = driver.FirstName,
            LastName = driver.LastName,
            Email = driver.Email,
            Phone = driver.Phone,
            CurrentLatitude = driver.CurrentLatitude,
            CurrentLongitude = driver.CurrentLongitude,
            LastLocationUpdate = driver.LastLocationUpdate,
            IsAvailable = driver.IsAvailable,
            UpdatedAt = driver.UpdatedAt,
            IsActive = driver.IsActive
        };
    }

    public async Task CreateAsync(DeliveryDriverViewModel viewModel)
    {
        if (!_access.IsAdmin)
            throw new UnauthorizedAccessException();

        var driver = new DeliveryDriver
        {
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName,
            Email = viewModel.Email,
            Phone = viewModel.Phone,
            CurrentLatitude = viewModel.CurrentLatitude,
            CurrentLongitude = viewModel.CurrentLongitude,
            LastLocationUpdate = DateTime.UtcNow,
            IsAvailable = viewModel.IsAvailable,
            IsActive = true
        };

        _context.DeliveryDrivers.Add(driver);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DeliveryDriverViewModel viewModel)
    {
        if (!_access.IsAdmin && !_access.IsDeliveryDriver)
            throw new UnauthorizedAccessException();

        var driver = await _access.ScopeDeliveryDrivers(_context.DeliveryDrivers)
            .FirstOrDefaultAsync(d => d.Id == viewModel.Id && d.IsActive)
            ?? throw new KeyNotFoundException($"DeliveryDriver with Id {viewModel.Id} not found.");

        driver.FirstName = viewModel.FirstName;
        driver.LastName = viewModel.LastName;
        driver.Email = viewModel.Email;
        driver.Phone = viewModel.Phone;
        driver.CurrentLatitude = viewModel.CurrentLatitude;
        driver.CurrentLongitude = viewModel.CurrentLongitude;
        driver.IsAvailable = viewModel.IsAvailable;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        if (!_access.IsAdmin)
            throw new UnauthorizedAccessException();

        var driver = await _access.ScopeDeliveryDrivers(_context.DeliveryDrivers)
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive)
            ?? throw new KeyNotFoundException($"DeliveryDriver with Id {id} not found.");

        driver.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
