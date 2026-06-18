BEGIN;

INSERT INTO "StoreCategories" ("Id", "Name", "Description", "IsActive", "CreatedAt", "UpdatedAt")
VALUES
    (1, 'Restaurant', 'Prepared meals and beverages.', true, now(), now()),
    (2, 'Pharmacy', 'Medicine and wellness products.', true, now(), now()),
    (3, 'Supermarket', 'Groceries and household items.', true, now(), now()),
    (4, 'Bakery', 'Bread, pastries and desserts.', true, now(), now()),
    (5, 'Coffee Shop', 'Coffee, drinks and snacks.', true, now(), now()),
    (6, 'Pet Store', 'Pet food and accessories.', true, now(), now()),
    (7, 'Electronics', 'Small electronics and accessories.', true, now(), now()),
    (8, 'Bookstore', 'Books and office supplies.', true, now(), now()),
    (9, 'Florist', 'Flowers and gifts.', true, now(), now()),
    (10, 'Hardware', 'Tools and home supplies.', true, now(), now())
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
    (1, 'Credit Card', 'Payment via credit card.', true, now(), now()),
    (2, 'Debit Card', 'Payment via debit card.', true, now(), now()),
    (3, 'Cash', 'Payment in cash upon delivery.', true, now(), now()),
    (4, 'Digital Wallet', 'Payment via digital wallet.', true, now(), now()),
    (5, 'Bank Transfer', 'Payment via bank transfer.', true, now(), now()),
    (6, 'Gift Card', 'Payment via gift card balance.', true, now(), now())
ON CONFLICT ("Name") DO NOTHING;

SELECT setval(pg_get_serial_sequence('"StoreCategories"', 'Id'), (SELECT max("Id") FROM "StoreCategories"));
SELECT setval(pg_get_serial_sequence('"OrderStatuses"', 'Id'), (SELECT max("Id") FROM "OrderStatuses"));
SELECT setval(pg_get_serial_sequence('"PaymentMethods"', 'Id'), (SELECT max("Id") FROM "PaymentMethods"));

COMMIT;

