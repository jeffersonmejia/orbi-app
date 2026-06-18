BEGIN;

INSERT INTO "Payments" ("OrderId", "PaymentMethodId", "Amount", "PaymentDate", "TransactionId", "Status", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    o."Id",
    ((o."Id" - 1) % 6) + 1,
    o."TotalAmount",
    o."OrderDate" + interval '3 minutes',
    CASE WHEN o."OrderStatusId" IN (1, 6) THEN NULL ELSE 'TXN-SEED-' || lpad(o."Id"::text, 8, '0') END,
    CASE
        WHEN o."OrderStatusId" = 1 THEN 'Pending'
        WHEN o."OrderStatusId" = 6 THEN 'Failed'
        ELSE 'Completed'
    END,
    true,
    o."CreatedAt",
    now()
FROM "Orders" o;

INSERT INTO "Reviews" ("CustomerId", "StoreId", "Rating", "Comment", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    ((g - 1) % 40000) + 1,
    (((g * 37) - 1) % 2000) + 1,
    1 + (g % 5),
    'Review seed ' || g || ' para pruebas de consultas y promedios.',
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 27978) AS g;

COMMIT;

