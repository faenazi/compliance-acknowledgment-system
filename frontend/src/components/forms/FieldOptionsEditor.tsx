"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import type { FieldOption } from "@/lib/forms/types";

interface Props {
  options: FieldOption[];
  onChange: (options: FieldOption[]) => void;
  disabled?: boolean;
}

/**
 * Inline editor for field options (radio/dropdown/multi-select).
 * Allows add, edit, remove, and reorder (BR-075).
 */
export function FieldOptionsEditor({ options, onChange, disabled }: Props) {
  const [draft, setDraft] = useState({ value: "", label: "" });

  const addOption = () => {
    if (!draft.value.trim() || !draft.label.trim()) return;
    onChange([...options, { value: draft.value.trim(), label: draft.label.trim() }]);
    setDraft({ value: "", label: "" });
  };

  const removeOption = (index: number) => {
    onChange(options.filter((_, i) => i !== index));
  };

  const moveUp = (index: number) => {
    if (index === 0) return;
    const next = [...options];
    [next[index - 1], next[index]] = [next[index], next[index - 1]];
    onChange(next);
  };

  const moveDown = (index: number) => {
    if (index >= options.length - 1) return;
    const next = [...options];
    [next[index], next[index + 1]] = [next[index + 1], next[index]];
    onChange(next);
  };

  return (
    <div className="space-y-2">
      <p className="text-xs font-medium text-[var(--color-text-secondary)]">الخيارات</p>

      {options.length > 0 && (
        <ul className="divide-y divide-[var(--color-border-default)] rounded-[10px] border border-[var(--color-border-default)] bg-white text-sm">
          {options.map((opt, i) => (
            <li key={i} className="flex items-center gap-2 px-3 py-1.5">
              <span className="flex-1" dir="auto">{opt.label}</span>
              <span className="text-xs text-[var(--color-text-tertiary)]" dir="ltr">{opt.value}</span>
              {!disabled && (
                <>
                  <button type="button" onClick={() => moveUp(i)} className="text-xs hover:underline" disabled={i === 0}>▲</button>
                  <button type="button" onClick={() => moveDown(i)} className="text-xs hover:underline" disabled={i === options.length - 1}>▼</button>
                  <button type="button" onClick={() => removeOption(i)} className="text-xs text-red-600 hover:underline">حذف</button>
                </>
              )}
            </li>
          ))}
        </ul>
      )}

      {!disabled && (
        <div className="flex items-end gap-2">
          <div className="flex-1">
            <label className="text-xs text-[var(--color-text-tertiary)]">القيمة</label>
            <input
              type="text"
              value={draft.value}
              onChange={(e) => setDraft((d) => ({ ...d, value: e.target.value }))}
              className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm"
              dir="ltr"
            />
          </div>
          <div className="flex-1">
            <label className="text-xs text-[var(--color-text-tertiary)]">التسمية</label>
            <input
              type="text"
              value={draft.label}
              onChange={(e) => setDraft((d) => ({ ...d, label: e.target.value }))}
              className="block w-full rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 py-1.5 text-sm"
            />
          </div>
          <Button type="button" variant="secondary" onClick={addOption}>
            إضافة
          </Button>
        </div>
      )}
    </div>
  );
}
