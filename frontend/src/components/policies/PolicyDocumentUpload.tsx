"use client";

import { useRef, useState } from "react";
import { Button } from "@/components/ui/button";
import type { ApiError } from "@/lib/api/client";
import { formatFileSize } from "@/lib/policies/labels";
import { useUploadPolicyDocument } from "@/lib/policies/hooks";
import { policyDocumentDownloadUrl } from "@/lib/api/policies";
import type { PolicyDocument } from "@/lib/policies/types";

interface PolicyDocumentUploadProps {
  policyId: string;
  versionId: string;
  existingDocument: PolicyDocument | null;
  disabled?: boolean;
  /** Array of allowed extensions, e.g. [".pdf"]. Used for the accept attribute. */
  acceptExtensions?: readonly string[];
}

/**
 * Upload control for the single document attached to a draft version.
 * The "accept" filter is advisory — the backend enforces the whitelist
 * configured in PolicyDocuments:AllowedExtensions / AllowedContentTypes.
 */
export function PolicyDocumentUpload({
  policyId,
  versionId,
  existingDocument,
  disabled,
  acceptExtensions = [".pdf"],
}: PolicyDocumentUploadProps) {
  const fileInput = useRef<HTMLInputElement>(null);
  const [error, setError] = useState<string | null>(null);
  const [selectedName, setSelectedName] = useState<string | null>(null);
  const upload = useUploadPolicyDocument(policyId, versionId, {
    onSuccess: () => {
      setError(null);
      setSelectedName(null);
      if (fileInput.current) fileInput.current.value = "";
    },
    onError: (err) => {
      setError(formatError(err));
    },
  });

  const onSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setSelectedName(file.name);
    upload.mutate(file);
  };

  return (
    <div className="space-y-4">
      {existingDocument ? (
        <div className="flex flex-wrap items-center justify-between gap-3 rounded-[10px] border border-[var(--color-border-default)] bg-[var(--color-surface-subtle)] px-4 py-3">
          <div>
            <div className="text-sm font-medium">{existingDocument.fileName}</div>
            <div className="text-xs text-[var(--color-text-tertiary)]">
              {existingDocument.contentType} · {formatFileSize(existingDocument.fileSize)}
            </div>
          </div>
          <a
            href={policyDocumentDownloadUrl(policyId, versionId)}
            target="_blank"
            rel="noopener noreferrer"
            className="text-sm font-medium text-[var(--color-text-link)] hover:underline"
          >
            فتح الملف
          </a>
        </div>
      ) : (
        <p className="text-sm text-[var(--color-text-tertiary)]">
          لم يتم إرفاق ملف لهذه النسخة بعد. يتعيّن إرفاق ملف قبل نشر النسخة (BR-010).
        </p>
      )}

      {!disabled ? (
        <div className="flex flex-wrap items-center gap-3">
          <input
            ref={fileInput}
            type="file"
            accept={acceptExtensions.join(",")}
            onChange={onSelect}
            className="hidden"
            id="policy-document-file"
          />
          <Button
            type="button"
            variant="secondary"
            onClick={() => fileInput.current?.click()}
            disabled={upload.isPending}
          >
            {upload.isPending
              ? "جاري الرفع…"
              : existingDocument
                ? "استبدال الملف"
                : "رفع ملف"}
          </Button>
          {selectedName && upload.isPending ? (
            <span className="text-xs text-[var(--color-text-tertiary)]">
              {selectedName}
            </span>
          ) : null}
        </div>
      ) : (
        <p className="text-xs text-[var(--color-text-tertiary)]">
          يُسمح برفع الملفات على النسخ في حالة "مسودة" فقط.
        </p>
      )}

      {error ? (
        <div
          role="alert"
          className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800"
        >
          {error}
        </div>
      ) : null}
    </div>
  );
}

function formatError(err: ApiError): string {
  if (err.errors) {
    return Object.values(err.errors).flat().join(" ") || err.title;
  }
  return err.detail ?? err.title ?? "تعذّر رفع الملف.";
}
