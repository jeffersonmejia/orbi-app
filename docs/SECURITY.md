# Security

## Supported Versions

| Version | Supported |
|---------|-----------|
| 1.0.x | Yes |

## Reporting

Email `security@orbi-platform.com`. Expect a response within 48 hours. Do not report vulnerabilities in public issues.

Include:
- Type of issue
- File paths and line numbers
- Steps to reproduce
- Proof of concept if available

## Disclosure

1. Acknowledge within 48 hours
2. Validate the issue
3. Develop and test a fix
4. Release a patched version
5. Credit the reporter

## Protections

| Area | Measure |
|------|---------|
| Passwords | Hashed with salt, never stored in plain text |
| CSRF | ValidateAntiForgeryToken on all POST actions |
| SQL Injection | EF Core parameterized queries prevent injection |
| Input Validation | Data Annotations with ModelState validation |
| XSS | Razor auto-encodes output by default |
| Soft Delete | IsActive flag preserves records while hiding them |
| Secrets | Connection strings in environment variables or User Secrets |
| CORS | Configured for trusted origins only |

## Production Checklist

- Use HTTPS redirection and HSTS
- Run dotnet list package --vulnerable regularly
- Use least-privilege database user
- Enable centralized logging
- Configure rate limiting on auth endpoints
