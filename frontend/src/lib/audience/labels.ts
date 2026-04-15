import { AudienceRuleType, AudienceType } from "./types";

/**
 * Human-readable labels (Arabic + English) for audience rule/audience types.
 * The platform defaults to Arabic — English strings are kept as fallbacks for
 * the rare language-switched view.
 */
export const audienceRuleTypeLabelsAr: Record<AudienceRuleType, string> = {
  [AudienceRuleType.AllUsers]: "كل المستخدمين",
  [AudienceRuleType.Department]: "قسم",
  [AudienceRuleType.AdGroup]: "مجموعة Active Directory",
  [AudienceRuleType.User]: "مستخدم محدد",
};

export const audienceRuleTypeLabelsEn: Record<AudienceRuleType, string> = {
  [AudienceRuleType.AllUsers]: "All users",
  [AudienceRuleType.Department]: "Department",
  [AudienceRuleType.AdGroup]: "AD group",
  [AudienceRuleType.User]: "Specific user",
};

export const audienceTypeLabelsAr: Record<AudienceType, string> = {
  [AudienceType.AllUsers]: "كل المستخدمين",
  [AudienceType.Departments]: "أقسام محددة",
  [AudienceType.AdGroups]: "مجموعات Active Directory",
  [AudienceType.Mixed]: "قواعد مختلطة",
};

export function audienceRuleTypeLabel(type: AudienceRuleType): string {
  return audienceRuleTypeLabelsAr[type];
}

export function audienceTypeLabel(type: AudienceType): string {
  return audienceTypeLabelsAr[type];
}
