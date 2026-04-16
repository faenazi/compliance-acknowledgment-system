"use client";

import { useQuery, type UseQueryOptions } from "@tanstack/react-query";
import type { ApiError } from "@/lib/api/client";
import {
  getComplianceDashboard,
  listNonCompliantUsers,
} from "@/lib/api/compliance";
import type { PagedResult } from "@/lib/policies/types";
import type {
  ComplianceDashboardDto,
  ComplianceDashboardParams,
  NonCompliantUserDetailDto,
  NonCompliantUsersParams,
} from "./types";

export const complianceKeys = {
  all: ["compliance"] as const,
  dashboard: (params: ComplianceDashboardParams) =>
    [...complianceKeys.all, "dashboard", params] as const,
  nonCompliant: (params: NonCompliantUsersParams) =>
    [...complianceKeys.all, "non-compliant", params] as const,
};

export function useComplianceDashboard(
  params: ComplianceDashboardParams = {},
  options?: Omit<UseQueryOptions<ComplianceDashboardDto, ApiError>, "queryKey" | "queryFn">,
) {
  return useQuery<ComplianceDashboardDto, ApiError>({
    queryKey: complianceKeys.dashboard(params),
    queryFn: () => getComplianceDashboard(params),
    ...options,
  });
}

export function useNonCompliantUsers(
  params: NonCompliantUsersParams = {},
  options?: Omit<
    UseQueryOptions<PagedResult<NonCompliantUserDetailDto>, ApiError>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery<PagedResult<NonCompliantUserDetailDto>, ApiError>({
    queryKey: complianceKeys.nonCompliant(params),
    queryFn: () => listNonCompliantUsers(params),
    ...options,
  });
}
