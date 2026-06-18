# API and MVC routes by role

Orbi currently exposes MVC routes backed by Razor views. The routes below follow the default pattern:

```text
/{Controller}/{Action}/{id}
```

For `Index`, the shorter route `/{Controller}` is also valid. List screens commonly accept:

| Query | Type | Use |
| --- | --- | --- |
| `searchField` | string | Field selected for filtering |
| `searchTerm` | string | Search text |
| `page` | int | Page number |

Current security note: roles exist in `DbSeeder`, but most business controllers still need `[Authorize]`, role checks, and ownership filters.

## Public

| HTTP | Route | Action |
| --- | --- | --- |
| GET | `/` | Home page |
| GET | `/Home/Index` | Home page |
| GET | `/Home/Privacy` | Privacy page |
| GET | `/Account/Login` | Login form |
| POST | `/Account/Login` | Sign in |
| GET | `/Account/Register` | Registration form |
| POST | `/Account/Register` | Create account and assign role |
| POST | `/Account/Logout` | Sign out |

Registration can create linked business records depending on selected role:

| Role | Created business record |
| --- | --- |
| `Customer` | `Customer` and optional `Address` |
| `DeliveryDriver` | `DeliveryDriver` |
| `StoreOwner` | `Store` |

## Admin

`Admin` should have full CRUD access to every business module.

| Module | List | Details | Create | Edit | Delete |
| --- | --- | --- | --- | --- | --- |
| Customers | GET `/Customers` | GET `/Customers/Details/{id}` | GET, POST `/Customers/Create` | GET, POST `/Customers/Edit/{id}` | POST `/Customers/Delete/{id}` |
| Addresses | GET `/Addresses` | GET `/Addresses/Details/{id}` | GET, POST `/Addresses/Create` | GET, POST `/Addresses/Edit/{id}` | POST `/Addresses/Delete/{id}` |
| StoreCategories | GET `/StoreCategories` | GET `/StoreCategories/Details/{id}` | GET, POST `/StoreCategories/Create` | GET, POST `/StoreCategories/Edit/{id}` | POST `/StoreCategories/Delete/{id}` |
| Stores | GET `/Stores` | GET `/Stores/Details/{id}` | GET, POST `/Stores/Create` | GET, POST `/Stores/Edit/{id}` | POST `/Stores/Delete/{id}` |
| Products | GET `/Products` | GET `/Products/Details/{id}` | GET, POST `/Products/Create` | GET, POST `/Products/Edit/{id}` | POST `/Products/Delete/{id}` |
| Orders | GET `/Orders` | GET `/Orders/Details/{id}` | GET, POST `/Orders/Create` | GET, POST `/Orders/Edit/{id}` | POST `/Orders/Delete/{id}` |
| OrderStatuses | GET `/OrderStatuses` | GET `/OrderStatuses/Details/{id}` | GET, POST `/OrderStatuses/Create` | GET, POST `/OrderStatuses/Edit/{id}` | POST `/OrderStatuses/Delete/{id}` |
| DeliveryDrivers | GET `/DeliveryDrivers` | GET `/DeliveryDrivers/Details/{id}` | GET, POST `/DeliveryDrivers/Create` | GET, POST `/DeliveryDrivers/Edit/{id}` | POST `/DeliveryDrivers/Delete/{id}` |
| PaymentMethods | GET `/PaymentMethods` | GET `/PaymentMethods/Details/{id}` | GET, POST `/PaymentMethods/Create` | GET, POST `/PaymentMethods/Edit/{id}` | POST `/PaymentMethods/Delete/{id}` |
| Payments | GET `/Payments` | GET `/Payments/Details/{id}` | GET, POST `/Payments/Create` | GET, POST `/Payments/Edit/{id}` | POST `/Payments/Delete/{id}` |
| Reviews | GET `/Reviews` | GET `/Reviews/Details/{id}` | GET, POST `/Reviews/Create` | GET, POST `/Reviews/Edit/{id}` | POST `/Reviews/Delete/{id}` |

## StoreOwner

`StoreOwner` should operate only on stores linked by `Stores.UserId`.

| Module | Allowed routes | Ownership rule |
| --- | --- | --- |
| Stores | GET `/Stores`, GET `/Stores/Details/{id}`, GET and POST `/Stores/Edit/{id}` | Only own store |
| Products | GET `/Products`, GET `/Products/Details/{id}`, GET and POST `/Products/Create`, GET and POST `/Products/Edit/{id}`, POST `/Products/Delete/{id}` | Only products from own store |
| Orders | GET `/Orders`, GET `/Orders/Details/{id}`, GET and POST `/Orders/Edit/{id}` | Only orders received by own store |
| OrderDetails | Through `/Orders/Details/{id}` | Only details from own store orders |
| Payments | GET `/Payments`, GET `/Payments/Details/{id}` | Only payments for own store orders |
| Reviews | GET `/Reviews`, GET `/Reviews/Details/{id}` | Only reviews for own store |
| StoreCategories | GET `/StoreCategories`, GET `/StoreCategories/Details/{id}` | Read only |
| PaymentMethods | GET `/PaymentMethods`, GET `/PaymentMethods/Details/{id}` | Read only |

Recommended restrictions:

- Do not allow access to `/Customers` as a directory.
- Do not allow direct writes to `/OrderStatuses`.
- Do not allow editing another store, product, order, payment, or review.

## DeliveryDriver

`DeliveryDriver` should operate only on the profile linked by `DeliveryDrivers.UserId` and assigned orders.

| Module | Allowed routes | Ownership rule |
| --- | --- | --- |
| DeliveryDrivers | GET `/DeliveryDrivers/Details/{id}`, GET and POST `/DeliveryDrivers/Edit/{id}` | Only own profile |
| Orders | GET `/Orders`, GET `/Orders/Details/{id}`, GET and POST `/Orders/Edit/{id}` | Only assigned orders |
| OrderDetails | Through `/Orders/Details/{id}` | Only assigned orders |
| Addresses | Through assigned order views | Read only delivery address |
| Stores | GET `/Stores`, GET `/Stores/Details/{id}` | Read only |
| Products | GET `/Products`, GET `/Products/Details/{id}` | Read only |
| OrderStatuses | Through order update flow | Delivery status updates only |

Recommended restrictions:

- Do not allow creating or deleting orders.
- Do not allow access to customers as a directory.
- Do not allow payment, payment method, category, or review management.

## Customer

`Customer` should operate only on records linked by `Customers.UserId`.

| Module | Allowed routes | Ownership rule |
| --- | --- | --- |
| Customers | GET `/Customers/Details/{id}`, GET and POST `/Customers/Edit/{id}` | Only own profile |
| Addresses | GET `/Addresses`, GET `/Addresses/Details/{id}`, GET and POST `/Addresses/Create`, GET and POST `/Addresses/Edit/{id}`, POST `/Addresses/Delete/{id}` | Only own addresses |
| Stores | GET `/Stores`, GET `/Stores/Details/{id}` | Read only |
| Products | GET `/Products`, GET `/Products/Details/{id}` | Read only |
| Orders | GET `/Orders`, GET `/Orders/Details/{id}`, GET and POST `/Orders/Create` | Only own orders |
| OrderDetails | Through `/Orders/Details/{id}` | Only own orders |
| PaymentMethods | GET `/PaymentMethods`, GET `/PaymentMethods/Details/{id}` | Read only |
| Payments | GET `/Payments`, GET `/Payments/Details/{id}`, GET and POST `/Payments/Create` | Only own order payments |
| Reviews | GET `/Reviews`, GET `/Reviews/Details/{id}`, GET and POST `/Reviews/Create`, GET and POST `/Reviews/Edit/{id}`, POST `/Reviews/Delete/{id}` | Only own reviews |

Recommended restrictions:

- Do not allow creating stores, products, categories, statuses, or drivers.
- Do not allow editing orders after payment or after store confirmation unless a cancellation flow exists.
- Do not expose other customers, addresses, orders, payments, or reviews.

## Common CRUD route shape

Most business controllers use the same action names:

| Action | HTTP | Route |
| --- | --- | --- |
| Index | GET | `/{Controller}` |
| Details | GET | `/{Controller}/Details/{id}` |
| Create | GET | `/{Controller}/Create` |
| Create | POST | `/{Controller}/Create` |
| Edit | GET | `/{Controller}/Edit/{id}` |
| Edit | POST | `/{Controller}/Edit/{id}` |
| Delete | POST | `/{Controller}/Delete/{id}` |

`Delete` is a soft delete in services, using `IsActive = false`.

## Important fields

### Customer

| Field | Type | Required |
| --- | --- | --- |
| `FirstName` | string | Yes |
| `LastName` | string | Yes |
| `Email` | string | Yes |
| `Phone` | string | Yes |

### Address

| Field | Type | Required |
| --- | --- | --- |
| `CustomerId` | int | Yes |
| `Street` | string | Yes |
| `City` | string | Yes |
| `State` | string | Yes |
| `ZipCode` | string | Yes |
| `Country` | string | Yes |

### Store

| Field | Type | Required |
| --- | --- | --- |
| `CategoryId` | int | Yes |
| `Name` | string | Yes |
| `Description` | string | No |
| `Phone` | string | Yes |
| `Email` | string | Yes |
| `Address` | string | Yes |

### Product

| Field | Type | Required |
| --- | --- | --- |
| `StoreId` | int | Yes |
| `Name` | string | Yes |
| `Description` | string | No |
| `Price` | decimal | Yes |
| `Stock` | int | Yes |
| `ImageUrl` | string | No |

### Order

| Field | Type | Required |
| --- | --- | --- |
| `CustomerId` | int | Yes |
| `StoreId` | int | Yes |
| `DeliveryDriverId` | int | No |
| `OrderStatusId` | int | Yes |
| `AddressId` | int | Yes |
| `Items.ProductId` | int | Yes |
| `Items.Quantity` | int | Yes |

The order total is calculated server-side from product prices and quantities.

### Payment

| Field | Type | Required |
| --- | --- | --- |
| `OrderId` | int | Yes |
| `PaymentMethodId` | int | Yes |
| `Amount` | decimal | Yes |
| `PaymentDate` | datetime | Yes |
| `TransactionId` | string | No |
| `Status` | string | Yes |

### Review

| Field | Type | Required |
| --- | --- | --- |
| `CustomerId` | int | Yes |
| `StoreId` | int | Yes |
| `Rating` | int | Yes |
| `Comment` | string | No |

