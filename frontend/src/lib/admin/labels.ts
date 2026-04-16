import { RecurrenceModel } from "@/lib/acknowledgments/types";
import { UserActionRequirementStatus } from "@/lib/requirements/types";
import type { StatusKey } from "@/lib/tokens/design-tokens";

/** Arabic labels for requirement statuses used in the admin monitoring table. */
export const requirementStatusLabel: Record<UserActionRequirementStatus, string> = {
  [UserActionRequirementStatus.Pending]: "معلّق",
  [UserActionRequirementStatus.Completed]: "مكتمل",
  [UserActionRequirementStatus.Overdue]: "متأخر",
  [UserActionRequirementStatus.Cancelled]: "ملغي",
};

/** Badge palette mapping for requirement statuses. */
export const requirementStatusBadge: Record<UserActionRequirementStatus, StatusKey> = {
  [UserActionRequirementStatus.Pending]: "pending",
  [UserActionRequirementStatus.Completed]: "completed",
  [UserActionRequirementStatus.Overdue]: "overdue",
  [UserActionRequirementStatus.Cancelled]: "archived",
};

/** Arabic labels for recurrence models used in filters and tables. */
export const recurrenceModelLabel: Record<RecurrenceModel, string> = {
  [RecurrenceModel.Unspecified]: "غير محدد",
  [RecurrenceModel.OnboardingOnly]: "عند الالتحاق فقط",
  [RecurrenceModel.Annual]: "سنوي",
  [RecurrenceModel.OnboardingAndAnnual]: "التحاق + سنوي",
  [RecurrenceModel.OnChange]: "عند التغيير",
  [RecurrenceModel.EventDriven]: "حسب الحدث",
};

/** Utility: format an ISO date string to a readable Arabic-friendly date. */
export function formatDate(isoDate: string | null | undefined): string {
  if (!isoDate) return "—";
  try {
    return new Date(isoDate).toLocaleDateString("ar-SA", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  } catch {
    return isoDate;
  }
}

/** Utility: format an ISO datetime string to a readable timestamp. */
export function formatDateTime(isoDateTime: string | null | undefined): string {
  if (!isoDateTime) return "—";
  try {
    return new Date(isoDateTime).toLocaleString("ar-SA", {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch {
    return isoDateTime;
  }
}
