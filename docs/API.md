# API

## Products

| Action | HTTP | Route | Description |
|--------|------|-------|-------------|
| Index | GET | `/Products` | List all active products |
| Details | GET | `/Products/{id}` | Get product by ID |
| Create | GET | `/Products/Create` | Show create form |
| Create | POST | `/Products/Create` | Create a new product |
| Edit | GET | `/Products/Edit/{id}` | Show edit form |
| Edit | POST | `/Products/Edit/{id}` | Update product |
| Delete | POST | `/Products/Delete/{id}` | Soft delete product |

### Product fields

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| StoreId | int | Yes | Foreign key to Store |
| Name | string | Yes | Max 200 characters |
| Description | string | No | Max 1000 characters |
| Price | decimal | Yes | Greater than zero |
| Stock | int | Yes | Non-negative |
| ImageUrl | string | No | Max 500 characters |

---

## Orders

| Action | HTTP | Route | Description |
|--------|------|-------|-------------|
| Index | GET | `/Orders` | List all active orders |
| Details | GET | `/Orders/{id}` | Get order by ID |
| Create | GET | `/Orders/Create` | Show create form |
| Create | POST | `/Orders/Create` | Create a new order |
| Edit | GET | `/Orders/Edit/{id}` | Show edit form |
| Edit | POST | `/Orders/Edit/{id}` | Update order |
| Delete | POST | `/Orders/Delete/{id}` | Soft delete order |

### Order fields

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| CustomerId | int | Yes | Foreign key to Customer |
| StoreId | int | Yes | Foreign key to Store |
| DeliveryDriverId | int | No | Assigned after creation |
| OrderStatusId | int | Yes | Default Pending |
| AddressId | int | Yes | Delivery address |

### Order items

Each order contains one or more items with:

| Field | Type | Notes |
|-------|------|-------|
| ProductId | int | Product to order |
| Quantity | int | Minimum 1 |

Total amount is calculated server-side from current product prices.
