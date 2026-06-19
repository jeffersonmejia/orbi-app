using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Models;

namespace Orbi.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser>? userManager = null, RoleManager<IdentityRole>? roleManager = null)
    {
        if (roleManager != null)
        {
            var roles = new[] { "Customer", "DeliveryDriver", "StoreOwner", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (userManager != null)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@orbi.com");
            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@orbi.com",
                    Email = "admin@orbi.com",
                    FirstName = "Admin",
                    LastName = "Orbi",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin1234!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
            else if (string.IsNullOrWhiteSpace(adminUser.FirstName))
            {
                adminUser.FirstName = "Admin";
                adminUser.LastName = "Orbi";
                await userManager.UpdateAsync(adminUser);
            }
        }

        if (await context.StoreCategories.AnyAsync()) return;

        // OrderStatus
        var statuses = new List<OrderStatus>
        {
            new() { Name = "Pending", Description = "Order has been placed but not yet confirmed." },
            new() { Name = "Confirmed", Description = "Order has been confirmed by the store." },
            new() { Name = "Preparing", Description = "The store is preparing the order." },
            new() { Name = "In Transit", Description = "The order is on its way to the customer." },
            new() { Name = "Delivered", Description = "The order has been delivered to the customer." },
            new() { Name = "Cancelled", Description = "The order has been cancelled." }
        };
        context.OrderStatuses.AddRange(statuses);

        // PaymentMethod
        var paymentMethods = new List<PaymentMethod>
        {
            new() { Name = "Credit Card", Description = "Payment via credit card." },
            new() { Name = "Debit Card", Description = "Payment via debit card." },
            new() { Name = "Cash", Description = "Payment in cash upon delivery." },
            new() { Name = "Digital Wallet", Description = "Payment via digital wallet." },
            new() { Name = "Bank Transfer", Description = "Payment via bank transfer." }
        };
        context.PaymentMethods.AddRange(paymentMethods);

        // StoreCategory
        var categories = new List<StoreCategory>
        {
            new() { Name = "Restaurant", Description = "Restaurants offering prepared meals and beverages." },
            new() { Name = "Pharmacy", Description = "Pharmacies offering medicines and health products." },
            new() { Name = "Supermarket", Description = "Supermarkets offering groceries and household items." },
            new() { Name = "Bakery", Description = "Bakeries offering fresh bread and pastries." },
            new() { Name = "Coffee Shop", Description = "Coffee shops offering beverages and light snacks." }
        };
        context.StoreCategories.AddRange(categories);
        await context.SaveChangesAsync();

        // DeliveryDriver
        var drivers = new List<DeliveryDriver>
        {
            new() { FirstName = "Carlos", LastName = "Mendoza", Email = "carlos.mendoza@orbi.com", Phone = "3001112233", CurrentLatitude = 4.710989, CurrentLongitude = -74.072092, LastLocationUpdate = DateTime.UtcNow, IsAvailable = true },
            new() { FirstName = "Ana", LastName = "Lopez", Email = "ana.lopez@orbi.com", Phone = "3002223344", CurrentLatitude = 4.711500, CurrentLongitude = -74.073000, LastLocationUpdate = DateTime.UtcNow, IsAvailable = true },
            new() { FirstName = "Pedro", LastName = "Ramirez", Email = "pedro.ramirez@orbi.com", Phone = "3003334455", CurrentLatitude = 4.712000, CurrentLongitude = -74.071500, LastLocationUpdate = DateTime.UtcNow, IsAvailable = false },
            new() { FirstName = "Maria", LastName = "Torres", Email = "maria.torres@orbi.com", Phone = "3004445566", CurrentLatitude = 4.710500, CurrentLongitude = -74.074000, LastLocationUpdate = DateTime.UtcNow, IsAvailable = true },
            new() { FirstName = "Jose", LastName = "Garcia", Email = "jose.garcia@orbi.com", Phone = "3005556677", CurrentLatitude = 4.713000, CurrentLongitude = -74.072500, LastLocationUpdate = DateTime.UtcNow, IsAvailable = true }
        };
        context.DeliveryDrivers.AddRange(drivers);

        // Customer (UserId is empty string for seed data; real customers get it via registration)
        var customers = new List<Customer>
        {
            new() { FirstName = "Laura", LastName = "Gomez", Email = "laura.gomez@email.com", Phone = "3101112233" },
            new() { FirstName = "Andres", LastName = "Fernandez", Email = "andres.fernandez@email.com", Phone = "3102223344" },
            new() { FirstName = "Sofia", LastName = "Martinez", Email = "sofia.martinez@email.com", Phone = "3103334455" },
            new() { FirstName = "Diego", LastName = "Rodriguez", Email = "diego.rodriguez@email.com", Phone = "3104445566" },
            new() { FirstName = "Valentina", LastName = "Lopez", Email = "valentina.lopez@email.com", Phone = "3105556677" }
        };
        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();

        // Address
        var addresses = new List<Address>
        {
            new() { CustomerId = customers[0].Id, Street = "Carrera 7 # 45-12", City = "Bogota", State = "Cundinamarca", ZipCode = "110111", Country = "Colombia", Latitude = 4.710989, Longitude = -74.072092 },
            new() { CustomerId = customers[0].Id, Street = "Calle 100 # 15-30", City = "Bogota", State = "Cundinamarca", ZipCode = "110121", Country = "Colombia", Latitude = 4.705000, Longitude = -74.070000 },
            new() { CustomerId = customers[1].Id, Street = "Av. El Dorado # 68-90", City = "Bogota", State = "Cundinamarca", ZipCode = "110131", Country = "Colombia", Latitude = 4.680000, Longitude = -74.060000 },
            new() { CustomerId = customers[2].Id, Street = "Calle 26 # 50-10", City = "Bogota", State = "Cundinamarca", ZipCode = "110141", Country = "Colombia", Latitude = 4.620000, Longitude = -74.080000 },
            new() { CustomerId = customers[3].Id, Street = "Carrera 15 # 85-20", City = "Bogota", State = "Cundinamarca", ZipCode = "110151", Country = "Colombia", Latitude = 4.720000, Longitude = -74.065000 }
        };
        context.Addresses.AddRange(addresses);
        await context.SaveChangesAsync();

        // Store
        var stores = new List<Store>
        {
            new() { CategoryId = categories[0].Id, Name = "La Buena Mesa", Description = "Traditional Colombian cuisine.", Phone = "6011112233", Email = "contacto@labuenamesa.com", Address = "Calle 72 # 10-20", Latitude = 4.660000, Longitude = -74.058000 },
            new() { CategoryId = categories[1].Id, Name = "Farmacia Salud Total", Description = "Full-service pharmacy and wellness store.", Phone = "6012223344", Email = "info@saludtotal.com", Address = "Av. Caracas # 32-15", Latitude = 4.630000, Longitude = -74.075000 },
            new() { CategoryId = categories[2].Id, Name = "Supermercado El Ahorro", Description = "Low prices on all your grocery needs.", Phone = "6013334455", Email = "servicio@elahorro.com", Address = "Calle 80 # 20-40", Latitude = 4.690000, Longitude = -74.070000 },
            new() { CategoryId = categories[3].Id, Name = "Panaderia Delicias", Description = "Freshly baked bread and pastries daily.", Phone = "6014445566", Email = "ventas@delicias.com", Address = "Carrera 11 # 55-30", Latitude = 4.675000, Longitude = -74.062000 },
            new() { CategoryId = categories[4].Id, Name = "Cafe Aroma", Description = "Specialty coffee and artisan beverages.", Phone = "6015556677", Email = "hola@cafearoma.com", Address = "Calle 93 # 12-50", Latitude = 4.710000, Longitude = -74.055000 }
        };
        context.Stores.AddRange(stores);
        await context.SaveChangesAsync();

        // Product
        var products = new List<Product>
        {
            new() { StoreId = stores[0].Id, Name = "Bandeja Paisa", Description = "Traditional Colombian platter with rice, beans, plantain, avocado, and meat.", Price = 28000, Stock = 50, ImageUrl = "/images/bandeja-paisa.jpg" },
            new() { StoreId = stores[0].Id, Name = "Ajiaco Santafereño", Description = "Traditional Bogota chicken and potato soup.", Price = 22000, Stock = 40, ImageUrl = "/images/ajiaco.jpg" },
            new() { StoreId = stores[1].Id, Name = "Ibuprofeno 400mg", Description = "Anti-inflammatory pain reliever, 30 tablets.", Price = 12500, Stock = 200, ImageUrl = "/images/ibuprofeno.jpg" },
            new() { StoreId = stores[1].Id, Name = "Vitamina C 1000mg", Description = "Immune support supplement, 60 tablets.", Price = 18500, Stock = 150, ImageUrl = "/images/vitamina-c.jpg" },
            new() { StoreId = stores[2].Id, Name = "Arroz Diana 1kg", Description = "Premium long-grain white rice.", Price = 4500, Stock = 500, ImageUrl = "/images/arroz-diana.jpg" },
            new() { StoreId = stores[2].Id, Name = "Aceite Vegetal 900ml", Description = "Pure vegetable cooking oil.", Price = 8500, Stock = 300, ImageUrl = "/images/aceite.jpg" },
            new() { StoreId = stores[3].Id, Name = "Pan Frances", Description = "Fresh French bread loaf.", Price = 2500, Stock = 100, ImageUrl = "/images/pan-frances.jpg" },
            new() { StoreId = stores[3].Id, Name = "Pastel de Queso", Description = "Cream cheese pastry.", Price = 4500, Stock = 60, ImageUrl = "/images/pastel-queso.jpg" },
            new() { StoreId = stores[4].Id, Name = "Cafe Latte", Description = "Espresso with steamed milk.", Price = 7500, Stock = 80, ImageUrl = "/images/cafe-latte.jpg" },
            new() { StoreId = stores[4].Id, Name = "Capuchino", Description = "Espresso with frothed milk and cinnamon.", Price = 8500, Stock = 70, ImageUrl = "/images/capuchino.jpg" }
        };
        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Order
        var orders = new List<Order>
        {
            new() { CustomerId = customers[0].Id, StoreId = stores[0].Id, DeliveryDriverId = drivers[0].Id, OrderStatusId = statuses[4].Id, AddressId = addresses[0].Id, TotalAmount = 50000, OrderDate = DateTime.UtcNow.AddDays(-2), DeliveryDate = DateTime.UtcNow.AddDays(-2).AddHours(1) },
            new() { CustomerId = customers[1].Id, StoreId = stores[1].Id, DeliveryDriverId = drivers[1].Id, OrderStatusId = statuses[4].Id, AddressId = addresses[2].Id, TotalAmount = 12500, OrderDate = DateTime.UtcNow.AddDays(-1), DeliveryDate = DateTime.UtcNow.AddDays(-1).AddMinutes(45) },
            new() { CustomerId = customers[2].Id, StoreId = stores[2].Id, DeliveryDriverId = drivers[3].Id, OrderStatusId = statuses[3].Id, AddressId = addresses[3].Id, TotalAmount = 13000, OrderDate = DateTime.UtcNow.AddHours(-3) },
            new() { CustomerId = customers[0].Id, StoreId = stores[3].Id, DeliveryDriverId = null, OrderStatusId = statuses[0].Id, AddressId = addresses[1].Id, TotalAmount = 7000, OrderDate = DateTime.UtcNow.AddMinutes(-30) },
            new() { CustomerId = customers[3].Id, StoreId = stores[4].Id, DeliveryDriverId = drivers[4].Id, OrderStatusId = statuses[2].Id, AddressId = addresses[4].Id, TotalAmount = 16000, OrderDate = DateTime.UtcNow.AddHours(-1) }
        };
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();

        // OrderDetail
        var orderDetails = new List<OrderDetail>
        {
            new() { OrderId = orders[0].Id, ProductId = products[0].Id, Quantity = 1, UnitPrice = 28000, Subtotal = 28000 },
            new() { OrderId = orders[0].Id, ProductId = products[1].Id, Quantity = 1, UnitPrice = 22000, Subtotal = 22000 },
            new() { OrderId = orders[1].Id, ProductId = products[2].Id, Quantity = 1, UnitPrice = 12500, Subtotal = 12500 },
            new() { OrderId = orders[2].Id, ProductId = products[4].Id, Quantity = 2, UnitPrice = 4500, Subtotal = 9000 },
            new() { OrderId = orders[2].Id, ProductId = products[5].Id, Quantity = 1, UnitPrice = 4000, Subtotal = 4000 },
            new() { OrderId = orders[3].Id, ProductId = products[6].Id, Quantity = 2, UnitPrice = 2500, Subtotal = 5000 },
            new() { OrderId = orders[3].Id, ProductId = products[7].Id, Quantity = 1, UnitPrice = 2000, Subtotal = 2000 },
            new() { OrderId = orders[4].Id, ProductId = products[8].Id, Quantity = 2, UnitPrice = 7500, Subtotal = 15000 },
            new() { OrderId = orders[4].Id, ProductId = products[9].Id, Quantity = 1, UnitPrice = 1000, Subtotal = 1000 }
        };
        context.OrderDetails.AddRange(orderDetails);
        await context.SaveChangesAsync();

        // Payment
        var payments = new List<Payment>
        {
            new() { OrderId = orders[0].Id, PaymentMethodId = paymentMethods[0].Id, Amount = 50000, PaymentDate = DateTime.UtcNow.AddDays(-2), TransactionId = "TXN001", Status = "Completed" },
            new() { OrderId = orders[1].Id, PaymentMethodId = paymentMethods[2].Id, Amount = 12500, PaymentDate = DateTime.UtcNow.AddDays(-1), TransactionId = "TXN002", Status = "Completed" },
            new() { OrderId = orders[2].Id, PaymentMethodId = paymentMethods[1].Id, Amount = 13000, PaymentDate = DateTime.UtcNow.AddHours(-3), TransactionId = "TXN003", Status = "Completed" },
            new() { OrderId = orders[3].Id, PaymentMethodId = paymentMethods[2].Id, Amount = 7000, PaymentDate = DateTime.UtcNow.AddMinutes(-30), TransactionId = null, Status = "Pending" },
            new() { OrderId = orders[4].Id, PaymentMethodId = paymentMethods[3].Id, Amount = 16000, PaymentDate = DateTime.UtcNow.AddHours(-1), TransactionId = "TXN004", Status = "Completed" }
        };
        context.Payments.AddRange(payments);
        await context.SaveChangesAsync();

        // Review
        var reviews = new List<Review>
        {
            new() { CustomerId = customers[0].Id, StoreId = stores[0].Id, Rating = 5, Comment = "Excellent food and fast delivery!" },
            new() { CustomerId = customers[1].Id, StoreId = stores[1].Id, Rating = 4, Comment = "Good service, everything I needed was available." },
            new() { CustomerId = customers[2].Id, StoreId = stores[2].Id, Rating = 3, Comment = "Average prices, some items were out of stock." },
            new() { CustomerId = customers[3].Id, StoreId = stores[4].Id, Rating = 5, Comment = "Best coffee in town, highly recommended." },
            new() { CustomerId = customers[0].Id, StoreId = stores[3].Id, Rating = 4, Comment = "Fresh bread and friendly staff." }
        };
        context.Reviews.AddRange(reviews);
        await context.SaveChangesAsync();
    }
}
