import { apiClient } from "@/lib/api/client";
import type { PagedResult } from "@/lib/policies/types";
import type {
  AcknowledgmentDefinitionDetail,
  AcknowledgmentDefinitionSummary,
  AcknowledgmentListParams,
  AcknowledgmentVersionDetail,
  AcknowledgmentVersionSummary,
  CreateAcknowledgmentDefinitionInput,
  CreateAcknowledgmentVersionInput,
  SetRecurrenceInput,
  UpdateAcknowledgmentDefinitionInput,
  UpdateAcknowledgmentVersionDraftInput,
} from "@/lib/acknowledgments/types";

/**
 * Typed API adapter for the acknowledgment management endpoints. Keeps
 * request building out of hooks/components so the transport is easy to
 * refactor.
 */

export async function listAcknowledgmentDefinitions(
  params: AcknowledgmentListParams,
): Promise<PagedResult<AcknowledgmentDefinitionSummary>> {
  const { data } = await apiClient.get<PagedResult<AcknowledgmentDefinitionSummary>>(
    "/api/acknowledgments",
    { params },
  );
  return data;
}

export async function getAcknowledgmentDefinition(
  definitionId: string,
): Promise<AcknowledgmentDefinitionDetail> {
  const { data } = await apiClient.get<AcknowledgmentDefinitionDetail>(
    `/api/acknowledgments/${definitionId}`,
  );
  return data;
}

export async function createAcknowledgmentDefinition(
  input: CreateAcknowledgmentDefinitionInput,
): Promise<AcknowledgmentDefinitionDetail> {
  const { data } = await apiClient.post<AcknowledgmentDefinitionDetail>(
    "/api/acknowledgments",
    input,
  );
  return data;
}

export async function updateAcknowledgmentDefinition(
  definitionId: string,
  input: UpdateAcknowledgmentDefinitionInput,
): Promise<AcknowledgmentDefinitionDetail> {
  const { data } = await apiClient.put<AcknowledgmentDefinitionDetail>(
    `/api/acknowledgments/${definitionId}`,
    input,
  );
  return data;
}

export async function archiveAcknowledgmentDefinition(
  definitionId: string,
): Promise<AcknowledgmentDefinitionDetail> {
  const { data } = await apiClient.post<AcknowledgmentDefinitionDetail>(
    `/api/acknowledgments/${definitionId}/archive`,
  );
  return data;
}

export async function listAcknowledgmentVersions(
  definitionId: string,
): Promise<AcknowledgmentVersionSummary[]> {
  const { data } = await apiClient.get<AcknowledgmentVersionSummary[]>(
    `/api/acknowledgments/${definitionId}/versions`,
  );
  return data;
}

export async function getAcknowledgmentVersion(
  definitionId: string,
  versionId: string,
): Promise<AcknowledgmentVersionDetail> {
  const { data } = await apiClient.get<AcknowledgmentVersionDetail>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}`,
  );
  return data;
}

export async function createAcknowledgmentVersion(
  definitionId: string,
  input: CreateAcknowledgmentVersionInput,
): Promise<AcknowledgmentVersionDetail> {
  const { data } = await apiClient.post<AcknowledgmentVersionDetail>(
    `/api/acknowledgments/${definitionId}/versions`,
    input,
  );
  return data;
}

export async function updateAcknowledgmentVersionDraft(
  definitionId: string,
  versionId: string,
  input: UpdateAcknowledgmentVersionDraftInput,
): Promise<AcknowledgmentVersionDetail> {
  const { data } = await apiClient.put<AcknowledgmentVersionDetail>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}`,
    input,
  );
  return data;
}

export async function setAcknowledgmentVersionRecurrence(
  definitionId: string,
  versionId: string,
  input: SetRecurrenceInput,
): Promise<AcknowledgmentVersionDetail> {
  const { data } = await apiClient.put<AcknowledgmentVersionDetail>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/recurrence`,
    input,
  );
  return data;
}

export async function publishAcknowledgmentVersion(
  definitionId: string,
  versionId: string,
): Promise<AcknowledgmentVersionDetail> {
  const { data } = await apiClient.post<AcknowledgmentVersionDetail>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/publish`,
  );
  return data;
}

export async function archiveAcknowledgmentVersion(
  definitionId: string,
  versionId: string,
): Promise<AcknowledgmentVersionDetail> {
  const { data } = await apiClient.post<AcknowledgmentVersionDetail>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/archive`,
  );
  return data;
}
