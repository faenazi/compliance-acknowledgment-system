import { FormFieldType } from "./types";

/** Arabic labels for form field types (BR-073). */
export const fieldTypeLabel: Record<FormFieldType, string> = {
  [FormFieldType.ShortText]: "نص قصير",
  [FormFieldType.LongText]: "نص طويل",
  [FormFieldType.Number]: "رقم صحيح",
  [FormFieldType.Decimal]: "رقم عشري",
  [FormFieldType.Date]: "تاريخ",
  [FormFieldType.Checkbox]: "مربع اختيار",
  [FormFieldType.RadioGroup]: "اختيار واحد (Radio)",
  [FormFieldType.Dropdown]: "قائمة منسدلة",
  [FormFieldType.MultiSelect]: "اختيار متعدد",
  [FormFieldType.YesNo]: "نعم / لا",
  [FormFieldType.Email]: "بريد إلكتروني",
  [FormFieldType.PhoneNumber]: "رقم هاتف",
  [FormFieldType.FileUpload]: "رفع ملف",
  [FormFieldType.ReadOnlyDisplay]: "نص للقراءة فقط",
  [FormFieldType.SectionHeader]: "عنوان قسم",
};

/** Short description of each field type for the field editor. */
export const fieldTypeDescription: Record<FormFieldType, string> = {
  [FormFieldType.ShortText]: "حقل نصي قصير (سطر واحد)",
  [FormFieldType.LongText]: "حقل نصي متعدد الأسطر",
  [FormFieldType.Number]: "رقم صحيح",
  [FormFieldType.Decimal]: "رقم عشري",
  [FormFieldType.Date]: "تاريخ بالتقويم",
  [FormFieldType.Checkbox]: "مربع اختيار (صح / خطأ)",
  [FormFieldType.RadioGroup]: "اختيار واحد من مجموعة خيارات",
  [FormFieldType.Dropdown]: "قائمة منسدلة — اختيار واحد",
  [FormFieldType.MultiSelect]: "اختيار متعدد من قائمة خيارات",
  [FormFieldType.YesNo]: "سؤال بنعم أو لا",
  [FormFieldType.Email]: "حقل بريد إلكتروني مع تحقق",
  [FormFieldType.PhoneNumber]: "رقم هاتف مع تحقق",
  [FormFieldType.FileUpload]: "رفع ملف (حسب القيود المسموحة)",
  [FormFieldType.ReadOnlyDisplay]: "نص ثابت للعرض فقط — لا يُجمع",
  [FormFieldType.SectionHeader]: "عنوان قسم — لا يُجمع",
};
