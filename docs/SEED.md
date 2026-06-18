# Seed plan

## Proposed distribution

Exact total: 600,000 business records.

| No. | Table | Records |
| --: | --- | ---: |
| 1 | StoreCategories | 10 |
| 2 | OrderStatuses | 6 |
| 3 | PaymentMethods | 6 |
| 4 | Customers | 40,000 |
| 5 | Addresses | 60,000 |
| 6 | DeliveryDrivers | 5,000 |
| 7 | Stores | 2,000 |
| 8 | Products | 25,000 |
| 9 | Orders | 130,000 |
| 10 | OrderDetails | 180,000 |
| 11 | Payments | 130,000 |
| 12 | Reviews | 27,978 |

## Seed files

`db/00_reset.sql`

- Clears business tables with `TRUNCATE ... RESTART IDENTITY CASCADE`.
- Does not touch `AspNet*` tables unless test users are added later.

`db/01_catalogs.sql`

- Inserts store categories, order statuses, and payment methods.
- Uses `ON CONFLICT DO NOTHING` so the script can be rerun safely.

`db/02_people.sql`

- Inserts `Customers`, `Addresses`, and `DeliveryDrivers`.
- Generates unique emails by index.
- Uses coordinates around Bogota to stay consistent with the current seed data.

`db/03_stores_products.sql`

- Inserts `Stores` linked to categories.
- Inserts `Products` distributed across stores.
- Generates prices, stock, and image paths by category and index.

`db/04_orders.sql`

- Inserts `Orders` linked to customers, stores, addresses, statuses, and drivers.
- Spreads dates across the last 12 months.
- Keeps `DeliveryDriverId` nullable for pending and cancelled orders.

`db/05_order_details.sql`

- Inserts 1 or 2 details per order until reaching 180k details.
- Selects products from the same store as the order.
- Calculates `Quantity`, `UnitPrice`, and `Subtotal`.

`db/06_payments_reviews.sql`

- Inserts one payment per order.
- Keeps `Amount` aligned with the order total.
- Inserts reviews using customer and store combinations.

`db/99_validate.sql`

- Counts records by table.
- Validates key relationships.
- Validates `Orders.TotalAmount = SUM(OrderDetails.Subtotal)`.
- Validates `Payments.Amount = Orders.TotalAmount`.

## Execution order

Run the scripts in this order to avoid foreign key conflicts:

```bash
psql "$DATABASE_URL" -f db/00_reset.sql
psql "$DATABASE_URL" -f db/01_catalogs.sql
psql "$DATABASE_URL" -f db/02_people.sql
psql "$DATABASE_URL" -f db/03_stores_products.sql
psql "$DATABASE_URL" -f db/04_orders.sql
psql "$DATABASE_URL" -f db/05_order_details.sql
psql "$DATABASE_URL" -f db/06_payments_reviews.sql
psql "$DATABASE_URL" -f db/99_validate.sql
```

