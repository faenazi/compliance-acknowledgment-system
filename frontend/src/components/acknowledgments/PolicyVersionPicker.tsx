"use client";

import { useMemo, useState } from "react";
import { usePolicies, usePolicyVersions } from "@/lib/policies/hooks";
import { PolicyStatus, PolicyVersionStatus } from "@/lib/policies/types";

interface Props {
  value: string | null;
  onChange: (policyVersionId: string | null) => void;
  disabled?: boolean;
  id?: string;
}

/**
 * Two-step picker that lets the author choose (1) a master policy and
 * (2) one of its published versions. Per BR-019 an acknowledgment must
 * link to exactly one policy version, and only Published versions are
 * offered so authors cannot pin to drafts or archived snapshots.
 */
export function PolicyVersionPicker({ value, onChange, disabled, id }: Props) {
  const [selectedPolicyId, setSelectedPolicyId] = useState<string>("");

  const { data: policiesPage, isLoading: policiesLoading } = usePolicies({
    page: 1,
    pageSize: 100,
    status: PolicyStatus.Published,
  });

  const { data: versions, isLoading: versionsLoading } =
    usePolicyVersions(selectedPolicyId || undefined);

  const publishedVersions = useMemo(
    () =>
      (versions ?? [])
        .filter((v) => v.status === PolicyVersionStatus.Published)
        .sort((a, b) => b.versionNumber - a.versionNumber),
    [versions],
  );

  // If an existing value is provided and we haven't selected a policy yet, leave
  // the version dropdown empty — the parent is expected to display the current
  // selection as read-only text alongside the picker.
  return (
    <div className="grid gap-3 md:grid-cols-2">
      <div className="space-y-1.5">
        <label className="block text-xs font-medium text-[var(--color-text-tertiary)]">
          السياسة
        </label>
        <select
          disabled={disabled || policiesLoading}
          value={selectedPolicyId}
          onChange={(e) => {
            setSelectedPolicyId(e.target.value);
            onChange(null);
          }}
          className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)] disabled:bg-[var(--color-surface-subtle)]"
        >
          <option value="">
            {policiesLoading ? "جاري التحميل…" : "— اختر سياسة —"}
          </option>
          {(policiesPage?.items ?? []).map((p) => (
            <option key={p.id} value={p.id}>
              {p.title} ({p.policyCode})
            </option>
          ))}
        </select>
      </div>

      <div className="space-y-1.5">
        <label
          htmlFor={id}
          className="block text-xs font-medium text-[var(--color-text-tertiary)]"
        >
          نسخة السياسة (منشورة فقط)
        </label>
        <select
          id={id}
          disabled={disabled || !selectedPolicyId || versionsLoading}
          value={value ?? ""}
          onChange={(e) => onChange(e.target.value || null)}
          className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)] disabled:bg-[var(--color-surface-subtle)]"
        >
          <option value="">
            {!selectedPolicyId
              ? "اختر السياسة أولاً"
              : versionsLoading
                ? "جاري التحميل…"
                : publishedVersions.length === 0
                  ? "لا توجد نسخ منشورة"
                  : "— اختر النسخة —"}
          </option>
          {publishedVersions.map((v) => (
            <option key={v.id} value={v.id}>
              v{v.versionNumber}
              {v.versionLabel ? ` — ${v.versionLabel}` : ""}
              {v.effectiveDate ? ` · ${v.effectiveDate}` : ""}
            </option>
          ))}
        </select>
      </div>
    </div>
  );
}
