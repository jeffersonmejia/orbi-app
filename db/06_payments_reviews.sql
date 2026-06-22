BEGIN;

INSERT INTO "Payments" ("OrderId", "PaymentMethodId", "Amount", "PaymentDate", "TransactionId", "Status", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    o."Id",
    ((o."Id" - 1) % 6) + 1,
    o."TotalAmount",
    o."OrderDate" + interval '3 minutes',
    CASE WHEN o."OrderStatusId" IN (1, 6) THEN NULL ELSE 'ORB-' || to_char(o."OrderDate", 'YYYYMMDD') || '-' || lpad(o."Id"::text, 7, '0') END,
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
WITH comments AS (
    SELECT ARRAY[
        'El pedido llego completo y a tiempo.',
        'Buena atencion y productos en buen estado.',
        'La entrega fue rapida para la zona.',
        'El comercio confirmo el pedido sin demora.',
        'Precios razonables y buena presentacion.',
        'Volveria a pedir en este local.',
        'El repartidor encontro la direccion sin problema.',
        'Producto fresco y bien empacado.',
        'La compra fue sencilla desde la aplicacion.',
        'Servicio correcto para una entrega de barrio.'
    ] AS review_texts
)
SELECT
    ((g - 1) % 40000) + 1,
    (((g * 37) - 1) % 2000) + 1,
    CASE WHEN (g % 10) = 0 THEN 3 WHEN (g % 4) = 0 THEN 4 ELSE 5 END,
    review_texts[((g - 1) % array_length(review_texts, 1)) + 1],
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 27978) AS g
CROSS JOIN comments;

COMMIT;
