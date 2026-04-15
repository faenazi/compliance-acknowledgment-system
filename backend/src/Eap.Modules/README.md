# Modules

This folder hosts the feature modules of the EAP backend.

Per the documented architecture (Modular + Vertical Slice), each module is
self-contained and owns its features end-to-end: endpoints, request/response
DTOs, validators, MediatR handlers, domain logic, and persistence
configurations.

Planned modules (delivered in later sprints, not in Sprint 0):

- `Eap.Modules.Identity` — users, roles, scopes, LDAP/AD auth (Sprint 1)
- `Eap.Modules.Policies` — policies, versions, documents (Sprint 2)
- `Eap.Modules.Acknowledgments` — acknowledgment definitions and versions (Sprint 3)
- `Eap.Modules.Audience` — audience targeting and recurrence (Sprint 4)
- `Eap.Modules.Forms` — form-based disclosures (Sprint 5)
- `Eap.Modules.UserPortal` — user-facing read/submit flows (Sprint 6)
- `Eap.Modules.AdminPortal` — admin-facing operational queries (Sprint 7)
- `Eap.Modules.Compliance` — compliance/reporting queries (Sprint 8)
- `Eap.Modules.Notifications` — Exchange notification pipeline (Sprint 8)
- `Eap.Modules.Audit` — audit log capture and retrieval (Sprint 8)

No module projects are created in Sprint 0. Each module will be added as a
separate `.csproj` (class library) with its own `DependencyInjection.cs`,
referenced by `Eap.Api` and/or `Eap.Infrastructure` as appropriate.
