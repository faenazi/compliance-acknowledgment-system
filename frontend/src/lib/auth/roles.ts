/**
 * Canonical role names, kept in sync with Eap.Domain.Identity.SystemRoles
 * on the backend. Used for role-aware navigation and route guards.
 */
export const Roles = {
  SystemAdministrator: "SystemAdministrator",
  PolicyManager: "PolicyManager",
  AcknowledgmentManager: "AcknowledgmentManager",
  Publisher: "Publisher",
  ComplianceViewer: "ComplianceViewer",
  Auditor: "Auditor",
  EndUser: "EndUser",
} as const;

export type RoleName = (typeof Roles)[keyof typeof Roles];

/** Roles that may access the admin portal. */
export const ADMIN_ROLES: readonly RoleName[] = [
  Roles.SystemAdministrator,
  Roles.PolicyManager,
  Roles.AcknowledgmentManager,
  Roles.Publisher,
  Roles.ComplianceViewer,
  Roles.Auditor,
];

export function hasAnyRole(userRoles: readonly string[], required: readonly string[]): boolean {
  if (required.length === 0) return true;
  const owned = new Set(userRoles.map((r) => r.toLowerCase()));
  return required.some((r) => owned.has(r.toLowerCase()));
}
