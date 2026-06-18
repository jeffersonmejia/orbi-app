## Distribución propuesta

Total exacto: 600,000 registros de negocio.

| No. | Tabla           | Registros |
| --: | --------------- | --------: |
|   1 | StoreCategories |        10 |
|   2 | OrderStatuses   |         6 |
|   3 | PaymentMethods  |         6 |
|   4 | Customers       |    40,000 |
|   5 | Addresses       |    60,000 |
|   6 | DeliveryDrivers |     5,000 |
|   7 | Stores          |     2,000 |
|   8 | Products        |    25,000 |
|   9 | Orders          |   130,000 |
|  10 | OrderDetails    |   180,000 |
|  11 | Payments        |   130,000 |
|  12 | Reviews         |    27,978 |

1. `db/00_reset.sql`
   - Limpiar tablas de negocio con `TRUNCATE ... RESTART IDENTITY CASCADE`.
   - No tocar tablas `AspNet*` salvo que se decida incluir usuarios de prueba.

2. `db/01_catalogs.sql`
   - Insertar categorias, estados de orden y metodos de pago.
   - Usar `ON CONFLICT DO NOTHING` para poder reejecutar.

3. `db/02_people.sql`
   - Insertar `Customers`, `Addresses` y `DeliveryDrivers`.
   - Emails unicos por índice.
   - Coordenadas alrededor de Bogota para mantener coherencia con seeds actuales.

4. `db/03_stores_products.sql`
   - Insertar `Stores` ligados a categorias.
   - Insertar `Products` distribuidos por tienda.
   - Precios, stock e imagenes generados por categoria/índice.

5. `db/04_orders.sql`
   - Insertar `Orders` enlazando cliente, tienda, direccion, estado y repartidor.
   - Fechas repartidas en los ultimos 12 meses.
   - `DeliveryDriverId` nullable para ordenes pendientes/canceladas.

6. `db/05_order_details.sql`
   - Insertar entre 1 y 2 detalles por orden hasta llegar a 180k.
   - Tomar productos de la misma tienda de la orden.
   - Calcular `Quantity`, `UnitPrice` y `Subtotal`.

7. `db/06_payments_reviews.sql`
   - Insertar un pago por orden.
   - `Amount` debe coincidir con el total de la orden.
   - Insertar reviews por combinaciones cliente/tienda sin duplicar patrón innecesariamente.

8. `db/99_validate.sql`
   - Contar registros por tabla.
   - Validar FKs principales.
   - Validar `Orders.TotalAmount = SUM(OrderDetails.Subtotal)`.
   - Validar que `Payments.Amount = Orders.TotalAmount`.
