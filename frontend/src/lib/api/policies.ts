import { apiClient } from "@/lib/api/client";
import type {
  CreatePolicyInput,
  CreatePolicyVersionInput,
  PagedResult,
  PolicyDetail,
  PolicyDocument,
  PolicyListParams,
  PolicySummary,
  PolicyVersionDetail,
  PolicyVersionSummary,
  UpdatePolicyInput,
  UpdatePolicyVersionDraftInput,
} from "@/lib/policies/types";

/**
 * Typed API adapter for the policy management endpoints. Keeps request
 * building out of hooks/components so the transport is easy to refactor.
 */

export async function listPolicies(
  params: PolicyListParams,
): Promise<PagedResult<PolicySummary>> {
  const { data } = await apiClient.get<PagedResult<PolicySummary>>(
    "/api/policies",
    { params },
  );
  return data;
}

export async function getPolicyById(policyId: string): Promise<PolicyDetail> {
  const { data } = await apiClient.get<PolicyDetail>(`/api/policies/${policyId}`);
  return data;
}

export async function createPolicy(input: CreatePolicyInput): Promise<PolicyDetail> {
  const { data } = await apiClient.post<PolicyDetail>("/api/policies", input);
  return data;
}

export async function updatePolicy(
  policyId: string,
  input: UpdatePolicyInput,
): Promise<PolicyDetail> {
  const { data } = await apiClient.put<PolicyDetail>(`/api/policies/${policyId}`, input);
  return data;
}

export async function archivePolicy(policyId: string): Promise<PolicyDetail> {
  const { data } = await apiClient.post<PolicyDetail>(`/api/policies/${policyId}/archive`);
  return data;
}

export async function listPolicyVersions(
  policyId: string,
): Promise<PolicyVersionSummary[]> {
  const { data } = await apiClient.get<PolicyVersionSummary[]>(
    `/api/policies/${policyId}/versions`,
  );
  return data;
}

export async function getPolicyVersion(
  policyId: string,
  versionId: string,
): Promise<PolicyVersionDetail> {
  const { data } = await apiClient.get<PolicyVersionDetail>(
    `/api/policies/${policyId}/versions/${versionId}`,
  );
  return data;
}

export async function createPolicyVersion(
  policyId: string,
  input: CreatePolicyVersionInput,
): Promise<PolicyVersionDetail> {
  const { data } = await apiClient.post<PolicyVersionDetail>(
    `/api/policies/${policyId}/versions`,
    input,
  );
  return data;
}

export async function updatePolicyVersionDraft(
  policyId: string,
  versionId: string,
  input: UpdatePolicyVersionDraftInput,
): Promise<PolicyVersionDetail> {
  const { data } = await apiClient.put<PolicyVersionDetail>(
    `/api/policies/${policyId}/versions/${versionId}`,
    input,
  );
  return data;
}

export async function publishPolicyVersion(
  policyId: string,
  versionId: string,
): Promise<PolicyVersionDetail> {
  const { data } = await apiClient.post<PolicyVersionDetail>(
    `/api/policies/${policyId}/versions/${versionId}/publish`,
  );
  return data;
}

export async function archivePolicyVersion(
  policyId: string,
  versionId: string,
): Promise<PolicyVersionDetail> {
  const { data } = await apiClient.post<PolicyVersionDetail>(
    `/api/policies/${policyId}/versions/${versionId}/archive`,
  );
  return data;
}

export async function uploadPolicyDocument(
  policyId: string,
  versionId: string,
  file: File,
): Promise<PolicyDocument> {
  const form = new FormData();
  form.append("file", file, file.name);

  const { data } = await apiClient.post<PolicyDocument>(
    `/api/policies/${policyId}/versions/${versionId}/document`,
    form,
    { headers: { "Content-Type": "multipart/form-data" } },
  );
  return data;
}

/** Absolute URL to the current version's document (used for opening in a new tab). */
export function policyDocumentDownloadUrl(
  policyId: string,
  versionId: string,
): string {
  const base = apiClient.defaults.baseURL ?? "";
  return `${base}/api/policies/${policyId}/versions/${versionId}/document`;
}
