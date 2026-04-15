"use client";

import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import type { ApiError } from "@/lib/api/client";
import {
  actionTypeDescription,
  actionTypeLabel,
  collectFieldErrors,
} from "@/lib/acknowledgments/labels";
import {
  ActionType,
  type AcknowledgmentVersionDetail,
  type CreateAcknowledgmentVersionInput,
  type UpdateAcknowledgmentVersionDraftInput,
} from "@/lib/acknowledgments/types";
import { PolicyVersionPicker } from "./PolicyVersionPicker";

const dateRegex = /^\d{4}-\d{2}-\d{2}$/;

const schema = z
  .object({
    policyVersionId: z.string().uuid("يجب اختيار نسخة سياسة منشورة"),
    actionType: z.coerce.number().int().min(0).max(2),
    versionLabel: z.string().trim().max(128).optional().or(z.literal("")),
    summary: z.string().trim().max(4000).optional().or(z.literal("")),
    commitmentText: z.string().trim().max(4000).optional().or(z.literal("")),
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
    const action = Number(data.actionType) as ActionType;
    if (action === ActionType.AcknowledgmentWithCommitment) {
      if (!data.commitmentText || !data.commitmentText.trim()) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          path: ["commitmentText"],
          message: "نص التعهّد مطلوب مع نوع \"إقرار مع تعهّد\"",
        });
      }
    }
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
  mode: "create" | "edit";
  initialValue?: AcknowledgmentVersionDetail;
  defaultActionType: ActionType;
  onSubmit: (
    input: CreateAcknowledgmentVersionInput | UpdateAcknowledgmentVersionDraftInput,
  ) => Promise<unknown>;
  submitLabel?: string;
  submittingLabel?: string;
  onCancel?: () => void;
}

/**
 * Shared acknowledgment-version draft form. Used on both the "create" and
 * "edit draft" screens — hides nothing so authors can tweak action type,
 * link to a different policy version, etc. until publish.
 */
export function AcknowledgmentVersionForm({
  initialValue,
  defaultActionType,
  onSubmit,
  submitLabel,
  submittingLabel,
  onCancel,
}: Props) {
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const {
    register,
    control,
    handleSubmit,
    watch,
    formState: { isSubmitting },
  } = useForm<FormValues>({
    defaultValues: {
      policyVersionId: initialValue?.policyVersionId ?? "",
      actionType: initialValue?.actionType ?? defaultActionType,
      versionLabel: initialValue?.versionLabel ?? "",
      summary: initialValue?.summary ?? "",
      commitmentText: initialValue?.commitmentText ?? "",
      startDate: initialValue?.startDate ?? "",
      dueDate: initialValue?.dueDate ?? "",
    },
  });

  const selectedActionType = Number(watch("actionType")) as ActionType;

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
        policyVersionId: parsed.data.policyVersionId,
        actionType: parsed.data.actionType as ActionType,
        versionLabel: parsed.data.versionLabel?.trim() || null,
        summary: parsed.data.summary?.trim() || null,
        commitmentText:
          (parsed.data.actionType as ActionType) ===
          ActionType.AcknowledgmentWithCommitment
            ? parsed.data.commitmentText?.trim() || null
            : null,
        startDate: parsed.data.startDate || null,
        dueDate: parsed.data.dueDate || null,
      });
    } catch (error) {
      const apiError = error as ApiError;
      setFieldErrors(collectFieldErrors(apiError?.errors));
      setSubmissionError(formatError(apiError));
    }
  });

  const currentPolicyVersion = initialValue?.policyVersionId;

  return (
    <form onSubmit={submit} className="space-y-5" noValidate>
      <div className="space-y-1.5">
        <label className="block text-sm font-medium">النسخة المرتبطة من السياسة</label>
        <Controller
          control={control}
          name="policyVersionId"
          render={({ field }) => (
            <PolicyVersionPicker
              id="policyVersionId"
              value={field.value || null}
              onChange={(v) => field.onChange(v ?? "")}
            />
          )}
        />
        {currentPolicyVersion && !watch("policyVersionId") ? (
          <p className="text-xs text-[var(--color-text-tertiary)]">
            النسخة الحالية: <span dir="ltr">{currentPolicyVersion}</span> — اختر
            السياسة مرة أخرى لتغيير الربط.
          </p>
        ) : null}
        {fieldErrors.policyVersionId ? (
          <p role="alert" className="text-xs text-red-700">
            {fieldErrors.policyVersionId}
          </p>
        ) : null}
      </div>

      <div className="grid gap-5 md:grid-cols-2">
        <div className="space-y-1.5">
          <label htmlFor="actionType" className="block text-sm font-medium">
            نوع الإجراء
          </label>
          <select
            id="actionType"
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            {...register("actionType")}
          >
            <option value={ActionType.SimpleAcknowledgment}>
              {actionTypeLabel[ActionType.SimpleAcknowledgment]}
            </option>
            <option value={ActionType.AcknowledgmentWithCommitment}>
              {actionTypeLabel[ActionType.AcknowledgmentWithCommitment]}
            </option>
            <option value={ActionType.FormBasedDisclosure}>
              {actionTypeLabel[ActionType.FormBasedDisclosure]}
            </option>
          </select>
          <p className="text-xs text-[var(--color-text-tertiary)]">
            {actionTypeDescription[selectedActionType]}
          </p>
          {fieldErrors.actionType ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.actionType}
            </p>
          ) : null}
        </div>

        <div className="space-y-1.5">
          <label htmlFor="versionLabel" className="block text-sm font-medium">
            التسمية (اختياري)
          </label>
          <input
            id="versionLabel"
            type="text"
            placeholder="مثلاً: مراجعة 2026-Q2"
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            {...register("versionLabel")}
          />
          {fieldErrors.versionLabel ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.versionLabel}
            </p>
          ) : null}
        </div>

        <div className="space-y-1.5">
          <label htmlFor="startDate" className="block text-sm font-medium">
            تاريخ البدء (اختياري)
          </label>
          <input
            id="startDate"
            type="date"
            dir="ltr"
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
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
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            {...register("dueDate")}
          />
          {fieldErrors.dueDate ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.dueDate}
            </p>
          ) : null}
        </div>
      </div>

      <div className="space-y-1.5">
        <label htmlFor="summary" className="block text-sm font-medium">
          ملخص النسخة (اختياري)
        </label>
        <textarea
          id="summary"
          rows={3}
          className="block w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 py-2 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
          {...register("summary")}
        />
        {fieldErrors.summary ? (
          <p role="alert" className="text-xs text-red-700">
            {fieldErrors.summary}
          </p>
        ) : null}
      </div>

      {selectedActionType === ActionType.AcknowledgmentWithCommitment ? (
        <div className="space-y-1.5">
          <label htmlFor="commitmentText" className="block text-sm font-medium">
            نص التعهّد
          </label>
          <textarea
            id="commitmentText"
            rows={5}
            className="block w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 py-2 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            placeholder="أتعهد بالالتزام بأحكام هذه السياسة…"
            {...register("commitmentText")}
          />
          <p className="text-xs text-[var(--color-text-tertiary)]">
            سيظهر هذا النص للموظف عند التوقيع ويُحفظ كجزء من سجل الإقرار.
          </p>
          {fieldErrors.commitmentText ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.commitmentText}
            </p>
          ) : null}
        </div>
      ) : null}

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
