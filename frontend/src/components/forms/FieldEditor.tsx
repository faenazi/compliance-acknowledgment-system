"use client";

import { FormFieldType, requiresOptions, isDisplayOnly, type FormFieldInput, type FieldOption } from "@/lib/forms/types";
import { fieldTypeLabel } from "@/lib/forms/labels";
import { FieldOptionsEditor } from "./FieldOptionsEditor";

interface Props {
  field: FormFieldInput;
  index: number;
  total: number;
  onChange: (updated: FormFieldInput) => void;
  onRemove: () => void;
  onMoveUp: () => void;
  onMoveDown: () => void;
  disabled?: boolean;
}

/**
 * Editor for a single form field definition. Shows type-appropriate inputs
 * and an options sub-editor for select-based types.
 */
export function FieldEditor({
  field,
  index,
  total,
  onChange,
  onRemove,
  onMoveUp,
  onMoveDown,
  disabled,
}: Props) {
  const update = (partial: Partial<FormFieldInput>) => onChange({ ...field, ...partial });

  return (
    <div className="rounded-[10px] border border-[var(--color-border-default)] bg-[var(--color-surface-secondary)] p-4">
      <div className="mb-3 flex items-center justify-between">
        <span className="text-sm font-semibold">
          حقل {index + 1}: {fieldTypeLabel[field.fieldType]}
        </span>
        {!disabled && (
          <div className="flex items-center gap-1">
            <button type="button" onClick={onMoveUp} disabled={index === 0} className="rounded px-2 py-0.5 text-xs hover:bg-[var(--color-surface-hover)] disabled:opacity-30">▲</button>
            <button type="button" onClick={onMoveDown} disabled={index >= total - 1} className="rounded px-2 py-0.5 text-xs hover:bg-[var(--color-surface-hover)] disabled:opacity-30">▼</button>
            <button type="button" onClick={onRemove} className="rounded px-2 py-0.5 text-xs text-red-600 hover:bg-red-50">حذف</button>
          </div>
        )}
      </div>

      <div className="grid gap-3 md:grid-cols-2">
        <div>
          <label className="text-xs text-[var(--color-text-tertiary)]">المفتاح (Field Key)</label>
          <input
            type="text"
            value={field.fieldKey}
            onChange={(e) => update({ fieldKey: e.target.value })}
            disabled={disabled}
            dir="ltr"
            className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm disabled:opacity-60"
          />
        </div>
        <div>
          <label className="text-xs text-[var(--color-text-tertiary)]">التسمية (Label)</label>
          <input
            type="text"
            value={field.label}
            onChange={(e) => update({ label: e.target.value })}
            disabled={disabled}
            className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm disabled:opacity-60"
          />
        </div>
        <div>
          <label className="text-xs text-[var(--color-text-tertiary)]">نوع الحقل</label>
          <select
            value={field.fieldType}
            onChange={(e) => update({ fieldType: Number(e.target.value) as FormFieldType })}
            disabled={disabled}
            className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm disabled:opacity-60"
          >
            {Object.entries(fieldTypeLabel).map(([val, lbl]) => (
              <option key={val} value={val}>{lbl}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="text-xs text-[var(--color-text-tertiary)]">القسم (Section Key)</label>
          <input
            type="text"
            value={field.sectionKey ?? ""}
            onChange={(e) => update({ sectionKey: e.target.value || null })}
            disabled={disabled}
            dir="ltr"
            className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm disabled:opacity-60"
          />
        </div>
      </div>

      {!isDisplayOnly(field.fieldType) && (
        <div className="mt-3 flex items-center gap-2">
          <input
            type="checkbox"
            checked={field.isRequired}
            onChange={(e) => update({ isRequired: e.target.checked })}
            disabled={disabled}
            id={`req-${index}`}
          />
          <label htmlFor={`req-${index}`} className="text-sm">مطلوب</label>
        </div>
      )}

      <div className="mt-3 grid gap-3 md:grid-cols-2">
        <div>
          <label className="text-xs text-[var(--color-text-tertiary)]">نص المساعدة</label>
          <input
            type="text"
            value={field.helpText ?? ""}
            onChange={(e) => update({ helpText: e.target.value || null })}
            disabled={disabled}
            className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm disabled:opacity-60"
          />
        </div>
        <div>
          <label className="text-xs text-[var(--color-text-tertiary)]">نص التعليمات (Placeholder)</label>
          <input
            type="text"
            value={field.placeholder ?? ""}
            onChange={(e) => update({ placeholder: e.target.value || null })}
            disabled={disabled}
            className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm disabled:opacity-60"
          />
        </div>
      </div>

      {isDisplayOnly(field.fieldType) && (
        <div className="mt-3">
          <label className="text-xs text-[var(--color-text-tertiary)]">النص المعروض</label>
          <textarea
            value={field.displayText ?? ""}
            onChange={(e) => update({ displayText: e.target.value || null })}
            disabled={disabled}
            rows={2}
            className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm disabled:opacity-60"
          />
        </div>
      )}

      {requiresOptions(field.fieldType) && (
        <div className="mt-3">
          <FieldOptionsEditor
            options={field.options ?? []}
            onChange={(opts: FieldOption[]) => update({ options: opts })}
            disabled={disabled}
          />
        </div>
      )}
    </div>
  );
}
