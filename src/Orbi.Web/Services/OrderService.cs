using Microsoft.EntityFrameworkCore;
using Orbi.Web.Data;
using Orbi.Web.Models;
using Orbi.Web.Security;
using Orbi.Web.ViewModels;

namespace Orbi.Web.Services;

public class OrderService : IEntityService<Order, OrderViewModel>
{
    private readonly AppDbContext _context;
    private readonly CurrentUserAccess _access;

    public OrderService(AppDbContext context, CurrentUserAccess access)
    {
        _context = context;
        _access = access;
    }

    public async Task<IEnumerable<OrderViewModel>> GetAllAsync()
    {
        return await GetAllQuery().ToListAsync();
    }

    public async Task<PaginatedList<OrderViewModel>> GetPaginatedAsync(
        string? searchField, string? searchTerm, int page, int pageSize)
    {
        var query = GetAllQuery();

        if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = searchField.ToLower() switch
            {
                "customer" => query.Where(o => o.CustomerName!.ToLower().Contains(term)),
                "store" => query.Where(o => o.StoreName!.ToLower().Contains(term)),
                "status" => query.Where(o => o.OrderStatusName!.ToLower().Contains(term)),
                _ => query
            };
        }

        var count = await query.CountAsync();
        var items = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<OrderViewModel>(items, count, page, pageSize);
    }

    private IQueryable<OrderViewModel> GetAllQuery()
    {
        return _access.ScopeOrders(_context.Orders)
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Store)
            .Include(o => o.DeliveryDriver)
            .Include(o => o.OrderStatus)
            .Include(o => o.Address)
            .Where(o => o.IsActive)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                StoreId = o.StoreId,
                DeliveryDriverId = o.DeliveryDriverId,
                OrderStatusId = o.OrderStatusId,
                AddressId = o.AddressId,
                TotalAmount = o.TotalAmount,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                StoreName = o.Store.Name,
                DriverName = o.DeliveryDriver != null ? o.DeliveryDriver.FirstName + " " + o.DeliveryDriver.LastName : null,
                OrderStatusName = o.OrderStatus.Name,
                AddressLine = $"{o.Address.Street}, {o.Address.City}",
                IsActive = o.IsActive,
                Items = new List<OrderItemViewModel>()
            });
    }

    public async Task<OrderViewModel?> GetByIdAsync(int id)
    {
        var order = await _access.ScopeOrders(_context.Orders)
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Store)
            .Include(o => o.DeliveryDriver)
            .Include(o => o.OrderStatus)
            .Include(o => o.Address)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.IsActive);

        if (order == null) return null;

        return new OrderViewModel
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            StoreId = order.StoreId,
            DeliveryDriverId = order.DeliveryDriverId,
            OrderStatusId = order.OrderStatusId,
            AddressId = order.AddressId,
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            DeliveryDate = order.DeliveryDate,
            CustomerName = order.Customer.FirstName + " " + order.Customer.LastName,
            StoreName = order.Store.Name,
            DriverName = order.DeliveryDriver != null ? order.DeliveryDriver.FirstName + " " + order.DeliveryDriver.LastName : null,
            OrderStatusName = order.OrderStatus.Name,
            AddressLine = $"{order.Address.Street}, {order.Address.City}",
            IsActive = order.IsActive,
            Items = order.OrderDetails.Select(od => new OrderItemViewModel
            {
                ProductId = od.ProductId,
                ProductName = od.Product.Name,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Subtotal = od.Subtotal
            }).ToList()
        };
    }

    public async Task CreateAsync(OrderViewModel viewModel)
    {
        if (_access.IsCustomer)
        {
            viewModel.CustomerId = await _access.RequireOwnCustomerIdAsync(_context);
            var ownsAddress = await _context.Addresses
                .AnyAsync(a => a.Id == viewModel.AddressId && a.CustomerId == viewModel.CustomerId);

            if (!ownsAddress) throw new UnauthorizedAccessException();
        }
        else if (!_access.IsAdmin)
        {
            throw new UnauthorizedAccessException();
        }

        var order = new Order
        {
            CustomerId = viewModel.CustomerId,
            StoreId = viewModel.StoreId,
            DeliveryDriverId = viewModel.DeliveryDriverId,
            OrderStatusId = viewModel.OrderStatusId,
            AddressId = viewModel.AddressId,
            TotalAmount = 0,
            OrderDate = DateTime.UtcNow,
            IsActive = true
        };

        decimal totalAmount = 0;

        foreach (var item in viewModel.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId && p.IsActive)
                ?? throw new KeyNotFoundException($"Product with Id {item.ProductId} not found.");

            var unitPrice = product.Price;
            var subtotal = unitPrice * item.Quantity;

            order.OrderDetails.Add(new OrderDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                Subtotal = subtotal,
                IsActive = true
            });

            totalAmount += subtotal;
        }

        order.TotalAmount = totalAmount;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrderViewModel viewModel)
    {
        var order = await _access.ScopeOrders(_context.Orders)
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == viewModel.Id && o.IsActive)
            ?? throw new KeyNotFoundException($"Order with Id {viewModel.Id} not found.");

        if (_access.IsDeliveryDriver)
        {
            order.OrderStatusId = viewModel.OrderStatusId;
            await _context.SaveChangesAsync();
            return;
        }

        if (_access.IsStoreOwner)
        {
            order.DeliveryDriverId = viewModel.DeliveryDriverId;
            order.OrderStatusId = viewModel.OrderStatusId;
        }
        else
        {
            order.CustomerId = viewModel.CustomerId;
            order.StoreId = viewModel.StoreId;
            order.DeliveryDriverId = viewModel.DeliveryDriverId;
            order.OrderStatusId = viewModel.OrderStatusId;
            order.AddressId = viewModel.AddressId;
        }

        if (_access.IsAdmin && viewModel.Items.Count > 0)
        {
            _context.OrderDetails.RemoveRange(order.OrderDetails);

            decimal totalAmount = 0;
            foreach (var item in viewModel.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId && p.IsActive)
                    ?? throw new KeyNotFoundException($"Product with Id {item.ProductId} not found.");

                var unitPrice = product.Price;
                var subtotal = unitPrice * item.Quantity;

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    Subtotal = subtotal,
                    IsActive = true
                });

                totalAmount += subtotal;
            }

            order.TotalAmount = totalAmount;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        if (!_access.IsAdmin)
            throw new UnauthorizedAccessException();

        var order = await _access.ScopeOrders(_context.Orders)
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == id && o.IsActive)
            ?? throw new KeyNotFoundException($"Order with Id {id} not found.");

        order.IsActive = false;

        foreach (var detail in order.OrderDetails)
        {
            detail.IsActive = false;
        }

        await _context.SaveChangesAsync();
    }
}
