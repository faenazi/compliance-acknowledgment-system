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
  generateRequirementsForVersion,
  listRequirementsForVersion,
} from "@/lib/api/requirements";
import type {
  GenerateRequirementsInput,
  RequirementGenerationSummary,
  UserActionRequirement,
  UserActionRequirementStatus,
} from "./types";

export const requirementKeys = {
  all: ["requirements"] as const,
  forVersion: (
    definitionId: string,
    versionId: string,
    params: { status?: UserActionRequirementStatus; currentOnly?: boolean } = {},
  ) => [...requirementKeys.all, definitionId, versionId, params] as const,
};

export function useRequirementsForVersion(
  definitionId: string | undefined,
  versionId: string | undefined,
  params: { status?: UserActionRequirementStatus; currentOnly?: boolean } = { currentOnly: true },
  options?: Omit<UseQueryOptions<UserActionRequirement[], ApiError>, "queryKey" | "queryFn">,
) {
  return useQuery<UserActionRequirement[], ApiError>({
    queryKey:
      definitionId && versionId
        ? requirementKeys.forVersion(definitionId, versionId, params)
        : ["requirements", "__none__"],
    queryFn: () =>
      listRequirementsForVersion(definitionId as string, versionId as string, params),
    enabled: !!definitionId && !!versionId,
    ...options,
  });
}

export function useGenerateRequirements(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<
    RequirementGenerationSummary,
    ApiError,
    GenerateRequirementsInput
  >,
) {
  const qc = useQueryClient();
  return useMutation<RequirementGenerationSummary, ApiError, GenerateRequirementsInput>({
    mutationFn: (input) => generateRequirementsForVersion(definitionId, versionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: requirementKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}
