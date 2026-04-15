# Implementation Rules

These rules are non-negotiable. Any implementation that violates them must be rejected, regardless of convenience.

## Versioning Rules

1. Every policy has one or more versions. A policy without at least one version is invalid.
2. A version is in exactly one state: `Draft`, `Published`, or `Archived`.
3. Once a version transitions to `Published`, its content, metadata, and effective date are **immutable**. No update, patch, or "silent fix" is permitted.
4. Corrections to published content are made by creating a new version. The previous version is archived, not deleted.
5. Archived versions remain readable forever for audit purposes.

## Acknowledgment Rules

1. Every acknowledgment **must** reference a specific `PolicyVersion.Id`. Acknowledgments against a policy without a version are invalid and must be rejected at the domain layer.
2. An acknowledgment is immutable once recorded. It cannot be edited or deleted — only superseded by a newer acknowledgment against a newer version.
3. When a new version of a policy is published, prior acknowledgments do **not** carry over. Users must re-acknowledge the new version.
4. The acknowledgment record must capture, at minimum: user ID, policy version ID, UTC timestamp, and source IP / user agent.

## Publishing Rules

1. Published content cannot be modified. This applies to policy body, title, category, and any embedded assets referenced by the version.
2. Only Compliance Administrators may publish. The act of publishing is itself an audited event.
3. A draft may be edited freely; a published version may not.
4. Unpublishing is not supported. A published version is either superseded (new version published) or archived.

## Audit Logging Rules

1. Every state-changing action produces exactly one audit log entry. No action is exempt.
2. Audit entries are append-only. There is no update or delete path, at any layer, for any reason.
3. Each entry records: actor (user ID), action type, target entity type and ID, UTC timestamp, and a structured payload of before/after values where applicable.
4. Read-only actions are not audited unless they involve sensitive exports (e.g., compliance reports); those exports are audited.
5. Audit log storage must be separable from operational data so it can be retained independently.

## Authorization Rules

1. Role-based: `Employee`, `ComplianceAdministrator`, `Auditor`. No implicit permissions.
2. Employees can only read their own acknowledgments and assigned policies.
3. Auditors are strictly read-only across all data.
4. No endpoint may infer permissions from client-supplied role claims without server-side verification.

## Portal Rules

1. The User Portal must be full-featured: listing assigned policies, reading them, acknowledging, and viewing personal history must all be first-class flows.
2. The Admin Portal must remain lean. It implements only what compliance operations require; it is not a general-purpose CMS.
3. The two portals share the same backend but must not share UI navigation, layout, or access surfaces.

## Data Integrity Rules

1. Referential integrity between `Acknowledgment → PolicyVersion → Policy` must be enforced at the database level, not only in application code.
2. Soft-deletes are not permitted for `Policy`, `PolicyVersion`, `Acknowledgment`, or `AuditLog`. Hard deletes are also not permitted for these entities in production.
3. All timestamps are stored in UTC. Display-time localization is a UI concern.

## Change Control

1. Any schema change affecting `Policy`, `PolicyVersion`, `Acknowledgment`, or `AuditLog` requires an explicit migration and is itself an auditable event in the deployment record.
2. No direct database writes to these tables outside of the application's domain services.
