BEGIN;

INSERT INTO "StoreCategories" ("Id", "Name", "Description", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (1, 'Restaurante', 'Comidas preparadas, almuerzos, meriendas y bebidas.', true, now(), now()),
    (2, 'Farmacia', 'Medicinas de venta libre, cuidado personal y bienestar.', true, now(), now()),
    (3, 'Supermercado', 'Viveres, limpieza, bebidas y productos para el hogar.', true, now(), now()),
    (4, 'Panaderia', 'Pan, postres, bocaditos y productos horneados.', true, now(), now()),
    (5, 'Cafeteria', 'Cafe, bebidas frias, desayunos y snacks.', true, now(), now()),
    (6, 'Tienda de mascotas', 'Alimento, higiene y accesorios para mascotas.', true, now(), now()),
    (7, 'Tecnologia', 'Accesorios moviles, cables, cargadores y perifericos.', true, now(), now()),
    (8, 'Libreria', 'Utiles escolares, papeleria, libros y oficina.', true, now(), now()),
    (9, 'Floreria', 'Flores, arreglos, plantas y detalles.', true, now(), now()),
    (10, 'Ferreteria', 'Herramientas, electricidad, pintura y articulos de hogar.', true, now(), now())
ON CONFLICT ("Name") DO NOTHING;

INSERT INTO "OrderStatuses" ("Id", "Name", "Description", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (1, 'Pending', 'Order has been placed.', true, now(), now()),
    (2, 'Confirmed', 'Order was confirmed by the store.', true, now(), now()),
    (3, 'Preparing', 'Order is being prepared.', true, now(), now()),
    (4, 'In Transit', 'Order is on the way.', true, now(), now()),
    (5, 'Delivered', 'Order was delivered.', true, now(), now()),
    (6, 'Cancelled', 'Order was cancelled.', true, now(), now())
ON CONFLICT ("Name") DO NOTHING;

INSERT INTO "PaymentMethods" ("Id", "Name", "Description", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (1, 'Tarjeta de credito', 'Pago con tarjeta de credito.', true, now(), now()),
    (2, 'Tarjeta de debito', 'Pago con tarjeta de debito.', true, now(), now()),
    (3, 'Efectivo', 'Pago en efectivo al recibir el pedido.', true, now(), now()),
    (4, 'Billetera digital', 'Pago con billetera movil.', true, now(), now()),
    (5, 'Transferencia bancaria', 'Pago mediante transferencia bancaria.', true, now(), now()),
    (6, 'Saldo promocional', 'Pago con saldo promocional de la plataforma.', true, now(), now())
ON CONFLICT ("Name") DO NOTHING;

SELECT setval(pg_get_serial_sequence('"StoreCategories"', 'Id'), (SELECT max("Id") FROM "StoreCategories"));
SELECT setval(pg_get_serial_sequence('"OrderStatuses"', 'Id'), (SELECT max("Id") FROM "OrderStatuses"));
SELECT setval(pg_get_serial_sequence('"PaymentMethods"', 'Id'), (SELECT max("Id") FROM "PaymentMethods"));

COMMIT;
