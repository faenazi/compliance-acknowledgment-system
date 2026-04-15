"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { audienceRuleTypeLabel } from "@/lib/audience/labels";
import {
  AudienceRuleType,
  type AudienceRule,
  type AudienceRuleInput,
} from "@/lib/audience/types";

interface Props {
  /** Current exclusion rules loaded from the server. */
  initialRules: AudienceRule[];
  onSave: (rules: AudienceRuleInput[]) => Promise<unknown>;
  disabled?: boolean;
}

type DraftRule = {
  key: string;
  ruleType: AudienceRuleType;
  ruleValue: string;
};

let sequence = 0;
function newKey(): string {
  sequence += 1;
  return `excl-${sequence}`;
}

/**
 * Exclusions editor (BR-055). Supports Department, AD Group, and specific
 * User exclusions. "All users" is intentionally absent — the backend rejects
 * it because excluding everyone would defeat the purpose of the audience.
 * Exclusions always override inclusions during audience resolution.
 */
export function AudienceExclusionsEditor({ initialRules, onSave, disabled }: Props) {
  const [drafts, setDrafts] = useState<DraftRule[]>(() =>
    initialRules.map((r) => ({
      key: newKey(),
      ruleType: r.ruleType,
      ruleValue: r.ruleValue ?? "",
    })),
  );
  const [error, setError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const addRule = (ruleType: AudienceRuleType) => {
    setDrafts((rows) => [...rows, { key: newKey(), ruleType, ruleValue: "" }]);
  };

  const updateType = (key: string, ruleType: AudienceRuleType) => {
    setDrafts((rows) =>
      rows.map((r) => (r.key === key ? { ...r, ruleType } : r)),
    );
  };

  const updateValue = (key: string, value: string) => {
    setDrafts((rows) =>
      rows.map((r) => (r.key === key ? { ...r, ruleValue: value } : r)),
    );
  };

  const removeRule = (key: string) => {
    setDrafts((rows) => rows.filter((r) => r.key !== key));
  };

  const save = async () => {
    setError(null);
    const normalized = drafts.map((r) => ({ ...r, ruleValue: r.ruleValue.trim() }));
    const blank = normalized.find((r) => !r.ruleValue);
    if (blank) {
      setError("لا يمكن ترك قيمة الاستثناء فارغة.");
      return;
    }
    try {
      setIsSaving(true);
      await onSave(
        normalized.map<AudienceRuleInput>((r) => ({
          ruleType: r.ruleType,
          ruleValue: r.ruleValue,
        })),
      );
    } catch (e) {
      setError(formatError(e));
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="space-y-4">
      <p className="text-xs text-[var(--color-text-tertiary)]">
        الاستثناءات تُقدَّم دائماً على قواعد الإدراج — يُستَبعَد أي موظف يطابق
        استثناءً حتى لو طابق قاعدة إدراج.
      </p>

      <ul className="space-y-2">
        {drafts.map((rule) => (
          <li
            key={rule.key}
            className="flex flex-wrap items-center gap-2 rounded-[10px] border border-[var(--color-border-default)] bg-white p-3"
          >
            <select
              aria-label="نوع الاستثناء"
              className="h-10 rounded-[8px] border border-[var(--color-border-strong)] bg-white px-2 text-sm"
              value={rule.ruleType}
              disabled={disabled}
              onChange={(e) =>
                updateType(rule.key, Number(e.target.value) as AudienceRuleType)
              }
            >
              <option value={AudienceRuleType.Department}>
                {audienceRuleTypeLabel(AudienceRuleType.Department)}
              </option>
              <option value={AudienceRuleType.AdGroup}>
                {audienceRuleTypeLabel(AudienceRuleType.AdGroup)}
              </option>
              <option value={AudienceRuleType.User}>
                {audienceRuleTypeLabel(AudienceRuleType.User)}
              </option>
            </select>
            <input
              aria-label="قيمة الاستثناء"
              type="text"
              dir={rule.ruleType === AudienceRuleType.User ? "ltr" : "rtl"}
              value={rule.ruleValue}
              disabled={disabled}
              onChange={(e) => updateValue(rule.key, e.target.value)}
              className="h-10 flex-1 min-w-[200px] rounded-[8px] border border-[var(--color-border-strong)] bg-white px-3 text-sm"
            />
            <Button
              type="button"
              variant="ghost"
              size="sm"
              onClick={() => removeRule(rule.key)}
              disabled={disabled}
            >
              إزالة
            </Button>
          </li>
        ))}
        {drafts.length === 0 ? (
          <li className="rounded-[10px] border border-dashed border-[var(--color-border-default)] bg-[var(--color-surface-subtle)] p-4 text-sm text-[var(--color-text-tertiary)]">
            لا توجد استثناءات. احفظ القائمة فارغة لإزالة الاستثناءات السابقة.
          </li>
        ) : null}
      </ul>

      <div className="flex flex-wrap gap-2">
        <Button
          type="button"
          variant="secondary"
          size="sm"
          onClick={() => addRule(AudienceRuleType.Department)}
          disabled={disabled}
        >
          + استثناء قسم
        </Button>
        <Button
          type="button"
          variant="secondary"
          size="sm"
          onClick={() => addRule(AudienceRuleType.AdGroup)}
          disabled={disabled}
        >
          + استثناء مجموعة AD
        </Button>
        <Button
          type="button"
          variant="secondary"
          size="sm"
          onClick={() => addRule(AudienceRuleType.User)}
          disabled={disabled}
        >
          + استثناء مستخدم
        </Button>
      </div>

      {error ? (
        <div
          role="alert"
          className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800"
        >
          {error}
        </div>
      ) : null}

      <div>
        <Button type="button" onClick={save} disabled={disabled || isSaving}>
          {isSaving ? "جاري الحفظ…" : "حفظ الاستثناءات"}
        </Button>
      </div>
    </div>
  );
}

function formatError(e: unknown): string {
  const err = e as { detail?: string; title?: string; errors?: Record<string, string[]> };
  if (err?.errors) {
    const flat = Object.values(err.errors).flat();
    if (flat.length) return flat.join(" ");
  }
  return err?.detail ?? err?.title ?? "تعذّر حفظ الاستثناءات.";
}
