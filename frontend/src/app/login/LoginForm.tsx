"use client";

import { useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import { useSession } from "@/lib/auth/SessionProvider";
import { ADMIN_ROLES, hasAnyRole } from "@/lib/auth/roles";
import { fetchCurrentUser } from "@/lib/auth/session-api";
import type { ApiError } from "@/lib/api/client";

const loginSchema = z.object({
  username: z.string().trim().min(1, "اسم المستخدم مطلوب"),
  password: z.string().min(1, "كلمة المرور مطلوبة"),
});

type FormValues = z.infer<typeof loginSchema>;
type FieldErrors = Partial<Record<keyof FormValues, string>>;

/**
 * Client-side LDAP login form. Credentials are posted to the backend
 * /api/auth/login endpoint; authentication is performed against the
 * corporate directory on the server, never in the browser.
 */
export function LoginForm() {
  const router = useRouter();
  const params = useSearchParams();
  const { login } = useSession();
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  const { register, handleSubmit, formState: { isSubmitting } } = useForm<FormValues>({
    defaultValues: { username: "", password: "" },
  });

  const onSubmit = handleSubmit(async (values) => {
    setSubmissionError(null);
    setFieldErrors({});

    const parsed = loginSchema.safeParse(values);
    if (!parsed.success) {
      const errors: FieldErrors = {};
      for (const issue of parsed.error.issues) {
        const key = issue.path[0] as keyof FormValues | undefined;
        if (key && !errors[key]) {
          errors[key] = issue.message;
        }
      }
      setFieldErrors(errors);
      return;
    }

    try {
      await login(parsed.data.username, parsed.data.password);
      const next = params.get("next");
      if (next && next.startsWith("/")) {
        router.replace(next);
        return;
      }

      const me = await safeFetchMe();
      const landing =
        me && hasAnyRole(me.roles, ADMIN_ROLES) ? "/admin/dashboard" : "/dashboard";
      router.replace(landing);
    } catch (error) {
      setSubmissionError(formatError(error));
    }
  });

  return (
    <form onSubmit={onSubmit} noValidate className="space-y-4">
      <div className="space-y-1.5">
        <label htmlFor="username" className="block text-sm font-medium">
          اسم المستخدم
        </label>
        <input
          id="username"
          type="text"
          autoComplete="username"
          dir="ltr"
          className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
          {...register("username")}
        />
        {fieldErrors.username ? (
          <p role="alert" className="text-xs text-[var(--color-feedback-error-text)]">
            {fieldErrors.username}
          </p>
        ) : null}
      </div>

      <div className="space-y-1.5">
        <label htmlFor="password" className="block text-sm font-medium">
          كلمة المرور
        </label>
        <input
          id="password"
          type="password"
          autoComplete="current-password"
          dir="ltr"
          className="block h-11 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none transition-colors focus:border-[var(--color-brand-primary)]"
          {...register("password")}
        />
        {fieldErrors.password ? (
          <p role="alert" className="text-xs text-[var(--color-feedback-error-text)]">
            {fieldErrors.password}
          </p>
        ) : null}
      </div>

      {submissionError ? (
        <div
          role="alert"
          className="rounded-md border px-3 py-2 text-sm"
          style={{
            backgroundColor: "var(--color-feedback-error-bg)",
            borderColor: "var(--color-feedback-error-border)",
            color: "var(--color-feedback-error-text)",
          }}
        >
          {submissionError}
        </div>
      ) : null}

      <Button type="submit" disabled={isSubmitting} className="w-full">
        {isSubmitting ? "جاري التحقق…" : "تسجيل الدخول"}
      </Button>
    </form>
  );
}

async function safeFetchMe() {
  try {
    return await fetchCurrentUser();
  } catch {
    return null;
  }
}

function formatError(error: unknown): string {
  const apiError = error as ApiError | undefined;
  if (!apiError) return "حدث خطأ غير متوقع.";
  if (apiError.status === 401) return "اسم المستخدم أو كلمة المرور غير صحيحة.";
  if (apiError.errors) {
    return Object.values(apiError.errors).flat().join(" ") || apiError.title || "تحقق من المدخلات.";
  }
  return apiError.detail ?? apiError.title ?? "تعذّر تسجيل الدخول.";
}
