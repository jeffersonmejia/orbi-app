BEGIN;

TRUNCATE TABLE
    "OrderDetails",
    "Payments",
    "Reviews",
    "Orders",
    "Products",
    "Addresses",
    "Stores",
    "DeliveryDrivers",
    "Customers",
    "PaymentMethods",
    "OrderStatuses",
    "StoreCategories"
RESTART IDENTITY CASCADE;

COMMIT;

