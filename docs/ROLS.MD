# Roles and Permissions

Orbi uses ASP.NET Identity roles stored in `AspNetRoles`. These are application roles, not PostgreSQL roles.

## Roles

| Role | Purpose |
| --- | --- |
| `Admin` | Full system administration and operational support. |
| `StoreOwner` | Manages one store, its products and incoming orders. |
| `DeliveryDriver` | Manages assigned deliveries and availability. |
| `Customer` | Browses stores/products and places orders. Personal data is managed through Profile. |

## Default Seeded Admin

`DbSeeder` creates the default admin only when `admin@orbi.com` does not already exist.

| Role | Email | Password |
| --- | --- | --- |
| `Admin` | `admin@orbi.com` | `Admin1234!` |

## Access Table

Unauthenticated users only see `Sign In` and `Sign up` in the aside. Authenticated users only see sections authorized for their role.

| Role | Home | Catalog: Products, Categories, Stores, Reviews | Orders: Orders, Statuses, Payments, Pay Methods | Directory: Customers, Addresses, Drivers |
| --- | --- | --- | --- | --- |
| `Admin` | Read | Full CRUD | Full CRUD | Full CRUD |
| `StoreOwner` | Read | Own store/products, own store reviews read, categories read | Own store orders read/edit, own store payments read, statuses/pay methods read | Drivers read only |
| `DeliveryDriver` | Read | Stores and products read only | Assigned orders read/edit status, statuses read | Own driver profile, assigned order addresses read |
| `Customer` | Read | Stores/products read | Own orders read/create | Hidden; profile only |

| Aside section | Module | Admin | StoreOwner | DeliveryDriver | Customer |
| --- | --- | --- | --- | --- | --- |
| Home | Dashboard | Read | Read | Read | Read |
| Catalog | Products | CRUD | Own store CRUD | Read | Read |
| Catalog | Categories | CRUD | Read | Read | Hidden |
| Catalog | Stores | CRUD | Own store CRUD | Read | Read |
| Catalog | Reviews | CRUD | Own store read | No access | Hidden |
| Orders | Orders | CRUD | Own store read/edit | Assigned read/edit status | Own read/create |
| Orders | Statuses | CRUD | Read | Read | Hidden |
| Orders | Payments | CRUD | Own store read | No access | Hidden |
| Orders | Pay Methods | CRUD | Read | No access | Hidden |
| Directory | Customers | CRUD | No access | No access | Hidden; use Profile |
| Directory | Addresses | CRUD | No access | Assigned order read | Hidden |
| Directory | Drivers | CRUD | Read | Own read/edit | No access |

## Ownership Rules

| Role | Main entity | Link field |
| --- | --- | --- |
| `Customer` | `Customers` | `Customer.UserId == User.Id` |
| `StoreOwner` | `Stores` | `Store.UserId == User.Id` |
| `DeliveryDriver` | `DeliveryDrivers` | `DeliveryDriver.UserId == User.Id` |

Role authorization is not enough. Sensitive queries and writes must also filter by the signed-in user.
