import { apiClient } from "@/lib/api/client";
import type {
  AudienceDefinition,
  AudiencePreview,
  ConfigureAudienceExclusionsInput,
  ConfigureAudienceInclusionInput,
} from "@/lib/audience/types";

/** Returns the audience for a version, or null when none has been configured yet. */
export async function getAudienceByVersion(
  definitionId: string,
  versionId: string,
): Promise<AudienceDefinition | null> {
  const { data, status } = await apiClient.get<AudienceDefinition | "">(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/audience`,
    { validateStatus: (s) => s === 200 || s === 204 },
  );
  if (status === 204 || data === "") {
    return null;
  }
  return data as AudienceDefinition;
}

export async function configureAudienceInclusion(
  definitionId: string,
  versionId: string,
  input: ConfigureAudienceInclusionInput,
): Promise<AudienceDefinition> {
  const { data } = await apiClient.put<AudienceDefinition>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/audience/inclusion`,
    input,
  );
  return data;
}

export async function configureAudienceExclusions(
  definitionId: string,
  versionId: string,
  input: ConfigureAudienceExclusionsInput,
): Promise<AudienceDefinition> {
  const { data } = await apiClient.put<AudienceDefinition>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/audience/exclusions`,
    input,
  );
  return data;
}

export async function setAllUsersAudience(
  definitionId: string,
  versionId: string,
): Promise<AudienceDefinition> {
  const { data } = await apiClient.post<AudienceDefinition>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/audience/all-users`,
  );
  return data;
}

export async function previewAudience(
  definitionId: string,
  versionId: string,
  sampleSize = 25,
): Promise<AudiencePreview> {
  const { data } = await apiClient.get<AudiencePreview>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/audience/preview`,
    { params: { sampleSize } },
  );
  return data;
}
