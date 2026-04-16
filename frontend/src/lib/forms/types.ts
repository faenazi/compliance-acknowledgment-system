/**
 * Frontend-facing contracts mirroring the Eap.Domain.Forms domain.
 * Numeric codes match the backend enum values exactly.
 */

export enum FormFieldType {
  ShortText = 0,
  LongText = 1,
  Number = 2,
  Decimal = 3,
  Date = 4,
  Checkbox = 5,
  RadioGroup = 6,
  Dropdown = 7,
  MultiSelect = 8,
  YesNo = 9,
  Email = 10,
  PhoneNumber = 11,
  FileUpload = 12,
  ReadOnlyDisplay = 13,
  SectionHeader = 14,
}

export interface FieldOption {
  value: string;
  label: string;
}

export interface FormFieldDto {
  id: string;
  fieldKey: string;
  label: string;
  fieldType: FormFieldType;
  isRequired: boolean;
  sortOrder: number;
  sectionKey: string | null;
  helpText: string | null;
  placeholder: string | null;
  displayText: string | null;
  options: FieldOption[];
}

export interface FormDefinitionDto {
  id: string;
  acknowledgmentVersionId: string;
  schemaVersion: number;
  isActive: boolean;
  fields: FormFieldDto[];
  createdAtUtc: string;
  createdBy: string | null;
  updatedAtUtc: string | null;
  updatedBy: string | null;
}

export interface FormFieldInput {
  fieldKey: string;
  label: string;
  fieldType: FormFieldType;
  isRequired: boolean;
  sectionKey?: string | null;
  helpText?: string | null;
  placeholder?: string | null;
  displayText?: string | null;
  options?: FieldOption[] | null;
}

export interface ConfigureFormDefinitionInput {
  fields: FormFieldInput[];
}

export interface SubmitFormInput {
  submissionJson: string;
}

export interface SubmissionFieldValueDto {
  id: string;
  fieldKey: string;
  fieldLabel: string;
  fieldType: FormFieldType;
  valueText: string | null;
  valueNumber: number | null;
  valueDate: string | null;
  valueBoolean: boolean | null;
  valueJson: string | null;
}

export interface UserSubmissionSummaryDto {
  id: string;
  userId: string;
  acknowledgmentVersionId: string;
  formDefinitionId: string;
  status: number;
  submittedAtUtc: string;
  createdBy: string | null;
}

export interface UserSubmissionDetailDto {
  id: string;
  userId: string;
  acknowledgmentVersionId: string;
  formDefinitionId: string;
  submissionJson: string;
  formDefinitionSnapshotJson: string;
  status: number;
  submittedAtUtc: string;
  fieldValues: SubmissionFieldValueDto[];
  createdBy: string | null;
  createdAtUtc: string;
}

export interface ListSubmissionsResult {
  items: UserSubmissionSummaryDto[];
  totalCount: number;
}

/** Helper: does a field type collect user input? */
export function isDisplayOnly(type: FormFieldType): boolean {
  return type === FormFieldType.ReadOnlyDisplay || type === FormFieldType.SectionHeader;
}

/** Helper: does a field type require options? */
export function requiresOptions(type: FormFieldType): boolean {
  return (
    type === FormFieldType.RadioGroup ||
    type === FormFieldType.Dropdown ||
    type === FormFieldType.MultiSelect
  );
}
