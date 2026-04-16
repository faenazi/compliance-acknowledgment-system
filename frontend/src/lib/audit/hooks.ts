"use client";

import { useQuery, useMutation, type UseQueryOptions } from "@tanstack/react-query";
import type { ApiError } from "@/lib/api/client";
import {
  listAuditLogs,
  listNotifications,
  sendAssignmentNotifications,
  sendReminderNotifications,
  sendOverdueNotifications,
} from "@/lib/api/audit";
import type { PagedResult } from "@/lib/policies/types";
import type {
  AuditLogDto,
  AuditLogListParams,
  NotificationListParams,
  NotificationSummaryDto,
} from "./types";

export const auditKeys = {
  all: ["audit"] as const,
  logs: (params: AuditLogListParams) =>
    [...auditKeys.all, "logs", params] as const,
  notifications: (params: NotificationListParams) =>
    [...auditKeys.all, "notifications", params] as const,
};

export function useAuditLogs(
  params: AuditLogListParams = {},
  options?: Omit<UseQueryOptions<PagedResult<AuditLogDto>, ApiError>, "queryKey" | "queryFn">,
) {
  return useQuery<PagedResult<AuditLogDto>, ApiError>({
    queryKey: auditKeys.logs(params),
    queryFn: () => listAuditLogs(params),
    ...options,
  });
}

export function useNotifications(
  params: NotificationListParams = {},
  options?: Omit<
    UseQueryOptions<PagedResult<NotificationSummaryDto>, ApiError>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery<PagedResult<NotificationSummaryDto>, ApiError>({
    queryKey: auditKeys.notifications(params),
    queryFn: () => listNotifications(params),
    ...options,
  });
}

export function useSendAssignmentNotifications() {
  return useMutation({
    mutationFn: (versionId?: string) => sendAssignmentNotifications(versionId),
  });
}

export function useSendReminderNotifications() {
  return useMutation({
    mutationFn: (days?: number) => sendReminderNotifications(days),
  });
}

export function useSendOverdueNotifications() {
  return useMutation({
    mutationFn: () => sendOverdueNotifications(),
  });
}
