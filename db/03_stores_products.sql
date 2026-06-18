BEGIN;

INSERT INTO "Stores" ("CategoryId", "Name", "Description", "Phone", "Email", "Address", "Latitude", "Longitude", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    ((g - 1) % 10) + 1,
    'Tienda Orbi ' || g,
    'Comercio seed de categoria ' || (((g - 1) % 10) + 1),
    '601' || lpad((3000000 + g)::text, 7, '0'),
    'store' || g || '@seed.orbi.local',
    'Carrera ' || ((g % 120) + 1) || ' # ' || ((g % 80) + 1) || '-' || ((g % 70) + 1),
    4.55 + ((g % 2500)::numeric / 10000.0),
    -74.20 + ((g % 2500)::numeric / 10000.0),
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 2000) AS g;

INSERT INTO "Products" ("StoreId", "Name", "Description", "Price", "Stock", "ImageUrl", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    ((g - 1) % 2000) + 1,
    'Producto ' || g,
    'Producto seed para pruebas de carga y paginacion.',
    (2500 + ((g % 220) * 350))::numeric(18,2),
    10 + (g % 500),
    '/images/seeds/product-' || ((g % 50) + 1) || '.jpg',
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 25000) AS g;

COMMIT;

