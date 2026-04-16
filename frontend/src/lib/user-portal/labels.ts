import { UserActionRequirementStatus } from "./types";
import type { StatusKey } from "@/lib/tokens/design-tokens";

/**
 * Arabic-first labels and badge palettes for user action requirement statuses.
 */

export const requirementStatusLabel: Record<UserActionRequirementStatus, string> = {
  [UserActionRequirementStatus.Pending]: "معلّق",
  [UserActionRequirementStatus.Completed]: "مكتمل",
  [UserActionRequirementStatus.Overdue]: "متأخر",
  [UserActionRequirementStatus.Cancelled]: "ملغى",
};

export const requirementStatusBadge: Record<UserActionRequirementStatus, StatusKey> = {
  [UserActionRequirementStatus.Pending]: "pending",
  [UserActionRequirementStatus.Completed]: "completed",
  [UserActionRequirementStatus.Overdue]: "overdue",
  [UserActionRequirementStatus.Cancelled]: "archived",
};

/** Formats a date string to a human-readable Arabic format. */
export function formatDateAr(dateStr: string | null | undefined): string {
  if (!dateStr) return "—";
  try {
    const date = new Date(dateStr);
    return date.toLocaleDateString("ar-SA", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  } catch {
    return dateStr;
  }
}

/** Formats a datetime string with time. */
export function formatDateTimeAr(dateStr: string | null | undefined): string {
  if (!dateStr) return "—";
  try {
    const date = new Date(dateStr);
    return date.toLocaleDateString("ar-SA", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch {
    return dateStr;
  }
}
