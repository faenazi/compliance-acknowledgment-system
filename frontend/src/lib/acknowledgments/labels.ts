import {
  AcknowledgmentStatus,
  AcknowledgmentVersionStatus,
  ActionType,
} from "./types";
import type { StatusKey } from "@/lib/tokens/design-tokens";

/**
 * Arabic-first localized labels and badge palettes for acknowledgment +
 * version status. Kept in one module so tables, badges, and filters agree
 * on terminology.
 */

export const acknowledgmentStatusLabel: Record<AcknowledgmentStatus, string> = {
  [AcknowledgmentStatus.Draft]: "مسودة",
  [AcknowledgmentStatus.Published]: "منشور",
  [AcknowledgmentStatus.Archived]: "مؤرشف",
};

export const acknowledgmentStatusBadge: Record<AcknowledgmentStatus, StatusKey> = {
  [AcknowledgmentStatus.Draft]: "draft",
  [AcknowledgmentStatus.Published]: "published",
  [AcknowledgmentStatus.Archived]: "archived",
};

export const acknowledgmentVersionStatusLabel: Record<AcknowledgmentVersionStatus, string> = {
  [AcknowledgmentVersionStatus.Draft]: "مسودة",
  [AcknowledgmentVersionStatus.Published]: "منشورة",
  [AcknowledgmentVersionStatus.Superseded]: "مستبدلة",
  [AcknowledgmentVersionStatus.Archived]: "مؤرشفة",
};

export const acknowledgmentVersionStatusBadge: Record<AcknowledgmentVersionStatus, StatusKey> = {
  [AcknowledgmentVersionStatus.Draft]: "draft",
  [AcknowledgmentVersionStatus.Published]: "published",
  [AcknowledgmentVersionStatus.Superseded]: "superseded",
  [AcknowledgmentVersionStatus.Archived]: "archived",
};

export const actionTypeLabel: Record<ActionType, string> = {
  [ActionType.SimpleAcknowledgment]: "إقرار بسيط",
  [ActionType.AcknowledgmentWithCommitment]: "إقرار مع تعهّد",
  [ActionType.FormBasedDisclosure]: "إفصاح عبر نموذج",
};

export const actionTypeDescription: Record<ActionType, string> = {
  [ActionType.SimpleAcknowledgment]: "يؤكّد الموظف اطّلاعه على السياسة فقط.",
  [ActionType.AcknowledgmentWithCommitment]:
    "يوقّع الموظف على نص تعهّد مكتوب بجانب الإقرار.",
  [ActionType.FormBasedDisclosure]:
    "يُعبّئ الموظف نموذج إفصاح مُخصّص (يُفعّل في مرحلة لاحقة).",
};

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
