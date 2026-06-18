BEGIN;

INSERT INTO "OrderDetails" ("OrderId", "ProductId", "Quantity", "UnitPrice", "Subtotal", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    o."Id",
    p."Id",
    q.quantity,
    p."Price",
    (q.quantity * p."Price")::numeric(18,2),
    true,
    o."CreatedAt",
    now()
FROM "Orders" o
CROSS JOIN LATERAL generate_series(1, CASE WHEN o."Id" <= 50000 THEN 2 ELSE 1 END) AS d(detail_no)
CROSS JOIN LATERAL (
    SELECT 1 + ((o."Id" + d.detail_no) % 4) AS quantity
) q
JOIN "Products" p
    ON p."Id" = o."StoreId" + (2000 * ((o."Id" + d.detail_no) % 12));

UPDATE "Orders" o
SET
    "TotalAmount" = totals.total_amount,
    "UpdatedAt" = now()
FROM (
    SELECT "OrderId", sum("Subtotal")::numeric(18,2) AS total_amount
    FROM "OrderDetails"
    GROUP BY "OrderId"
) totals
WHERE totals."OrderId" = o."Id";

COMMIT;

