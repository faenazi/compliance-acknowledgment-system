# Business Rules

## 1. Purpose

This document defines the mandatory business rules that govern the behavior of the Enterprise Acknowledgment Platform (EAP).

These rules represent non-negotiable business constraints and must be enforced by the system.

---

## 2. General Rules

### BR-001
The platform shall support policy-based actions only.
Every acknowledgment or disclosure must be linked to a policy.

### BR-002
Every policy-linked action must be associated with a specific policy version.

### BR-003
A published policy version is immutable and must not be modified in place.

### BR-004
A published acknowledgment or disclosure version is immutable and must not be modified in place.

### BR-005
Any material change to published content must create a new version.

---

## 3. Policy Rules

### BR-010
A policy cannot be published unless a policy document is attached.

### BR-011
Only one active published version is allowed per policy at a time.

### BR-012
Archived policy versions must remain available for historical reference.

### BR-013
A policy may exist in draft state before publication.

### BR-014
A policy must have an owning department.

### BR-015
A policy must have at least one supported business action before being considered operationally active in the platform.

---

## 4. Action Type Rules

### BR-020
The system shall support the following action types:
- Simple Acknowledgment
- Acknowledgment with Commitment
- Form-Based Disclosure

### BR-021
A simple acknowledgment requires user confirmation only.

### BR-022
A form-based disclosure requires structured user input before submission.

### BR-023
A form-based disclosure must not be treated as a simple acknowledgment.

### BR-024
The action type must be defined before publishing the action.

---

## 5. Acknowledgment & Disclosure Rules

### BR-030
Every acknowledgment or disclosure must be linked to:
- a policy
- a specific policy version

### BR-031
Every acknowledgment or disclosure must have:
- title
- owner department
- action type
- target audience
- recurrence model
- effective start date

### BR-032
An action cannot be published unless its target audience is defined.

### BR-033
An action cannot be published unless its recurrence model is defined.

### BR-034
An action may have a due date when business requires a completion deadline.

### BR-035
An action may be archived, but archived actions must remain available historically.

### BR-036
A user must not be allowed to submit the same acknowledgment twice for the same version unless the business rule explicitly requires resubmission.

---

## 6. Recurrence Rules

### BR-040
The system shall support the following recurrence models:
- Onboarding Only
- Annual
- Onboarding + Annual
- On Change
- Event-Driven

### BR-041
Onboarding Only actions are required once per employee upon joining.

### BR-042
Annual actions must be re-triggered every year according to the configured recurrence cycle.

### BR-043
Onboarding + Annual actions must be triggered at onboarding and then re-triggered annually.

### BR-044
On Change actions must be re-submitted whenever the user declares that a relevant change has occurred.

### BR-045
Event-Driven actions must be submitted only when the corresponding event occurs.

### BR-046
A recurrence model must be defined at the action level.

### BR-047
If a new version is published for an annually recurring action, the system must evaluate whether users must act on the new version based on the configured business policy.

---

## 7. Audience Targeting Rules

### BR-050
The system shall support targeting by:
- all users
- department
- AD group
- explicit exclusions

### BR-051
Target audience resolution must rely on current LDAP / Active Directory attributes.

### BR-052
A user must only see actions assigned to their resolved audience.

### BR-053
Explicit exclusions must override inclusion rules.

### BR-054
Department must be treated as an organizational attribute, not as a permission by itself.

### BR-055
If a user changes department or group membership, future targeting must be evaluated based on the updated AD data.

---

## 8. Identity & User Rules

### BR-060
Authentication must be performed through LDAP / Active Directory.

### BR-061
AD is the source of truth for:
- username
- display name
- email
- department
- job title
- group memberships

### BR-062
The system shall create a local user profile upon first successful login.

### BR-063
The system shall update cached user profile attributes based on AD on login or synchronization.

### BR-064
A user cannot submit an acknowledgment unless authenticated.

---

## 9. Form-Based Disclosure Rules

### BR-070
A form-based disclosure must have a form definition attached to the acknowledgment version.

### BR-071
The form definition is part of the acknowledgment version and must be version-controlled with it.

### BR-072
The first release shall support controlled structured fields without a full visual form builder.

### BR-073
Supported field types in Phase 1 are:
- Short Text
- Long Text / Text Area
- Number
- Decimal
- Date
- Checkbox
- Radio Group
- Dropdown / Select
- Multi Select
- Yes / No
- Email
- Phone Number
- File Upload
- Read-Only Display Field
- Section Header / Static Text

### BR-074
A field marked as required must be completed before submission.

### BR-075
A field option list must be respected when the field type is Select, Radio, or Multi Select.

### BR-076
File upload fields must follow allowed file type and size constraints defined by the platform.

### BR-077
The system must reject invalid form submissions.

### BR-078
The system must store the submitted disclosure values in a structured format linked to the user acknowledgment record.

### BR-079
The system should preserve the form definition snapshot used at the time of submission.

---

## 10. Form Versioning Rules

### BR-080
Any material change to a form definition requires a new acknowledgment version.

### BR-081
Material changes include:
- adding a field
- removing a field
- changing field type
- changing required/optional behavior
- changing field options
- changing the business meaning of the form

### BR-082
Previously submitted disclosures must remain linked to the version that was active at submission time.

### BR-083
New submissions must use the latest published applicable version only.

---

## 11. Submission Rules

### BR-090
A user submission must record at minimum:
- user id
- action id
- action version id
- submitted date and time
- status

### BR-091
For form-based disclosures, the system must also record the submitted field values.

### BR-092
A completed submission must not be editable by the end user unless the business rule explicitly allows re-submission.

### BR-093
If a recurrence model requires renewal, a previous completed submission does not satisfy the new required cycle.

### BR-094
If an action is event-driven, the existence of no event means no submission is required.

### BR-095
If an action is change-triggered, the user must be able to submit an updated disclosure when a relevant change occurs.

---

## 12. Status Rules

### BR-100
The system shall support at minimum the following user action statuses in Phase 1:
- Pending
- Completed
- Overdue

### BR-101
A status becomes Pending when the action is assigned to the user and has not yet been completed.

### BR-102
A status becomes Completed when a valid submission is successfully recorded.

### BR-103
A status becomes Overdue when the due date passes without valid completion.

### BR-104
An action without a due date cannot become Overdue.

---

## 13. Notification Rules

### BR-110
The system shall send email notifications via Exchange.

### BR-111
Notifications may include:
- new action assignment
- reminder before due date
- overdue alert

### BR-112
A notification failure must be logged.

### BR-113
Notification delivery status must not alter the user submission status.

### BR-114
Reminder schedules must follow the configured recurrence or due-date behavior.

---

## 14. Compliance Rules

### BR-120
Compliance must be measurable per:
- user
- department
- action
- policy

### BR-121
A user is compliant for an action only if the required submission for the current applicable cycle/version has been completed.

### BR-122
A user with Pending or Overdue status is considered non-compliant for that action.

### BR-123
Compliance reporting must reflect the latest applicable status.

---

## 15. Audit Rules

### BR-130
The system must record audit logs for critical actions.

### BR-131
Critical actions include at minimum:
- policy creation
- policy publication
- action creation
- action publication
- user submission
- administrative updates to active business objects

### BR-132
Audit logs must include:
- actor
- action
- timestamp
- target entity
- target version where applicable

### BR-133
Audit logs must not be editable through standard business interfaces.

---

## 16. Access Control Rules

### BR-140
The system shall use Role-Based Access Control with scope.

### BR-141
Permissions must not be modeled directly as department names.

### BR-142
Access must be determined by:
- role
- permission set
- organizational scope

### BR-143
The system shall support roles such as:
- System Administrator
- Policy Manager
- Acknowledgment Manager
- Publisher
- Compliance Viewer
- Auditor
- End User

### BR-144
The system shall support scopes such as:
- Global
- Department Scope
- Owned Content Scope

### BR-145
A user must not perform actions outside their assigned scope.

---

## 17. Business Ownership Rules

### BR-150
Every policy must have a business owner.

### BR-151
Every acknowledgment or disclosure must have an owner department.

### BR-152
Human Capital and GRC are initial primary business owners, but the model must support future departments.

---

## 18. Phase 1 Constraints

### BR-160
The first release shall not include blocking or forced enforcement.

### BR-161
The first release shall not include a full visual form builder.

### BR-162
The first release shall not include exception management workflows.

### BR-163
The first release shall not include defer workflows.

### BR-164
The first release shall not include advanced conditional field logic.

---

## 19. Data Retention & Historical Integrity Rules

### BR-170
Historical submissions must remain accessible for governance and audit purposes.

### BR-171
Historical records must not be overwritten by later versions.

### BR-172
Archived policies and archived actions must remain available for authorized historical reference.

---

## 20. Priority Business Scenarios

### BR-180
The platform must support at minimum the following business scenarios in Phase 1:
- policy acknowledgment on onboarding
- annual conflict of interest disclosure
- disclosure update when user circumstances change
- gifts and hospitality disclosure when an event occurs
- acceptable use policy acknowledgment
- code of conduct acknowledgment
