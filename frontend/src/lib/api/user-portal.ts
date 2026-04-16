import { apiClient } from "@/lib/api/client";
import type { PagedResult } from "@/lib/policies/types";
import type {
  MyActionsListParams,
  MyActionDetailDto,
  MyActionSummaryDto,
  MyDashboardDto,
  MyHistoryItemDto,
  MySubmissionDetailDto,
  SubmissionResultDto,
} from "@/lib/user-portal/types";

/**
 * Typed API adapter for user portal endpoints (Sprint 6).
 * All endpoints are scoped to the current authenticated user via /api/me/.
 */

export async function getMyDashboard(
  pendingLimit = 5,
  recentLimit = 5,
): Promise<MyDashboardDto> {
  const { data } = await apiClient.get<MyDashboardDto>("/api/me/dashboard", {
    params: { pendingLimit, recentLimit },
  });
  return data;
}

export async function listMyActions(
  params: MyActionsListParams = {},
): Promise<PagedResult<MyActionSummaryDto>> {
  const { data } = await apiClient.get<PagedResult<MyActionSummaryDto>>(
    "/api/me/actions",
    { params },
  );
  return data;
}

export async function getMyActionDetail(
  requirementId: string,
): Promise<MyActionDetailDto> {
  const { data } = await apiClient.get<MyActionDetailDto>(
    `/api/me/actions/${requirementId}`,
  );
  return data;
}

export async function submitAcknowledgment(
  requirementId: string,
): Promise<SubmissionResultDto> {
  const { data } = await apiClient.post<SubmissionResultDto>(
    `/api/me/actions/${requirementId}/acknowledge`,
  );
  return data;
}

export async function submitDisclosure(
  requirementId: string,
  submissionJson: string,
): Promise<SubmissionResultDto> {
  const { data } = await apiClient.post<SubmissionResultDto>(
    `/api/me/actions/${requirementId}/disclose`,
    { submissionJson },
  );
  return data;
}

export async function listMyHistory(
  page = 1,
  pageSize = 20,
): Promise<PagedResult<MyHistoryItemDto>> {
  const { data } = await apiClient.get<PagedResult<MyHistoryItemDto>>(
    "/api/me/history",
    { params: { page, pageSize } },
  );
  return data;
}

export async function getMySubmissionDetail(
  submissionId: string,
): Promise<MySubmissionDetailDto> {
  const { data } = await apiClient.get<MySubmissionDetailDto>(
    `/api/me/history/${submissionId}`,
  );
  return data;
}
