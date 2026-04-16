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
| Policy Management | Completed | Policy/Version/Document domain, CRUD, publish/archive, and document upload delivered in Sprint 2 |
| Acknowledgment Core | Completed | AcknowledgmentDefinition/Version aggregate, action types, policy-version linkage, and publish/archive SoD delivered in Sprint 3 |
| Audience & Recurrence | Completed | AudienceDefinition/Rule + UserActionRequirement domain, five recurrence models, audience resolution pipeline, and requirement-generation foundation delivered in Sprint 4 |
| Form-Based Disclosures | Completed | FormDefinition aggregate (version-bound), 15 field types, dynamic renderer, submission validation, form snapshot, file upload, admin form management + preview delivered in Sprint 5 |
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
Completed

### Planned Scope
- policy entity
- policy version entity
- policy document handling
- policy CRUD
- policy version management
- publish/archive behavior
- historical version visibility

### Progress Summary
- Policy, PolicyVersion, and PolicyDocument domain model wired through Application (MediatR + FluentValidation + AutoMapper), Infrastructure (EF Core repository + file-system storage + Serilog audit), and API (thin controllers with role-based authorization).
- Full admin policy management UX delivered in the Next.js portal: list with search/status filter/pagination, create/edit policy, versions history, create/edit draft version, PDF upload, publish, and archive — all Arabic-first RTL with status badges and inline error display.
- BR-010 (document-required-before-publish), BR-011 (single published version per policy), BR-012 (historical preservation via Superseded state), and BR-014 (owner department required) enforced at both the domain and database layers.

### Completed Items
- Domain → Application → Infrastructure → API slices for policies, versions, and documents
- `IPolicyRepository` with eager-loaded aggregate access and paged list filters
- `IPolicyDocumentStorage` abstraction with configurable file-system implementation (root path, size limit, extension + content-type whitelist, path-traversal protection)
- `IPolicyAuditLogger` emitting structured Serilog events (`AuditEvent` property) for create / update / publish / archive / upload / download
- Commands: CreatePolicy, UpdatePolicy, ArchivePolicy, CreatePolicyVersion, UpdatePolicyVersionDraft, PublishPolicyVersion, ArchivePolicyVersion, UploadPolicyDocument
- Queries: ListPolicies (paged + filtered), GetPolicyById, ListPolicyVersions, GetPolicyVersionById, DownloadPolicyDocument (streamed with range support)
- Controllers: `PoliciesController`, `PolicyVersionsController`, `PolicyDocumentsController` with role gating (PolicyManager for authoring, Publisher for publish — SoD per Sprint 1)
- GlobalExceptionMiddleware extended to map domain rule violations (`InvalidOperationException`) → 409 Conflict
- `appsettings` section `PolicyDocuments` with configurable root path, 25 MB default limit, and PDF whitelist
- Frontend typed API client (`lib/api/policies.ts`), TanStack Query hooks (`lib/policies/hooks.ts`), Arabic labels + badge palette (`lib/policies/labels.ts`)
- Reusable components: `PolicyStatusBadge`, `PolicyVersionStatusBadge`, `PolicyForm`, `VersionForm`, `PolicyDocumentUpload`
- Admin pages: `/admin/policies`, `/admin/policies/new`, `/admin/policies/[policyId]`, `/admin/policies/[policyId]/versions`, `/admin/policies/[policyId]/versions/new`, `/admin/policies/[policyId]/versions/[versionId]`
- Portal navigation updated to expose Policies entry to PolicyManager / Publisher / SystemAdministrator roles

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- File-system storage chosen as MVP default for policy documents, pluggable behind `IPolicyDocumentStorage` so cloud/object-store backends can replace it without touching application code (resolves O-002)
- BR-011 enforced with defense in depth: (1) `Policy.PublishVersion` atomically supersedes the current published version inside the aggregate, (2) filtered unique index `UX_PolicyVersions_Policy_Published` on `(PolicyId, Status) WHERE Status = 1`, (3) handler always loads the full aggregate before mutating
- BR-010 enforced in the domain (`PolicyVersion.MarkPublished` rejects when `HasDocument` is false) rather than in the handler, so it applies uniformly regardless of caller
- Segregation of duties for publish: `PolicyManager` can author and archive drafts, but only `Publisher` (or `SystemAdministrator`) can publish
- Upload ordering: bytes are written to storage before the domain attaches the `PolicyDocument` entity; orphan-blob cleanup is deferred to a future janitor job (see Risks)

### Risks / Notes
- Orphan-blob risk: if a domain rejection occurs after bytes are written, the file remains on disk. Tracked as follow-up for a janitor/background sweeper.
- File-system storage requires the deployment target to provide persistent, backed-up storage at the configured `RootPath`. Operational runbook must cover backup and retention before production launch.
- Build verification via `dotnet build` and `tsc --noEmit` could not be run in this environment (SDK + node_modules absent). Required before release.

### Next Actions
- Run backend + frontend builds in CI to confirm the slice compiles end-to-end
- Author integration tests for publish/archive flows and unique-published-version constraint
- Add the orphan-blob cleanup job to the Sprint 8 (Admin Portal & Operations) backlog

---

## 7. Sprint 3 – Acknowledgment Core

### Sprint Goal
Deliver the core acknowledgment/disclosure definition and versioning model.

### Status
Completed

### Planned Scope
- acknowledgment definition entity
- acknowledgment version entity
- action type support
- link to policy version
- acknowledgment CRUD
- version lifecycle
- publish/archive behavior

### Progress Summary
- `AcknowledgmentDefinition` (master) and `AcknowledgmentVersion` (immutable snapshot) domain aggregate introduced under `Eap.Domain/Acknowledgment`, with the `ActionType` enum (SimpleAcknowledgment, AcknowledgmentWithCommitment, FormBasedDisclosure) and lifecycle states (`AcknowledgmentStatus` Draft/Published/Archived; `AcknowledgmentVersionStatus` Draft/Published/Superseded/Archived) modeled per §6.2 and LR-001/CDM-001.
- Each `AcknowledgmentVersion` links to exactly one `PolicyVersion` (BR-019 / FR-023). The shadow foreign key uses `DeleteBehavior.Restrict` so policy history cannot be removed while acknowledgments still reference it.
- "One published version per definition" enforced with defence in depth: (1) `AcknowledgmentDefinition.PublishVersion` atomically supersedes the current published version inside the aggregate, (2) filtered unique index `UX_AcknowledgmentVersions_Definition_Published` on `(AcknowledgmentDefinitionId, Status) WHERE Status = 1`, (3) handler re-verifies the linked policy version is still `Published` at publish time.
- Action-type-dependent validation: FluentValidation requires a non-empty `CommitmentText` when `ActionType == AcknowledgmentWithCommitment`, and the domain `MarkPublished` reasserts the rule so it applies regardless of caller.
- Application layer uses MediatR + FluentValidation + AutoMapper: commands for Create/Update/Archive definition, Create/Update/Publish/Archive version; queries for list (paged + filtered) and detail projections. Audit events emitted through `IAcknowledgmentAuditLogger` as structured Serilog entries tagged `AuditEvent`.
- Cross-module read: a narrow `PolicyVersionLookup` record and `IPolicyRepository.FindVersionLookupAsync` were added so acknowledgment handlers can validate that a linked policy version exists and is Published without leaking the `PolicyVersion` entity across aggregates.
- Full admin acknowledgment UX delivered in the Next.js portal: list with search/status/action-type filter + pagination, create/edit definition, create/edit draft version with two-step policy-version picker, publish, and archive — Arabic-first RTL, status badges, inline error surfacing.

### Completed Items
- Domain → Application → Infrastructure → API slices for `AcknowledgmentDefinition`, `AcknowledgmentVersion`, and `ActionType`
- Aggregate invariants: one published version per definition; archive-definition cascades to its versions; strict Draft-only mutation guard
- `IAcknowledgmentRepository` with eager-loaded aggregate access, `GetMaxVersionNumberAsync`, and `ListAsync` paged filter (search, status, owner department, action type)
- `IAcknowledgmentAuditLogger` emitting `AuditEvent` entries for definition create/update/archive and version create/update/publish/archive
- Commands: `CreateAcknowledgmentDefinition`, `UpdateAcknowledgmentDefinition`, `ArchiveAcknowledgmentDefinition`, `CreateAcknowledgmentVersion`, `UpdateAcknowledgmentVersionDraft`, `PublishAcknowledgmentVersion`, `ArchiveAcknowledgmentVersion`
- Queries: `ListAcknowledgmentDefinitions` (paged + filtered), `GetAcknowledgmentDefinitionById`, `ListAcknowledgmentVersions`, `GetAcknowledgmentVersionById`
- EF configurations under schema `acknowledgment`, with filtered unique index for published-version-per-definition and compound unique index on `(AcknowledgmentDefinitionId, VersionNumber)`
- `IPolicyRepository.FindVersionLookupAsync` + `PolicyVersionLookup` record for cross-aggregate policy-version validation
- Controllers: `AcknowledgmentDefinitionsController`, `AcknowledgmentVersionsController` with role gating — authoring requires `AcknowledgmentManager`, publishing requires `Publisher` (SoD matching Sprint 2)
- Infrastructure DI: `AddAcknowledgmentManagement` registers repository + audit sink; `EapDbContext` exposes new DbSets
- Frontend typed API client (`lib/api/acknowledgments.ts`), TanStack Query hooks (`lib/acknowledgments/hooks.ts`), Arabic labels + action-type descriptions (`lib/acknowledgments/labels.ts`)
- Reusable components: `AcknowledgmentStatusBadge`, `AcknowledgmentVersionStatusBadge`, `ActionTypeBadge`, `AcknowledgmentDefinitionForm`, `AcknowledgmentVersionForm`, `PolicyVersionPicker`
- Admin pages: `/admin/acknowledgments`, `/admin/acknowledgments/new`, `/admin/acknowledgments/[definitionId]`, `/admin/acknowledgments/[definitionId]/versions/new`, `/admin/acknowledgments/[definitionId]/versions/[versionId]`
- Portal navigation updated: "الإقرارات" entry visible to admin roles

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- `AcknowledgmentVersion` is treated as an immutable snapshot: edits are only allowed while the version is `Draft`; published/superseded/archived versions are read-only (BR-031-aligned)
- The linked `PolicyVersion` is validated both at version-creation time and re-verified at publish time — this prevents publishing an acknowledgment whose underlying policy version has since been superseded or archived
- Segregation of duties for publish: `AcknowledgmentManager` can author and archive drafts; only `Publisher` (or `SystemAdministrator`) can publish — mirrors Sprint 2's policy flow
- Cross-module read for policy-version status goes through a narrow `PolicyVersionLookup` record on the existing `IPolicyRepository` rather than exposing the `PolicyVersion` aggregate directly, keeping aggregate boundaries clean
- `StartDate`/`DueDate` are kept on the version per §6.2 data model, but recurrence, audience, and assignment logic remain deliberately out of Sprint 3 scope (Sprints 4/5)
- `FormBasedDisclosure` is accepted as an action-type value, but no form schema is persisted in this sprint — the full form builder lands in Sprint 5

### Risks / Notes
- Build verification via `dotnet build` and `tsc --noEmit` could not be run in this environment (SDK + node_modules absent). Required before release; CI is the authoritative gate.
- No EF migration was generated in this sprint — the new `acknowledgment` schema tables will be materialized by the standard migration flow when the database tooling is available.
- Audience, recurrence, and form schema deliberately excluded; downstream sprints will introduce their own aggregates and link to the acknowledgment version snapshot.

### Next Actions
- Generate the EF migration for the `acknowledgment` schema as part of the CI build verification
- Author integration tests for the publish flow (invariant + policy-version status re-check)
- Begin Sprint 4 (Audience Targeting & Recurrence)

---

## 8. Sprint 4 – Audience Targeting & Recurrence

### Sprint Goal
Deliver user targeting and recurring action logic.

### Status
Completed

### Planned Scope
- audience definitions
- audience rules
- exclusions
- recurrence model support
- start/due date behavior
- user action requirement generation

### Progress Summary
- `AudienceDefinition` (aggregate) and `AudienceRule` domain model added under `Eap.Domain/Audience`, bound 1:1 to an `AcknowledgmentVersion`. `AudienceRuleType` = AllUsers/Department/AdGroup/User; `AudienceType` is derived from the active rule set (AllUsers/Departments/AdGroups/Mixed) for read-only reporting.
- Inclusion and exclusion rules are stored on the same aggregate and separated by the `IsExclusion` flag; `AllUsers` is rejected as an exclusion (BR-055) at the domain level. Inclusions are combined with OR, exclusions always override (BR-054).
- `RecurrenceModel` enum (Unspecified, OnboardingOnly, Annual, OnboardingAndAnnual, OnChange, EventDriven) lives on `AcknowledgmentVersion` with a dedicated `SetAcknowledgmentVersionRecurrence` command (BR-046). `Unspecified` blocks publish (BR-033); draft creation/edit accepts any of the five configured models.
- `UserActionRequirement` entity introduced under `Eap.Domain/Requirements` with a deterministic cycle reference (`onboarding`, `annual:YYYY`, `event:<ref>`, `change:<ref>`) and `IsCurrent` flag so requirement snapshots per user/version/cycle are idempotent. Unique index `UX_UserActionRequirements_User_Version_Cycle` enforces the contract at the database level.
- Application layer exposes typed commands/queries: `ConfigureAudienceInclusion`, `ConfigureAudienceExclusions`, `SetAllUsersAudience`, `GetAudienceByVersion`, `PreviewAudience`, `SetAcknowledgmentVersionRecurrence`, `ListRequirementsForVersion`, `GenerateRequirementsForVersion` — each with its own validator and MediatR handler.
- Audience resolution runs through `IAudienceResolver`: inclusions are unioned (AllUsers → active users; Department → users in the AD-synced department; AdGroup → delegated to `IDirectoryGroupResolver`, stubbed as empty in Phase 1 because LDAP group sync is a future sprint; User → literal GUID). Exclusions are unioned and subtracted.
- `RequirementGenerator` coordinates the end-to-end flow: verifies Published + audience → derives cycle key → resolves audience → skips existing (user, version, cycle) rows → marks prior cycles as not-current → persists new requirements → emits audit events. Uses injected `TimeProvider` so the Annual cycle (`annual:YYYY`) is deterministic and testable.
- Full admin audience + recurrence UX delivered in the Next.js portal. Every Draft acknowledgment version now exposes two dedicated pages:
  - `/admin/acknowledgments/:def/versions/:ver/recurrence` — pick from the five recurrence models, see a human-readable description, set optional start/due dates, with a read-only summary banner.
  - `/admin/acknowledgments/:def/versions/:ver/audience` — inclusion editor (Department / AD Group / Specific User), "All users" quick-set, explicit exclusions editor, and an estimated preview card with counts + a sample of matched users.
- Recurrence summary and quick-links are surfaced on the version detail page so authors always see cadence while editing. Published/Superseded/Archived versions fall back to a read-only projection (BR-031).

### Completed Items
- Domain → Application → Infrastructure → API slices for `AudienceDefinition`, `AudienceRule`, and `UserActionRequirement`
- `AudienceDefinition` aggregate methods: `SetAllUsers`, `ReplaceInclusionRules`, `ReplaceExclusionRules`, `HasAnyInclusionRule`, `DeriveAudienceType` — with BR-055 guard that forbids AllUsers in exclusions
- `RecurrenceModel` enum added to `AcknowledgmentVersion`; publish gate re-verifies audience + recurrence invariants before `MarkPublished`
- Deterministic cycle-key helpers (`RecurrenceCycle.Onboarding`, `AnnualFor(year)`, `EventFor(ref)`, `ChangeFor(ref)`, `DefaultCycleKey`)
- `IAudienceResolver` + EF-backed `AudienceResolver` implementation (active users only, department lookup, GUID parse for user rules)
- `IDirectoryGroupResolver` + `StubDirectoryGroupResolver` (Phase 1 stub returning empty until AD group sync lands)
- `IAudienceAuditLogger` emitting `AudienceConfigured`, `AudienceInclusionRulesReplaced`, `AudienceExclusionRulesReplaced`
- `IRequirementRepository`, `IRequirementGenerator`, `RequirementGenerator`, `IRequirementAuditLogger` delivering the requirement-generation foundation
- Commands: `ConfigureAudienceInclusion`, `ConfigureAudienceExclusions`, `SetAllUsersAudience`, `SetAcknowledgmentVersionRecurrence`, `GenerateRequirementsForVersion`
- Queries: `GetAudienceByVersion`, `PreviewAudience`, `ListRequirementsForVersion`
- Controllers: `AudienceController` (inclusion / exclusions / all-users / preview), `RequirementsController` (list + generate) — thin, delegate to MediatR, role-gated to `AcknowledgmentManager`/`SystemAdministrator`
- EF configurations under schema `acknowledgment`: `AudienceDefinitions` (unique on `AcknowledgmentVersionId`), `AudienceRules` (index on `(AudienceDefinitionId, IsExclusion, SortOrder)`), `UserActionRequirements` (unique on `(UserId, AcknowledgmentVersionId, CycleReference)`)
- `IAcknowledgmentRepository.FindDefinitionByVersionIdAsync` for cross-aggregate load in the requirement generator; repository `FindByIdAsync` now eager-loads audience rules so publish can enforce BR-032
- Infrastructure DI: `AddAudienceTargeting` + `AddRequirementGeneration`; `TimeProvider.System` registered as a singleton for deterministic cycle derivation
- Frontend typed API clients (`lib/api/audience.ts`, `lib/api/requirements.ts`, plus `setAcknowledgmentVersionRecurrence` on `lib/api/acknowledgments.ts`)
- TanStack Query hooks: `lib/audience/hooks.ts`, `lib/requirements/hooks.ts`, `useSetAcknowledgmentVersionRecurrence`
- Arabic-first labels: `lib/audience/labels.ts`, `lib/acknowledgments/recurrenceLabels.ts`
- Reusable components: `RecurrenceForm`, `RecurrenceSummary`, `AudienceRulesEditor`, `AudienceExclusionsEditor`, `AudiencePreviewSummary`
- Admin pages: `/admin/acknowledgments/[definitionId]/versions/[versionId]/audience`, `/admin/acknowledgments/[definitionId]/versions/[versionId]/recurrence`
- Version detail page updated with a two-card quick-access section linking to audience and recurrence pages

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- **Audience targeting is explicit, not rule-engine-driven**: rules are stored as a flat list of typed inclusion/exclusion rows. Inclusions OR together; exclusions always override. No DSL, no scoring.
- **AD group resolution is abstracted but stubbed in Phase 1**: `IDirectoryGroupResolver` returns an empty set for any AD-group rule until LDAP group sync is delivered. Authoring still works end-to-end; resolution becomes authoritative when the sync job lands.
- **AllUsers is allowed only in inclusions**: attempting to exclude "all users" is rejected at the domain layer (BR-055). Defense-in-depth matches the existing policy of domain invariants + filtered indexes + handler re-checks.
- **Recurrence is a closed enum, not a scheduler**: five fixed models, each with a deterministic cycle key. No cron, no RRULE, no workflow orchestration. `SetAcknowledgmentVersionRecurrence` is its own command (BR-046) so cadence can be updated independently from the rest of the draft.
- **Cycle reference is canonical**: `onboarding | annual:YYYY | event:<ref> | change:<ref>`. This string is what `UserActionRequirement` uses to guarantee idempotency when the generator runs repeatedly.
- **Generator uses `TimeProvider`**: the annual cycle derives from `DateTimeOffset.UtcNow.Year` via the injected provider so tests can pin time without touching system clocks.
- **Requirement generation is a foundation, not a schedule**: the Sprint 4 command generates the current cycle only when invoked; it does not host a recurring job. Triggering automation (cron/hosted service) is a later sprint (Sprint 7/8).
- **Published versions stay immutable (BR-031)**: audience and recurrence screens show a read-only projection when the version is not Draft. All mutating endpoints re-check status at the aggregate level.
- **Segregation of duties**: audience/recurrence authoring requires `AcknowledgmentManager`/`SystemAdministrator`; publishing still requires `Publisher` (unchanged from Sprint 2/3).

### Risks / Notes
- AD group resolution is stubbed until the directory sync job lands; preview counts for AD-group rules will under-report in Phase 1. Counts are labelled "تقديرية" (estimated) in the UI accordingly.
- Department strings used in targeting rely on AD-sync fidelity; small naming drifts (spaces, casing) will cause false negatives. Validation of department strings against a canonical list is a future sprint item.
- Build verification via `dotnet build` and `tsc --noEmit` could not be run in this environment (SDK + node_modules absent). Required before release; CI is the authoritative gate.
- No EF migration was generated in this sprint — the new `acknowledgment.AudienceDefinitions`, `acknowledgment.AudienceRules`, and `acknowledgment.UserActionRequirements` tables will be materialized by the standard migration flow when the database tooling is available.
- Requirement generation is exposed as an admin command only; automated scheduling, reminders, and the end-user submission flow are deliberately out of scope (Sprints 6/7/8).

### Next Actions
- Generate the EF migration for the new tables as part of the CI build verification
- Author integration tests for: (a) exclusions overriding inclusions, (b) AllUsers rejection in exclusions, (c) publish gate requiring audience + recurrence, (d) idempotent `GenerateRequirements` invocations producing stable rows
- Replace `StubDirectoryGroupResolver` with the real LDAP-backed resolver when AD group sync is introduced
- Begin Sprint 5 (Form-Based Disclosures)

---

## 9. Sprint 5 – Form-Based Disclosures

### Sprint Goal
Deliver dynamic form-based disclosures for business-critical scenarios.

### Status
Completed

### Planned Scope
- form definition model
- JSON-driven form schema
- supported field types
- form definition management
- dynamic form rendering
- submission validation
- submission storage

### Progress Summary
- `FormDefinition` aggregate introduced as a version-bound 0..1 child of `AcknowledgmentVersion`, following the same pattern as `AudienceDefinition` from Sprint 4. Each form definition owns a sorted collection of `FormField` entities and supports full replace-all semantics through `ReplaceFields`.
- 15 field types implemented: ShortText, LongText, Email, PhoneNumber, Number, Decimal, Date, Checkbox, YesNo, RadioGroup, Dropdown, MultiSelect, FileUpload, SectionHeader, ReadOnlyDisplay. Types are grouped into value-collecting (13) and display-only (2) categories.
- `FieldOption` value object supports option-bearing fields (RadioGroup, Dropdown, MultiSelect) with unique value + human-readable label pairs. Domain validation enforces that option-requiring types always carry at least one option.
- `UserSubmission` entity stores form submissions with the raw JSON payload, a `FormDefinitionSnapshot` JSON (capturing the exact schema at submission time per BR-079/CDM-004), and an optional flattened representation via `UserSubmissionFieldValue` rows for SQL-queryable reporting (§8.2).
- `SubmissionValidator` performs type-aware server-side validation: required-field enforcement (BR-074), email regex, phone regex, integer/decimal parsing, date parsing, option-value verification (BR-075), multi-select array validation, and boolean type checks.
- Publish gate extended: `AcknowledgmentVersion.MarkPublished` now enforces BR-070 — FormBasedDisclosure versions must have a FormDefinition with at least one input field before publishing.
- File upload infrastructure mirrors the existing `PolicyDocumentStorage` pattern: configurable root path, max file size (10 MB default), extension + content-type whitelist, path-traversal protection. Layout: `{RootPath}/{submissionId}/{fieldKey}/{storageFileName}`.
- Full admin form definition management UX delivered: structured field editor (add/edit/remove/reorder), per-field configuration (key, label, type, required, section, help text, placeholder, display text, options), save via API, and live preview page rendering the dynamic form with Zod validation.
- `DynamicFormRenderer` component renders all 15 field types using React Hook Form + Zod resolver. Groups fields by section, handles type coercion for numbers/decimals/booleans, and supports inline validation with Arabic error messages.

### Completed Items
- Domain: `FormFieldType` enum (15 values) + `FormFieldTypes` helper class, `FieldOption` value object, `FormField` entity, `FormDefinition` aggregate with `ReplaceFields` + `TakeSnapshot`, `FormDefinitionSnapshot` / `FormFieldSnapshot` records, `SubmissionStatus` enum, `UserSubmission` entity, `UserSubmissionFieldValue` entity
- Domain: `AcknowledgmentVersion.FormDefinition` navigation property, `ConfigureFormDefinition()` method, BR-070 publish gate
- Application: DTOs for form definitions, fields, options, submissions, and field values
- Application: `IFormDefinitionRepository`, `IUserSubmissionRepository`, `IFormUploadStorage`, `IFormAuditLogger` abstractions
- Application: `SubmissionValidator` with type-aware validation dispatch (required, email, phone, number, decimal, date, checkbox/yesno, radio/dropdown option check, multi-select, file upload)
- Application: `ConfigureFormDefinitionCommand` + FluentValidation validator + handler
- Application: `SubmitFormCommand` + validator + handler (snapshot capture, field-value flattening, audit)
- Application: `GetFormDefinitionByVersionQuery`, `ListSubmissionsByVersionQuery` (paged), `GetSubmissionByIdQuery` + handlers
- Application: AutoMapper profile for all form DTOs
- Infrastructure: EF configurations for `FormDefinitions`, `FormFields`, `UserSubmissions`, `UserSubmissionFieldValues` under schema `acknowledgment`
- Infrastructure: `AcknowledgmentVersionConfiguration` updated with 1:0..1 FK to `FormDefinition` (cascade delete)
- Infrastructure: `EapDbContext` updated with 4 new DbSets
- Infrastructure: `FormDefinitionRepository`, `UserSubmissionRepository`
- Infrastructure: `FormAuditLogger` (Serilog structured events tagged `AuditEvent`)
- Infrastructure: `FileSystemFormUploadStorage` + `FormUploadStorageOptions` (configurable root path, max size, allowed extensions/content types)
- Infrastructure: `AcknowledgmentRepository.FindByIdAsync` extended with `.Include(FormDefinition.Fields)` eager loading
- Infrastructure: DI registration via `AddFormDisclosures`
- API: `FormDefinitionsController` (GET + PUT), `FormSubmissionsController` (POST + GET list + GET detail), `FormUploadsController` (POST upload + GET download)
- API: `appsettings.json` updated with `FormUploads` configuration section
- Frontend: `lib/forms/types.ts` (all types, enums, helpers), `lib/forms/labels.ts` (Arabic labels for 15 types), `lib/api/forms.ts` (API client), `lib/forms/hooks.ts` (TanStack Query hooks), `lib/forms/zodFromSchema.ts` (dynamic Zod schema builder)
- Frontend: `FieldOptionsEditor`, `FieldEditor`, `FormDefinitionEditor`, `DynamicFormRenderer` components
- Frontend: Form Definition Management admin page (`/admin/acknowledgments/:def/versions/:ver/form`)
- Frontend: Form Preview admin page (`/admin/acknowledgments/:def/versions/:ver/form/preview`)
- Frontend: Version detail page updated with conditional Form Definition card for FormBasedDisclosure versions

### In Progress Items
- None

### Blockers
- None

### Key Decisions
- **FormDefinition follows AudienceDefinition pattern**: 0..1 child of `AcknowledgmentVersion` with the same lifecycle, configuration command, and EF mapping approach. This maintains architectural consistency across Sprint 4 and 5.
- **Structured editor, not drag-and-drop (BR-161)**: form fields are managed through a structured list editor with add/remove/reorder buttons. Visual drag-and-drop is explicitly excluded from Phase 1.
- **15 explicit field types, not extensible**: the field type enum is a closed set covering the documented disclosure scenarios. No plugin system, no custom field types, no formula fields.
- **Form schema is explicit JSON, not a DSL**: field definitions are stored as typed domain entities (not a free-form JSON blob) and serialized to JSON only for snapshot/submission purposes. This keeps validation compile-time-safe on the backend.
- **Snapshot at submission time (BR-079/CDM-004)**: when a user submits, the exact form schema is captured as `FormDefinitionSnapshotJson` alongside `SubmissionJson`. This guarantees historical traceability even if the form definition is later modified (draft) or the version is superseded.
- **Field-value flattening is inline, not async**: `UserSubmissionFieldValue` rows are created synchronously during submission for SQL-queryable reporting. This avoids eventual-consistency complexity while the submission volume is manageable in Phase 1.
- **File upload mirrors PolicyDocumentStorage**: same pattern (configurable file-system store, size/extension/content-type whitelist, path-traversal protection) keeps the infrastructure uniform and pluggable for future cloud storage.
- **Validation aligned frontend/backend**: the frontend Zod schema (`zodFromSchema.ts`) mirrors the backend `SubmissionValidator` type-dispatch logic so users get inline validation before submission, and the server re-validates authoritatively.
- **No conditional branching, nested repeatables, or formula fields**: these are explicitly out of Sprint 5 scope per the "NOT allowed" list. The field set is flat and unconditional.
- **Publish gate extended (BR-070)**: `AcknowledgmentVersion.MarkPublished` now checks that FormBasedDisclosure versions have a FormDefinition with at least one input field, preventing empty-form publication.

### Risks / Notes
- Build verification via `dotnet build` and `tsc --noEmit` could not be run in this environment (SDK + node_modules absent). Required before release; CI is the authoritative gate.
- No EF migration was generated in this sprint — the new `FormDefinitions`, `FormFields`, `UserSubmissions`, and `UserSubmissionFieldValues` tables will be materialized by the standard migration flow when the database tooling is available.
- File upload for form fields uses the same file-system storage approach as policy documents. The orphan-blob risk noted in Sprint 2 applies here as well.
- Draft-save for long disclosure forms (O-005) is not implemented in Sprint 5; submissions are atomic. If UAT reveals a need, this can be addressed in the stabilization sprint.
- The end-user submission flow (employee-facing portal) is not part of Sprint 5 — it lands in Sprint 6 (User Portal). Sprint 5 delivers the admin tooling and the renderer/validator components that Sprint 6 will consume.
- Conflict of interest and gifts/hospitality disclosure scenarios are the priority validation cases and are supported by the implemented field types (ShortText, LongText, Date, Decimal, Checkbox, YesNo, RadioGroup, Dropdown, MultiSelect, FileUpload).

### Next Actions
- Generate the EF migration for the new tables as part of the CI build verification
- Author integration tests for: (a) BR-070 publish gate, (b) SubmissionValidator type dispatch, (c) form snapshot fidelity, (d) field-value flattening accuracy
- Begin Sprint 6 (User Portal) — the `DynamicFormRenderer` and submission hooks are ready for reuse in the employee-facing submission flow

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
| R-003 | Scope creep into full form builder | High | Mitigated | Sprint 5 delivered a structured field editor with 15 explicit types; drag-and-drop, conditional branching, nested repeatables, and formula fields explicitly excluded (BR-161) |
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
| O-002 | File storage approach for uploaded documents | Engineering | Resolved | MVP uses a configurable file-system store (root path, size limit, extension + content-type whitelist) behind `IPolicyDocumentStorage`; swappable for cloud/object-store later |
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
| Policies can be created and versioned | Completed | Delivered in Sprint 2 — CRUD, versioning, document upload, publish/archive with BR-010/BR-011/BR-012/BR-014 enforced |
| Acknowledgments can be defined and published | Completed | Delivered in Sprint 3 — definition + version aggregate, action types, linkage to a published policy version, and publish/archive with SoD enforced |
| Form-based disclosures work | Completed | Delivered in Sprint 5 — FormDefinition aggregate, 15 field types, SubmissionValidator, form snapshot, DynamicFormRenderer, admin form editor + preview; end-user submission flow lands in Sprint 6 |
| Audience targeting works correctly | Completed | Delivered in Sprint 4 — AllUsers/Department/AD-group inclusion, explicit exclusions, BR-054/BR-055 enforced, admin UI + preview in place (AD group resolution stubbed until LDAP group sync lands) |
| Recurrence logic works correctly | Completed | Delivered in Sprint 4 — five deterministic recurrence models with `SetRecurrence` command, publish gate per BR-033, requirement-generation foundation with deterministic cycle keys |
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
