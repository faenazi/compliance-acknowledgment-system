import { apiClient } from "@/lib/api/client";
import type {
  ConfigureFormDefinitionInput,
  FormDefinitionDto,
  ListSubmissionsResult,
  SubmitFormInput,
  UserSubmissionDetailDto,
} from "@/lib/forms/types";

/**
 * Typed API adapter for form-definition and submission endpoints (Sprint 5).
 */

export async function getFormDefinition(
  definitionId: string,
  versionId: string,
): Promise<FormDefinitionDto | null> {
  const { data, status } = await apiClient.get<FormDefinitionDto>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/form`,
    { validateStatus: (s) => s === 200 || s === 204 },
  );
  return status === 204 ? null : data;
}

export async function configureFormDefinition(
  definitionId: string,
  versionId: string,
  input: ConfigureFormDefinitionInput,
): Promise<FormDefinitionDto> {
  const { data } = await apiClient.put<FormDefinitionDto>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/form`,
    input,
  );
  return data;
}

export async function submitForm(
  definitionId: string,
  versionId: string,
  input: SubmitFormInput,
): Promise<UserSubmissionDetailDto> {
  const { data } = await apiClient.post<UserSubmissionDetailDto>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/submissions`,
    input,
  );
  return data;
}

export async function listSubmissions(
  definitionId: string,
  versionId: string,
  page = 1,
  pageSize = 25,
): Promise<ListSubmissionsResult> {
  const { data } = await apiClient.get<ListSubmissionsResult>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/submissions`,
    { params: { page, pageSize } },
  );
  return data;
}

export async function getSubmission(
  submissionId: string,
): Promise<UserSubmissionDetailDto> {
  const { data } = await apiClient.get<UserSubmissionDetailDto>(
    `/api/submissions/${submissionId}`,
  );
  return data;
}
