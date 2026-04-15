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
  archiveAcknowledgmentDefinition,
  archiveAcknowledgmentVersion,
  createAcknowledgmentDefinition,
  createAcknowledgmentVersion,
  getAcknowledgmentDefinition,
  getAcknowledgmentVersion,
  listAcknowledgmentDefinitions,
  listAcknowledgmentVersions,
  publishAcknowledgmentVersion,
  setAcknowledgmentVersionRecurrence,
  updateAcknowledgmentDefinition,
  updateAcknowledgmentVersionDraft,
} from "@/lib/api/acknowledgments";
import type { PagedResult } from "@/lib/policies/types";
import type {
  AcknowledgmentDefinitionDetail,
  AcknowledgmentDefinitionSummary,
  AcknowledgmentListParams,
  AcknowledgmentVersionDetail,
  AcknowledgmentVersionSummary,
  CreateAcknowledgmentDefinitionInput,
  CreateAcknowledgmentVersionInput,
  SetRecurrenceInput,
  UpdateAcknowledgmentDefinitionInput,
  UpdateAcknowledgmentVersionDraftInput,
} from "./types";

/** Stable query key factories so cache invalidation is localized. */
export const acknowledgmentKeys = {
  all: ["acknowledgments"] as const,
  list: (params: AcknowledgmentListParams) =>
    [...acknowledgmentKeys.all, "list", params] as const,
  detail: (definitionId: string) =>
    [...acknowledgmentKeys.all, "detail", definitionId] as const,
  versions: (definitionId: string) =>
    [...acknowledgmentKeys.all, "versions", definitionId] as const,
  version: (definitionId: string, versionId: string) =>
    [...acknowledgmentKeys.all, "version", definitionId, versionId] as const,
};

export function useAcknowledgmentDefinitions(
  params: AcknowledgmentListParams = {},
  options?: Omit<
    UseQueryOptions<PagedResult<AcknowledgmentDefinitionSummary>, ApiError>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery<PagedResult<AcknowledgmentDefinitionSummary>, ApiError>({
    queryKey: acknowledgmentKeys.list(params),
    queryFn: () => listAcknowledgmentDefinitions(params),
    ...options,
  });
}

export function useAcknowledgmentDefinition(definitionId: string | undefined) {
  return useQuery<AcknowledgmentDefinitionDetail, ApiError>({
    queryKey: definitionId
      ? acknowledgmentKeys.detail(definitionId)
      : ["acknowledgments", "detail", "__none__"],
    queryFn: () => getAcknowledgmentDefinition(definitionId as string),
    enabled: !!definitionId,
  });
}

export function useAcknowledgmentVersions(definitionId: string | undefined) {
  return useQuery<AcknowledgmentVersionSummary[], ApiError>({
    queryKey: definitionId
      ? acknowledgmentKeys.versions(definitionId)
      : ["acknowledgments", "versions", "__none__"],
    queryFn: () => listAcknowledgmentVersions(definitionId as string),
    enabled: !!definitionId,
  });
}

export function useAcknowledgmentVersion(
  definitionId: string | undefined,
  versionId: string | undefined,
) {
  return useQuery<AcknowledgmentVersionDetail, ApiError>({
    queryKey:
      definitionId && versionId
        ? acknowledgmentKeys.version(definitionId, versionId)
        : ["acknowledgments", "version", "__none__"],
    queryFn: () =>
      getAcknowledgmentVersion(definitionId as string, versionId as string),
    enabled: !!definitionId && !!versionId,
  });
}

export function useCreateAcknowledgmentDefinition(
  options?: UseMutationOptions<
    AcknowledgmentDefinitionDetail,
    ApiError,
    CreateAcknowledgmentDefinitionInput
  >,
) {
  const qc = useQueryClient();
  return useMutation<
    AcknowledgmentDefinitionDetail,
    ApiError,
    CreateAcknowledgmentDefinitionInput
  >({
    mutationFn: (input) => createAcknowledgmentDefinition(input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useUpdateAcknowledgmentDefinition(
  definitionId: string,
  options?: UseMutationOptions<
    AcknowledgmentDefinitionDetail,
    ApiError,
    UpdateAcknowledgmentDefinitionInput
  >,
) {
  const qc = useQueryClient();
  return useMutation<
    AcknowledgmentDefinitionDetail,
    ApiError,
    UpdateAcknowledgmentDefinitionInput
  >({
    mutationFn: (input) => updateAcknowledgmentDefinition(definitionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.detail(definitionId) });
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useArchiveAcknowledgmentDefinition(
  definitionId: string,
  options?: UseMutationOptions<AcknowledgmentDefinitionDetail, ApiError, void>,
) {
  const qc = useQueryClient();
  return useMutation<AcknowledgmentDefinitionDetail, ApiError, void>({
    mutationFn: () => archiveAcknowledgmentDefinition(definitionId),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.detail(definitionId) });
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useCreateAcknowledgmentVersion(
  definitionId: string,
  options?: UseMutationOptions<
    AcknowledgmentVersionDetail,
    ApiError,
    CreateAcknowledgmentVersionInput
  >,
) {
  const qc = useQueryClient();
  return useMutation<
    AcknowledgmentVersionDetail,
    ApiError,
    CreateAcknowledgmentVersionInput
  >({
    mutationFn: (input) => createAcknowledgmentVersion(definitionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.detail(definitionId) });
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.versions(definitionId) });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useUpdateAcknowledgmentVersionDraft(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<
    AcknowledgmentVersionDetail,
    ApiError,
    UpdateAcknowledgmentVersionDraftInput
  >,
) {
  const qc = useQueryClient();
  return useMutation<
    AcknowledgmentVersionDetail,
    ApiError,
    UpdateAcknowledgmentVersionDraftInput
  >({
    mutationFn: (input) =>
      updateAcknowledgmentVersionDraft(definitionId, versionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({
        queryKey: acknowledgmentKeys.version(definitionId, versionId),
      });
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.versions(definitionId) });
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.detail(definitionId) });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useSetAcknowledgmentVersionRecurrence(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<AcknowledgmentVersionDetail, ApiError, SetRecurrenceInput>,
) {
  const qc = useQueryClient();
  return useMutation<AcknowledgmentVersionDetail, ApiError, SetRecurrenceInput>({
    mutationFn: (input) => setAcknowledgmentVersionRecurrence(definitionId, versionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({
        queryKey: acknowledgmentKeys.version(definitionId, versionId),
      });
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.versions(definitionId) });
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.detail(definitionId) });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function usePublishAcknowledgmentVersion(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<AcknowledgmentVersionDetail, ApiError, void>,
) {
  const qc = useQueryClient();
  return useMutation<AcknowledgmentVersionDetail, ApiError, void>({
    mutationFn: () => publishAcknowledgmentVersion(definitionId, versionId),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useArchiveAcknowledgmentVersion(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<AcknowledgmentVersionDetail, ApiError, void>,
) {
  const qc = useQueryClient();
  return useMutation<AcknowledgmentVersionDetail, ApiError, void>({
    mutationFn: () => archiveAcknowledgmentVersion(definitionId, versionId),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: acknowledgmentKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}
