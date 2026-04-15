"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
  type UseMutationOptions,
  type UseQueryOptions,
} from "@tanstack/react-query";
import type { ApiError } from "@/lib/api/client";
import {
  archivePolicy,
  archivePolicyVersion,
  createPolicy,
  createPolicyVersion,
  getPolicyById,
  getPolicyVersion,
  listPolicies,
  listPolicyVersions,
  publishPolicyVersion,
  updatePolicy,
  updatePolicyVersionDraft,
  uploadPolicyDocument,
} from "@/lib/api/policies";
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
} from "./types";

/** Stable query key factories so cache invalidation is localized. */
export const policyKeys = {
  all: ["policies"] as const,
  list: (params: PolicyListParams) => [...policyKeys.all, "list", params] as const,
  detail: (policyId: string) => [...policyKeys.all, "detail", policyId] as const,
  versions: (policyId: string) => [...policyKeys.all, "versions", policyId] as const,
  version: (policyId: string, versionId: string) =>
    [...policyKeys.all, "version", policyId, versionId] as const,
};

export function usePolicies(
  params: PolicyListParams = {},
  options?: Omit<UseQueryOptions<PagedResult<PolicySummary>, ApiError>, "queryKey" | "queryFn">,
) {
  return useQuery<PagedResult<PolicySummary>, ApiError>({
    queryKey: policyKeys.list(params),
    queryFn: () => listPolicies(params),
    ...options,
  });
}

export function usePolicy(policyId: string | undefined) {
  return useQuery<PolicyDetail, ApiError>({
    queryKey: policyId ? policyKeys.detail(policyId) : ["policies", "detail", "__none__"],
    queryFn: () => getPolicyById(policyId as string),
    enabled: !!policyId,
  });
}

export function usePolicyVersions(policyId: string | undefined) {
  return useQuery<PolicyVersionSummary[], ApiError>({
    queryKey: policyId ? policyKeys.versions(policyId) : ["policies", "versions", "__none__"],
    queryFn: () => listPolicyVersions(policyId as string),
    enabled: !!policyId,
  });
}

export function usePolicyVersion(policyId: string | undefined, versionId: string | undefined) {
  return useQuery<PolicyVersionDetail, ApiError>({
    queryKey:
      policyId && versionId
        ? policyKeys.version(policyId, versionId)
        : ["policies", "version", "__none__"],
    queryFn: () => getPolicyVersion(policyId as string, versionId as string),
    enabled: !!policyId && !!versionId,
  });
}

export function useCreatePolicy(
  options?: UseMutationOptions<PolicyDetail, ApiError, CreatePolicyInput>,
) {
  const qc = useQueryClient();
  return useMutation<PolicyDetail, ApiError, CreatePolicyInput>({
    mutationFn: (input) => createPolicy(input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: policyKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useUpdatePolicy(
  policyId: string,
  options?: UseMutationOptions<PolicyDetail, ApiError, UpdatePolicyInput>,
) {
  const qc = useQueryClient();
  return useMutation<PolicyDetail, ApiError, UpdatePolicyInput>({
    mutationFn: (input) => updatePolicy(policyId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: policyKeys.detail(policyId) });
      qc.invalidateQueries({ queryKey: policyKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useArchivePolicy(
  policyId: string,
  options?: UseMutationOptions<PolicyDetail, ApiError, void>,
) {
  const qc = useQueryClient();
  return useMutation<PolicyDetail, ApiError, void>({
    mutationFn: () => archivePolicy(policyId),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: policyKeys.detail(policyId) });
      qc.invalidateQueries({ queryKey: policyKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useCreatePolicyVersion(
  policyId: string,
  options?: UseMutationOptions<PolicyVersionDetail, ApiError, CreatePolicyVersionInput>,
) {
  const qc = useQueryClient();
  return useMutation<PolicyVersionDetail, ApiError, CreatePolicyVersionInput>({
    mutationFn: (input) => createPolicyVersion(policyId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: policyKeys.detail(policyId) });
      qc.invalidateQueries({ queryKey: policyKeys.versions(policyId) });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useUpdatePolicyVersionDraft(
  policyId: string,
  versionId: string,
  options?: UseMutationOptions<PolicyVersionDetail, ApiError, UpdatePolicyVersionDraftInput>,
) {
  const qc = useQueryClient();
  return useMutation<PolicyVersionDetail, ApiError, UpdatePolicyVersionDraftInput>({
    mutationFn: (input) => updatePolicyVersionDraft(policyId, versionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: policyKeys.version(policyId, versionId) });
      qc.invalidateQueries({ queryKey: policyKeys.versions(policyId) });
      qc.invalidateQueries({ queryKey: policyKeys.detail(policyId) });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function usePublishPolicyVersion(
  policyId: string,
  versionId: string,
  options?: UseMutationOptions<PolicyVersionDetail, ApiError, void>,
) {
  const qc = useQueryClient();
  return useMutation<PolicyVersionDetail, ApiError, void>({
    mutationFn: () => publishPolicyVersion(policyId, versionId),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: policyKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useArchivePolicyVersion(
  policyId: string,
  versionId: string,
  options?: UseMutationOptions<PolicyVersionDetail, ApiError, void>,
) {
  const qc = useQueryClient();
  return useMutation<PolicyVersionDetail, ApiError, void>({
    mutationFn: () => archivePolicyVersion(policyId, versionId),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: policyKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useUploadPolicyDocument(
  policyId: string,
  versionId: string,
  options?: UseMutationOptions<PolicyDocument, ApiError, File>,
) {
  const qc = useQueryClient();
  return useMutation<PolicyDocument, ApiError, File>({
    mutationFn: (file) => uploadPolicyDocument(policyId, versionId, file),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: policyKeys.version(policyId, versionId) });
      qc.invalidateQueries({ queryKey: policyKeys.versions(policyId) });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}
