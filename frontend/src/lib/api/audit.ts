import { apiClient } from "@/lib/api/client";
import type { PagedResult } from "@/lib/policies/types";
import type {
  AuditLogDto,
  AuditLogListParams,
  NotificationListParams,
  NotificationResultDto,
  NotificationSummaryDto,
} from "@/lib/audit/types";

export async function listAuditLogs(
  params: AuditLogListParams = {},
): Promise<PagedResult<AuditLogDto>> {
  const { data } = await apiClient.get<PagedResult<AuditLogDto>>(
    "/api/admin/audit",
    { params },
  );
  return data;
}

export function exportAuditLogCsvUrl(params: Record<string, string> = {}): string {
  const base = apiClient.defaults.baseURL ?? "";
  const qs = new URLSearchParams(params).toString();
  return `${base}/api/admin/audit/export${qs ? `?${qs}` : ""}`;
}

export async function listNotifications(
  params: NotificationListParams = {},
): Promise<PagedResult<NotificationSummaryDto>> {
  const { data } = await apiClient.get<PagedResult<NotificationSummaryDto>>(
    "/api/admin/notifications",
    { params },
  );
  return data;
}

export async function sendAssignmentNotifications(
  acknowledgmentVersionId?: string,
): Promise<NotificationResultDto> {
  const { data } = await apiClient.post<NotificationResultDto>(
    "/api/admin/notifications/send-assignments",
    null,
    { params: acknowledgmentVersionId ? { acknowledgmentVersionId } : {} },
  );
  return data;
}

export async function sendReminderNotifications(
  reminderDaysBeforeDue?: number,
): Promise<NotificationResultDto> {
  const { data } = await apiClient.post<NotificationResultDto>(
    "/api/admin/notifications/send-reminders",
    null,
    { params: reminderDaysBeforeDue ? { reminderDaysBeforeDue } : {} },
  );
  return data;
}

export async function sendOverdueNotifications(): Promise<NotificationResultDto> {
  const { data } = await apiClient.post<NotificationResultDto>(
    "/api/admin/notifications/send-overdue",
  );
  return data;
}
