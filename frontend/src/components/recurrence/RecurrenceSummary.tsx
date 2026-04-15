"use client";

import { Badge } from "@/components/ui/badge";
import {
  recurrenceModelDescription,
  recurrenceModelLabel,
} from "@/lib/acknowledgments/recurrenceLabels";
import { RecurrenceModel } from "@/lib/acknowledgments/types";

interface Props {
  recurrenceModel: RecurrenceModel;
  startDate?: string | null;
  dueDate?: string | null;
}

/**
 * Read-only summary of the active recurrence configuration for a version
 * (BR-040..BR-046). Shown on the version detail page and on the audience page
 * so authors never lose sight of cadence while editing targeting rules.
 */
export function RecurrenceSummary({ recurrenceModel, startDate, dueDate }: Props) {
  const unspecified = recurrenceModel === RecurrenceModel.Unspecified;
  return (
    <div className="space-y-2">
      <div className="flex items-center gap-2">
        <Badge status={unspecified ? "draft" : "published"}>
          {recurrenceModelLabel(recurrenceModel)}
        </Badge>
        {unspecified ? (
          <span className="text-xs text-[var(--color-text-tertiary)]">
            يجب اختيار النموذج قبل النشر (BR-033).
          </span>
        ) : null}
      </div>
      <p className="text-sm text-[var(--color-text-secondary)]">
        {recurrenceModelDescription(recurrenceModel)}
      </p>
      <dl className="grid gap-2 text-xs text-[var(--color-text-tertiary)] md:grid-cols-2">
        <div>
          <dt>تاريخ البدء</dt>
          <dd dir="ltr" className="text-[var(--color-text-primary)]">
            {startDate ?? "—"}
          </dd>
        </div>
        <div>
          <dt>تاريخ الاستحقاق</dt>
          <dd dir="ltr" className="text-[var(--color-text-primary)]">
            {dueDate ?? "—"}
          </dd>
        </div>
      </dl>
    </div>
  );
}
