"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import type { ApiError } from "@/lib/api/client";
import { collectFieldErrors } from "@/lib/acknowledgments/labels";
import {
  configurableRecurrenceModels,
  recurrenceModelDescription,
  recurrenceModelLabel,
} from "@/lib/acknowledgments/recurrenceLabels";
import {
  RecurrenceModel,
  type SetRecurrenceInput,
} from "@/lib/acknowledgments/types";

const dateRegex = /^\d{4}-\d{2}-\d{2}$/;

const schema = z
  .object({
    recurrenceModel: z.coerce.number().int().min(1).max(5),
    startDate: z
      .string()
      .optional()
      .refine((v) => !v || dateRegex.test(v), "صيغة التاريخ غير صحيحة (YYYY-MM-DD)")
      .or(z.literal("")),
    dueDate: z
      .string()
      .optional()
      .refine((v) => !v || dateRegex.test(v), "صيغة التاريخ غير صحيحة (YYYY-MM-DD)")
      .or(z.literal("")),
  })
  .superRefine((data, ctx) => {
    if (data.startDate && data.dueDate && data.startDate > data.dueDate) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ["dueDate"],
        message: "تاريخ الاستحقاق يجب أن يكون بعد تاريخ البدء",
      });
    }
  });

type FormValues = z.infer<typeof schema>;

interface Props {
  initialValue: {
    recurrenceModel: RecurrenceModel;
    startDate?: string | null;
    dueDate?: string | null;
  };
  onSubmit: (input: SetRecurrenceInput) => Promise<unknown>;
  onCancel?: () => void;
  disabled?: boolean;
}

/**
 * Dedicated recurrence-configuration form bound to the
 * PUT /versions/:id/recurrence endpoint (BR-046). Lets authors pick any of
 * the five supported cadence models and optionally narrow the start/due
 * window. Unspecified is deliberately not selectable — authors must commit
 * to a model before publish.
 */
export function RecurrenceForm({ initialValue, onSubmit, onCancel, disabled }: Props) {
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const defaultModel =
    initialValue.recurrenceModel === RecurrenceModel.Unspecified
      ? RecurrenceModel.Annual
      : initialValue.recurrenceModel;

  const {
    register,
    handleSubmit,
    watch,
    formState: { isSubmitting },
  } = useForm<FormValues>({
    defaultValues: {
      recurrenceModel: defaultModel,
      startDate: initialValue.startDate ?? "",
      dueDate: initialValue.dueDate ?? "",
    },
  });

  const selectedModel = Number(watch("recurrenceModel")) as RecurrenceModel;

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
        recurrenceModel: parsed.data.recurrenceModel as RecurrenceModel,
        startDate: parsed.data.startDate || null,
        dueDate: parsed.data.dueDate || null,
      });
    } catch (error) {
      const apiError = error as ApiError;
      setFieldErrors(collectFieldErrors(apiError?.errors));
      setSubmissionError(formatError(apiError));
    }
  });

  return (
    <form onSubmit={submit} className="space-y-5" noValidate>
      <div className="space-y-1.5">
        <label htmlFor="recurrenceModel" className="block text-sm font-medium">
          نموذج التكرار
        </label>
        <select
          id="recurrenceModel"
          disabled={disabled}
          className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)] disabled:bg-[var(--color-surface-subtle)]"
          {...register("recurrenceModel")}
        >
          {configurableRecurrenceModels.map((model) => (
            <option key={model} value={model}>
              {recurrenceModelLabel(model)}
            </option>
          ))}
        </select>
        <p className="text-xs text-[var(--color-text-tertiary)]">
          {recurrenceModelDescription(selectedModel)}
        </p>
        {fieldErrors.recurrenceModel ? (
          <p role="alert" className="text-xs text-red-700">
            {fieldErrors.recurrenceModel}
          </p>
        ) : null}
      </div>

      <div className="grid gap-5 md:grid-cols-2">
        <div className="space-y-1.5">
          <label htmlFor="startDate" className="block text-sm font-medium">
            تاريخ البدء (اختياري)
          </label>
          <input
            id="startDate"
            type="date"
            dir="ltr"
            disabled={disabled}
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)] disabled:bg-[var(--color-surface-subtle)]"
            {...register("startDate")}
          />
          {fieldErrors.startDate ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.startDate}
            </p>
          ) : null}
        </div>

        <div className="space-y-1.5">
          <label htmlFor="dueDate" className="block text-sm font-medium">
            تاريخ الاستحقاق (اختياري)
          </label>
          <input
            id="dueDate"
            type="date"
            dir="ltr"
            disabled={disabled}
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)] disabled:bg-[var(--color-surface-subtle)]"
            {...register("dueDate")}
          />
          {fieldErrors.dueDate ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.dueDate}
            </p>
          ) : null}
        </div>
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
        <Button type="submit" disabled={disabled || isSubmitting}>
          {isSubmitting ? "جاري الحفظ…" : "حفظ نموذج التكرار"}
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
