# Orbi

## 1. Summary

Orbi is an ASP.NET Core MVC delivery platform for restaurants, pharmacies and supermarkets. It manages catalog data, customers, addresses, stores, products, orders, delivery drivers, payments and reviews with ASP.NET Identity roles.

## 2. Technologies

| Technology | Exact version | Source |
| --- | --- | --- |
| .NET SDK | 10.0.300 | Local SDK |
| Target framework | net10.0 | `src/Orbi.Web/Orbi.Web.csproj` |
| ASP.NET Core MVC | 10.0 | Target framework |
| ASP.NET Identity EF Core | 10.0.2 | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` |
| Entity Framework Core Design | 10.0.9 | `Microsoft.EntityFrameworkCore.Design` |
| Entity Framework Core Tools | 10.0.9 | `Microsoft.EntityFrameworkCore.Tools` |
| Npgsql EF Core provider | 10.0.2 | `Npgsql.EntityFrameworkCore.PostgreSQL` |
| Visual Studio Web Code Generation | 10.0.2 | `Microsoft.VisualStudio.Web.CodeGeneration.Design` |
| PostgreSQL Docker image | postgres:16-alpine | `docker-compose.yml` |
| Seed Docker image | postgres:16-alpine | `docker-compose.yml` |
| Docker Engine | 26.1.5+dfsg1 | Local Docker CLI |
| Docker Compose | 2.26.1-4 | Local Docker Compose plugin |
| Bootstrap | 5.3.3 | `wwwroot/lib/bootstrap` |
| Bootstrap Icons | 1.11.3 | CDN in `_Layout.cshtml` |
| jQuery | 3.7.1 | `wwwroot/lib/jquery` |
| jQuery Validation | 1.21.0 | `wwwroot/lib/jquery-validation` |
| jQuery Validation Unobtrusive | 4.0.0 | `wwwroot/lib/jquery-validation-unobtrusive` |

## 3. Installation

```bash
git clone git@github.com:jeffersonmejia/orbi-app.git
cd orbi-app
docker compose up -d
dotnet run --project src/Orbi.Web
```

Open `http://localhost:5130`.

The app applies pending EF Core migrations on startup. The seed container waits for the EF Core schema and then loads the SQL files in `db/`.

## 4. Application Architecture

```mermaid
flowchart TB
    User[Browser user] -->|HTTP requests| MVC[ASP.NET Core MVC]

    subgraph WebApp[Orbi.Web]
        MVC --> Controllers[Controllers]
        Controllers --> RoleFilter[RoleAccessFilter]
        Controllers --> Services[Entity services]
        Controllers --> Identity[ASP.NET Identity]
        Services --> Access[CurrentUserAccess ownership filters]
        Services --> EF[Entity Framework Core DbContext]
        Identity --> EF
        RoleFilter --> Identity
        Controllers --> Views[Razor views and view models]
        Views --> User
    end

    EF -->|Npgsql provider| Postgres[(PostgreSQL OrbiDb)]

    subgraph Startup[Startup and data loading]
        Migrations[EF Core migrations] --> Postgres
        Seed[Seed container SQL scripts] --> Postgres
    end

    Docker[Docker Compose] --> Postgres
    Docker --> Seed
```

Orbi follows a classic MVC structure. Controllers receive requests and coordinate validation, authorization and service calls. Services build scoped `IQueryable` queries, apply pagination/search rules and project entities into view models. `CurrentUserAccess` adds ownership limits for roles such as `Customer`, `StoreOwner` and `DeliveryDriver`, while ASP.NET Identity stores users and roles in the same PostgreSQL database.

## 5. Role and Business Flow

```mermaid
flowchart LR
    subgraph People[User roles]
        Admin[Admin]
        Owner[StoreOwner]
        Driver[DeliveryDriver]
        Customer[Customer]
    end

    subgraph Access[Access and ownership rules]
        Login[Sign in]
        Roles[RoleAccessFilter]
        Scope[CurrentUserAccess data scope]
    end

    subgraph Business[Business capabilities]
        Catalog[Catalog management]
        Orders[Order lifecycle]
        Delivery[Delivery assignment and status]
        Payments[Payments and payment methods]
        Reviews[Reviews and customer feedback]
        Directory[Customers addresses and drivers]
    end

    subgraph Data[Shared operational data]
        Db[(PostgreSQL OrbiDb)]
    end

    Admin --> Login
    Owner --> Login
    Driver --> Login
    Customer --> Login
    Login --> Roles
    Roles --> Scope

    Scope --> Catalog
    Scope --> Orders
    Scope --> Delivery
    Scope --> Payments
    Scope --> Reviews
    Scope --> Directory

    Catalog --> Db
    Orders --> Db
    Delivery --> Db
    Payments --> Db
    Reviews --> Db
    Directory --> Db
```

This view is intended for non-technical readers. Every user signs in through the same application, but their role decides which business areas they can open. After that, ownership rules decide which records they can see or change: admins work across the platform, store owners work on their own store data, delivery drivers work on assigned deliveries, and customers work on their own orders, addresses, payments and reviews.

## 6. Database Schema

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
