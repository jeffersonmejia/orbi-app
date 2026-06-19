# API and MVC Routes

Orbi exposes MVC routes with the default pattern:

```text
/{Controller}/{Action}/{id?}
```

List screens accept `searchField`, `searchTerm` and `page`.

## Public

| Method | Route | Use |
| --- | --- | --- |
| GET | `/` | Home |
| GET | `/Home/Index` | Home |
| GET | `/Home/RecordCounts` | Home chart counts |
| GET | `/Home/DbStatus` | Database status |
| GET | `/Account/Login` | Login form |
| POST | `/Account/Login` | Sign in |
| GET | `/Account/Register` | Registration form |
| POST | `/Account/Register` | Create `Customer`, `DeliveryDriver` or `StoreOwner` user |

`POST /Account/Logout` requires an authenticated user.

## Role Access

```mermaid
flowchart TD
    Admin --> AllModules[All modules: CRUD]

    StoreOwner --> StoreOwned[Stores: own CRUD]
    StoreOwner --> ProductOwned[Products: own store CRUD]
    StoreOwner --> OrderOwned[Orders: own store read/edit]
    StoreOwner --> PaymentOwned[Payments: own store read]
    StoreOwner --> ReviewOwned[Reviews: own store read]
    StoreOwner --> StoreOwnerRead[Categories and payment methods: read]

    DeliveryDriver --> DriverProfile[Drivers: own read/edit]
    DeliveryDriver --> DriverOrders[Orders: assigned read/edit status]
    DeliveryDriver --> DriverCatalog[Stores and products: read]

    Customer --> CustomerProfile[Profile: personal data only]
    Customer --> CustomerOrders[Orders: own read/create]
    Customer --> CustomerCatalog[Stores and products: read]
```

| Module | Admin | StoreOwner | DeliveryDriver | Customer |
| --- | --- | --- | --- | --- |
| StoreCategories | CRUD | Read | Read | Forbidden |
| Stores | CRUD | Own CRUD | Read | Read |
| Products | CRUD | Own store CRUD | Read | Read |
| Customers | CRUD | Forbidden | Forbidden | Forbidden; use Profile |
| Addresses | CRUD | Forbidden | Assigned order address read | Forbidden |
| Orders | CRUD | Own store read/edit | Assigned read/edit status | Own read/create |
| OrderStatuses | CRUD | Read | Read | Forbidden |
| DeliveryDrivers | CRUD | Read | Own read/edit | Forbidden |
| PaymentMethods | CRUD | Read | Forbidden | Forbidden |
| Payments | CRUD | Own store read | Forbidden | Forbidden |
| Reviews | CRUD | Own store read | Forbidden | Forbidden |

Authenticated navigation only shows sections authorized for the current role. Forbidden direct access returns `403` and shows the access denied page.

## CRUD Routes

| Action | Method | Route |
| --- | --- | --- |
| Index | GET | `/{Controller}` |
| Details | GET | `/{Controller}/Details/{id}` |
| Create | GET/POST | `/{Controller}/Create` |
| Edit | GET/POST | `/{Controller}/Edit/{id}` |
| Delete | POST | `/{Controller}/Delete/{id}` |

`Delete` is a soft delete through `IsActive = false`.

## Ownership Rules

| Role | Rule |
| --- | --- |
| Customer | `Customers.UserId == User.Id` |
| StoreOwner | `Stores.UserId == User.Id` |
| DeliveryDriver | `DeliveryDrivers.UserId == User.Id` |

Sensitive queries are scoped by owner in services. Sensitive writes validate ownership again before saving.

## Input Security

- All POST actions use antiforgery tokens.
- Order totals are calculated server-side.
- Public registration cannot create `Admin`.
- Form dropdowns only load records within the signed-in user's scope.
