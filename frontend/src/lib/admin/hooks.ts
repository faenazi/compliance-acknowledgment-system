"use client";

import { useQuery, type UseQueryOptions } from "@tanstack/react-query";
import type { ApiError } from "@/lib/api/client";
import {
  getAdminDashboard,
  getAdminRequirementDetail,
  getAdminSubmissionDetail,
  listAdminRequirements,
} from "@/lib/api/admin";
import type { PagedResult } from "@/lib/policies/types";
import type {
  AdminDashboardDto,
  AdminRequirementDetailDto,
  AdminRequirementSummaryDto,
  AdminRequirementsListParams,
  AdminSubmissionDetailDto,
} from "./types";

/** Stable query key factories for admin portal queries. */
export const adminKeys = {
  all: ["admin"] as const,
  dashboard: (recentLimit: number) =>
    [...adminKeys.all, "dashboard", recentLimit] as const,
  requirements: (params: AdminRequirementsListParams) =>
    [...adminKeys.all, "requirements", params] as const,
  requirementDetail: (id: string) =>
    [...adminKeys.all, "requirement", id] as const,
  submissionDetail: (id: string) =>
    [...adminKeys.all, "submission", id] as const,
};

export function useAdminDashboard(
  recentLimit = 5,
  options?: Omit<UseQueryOptions<AdminDashboardDto, ApiError>, "queryKey" | "queryFn">,
) {
  return useQuery<AdminDashboardDto, ApiError>({
    queryKey: adminKeys.dashboard(recentLimit),
    queryFn: () => getAdminDashboard(recentLimit),
    ...options,
  });
}

export function useAdminRequirements(
  params: AdminRequirementsListParams = {},
  options?: Omit<
    UseQueryOptions<PagedResult<AdminRequirementSummaryDto>, ApiError>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery<PagedResult<AdminRequirementSummaryDto>, ApiError>({
    queryKey: adminKeys.requirements(params),
    queryFn: () => listAdminRequirements(params),
    ...options,
  });
}

export function useAdminRequirementDetail(
  requirementId: string | undefined,
  options?: Omit<UseQueryOptions<AdminRequirementDetailDto, ApiError>, "queryKey" | "queryFn">,
) {
  return useQuery<AdminRequirementDetailDto, ApiError>({
    queryKey: requirementId
      ? adminKeys.requirementDetail(requirementId)
      : ["admin", "requirement", "__none__"],
    queryFn: () => getAdminRequirementDetail(requirementId as string),
    enabled: !!requirementId,
    ...options,
  });
}

export function useAdminSubmissionDetail(
  submissionId: string | undefined,
  options?: Omit<UseQueryOptions<AdminSubmissionDetailDto, ApiError>, "queryKey" | "queryFn">,
) {
  return useQuery<AdminSubmissionDetailDto, ApiError>({
    queryKey: submissionId
      ? adminKeys.submissionDetail(submissionId)
      : ["admin", "submission", "__none__"],
    queryFn: () => getAdminSubmissionDetail(submissionId as string),
    enabled: !!submissionId,
    ...options,
  });
}
