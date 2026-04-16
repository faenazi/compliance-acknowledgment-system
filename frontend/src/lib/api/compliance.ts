import { apiClient } from "@/lib/api/client";
import type { PagedResult } from "@/lib/policies/types";
import type {
  ComplianceDashboardDto,
  ComplianceDashboardParams,
  NonCompliantUserDetailDto,
  NonCompliantUsersParams,
} from "@/lib/compliance/types";

export async function getComplianceDashboard(
  params: ComplianceDashboardParams = {},
): Promise<ComplianceDashboardDto> {
  const { data } = await apiClient.get<ComplianceDashboardDto>(
    "/api/admin/compliance/dashboard",
    { params },
  );
  return data;
}

export async function listNonCompliantUsers(
  params: NonCompliantUsersParams = {},
): Promise<PagedResult<NonCompliantUserDetailDto>> {
  const { data } = await apiClient.get<PagedResult<NonCompliantUserDetailDto>>(
    "/api/admin/compliance/non-compliant",
    { params },
  );
  return data;
}

export function exportNonCompliantUsersCsvUrl(params: Record<string, string> = {}): string {
  const base = apiClient.defaults.baseURL ?? "";
  const qs = new URLSearchParams(params).toString();
  return `${base}/api/admin/compliance/export/non-compliant${qs ? `?${qs}` : ""}`;
}

export function exportDepartmentComplianceCsvUrl(params: Record<string, string> = {}): string {
  const base = apiClient.defaults.baseURL ?? "";
  const qs = new URLSearchParams(params).toString();
  return `${base}/api/admin/compliance/export/departments${qs ? `?${qs}` : ""}`;
}

export function exportActionComplianceCsvUrl(params: Record<string, string> = {}): string {
  const base = apiClient.defaults.baseURL ?? "";
  const qs = new URLSearchParams(params).toString();
  return `${base}/api/admin/compliance/export/actions${qs ? `?${qs}` : ""}`;
}
