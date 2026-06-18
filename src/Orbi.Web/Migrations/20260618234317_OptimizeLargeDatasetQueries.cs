using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbi.Web.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeLargeDatasetQueries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_CategoryId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_StoreId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentMethodId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_CustomerId",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CategoryId_IsActive",
                table: "Stores",
                columns: new[] { "CategoryId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_IsActive_Name",
                table: "Stores",
                columns: new[] { "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_StoreCategories_IsActive_Name",
                table: "StoreCategories",
                columns: new[] { "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId_IsActive",
                table: "Reviews",
                columns: new[] { "CustomerId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IsActive_UpdatedAt",
                table: "Reviews",
                columns: new[] { "IsActive", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StoreId_IsActive",
                table: "Reviews",
                columns: new[] { "StoreId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive_Name",
                table: "Products",
                columns: new[] { "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId_IsActive_Name",
                table: "Products",
                columns: new[] { "StoreId", "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_IsActive_PaymentDate",
                table: "Payments",
                columns: new[] { "IsActive", "PaymentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_IsActive_Status",
                table: "Payments",
                columns: new[] { "IsActive", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodId_IsActive",
                table: "Payments",
                columns: new[] { "PaymentMethodId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_IsActive_Name",
                table: "PaymentMethods",
                columns: new[] { "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatuses_IsActive_Name",
                table: "OrderStatuses",
                columns: new[] { "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsActive_CustomerId",
                table: "Orders",
                columns: new[] { "IsActive", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsActive_DeliveryDriverId",
                table: "Orders",
                columns: new[] { "IsActive", "DeliveryDriverId" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsActive_OrderDate",
                table: "Orders",
                columns: new[] { "IsActive", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsActive_OrderStatusId",
                table: "Orders",
                columns: new[] { "IsActive", "OrderStatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsActive_StoreId",
                table: "Orders",
                columns: new[] { "IsActive", "StoreId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId_IsActive",
                table: "OrderDetails",
                columns: new[] { "OrderId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId_IsActive",
                table: "OrderDetails",
                columns: new[] { "ProductId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDrivers_IsActive_Email",
                table: "DeliveryDrivers",
                columns: new[] { "IsActive", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDrivers_IsActive_IsAvailable",
                table: "DeliveryDrivers",
                columns: new[] { "IsActive", "IsAvailable" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDrivers_IsActive_LastName_FirstName",
                table: "DeliveryDrivers",
                columns: new[] { "IsActive", "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IsActive_Email",
                table: "Customers",
                columns: new[] { "IsActive", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IsActive_LastName_FirstName",
                table: "Customers",
                columns: new[] { "IsActive", "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId_IsActive",
                table: "Addresses",
                columns: new[] { "CustomerId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_IsActive_City_Street",
                table: "Addresses",
                columns: new[] { "IsActive", "City", "Street" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_CategoryId_IsActive",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_IsActive_Name",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_StoreCategories_IsActive_Name",
                table: "StoreCategories");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CustomerId_IsActive",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_IsActive_UpdatedAt",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_StoreId_IsActive",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsActive_Name",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_StoreId_IsActive_Name",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Payments_IsActive_PaymentDate",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_IsActive_Status",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentMethodId_IsActive",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_IsActive_Name",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_OrderStatuses_IsActive_Name",
                table: "OrderStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Orders_IsActive_CustomerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_IsActive_DeliveryDriverId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_IsActive_OrderDate",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_IsActive_OrderStatusId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_IsActive_StoreId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderId_IsActive",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductId_IsActive",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryDrivers_IsActive_Email",
                table: "DeliveryDrivers");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryDrivers_IsActive_IsAvailable",
                table: "DeliveryDrivers");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryDrivers_IsActive_LastName_FirstName",
                table: "DeliveryDrivers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_IsActive_Email",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_IsActive_LastName_FirstName",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_CustomerId_IsActive",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_IsActive_City_Street",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CategoryId",
                table: "Stores",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StoreId",
                table: "Reviews",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodId",
                table: "Payments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId",
                table: "Addresses",
                column: "CustomerId");
        }
    }
}
