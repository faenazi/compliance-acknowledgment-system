# Sprint Implementation Status

## 1. Purpose

This document is used to track the implementation status of the Enterprise Acknowledgment Platform (EAP) across all planned sprints.

It provides a simple and structured way to track:

- sprint objectives
- implementation progress
- completed scope
- in-progress scope
- blockers
- key decisions
- risks
- next steps

This file should be updated continuously during delivery.

---

## 2. Status Scale

Use the following status values consistently:

- Not Started
- In Progress
- Completed
- Blocked
- Deferred

---

## 3. Overall Delivery Status

| Area | Status | Notes |
|------|--------|-------|
| Foundation & Setup | Completed | Backend and frontend skeletons created; authenticated app shell in place |
| Identity & Access | In Progress | LDAP auth, User/Role/Scope/Assignment model, access context and role-aware shell delivered in Sprint 1 |
| Policy Management | Not Started | |
| Acknowledgment Core | Not Started | |
| Audience & Recurrence | Not Started | |
| Form-Based Disclosures | Not Started | |
| User Portal | Not Started | |
| Admin Portal & Operations | Not Started | |
| Compliance, Notifications & Reports | Not Started | |
| UAT / Stabilization | Not Started | |

---

## 4. Sprint 0 – Foundation & Setup

### Sprint Goal
Establish the technical and UI foundation of the platform.

### Status
In Progress

### Planned Scope
- ASP.NET Core solution structure
- modular architecture setup
- vertical slice conventions
- MediatR setup
- FluentValidation setup
- AutoMapper setup
- Serilog setup
- Swagger setup
- EF Core setup
- Next.js app structure
- Tailwind configuration
- TypeScript setup
- React Query setup
- Axios setup
- React Hook Form setup
- Zod setup
- RTL app shell
- design tokens foundation
- shared base UI components

### Progress Summary
- Backend solution skeleton created under `backend/` with Modular + Vertical Slice layering (`Eap.Api`, `Eap.Application`, `Eap.Domain`, `Eap.Infrastructure`, plus `Eap.Modules/` placeholder for per-feature modules added in later sprints).
- MediatR 14.1.0, FluentValidation 12.1.1, AutoMapper 16.1.1, Serilog.AspNetCore 10.0.0, Swashbuckle.AspNetCore 10.1.7, and EF Core 10.0.0 (SQL Server) wired in with pinned versions; `ValidationBehavior` registered in the MediatR pipeline.
- Centralized `GlobalExceptionMiddleware` and standardized `ApiError` response contract established; `/health` endpoint added; Swagger UI enabled in Development only.
- `EapDbContext` scaffolded with empty model — concrete entity sets are introduced per module from Sprint 1 onwards.
- Frontend Next.js 16.2.3 skeleton created under `frontend/` with React 19.2.5, TypeScript 6.0.2, Tailwind CSS 4.2.2 (+ @tailwindcss/postcss 4.2.2, postcss 8.5.9), TanStack Query 5.97.0, Axios 1.15.0, React Hook Form 7.72.1, Zod 4.3.6, clsx 2.1.1, tailwind-merge 3.5.0, lucide-react 1.8.0 — all versions pinned.
- Design tokens foundation implemented as CSS custom properties (`src/styles/tokens.css`) mapped into Tailwind 4 via `@theme`, and mirrored in TypeScript (`src/lib/tokens/design-tokens.ts`); tokens match `docs/09-ux-ui/design-system-tokens.md`.
- RTL-first root layout (`lang="ar"`, `dir="rtl"`), base app shell, and route foundation in place: landing page, `(user)/dashboard`, `admin/dashboard`, and `login` placeholders.
- Base UI primitives (`Button`, `Card`, `Badge`) created using brand tokens; status badge variants map directly to the documented status color tokens.
- Centralized Axios client with normalized `ApiError` shape, and a TanStack Query provider wired through an app-level `Providers` component.

### Completed Items
- Backend solution, projects, layer boundaries, and package pinning
- MediatR + FluentValidation pipeline behavior
- AutoMapper base profile and DI registration
- Serilog host integration (reads from configuration)
- Swagger/OpenAPI configuration (Development-only UI)
- EF Core `EapDbContext` and Infrastructure DI skeleton
- Standardized API error contract and global exception middleware
- Frontend Next.js app scaffolded with pinned versions
- Tailwind CSS 4 configured via PostCSS + `@theme`
- RTL-first root layout and app shell
- Design token CSS + TypeScript mirror
- Axios client and TanStack Query provider
- React Hook Form and Zod available as dependencies (first use will land with the login form in Sprint 1)
- Base UI components: Button, Card, Badge
- Route foundation: landing, user dashboard placeholder, admin dashboard placeholder, login placeholder
- `backend/` and `frontend/` README files

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- Backend layered as `Eap.Api` / `Eap.Application` / `Eap.Domain` / `Eap.Infrastructure`, with `Eap.Modules/` reserved for per-module class libraries added in later sprints (Modular + Vertical Slice).
- Design tokens are defined once as CSS custom properties and exposed to Tailwind 4 through `@theme`; JS consumers use a typed mirror in `src/lib/tokens/design-tokens.ts`.
- Frontend is RTL-first by default at the `<html>` level; LTR is opt-in via `lang="en"`.
- Admin routes live under the `/admin` URL segment; the user portal uses a `(user)` route group so end-user pages sit at the root (e.g. `/dashboard`).

### Risks / Notes
- Foundation quality directly affects all later sprints
- No business entities or endpoints exist yet — feature modules begin arriving in Sprint 1
- No authentication is wired yet; all pages are placeholders

### Next Actions
- Close out remaining Sprint 0 verification (restore/build/dev-server smoke test in the target environment)
- Begin Sprint 1 (Identity & Access Foundations) after sign-off

---

## 5. Sprint 1 – Identity & Access Foundations

### Sprint Goal
Implement authentication, user profile creation, roles, scopes, and access foundations.

### Status
Completed

### Planned Scope
- LDAP / AD authentication
- user login flow
- local user profile creation
- role model
- scope model
- user-role assignment model
- authenticated app shell
- role-aware navigation foundation

### Progress Summary
- LDAP / Active Directory authentication implemented end-to-end via `System.DirectoryServices.Protocols` (Microsoft BCL package, pinned at 10.0.0). Credentials are validated on the server through a search-then-user-bind flow that works cross-platform and keeps the service account optional.
- All LDAP configuration is externalised through the strongly-typed `LdapOptions` (host, port, SSL, domain, base DN, bind DN, user search filter, attribute map, timeout) and validated via DataAnnotations. Bind password is read from environment variables/secrets only; `appsettings.json` ships placeholders.
- Domain model introduced under `Eap.Domain/Identity`: `User`, `Role`, `Scope` (with `ScopeType` = Global/Department/OwnedContent), `UserRoleAssignment`, plus a canonical `SystemRoles` constant list mirrored on the frontend.
- Persistence configurations created for all four entities under the `identity` schema, with unique indexes on `Users.Username`, `Roles.Name`, and `(Scopes.Type, Reference)`. `EapDbContext` now applies configurations from assembly.
- Application layer owns the auth use cases as explicit MediatR commands/queries: `LoginCommand`, `LogoutCommand`, `GetCurrentUserQuery`. `LoginCommandHandler` orchestrates LDAP bind → profile provisioning → session sign-in → audit emission; nothing business-logic-related lives in the controller.
- `UserProvisioner` creates the local profile on first successful login and refreshes AD-derived attributes (display name, email, department, job title) on every subsequent login (BR-061/BR-063). Bootstrap role assignment (default End User role + configurable administrator roster) is delegated to `IDefaultRoleAssigner` so policy can evolve independently of profile sync.
- `ICurrentUser` access context surfaced to the Application layer with Roles + strongly-typed scope tuples. Authentication (cookie sign-in, claims) is implemented behind `IAuthenticationSession` in the API layer, keeping authentication concerns distinct from the authorization model.
- Cookie-based authentication scheme `eap.session` configured with HttpOnly, SameSite=Lax, sliding 8-hour expiration. Unauthenticated API calls return 401/403 (no server-side redirects) so the frontend owns the login UX.
- Identity audit hooks emitted via a dedicated `IIdentityAuditLogger` — `LoginSucceeded`, `LoginFailed`, `UserProvisioned`, `UserSynchronized`, `LoggedOut` — tagged with `AuditEvent` for downstream extraction. A dedicated immutable AuditLog table remains a later-sprint concern per the audit module roadmap.
- Reference-data seeder (`IdentitySeeder`) runs on start-up, seeding all `SystemRoles` and the Global `Scope` idempotently. Seeding is toggleable via `Identity:Seed:Enabled`.
- Frontend login page now posts credentials to `/api/auth/login` using React Hook Form + Zod manual validation. Successful login routes to `/admin/dashboard` for admin roles, otherwise `/dashboard`.
- New `SessionProvider` exposes the authenticated user via React context + TanStack Query (`/api/auth/me`). `AuthGuard` wraps protected routes with role allow-list support; `AuthenticatedAppShell` + `PortalNav` + `UserMenu` compose a role-aware header.
- Route layouts added: `(user)/layout.tsx` guards user portal pages with any authenticated session; `admin/layout.tsx` additionally requires one of the admin-capable roles. The root `/` resolves the session and redirects to the correct landing page.
- Lightweight profile page (`(user)/profile/page.tsx`) displays AD-sourced attributes and effective roles as required by `user-portal-pages §14`.

### Completed Items
- `LdapOptions` + `LdapAttributeMap` with full env/secret-driven configuration
- `LdapConnectionFactory`, `LdapUserDirectory`, `LdapAuthenticationService` (with RFC 4515 filter escaping)
- `User` / `Role` / `Scope` / `UserRoleAssignment` domain entities and EF configurations
- `EapDbContext` updated to expose identity DbSets and scan assembly for configurations
- `UserRepository` with active-assignment projection, `DefaultRoleAssigner`
- `UserProvisioner` (first-login create, AD attribute refresh on every sync)
- `LoginCommand` / `LogoutCommand` / `GetCurrentUserQuery` with validators and handlers
- `ICurrentUser`, `IAuthenticationSession`, `IIdentityAuditLogger` application abstractions
- Cookie authentication scheme (`EapCookie`) + `CurrentUserService` + `HttpAuthenticationSession`
- `IdentitySeeder` (roles + Global scope) wired into application start-up
- `AuthController` exposing `/api/auth/{login,logout,me}` (thin, delegates to MediatR)
- `appsettings.json` + `appsettings.Development.json` carry `Ldap`, `Identity:Provisioning`, `Identity:Seed` sections
- Frontend `SessionProvider`, `AuthGuard`, `AuthenticatedAppShell`, `PortalNav`, `UserMenu`
- Real LDAP-backed login page with Zod-based validation and role-aware redirect
- `(user)` and `admin` route group layouts with server-side auth gate
- Lightweight profile page showing AD attributes and effective roles
- Design-system-aligned styling for the login form, header, and guard states

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- **Identity library**: authentication uses `System.DirectoryServices.Protocols` (Microsoft BCL-provided package, pinned at 10.0.0). This package is part of the Microsoft platform family alongside the approved `Microsoft.*` stack and keeps LDAP integration first-party.
- **Session transport**: HTTP-only `eap.session` cookie issued by ASP.NET Core cookie authentication. API endpoints never redirect — unauthenticated calls receive 401/403 and the SPA handles the UX. This keeps the frontend and backend authentication concerns separate.
- **Role model is local**: LDAP group memberships are captured in the directory snapshot but are **not** mapped to platform roles at this stage (BR-141/BR-142). Roles and scopes remain local application data and are assigned through provisioning config + future admin tooling.
- **Bootstrap administrators**: `Identity:Provisioning:SystemAdministrators` list is the approved mechanism to grant the initial System Administrator role without manual DB edits. Empty by default; per-environment list recommended.
- **Audit hooks**: authentication events are logged via Serilog with `AuditEvent` structured property. A dedicated immutable AuditLog store is deferred to the Audit module sprint; switching sinks does not affect callers of `IIdentityAuditLogger`.
- **LDAP bind strategy**: service-account lookup followed by a second bind as the resolved user DN. Works cross-platform and avoids `DOMAIN\user` shortcuts that are not always available outside Windows.
- **Directory attribute mapping is configurable**: defaults match standard AD schema but every attribute name is overridable through `Ldap:Attributes`.

### Risks / Notes
- Real LDAP validation in a production-like environment remains a pre-pilot activity. A containerised OpenLDAP or AD instance should be used for integration tests before Sprint 2 stabilises.
- Without a running SQL Server, first request that touches Identity will fail. `IdentitySeeder` deliberately skips silently if the database is unreachable so Swagger and `/health` remain usable during local development.
- Cookie `SecurePolicy` is `SameAsRequest` to match the Sprint 0 dev experience (HTTP). Production deployments should flip this to `Always`.
- Blocking enforcement remains explicitly out of Phase 1 (BR-160, FR-170). Role gates only limit admin portal navigation; per-feature authorization is introduced alongside the feature modules in later sprints.

### Next Actions
- Begin Sprint 2 (Policy Management)
- Provide a concrete LDAP test environment / fixture for integration testing
- Capture the first-administrator bootstrap convention in the deployment runbook

---

## 6. Sprint 2 – Policy Management

### Sprint Goal
Deliver policy creation, document upload, policy versioning, and publication.

### Status
Not Started

### Planned Scope
- policy entity
- policy version entity
- policy document handling
- policy CRUD
- policy version management
- publish/archive behavior
- historical version visibility

### Progress Summary
- Not started yet

### Completed Items
- None

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- None yet

### Risks / Notes
- historical integrity must be preserved
- document handling approach must remain simple in MVP

### Next Actions
- define upload storage approach
- implement policy list and version screens
- enforce one published version rule

---

## 7. Sprint 3 – Acknowledgment Core

### Sprint Goal
Deliver the core acknowledgment/disclosure definition and versioning model.

### Status
Not Started

### Planned Scope
- acknowledgment definition entity
- acknowledgment version entity
- action type support
- link to policy version
- acknowledgment CRUD
- version lifecycle
- publish/archive behavior

### Progress Summary
- Not started yet

### Completed Items
- None

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- None yet

### Risks / Notes
- version linkage to policy must remain strict
- action types must remain aligned with business model

### Next Actions
- implement definition and version entities
- implement version-aware APIs
- build acknowledgment admin screens

---

## 8. Sprint 4 – Audience Targeting & Recurrence

### Sprint Goal
Deliver user targeting and recurring action logic.

### Status
Not Started

### Planned Scope
- audience definitions
- audience rules
- exclusions
- recurrence model support
- start/due date behavior
- user action requirement generation

### Progress Summary
- Not started yet

### Completed Items
- None

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- None yet

### Risks / Notes
- targeting accuracy depends on AD data quality
- recurrence logic must remain deterministic and simple

### Next Actions
- implement audience rule model
- implement recurrence model behavior
- build audience and recurrence admin pages

---

## 9. Sprint 5 – Form-Based Disclosures

### Sprint Goal
Deliver dynamic form-based disclosures for business-critical scenarios.

### Status
Not Started

### Planned Scope
- form definition model
- JSON-driven form schema
- supported field types
- form definition management
- dynamic form rendering
- submission validation
- submission storage

### Progress Summary
- Not started yet

### Completed Items
- None

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- None yet

### Risks / Notes
- form complexity must be controlled
- do not allow sprint scope to turn into a full form builder
- conflict of interest and gifts/hospitality are priority validation cases

### Next Actions
- implement schema structure
- implement renderer and validator
- test priority business forms

---

## 10. Sprint 6 – User Portal

### Sprint Goal
Deliver the employee-facing portal and completion flows.

### Status
Not Started

### Planned Scope
- user dashboard
- my required actions
- action details
- policy viewer
- simple acknowledgment submission
- disclosure submission
- submission confirmation
- my history

### Progress Summary
- Not started yet

### Completed Items
- None

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- None yet

### Risks / Notes
- user experience must remain low-friction
- policy reading and submission path must be very clear
- no blocking behavior should be introduced in Phase 1

### Next Actions
- implement end-user APIs
- build dashboard and action flow pages
- validate long form readability

---

## 11. Sprint 7 – Admin Portal & Operations

### Sprint Goal
Deliver the admin operational experience for managing actions and monitoring usage.

### Status
Not Started

### Planned Scope
- admin dashboard
- policy management refinements
- acknowledgment management refinements
- user action monitoring
- historical detail access
- operational summary views

### Progress Summary
- Not started yet

### Completed Items
- None

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- None yet

### Risks / Notes
- admin portal must stay structured and efficient
- avoid making the dashboard overly analytical in MVP

### Next Actions
- implement monitoring queries
- build admin monitoring screens
- refine operational summaries

---

## 12. Sprint 8 – Compliance, Notifications, Audit & Reports

### Sprint Goal
Deliver operational visibility, outbound notifications, and release-readiness functions.

### Status
Not Started

### Planned Scope
- compliance dashboard
- non-compliant user reporting
- department-level reporting
- Exchange notifications
- reminders
- overdue notifications
- audit log explorer
- exports
- final MVP integration checks

### Progress Summary
- Not started yet

### Completed Items
- None

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- None yet

### Risks / Notes
- Exchange integration may require environment coordination
- reports must stay operational, not become BI-heavy
- audit logs must remain trustworthy and easy to inspect

### Next Actions
- implement compliance queries
- implement notification sender
- implement audit explorer and export flows

---

## 13. Optional Sprint 9 – Stabilization & Launch Readiness

### Sprint Goal
Provide a final hardening sprint if needed before release.

### Status
Not Started

### Planned Scope
- defect fixes
- UAT fixes
- performance tuning
- permission validation
- release readiness checks
- documentation alignment
- deployment support

### Progress Summary
- Not started yet

### Completed Items
- None

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- None yet

### Risks / Notes
- this sprint should not become a place for new features
- use only if required by delivery quality or UAT findings

### Next Actions
- reserve only if needed
- focus on stability, not expansion

---

## 14. Current Risks Register

| Risk ID | Risk | Impact | Status | Mitigation |
|--------|------|--------|--------|------------|
| R-001 | LDAP / AD integration complexity | High | Open | Validate authentication design early |
| R-002 | Exchange email delivery issues | Medium | Open | Test integration before reporting sprint ends |
| R-003 | Scope creep into full form builder | High | Open | Keep form scope controlled and JSON-based |
| R-004 | AD data quality affecting targeting | Medium | Open | Validate department/group mapping early |
| R-005 | Recurrence logic becoming too complex | Medium | Open | Keep MVP recurrence rules explicit and limited |
| R-006 | Reporting queries becoming slow | Medium | Open | Optimize query design and pagination |
| R-007 | Admin portal becoming overly dense | Medium | Open | Follow UX principles and page discipline |

---

## 15. Current Decisions Log

| Decision ID | Decision | Date | Status | Notes |
|------------|----------|------|--------|-------|
| D-001 | Phase 1 will not include blocking enforcement | TBD | Approved | |
| D-002 | Form-based disclosures are included in Phase 1 | TBD | Approved | |
| D-003 | No full visual form builder in Phase 1 | TBD | Approved | |
| D-004 | Access model uses Role + Scope, not department as permission | TBD | Approved | |
| D-005 | LDAP / AD is the identity source of truth | TBD | Approved | |
| D-006 | Exchange is the approved notification channel | TBD | Approved | |

---

## 16. Open Items / Pending Clarifications

| Item ID | Topic | Owner | Status | Notes |
|--------|-------|-------|--------|-------|
| O-001 | Final deployment/authentication topology | TBD | Open | |
| O-002 | File storage approach for uploaded documents | TBD | Open | |
| O-003 | Export format expectations for reports | TBD | Open | |
| O-004 | Exact reminder schedule rules | TBD | Open | |
| O-005 | Whether draft-save is needed for long disclosures in MVP | TBD | Open | |

---

## 17. Suggested Update Format Per Sprint

When updating this file after each sprint, use the following structure:

### Sprint Status Update
- Status:
- Completed:
- In Progress:
- Deferred:
- Blockers:
- Decisions:
- Risks:
- Next Sprint Readiness:

---

## 18. Weekly Progress Summary Template

Use this section optionally for short operational updates.

### Week [X]
- overall progress:
- key completed items:
- main blocker:
- key decision:
- next focus:

---

## 19. MVP Readiness Checklist

Use this checklist to assess whether the platform is ready for controlled launch.

| Item | Status | Notes |
|------|--------|-------|
| LDAP / AD authentication works | In Progress | Implemented in Sprint 1; pending validation against a real AD instance |
| User profiles are created and synced | In Progress | Provisioning + on-login sync delivered in Sprint 1 |
| Policies can be created and versioned | Not Started | |
| Acknowledgments can be defined and published | Not Started | |
| Form-based disclosures work | Not Started | |
| Audience targeting works correctly | Not Started | |
| Recurrence logic works correctly | Not Started | |
| User portal is usable end-to-end | Not Started | |
| Admin portal is usable end-to-end | Not Started | |
| Notifications are sent through Exchange | Not Started | |
| Compliance views are available | Not Started | |
| Reports can be exported | Not Started | |
| Audit records are visible and trustworthy | Not Started | |
| UAT critical issues are closed | Not Started | |

---

## 20. Summary

This document is the living delivery tracker for the EAP project.

It should be updated continuously to reflect:

- current sprint progress
- decisions made
- blockers encountered
- delivery risks
- readiness for the next sprint
- readiness for MVP launch

It is intended to keep business, engineering, and project leadership aligned throughout implementation.
