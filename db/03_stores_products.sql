BEGIN;

INSERT INTO "Stores" ("CategoryId", "Name", "Description", "Phone", "Email", "Address", "Latitude", "Longitude", "IsActive", "CreatedAt", "UpdatedAt")
WITH store_data AS (
    SELECT
        ARRAY['Sabor Costeno','La Esquina Criolla','Parrilla del Norte','Don Ceviche','Almuerzos La 9'] AS restaurant_names,
        ARRAY['Farmacia Central','Botica Salud','Farmacia Vida','Botica del Barrio','Farmacia San Jose'] AS pharmacy_names,
        ARRAY['Market Express','Despensa Familiar','Mini Market Sol','Super Ahorro','Tienda La Vecina'] AS supermarket_names,
        ARRAY['Pan Caliente','Dulce Horno','Panaderia Victoria','La Espiga','Horno de Casa'] AS bakery_names,
        ARRAY['Cafe Puerto','Taza Nueve','Cafe La Plaza','Buen Cafe','Cafeteria Orilla'] AS coffee_names,
        ARRAY['Pet House','Mundo Mascota','Patitas Market','Pet Barrio','Animalia Express'] AS pet_names,
        ARRAY['Tecno Movil','Click Accesorios','Zona Tech','Digital Express','Smart Store'] AS tech_names,
        ARRAY['Libreria Central','Papel y Tinta','Utiles Express','Mundo Escolar','Oficina Plus'] AS bookstore_names,
        ARRAY['Flores del Rio','Floristeria Bella','Detalles Verdes','Rosas y Mas','Jardin Express'] AS florist_names,
        ARRAY['Ferreteria Union','Herramientas Norte','Casa Tornillo','Pinturas y Mas','Ferre Express'] AS hardware_names,
        ARRAY['Alborada','Sauces','Garzota','Kennedy','Urdesa','Ceibos','Samanes','Guayacanes','Centro','Via a la Costa'] AS sectors,
        ARRAY['Av. Francisco de Orellana','Av. Juan Tanca Marengo','Av. de las Americas','Av. Victor Emilio Estrada','Av. Isidro Ayora','Calle 9 de Octubre'] AS streets
)
SELECT
    ((g - 1) % 10) + 1 AS category_id,
    CASE ((g - 1) % 10) + 1
        WHEN 1 THEN restaurant_names[((g - 1) % array_length(restaurant_names, 1)) + 1]
        WHEN 2 THEN pharmacy_names[((g - 1) % array_length(pharmacy_names, 1)) + 1]
        WHEN 3 THEN supermarket_names[((g - 1) % array_length(supermarket_names, 1)) + 1]
        WHEN 4 THEN bakery_names[((g - 1) % array_length(bakery_names, 1)) + 1]
        WHEN 5 THEN coffee_names[((g - 1) % array_length(coffee_names, 1)) + 1]
        WHEN 6 THEN pet_names[((g - 1) % array_length(pet_names, 1)) + 1]
        WHEN 7 THEN tech_names[((g - 1) % array_length(tech_names, 1)) + 1]
        WHEN 8 THEN bookstore_names[((g - 1) % array_length(bookstore_names, 1)) + 1]
        WHEN 9 THEN florist_names[((g - 1) % array_length(florist_names, 1)) + 1]
        ELSE hardware_names[((g - 1) % array_length(hardware_names, 1)) + 1]
    END || ' ' || sectors[((g * 3 - 1) % array_length(sectors, 1)) + 1],
    CASE ((g - 1) % 10) + 1
        WHEN 1 THEN 'Comidas preparadas para almuerzo, merienda y pedidos familiares.'
        WHEN 2 THEN 'Medicinas de venta libre, vitaminas y cuidado personal.'
        WHEN 3 THEN 'Viveres, bebidas, limpieza y productos basicos para el hogar.'
        WHEN 4 THEN 'Pan fresco, dulces, tortas pequenas y bocaditos.'
        WHEN 5 THEN 'Cafe, jugos, desayunos y snacks para llevar.'
        WHEN 6 THEN 'Alimentos, arena, juguetes y cuidado para mascotas.'
        WHEN 7 THEN 'Cargadores, audifonos, cables y accesorios moviles.'
        WHEN 8 THEN 'Utiles escolares, papeleria, copias y articulos de oficina.'
        WHEN 9 THEN 'Arreglos florales, plantas pequenas y detalles.'
        ELSE 'Herramientas menores, pintura, focos y articulos para reparaciones.'
    END,
    '04' || lpad((2000000 + g)::text, 7, '0'),
    'local' || lpad(g::text, 4, '0') || '@orbi.ec',
    streets[((g - 1) % array_length(streets, 1)) + 1] || ', ' ||
        sectors[((g * 3 - 1) % array_length(sectors, 1)) + 1] ||
        ', local ' || ((g % 90) + 1),
    -2.24 + ((g % 1800)::numeric / 10000.0),
    -79.96 + ((g % 2200)::numeric / 10000.0),
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM generate_series(1, 2000) AS g
CROSS JOIN store_data;

INSERT INTO "Products" ("StoreId", "Name", "Description", "Price", "Stock", "ImageUrl", "IsActive", "CreatedAt", "UpdatedAt")
WITH product_seed AS (
    SELECT
        ARRAY['Almuerzo ejecutivo','Seco de pollo','Encebollado','Arroz con menestra','Hamburguesa sencilla','Ceviche mixto','Chaulafan personal','Pollo asado porcion'] AS restaurant_products,
        ARRAY['Paracetamol 500mg','Ibuprofeno 400mg','Alcohol antiseptico','Vitamina C','Suero oral','Curitas','Gel antibacterial','Protector solar pequeno'] AS pharmacy_products,
        ARRAY['Arroz 1 kg','Leche 1 litro','Huevos docena','Aceite 1 litro','Azucar 1 kg','Atun lata','Detergente 500g','Agua 1 litro'] AS supermarket_products,
        ARRAY['Pan enrollado','Pan de yuca','Croissant','Torta porcion','Empanada de queso','Donut glaseada','Pan integral','Bocadito mixto'] AS bakery_products,
        ARRAY['Cafe americano','Capuchino','Jugo natural','Sanduche mixto','Tostada con queso','Chocolate caliente','Te helado','Muffin de vainilla'] AS coffee_products,
        ARRAY['Alimento perro 1 kg','Alimento gato 1 kg','Arena sanitaria','Shampoo mascota','Correa sencilla','Snack dental','Plato plastico','Juguete pequeno'] AS pet_products,
        ARRAY['Cable USB C','Cargador pared','Audifonos basicos','Protector pantalla','Mouse inalambrico','Memoria USB 32GB','Adaptador HDMI','Soporte celular'] AS tech_products,
        ARRAY['Cuaderno universitario','Lapicero azul','Resma papel A4','Carpeta plastica','Marcadores paquete','Corrector liquido','Regla 30 cm','Block notas'] AS bookstore_products,
        ARRAY['Ramo pequeno','Rosa individual','Girasol individual','Planta suculenta','Arreglo sencillo','Tarjeta dedicatoria','Globo decorativo','Base pequena'] AS florist_products,
        ARRAY['Foco LED','Cinta aislante','Brocha 2 pulgadas','Tornillos paquete','Candado pequeno','Pilas AA','Silicona barra','Guantes trabajo'] AS hardware_products
),
product_rows AS (
    SELECT
        g,
        ((g - 1) % 2000) + 1 AS store_id,
        ((((g - 1) % 2000)) % 10) + 1 AS category_id
    FROM generate_series(1, 25000) AS g
)
SELECT
    store_id,
    CASE category_id
        WHEN 1 THEN restaurant_products[((g - 1) % array_length(restaurant_products, 1)) + 1]
        WHEN 2 THEN pharmacy_products[((g - 1) % array_length(pharmacy_products, 1)) + 1]
        WHEN 3 THEN supermarket_products[((g - 1) % array_length(supermarket_products, 1)) + 1]
        WHEN 4 THEN bakery_products[((g - 1) % array_length(bakery_products, 1)) + 1]
        WHEN 5 THEN coffee_products[((g - 1) % array_length(coffee_products, 1)) + 1]
        WHEN 6 THEN pet_products[((g - 1) % array_length(pet_products, 1)) + 1]
        WHEN 7 THEN tech_products[((g - 1) % array_length(tech_products, 1)) + 1]
        WHEN 8 THEN bookstore_products[((g - 1) % array_length(bookstore_products, 1)) + 1]
        WHEN 9 THEN florist_products[((g - 1) % array_length(florist_products, 1)) + 1]
        ELSE hardware_products[((g - 1) % array_length(hardware_products, 1)) + 1]
    END,
    CASE category_id
        WHEN 1 THEN 'Producto preparado por el restaurante para entrega a domicilio.'
        WHEN 2 THEN 'Producto de farmacia para compra rapida y entrega local.'
        WHEN 3 THEN 'Producto basico de supermercado para consumo diario.'
        WHEN 4 THEN 'Producto fresco de panaderia para consumo el mismo dia.'
        WHEN 5 THEN 'Bebida o snack preparado por la cafeteria.'
        WHEN 6 THEN 'Producto para cuidado o alimentacion de mascotas.'
        WHEN 7 THEN 'Accesorio tecnologico de uso cotidiano.'
        WHEN 8 THEN 'Articulo de libreria, estudio u oficina.'
        WHEN 9 THEN 'Detalle floral disponible para entrega local.'
        ELSE 'Articulo de ferreteria para reparaciones domesticas.'
    END,
    CASE category_id
        WHEN 1 THEN (2.50 + ((g % 24) * 0.25))
        WHEN 2 THEN (1.00 + ((g % 28) * 0.30))
        WHEN 3 THEN (0.65 + ((g % 22) * 0.20))
        WHEN 4 THEN (0.35 + ((g % 14) * 0.15))
        WHEN 5 THEN (1.00 + ((g % 16) * 0.20))
        WHEN 6 THEN (2.00 + ((g % 30) * 0.45))
        WHEN 7 THEN (2.50 + ((g % 45) * 0.50))
        WHEN 8 THEN (0.45 + ((g % 24) * 0.20))
        WHEN 9 THEN (2.00 + ((g % 32) * 0.50))
        ELSE (0.75 + ((g % 28) * 0.35))
    END::numeric(18,2),
    8 + (g % 80),
    '/images/products/product-' || lpad(((g % 60) + 1)::text, 2, '0') || '.jpg',
    true,
    now() - ((g % 365) || ' days')::interval,
    now()
FROM product_rows
CROSS JOIN product_seed;

COMMIT;
