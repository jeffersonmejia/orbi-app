BEGIN;

INSERT INTO "Orders" ("CustomerId", "StoreId", "DeliveryDriverId", "OrderStatusId", "AddressId", "TotalAmount", "OrderDate", "DeliveryDate", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    ((g - 1) % 40000) + 1 AS customer_id,
    ((g - 1) % 2000) + 1 AS store_id,
    CASE WHEN (((g - 1) % 6) + 1) IN (1, 6) THEN NULL ELSE (((g - 1) % 5000) + 1) END AS driver_id,
    ((g - 1) % 6) + 1 AS status_id,
    ((g - 1) % 40000) + 1 AS address_id,
    0::numeric(18,2),
    now() - ((g % 365) || ' days')::interval - ((g % 24) || ' hours')::interval,
    CASE WHEN (((g - 1) % 6) + 1) = 5
        THEN now() - ((g % 365) || ' days')::interval - ((g % 24) || ' hours')::interval + interval '55 minutes'
        ELSE NULL
    END,
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 130000) AS g;

COMMIT;

