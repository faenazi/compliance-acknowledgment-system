/**
 * Frontend contracts for audit log APIs (Sprint 8).
 */

export interface AuditLogDto {
  id: string;
  actorUserId: string | null;
  actorUsername: string | null;
  actionType: string;
  entityType: string;
  entityId: string | null;
  entityVersionId: string | null;
  description: string | null;
  actionTimestampUtc: string;
  hasBeforeSnapshot: boolean;
  hasAfterSnapshot: boolean;
}

export interface AuditLogListParams {
  page?: number;
  pageSize?: number;
  actionType?: string;
  entityType?: string;
  actorUserId?: string;
  fromDate?: string;
  toDate?: string;
  search?: string;
}

export interface NotificationSummaryDto {
  id: string;
  userId: string;
  recipientEmail: string;
  recipientName: string | null;
  notificationType: number;
  status: number;
  relatedEntityType: string;
  relatedEntityId: string;
  subject: string;
  createdAtUtc: string;
  sentAtUtc: string | null;
  attemptCount: number;
  lastFailureReason: string | null;
}

export interface NotificationResultDto {
  totalProcessed: number;
  sent: number;
  failed: number;
  skipped: number;
}

export interface NotificationListParams {
  page?: number;
  pageSize?: number;
  type?: number;
  status?: number;
  search?: string;
}
