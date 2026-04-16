"use client";

import {
  useMutation,
  useQuery,
  useQueryClient,
  type UseMutationOptions,
} from "@tanstack/react-query";
import type { ApiError } from "@/lib/api/client";
import {
  getMyActionDetail,
  getMyDashboard,
  getMySubmissionDetail,
  listMyActions,
  listMyHistory,
  submitAcknowledgment,
  submitDisclosure,
} from "@/lib/api/user-portal";
import type {
  MyActionsListParams,
  MyActionDetailDto,
  MyActionSummaryDto,
  MyDashboardDto,
  MyHistoryItemDto,
  MySubmissionDetailDto,
  SubmissionResultDto,
} from "./types";
import type { PagedResult } from "@/lib/policies/types";

/** Stable query key factories for user portal cache management. */
export const userPortalKeys = {
  all: ["user-portal"] as const,
  dashboard: () => [...userPortalKeys.all, "dashboard"] as const,
  actions: (params: MyActionsListParams) =>
    [...userPortalKeys.all, "actions", params] as const,
  actionDetail: (requirementId: string) =>
    [...userPortalKeys.all, "action", requirementId] as const,
  history: (page: number, pageSize: number) =>
    [...userPortalKeys.all, "history", page, pageSize] as const,
  submissionDetail: (submissionId: string) =>
    [...userPortalKeys.all, "submission", submissionId] as const,
};

export function useMyDashboard() {
  return useQuery<MyDashboardDto, ApiError>({
    queryKey: userPortalKeys.dashboard(),
    queryFn: () => getMyDashboard(),
  });
}

export function useMyActions(params: MyActionsListParams = {}) {
  return useQuery<PagedResult<MyActionSummaryDto>, ApiError>({
    queryKey: userPortalKeys.actions(params),
    queryFn: () => listMyActions(params),
  });
}

export function useMyActionDetail(requirementId: string | undefined) {
  return useQuery<MyActionDetailDto, ApiError>({
    queryKey: requirementId
      ? userPortalKeys.actionDetail(requirementId)
      : ["user-portal", "action", "__none__"],
    queryFn: () => getMyActionDetail(requirementId as string),
    enabled: !!requirementId,
  });
}

export function useMyHistory(page = 1, pageSize = 20) {
  return useQuery<PagedResult<MyHistoryItemDto>, ApiError>({
    queryKey: userPortalKeys.history(page, pageSize),
    queryFn: () => listMyHistory(page, pageSize),
  });
}

export function useMySubmissionDetail(submissionId: string | undefined) {
  return useQuery<MySubmissionDetailDto, ApiError>({
    queryKey: submissionId
      ? userPortalKeys.submissionDetail(submissionId)
      : ["user-portal", "submission", "__none__"],
    queryFn: () => getMySubmissionDetail(submissionId as string),
    enabled: !!submissionId,
  });
}

export function useSubmitAcknowledgment(
  options?: UseMutationOptions<SubmissionResultDto, ApiError, string>,
) {
  const qc = useQueryClient();
  return useMutation<SubmissionResultDto, ApiError, string>({
    mutationFn: (requirementId) => submitAcknowledgment(requirementId),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: userPortalKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}

export function useSubmitDisclosure(
  options?: UseMutationOptions<
    SubmissionResultDto,
    ApiError,
    { requirementId: string; submissionJson: string }
  >,
) {
  const qc = useQueryClient();
  return useMutation<
    SubmissionResultDto,
    ApiError,
    { requirementId: string; submissionJson: string }
  >({
    mutationFn: ({ requirementId, submissionJson }) =>
      submitDisclosure(requirementId, submissionJson),
    onSuccess: (data, vars, ctx) => {
      qc.invalidateQueries({ queryKey: userPortalKeys.all });
      options?.onSuccess?.(data, vars, ctx);
    },
    ...options,
  });
}
