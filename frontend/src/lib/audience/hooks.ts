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
  configureAudienceExclusions,
  configureAudienceInclusion,
  getAudienceByVersion,
  previewAudience,
  setAllUsersAudience,
} from "@/lib/api/audience";
import type {
  AudienceDefinition,
  AudiencePreview,
  ConfigureAudienceExclusionsInput,
  ConfigureAudienceInclusionInput,
} from "./types";

export const audienceKeys = {
  all: ["audience"] as const,
  byVersion: (definitionId: string, versionId: string) =>
    [...audienceKeys.all, definitionId, versionId] as const,
  preview: (definitionId: string, versionId: string) =>
    [...audienceKeys.all, "preview", definitionId, versionId] as const,
};

export function useAudience(
  definitionId: string | undefined,
  versionId: string | undefined,
  options?: Omit<
    UseQueryOptions<AudienceDefinition | null, ApiError>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery<AudienceDefinition | null, ApiError>({
    queryKey:
      definitionId && versionId
        ? audienceKeys.byVersion(definitionId, versionId)
        : ["audience", "__none__"],
    queryFn: () => getAudienceByVersion(definitionId as string, versionId as string),
    enabled: !!definitionId && !!versionId,
    ...options,
  });
}

export function useAudiencePreview(
  definitionId: string | undefined,
  versionId: string | undefined,
  options?: Omit<UseQueryOptions<AudiencePreview, ApiError>, "queryKey" | "queryFn">,
) {
  return useQuery<AudiencePreview, ApiError>({
    queryKey:
      definitionId && versionId
        ? audienceKeys.preview(definitionId, versionId)
        : ["audience", "preview", "__none__"],
    queryFn: () => previewAudience(definitionId as string, versionId as string),
    enabled: !!definitionId && !!versionId,
    ...options,
  });
}

export function useConfigureAudienceInclusion(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<
    AudienceDefinition,
    ApiError,
    ConfigureAudienceInclusionInput
  >,
) {
  const qc = useQueryClient();
  return useMutation<AudienceDefinition, ApiError, ConfigureAudienceInclusionInput>({
    mutationFn: (input) => configureAudienceInclusion(definitionId, versionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: audienceKeys.byVersion(definitionId, versionId) });
      qc.invalidateQueries({ queryKey: audienceKeys.preview(definitionId, versionId) });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useConfigureAudienceExclusions(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<
    AudienceDefinition,
    ApiError,
    ConfigureAudienceExclusionsInput
  >,
) {
  const qc = useQueryClient();
  return useMutation<AudienceDefinition, ApiError, ConfigureAudienceExclusionsInput>({
    mutationFn: (input) => configureAudienceExclusions(definitionId, versionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: audienceKeys.byVersion(definitionId, versionId) });
      qc.invalidateQueries({ queryKey: audienceKeys.preview(definitionId, versionId) });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useSetAllUsersAudience(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<AudienceDefinition, ApiError, void>,
) {
  const qc = useQueryClient();
  return useMutation<AudienceDefinition, ApiError, void>({
    mutationFn: () => setAllUsersAudience(definitionId, versionId),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: audienceKeys.byVersion(definitionId, versionId) });
      qc.invalidateQueries({ queryKey: audienceKeys.preview(definitionId, versionId) });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}
