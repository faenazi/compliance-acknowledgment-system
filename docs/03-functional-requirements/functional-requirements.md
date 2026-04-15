# Functional Requirements

## 1. Purpose

This document defines the functional requirements for the Enterprise Acknowledgment Platform (EAP).

It translates the approved business requirements into system capabilities that can be implemented by design and development teams.

This document covers:

- policy management
- acknowledgment and disclosure management
- user portal
- admin portal
- audience targeting
- recurring actions
- notifications
- reporting
- audit-related functionality

---

## 2. Functional Scope

The platform shall support the following functional areas in Phase 1:

1. Identity and user management
2. Policy management
3. Acknowledgment and disclosure management
4. Form-based disclosure support
5. Audience targeting
6. User portal
7. Admin portal
8. Compliance tracking
9. Notifications
10. Reporting
11. Audit logging

---

## 3. Identity & User Management

### FR-001 Authentication
The system shall authenticate users using LDAP / Active Directory.

### FR-002 User Login
The system shall allow users to log in using their corporate credentials.

### FR-003 User Profile Creation
The system shall create a local user profile upon first successful login.

### FR-004 User Profile Synchronization
The system shall synchronize user profile attributes from Active Directory during login and/or scheduled sync.

### FR-005 User Attributes
The system shall retrieve at minimum the following user attributes from AD:
- username
- full name
- email
- department
- job title
- group memberships

### FR-006 User Access Context
The system shall use AD-derived organizational attributes for:
- audience targeting
- reporting context
- scoped access decisions

---

## 4. Policy Management

### FR-010 Create Policy
The system shall allow authorized users to create a policy record.

### FR-011 Policy Metadata
The system shall allow authorized users to define policy metadata, including:
- title
- description
- owner department
- effective date

### FR-012 Upload Policy Document
The system shall allow authorized users to upload a policy document.

### FR-013 Supported Document Format
The system shall support PDF as the minimum required policy document format in Phase 1.

### FR-014 Policy Versioning
The system shall support multiple versions of a policy.

### FR-015 Publish Policy Version
The system shall allow authorized users to publish a policy version.

### FR-016 Archive Policy Version
The system shall allow authorized users to archive historical policy versions.

### FR-017 View Historical Versions
The system shall allow authorized users to view historical policy versions for reference.

---

## 5. Acknowledgment & Disclosure Management

### FR-020 Create Acknowledgment
The system shall allow authorized users to create an acknowledgment or disclosure definition.

### FR-021 Link to Policy Version
The system shall require each acknowledgment or disclosure to be linked to a specific policy version.

### FR-022 Action Types
The system shall support the following action types:
- Simple Acknowledgment
- Acknowledgment with Commitment
- Form-Based Disclosure

### FR-023 Define Action Metadata
The system shall allow authorized users to define action metadata, including:
- title
- description
- owner department
- recurrence model
- start date
- due date, if applicable

### FR-024 Publish Acknowledgment Version
The system shall allow authorized users to publish an acknowledgment/disclosure version.

### FR-025 Archive Acknowledgment Version
The system shall allow authorized users to archive historical acknowledgment/disclosure versions.

### FR-026 View Historical Acknowledgment Versions
The system shall allow authorized users to view historical versions of acknowledgments and disclosures.

---

## 6. Form-Based Disclosure Support

### FR-030 Support Form-Based Actions
The system shall support form-based acknowledgments and disclosures.

### FR-031 Form Definition Storage
The system shall store the form definition at the acknowledgment version level.

### FR-032 Dynamic Form Rendering
The system shall render form-based disclosures dynamically in the user portal based on the configured definition.

### FR-033 Supported Field Types
The system shall support the following field types in Phase 1:
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

### FR-034 Required Fields
The system shall support required and optional fields.

### FR-035 Selectable Options
The system shall support predefined options for:
- radio group
- dropdown
- multi select

### FR-036 Field Ordering
The system shall support ordering of fields and sections within a form-based disclosure.

### FR-037 Field Help Text
The system shall support help text and descriptive labels for fields.

### FR-038 File Upload Support
The system shall support file uploads in applicable form-based disclosures, subject to configured validation constraints.

### FR-039 Form Validation
The system shall validate submitted form values before acceptance.

### FR-040 Submission Storage
The system shall store submitted field values in a structured form linked to the user acknowledgment record.

### FR-041 Version-Aware Forms
The system shall ensure that each submitted disclosure remains linked to the exact acknowledgment version used at submission time.

---

## 7. Audience Targeting

### FR-050 Target All Users
The system shall allow an action to be assigned to all users.

### FR-051 Target by Department
The system shall allow an action to be assigned by department.

### FR-052 Target by AD Group
The system shall allow an action to be assigned by Active Directory group.

### FR-053 Explicit Exclusions
The system shall allow explicitly excluding individual users from a target audience.

### FR-054 Audience Resolution
The system shall resolve target audience membership using current AD-derived attributes.

### FR-055 Preview Audience
The system should allow authorized users to view or estimate the target audience before publishing.

---

## 8. Recurrence & Scheduling

### FR-060 Supported Recurrence Models
The system shall support the following recurrence models:
- Onboarding Only
- Annual
- Onboarding + Annual
- On Change
- Event-Driven

### FR-061 Onboarding Assignment
The system shall support assigning onboarding actions to newly joined employees.

### FR-062 Annual Renewal
The system shall support annual recurrence for selected actions.

### FR-063 Onboarding + Annual
The system shall support actions that occur once at onboarding and then recur annually.

### FR-064 On Change
The system shall support actions that must be resubmitted when the user declares a relevant change.

### FR-065 Event-Driven
The system shall support actions that are submitted when a relevant event occurs.

### FR-066 Start Date
The system shall support a configured start date for each published action.

### FR-067 Due Date
The system shall support a configured due date where required by business.

---

## 9. User Portal

### FR-070 Dashboard
The system shall provide a user dashboard.

### FR-071 Dashboard Sections
The dashboard shall display at minimum:
- pending actions
- completed actions
- overdue actions

### FR-072 View Action Details
The user shall be able to open the details of an assigned acknowledgment or disclosure.

### FR-073 View Linked Policy
The user shall be able to view the linked policy document from the action details screen.

### FR-074 Submit Simple Acknowledgment
The user shall be able to submit a simple acknowledgment.

### FR-075 Submit Commitment Acknowledgment
The user shall be able to submit an acknowledgment that includes commitment text.

### FR-076 Submit Form-Based Disclosure
The user shall be able to complete and submit a form-based disclosure.

### FR-077 View Personal History
The user shall be able to view their own history of submitted acknowledgments and disclosures.

### FR-078 View Status
The user shall be able to view the current status of each assigned action.

### FR-079 Status Visibility
The user portal shall visually distinguish:
- Pending
- Completed
- Overdue

### FR-080 No Enforcement in Phase 1
The system shall not block the user from accessing the platform due to incomplete actions in Phase 1.

---

## 10. Admin Portal

### FR-090 Manage Policies
Authorized users shall be able to create, edit draft, publish, archive, and view policy records and versions.

### FR-091 Manage Acknowledgments
Authorized users shall be able to create, edit draft, publish, archive, and view acknowledgment/disclosure definitions and versions.

### FR-092 Define Recurrence
Authorized users shall be able to configure recurrence model and dates.

### FR-093 Define Audience
Authorized users shall be able to configure audience targeting for each action.

### FR-094 Configure Form-Based Disclosure
Authorized users shall be able to define a controlled structured form for form-based actions.

### FR-095 View Compliance Data
Authorized users shall be able to access compliance-related views and reports according to role and scope.

### FR-096 Access Historical Records
Authorized users shall be able to access historical policy, action, and submission records according to role and scope.

---

## 11. Submission Processing

### FR-100 Record Submission
The system shall record each valid submission against:
- user
- action
- action version
- date/time
- status

### FR-101 Prevent Duplicate Submission
The system shall prevent duplicate submission for the same user and same action version unless the business flow explicitly requires resubmission.

### FR-102 Mark Completion
A successful submission shall update the corresponding user requirement status to Completed.

### FR-103 Mark Overdue
If a due date exists and a submission is not completed on time, the system shall mark the requirement as Overdue.

### FR-104 Late Completion
The system shall allow late completion unless restricted by future business rules.

---

## 12. Compliance Tracking

### FR-110 Track User Status
The system shall track status per user per action.

### FR-111 Compliance Metrics
The system shall calculate compliance metrics by:
- user
- department
- policy
- acknowledgment/disclosure

### FR-112 Identify Non-Compliant Users
The system shall identify users with Pending or Overdue status for required actions.

### FR-113 Department-Level View
The system shall provide department-level compliance visibility.

---

## 13. Notifications

### FR-120 Send Assignment Notification
The system shall send an email when a new action becomes applicable to a user.

### FR-121 Send Reminder Notification
The system shall send reminder notifications before due date where applicable.

### FR-122 Send Overdue Notification
The system shall send overdue notifications after due date where applicable.

### FR-123 Exchange Integration
The system shall send notifications through Exchange integration.

### FR-124 Log Delivery Outcome
The system shall log notification delivery outcome for operational traceability.

---

## 14. Reporting

### FR-130 Compliance Report
The system shall provide a compliance report.

### FR-131 Non-Compliant Users Report
The system shall provide a report of non-compliant users.

### FR-132 Completion Report by Action
The system shall provide a completion report by acknowledgment/disclosure.

### FR-133 Completion Report by Department
The system shall provide a completion report by department.

### FR-134 User History Report
The system shall provide a report of user submission history for authorized roles.

### FR-135 Export
The system shall support export to:
- Excel
- PDF

---

## 15. Audit Logging

### FR-140 Administrative Audit
The system shall log critical administrative actions.

### FR-141 User Submission Audit
The system shall log user submission events.

### FR-142 Audit Data
The audit log shall preserve:
- actor
- action
- timestamp
- target entity
- target version where applicable

### FR-143 Historical Integrity
The system shall preserve historical relationships between policies, versions, actions, and submissions.

---

## 16. Access Control

### FR-150 Role-Based Access Control
The system shall implement role-based access control.

### FR-151 Scoped Access
The system shall support access scope such as:
- Global
- Department Scope
- Owned Content Scope

### FR-152 Role Support
The system shall support at minimum the following roles:
- System Administrator
- Policy Manager
- Acknowledgment Manager
- Publisher
- Compliance Viewer
- Auditor
- End User

### FR-153 Access Restrictions
Users shall only be allowed to perform actions permitted by their role and scope.

---

## 17. Phase 1 Priority Scenarios

### FR-160 Conflict of Interest
The platform shall support:
- policy acknowledgment
- form-based disclosure
- annual recurrence
- update on change

### FR-161 Gifts and Hospitality
The platform shall support:
- policy acknowledgment
- event-driven disclosure submission

### FR-162 Acceptable Use Policy
The platform shall support onboarding-only acknowledgment for acceptable use.

### FR-163 HR Policy Acknowledgments
The platform shall support onboarding-only policy acknowledgments for HR-related policies.

---

## 18. Phase 1 Exclusions

### FR-170
The system shall not include blocking or forced enforcement in Phase 1.

### FR-171
The system shall not include a full visual form builder in Phase 1.

### FR-172
The system shall not include exception workflows in Phase 1.

### FR-173
The system shall not include defer workflows in Phase 1.

### FR-174
The system shall not include advanced approval workflows in Phase 1.

### FR-175
The system shall not include advanced conditional form logic in Phase 1.
