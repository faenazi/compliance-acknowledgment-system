# Conceptual Data Model

## 1. Purpose

This document defines the conceptual data model for the Enterprise Acknowledgment Platform (EAP).

It identifies the main business entities, their roles, and the relationships between them.

This is a conceptual model, not a physical database design.
It is intended to provide a shared understanding of the data structure before detailed schema design.

---

## 2. Modeling Principles

The conceptual data model follows these principles:

- policy and acknowledgment records are versioned
- historical integrity must be preserved
- form-based disclosures are first-class business objects
- user submissions must remain linked to the exact version used at the time of submission
- identity data is sourced from LDAP / Active Directory
- roles and scopes are modeled independently from organizational departments

---

## 3. Core Entity Groups

The solution data model consists of the following groups:

### 3.1 Identity & Access
- User
- UserRoleAssignment
- Role
- Scope

### 3.2 Policy Management
- Policy
- PolicyVersion
- PolicyDocument

### 3.3 Acknowledgment & Disclosure
- AcknowledgmentDefinition
- AcknowledgmentVersion
- FormDefinition

### 3.4 Targeting & Assignment
- AudienceDefinition
- AudienceRule
- UserActionRequirement

### 3.5 Submission & Compliance
- UserSubmission
- UserSubmissionFieldValue
- ComplianceSnapshot

### 3.6 Notifications & Audit
- Notification
- NotificationAttempt
- AuditLog

---

## 4. Identity & Access Entities

## 4.1 User

Represents an authenticated employee or internal platform user.

### Key Attributes
- UserId
- Username
- FullName
- Email
- Department
- JobTitle
- IsActive
- ADSourceId / DirectoryReference
- CreatedAt
- LastSyncedAt

### Notes
- Created from LDAP / Active Directory
- AD is the source of truth for core identity attributes
- A local user profile is maintained for system operations and history

---

## 4.2 Role

Represents a system role.

### Examples
- System Administrator
- Policy Manager
- Acknowledgment Manager
- Publisher
- Compliance Viewer
- Auditor
- End User

### Key Attributes
- RoleId
- RoleName
- Description
- IsSystemRole

---

## 4.3 Scope

Represents the organizational or business scope attached to a role assignment.

### Examples
- Global
- Department Scope
- Owned Content Scope

### Key Attributes
- ScopeId
- ScopeType
- ScopeReference
- Description

---

## 4.4 UserRoleAssignment

Represents the mapping between a user, a role, and a scope.

### Key Attributes
- UserRoleAssignmentId
- UserId
- RoleId
- ScopeId
- EffectiveFrom
- EffectiveTo
- IsActive

### Relationship Summary
- one User can have many UserRoleAssignments
- one Role can be assigned to many users
- one Scope can be assigned to many role assignments

---

## 5. Policy Management Entities

## 5.1 Policy

Represents the master policy object.

### Key Attributes
- PolicyId
- PolicyCode
- PolicyTitle
- OwnerDepartment
- Category
- Status
- CreatedAt
- CreatedBy

### Notes
- A policy may have many versions
- The policy itself is the long-lived parent object

---

## 5.2 PolicyVersion

Represents a versioned business snapshot of a policy.

### Key Attributes
- PolicyVersionId
- PolicyId
- VersionNumber
- VersionLabel
- EffectiveDate
- PublishedAt
- Status
- CreatedAt
- CreatedBy
- SupersededByPolicyVersionId (optional)

### Notes
- Only one version should be active/published at a time per policy
- Published versions are immutable

### Relationship Summary
- one Policy has many PolicyVersions
- one PolicyVersion belongs to one Policy

---

## 5.3 PolicyDocument

Represents the uploaded file associated with a policy version.

### Key Attributes
- PolicyDocumentId
- PolicyVersionId
- FileName
- FileType
- FilePath / FileReference
- FileSize
- UploadedAt
- UploadedBy

### Relationship Summary
- one PolicyVersion may have one or more documents
- in Phase 1, a single primary PDF is expected

---

## 6. Acknowledgment & Disclosure Entities

## 6.1 AcknowledgmentDefinition

Represents the master business definition of an acknowledgment or disclosure action.

### Key Attributes
- AcknowledgmentDefinitionId
- Title
- Description
- OwnerDepartment
- ActionType
- Status
- CreatedAt
- CreatedBy

### Supported Action Types
- Simple Acknowledgment
- Acknowledgment with Commitment
- Form-Based Disclosure

### Relationship Summary
- one AcknowledgmentDefinition has many AcknowledgmentVersions

---

## 6.2 AcknowledgmentVersion

Represents a specific version of an acknowledgment/disclosure definition.

### Key Attributes
- AcknowledgmentVersionId
- AcknowledgmentDefinitionId
- VersionNumber
- VersionLabel
- PolicyVersionId
- ActionType
- RecurrenceModel
- StartDate
- DueDate
- Status
- PublishedAt
- CreatedAt
- CreatedBy
- SupersededByAcknowledgmentVersionId (optional)

### Notes
- This entity is the actual executable business version
- It defines what users must complete
- It is linked to the exact policy version
- If the form changes materially, a new acknowledgment version is created

### Relationship Summary
- one AcknowledgmentDefinition has many AcknowledgmentVersions
- one AcknowledgmentVersion belongs to one AcknowledgmentDefinition
- one AcknowledgmentVersion references one PolicyVersion

---

## 6.3 FormDefinition

Represents the structured form definition associated with a form-based acknowledgment version.

### Key Attributes
- FormDefinitionId
- AcknowledgmentVersionId
- FormSchemaJson
- SchemaVersion
- IsActive
- CreatedAt
- CreatedBy

### Notes
- only used when ActionType = Form-Based Disclosure
- stores the form schema as a controlled JSON structure
- should be version-bound, not shared across active versions without control

### Relationship Summary
- one AcknowledgmentVersion may have one FormDefinition
- one FormDefinition belongs to one AcknowledgmentVersion

---

## 7. Targeting & Assignment Entities

## 7.1 AudienceDefinition

Represents the target audience configuration for an acknowledgment version.

### Key Attributes
- AudienceDefinitionId
- AcknowledgmentVersionId
- AudienceType
- CreatedAt
- CreatedBy

### Examples of AudienceType
- All Users
- Department-Based
- AD Group-Based
- Mixed Rules

### Relationship Summary
- one AcknowledgmentVersion may have one AudienceDefinition
- one AudienceDefinition may have many AudienceRules

---

## 7.2 AudienceRule

Represents an individual targeting rule.

### Key Attributes
- AudienceRuleId
- AudienceDefinitionId
- RuleType
- RuleOperator
- RuleValue
- IsExclusion
- SortOrder

### Examples
- Department = HR
- AD Group = EAP_GRC_USERS
- Exclude User = 12345

### Relationship Summary
- one AudienceDefinition has many AudienceRules

---

## 7.3 UserActionRequirement

Represents the required action assigned to a specific user for a specific acknowledgment version and cycle.

### Key Attributes
- UserActionRequirementId
- UserId
- AcknowledgmentVersionId
- CycleReference
- RecurrenceInstanceDate
- Status
- AssignedAt
- DueDate
- CompletedAt
- IsCurrent

### Supported Statuses
- Pending
- Completed
- Overdue
- Cancelled

### Notes
- this is the bridge between action definitions and real user obligations
- one user may receive the same acknowledgment definition multiple times across cycles
- must remain version-aware

### Relationship Summary
- one User has many UserActionRequirements
- one AcknowledgmentVersion has many UserActionRequirements

---

## 8. Submission & Compliance Entities

## 8.1 UserSubmission

Represents the actual submission made by the user.

### Key Attributes
- UserSubmissionId
- UserId
- UserActionRequirementId
- AcknowledgmentVersionId
- SubmissionType
- SubmittedAt
- Status
- SubmissionJson
- FormDefinitionSnapshotJson (optional but recommended)
- SubmittedBy
- IsLateSubmission

### Notes
- supports both simple acknowledgments and form-based disclosures
- SubmissionJson stores the business payload
- FormDefinitionSnapshotJson protects historical integrity

### Relationship Summary
- one UserActionRequirement may result in one or more submissions depending on business rules
- in MVP, normally one valid submission per requirement is expected

---

## 8.2 UserSubmissionFieldValue

Represents optional flattened storage of individual submitted form values.

### Key Attributes
- UserSubmissionFieldValueId
- UserSubmissionId
- FieldName
- FieldLabel
- FieldType
- ValueText
- ValueNumber
- ValueDate
- ValueBoolean
- ValueJson

### Notes
- useful when reporting/searching individual form values is required
- may be optional in MVP if SubmissionJson is sufficient

### Relationship Summary
- one UserSubmission may have many UserSubmissionFieldValues

---

## 8.3 ComplianceSnapshot

Represents stored or derived compliance summary data.

### Key Attributes
- ComplianceSnapshotId
- SnapshotDate
- ScopeType
- ScopeReference
- PolicyId
- AcknowledgmentDefinitionId
- TotalAssigned
- TotalCompleted
- TotalOverdue
- CompletionPercentage

### Notes
- may be persisted or derived depending on implementation approach
- used for reporting and dashboards

---

## 9. Notifications & Audit Entities

## 9.1 Notification

Represents a logical notification created by the system.

### Key Attributes
- NotificationId
- UserId
- NotificationType
- RelatedEntityType
- RelatedEntityId
- Status
- CreatedAt
- ScheduledAt
- SentAt

### Notification Types
- Assignment
- Reminder
- Overdue

### Relationship Summary
- one User may have many Notifications

---

## 9.2 NotificationAttempt

Represents one attempt to send a notification.

### Key Attributes
- NotificationAttemptId
- NotificationId
- AttemptNumber
- AttemptedAt
- ResultStatus
- FailureReason

### Relationship Summary
- one Notification may have many NotificationAttempts

---

## 9.3 AuditLog

Represents a business/audit event.

### Key Attributes
- AuditLogId
- ActorUserId
- ActionType
- EntityType
- EntityId
- EntityVersionId
- ActionTimestamp
- Description
- BeforeSnapshotJson (optional)
- AfterSnapshotJson (optional)

### Notes
- immutable
- separate from application log stream
- supports governance and audit review

---

## 10. Entity Relationships Summary

## 10.1 Identity Relationships

- User 1 → many UserRoleAssignments
- Role 1 → many UserRoleAssignments
- Scope 1 → many UserRoleAssignments

## 10.2 Policy Relationships

- Policy 1 → many PolicyVersions
- PolicyVersion 1 → many PolicyDocuments

## 10.3 Acknowledgment Relationships

- AcknowledgmentDefinition 1 → many AcknowledgmentVersions
- PolicyVersion 1 → many AcknowledgmentVersions
- AcknowledgmentVersion 0..1 → one FormDefinition
- AcknowledgmentVersion 0..1 → one AudienceDefinition

## 10.4 Targeting Relationships

- AudienceDefinition 1 → many AudienceRules
- User 1 → many UserActionRequirements
- AcknowledgmentVersion 1 → many UserActionRequirements

## 10.5 Submission Relationships

- UserActionRequirement 1 → many UserSubmissions
- UserSubmission 1 → many UserSubmissionFieldValues

## 10.6 Operational Relationships

- User 1 → many Notifications
- Notification 1 → many NotificationAttempts
- User / System Actions → many AuditLogs

---

## 11. Versioning Model

Versioning is a core structural rule in the model.

### 11.1 Policy Versioning
Each policy may have multiple versions over time.
Only one version may be published at a time.

### 11.2 Acknowledgment Versioning
Each acknowledgment definition may have multiple versions over time.
Each version points to one policy version.

### 11.3 Form Versioning
Form definitions belong to acknowledgment versions.
If the form changes materially, a new acknowledgment version is required.

### 11.4 Submission Traceability
Each submission must point to:
- the user
- the user requirement
- the acknowledgment version
- implicitly the policy version through the acknowledgment version

This ensures full historical traceability.

---

## 12. Recurrence Model

Recurring actions are modeled through UserActionRequirement and cycle metadata.

### Examples

#### Onboarding Only
- one requirement generated once when the employee joins

#### Annual
- one requirement generated for each annual cycle

#### Onboarding + Annual
- one onboarding requirement
- then annual cycle requirements

#### On Change
- new requirement generated when change event is declared

#### Event-Driven
- requirement created only when a specific user event exists

---

## 13. Form Modeling Strategy

The conceptual approach for form-based disclosures is:

- store form definition as structured JSON at the AcknowledgmentVersion level
- render dynamically in frontend
- validate in backend
- store submission payload as structured submission data
- optionally flatten field values for query/reporting

This approach balances:
- flexibility
- speed of implementation
- historical integrity
- reduced need for a visual form builder

---

## 14. Data Ownership Principles

### 14.1 Identity Data
Owned by LDAP / Active Directory as source of truth.

### 14.2 Policy Data
Owned by the policy owner department through authorized users.

### 14.3 Acknowledgment Data
Owned by the acknowledgment owner department.

### 14.4 User Submission Data
Owned by the platform as official business record, based on user input.

### 14.5 Audit Data
Owned by the platform as immutable governance evidence.

---

## 15. Conceptual Integrity Rules

The conceptual data model must maintain the following integrity rules:

### CDM-001
No acknowledgment version may exist without a linked policy version.

### CDM-002
No user submission may exist without an acknowledgment version.

### CDM-003
No form definition may be shared loosely across unrelated live versions without version control.

### CDM-004
Historical submissions must remain queryable even after a policy or acknowledgment version is superseded.

### CDM-005
Department is an attribute of the user and a business ownership reference, not a permission by itself.

### CDM-006
Role and scope must be modeled independently of department names.

---

## 16. Summary

The conceptual data model establishes a version-aware, user-centered, policy-linked structure for the platform.

It is designed to support:

- policy management
- acknowledgment and disclosure management
- recurring user obligations
- form-based submissions
- compliance tracking
- notifications
- auditability

while preserving:
- historical integrity
- business traceability
- scalability for future enhancements
