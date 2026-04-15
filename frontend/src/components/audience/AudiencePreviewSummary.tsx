"use client";

import { Button } from "@/components/ui/button";
import type { AudiencePreview } from "@/lib/audience/types";

interface Props {
  preview: AudiencePreview | undefined;
  isLoading: boolean;
  isError: boolean;
  errorMessage?: string | null;
  onRefresh: () => void;
}

/**
 * Renders the audience preview: counts + a sample of matched users. Phase 1
 * calls this an *estimate* because AD-group membership is resolved by a
 * stubbed directory resolver — the count will become authoritative when the
 * AD sync job lands in a later sprint.
 */
export function AudiencePreviewSummary({
  preview,
  isLoading,
  isError,
  errorMessage,
  onRefresh,
}: Props) {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between gap-2">
        <h3 className="text-sm font-semibold text-[var(--color-text-primary)]">
          معاينة الجمهور المستهدف (تقديرية)
        </h3>
        <Button type="button" variant="ghost" size="sm" onClick={onRefresh}>
          تحديث
        </Button>
      </div>

      {isLoading ? (
        <p className="text-sm text-[var(--color-text-tertiary)]">جاري الحساب…</p>
      ) : isError ? (
        <div
          role="alert"
          className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800"
        >
          تعذّر حساب المعاينة{errorMessage ? `: ${errorMessage}` : "."}
        </div>
      ) : preview ? (
        <>
          <dl className="grid gap-3 md:grid-cols-3">
            <Stat label="العدد المُقدَّر" value={preview.estimatedUserCount} emphasize />
            <Stat
              label="مطابقات الإدراج"
              value={preview.inclusionMatchedCount}
            />
            <Stat
              label="مطابقات الاستثناء"
              value={preview.exclusionMatchedCount}
            />
          </dl>

          <div className="rounded-[10px] border border-[var(--color-border-default)] bg-white">
            <div className="border-b border-[var(--color-border-default)] px-4 py-2 text-xs font-semibold text-[var(--color-text-secondary)]">
              عينة من المستخدمين (حتى 10)
            </div>
            {preview.sampleUsers.length === 0 ? (
              <p className="p-4 text-sm text-[var(--color-text-tertiary)]">
                لا توجد عينة لعرضها.
              </p>
            ) : (
              <ul className="divide-y divide-[var(--color-border-default)]">
                {preview.sampleUsers.map((u) => (
                  <li key={u.userId} className="flex items-center justify-between px-4 py-2 text-sm">
                    <div>
                      <div>{u.displayName}</div>
                      <div className="text-xs text-[var(--color-text-tertiary)]" dir="ltr">
                        {u.username}
                      </div>
                    </div>
                    <span className="text-xs text-[var(--color-text-tertiary)]">
                      {u.department ?? "—"}
                    </span>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </>
      ) : (
        <p className="text-sm text-[var(--color-text-tertiary)]">لا توجد بيانات.</p>
      )}
    </div>
  );
}

function Stat({
  label,
  value,
  emphasize,
}: {
  label: string;
  value: number;
  emphasize?: boolean;
}) {
  return (
    <div
      className={
        emphasize
          ? "rounded-[10px] border border-[var(--color-brand-primary)] bg-[rgba(44,58,130,0.04)] p-4"
          : "rounded-[10px] border border-[var(--color-border-default)] bg-white p-4"
      }
    >
      <dt className="text-xs text-[var(--color-text-tertiary)]">{label}</dt>
      <dd className="mt-1 text-2xl font-semibold text-[var(--color-text-primary)]">
        {value.toLocaleString("ar-EG")}
      </dd>
    </div>
  );
}
