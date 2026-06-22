BEGIN;

INSERT INTO "Customers" ("FirstName", "LastName", "Email", "Phone", "IsActive", "CreatedAt", "UpdatedAt")
WITH names AS (
    SELECT
        ARRAY['Ana','Carlos','Maria','Jose','Luis','Daniela','Jorge','Valeria','Miguel','Camila','Andres','Paola','Diego','Gabriela','Sofia','Ricardo','Fernanda','David','Karla','Mateo'] AS first_names,
        ARRAY['Vera','Mendoza','Zambrano','Cedeno','Mora','Alvarado','Castro','Paredes','Loor','Sanchez','Torres','Molina','Bravo','Reyes','Leon','Navarrete','Cruz','Delgado','Coronel','Santos'] AS last_names,
        ARRAY['gmail.com','hotmail.com','outlook.com','yahoo.com','icloud.com'] AS domains
)
SELECT
    first_names[((g - 1) % array_length(first_names, 1)) + 1],
    last_names[((g * 7 - 1) % array_length(last_names, 1)) + 1] || ' ' || last_names[((g * 11 - 1) % array_length(last_names, 1)) + 1],
    lower(first_names[((g - 1) % array_length(first_names, 1)) + 1] || '.' ||
          last_names[((g * 7 - 1) % array_length(last_names, 1)) + 1] ||
          lpad(g::text, 5, '0') || '@' ||
          domains[((g - 1) % array_length(domains, 1)) + 1]),
    '09' || lpad((70000000 + g)::text, 8, '0'),
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 40000) AS g
CROSS JOIN names;

INSERT INTO "Addresses" ("CustomerId", "Street", "City", "State", "ZipCode", "Country", "Latitude", "Longitude", "IsActive", "CreatedAt", "UpdatedAt")
WITH address_data AS (
    SELECT
        ARRAY['Alborada','Sauces','Garzota','Kennedy','Urdesa','Ceibos','Samanes','Guayacanes','Mucho Lote','Centro','Puerto Santa Ana','Mapasingue','Floresta','Suburbio','Via a la Costa'] AS sectors,
        ARRAY['Av. Francisco de Orellana','Av. Las Monjas','Av. Victor Emilio Estrada','Av. Juan Tanca Marengo','Av. de las Americas','Calle 9 de Octubre','Av. Isidro Ayora','Av. Carlos Julio Arosemena','Av. Casuarina','Calle Boyaca'] AS streets
)
SELECT
    ((g - 1) % 40000) + 1,
    streets[((g - 1) % array_length(streets, 1)) + 1] || ', ' ||
        sectors[((g * 3 - 1) % array_length(sectors, 1)) + 1] ||
        ', mz. ' || ((g % 180) + 1) || ', villa ' || ((g % 40) + 1),
    'Guayaquil',
    'Guayas',
    '090' || lpad((100 + (g % 899))::text, 3, '0'),
    'Ecuador',
    -2.24 + ((g % 1800)::numeric / 10000.0),
    -79.96 + ((g % 2200)::numeric / 10000.0),
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 60000) AS g
CROSS JOIN address_data;

INSERT INTO "DeliveryDrivers" ("FirstName", "LastName", "Email", "Phone", "CurrentLatitude", "CurrentLongitude", "LastLocationUpdate", "IsAvailable", "IsActive", "CreatedAt", "UpdatedAt")
WITH driver_names AS (
    SELECT
        ARRAY['Kevin','Bryan','Jonathan','Steven','Anthony','Cristian','Eduardo','Marco','Joel','Fernando','Adrian','Henry','Sebastian','Nicolas','Roberto'] AS first_names,
        ARRAY['Villacis','Preciado','Macias','Mero','Quimis','Cantos','Anchundia','Baque','Zuniga','Cevallos','Tumbaco','Solorzano','Viteri','Espinoza','Palacios'] AS last_names
)
SELECT
    first_names[((g - 1) % array_length(first_names, 1)) + 1],
    last_names[((g * 5 - 1) % array_length(last_names, 1)) + 1],
    lower(first_names[((g - 1) % array_length(first_names, 1)) + 1] || '.' ||
          last_names[((g * 5 - 1) % array_length(last_names, 1)) + 1] ||
          lpad(g::text, 4, '0') || '@repartosorbi.com'),
    '09' || lpad((60000000 + g)::text, 8, '0'),
    -2.24 + ((g % 1800)::numeric / 10000.0),
    -79.96 + ((g % 2200)::numeric / 10000.0),
    now() - ((g % 240) || ' minutes')::interval,
    (g % 5) <> 0,
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 5000) AS g
CROSS JOIN driver_names;

COMMIT;
