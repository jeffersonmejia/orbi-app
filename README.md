# Orbi

Multi-service delivery platform. Restaurants, pharmacies and supermarkets in one web app.

## Tech Stack

| Technology | Version |
|------------|---------|
| ASP.NET Core MVC | 10.0 |
| Entity Framework Core | 10.0 |
| C# | 13 |
| PostgreSQL | 16 |
| Bootstrap | 5.3 |
| Npgsql | 10.0.2 |

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
        string FirstName
        string LastName
        string Email UK
        string Phone
        string PasswordHash
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

## Installation

```bash
# clone
git clone git@github.com:jeffersonmejia/orbi-app.git
cd orbi-app

# database
docker compose up -d

# run
dotnet run --project src/Orbi.Web
```

Open `http://localhost:5130`.

## Documentation

| File | Description |
|------|-------------|
| [ARCHITECTURE.md](docs/ARCHITECTURE.md) | Layers, patterns, ERD, scalability |
| [API.md](docs/API.md) | Endpoints, request and response schemas |
| [SECURITY.md](docs/SECURITY.md) | Auth, data protection, reporting |
