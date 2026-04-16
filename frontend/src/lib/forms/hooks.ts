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
  configureFormDefinition,
  getFormDefinition,
  getSubmission,
  listSubmissions,
  submitForm,
} from "@/lib/api/forms";
import type {
  ConfigureFormDefinitionInput,
  FormDefinitionDto,
  ListSubmissionsResult,
  SubmitFormInput,
  UserSubmissionDetailDto,
} from "./types";

/** Stable query-key factories for form-related queries. */
export const formKeys = {
  all: ["forms"] as const,
  definition: (definitionId: string, versionId: string) =>
    [...formKeys.all, "definition", definitionId, versionId] as const,
  submissions: (definitionId: string, versionId: string) =>
    [...formKeys.all, "submissions", definitionId, versionId] as const,
  submission: (submissionId: string) =>
    [...formKeys.all, "submission", submissionId] as const,
};

export function useFormDefinition(
  definitionId: string | undefined,
  versionId: string | undefined,
  options?: Omit<UseQueryOptions<FormDefinitionDto | null, ApiError>, "queryKey" | "queryFn">,
) {
  return useQuery<FormDefinitionDto | null, ApiError>({
    queryKey:
      definitionId && versionId
        ? formKeys.definition(definitionId, versionId)
        : ["forms", "definition", "__none__"],
    queryFn: () => getFormDefinition(definitionId as string, versionId as string),
    enabled: !!definitionId && !!versionId,
    ...options,
  });
}

export function useConfigureFormDefinition(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<FormDefinitionDto, ApiError, ConfigureFormDefinitionInput>,
) {
  const qc = useQueryClient();
  return useMutation<FormDefinitionDto, ApiError, ConfigureFormDefinitionInput>({
    mutationFn: (input) => configureFormDefinition(definitionId, versionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({
        queryKey: formKeys.definition(definitionId, versionId),
      });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useSubmitForm(
  definitionId: string,
  versionId: string,
  options?: UseMutationOptions<UserSubmissionDetailDto, ApiError, SubmitFormInput>,
) {
  const qc = useQueryClient();
  return useMutation<UserSubmissionDetailDto, ApiError, SubmitFormInput>({
    mutationFn: (input) => submitForm(definitionId, versionId, input),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({
        queryKey: formKeys.submissions(definitionId, versionId),
      });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useSubmissions(
  definitionId: string | undefined,
  versionId: string | undefined,
  page = 1,
  pageSize = 25,
) {
  return useQuery<ListSubmissionsResult, ApiError>({
    queryKey:
      definitionId && versionId
        ? [...formKeys.submissions(definitionId, versionId), page, pageSize]
        : ["forms", "submissions", "__none__"],
    queryFn: () => listSubmissions(definitionId as string, versionId as string, page, pageSize),
    enabled: !!definitionId && !!versionId,
  });
}

export function useSubmission(submissionId: string | undefined) {
  return useQuery<UserSubmissionDetailDto, ApiError>({
    queryKey: submissionId
      ? formKeys.submission(submissionId)
      : ["forms", "submission", "__none__"],
    queryFn: () => getSubmission(submissionId as string),
    enabled: !!submissionId,
  });
}
