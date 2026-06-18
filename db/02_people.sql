BEGIN;

INSERT INTO "Customers" ("FirstName", "LastName", "Email", "Phone", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    'Cliente ' || g,
    'Orbi ' || ((g % 997) + 1),
    'cliente' || g || '@seed.orbi.local',
    '310' || lpad((1000000 + g)::text, 7, '0'),
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 40000) AS g;

INSERT INTO "Addresses" ("CustomerId", "Street", "City", "State", "ZipCode", "Country", "Latitude", "Longitude", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    ((g - 1) % 40000) + 1,
    'Calle ' || ((g % 170) + 1) || ' # ' || ((g % 90) + 1) || '-' || ((g % 80) + 1),
    'Bogota',
    'Cundinamarca',
    lpad((110000 + (g % 999))::text, 6, '0'),
    'Colombia',
    4.55 + ((g % 2500)::numeric / 10000.0),
    -74.20 + ((g % 2500)::numeric / 10000.0),
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 60000) AS g;

INSERT INTO "DeliveryDrivers" ("FirstName", "LastName", "Email", "Phone", "CurrentLatitude", "CurrentLongitude", "LastLocationUpdate", "IsAvailable", "IsActive", "CreatedAt", "UpdatedAt")
SELECT
    'Repartidor ' || g,
    'Zona ' || ((g % 50) + 1),
    'driver' || g || '@seed.orbi.local',
    '300' || lpad((2000000 + g)::text, 7, '0'),
    4.55 + ((g % 2500)::numeric / 10000.0),
    -74.20 + ((g % 2500)::numeric / 10000.0),
    now() - ((g % 240) || ' minutes')::interval,
    (g % 5) <> 0,
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 5000) AS g;

COMMIT;

