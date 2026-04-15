"use client";

import { useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import { audienceRuleTypeLabel } from "@/lib/audience/labels";
import {
  AudienceRuleType,
  type AudienceRule,
  type AudienceRuleInput,
} from "@/lib/audience/types";

interface Props {
  /** Current inclusion rules loaded from the server (null if not configured yet). */
  initialRules: AudienceRule[] | null;
  onSave: (rules: AudienceRuleInput[]) => Promise<unknown>;
  onSetAllUsers: () => Promise<unknown>;
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
  return `rule-${sequence}`;
}

/**
 * Inclusion-rules editor for audience targeting (BR-050..BR-054). Supports
 * three rule kinds: Department, AD Group, Specific User. The special
 * "All users" audience is managed via a dedicated button that calls the
 * set-all-users endpoint — when active, the per-rule editor is hidden to
 * avoid ambiguity.
 */
export function AudienceRulesEditor({
  initialRules,
  onSave,
  onSetAllUsers,
  disabled,
}: Props) {
  const isAllUsers = useMemo(
    () =>
      (initialRules ?? []).length === 1 &&
      initialRules![0].ruleType === AudienceRuleType.AllUsers,
    [initialRules],
  );

  const [drafts, setDrafts] = useState<DraftRule[]>(() =>
    (initialRules ?? [])
      .filter((r) => r.ruleType !== AudienceRuleType.AllUsers)
      .map((r) => ({
        key: newKey(),
        ruleType: r.ruleType,
        ruleValue: r.ruleValue ?? "",
      })),
  );
  const [error, setError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [isMakingAllUsers, setIsMakingAllUsers] = useState(false);

  const addRule = (ruleType: AudienceRuleType) => {
    setDrafts((rows) => [...rows, { key: newKey(), ruleType, ruleValue: "" }]);
  };

  const updateValue = (key: string, value: string) => {
    setDrafts((rows) =>
      rows.map((r) => (r.key === key ? { ...r, ruleValue: value } : r)),
    );
  };

  const updateType = (key: string, ruleType: AudienceRuleType) => {
    setDrafts((rows) =>
      rows.map((r) => (r.key === key ? { ...r, ruleType } : r)),
    );
  };

  const removeRule = (key: string) => {
    setDrafts((rows) => rows.filter((r) => r.key !== key));
  };

  const save = async () => {
    setError(null);
    const normalized = drafts.map((r) => ({ ...r, ruleValue: r.ruleValue.trim() }));
    if (normalized.length === 0) {
      setError("يجب إضافة قاعدة واحدة على الأقل أو استخدام \"كل المستخدمين\".");
      return;
    }
    const blank = normalized.find((r) => !r.ruleValue);
    if (blank) {
      setError("لا يمكن ترك قيمة القاعدة فارغة.");
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

  const setAllUsers = async () => {
    setError(null);
    try {
      setIsMakingAllUsers(true);
      await onSetAllUsers();
      setDrafts([]);
    } catch (e) {
      setError(formatError(e));
    } finally {
      setIsMakingAllUsers(false);
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center gap-2">
        <Button
          type="button"
          variant={isAllUsers ? "primary" : "secondary"}
          size="sm"
          onClick={setAllUsers}
          disabled={disabled || isMakingAllUsers}
        >
          {isMakingAllUsers
            ? "جاري التطبيق…"
            : isAllUsers
              ? "الاستهداف الحالي: كل المستخدمين"
              : "استهداف كل المستخدمين"}
        </Button>
        <span className="text-xs text-[var(--color-text-tertiary)]">
          أو أضف قواعد محددة أدناه. يتم توحيد نتائج القواعد (OR).
        </span>
      </div>

      {!isAllUsers ? (
        <>
          <ul className="space-y-2">
            {drafts.map((rule) => (
              <li
                key={rule.key}
                className="flex flex-wrap items-center gap-2 rounded-[10px] border border-[var(--color-border-default)] bg-white p-3"
              >
                <select
                  aria-label="نوع القاعدة"
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
                  aria-label="قيمة القاعدة"
                  type="text"
                  dir={rule.ruleType === AudienceRuleType.User ? "ltr" : "rtl"}
                  placeholder={placeholderFor(rule.ruleType)}
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
                لا توجد قواعد بعد. أضف قسم أو مجموعة أو مستخدم محدد.
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
              + قسم
            </Button>
            <Button
              type="button"
              variant="secondary"
              size="sm"
              onClick={() => addRule(AudienceRuleType.AdGroup)}
              disabled={disabled}
            >
              + مجموعة AD
            </Button>
            <Button
              type="button"
              variant="secondary"
              size="sm"
              onClick={() => addRule(AudienceRuleType.User)}
              disabled={disabled}
            >
              + مستخدم
            </Button>
          </div>
        </>
      ) : null}

      {error ? (
        <div
          role="alert"
          className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800"
        >
          {error}
        </div>
      ) : null}

      {!isAllUsers ? (
        <div>
          <Button type="button" onClick={save} disabled={disabled || isSaving}>
            {isSaving ? "جاري الحفظ…" : "حفظ قواعد الاستهداف"}
          </Button>
        </div>
      ) : null}
    </div>
  );
}

function placeholderFor(type: AudienceRuleType): string {
  switch (type) {
    case AudienceRuleType.Department:
      return "اسم القسم كما يرد من Active Directory";
    case AudienceRuleType.AdGroup:
      return "اسم المجموعة (sAMAccountName)";
    case AudienceRuleType.User:
      return "GUID المستخدم";
    default:
      return "";
  }
}

function formatError(e: unknown): string {
  const err = e as { detail?: string; title?: string; errors?: Record<string, string[]> };
  if (err?.errors) {
    const flat = Object.values(err.errors).flat();
    if (flat.length) return flat.join(" ");
  }
  return err?.detail ?? err?.title ?? "تعذّر حفظ القواعد.";
}
