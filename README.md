# Orbi Platform

**Orbi** is a multi-service delivery platform that unifies restaurants, pharmacies, and supermarkets into a single ecosystem. Built with ASP.NET Core MVC, Entity Framework Core, and PostgreSQL, Orbi provides a centralized web application where users can order products from different store categories, track deliveries in real time, and manage their entire experience from one place.

## Table of Contents

- [Business Problem](#business-problem)
- [Solution](#solution)
- [Differentiator](#differentiator)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Database Model](#database-model)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Contributing](#contributing)
- [License](#license)

## Business Problem

Users currently need to juggle multiple applications to place orders from different types of businesses — one app for restaurant food, another for pharmacy items, and yet another for supermarket groceries. This fragmented experience leads to wasted time, confusing order tracking, and a lack of unified payment and delivery management.

## Solution

Orbi brings all commerce types under one roof. A single registration gives users access to:

- **Restaurants** — Order prepared meals and beverages.
- **Pharmacies** — Purchase medicines and health products.
- **Supermarkets** — Buy groceries and household essentials.

Each order flows through a unified pipeline: placement, confirmation, preparation, transit, and delivery — with full visibility at every stage.

## Differentiator

Real-time delivery driver tracking powered by geospatial coordinates stored directly in the database. Customers can see their driver's current latitude and longitude, updated continuously throughout the delivery process. This feature is built into the core data model rather than relying on third-party mapping SDKs, keeping the architecture self-contained and extensible.

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core MVC (.NET 10) |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL |
| Language | C# |
| Frontend | Razor Views + Bootstrap 5 |
| Icons | Bootstrap Icons |

### NuGet Packages

| Package | Version | Purpose |
|---------|---------|---------|
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.2 | PostgreSQL database provider for EF Core; enables communication between the ORM and the PostgreSQL engine, handling connection management, query translation, and type mapping. |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.9 | Provides design-time EF Core tools; required for creating and applying migrations from the command line via `dotnet ef` commands. |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.9 | Adds EF Core PowerShell commands to the Visual Studio Package Manager Console; enables `Add-Migration`, `Update-Database`, and other scaffolding operations. |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 10.0.2 | Provides scaffolding engines for ASP.NET Core; used to auto-generate controllers and views from model classes during development. |

## Project Structure

```
Orbi/
├── Orbi.sln
├── README.md
├── SECURITY.md
└── src/
    └── Orbi.Web/
        ├── Program.cs                  # Application entry point; service registration, middleware pipeline, DB migration, and seeding
        ├── appsettings.json            # Configuration file; connection strings, logging levels, and app settings
        ├── appsettings.Development.json# Development-specific overrides
        ├── Models/                     # Domain entities (POCO classes mapped to database tables)
        │   ├── BaseEntity.cs           # Abstract base with Id, IsActive, CreatedAt, UpdatedAt
        │   ├── Customer.cs
        │   ├── Address.cs
        │   ├── StoreCategory.cs
        │   ├── Store.cs
        │   ├── Product.cs
        │   ├── Order.cs
        │   ├── OrderDetail.cs
        │   ├── DeliveryDriver.cs
        │   ├── OrderStatus.cs
        │   ├── PaymentMethod.cs
        │   ├── Payment.cs
        │   └── Review.cs
        ├── Data/                       # Data access and configuration layer
        │   ├── AppDbContext.cs         # EF Core DbContext; entity configurations, query filters, relationships
        │   └── DbSeeder.cs            # Seed data generator; populates tables with sample records
        ├── ViewModels/                 # Presentation models tailored to specific views
        │   ├── ProductViewModel.cs
        │   └── OrderViewModel.cs
        ├── Services/                   # Business logic layer; encapsulates CRUD operations
        │   ├── IEntityService.cs       # Generic service interface
        │   ├── ProductService.cs
        │   └── OrderService.cs
        ├── Controllers/                # MVC controllers; handle HTTP requests and orchestrate views
        │   ├── HomeController.cs
        │   ├── ProductsController.cs
        │   └── OrdersController.cs
        ├── Views/                      # Razor view templates
        │   ├── Home/
        │   │   └── Index.cshtml
        │   ├── Products/
        │   │   ├── Index.cshtml        # Product list with CRUD actions
        │   │   ├── Create.cshtml
        │   │   ├── Edit.cshtml
        │   │   └── Details.cshtml
        │   ├── Orders/
        │   │   ├── Index.cshtml        # Order list with status badges
        │   │   ├── Create.cshtml
        │   │   ├── Edit.cshtml
        │   │   └── Details.cshtml
        │   └── Shared/
        │       └── _Layout.cshtml      # Master layout with Orbi branding
        ├── Migrations/                 # EF Core migration files (auto-generated)
        └── wwwroot/                    # Static assets (CSS, JS, images, libraries)
```

## Database Model

### Entity-Relationship Diagram (Textual)

```
┌─────────────┐     ┌──────────────┐
│   Customer  │1──N→│   Address    │
└─────────────┘     └──────────────┘
       │1
       │
       │N
       ▼
┌─────────────┐     ┌──────────────┐
│    Order    │N──1→│  OrderStatus │
└─────────────┘     └──────────────┘
       │1               ┌─────────┐
       │                │Payment  │
       │1               │Method   │
       ▼                └─────────┘
┌─────────────┐     ┌───┘      1
│  OrderDetail│N──1→│Payment    │
└─────────────┘     └──────────────┘
       │N              1│
      1│                │
       ▼                │
┌─────────────┐         │
│   Product   │N──1→Store│
└─────────────┘     └──│──┘
       │               ││
       │               ││N
       │               │▼
       │         ┌──────────────┐
       │     N──1│StoreCategory │
       │         └──────────────┘
       │
       │N
       ▼
┌─────────────┐     ┌──────────────┐
│   Review    │1──N→│  Customer    │
└─────────────┘     └──────────────┘
       │N
      1│
       ▼
┌──────────────┐
│    Store     │
└──────────────┘

┌────────────────┐
│ DeliveryDriver │1──N→Order
└────────────────┘
```

### Cardinalities Explained

- **Customer : Address** — 1:N. One customer can register multiple delivery addresses.
- **Customer : Order** — 1:N. One customer can place many orders.
- **Order : OrderDetail** — 1:N. One order contains multiple line items.
- **Order : Payment** — 1:1. Each order has exactly one payment transaction.
- **Order : OrderStatus** — N:1. Many orders share the same status value (Pending, Delivered, etc.).
- **Order : DeliveryDriver** — N:1. A driver can be assigned to many orders; a driver is not required at order creation time (nullable FK).
- **Order : Store** — N:1. Each order belongs to a single store.
- **OrderDetail : Product** — N:1. Many order detail rows can reference the same product.
- **Product : Store** — N:1. Each product belongs to exactly one store.
- **Store : StoreCategory** — N:1. Each store belongs to one category (Restaurant, Pharmacy, Supermarket).
- **Payment : PaymentMethod** — N:1. Each payment uses one payment method.
- **Review : Customer** — N:1. A customer can write many reviews.
- **Review : Store** — N:1. A store receives many reviews from different customers.

### Logical Deletion

All entities inherit from `BaseEntity` which includes an `IsActive` flag. EF Core global query filters (`HasQueryFilter`) automatically exclude inactive records from all queries. This preserves referential integrity and audit trails while providing soft-delete semantics.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (14 or later)
- [Entity Framework Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/orbi.git
   cd orbi
   ```

2. **Configure the connection string**  
   Update `src/Orbi.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=OrbiDb;Username=postgres;Password=your_password"
     }
   }
   ```

3. **Create the database and apply migrations**
   ```bash
   dotnet ef database update --project src/Orbi.Web
   ```

   Alternatively, the application applies pending migrations automatically on startup via `Program.cs`:
   ```csharp
   await context.Database.MigrateAsync();
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/Orbi.Web
   ```

5. **Access the web interface**  
   Open `http://localhost:5000` in your browser.

### Commands Used During Development

```bash
# Create solution and project
dotnet new sln -n Orbi
dotnet new mvc -n Orbi.Web -o src/Orbi.Web --no-https
dotnet sln add src/Orbi.Web/Orbi.Web.csproj

# Add NuGet packages
dotnet add src/Orbi.Web package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/Orbi.Web package Microsoft.EntityFrameworkCore.Design
dotnet add src/Orbi.Web package Microsoft.EntityFrameworkCore.Tools
dotnet add src/Orbi.Web package Microsoft.VisualStudio.Web.CodeGeneration.Design

# Create initial migration
dotnet ef migrations add InitialCreate --project src/Orbi.Web

# Apply migration to database
dotnet ef database update --project src/Orbi.Web
```

## Architectural Decisions for Scale

Orbi is designed to handle up to 500,000 records per table. The following patterns ensure performance at scale:

| Concern | Strategy |
|---------|----------|
| **Pagination** | All list views should use `Skip()` / `Take()` with a configurable page size rather than loading all records into memory. The service layer accepts `page` and `pageSize` parameters. |
| **Filtering** | `IQueryable` chains allow dynamic filter composition (by store, category, date range, status) before execution. Filters are applied server-side. |
| **Soft Delete** | `IsActive` flag with global query filters; deleted records remain in the database for auditing but are invisible to application queries. |
| **Indexing** | Composite indexes on frequently queried columns (e.g., `(StoreId, Name)` for products, `OrderDate` for orders, `(CustomerId, StoreId)` for reviews). |
| **Eager Loading** | `Include()` and `ThenInclude()` are used selectively to avoid N+1 queries while preventing cartesian explosion on large result sets. |
| **Sessions** | ASP.NET Core session state combined with distributed caching (Redis/SQL Server) for user context across requests. |
| **Reports & Statistics** | Dedicated read-optimized queries using compiled LINQ queries for aggregations: total sales per store, most ordered products, average delivery time, driver utilization. |

## Contributing

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -m 'Add your feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Open a Pull Request.

## License

This project is for academic purposes as part of the "Integration of Multi-Platform Architecture" course.
