# Technical Preferences

## Stack

| Layer        | Technology                          |
|--------------|-------------------------------------|
| Backend      | ASP.NET Core (latest stable)        |
| Frontend     | Next.js (latest stable, App Router) |
| Styling      | Tailwind CSS                        |
| Database     | SQL Server                          |
| Language     | C# (backend), TypeScript (frontend) |

## Backend Architecture

- **Modular architecture** at the top level: each core domain (Policies, Acknowledgments, Compliance, Audit, Reporting) is an isolated module with its own folder, endpoints, domain models, and persistence.
- **Vertical slice** inside each module: each feature (e.g., `PublishPolicyVersion`, `SubmitAcknowledgment`) is a self-contained slice with its request, handler, validator, and response types co-located.
- Modules communicate through well-defined contracts, not by reaching into each other's internals.
- Cross-cutting concerns (audit logging, authorization, validation) are implemented as pipeline behaviors / middleware, not duplicated per slice.

### API Style

- HTTP + JSON. Minimal APIs or controllers are acceptable; pick one convention per module and stay consistent.
- Endpoints are versioned under `/api/v1/...`.
- Request/response DTOs are distinct from domain entities. Entities are never serialized directly.

### Persistence

- Entity Framework Core against SQL Server.
- Migrations are code-first and checked into the repository.
- Each module owns its own `DbContext` partition or schema; shared tables are avoided.
- No raw SQL for domain writes. Reporting queries may use SQL or Dapper for performance.

### Validation and Errors

- FluentValidation or equivalent at the slice boundary.
- Domain invariants are enforced in the domain layer, not the validator.
- Errors returned as RFC 7807 Problem Details.

## Frontend Architecture

- Next.js App Router, TypeScript strict mode.
- Two separate route groups: `(user-portal)` and `(admin-portal)`. They do not share layouts or navigation.
- Server components by default; client components only where interactivity requires it.
- Data fetching via server components or a typed API client; no direct `fetch` calls scattered in leaf components.
- State management kept local; global stores are introduced only when a concrete need exists.

### Styling

- Tailwind CSS utility-first. No parallel CSS-in-JS system.
- Arabic-first RTL: `dir="rtl"` is the default on the root layout; LTR is an explicit opt-in for English views.
- Design tokens (colors, spacing, typography) defined in `tailwind.config` and referenced by name, never as arbitrary values in components.

### Internationalization

- Arabic is the default locale. English is secondary.
- All user-facing strings go through the i18n layer — no hardcoded strings in components.

## Database Conventions

- Schema per module (e.g., `policies`, `acknowledgments`, `audit`).
- Primary keys are `GUID` (sequential where supported) unless a specific reason exists to use `bigint`.
- Every table has `CreatedAtUtc` and, where applicable, `CreatedBy`.
- Append-only tables (`AuditLog`, `Acknowledgment`) have no update or delete triggers and no update paths in EF configuration.
- Foreign keys are always enforced; cascading deletes are forbidden on audit and acknowledgment relationships.

## Security

- Authentication against the organization's internal identity provider.
- Authorization is role-based and checked server-side on every request.
- All secrets come from configuration/secret store; none are committed.
- HTTPS only. HSTS enabled in non-dev environments.

## Observability

- Structured logging (Serilog or built-in `ILogger` with JSON formatter).
- Correlation IDs flow from frontend requests through to backend logs and audit entries.
- Audit logs are separate from operational logs and are not subject to log rotation.

## Testing

- Unit tests for domain logic and slice handlers.
- Integration tests for each module against a real SQL Server (containerized).
- Frontend component tests for critical user flows (acknowledgment submission, admin publishing).
- No test uses production data.

## Tooling

- `.editorconfig` enforced; formatting is not a code review discussion.
- CI runs build, tests, and migrations on every pull request.
- Database migrations are applied automatically in non-production and gated manually in production.
