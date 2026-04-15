import { PolicyStatus, PolicyVersionStatus } from "./types";
import type { StatusKey } from "@/lib/tokens/design-tokens";

/**
 * Arabic-first localized labels and badge palettes for policy + version status.
 * Kept in one module so tables, badges, and filters agree on terminology.
 */

export const policyStatusLabel: Record<PolicyStatus, string> = {
  [PolicyStatus.Draft]: "مسودة",
  [PolicyStatus.Published]: "منشورة",
  [PolicyStatus.Archived]: "مؤرشفة",
};

export const policyStatusBadge: Record<PolicyStatus, StatusKey> = {
  [PolicyStatus.Draft]: "draft",
  [PolicyStatus.Published]: "published",
  [PolicyStatus.Archived]: "archived",
};

export const versionStatusLabel: Record<PolicyVersionStatus, string> = {
  [PolicyVersionStatus.Draft]: "مسودة",
  [PolicyVersionStatus.Published]: "منشورة",
  [PolicyVersionStatus.Superseded]: "مستبدلة",
  [PolicyVersionStatus.Archived]: "مؤرشفة",
};

export const versionStatusBadge: Record<PolicyVersionStatus, StatusKey> = {
  [PolicyVersionStatus.Draft]: "draft",
  [PolicyVersionStatus.Published]: "published",
  [PolicyVersionStatus.Superseded]: "superseded",
  [PolicyVersionStatus.Archived]: "archived",
};

export function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} بايت`;
  const kb = bytes / 1024;
  if (kb < 1024) return `${kb.toFixed(1)} كيلوبايت`;
  const mb = kb / 1024;
  return `${mb.toFixed(1)} ميجابايت`;
}

/** Parses ApiError payloads into a flat field→message map for form-level display. */
export function collectFieldErrors(errors?: Record<string, string[]>): Record<string, string> {
  if (!errors) return {};
  return Object.fromEntries(
    Object.entries(errors).map(([field, msgs]) => [
      // Handler-side validators use PascalCase property names; hook-form uses camelCase.
      field.charAt(0).toLowerCase() + field.slice(1),
      msgs.join(" "),
    ]),
  );
}
