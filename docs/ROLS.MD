# Roles and permissions

This file defines the application roles for Orbi. These are ASP.NET Identity roles stored in `AspNetRoles`, not PostgreSQL roles.

## Roles

| Role | Purpose |
| --- | --- |
| `Admin` | Full system administration and operational support. |
| `StoreOwner` | Manages one store, its products, and incoming orders. |
| `DeliveryDriver` | Manages assigned deliveries and availability. |
| `Customer` | Places orders, manages addresses, payments, and reviews. |

These roles already exist in `DbSeeder`: `Customer`, `DeliveryDriver`, `StoreOwner`, `Admin`.

## Access model

`Admin` can read, create, update, and soft delete every business entity.

`StoreOwner` can only manage resources linked to their store through `Stores.UserId`.

`DeliveryDriver` can only manage resources linked to their driver profile through `DeliveryDrivers.UserId`.

`Customer` can only manage resources linked to their customer profile through `Customers.UserId`.

Anonymous users can access home, login, register, record counts and database status only.

## Permission matrix

| Module | Admin | StoreOwner | DeliveryDriver | Customer |
| --- | --- | --- | --- | --- |
| StoreCategories | CRUD | Read | Read | Read |
| Stores | CRUD | Own store CRUD | Read | Read |
| Products | CRUD | Own store product CRUD | Read | Read |
| Customers | CRUD | No direct access | No direct access | Own profile |
| Addresses | CRUD | No direct access | Assigned order address read | Own address CRUD |
| Orders | CRUD | Own store orders | Assigned orders | Own orders |
| OrderDetails | CRUD | Own store order details | Assigned order details | Own order details |
| OrderStatuses | CRUD | Read and operational updates | Delivery updates | Read |
| DeliveryDrivers | CRUD | Read for assignment | Own profile and availability | No access |
| PaymentMethods | CRUD | Read | No access | Read |
| Payments | CRUD | Own store payments | No access | Own payments |
| Reviews | CRUD | Own store reviews read | No access | Own review CRUD |

## Ownership rules

| Role | Main entity | Link field |
| --- | --- | --- |
| `Customer` | `Customers` | `Customer.UserId == User.Id` |
| `StoreOwner` | `Stores` | `Store.UserId == User.Id` |
| `DeliveryDriver` | `DeliveryDrivers` | `DeliveryDriver.UserId == User.Id` |

Role authorization is not enough. Sensitive queries must also filter by the signed-in user.

Examples:

- A `Customer` must not read another customer's order by guessing an `OrderId`.
- A `StoreOwner` must not edit products from another store.
- A `DeliveryDriver` must not update orders that are not assigned to them.

## Controller policy

| Controller | Recommended access |
| --- | --- |
| `AccountController` | Anonymous access for login and register. Authenticated access for logout. |
| `CustomersController` | `Admin`. `Customer` only for own profile through a dedicated flow. |
| `AddressesController` | `Admin`. `Customer` only for own addresses. |
| `StoreCategoriesController` | Authenticated read. Write only for `Admin`. |
| `StoresController` | Authenticated read. Write for `Admin` or owning `StoreOwner`. |
| `ProductsController` | Authenticated read. Write for `Admin` or owning `StoreOwner`. |
| `OrdersController` | `Admin`, `StoreOwner`, `DeliveryDriver`, and `Customer` with ownership filters. |
| `OrderStatusesController` | Write only for `Admin`. Operational status changes should happen through `OrdersController`. |
| `DeliveryDriversController` | `Admin`. `DeliveryDriver` only for own profile and availability. |
| `PaymentMethodsController` | Write only for `Admin`. Read for `Customer` and `StoreOwner`. |
| `PaymentsController` | `Admin`. `Customer` for own payments. `StoreOwner` for own store payments. |
| `ReviewsController` | `Admin`. `Customer` for own reviews. `StoreOwner` for own store review reads. |

## Implementation status

Applied:

- Global MVC role gate through `RoleAccessFilter`.
- Ownership scoping through `CurrentUserAccess`.
- Service-level filters for customers, addresses, stores, products, orders, drivers, payments and reviews.
- Admin-only write guards for store categories, order statuses and payment methods.
- POST ownership validation before sensitive writes.
- Forbidden service access converted to `403`.
- `AccessDeniedPath` points to `/Home/NotAuthorized`.
- Navigation remains visible; forbidden sections show an access denied page instead of being hidden.

Still pending:

- Add automated access tests for cross-user data protection.
- Add audit logs for sensitive writes.
- Add dedicated status-change flows for operational order updates.
- Add email confirmation and password recovery.

## Error pages

Current state:

- Generic error view: `Views/Shared/Error.cshtml`.
- Unauthorized page: `Views/Home/NotAuthorized.cshtml`.
- Not found page: `Views/Home/NotFound.cshtml`.
- `AccessDeniedPath`: `/Home/NotAuthorized`.
- `404` status handling routes to the not found page.
