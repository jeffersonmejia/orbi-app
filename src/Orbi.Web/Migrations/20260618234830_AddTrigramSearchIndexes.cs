using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbi.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTrigramSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.Sql("""CREATE EXTENSION IF NOT EXISTS pg_trgm;""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_Customers_FullName_Trgm" ON "Customers" USING gin (lower("FirstName" || ' ' || "LastName") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_Customers_Email_Trgm" ON "Customers" USING gin (lower("Email") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_DeliveryDrivers_FullName_Trgm" ON "DeliveryDrivers" USING gin (lower("FirstName" || ' ' || "LastName") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_DeliveryDrivers_Email_Trgm" ON "DeliveryDrivers" USING gin (lower("Email") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_Stores_Name_Trgm" ON "Stores" USING gin (lower("Name") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_Products_Name_Trgm" ON "Products" USING gin (lower("Name") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_StoreCategories_Name_Trgm" ON "StoreCategories" USING gin (lower("Name") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_OrderStatuses_Name_Trgm" ON "OrderStatuses" USING gin (lower("Name") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_PaymentMethods_Name_Trgm" ON "PaymentMethods" USING gin (lower("Name") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_Payments_Status_Trgm" ON "Payments" USING gin (lower("Status") gin_trgm_ops);""");
            migrationBuilder.Sql("""CREATE INDEX IF NOT EXISTS "IX_Addresses_City_Trgm" ON "Addresses" USING gin (lower("City") gin_trgm_ops);""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_Addresses_City_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_Payments_Status_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_PaymentMethods_Name_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_OrderStatuses_Name_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_StoreCategories_Name_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_Products_Name_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_Stores_Name_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_DeliveryDrivers_Email_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_DeliveryDrivers_FullName_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_Customers_Email_Trgm";""");
            migrationBuilder.Sql("""DROP INDEX IF EXISTS "IX_Customers_FullName_Trgm";""");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
        }
    }
}
