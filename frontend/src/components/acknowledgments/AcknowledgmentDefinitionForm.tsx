"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
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
  type AcknowledgmentDefinitionDetail,
  type CreateAcknowledgmentDefinitionInput,
  type UpdateAcknowledgmentDefinitionInput,
} from "@/lib/acknowledgments/types";

const schema = z.object({
  title: z.string().trim().min(1, "العنوان مطلوب").max(256),
  ownerDepartment: z
    .string()
    .trim()
    .min(1, "الإدارة المالكة مطلوبة (BR-014)")
    .max(256),
  defaultActionType: z.coerce.number().int().min(0).max(2),
  description: z.string().trim().max(4000).optional().or(z.literal("")),
});

type FormValues = z.infer<typeof schema>;

interface Props {
  mode: "create" | "edit";
  initialValue?: AcknowledgmentDefinitionDetail;
  onSubmit: (
    input: CreateAcknowledgmentDefinitionInput | UpdateAcknowledgmentDefinitionInput,
  ) => Promise<unknown>;
  submitLabel?: string;
  submittingLabel?: string;
  onCancel?: () => void;
}

/**
 * Shared acknowledgment-definition form used by the "new" and "edit" admin
 * pages. The action type is persisted at the definition level as a default
 * value — each version may override it (e.g., switching from a simple ack
 * to an ack-with-commitment between revisions).
 */
export function AcknowledgmentDefinitionForm({
  initialValue,
  onSubmit,
  submitLabel,
  submittingLabel,
  onCancel,
}: Props) {
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const {
    register,
    handleSubmit,
    watch,
    formState: { isSubmitting },
  } = useForm<FormValues>({
    defaultValues: {
      title: initialValue?.title ?? "",
      ownerDepartment: initialValue?.ownerDepartment ?? "",
      defaultActionType:
        initialValue?.defaultActionType ?? ActionType.SimpleAcknowledgment,
      description: initialValue?.description ?? "",
    },
  });

  const selectedActionType = Number(watch("defaultActionType")) as ActionType;

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
        title: parsed.data.title,
        ownerDepartment: parsed.data.ownerDepartment,
        defaultActionType: parsed.data.defaultActionType as ActionType,
        description: parsed.data.description?.trim() || null,
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
          <label htmlFor="title" className="block text-sm font-medium">
            العنوان
          </label>
          <input
            id="title"
            type="text"
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            {...register("title")}
          />
          {fieldErrors.title ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.title}
            </p>
          ) : null}
        </div>

        <div className="space-y-1.5">
          <label htmlFor="ownerDepartment" className="block text-sm font-medium">
            الإدارة المالكة
          </label>
          <input
            id="ownerDepartment"
            type="text"
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            {...register("ownerDepartment")}
          />
          {fieldErrors.ownerDepartment ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.ownerDepartment}
            </p>
          ) : null}
        </div>

        <div className="space-y-1.5 md:col-span-2">
          <label htmlFor="defaultActionType" className="block text-sm font-medium">
            نوع الإجراء الافتراضي
          </label>
          <select
            id="defaultActionType"
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            {...register("defaultActionType")}
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
          {fieldErrors.defaultActionType ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.defaultActionType}
            </p>
          ) : null}
        </div>
      </div>

      <div className="space-y-1.5">
        <label htmlFor="description" className="block text-sm font-medium">
          الوصف (اختياري)
        </label>
        <textarea
          id="description"
          rows={4}
          className="block w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 py-2 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
          {...register("description")}
        />
        {fieldErrors.description ? (
          <p role="alert" className="text-xs text-red-700">
            {fieldErrors.description}
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
