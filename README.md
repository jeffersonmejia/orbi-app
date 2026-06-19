# Orbi

ASP.NET Core MVC delivery platform for restaurants, pharmacies, supermarkets, orders, payments, drivers and reviews.

## Stack

| Component | Use |
| --- | --- |
| ASP.NET Core MVC | Razor/MVC web application |
| Entity Framework Core | Data access and migrations |
| PostgreSQL | Database |
| ASP.NET Identity | Users, roles and sessions |
| Bootstrap | Base UI |

## Run

```bash
docker compose up -d
dotnet run --project src/Orbi.Web
```

Open `http://localhost:5130`.

The app applies pending migrations and seeds data on startup.

## Entity Relationship

```mermaid
erDiagram
    Customer ||--o{ Address : has
    Customer ||--o{ Order : places
    Customer ||--o{ Review : writes
    StoreCategory ||--o{ Store : categorizes
    Store ||--o{ Product : offers
    Store ||--o{ Order : receives
    Store ||--o{ Review : rated_by
    Order ||--o{ OrderDetail : contains
    Product ||--o{ OrderDetail : listed_in
    OrderStatus ||--o{ Order : tracks
    DeliveryDriver ||--o{ Order : delivers
    PaymentMethod ||--o{ Payment : used_in
    Order ||--o| Payment : pays

    Customer {
        int Id PK
        string UserId FK
        string FirstName
        string LastName
        string Email UK
        string Phone
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    Address {
        int Id PK
        int CustomerId FK
        string Street
        string City
        string State
        string ZipCode
        string Country
        double Latitude
        double Longitude
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    StoreCategory {
        int Id PK
        string Name UK
        string Description
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    Store {
        int Id PK
        string UserId FK
        int CategoryId FK
        string Name
        string Description
        string Phone
        string Email
        string Address
        double Latitude
        double Longitude
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    Product {
        int Id PK
        int StoreId FK
        string Name
        string Description
        decimal Price
        int Stock
        string ImageUrl
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    Order {
        int Id PK
        int CustomerId FK
        int StoreId FK
        int DeliveryDriverId FK
        int OrderStatusId FK
        int AddressId FK
        decimal TotalAmount
        datetime OrderDate
        datetime DeliveryDate
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    OrderDetail {
        int Id PK
        int OrderId FK
        int ProductId FK
        int Quantity
        decimal UnitPrice
        decimal Subtotal
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    OrderStatus {
        int Id PK
        string Name UK
        string Description
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    DeliveryDriver {
        int Id PK
        string UserId FK
        string FirstName
        string LastName
        string Email UK
        string Phone
        double CurrentLatitude
        double CurrentLongitude
        datetime LastLocationUpdate
        bool IsAvailable
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    PaymentMethod {
        int Id PK
        string Name UK
        string Description
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    Payment {
        int Id PK
        int OrderId FK
        int PaymentMethodId FK
        decimal Amount
        datetime PaymentDate
        string TransactionId
        string Status
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
    Review {
        int Id PK
        int CustomerId FK
        int StoreId FK
        int Rating
        string Comment
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }
```

## Role Access

Navigation stays visible for authenticated users. Forbidden sections return `403` and show the access denied page.

```mermaid
flowchart LR
    Admin[Admin] --> All[All business modules]

    StoreOwner[StoreOwner] --> SOStore[Own store]
    StoreOwner --> SOProducts[Own store products]
    StoreOwner --> SOOrders[Own store orders]
    StoreOwner --> SOPayments[Own store payments]
    StoreOwner --> SOReviews[Own store reviews]
    StoreOwner --> ReadCatalog[Read catalog modules]

    DeliveryDriver[DeliveryDriver] --> DriverProfile[Own driver profile]
    DeliveryDriver --> AssignedOrders[Assigned orders]
    DeliveryDriver --> DeliveryAddress[Assigned order addresses]
    DeliveryDriver --> DriverReadCatalog[Read stores and products]

    Customer[Customer] --> CustomerProfile[Own customer profile]
    Customer --> CustomerAddresses[Own addresses]
    Customer --> CustomerOrders[Own orders]
    Customer --> CustomerPayments[Own payments]
    Customer --> CustomerReviews[Own reviews]
    Customer --> CustomerReadCatalog[Read stores, products and payment methods]
```

| Role | Main access |
| --- | --- |
| `Admin` | Full CRUD on business entities. |
| `StoreOwner` | Own store, products, orders, payments and reviews. |
| `DeliveryDriver` | Own profile and assigned orders. |
| `Customer` | Own profile, addresses, orders, payments and reviews. |

## Applied Security

- Global MVC role gate for business controllers.
- Service-level ownership filters through `UserId`.
- Server-side validation against guessed IDs.
- Forbidden service access converted to `403`.
- Login lockout after failed attempts.
- Public registration limited to `Customer`, `DeliveryDriver` and `StoreOwner`.
- `HttpOnly`, `SameSite=Strict` cookies, with secure cookies in production.
- Security headers: CSP, `X-Frame-Options`, `X-Content-Type-Options`, `Referrer-Policy` and `Permissions-Policy`.
- HSTS outside development and HTTPS redirection.
- Home chart cache avoids re-rendering when counts have not changed.

## Applied Optimizations

- Server-side pagination with `Skip` and `Take`.
- Compact pagination UI with first, previous, page selector, next and last controls.
- ViewModel projections in services.
- `AsNoTracking` on read queries.
- Composite indexes for common filters.
- Trigram GIN indexes for text search.
- Large dropdown queries capped at 200 records.
- In-memory cache for small reference catalogs.

## Pending Work

- Automated authorization tests for cross-user data access.
- Audit logs for sensitive writes.
- Dedicated cancellation and operational status-change flows.
- Email confirmation and password recovery.
- Production security policies for real domains and infrastructure.

## Documentation

| File | Description |
| --- | --- |
| `docs/API.md` | MVC routes and permissions |
| `docs/ROLS.MD` | Role matrix and ownership rules |
| `SECURITY.md` | Security policy and controls |
| `docs/SEED.md` | Initial data |
| `docs/ARCHITECTURE.md` | Architecture notes |
