"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import type { ApiError } from "@/lib/api/client";
import { collectFieldErrors } from "@/lib/policies/labels";
import type {
  CreatePolicyVersionInput,
  PolicyVersionDetail,
  UpdatePolicyVersionDraftInput,
} from "@/lib/policies/types";

const schema = z.object({
  versionLabel: z.string().trim().max(128).optional().or(z.literal("")),
  effectiveDate: z
    .string()
    .optional()
    .refine((v) => !v || /^\d{4}-\d{2}-\d{2}$/.test(v), "صيغة التاريخ غير صحيحة (YYYY-MM-DD)")
    .or(z.literal("")),
  summary: z.string().trim().max(4000).optional().or(z.literal("")),
});

type FormValues = z.infer<typeof schema>;

interface VersionFormProps {
  mode: "create" | "edit";
  initialValue?: PolicyVersionDetail;
  onSubmit: (input: CreatePolicyVersionInput | UpdatePolicyVersionDraftInput) => Promise<unknown>;
  submitLabel?: string;
  submittingLabel?: string;
  onCancel?: () => void;
}

/** Shared form for creating a draft version and editing a draft version. */
export function VersionForm({
  initialValue,
  onSubmit,
  submitLabel,
  submittingLabel,
  onCancel,
}: VersionFormProps) {
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const {
    register,
    handleSubmit,
    formState: { isSubmitting },
  } = useForm<FormValues>({
    defaultValues: {
      versionLabel: initialValue?.versionLabel ?? "",
      effectiveDate: initialValue?.effectiveDate ?? "",
      summary: initialValue?.summary ?? "",
    },
  });

  const submit = handleSubmit(async (values) => {
    setSubmissionError(null);
    setFieldErrors({});

    const parsed = schema.safeParse(values);
    if (!parsed.success) {
      const fe: Record<string, string> = {};
      for (const issue of parsed.error.issues) {
        const key = issue.path[0] as string | undefined;
        if (key && !fe[key]) fe[key] = issue.message;
      }
      setFieldErrors(fe);
      return;
    }

    try {
      await onSubmit({
        versionLabel: parsed.data.versionLabel?.trim() || null,
        effectiveDate: parsed.data.effectiveDate ? parsed.data.effectiveDate : null,
        summary: parsed.data.summary?.trim() || null,
      });
    } catch (error) {
      const apiError = error as ApiError;
      setFieldErrors(collectFieldErrors(apiError?.errors));
      setSubmissionError(formatError(apiError));
    }
  });

  return (
    <form onSubmit={submit} className="space-y-5" noValidate>
      <div className="grid gap-5 md:grid-cols-2">
        <div className="space-y-1.5">
          <label htmlFor="versionLabel" className="block text-sm font-medium">
            التسمية (اختياري)
          </label>
          <input
            id="versionLabel"
            type="text"
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            placeholder="مثلاً: مراجعة 2026-Q1"
            {...register("versionLabel")}
          />
          {fieldErrors.versionLabel ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.versionLabel}
            </p>
          ) : null}
        </div>

        <div className="space-y-1.5">
          <label htmlFor="effectiveDate" className="block text-sm font-medium">
            تاريخ النفاذ (اختياري)
          </label>
          <input
            id="effectiveDate"
            type="date"
            dir="ltr"
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            {...register("effectiveDate")}
          />
          {fieldErrors.effectiveDate ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.effectiveDate}
            </p>
          ) : null}
        </div>
      </div>

      <div className="space-y-1.5">
        <label htmlFor="summary" className="block text-sm font-medium">
          ملخص التغييرات (اختياري)
        </label>
        <textarea
          id="summary"
          rows={5}
          className="block w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 py-2 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
          {...register("summary")}
        />
        {fieldErrors.summary ? (
          <p role="alert" className="text-xs text-red-700">
            {fieldErrors.summary}
          </p>
        ) : null}
      </div>

      {submissionError ? (
        <div
          role="alert"
          className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800"
        >
          {submissionError}
        </div>
      ) : null}

      <div className="flex items-center gap-3">
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? submittingLabel ?? "جاري الحفظ…" : submitLabel ?? "حفظ"}
        </Button>
        {onCancel ? (
          <Button type="button" variant="ghost" onClick={onCancel}>
            إلغاء
          </Button>
        ) : null}
      </div>
    </form>
  );
}

function formatError(error: ApiError | undefined): string {
  if (!error) return "حدث خطأ غير متوقع.";
  if (error.errors) {
    return Object.values(error.errors).flat().join(" ") || error.title;
  }
  return error.detail ?? error.title ?? "تعذّر إتمام العملية.";
}
