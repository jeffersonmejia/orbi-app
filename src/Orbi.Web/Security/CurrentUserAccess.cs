using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;

namespace Orbi.Web.Security;

public class CurrentUserAccess
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccess(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;
    public bool IsAdmin => User.IsInRole(OrbiRoles.Admin);
    public bool IsStoreOwner => User.IsInRole(OrbiRoles.StoreOwner);
    public bool IsDeliveryDriver => User.IsInRole(OrbiRoles.DeliveryDriver);
    public bool IsCustomer => User.IsInRole(OrbiRoles.Customer);

    public IQueryable<Customer> ScopeCustomers(IQueryable<Customer> query)
    {
        if (IsAdmin) return query;
        if (IsCustomer && UserId is not null) return query.Where(c => c.UserId == UserId);
        return query.Where(_ => false);
    }

    public IQueryable<Address> ScopeAddresses(IQueryable<Address> query)
    {
        if (IsAdmin) return query;
        if (IsCustomer && UserId is not null) return query.Where(a => a.Customer.UserId == UserId);
        if (IsDeliveryDriver && UserId is not null)
        {
            return query.Where(a => a.Customer.Orders.Any(o =>
                o.AddressId == a.Id &&
                o.DeliveryDriver != null &&
                o.DeliveryDriver.UserId == UserId));
        }

        return query.Where(_ => false);
    }

    public IQueryable<Store> ScopeStores(IQueryable<Store> query)
    {
        if (IsAdmin || IsCustomer || IsDeliveryDriver) return query;
        if (IsStoreOwner && UserId is not null) return query.Where(s => s.UserId == UserId);
        return query.Where(_ => false);
    }

    public IQueryable<Product> ScopeProducts(IQueryable<Product> query)
    {
        if (IsAdmin || IsCustomer || IsDeliveryDriver) return query;
        if (IsStoreOwner && UserId is not null) return query.Where(p => p.Store.UserId == UserId);
        return query.Where(_ => false);
    }

    public IQueryable<Order> ScopeOrders(IQueryable<Order> query)
    {
        if (IsAdmin) return query;
        if (IsCustomer && UserId is not null) return query.Where(o => o.Customer.UserId == UserId);
        if (IsStoreOwner && UserId is not null) return query.Where(o => o.Store.UserId == UserId);
        if (IsDeliveryDriver && UserId is not null) return query.Where(o => o.DeliveryDriver != null && o.DeliveryDriver.UserId == UserId);
        return query.Where(_ => false);
    }

    public IQueryable<DeliveryDriver> ScopeDeliveryDrivers(IQueryable<DeliveryDriver> query)
    {
        if (IsAdmin || IsStoreOwner) return query;
        if (IsDeliveryDriver && UserId is not null) return query.Where(d => d.UserId == UserId);
        return query.Where(_ => false);
    }

    public IQueryable<Payment> ScopePayments(IQueryable<Payment> query)
    {
        if (IsAdmin) return query;
        if (IsCustomer && UserId is not null) return query.Where(p => p.Order.Customer.UserId == UserId);
        if (IsStoreOwner && UserId is not null) return query.Where(p => p.Order.Store.UserId == UserId);
        return query.Where(_ => false);
    }

    public IQueryable<Review> ScopeReviews(IQueryable<Review> query)
    {
        if (IsAdmin) return query;
        if (IsCustomer && UserId is not null) return query.Where(r => r.Customer.UserId == UserId);
        if (IsStoreOwner && UserId is not null) return query.Where(r => r.Store.UserId == UserId);
        return query.Where(_ => false);
    }

    public async Task<int> RequireOwnCustomerIdAsync(AppDbContext context)
    {
        if (IsAdmin) throw new InvalidOperationException("Admin callers must provide the target customer explicitly.");
        if (!IsCustomer || UserId is null) throw new UnauthorizedAccessException();

        var customerId = await context.Customers
            .Where(c => c.UserId == UserId)
            .Select(c => c.Id)
            .SingleOrDefaultAsync();

        return customerId == 0 ? throw new UnauthorizedAccessException() : customerId;
    }
}
