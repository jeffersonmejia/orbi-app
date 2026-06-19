#!/usr/bin/env bash
set -euo pipefail

readonly CONTAINER_NAME="${CONTAINER_NAME:-orbi-postgres}"
readonly DB_NAME="${DB_NAME:-OrbiDb}"
readonly DB_USER="${DB_USER:-postgres}"
readonly DB_SCHEMA="${DB_SCHEMA:-public}"

print_header() {
    printf '\n%s\n' "Table totals for ${DB_NAME}.${DB_SCHEMA}"
    printf '%s\n\n' "Container: ${CONTAINER_NAME}"
}

ensure_docker_is_available() {
    if ! command -v docker >/dev/null 2>&1; then
        printf '%s\n' "Error: docker is not installed or not available in PATH." >&2
        exit 1
    fi
}

ensure_container_is_running() {
    local running

    if ! running="$(docker inspect -f '{{.State.Running}}' "${CONTAINER_NAME}" 2>/dev/null)"; then
        printf '%s\n' "Error: container '${CONTAINER_NAME}' was not found." >&2
        exit 1
    fi

    if [[ "${running}" != "true" ]]; then
        printf '%s\n' "Error: container '${CONTAINER_NAME}' is not running." >&2
        exit 1
    fi
}

run_psql() {
    docker exec "${CONTAINER_NAME}" \
        psql -U "${DB_USER}" -d "${DB_NAME}" -v ON_ERROR_STOP=1 "$@"
}

print_table_count_summary() {
    run_psql -Atc "
        SELECT COUNT(*)
        FROM information_schema.tables
        WHERE table_schema = '${DB_SCHEMA}'
          AND table_type = 'BASE TABLE';
    " | awk '{ printf "Total tables: %s\n\n", $1 }'
}

print_row_totals_by_table() {
    run_psql -c "
        WITH table_names AS (
            SELECT table_schema, table_name
            FROM information_schema.tables
            WHERE table_schema = '${DB_SCHEMA}'
              AND table_type = 'BASE TABLE'
        )
        SELECT table_name AS table,
               (xpath('/row/count/text()',
                    query_to_xml(
                        format('SELECT COUNT(*) AS count FROM %I.%I', table_schema, table_name),
                        false,
                        true,
                        ''
                    )
                ))[1]::text::bigint AS total_rows
        FROM table_names
        ORDER BY table_name;
    "
}

main() {
    ensure_docker_is_available
    ensure_container_is_running
    print_header
    print_table_count_summary
    print_row_totals_by_table
}

main "$@"
