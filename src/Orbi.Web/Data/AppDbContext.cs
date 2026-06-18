using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Orbi.Web.Models;

namespace Orbi.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<StoreCategory> StoreCategories => Set<StoreCategory>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<DeliveryDriver> DeliveryDrivers => Set<DeliveryDriver>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(c => c.Email).IsUnique();
            entity.HasIndex(c => new { c.IsActive, c.LastName, c.FirstName });
            entity.HasIndex(c => new { c.IsActive, c.Email });
            entity.HasQueryFilter(c => c.IsActive);
            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DeliveryDriver>(entity =>
        {
            entity.HasIndex(d => d.Email).IsUnique();
            entity.HasIndex(d => new { d.IsActive, d.LastName, d.FirstName });
            entity.HasIndex(d => new { d.IsActive, d.Email });
            entity.HasIndex(d => new { d.IsActive, d.IsAvailable });
            entity.HasQueryFilter(d => d.IsActive);
            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasOne(s => s.Category)
                  .WithMany(sc => sc.Stores)
                  .HasForeignKey(s => s.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(s => s.IsActive);
            entity.HasIndex(s => new { s.IsActive, s.Name });
            entity.HasIndex(s => new { s.CategoryId, s.IsActive });
            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasOne(a => a.Customer)
                  .WithMany(c => c.Addresses)
                  .HasForeignKey(a => a.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(a => a.IsActive);
            entity.HasIndex(a => new { a.IsActive, a.City, a.Street });
            entity.HasIndex(a => new { a.CustomerId, a.IsActive });
        });

        modelBuilder.Entity<StoreCategory>(entity =>
        {
            entity.HasIndex(sc => sc.Name).IsUnique();
            entity.HasIndex(sc => new { sc.IsActive, sc.Name });
            entity.HasQueryFilter(sc => sc.IsActive);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(p => p.Store)
                  .WithMany(s => s.Products)
                  .HasForeignKey(p => p.StoreId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(p => new { p.StoreId, p.Name });
            entity.HasIndex(p => new { p.IsActive, p.Name });
            entity.HasIndex(p => new { p.StoreId, p.IsActive, p.Name });

            entity.HasQueryFilter(p => p.IsActive);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Store)
                  .WithMany(s => s.Orders)
                  .HasForeignKey(o => o.StoreId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.DeliveryDriver)
                  .WithMany(d => d.Orders)
                  .HasForeignKey(o => o.DeliveryDriverId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(o => o.OrderStatus)
                  .WithMany(os => os.Orders)
                  .HasForeignKey(o => o.OrderStatusId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Address)
                  .WithMany()
                  .HasForeignKey(o => o.AddressId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(o => o.OrderDate);
            entity.HasIndex(o => o.CustomerId);
            entity.HasIndex(o => o.StoreId);
            entity.HasIndex(o => new { o.IsActive, o.OrderDate });
            entity.HasIndex(o => new { o.IsActive, o.CustomerId });
            entity.HasIndex(o => new { o.IsActive, o.StoreId });
            entity.HasIndex(o => new { o.IsActive, o.DeliveryDriverId });
            entity.HasIndex(o => new { o.IsActive, o.OrderStatusId });

            entity.HasQueryFilter(o => o.IsActive);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasOne(od => od.Order)
                  .WithMany(o => o.OrderDetails)
                  .HasForeignKey(od => od.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(od => od.Product)
                  .WithMany(p => p.OrderDetails)
                  .HasForeignKey(od => od.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(od => od.IsActive);
            entity.HasIndex(od => new { od.OrderId, od.IsActive });
            entity.HasIndex(od => new { od.ProductId, od.IsActive });
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasIndex(os => os.Name).IsUnique();
            entity.HasIndex(os => new { os.IsActive, os.Name });
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasIndex(pm => pm.Name).IsUnique();
            entity.HasIndex(pm => new { pm.IsActive, pm.Name });
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasOne(p => p.Order)
                  .WithOne(o => o.Payment)
                  .HasForeignKey<Payment>(p => p.OrderId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.PaymentMethod)
                  .WithMany(pm => pm.Payments)
                  .HasForeignKey(p => p.PaymentMethodId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(p => p.IsActive);
            entity.HasIndex(p => new { p.IsActive, p.PaymentDate });
            entity.HasIndex(p => new { p.IsActive, p.Status });
            entity.HasIndex(p => new { p.PaymentMethodId, p.IsActive });
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasOne(r => r.Customer)
                  .WithMany(c => c.Reviews)
                  .HasForeignKey(r => r.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Store)
                  .WithMany(s => s.Reviews)
                  .HasForeignKey(r => r.StoreId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.CustomerId, r.StoreId });
            entity.HasIndex(r => new { r.IsActive, r.UpdatedAt });
            entity.HasIndex(r => new { r.CustomerId, r.IsActive });
            entity.HasIndex(r => new { r.StoreId, r.IsActive });

            entity.HasQueryFilter(r => r.IsActive);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            entity.UpdatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}
