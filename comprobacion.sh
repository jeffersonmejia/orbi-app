#!/usr/bin/env bash
set -euo pipefail

readonly CONTAINER_NAME="${CONTAINER_NAME:-orbi-postgres}"
readonly DB_USER="${DB_USER:-postgres}"
readonly DB_NAME="${DB_NAME:-OrbiDb}"

validate_container() {
    docker inspect -f '{{.State.Running}}' "${CONTAINER_NAME}" 2>/dev/null | grep -qx true
}

execute_count() {
    docker exec -i "${CONTAINER_NAME}" \
        psql -U "${DB_USER}" -d "${DB_NAME}" -v ON_ERROR_STOP=1 <<'SQL'
WITH conteo_tablas AS (
    SELECT 1 AS orden, 'StoreCategories' AS tabla, COUNT(*) AS cantidad FROM "StoreCategories"
    UNION ALL
    SELECT 2, 'OrderStatuses', COUNT(*) FROM "OrderStatuses"
    UNION ALL
    SELECT 3, 'PaymentMethods', COUNT(*) FROM "PaymentMethods"
    UNION ALL
    SELECT 4, 'Customers', COUNT(*) FROM "Customers"
    UNION ALL
    SELECT 5, 'Addresses', COUNT(*) FROM "Addresses"
    UNION ALL
    SELECT 6, 'DeliveryDrivers', COUNT(*) FROM "DeliveryDrivers"
    UNION ALL
    SELECT 7, 'Stores', COUNT(*) FROM "Stores"
    UNION ALL
    SELECT 8, 'Products', COUNT(*) FROM "Products"
    UNION ALL
    SELECT 9, 'Orders', COUNT(*) FROM "Orders"
    UNION ALL
    SELECT 10, 'OrderDetails', COUNT(*) FROM "OrderDetails"
    UNION ALL
    SELECT 11, 'Payments', COUNT(*) FROM "Payments"
    UNION ALL
    SELECT 12, 'Reviews', COUNT(*) FROM "Reviews"
)
SELECT tabla, cantidad
FROM (
    SELECT orden, tabla, cantidad FROM conteo_tablas
    UNION ALL
    SELECT 13, 'TOTAL', SUM(cantidad) FROM conteo_tablas
) AS resultado
ORDER BY orden;
SQL
}

main() {
    if ! validate_container; then
        printf 'El contenedor %s no existe o no está en ejecución.\n' "${CONTAINER_NAME}" >&2
        exit 1
    fi

    execute_count
}

main "$@"
