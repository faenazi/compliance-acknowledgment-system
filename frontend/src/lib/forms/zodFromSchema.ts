import { z } from "zod";
import { FormFieldType, isDisplayOnly, type FormFieldDto } from "./types";

/**
 * Generates a Zod schema from a list of form fields so the dynamic renderer
 * can perform inline validation aligned with the backend (BR-074..BR-077).
 */
export function buildZodSchema(fields: FormFieldDto[]): z.ZodObject<Record<string, z.ZodTypeAny>> {
  const shape: Record<string, z.ZodTypeAny> = {};

  for (const field of fields) {
    if (isDisplayOnly(field.fieldType)) continue;

    let fieldSchema: z.ZodTypeAny;

    switch (field.fieldType) {
      case FormFieldType.ShortText:
      case FormFieldType.LongText:
      case FormFieldType.PhoneNumber:
        fieldSchema = z.string();
        break;

      case FormFieldType.Email:
        fieldSchema = z.string().email("يجب إدخال بريد إلكتروني صالح");
        break;

      case FormFieldType.Number:
        fieldSchema = z.number({ invalid_type_error: "يجب إدخال رقم صحيح" }).int("يجب إدخال رقم صحيح");
        break;

      case FormFieldType.Decimal:
        fieldSchema = z.number({ invalid_type_error: "يجب إدخال رقم" });
        break;

      case FormFieldType.Date:
        fieldSchema = z.string().regex(/^\d{4}-\d{2}-\d{2}$/, "يجب إدخال تاريخ صالح (yyyy-MM-dd)");
        break;

      case FormFieldType.Checkbox:
      case FormFieldType.YesNo:
        fieldSchema = z.boolean();
        break;

      case FormFieldType.RadioGroup:
      case FormFieldType.Dropdown: {
        const allowed = field.options.map((o) => o.value);
        fieldSchema = z.string().refine(
          (v) => allowed.includes(v),
          { message: "يرجى اختيار قيمة صالحة" },
        );
        break;
      }

      case FormFieldType.MultiSelect: {
        const allowedMulti = field.options.map((o) => o.value);
        fieldSchema = z.array(z.string()).refine(
          (arr) => arr.every((v) => allowedMulti.includes(v)),
          { message: "إحدى القيم المحددة غير صالحة" },
        );
        break;
      }

      case FormFieldType.FileUpload:
        fieldSchema = z.string();
        break;

      default:
        fieldSchema = z.any();
    }

    if (field.isRequired) {
      if (fieldSchema instanceof z.ZodString) {
        fieldSchema = (fieldSchema as z.ZodString).min(1, "هذا الحقل مطلوب");
      }
      // For arrays (multi-select) add min(1)
      if (fieldSchema instanceof z.ZodArray) {
        fieldSchema = (fieldSchema as z.ZodArray<z.ZodString>).min(1, "يجب اختيار قيمة واحدة على الأقل");
      }
    } else {
      fieldSchema = fieldSchema.optional().or(z.literal("")).or(z.null());
    }

    shape[field.fieldKey] = fieldSchema;
  }

  return z.object(shape);
}
