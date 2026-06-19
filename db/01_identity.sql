BEGIN;

INSERT INTO "AspNetRoles" (
    "Id",
    "Name",
    "NormalizedName",
    "ConcurrencyStamp"
)
VALUES (
    'seed-role-admin',
    'Admin',
    'ADMIN',
    'seed-role-admin-concurrency'
)
ON CONFLICT ("NormalizedName") DO NOTHING;

INSERT INTO "AspNetUsers" (
    "Id",
    "UserName",
    "NormalizedUserName",
    "Email",
    "NormalizedEmail",
    "EmailConfirmed",
    "PasswordHash",
    "SecurityStamp",
    "ConcurrencyStamp",
    "PhoneNumber",
    "PhoneNumberConfirmed",
    "TwoFactorEnabled",
    "LockoutEnd",
    "LockoutEnabled",
    "AccessFailedCount",
    "FirstName",
    "LastName"
)
VALUES (
    'seed-user-admin',
    'admin@orbi.com',
    'ADMIN@ORBI.COM',
    'admin@orbi.com',
    'ADMIN@ORBI.COM',
    true,
    'AQAAAAIAAYagAAAAEJyf8wmzutqUV/GWFQqqTwkNZQEZGut5hEiZZRFjta7MruG9VZuOPS5EMiNAx956sQ==',
    'seed-admin-security-stamp',
    'seed-admin-concurrency-stamp',
    null,
    false,
    false,
    null,
    true,
    0,
    'Admin',
    'Orbi'
)
ON CONFLICT ("NormalizedUserName") DO NOTHING;

INSERT INTO "AspNetUserRoles" (
    "UserId",
    "RoleId"
)
SELECT
    users."Id",
    roles."Id"
FROM "AspNetUsers" users
JOIN "AspNetRoles" roles
    ON roles."NormalizedName" = 'ADMIN'
WHERE users."NormalizedUserName" = 'ADMIN@ORBI.COM'
ON CONFLICT ("UserId", "RoleId") DO NOTHING;

COMMIT;
