WITH counts AS (
    SELECT 'StoreCategories' AS table_name, count(*)::bigint AS total FROM "StoreCategories"
    UNION ALL SELECT 'OrderStatuses', count(*) FROM "OrderStatuses"
    UNION ALL SELECT 'PaymentMethods', count(*) FROM "PaymentMethods"
    UNION ALL SELECT 'Customers', count(*) FROM "Customers"
    UNION ALL SELECT 'Addresses', count(*) FROM "Addresses"
    UNION ALL SELECT 'DeliveryDrivers', count(*) FROM "DeliveryDrivers"
    UNION ALL SELECT 'Stores', count(*) FROM "Stores"
    UNION ALL SELECT 'Products', count(*) FROM "Products"
    UNION ALL SELECT 'Orders', count(*) FROM "Orders"
    UNION ALL SELECT 'OrderDetails', count(*) FROM "OrderDetails"
    UNION ALL SELECT 'Payments', count(*) FROM "Payments"
    UNION ALL SELECT 'Reviews', count(*) FROM "Reviews"
)
SELECT * FROM counts
UNION ALL
SELECT 'TOTAL', sum(total) FROM counts
ORDER BY table_name;

SELECT count(*) AS orders_without_details
FROM "Orders" o
LEFT JOIN "OrderDetails" od ON od."OrderId" = o."Id"
WHERE od."Id" IS NULL;

SELECT count(*) AS orders_with_wrong_total
FROM "Orders" o
JOIN (
    SELECT "OrderId", sum("Subtotal")::numeric(18,2) AS total
    FROM "OrderDetails"
    GROUP BY "OrderId"
) od ON od."OrderId" = o."Id"
WHERE o."TotalAmount" <> od.total;

SELECT count(*) AS payments_with_wrong_amount
FROM "Payments" p
JOIN "Orders" o ON o."Id" = p."OrderId"
WHERE p."Amount" <> o."TotalAmount";

