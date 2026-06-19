# Security

## Reporting

Report vulnerabilities privately to `security@orbi-platform.com`.

Include:

- Issue type
- Affected paths and line numbers
- Reproduction steps
- Proof of concept, if available

Do not publish vulnerabilities in public issues.

## Supported Version

| Version | Supported |
| --- | --- |
| 1.0.x | Yes |

## Current Protections

| Area | Protection |
| --- | --- |
| Authentication | ASP.NET Identity with unique email checks, duplicate-registration validation and lockout after failed login attempts |
| Authorization | Global MVC role filter plus service-level ownership filters |
| Roles | `Admin`, `StoreOwner`, `DeliveryDriver`, `Customer` |
| Ownership | Sensitive reads and writes validate `UserId` links |
| Profile updates | Users can edit personal name and phone only; email, role, password and identifiers remain immutable from Profile |
| Passwords | Identity password hashing; minimum 10 chars, digit, uppercase and symbol |
| Sessions | `HttpOnly`, `SameSite=Strict`, secure cookies in production, 8-hour cookie lifetime and one active session per user |
| CSRF | Antiforgery validation on POST actions |
| SQL Injection | EF Core parameterized queries |
| XSS | Razor output encoding plus Content Security Policy |
| Clickjacking | `X-Frame-Options: DENY` and CSP `frame-ancestors 'none'` |
| MIME sniffing | `X-Content-Type-Options: nosniff` |
| Transport | HTTPS redirection and HSTS outside development |
| Data retention | Business delete actions use logical delete through `IsActive` |

## Access Behavior

Unauthenticated users only see sign-in and sign-up actions. Authenticated users only see sections authorized for their role. Customers are limited to store/product browsing and their own orders; administrative directories, payments, reviews and lookup tables are hidden and forbidden. If a role opens a forbidden route directly, the app returns `403` and shows the access denied page.

## Session Behavior

- A successful login marks the user as having an active session.
- A second login attempt with valid credentials is rejected while that session is active.
- Logout clears the active-session marker.
- The active-session marker follows the configured 8-hour cookie lifetime.

## Production Checklist

- Set production connection strings through environment variables or user secrets.
- Use a least-privilege PostgreSQL user.
- Configure real HTTPS certificates and production host names.
- Review CSP when adding external scripts, fonts or images.
- Add email confirmation and password recovery before public deployment.
- Add automated authorization tests for cross-user data access.
- Run package vulnerability checks before releases.
