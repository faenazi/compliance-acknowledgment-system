"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { FormFieldType, type FormFieldInput } from "@/lib/forms/types";
import { fieldTypeLabel } from "@/lib/forms/labels";
import { FieldEditor } from "./FieldEditor";

interface Props {
  initialFields: FormFieldInput[];
  onSave: (fields: FormFieldInput[]) => Promise<void>;
  disabled?: boolean;
}

/**
 * Full form definition editor (admin-portal-pages §13).
 * Supports add, edit, remove, and reorder fields. Not drag-and-drop (BR-161).
 */
export function FormDefinitionEditor({ initialFields, onSave, disabled }: Props) {
  const [fields, setFields] = useState<FormFieldInput[]>(initialFields);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [addType, setAddType] = useState<FormFieldType>(FormFieldType.ShortText);

  const addField = () => {
    const key = `field_${Date.now()}`;
    const newField: FormFieldInput = {
      fieldKey: key,
      label: "",
      fieldType: addType,
      isRequired: false,
      sectionKey: null,
      helpText: null,
      placeholder: null,
      displayText: null,
      options: null,
    };
    setFields((prev) => [...prev, newField]);
  };

  const updateField = (index: number, updated: FormFieldInput) => {
    setFields((prev) => prev.map((f, i) => (i === index ? updated : f)));
  };

  const removeField = (index: number) => {
    setFields((prev) => prev.filter((_, i) => i !== index));
  };

  const moveUp = (index: number) => {
    if (index === 0) return;
    setFields((prev) => {
      const next = [...prev];
      [next[index - 1], next[index]] = [next[index], next[index - 1]];
      return next;
    });
  };

  const moveDown = (index: number) => {
    setFields((prev) => {
      if (index >= prev.length - 1) return prev;
      const next = [...prev];
      [next[index], next[index + 1]] = [next[index + 1], next[index]];
      return next;
    });
  };

  const handleSave = async () => {
    setError(null);
    setSaving(true);
    try {
      await onSave(fields);
    } catch (e: unknown) {
      const err = e as { title?: string; detail?: string };
      setError(err?.detail ?? err?.title ?? "تعذّر حفظ تعريف النموذج.");
    } finally {
      setSaving(false);
    }
  };

  // Group fields by sectionKey for visual organization
  const sections = groupBySections(fields);

  return (
    <div className="space-y-4">
      {error && (
        <div role="alert" className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800">
          {error}
        </div>
      )}

      {sections.map(({ sectionKey, sectionFields }) => (
        <div key={sectionKey ?? "__root__"}>
          {sectionKey && (
            <h3 className="mb-2 text-sm font-semibold text-[var(--color-text-secondary)]">
              القسم: {sectionKey}
            </h3>
          )}
          <div className="space-y-3">
            {sectionFields.map(({ field, globalIndex }) => (
              <FieldEditor
                key={globalIndex}
                field={field}
                index={globalIndex}
                total={fields.length}
                onChange={(updated) => updateField(globalIndex, updated)}
                onRemove={() => removeField(globalIndex)}
                onMoveUp={() => moveUp(globalIndex)}
                onMoveDown={() => moveDown(globalIndex)}
                disabled={disabled}
              />
            ))}
          </div>
        </div>
      ))}

      {fields.length === 0 && (
        <p className="py-8 text-center text-sm text-[var(--color-text-tertiary)]">
          لم تتم إضافة حقول بعد. ابدأ بإضافة حقل جديد أدناه.
        </p>
      )}

      {!disabled && (
        <>
          <div className="flex items-end gap-2 rounded-[10px] border border-dashed border-[var(--color-border-default)] p-3">
            <div className="flex-1">
              <label className="text-xs text-[var(--color-text-tertiary)]">نوع الحقل الجديد</label>
              <select
                value={addType}
                onChange={(e) => setAddType(Number(e.target.value) as FormFieldType)}
                className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm"
              >
                {Object.entries(fieldTypeLabel).map(([val, lbl]) => (
                  <option key={val} value={val}>{lbl}</option>
                ))}
              </select>
            </div>
            <Button type="button" variant="secondary" onClick={addField}>
              إضافة حقل
            </Button>
          </div>

          <div className="flex items-center justify-end gap-2 pt-2">
            <Button onClick={handleSave} disabled={saving}>
              {saving ? "جاري الحفظ…" : "حفظ تعريف النموذج"}
            </Button>
          </div>
        </>
      )}
    </div>
  );
}

interface SectionGroup {
  sectionKey: string | null;
  sectionFields: { field: FormFieldInput; globalIndex: number }[];
}

function groupBySections(fields: FormFieldInput[]): SectionGroup[] {
  const groups: SectionGroup[] = [];
  const seen = new Set<string | null>();

  fields.forEach((field, index) => {
    const key = field.sectionKey ?? null;
    if (!seen.has(key)) {
      seen.add(key);
      groups.push({ sectionKey: key, sectionFields: [] });
    }
    const group = groups.find((g) => g.sectionKey === key)!;
    group.sectionFields.push({ field, globalIndex: index });
  });

  return groups;
}
