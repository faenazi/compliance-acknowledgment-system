/** Arabic labels for audit log views (Sprint 8). */

export const entityTypeLabel: Record<string, string> = {
  Policy: "سياسة",
  PolicyVersion: "نسخة سياسة",
  AcknowledgmentDefinition: "تعريف إقرار",
  AcknowledgmentVersion: "نسخة إقرار",
  UserSubmission: "تقديم مستخدم",
  UserActionRequirement: "متطلب إجراء",
  FormDefinition: "تعريف نموذج",
  AudienceDefinition: "تعريف جمهور",
  User: "مستخدم",
};

export const actionTypeLabel: Record<string, string> = {
  PolicyCreated: "إنشاء سياسة",
  PolicyUpdated: "تحديث سياسة",
  PolicyArchived: "أرشفة سياسة",
  PolicyVersionCreated: "إنشاء نسخة سياسة",
  PolicyVersionPublished: "نشر نسخة سياسة",
  PolicyVersionArchived: "أرشفة نسخة سياسة",
  PolicyDocumentUploaded: "رفع مستند سياسة",
  AcknowledgmentDefinitionCreated: "إنشاء إقرار",
  AcknowledgmentDefinitionUpdated: "تحديث إقرار",
  AcknowledgmentVersionCreated: "إنشاء نسخة إقرار",
  AcknowledgmentVersionPublished: "نشر نسخة إقرار",
  AcknowledgmentVersionArchived: "أرشفة نسخة إقرار",
  FormDefinitionConfigured: "تهيئة نموذج",
  UserSubmission: "تقديم مستخدم",
  AudienceConfigured: "تهيئة جمهور",
  RequirementGenerated: "إنشاء متطلب",
  LoginSucceeded: "دخول ناجح",
  LoginFailed: "فشل دخول",
  UserProvisioned: "تسجيل مستخدم",
  NotificationSent: "إرسال إشعار",
};
