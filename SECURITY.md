# Security Policy

## Supported Versions

| Version | Supported          |
|---------|-------------------|
| 1.0.x   | :white_check_mark: |

## Reporting a Vulnerability

We take the security of Orbi seriously. If you discover a security vulnerability, please follow the steps below.

**Do not** report security vulnerabilities through public GitHub issues, discussions, or pull requests.

Instead, please send an email to security@orbi-platform.com. You should receive a response within 48 hours. If you do not receive a response, please follow up to ensure your original message was received.

When reporting, include as much of the following information as possible:

- Type of issue (e.g., SQL injection, cross-site scripting, broken authentication, etc.)
- Full paths of source file(s) related to the issue
- The location of the affected source code (file name, line number)
- Any special configuration required to reproduce the issue
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if available)
- Impact of the issue, including how an attacker might exploit it

## Disclosure Policy

When we receive a security vulnerability report, we follow these steps:

1. **Acknowledge** receipt of the report within 48 hours.
2. **Validate** the issue to confirm it is a legitimate vulnerability.
3. **Develop** a fix and test it internally.
4. **Release** a patched version and publish a security advisory.
5. **Credit** the reporter (unless they wish to remain anonymous).

## Security Best Practices for Orbi

### Authentication & Authorization

- Passwords are stored as hashed values using a strong hashing algorithm with individual salts. Plain-text passwords are never stored or logged.
- Future releases will implement ASP.NET Core Identity for role-based access control (Customer, Administrator, DeliveryDriver).
- All controller actions that mutate data are protected with `[ValidateAntiForgeryToken]`.

### Data Protection

- The `IsActive` soft-delete pattern prevents accidental or malicious data loss by keeping records in the database while making them invisible to application queries.
- Connection strings and secrets are stored in `appsettings.json` (development) and should be overridden via environment variables or Azure Key Vault / User Secrets in production.
- EF Core parameterized queries prevent SQL injection by design.

### Input Validation

- All ViewModels use Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`, etc.) for client-side and server-side validation.
- Model validation is enforced in every POST handler via `ModelState.IsValid`.
- User-supplied HTML is not rendered directly; Razor views auto-encode output by default.

### HTTPS & Transport Security

- The development template uses `--no-https` for simplicity. In production, HTTPS redirection and HSTS headers must be enabled.
- Add the following to `Program.cs` for production:
  ```csharp
  app.UseHttpsRedirection();
  app.UseHsts();
  ```

### Dependency Security

- NuGet packages are pinned to specific versions in the `.csproj` file to prevent unexpected breaking changes from automatic updates.
- Regularly run `dotnet list package --vulnerable` to check for known vulnerabilities in dependencies.

### Logging & Monitoring

- Structured logging is configured via `ILogger<T>`. Logs include request IDs for traceability but never include passwords, tokens, or PII.
- In production, integrate with a centralized logging platform (e.g., Serilog + Seq, Application Insights) for anomaly detection.

## Secure Configuration Checklist

- [ ] Connection strings use a database user with least-privilege permissions (not the PostgreSQL superuser).
- [ ] Development secrets are stored with `dotnet user-secrets` or environment variables, not committed to source control.
- [ ] CORS policies are configured to allow only trusted origins.
- [ ] Rate limiting is enabled on authentication endpoints.
- [ ] Regular security audits and dependency scans are scheduled.
- [ ] The `globalqueryfilter` for `IsActive` is applied to all entities to prevent accidental exposure of soft-deleted records.

## Contact

For security-related inquiries, reach out to:
- **Email**: security@orbi-platform.com
- **PGP Key**: Available on the Orbi security page (coming soon)
