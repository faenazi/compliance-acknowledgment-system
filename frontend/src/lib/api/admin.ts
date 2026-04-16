import { apiClient } from "@/lib/api/client";
import type { PagedResult } from "@/lib/policies/types";
import type {
  AdminDashboardDto,
  AdminRequirementDetailDto,
  AdminRequirementSummaryDto,
  AdminRequirementsListParams,
  AdminSubmissionDetailDto,
} from "@/lib/admin/types";

/**
 * Typed API adapter for admin portal endpoints (Sprint 7).
 */

export async function getAdminDashboard(
  recentLimit = 5,
): Promise<AdminDashboardDto> {
  const { data } = await apiClient.get<AdminDashboardDto>(
    "/api/admin/dashboard",
    { params: { recentLimit } },
  );
  return data;
}

export async function listAdminRequirements(
  params: AdminRequirementsListParams,
): Promise<PagedResult<AdminRequirementSummaryDto>> {
  const { data } = await apiClient.get<PagedResult<AdminRequirementSummaryDto>>(
    "/api/admin/monitoring/requirements",
    { params },
  );
  return data;
}

export async function getAdminRequirementDetail(
  requirementId: string,
): Promise<AdminRequirementDetailDto> {
  const { data } = await apiClient.get<AdminRequirementDetailDto>(
    `/api/admin/monitoring/requirements/${requirementId}`,
  );
  return data;
}

export async function getAdminSubmissionDetail(
  submissionId: string,
): Promise<AdminSubmissionDetailDto> {
  const { data } = await apiClient.get<AdminSubmissionDetailDto>(
    `/api/admin/monitoring/submissions/${submissionId}`,
  );
  return data;
}
