import { RecurrenceModel } from "./types";

/**
 * Arabic/English labels for the recurrence models (BR-040..BR-046). Arabic is
 * the default locale; the English strings are used only by admin diagnostics.
 */
export const recurrenceModelLabelsAr: Record<RecurrenceModel, string> = {
  [RecurrenceModel.Unspecified]: "لم يتم التحديد",
  [RecurrenceModel.OnboardingOnly]: "عند الانضمام فقط",
  [RecurrenceModel.Annual]: "سنوي",
  [RecurrenceModel.OnboardingAndAnnual]: "عند الانضمام ثم سنوياً",
  [RecurrenceModel.OnChange]: "عند حدوث تغيير",
  [RecurrenceModel.EventDriven]: "مدفوع بالأحداث",
};

export const recurrenceModelLabelsEn: Record<RecurrenceModel, string> = {
  [RecurrenceModel.Unspecified]: "Not configured",
  [RecurrenceModel.OnboardingOnly]: "Onboarding only",
  [RecurrenceModel.Annual]: "Annual",
  [RecurrenceModel.OnboardingAndAnnual]: "Onboarding + annual",
  [RecurrenceModel.OnChange]: "On change",
  [RecurrenceModel.EventDriven]: "Event-driven",
};

export const recurrenceModelDescriptionsAr: Record<RecurrenceModel, string> = {
  [RecurrenceModel.Unspecified]:
    "يجب اختيار النموذج قبل النشر.",
  [RecurrenceModel.OnboardingOnly]:
    "يُطلب من كل موظف مرة واحدة عند انضمامه للمؤسسة.",
  [RecurrenceModel.Annual]:
    "يُعاد كل سنة ميلادية اعتباراً من الأول من يناير.",
  [RecurrenceModel.OnboardingAndAnnual]:
    "يُطلب عند الانضمام ثم يُعاد سنوياً.",
  [RecurrenceModel.OnChange]:
    "يُطلب تكراره عند إبلاغ المستخدم بتغيّر ذي صلة.",
  [RecurrenceModel.EventDriven]:
    "يُطلب عند حدوث حدث عمل محدد خارج النظام.",
};

export function recurrenceModelLabel(model: RecurrenceModel): string {
  return recurrenceModelLabelsAr[model];
}

export function recurrenceModelDescription(model: RecurrenceModel): string {
  return recurrenceModelDescriptionsAr[model];
}

export const configurableRecurrenceModels: RecurrenceModel[] = [
  RecurrenceModel.OnboardingOnly,
  RecurrenceModel.Annual,
  RecurrenceModel.OnboardingAndAnnual,
  RecurrenceModel.OnChange,
  RecurrenceModel.EventDriven,
];
