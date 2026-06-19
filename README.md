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

## 4. Role and Business Flow

Every user signs in through the same application. The role access filter decides which pages are visible, and ownership rules limit the records each role can read or change.

### Admin

```mermaid
graph TD;
    A[Sign in]-->B[Role access filter];
    B-->C[Admin];
    C-->D[Home read];
    C-->E[Catalog full CRUD];
    C-->F[Orders full CRUD];
    C-->G[Directory full CRUD];
```

### StoreOwner

```mermaid
graph TD;
    A[Sign in]-->B[Role access filter];
    B-->C[StoreOwner];
    C-->D[Home read];
    C-->E[Own store and products CRUD];
    C-->F[Reviews and categories read];
    C-->G[Own orders read and edit];
    C-->H[Own payments read];
    C-->I[Drivers read only];
```

### DeliveryDriver

```mermaid
graph TD;
    A[Sign in]-->B[Role access filter];
    B-->C[DeliveryDriver];
    C-->D[Home read];
    C-->E[Stores and products read];
    C-->F[Assigned orders read];
    C-->G[Edit assigned order status];
    C-->H[Own driver profile];
    C-->I[Assigned addresses read];
```

### Customer

```mermaid
graph TD;
    A[Sign in]-->B[Role access filter];
    B-->C[Customer];
    C-->D[Home read];
    C-->E[Stores and products read];
    C-->F[Own orders read];
    C-->G[Create own orders];
    C-->H[Profile only];
```
