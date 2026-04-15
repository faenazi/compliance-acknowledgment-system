"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import type { ApiError } from "@/lib/api/client";
import { collectFieldErrors } from "@/lib/policies/labels";
import type {
  CreatePolicyInput,
  PolicyDetail,
  UpdatePolicyInput,
} from "@/lib/policies/types";

const codePattern = /^[A-Za-z0-9._-]+$/;

const schema = z.object({
  policyCode: z
    .string()
    .trim()
    .min(1, "رمز السياسة مطلوب")
    .max(64)
    .regex(codePattern, "يُسمح فقط بالأحرف اللاتينية والأرقام و '.' '_' '-'."),
  title: z.string().trim().min(1, "العنوان مطلوب").max(256),
  ownerDepartment: z.string().trim().min(1, "الإدارة المالكة مطلوبة (BR-014)").max(256),
  category: z.string().trim().max(128).optional().or(z.literal("")),
  description: z.string().trim().max(4000).optional().or(z.literal("")),
});

type PolicyFormValues = z.infer<typeof schema>;

interface PolicyFormProps {
  mode: "create" | "edit";
  initialValue?: PolicyDetail;
  onSubmit: (input: CreatePolicyInput | UpdatePolicyInput) => Promise<unknown>;
  submittingLabel?: string;
  submitLabel?: string;
  onCancel?: () => void;
}

/**
 * Shared policy master form used by the "new" and "edit" admin pages.
 * In edit mode the policy code is not re-submitted (immutable identifier).
 */
export function PolicyForm({
  mode,
  initialValue,
  onSubmit,
  submitLabel,
  submittingLabel,
  onCancel,
}: PolicyFormProps) {
  const isEdit = mode === "edit";
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const {
    register,
    handleSubmit,
    formState: { isSubmitting },
  } = useForm<PolicyFormValues>({
    defaultValues: {
      policyCode: initialValue?.policyCode ?? "",
      title: initialValue?.title ?? "",
      ownerDepartment: initialValue?.ownerDepartment ?? "",
      category: initialValue?.category ?? "",
      description: initialValue?.description ?? "",
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
      const payload = {
        ...parsed.data,
        category: parsed.data.category?.trim() || null,
        description: parsed.data.description?.trim() || null,
      };

      if (isEdit) {
        // `policyCode` is display-only in edit mode; strip before sending.
        const { policyCode: _ignored, ...rest } = payload;
        await onSubmit(rest);
      } else {
        await onSubmit(payload);
      }
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
          <label htmlFor="policyCode" className="block text-sm font-medium">
            رمز السياسة
          </label>
          <input
            id="policyCode"
            type="text"
            dir="ltr"
            disabled={isEdit}
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)] disabled:bg-[var(--color-surface-subtle)] disabled:text-[var(--color-text-tertiary)]"
            {...register("policyCode")}
          />
          {fieldErrors.policyCode ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.policyCode}
            </p>
          ) : null}
        </div>

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

        <div className="space-y-1.5">
          <label htmlFor="category" className="block text-sm font-medium">
            الفئة (اختياري)
          </label>
          <input
            id="category"
            type="text"
            className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
            {...register("category")}
          />
          {fieldErrors.category ? (
            <p role="alert" className="text-xs text-red-700">
              {fieldErrors.category}
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
