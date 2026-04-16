"use client";

import { useForm, Controller, type SubmitHandler } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "@/components/ui/button";
import { FormFieldType, isDisplayOnly, type FormFieldDto, type FormDefinitionDto } from "@/lib/forms/types";
import { fieldTypeLabel } from "@/lib/forms/labels";
import { buildZodSchema } from "@/lib/forms/zodFromSchema";

interface Props {
  formDefinition: FormDefinitionDto;
  onSubmit: (values: Record<string, unknown>) => Promise<void>;
  disabled?: boolean;
  submitLabel?: string;
}

/**
 * Dynamic form renderer (Sprint 5). Renders fields from a FormDefinition,
 * validates inline using Zod, and submits structured values.
 */
export function DynamicFormRenderer({
  formDefinition,
  onSubmit,
  disabled,
  submitLabel = "إرسال",
}: Props) {
  const schema = buildZodSchema(formDefinition.fields);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors, isSubmitting },
  } = useForm<Record<string, unknown>>({
    resolver: zodResolver(schema),
    defaultValues: buildDefaults(formDefinition.fields),
  });

  const submit: SubmitHandler<Record<string, unknown>> = async (values) => {
    await onSubmit(values);
  };

  // Group fields by sectionKey
  const sections = groupBySection(formDefinition.fields);

  return (
    <form onSubmit={handleSubmit(submit)} className="space-y-6" noValidate>
      {sections.map(({ sectionKey, fields }) => (
        <div key={sectionKey ?? "__root__"}>
          {sectionKey && (
            <h3 className="mb-3 border-b border-[var(--color-border-default)] pb-1 text-lg font-semibold">
              {sectionKey}
            </h3>
          )}
          <div className="space-y-4">
            {fields.map((field) => (
              <FieldRenderer
                key={field.id}
                field={field}
                register={register}
                control={control}
                error={errors[field.fieldKey]?.message as string | undefined}
                disabled={disabled || isSubmitting}
              />
            ))}
          </div>
        </div>
      ))}

      <div className="flex justify-end pt-2">
        <Button type="submit" disabled={disabled || isSubmitting}>
          {isSubmitting ? "جاري الإرسال…" : submitLabel}
        </Button>
      </div>
    </form>
  );
}

function FieldRenderer({
  field,
  register,
  control,
  error,
  disabled,
}: {
  field: FormFieldDto;
  register: ReturnType<typeof useForm>["register"];
  control: ReturnType<typeof useForm>["control"];
  error?: string;
  disabled?: boolean;
}) {
  // Section headers
  if (field.fieldType === FormFieldType.SectionHeader) {
    return (
      <div className="border-b border-[var(--color-border-default)] pb-1">
        <h4 className="text-base font-semibold">{field.label}</h4>
        {field.displayText && (
          <p className="mt-1 text-sm text-[var(--color-text-secondary)]">{field.displayText}</p>
        )}
      </div>
    );
  }

  // Read-only display
  if (field.fieldType === FormFieldType.ReadOnlyDisplay) {
    return (
      <div className="rounded-[10px] bg-[var(--color-surface-secondary)] p-3">
        <p className="text-sm font-medium">{field.label}</p>
        {field.displayText && (
          <p className="mt-1 whitespace-pre-wrap text-sm text-[var(--color-text-secondary)]">{field.displayText}</p>
        )}
      </div>
    );
  }

  const id = `field-${field.fieldKey}`;

  return (
    <div>
      <label htmlFor={id} className="mb-1 block text-sm font-medium">
        {field.label}
        {field.isRequired && <span className="text-red-600 mr-1">*</span>}
      </label>
      {field.helpText && (
        <p className="mb-1 text-xs text-[var(--color-text-tertiary)]">{field.helpText}</p>
      )}

      {renderInput(field, id, register, control, disabled)}

      {error && <p className="mt-1 text-xs text-red-600">{error}</p>}
    </div>
  );
}

function renderInput(
  field: FormFieldDto,
  id: string,
  register: ReturnType<typeof useForm>["register"],
  control: ReturnType<typeof useForm>["control"],
  disabled?: boolean,
) {
  const inputClass =
    "block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-2 text-sm disabled:opacity-60";

  switch (field.fieldType) {
    case FormFieldType.ShortText:
    case FormFieldType.PhoneNumber:
      return (
        <input
          id={id}
          type={field.fieldType === FormFieldType.PhoneNumber ? "tel" : "text"}
          placeholder={field.placeholder ?? undefined}
          disabled={disabled}
          className={inputClass}
          dir={field.fieldType === FormFieldType.PhoneNumber ? "ltr" : undefined}
          {...register(field.fieldKey)}
        />
      );

    case FormFieldType.Email:
      return (
        <input
          id={id}
          type="email"
          placeholder={field.placeholder ?? undefined}
          disabled={disabled}
          className={inputClass}
          dir="ltr"
          {...register(field.fieldKey)}
        />
      );

    case FormFieldType.LongText:
      return (
        <textarea
          id={id}
          placeholder={field.placeholder ?? undefined}
          disabled={disabled}
          rows={4}
          className={inputClass}
          {...register(field.fieldKey)}
        />
      );

    case FormFieldType.Number:
      return (
        <Controller
          name={field.fieldKey}
          control={control}
          render={({ field: f }) => (
            <input
              id={id}
              type="number"
              step="1"
              placeholder={field.placeholder ?? undefined}
              disabled={disabled}
              className={inputClass}
              dir="ltr"
              value={f.value ?? ""}
              onChange={(e) => f.onChange(e.target.value ? parseInt(e.target.value, 10) : undefined)}
            />
          )}
        />
      );

    case FormFieldType.Decimal:
      return (
        <Controller
          name={field.fieldKey}
          control={control}
          render={({ field: f }) => (
            <input
              id={id}
              type="number"
              step="any"
              placeholder={field.placeholder ?? undefined}
              disabled={disabled}
              className={inputClass}
              dir="ltr"
              value={f.value ?? ""}
              onChange={(e) => f.onChange(e.target.value ? parseFloat(e.target.value) : undefined)}
            />
          )}
        />
      );

    case FormFieldType.Date:
      return (
        <input
          id={id}
          type="date"
          disabled={disabled}
          className={inputClass}
          dir="ltr"
          {...register(field.fieldKey)}
        />
      );

    case FormFieldType.Checkbox:
      return (
        <Controller
          name={field.fieldKey}
          control={control}
          render={({ field: f }) => (
            <div className="flex items-center gap-2">
              <input
                id={id}
                type="checkbox"
                disabled={disabled}
                checked={!!f.value}
                onChange={(e) => f.onChange(e.target.checked)}
              />
              <label htmlFor={id} className="text-sm">{field.placeholder ?? "نعم"}</label>
            </div>
          )}
        />
      );

    case FormFieldType.YesNo:
      return (
        <Controller
          name={field.fieldKey}
          control={control}
          render={({ field: f }) => (
            <div className="flex items-center gap-4">
              <label className="flex items-center gap-1 text-sm">
                <input
                  type="radio"
                  disabled={disabled}
                  checked={f.value === true}
                  onChange={() => f.onChange(true)}
                />
                نعم
              </label>
              <label className="flex items-center gap-1 text-sm">
                <input
                  type="radio"
                  disabled={disabled}
                  checked={f.value === false}
                  onChange={() => f.onChange(false)}
                />
                لا
              </label>
            </div>
          )}
        />
      );

    case FormFieldType.RadioGroup:
      return (
        <div className="space-y-1">
          {field.options.map((opt) => (
            <label key={opt.value} className="flex items-center gap-2 text-sm">
              <input
                type="radio"
                value={opt.value}
                disabled={disabled}
                {...register(field.fieldKey)}
              />
              {opt.label}
            </label>
          ))}
        </div>
      );

    case FormFieldType.Dropdown:
      return (
        <select
          id={id}
          disabled={disabled}
          className={inputClass}
          {...register(field.fieldKey)}
        >
          <option value="">{field.placeholder ?? "اختر..."}</option>
          {field.options.map((opt) => (
            <option key={opt.value} value={opt.value}>{opt.label}</option>
          ))}
        </select>
      );

    case FormFieldType.MultiSelect:
      return (
        <Controller
          name={field.fieldKey}
          control={control}
          render={({ field: f }) => {
            const selected: string[] = Array.isArray(f.value) ? f.value : [];
            return (
              <div className="space-y-1">
                {field.options.map((opt) => (
                  <label key={opt.value} className="flex items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      disabled={disabled}
                      checked={selected.includes(opt.value)}
                      onChange={(e) => {
                        const next = e.target.checked
                          ? [...selected, opt.value]
                          : selected.filter((v) => v !== opt.value);
                        f.onChange(next);
                      }}
                    />
                    {opt.label}
                  </label>
                ))}
              </div>
            );
          }}
        />
      );

    case FormFieldType.FileUpload:
      return (
        <input
          id={id}
          type="text"
          placeholder="مرجع الملف المرفوع"
          disabled={disabled}
          className={inputClass}
          dir="ltr"
          {...register(field.fieldKey)}
        />
      );

    default:
      return <p className="text-xs text-[var(--color-text-tertiary)]">نوع الحقل غير مدعوم: {fieldTypeLabel[field.fieldType]}</p>;
  }
}

function groupBySection(fields: FormFieldDto[]) {
  const groups: { sectionKey: string | null; fields: FormFieldDto[] }[] = [];
  const seen = new Set<string | null>();
  for (const field of fields) {
    const key = field.sectionKey;
    if (!seen.has(key)) {
      seen.add(key);
      groups.push({ sectionKey: key, fields: [] });
    }
    groups.find((g) => g.sectionKey === key)!.fields.push(field);
  }
  return groups;
}

function buildDefaults(fields: FormFieldDto[]): Record<string, unknown> {
  const defaults: Record<string, unknown> = {};
  for (const field of fields) {
    if (isDisplayOnly(field.fieldType)) continue;
    switch (field.fieldType) {
      case FormFieldType.Checkbox:
      case FormFieldType.YesNo:
        defaults[field.fieldKey] = false;
        break;
      case FormFieldType.MultiSelect:
        defaults[field.fieldKey] = [];
        break;
      default:
        defaults[field.fieldKey] = "";
    }
  }
  return defaults;
}
