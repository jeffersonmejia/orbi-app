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
            new() { Name = "Tarjeta de credito", Description = "Pago con tarjeta de credito." },
            new() { Name = "Tarjeta de debito", Description = "Pago con tarjeta de debito." },
            new() { Name = "Efectivo", Description = "Pago en efectivo al recibir el pedido." },
            new() { Name = "Billetera digital", Description = "Pago con billetera movil." },
            new() { Name = "Transferencia bancaria", Description = "Pago mediante transferencia bancaria." }
        };
        context.PaymentMethods.AddRange(paymentMethods);

        // StoreCategory
        var categories = new List<StoreCategory>
        {
            new() { Name = "Restaurante", Description = "Comidas preparadas, almuerzos, meriendas y bebidas." },
            new() { Name = "Farmacia", Description = "Medicinas de venta libre, cuidado personal y bienestar." },
            new() { Name = "Supermercado", Description = "Viveres, limpieza, bebidas y productos para el hogar." },
            new() { Name = "Panaderia", Description = "Pan, postres, bocaditos y productos horneados." },
            new() { Name = "Cafeteria", Description = "Cafe, bebidas frias, desayunos y snacks." }
        };
        context.StoreCategories.AddRange(categories);
        await context.SaveChangesAsync();

        // DeliveryDriver
        var drivers = new List<DeliveryDriver>
        {
            new() { FirstName = "Kevin", LastName = "Villacis", Email = "kevin.villacis@repartosorbi.com", Phone = "0960011122", CurrentLatitude = -2.170989, CurrentLongitude = -79.922092, LastLocationUpdate = DateTime.UtcNow, IsAvailable = true },
            new() { FirstName = "Bryan", LastName = "Macias", Email = "bryan.macias@repartosorbi.com", Phone = "0960022233", CurrentLatitude = -2.171500, CurrentLongitude = -79.913000, LastLocationUpdate = DateTime.UtcNow, IsAvailable = true },
            new() { FirstName = "Jonathan", LastName = "Mero", Email = "jonathan.mero@repartosorbi.com", Phone = "0960033344", CurrentLatitude = -2.172000, CurrentLongitude = -79.911500, LastLocationUpdate = DateTime.UtcNow, IsAvailable = false },
            new() { FirstName = "Steven", LastName = "Cantos", Email = "steven.cantos@repartosorbi.com", Phone = "0960044455", CurrentLatitude = -2.180500, CurrentLongitude = -79.914000, LastLocationUpdate = DateTime.UtcNow, IsAvailable = true },
            new() { FirstName = "Cristian", LastName = "Cevallos", Email = "cristian.cevallos@repartosorbi.com", Phone = "0960055566", CurrentLatitude = -2.183000, CurrentLongitude = -79.912500, LastLocationUpdate = DateTime.UtcNow, IsAvailable = true }
        };
        context.DeliveryDrivers.AddRange(drivers);

        // Customer (UserId is empty string for seed data; real customers get it via registration)
        var customers = new List<Customer>
        {
            new() { FirstName = "Ana", LastName = "Castro Torres", Email = "ana.castro@gmail.com", Phone = "0970011122" },
            new() { FirstName = "Carlos", LastName = "Reyes Mendoza", Email = "carlos.reyes@hotmail.com", Phone = "0970022233" },
            new() { FirstName = "Maria", LastName = "Vera Bravo", Email = "maria.vera@outlook.com", Phone = "0970033344" },
            new() { FirstName = "Jose", LastName = "Paredes Cedeno", Email = "jose.paredes@yahoo.com", Phone = "0970044455" },
            new() { FirstName = "Luis", LastName = "Leon Santos", Email = "luis.leon@icloud.com", Phone = "0970055566" }
        };
        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();

        // Address
        var addresses = new List<Address>
        {
            new() { CustomerId = customers[0].Id, Street = "Av. Francisco de Orellana, Alborada, mz. 14, villa 8", City = "Guayaquil", State = "Guayas", ZipCode = "090112", Country = "Ecuador", Latitude = -2.170989, Longitude = -79.922092 },
            new() { CustomerId = customers[0].Id, Street = "Av. Las Monjas, Urdesa, mz. 5, villa 12", City = "Guayaquil", State = "Guayas", ZipCode = "090113", Country = "Ecuador", Latitude = -2.175000, Longitude = -79.910000 },
            new() { CustomerId = customers[1].Id, Street = "Av. Juan Tanca Marengo, Garzota, mz. 21, villa 3", City = "Guayaquil", State = "Guayas", ZipCode = "090114", Country = "Ecuador", Latitude = -2.160000, Longitude = -79.900000 },
            new() { CustomerId = customers[2].Id, Street = "Av. Victor Emilio Estrada, Kennedy, mz. 9, villa 18", City = "Guayaquil", State = "Guayas", ZipCode = "090115", Country = "Ecuador", Latitude = -2.185000, Longitude = -79.905000 },
            new() { CustomerId = customers[3].Id, Street = "Av. Isidro Ayora, Sauces, mz. 32, villa 6", City = "Guayaquil", State = "Guayas", ZipCode = "090116", Country = "Ecuador", Latitude = -2.155000, Longitude = -79.895000 }
        };
        context.Addresses.AddRange(addresses);
        await context.SaveChangesAsync();

        // Store
        var stores = new List<Store>
        {
            new() { CategoryId = categories[0].Id, Name = "Sabor Costeno Alborada", Description = "Comidas preparadas para almuerzo, merienda y pedidos familiares.", Phone = "042001122", Email = "contacto@saborcosteno.ec", Address = "Av. Francisco de Orellana, Alborada, local 12", Latitude = -2.170000, Longitude = -79.920000 },
            new() { CategoryId = categories[1].Id, Name = "Botica Salud Garzota", Description = "Medicinas de venta libre, vitaminas y cuidado personal.", Phone = "042002233", Email = "info@boticasalud.ec", Address = "Av. de las Americas, Garzota, local 7", Latitude = -2.158000, Longitude = -79.892000 },
            new() { CategoryId = categories[2].Id, Name = "Market Express Urdesa", Description = "Viveres, bebidas, limpieza y productos basicos para el hogar.", Phone = "042003344", Email = "servicio@marketexpress.ec", Address = "Av. Las Monjas, Urdesa, local 4", Latitude = -2.180000, Longitude = -79.915000 },
            new() { CategoryId = categories[3].Id, Name = "Pan Caliente Kennedy", Description = "Pan fresco, dulces, tortas pequenas y bocaditos.", Phone = "042004455", Email = "ventas@pancaliente.ec", Address = "Av. Victor Emilio Estrada, Kennedy, local 9", Latitude = -2.182000, Longitude = -79.904000 },
            new() { CategoryId = categories[4].Id, Name = "Cafe Puerto Centro", Description = "Cafe, jugos, desayunos y snacks para llevar.", Phone = "042005566", Email = "hola@cafepuerto.ec", Address = "Calle 9 de Octubre, Centro, local 21", Latitude = -2.190000, Longitude = -79.885000 }
        };
        context.Stores.AddRange(stores);
        await context.SaveChangesAsync();

        // Product
        var products = new List<Product>
        {
            new() { StoreId = stores[0].Id, Name = "Almuerzo ejecutivo", Description = "Plato del dia con sopa, seco y bebida.", Price = 3.50m, Stock = 50, ImageUrl = "/images/products/product-01.jpg" },
            new() { StoreId = stores[0].Id, Name = "Encebollado", Description = "Sopa de pescado con yuca y curtido.", Price = 3.25m, Stock = 40, ImageUrl = "/images/products/product-02.jpg" },
            new() { StoreId = stores[1].Id, Name = "Ibuprofeno 400mg", Description = "Antiinflamatorio de venta libre.", Price = 1.60m, Stock = 200, ImageUrl = "/images/products/product-03.jpg" },
            new() { StoreId = stores[1].Id, Name = "Vitamina C", Description = "Suplemento de vitamina C.", Price = 2.25m, Stock = 150, ImageUrl = "/images/products/product-04.jpg" },
            new() { StoreId = stores[2].Id, Name = "Arroz 1 kg", Description = "Arroz blanco para consumo diario.", Price = 1.15m, Stock = 500, ImageUrl = "/images/products/product-05.jpg" },
            new() { StoreId = stores[2].Id, Name = "Aceite 1 litro", Description = "Aceite vegetal para cocina.", Price = 2.10m, Stock = 300, ImageUrl = "/images/products/product-06.jpg" },
            new() { StoreId = stores[3].Id, Name = "Pan enrollado", Description = "Pan fresco del dia.", Price = 0.35m, Stock = 100, ImageUrl = "/images/products/product-07.jpg" },
            new() { StoreId = stores[3].Id, Name = "Torta porcion", Description = "Porcion individual de torta.", Price = 1.25m, Stock = 60, ImageUrl = "/images/products/product-08.jpg" },
            new() { StoreId = stores[4].Id, Name = "Cafe americano", Description = "Cafe negro caliente.", Price = 1.50m, Stock = 80, ImageUrl = "/images/products/product-09.jpg" },
            new() { StoreId = stores[4].Id, Name = "Capuchino", Description = "Cafe con leche espumada.", Price = 2.00m, Stock = 70, ImageUrl = "/images/products/product-10.jpg" }
        };
        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Order
        var orders = new List<Order>
        {
            new() { CustomerId = customers[0].Id, StoreId = stores[0].Id, DeliveryDriverId = drivers[0].Id, OrderStatusId = statuses[4].Id, AddressId = addresses[0].Id, TotalAmount = 6.75m, OrderDate = DateTime.UtcNow.AddDays(-2), DeliveryDate = DateTime.UtcNow.AddDays(-2).AddHours(1) },
            new() { CustomerId = customers[1].Id, StoreId = stores[1].Id, DeliveryDriverId = drivers[1].Id, OrderStatusId = statuses[4].Id, AddressId = addresses[2].Id, TotalAmount = 1.60m, OrderDate = DateTime.UtcNow.AddDays(-1), DeliveryDate = DateTime.UtcNow.AddDays(-1).AddMinutes(45) },
            new() { CustomerId = customers[2].Id, StoreId = stores[2].Id, DeliveryDriverId = drivers[3].Id, OrderStatusId = statuses[3].Id, AddressId = addresses[3].Id, TotalAmount = 4.40m, OrderDate = DateTime.UtcNow.AddHours(-3) },
            new() { CustomerId = customers[0].Id, StoreId = stores[3].Id, DeliveryDriverId = null, OrderStatusId = statuses[0].Id, AddressId = addresses[1].Id, TotalAmount = 1.95m, OrderDate = DateTime.UtcNow.AddMinutes(-30) },
            new() { CustomerId = customers[3].Id, StoreId = stores[4].Id, DeliveryDriverId = drivers[4].Id, OrderStatusId = statuses[2].Id, AddressId = addresses[4].Id, TotalAmount = 5.00m, OrderDate = DateTime.UtcNow.AddHours(-1) }
        };
        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();

        // OrderDetail
        var orderDetails = new List<OrderDetail>
        {
            new() { OrderId = orders[0].Id, ProductId = products[0].Id, Quantity = 1, UnitPrice = 3.50m, Subtotal = 3.50m },
            new() { OrderId = orders[0].Id, ProductId = products[1].Id, Quantity = 1, UnitPrice = 3.25m, Subtotal = 3.25m },
            new() { OrderId = orders[1].Id, ProductId = products[2].Id, Quantity = 1, UnitPrice = 1.60m, Subtotal = 1.60m },
            new() { OrderId = orders[2].Id, ProductId = products[4].Id, Quantity = 2, UnitPrice = 1.15m, Subtotal = 2.30m },
            new() { OrderId = orders[2].Id, ProductId = products[5].Id, Quantity = 1, UnitPrice = 2.10m, Subtotal = 2.10m },
            new() { OrderId = orders[3].Id, ProductId = products[6].Id, Quantity = 2, UnitPrice = 0.35m, Subtotal = 0.70m },
            new() { OrderId = orders[3].Id, ProductId = products[7].Id, Quantity = 1, UnitPrice = 1.25m, Subtotal = 1.25m },
            new() { OrderId = orders[4].Id, ProductId = products[8].Id, Quantity = 2, UnitPrice = 1.50m, Subtotal = 3.00m },
            new() { OrderId = orders[4].Id, ProductId = products[9].Id, Quantity = 1, UnitPrice = 2.00m, Subtotal = 2.00m }
        };
        context.OrderDetails.AddRange(orderDetails);
        await context.SaveChangesAsync();

        // Payment
        var payments = new List<Payment>
        {
            new() { OrderId = orders[0].Id, PaymentMethodId = paymentMethods[0].Id, Amount = 6.75m, PaymentDate = DateTime.UtcNow.AddDays(-2), TransactionId = "ORB-DEMO-001", Status = "Completed" },
            new() { OrderId = orders[1].Id, PaymentMethodId = paymentMethods[2].Id, Amount = 1.60m, PaymentDate = DateTime.UtcNow.AddDays(-1), TransactionId = "ORB-DEMO-002", Status = "Completed" },
            new() { OrderId = orders[2].Id, PaymentMethodId = paymentMethods[1].Id, Amount = 4.40m, PaymentDate = DateTime.UtcNow.AddHours(-3), TransactionId = "ORB-DEMO-003", Status = "Completed" },
            new() { OrderId = orders[3].Id, PaymentMethodId = paymentMethods[2].Id, Amount = 1.95m, PaymentDate = DateTime.UtcNow.AddMinutes(-30), TransactionId = null, Status = "Pending" },
            new() { OrderId = orders[4].Id, PaymentMethodId = paymentMethods[3].Id, Amount = 5.00m, PaymentDate = DateTime.UtcNow.AddHours(-1), TransactionId = "ORB-DEMO-004", Status = "Completed" }
        };
        context.Payments.AddRange(payments);
        await context.SaveChangesAsync();

        // Review
        var reviews = new List<Review>
        {
            new() { CustomerId = customers[0].Id, StoreId = stores[0].Id, Rating = 5, Comment = "El pedido llego completo y a tiempo." },
            new() { CustomerId = customers[1].Id, StoreId = stores[1].Id, Rating = 4, Comment = "Buena atencion y productos en buen estado." },
            new() { CustomerId = customers[2].Id, StoreId = stores[2].Id, Rating = 4, Comment = "Precios razonables y buena presentacion." },
            new() { CustomerId = customers[3].Id, StoreId = stores[4].Id, Rating = 5, Comment = "Volveria a pedir en este local." },
            new() { CustomerId = customers[0].Id, StoreId = stores[3].Id, Rating = 4, Comment = "Producto fresco y bien empacado." }
        };
        context.Reviews.AddRange(reviews);
        await context.SaveChangesAsync();
    }
}
