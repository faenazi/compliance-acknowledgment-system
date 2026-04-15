/**
 * Frontend-facing contracts mirroring the Eap.Application.Policies DTOs.
 * Kept in sync with the backend on purpose — enums use numeric codes so
 * the API payload and the UI agree on the exact same state.
 */

export enum PolicyStatus {
  Draft = 0,
  Published = 1,
  Archived = 2,
}

export enum PolicyVersionStatus {
  Draft = 0,
  Published = 1,
  Superseded = 2,
  Archived = 3,
}

export interface PolicyDocument {
  id: string;
  policyVersionId: string;
  fileName: string;
  contentType: string;
  fileSize: number;
  uploadedAtUtc: string;
}

export interface PolicyVersionSummary {
  id: string;
  policyId: string;
  versionNumber: number;
  versionLabel: string | null;
  status: PolicyVersionStatus;
  effectiveDate: string | null;
  publishedAtUtc: string | null;
  archivedAtUtc: string | null;
  hasDocument: boolean;
}

export interface PolicyVersionDetail {
  id: string;
  policyId: string;
  versionNumber: number;
  versionLabel: string | null;
  summary: string | null;
  effectiveDate: string | null;
  status: PolicyVersionStatus;
  publishedAtUtc: string | null;
  publishedBy: string | null;
  archivedAtUtc: string | null;
  archivedBy: string | null;
  supersededByPolicyVersionId: string | null;
  document: PolicyDocument | null;
  createdAtUtc: string;
  createdBy: string | null;
  updatedAtUtc: string | null;
  updatedBy: string | null;
}

export interface PolicySummary {
  id: string;
  policyCode: string;
  title: string;
  ownerDepartment: string;
  category: string | null;
  status: PolicyStatus;
  currentPolicyVersionId: string | null;
  currentVersionNumber: number | null;
  currentEffectiveDate: string | null;
  versionsCount: number;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}

export interface PolicyDetail {
  id: string;
  policyCode: string;
  title: string;
  ownerDepartment: string;
  category: string | null;
  description: string | null;
  status: PolicyStatus;
  currentPolicyVersionId: string | null;
  versions: PolicyVersionSummary[];
  createdAtUtc: string;
  createdBy: string | null;
  updatedAtUtc: string | null;
  updatedBy: string | null;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface PolicyListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PolicyStatus;
  ownerDepartment?: string;
  category?: string;
}

export interface CreatePolicyInput {
  policyCode: string;
  title: string;
  ownerDepartment: string;
  category?: string | null;
  description?: string | null;
}

export type UpdatePolicyInput = Omit<CreatePolicyInput, "policyCode">;

export interface CreatePolicyVersionInput {
  versionLabel?: string | null;
  effectiveDate?: string | null;
  summary?: string | null;
}

export type UpdatePolicyVersionDraftInput = CreatePolicyVersionInput;
