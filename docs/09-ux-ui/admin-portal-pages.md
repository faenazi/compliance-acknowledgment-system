# Admin Portal Pages

## 1. Purpose

This document defines the pages, page objectives, and UX expectations for the Admin Portal of the Enterprise Acknowledgment Platform (EAP).

The Admin Portal is designed for authorized users such as:

- Policy Managers
- Acknowledgment Managers
- Publishers
- Compliance Viewers
- Auditors
- System Administrators

It supports the management and monitoring of:

- policies
- policy versions
- acknowledgments and disclosures
- form-based actions
- audience targeting
- recurrence configuration
- compliance reporting
- audit and history

This document defines:
- required pages
- page purpose
- key content blocks
- main user actions
- UX notes for implementation

---

## 2. Admin Portal Design Goals

The Admin Portal must be:

- structured
- professional
- operationally efficient
- easy to scan
- role-aware
- consistent
- Arabic-first
- suitable for internal enterprise administration

The Admin Portal should prioritize:
- efficient management
- visibility of status
- low-friction operational workflows
- clear data structures
- strong hierarchy for tables, filters, and forms

---

## 3. Admin Portal Information Architecture

The Admin Portal should include the following primary sections:

1. Admin Dashboard
2. Policy Management
3. Policy Version Management
4. Acknowledgment Management
5. Acknowledgment Version Management
6. Form Definition Management
7. Audience Targeting Management
8. User Requirements / Assignment Monitoring
9. Compliance Reporting
10. Audit & History
11. Role / Access Context
12. Settings / Operational Configuration (limited in Phase 1)

---

## 4. Page 1: Admin Dashboard

### Page Name
Admin Dashboard

### Purpose
Provide an operational summary for authorized users.

### Main Content Blocks
- Page header
- Summary KPI cards
- Compliance overview
- Pending operational items
- Overdue action summary
- Recently published policies/actions
- Quick links

### Suggested KPI Cards
- Active Policies
- Active Acknowledgments
- Pending User Actions
- Overdue User Actions
- Annual Actions Due
- Completion Rate

### Main User Actions
- Go to policy management
- Go to acknowledgment management
- Open compliance report
- Open overdue actions view

### UX Notes
- Keep this page operational, not decorative
- Use charts sparingly and only where helpful
- The dashboard should support rapid decision-making

---

## 5. Page 2: Policy List

### Page Name
Policy Management

### Purpose
Allow authorized users to view and manage policy records.

### Main Content Blocks
- Page header
- Search/filter toolbar
- Policy table

### Suggested Filters
- Status
- Owner Department
- Category
- Has Published Version
- Date Range

### Suggested Columns
- Policy Code
- Policy Title
- Owner Department
- Current Status
- Active Version
- Last Updated
- Actions

### Main User Actions
- Create policy
- Open policy details
- Search policies
- Filter policies

### UX Notes
- This page should be table-first
- Primary action should be “Create Policy”
- Published vs draft distinction must be obvious

---

## 6. Page 3: Create / Edit Policy

### Page Name
Policy Editor

### Purpose
Allow authorized users to create and edit policy master records and draft metadata.

### Main Content Blocks
- Basic information form
- Owner department
- Description
- Effective date
- Category / tags if used

### Main User Actions
- Save draft
- Cancel
- Go to versions

### UX Notes
- Separate policy metadata from policy version content where possible
- Avoid mixing version management into the same screen if it reduces clarity

---

## 7. Page 4: Policy Versions List

### Page Name
Policy Versions

### Purpose
Allow authorized users to manage policy versions.

### Main Content Blocks
- Policy context header
- Version table / timeline
- Active version indicator

### Suggested Columns
- Version Number
- Version Label
- Status
- Effective Date
- Published Date
- Created By
- Actions

### Main User Actions
- Create version
- Open version details
- Publish version
- Archive version
- View historical version

### UX Notes
- Active version must be visually distinct
- Historical versions must remain easy to inspect
- Publishing action should be clearly separated from editing

---

## 8. Page 5: Policy Version Details

### Page Name
Policy Version Details

### Purpose
Allow authorized users to manage one draft version or inspect one historical version.

### Main Content Blocks
- Version metadata
- Document section
- Status info
- Publish/archive actions

### Main User Actions
- Upload or replace draft document
- Save draft
- Publish version
- Archive version
- View linked acknowledgments

### UX Notes
- Published versions should become read-only in business terms
- Historical versions should be clearly labeled as non-editable

---

## 9. Page 6: Acknowledgment List

### Page Name
Acknowledgment Management

### Purpose
Allow authorized users to view and manage acknowledgment/disclosure definitions.

### Main Content Blocks
- Search/filter toolbar
- Acknowledgment table

### Suggested Filters
- Status
- Action Type
- Owner Department
- Recurrence Model
- Linked Policy
- Date Range

### Suggested Columns
- Title
- Action Type
- Owner Department
- Current Status
- Active Version
- Recurrence Model
- Last Updated
- Actions

### Main User Actions
- Create acknowledgment
- Open acknowledgment details
- Search/filter

### UX Notes
- Action type should be visible at list level
- Form-based disclosures should be easily distinguishable from simple acknowledgments

---

## 10. Page 7: Create / Edit Acknowledgment

### Page Name
Acknowledgment Editor

### Purpose
Allow authorized users to create and manage the master definition of an acknowledgment/disclosure.

### Main Content Blocks
- Basic information form
- Description
- Owner department
- Action type

### Main User Actions
- Save draft
- Cancel
- Go to versions

### UX Notes
- This page defines the business object, not the versioned execution details
- Keep it simple and metadata-focused

---

## 11. Page 8: Acknowledgment Versions List

### Page Name
Acknowledgment Versions

### Purpose
Allow authorized users to manage versions of acknowledgments/disclosures.

### Main Content Blocks
- Acknowledgment context header
- Version table / timeline
- Active version indicator

### Suggested Columns
- Version Number
- Status
- Linked Policy Version
- Action Type
- Recurrence Model
- Start Date
- Due Date
- Published Date
- Actions

### Main User Actions
- Create version
- Open version details
- Publish version
- Archive version
- View historical version

### UX Notes
- This page is a core admin page and must be very clear
- Version data and recurrence data should be visible without opening every record

---

## 12. Page 9: Acknowledgment Version Details

### Page Name
Acknowledgment Version Details

### Purpose
Allow authorized users to configure a specific version of an acknowledgment/disclosure.

### Main Content Blocks
- Version metadata
- Linked policy version
- Action type
- Recurrence model
- Dates
- Audience summary
- Form configuration entry point if applicable
- Publish/archive controls

### Main User Actions
- Edit draft version
- Configure recurrence
- Configure audience
- Configure form definition
- Publish version
- Archive version

### UX Notes
- This is the main orchestration page for one action version
- It should clearly show whether the version is:
  - simple acknowledgment
  - commitment acknowledgment
  - form-based disclosure

---

## 13. Page 10: Form Definition Management

### Page Name
Form Definition

### Purpose
Allow authorized users to define and manage structured form fields for form-based disclosures.

### Main Content Blocks
- Form context header
- Section list
- Field definition list
- Field editor panel or inline edit area

### Supported Operations
- Add field
- Edit field
- Delete field from draft
- Reorder fields
- Define required/optional
- Define options for select/radio/multi select
- Add section header / static text

### Suggested Field Properties
- Field Label
- Field Name / Key
- Field Type
- Required
- Help Text
- Placeholder
- Options
- Order

### Main User Actions
- Add field
- Save form definition
- Preview disclosure form
- Return to version details

### UX Notes
- This is not a visual drag-and-drop builder in Phase 1
- The UX should be controlled and structured
- Reordering should be simple and not overly interactive
- Long or complex forms must remain manageable

---

## 14. Page 11: Audience Targeting Management

### Page Name
Audience Targeting

### Purpose
Allow authorized users to define who should receive the action.

### Main Content Blocks
- Audience type selector
- Rules list
- Exclusions list
- Audience preview summary

### Supported Rule Types
- All Users
- Department
- AD Group
- Explicit Exclusion

### Main User Actions
- Add targeting rule
- Add exclusion
- Remove rule
- Preview audience result
- Save targeting

### UX Notes
- The screen must make inclusion/exclusion logic very clear
- Exclusions should be visually separated from inclusion rules
- Audience preview is highly recommended for confidence before publish

---

## 15. Page 12: Recurrence & Scheduling Configuration

### Page Name
Recurrence Configuration

### Purpose
Allow authorized users to configure when and how the action repeats.

### Main Content Blocks
- Recurrence model selector
- Start date
- Due date
- Recurrence notes / summary
- Optional annual renewal settings

### Supported Models
- Onboarding Only
- Annual
- Onboarding + Annual
- On Change
- Event-Driven

### Main User Actions
- Select recurrence model
- Set dates
- Save recurrence settings

### UX Notes
- Recurrence rules must be easy to understand
- This page should explain operational effect, not just collect values
- The selected recurrence model should display a human-readable summary

---

## 16. Page 13: User Requirement Monitoring

### Page Name
User Action Monitoring

### Purpose
Allow authorized users to monitor which users have which active requirements.

### Main Content Blocks
- Search/filter toolbar
- User requirement table
- Status breakdown

### Suggested Filters
- Action
- Policy
- Department
- Status
- Due date range
- Recurrence model

### Suggested Columns
- User
- Department
- Action
- Version
- Status
- Assigned Date
- Due Date
- Completed Date

### Main User Actions
- Search/filter
- Open requirement details
- Export list

### UX Notes
- This page is crucial for operations and follow-up
- Overdue records should be easy to isolate

---

## 17. Page 14: Compliance Dashboard

### Page Name
Compliance Dashboard

### Purpose
Allow compliance viewers and business owners to monitor completion at an aggregate level.

### Main Content Blocks
- KPI cards
- Completion by department
- Completion by action
- Overdue trends
- Non-compliant users summary

### Main User Actions
- Filter by period
- Filter by owner department
- Filter by action/policy
- Open detailed reports

### UX Notes
- Keep charts clean and brand-aligned
- Use only the approved visual palette
- This page should support operational insight, not deep BI complexity

---

## 18. Page 15: Compliance Report List / Detail

### Page Name
Compliance Reports

### Purpose
Allow authorized users to view and export compliance reports.

### Main Content Blocks
- Report filters
- Results table
- Export actions

### Supported Reports
- Compliance by action
- Compliance by department
- Non-compliant users
- Submission history

### Main User Actions
- Run report
- Export Excel
- Export PDF
- Open details

### UX Notes
- Filters should appear before heavy results
- Exports should be easy to access but not dominant

---

## 19. Page 16: Audit Log Explorer

### Page Name
Audit & History

### Purpose
Allow authorized users to inspect historical events for governance and audit purposes.

### Main Content Blocks
- Search/filter toolbar
- Audit log table
- Event detail panel

### Suggested Filters
- Actor
- Entity Type
- Action Type
- Date Range
- Department
- Related Policy / Action

### Suggested Columns
- Timestamp
- Actor
- Action
- Entity Type
- Entity Reference
- Version
- Description

### Main User Actions
- Search/filter logs
- Open log details
- Export if permitted

### UX Notes
- This page must be highly readable
- Historical context is more important than decorative UI
- Audit records should feel authoritative and trustworthy

---

## 20. Page 17: Historical Submission Review

### Page Name
Submission Details Review

### Purpose
Allow authorized users to inspect a past user submission.

### Main Content Blocks
- User context
- Related action/version
- Submission date
- Submission status
- Submitted data
- Linked policy/version reference

### Main User Actions
- View submission details
- Return to compliance list/history

### UX Notes
- If the submission is form-based, show the answers grouped by section
- Historical context must be preserved
- The page should not imply editability of historical data

---

## 21. Page 18: Role & Access Context

### Page Name
Access Context / Role View

### Purpose
Allow administrators to understand and manage the current role/scope behavior of the portal.

### Main Content Blocks
- Role assignments summary
- Scope summary
- User-to-role mapping view

### Main User Actions
- View access assignments
- Search users
- Inspect role/scope context

### UX Notes
- Keep Phase 1 lightweight unless deeper admin controls are explicitly required
- Focus on visibility rather than a full IAM console

---

## 22. Page 19: Settings / Operational Configuration

### Page Name
System Settings

### Purpose
Allow limited operational configuration in Phase 1.

### Possible Scope
- notification configuration references
- recurrence defaults
- status labels reference
- file rules reference

### Main User Actions
- view settings
- update limited settings if authorized

### UX Notes
- Keep this page very minimal in Phase 1
- Avoid opening complex platform administration unless required

---

## 23. Common Admin Portal Components

The following reusable components should exist across admin pages:

### 23.1 KPI Card
Used in:
- dashboard
- compliance summaries
- operational overview

### 23.2 Management Table
Used in:
- policy list
- acknowledgment list
- monitoring pages
- reports
- audit logs

### 23.3 Filter Bar
Used in:
- all major list/report pages

### 23.4 Status Badge
Used for:
- Draft
- Published
- Superseded
- Archived
- Pending
- Completed
- Overdue

### 23.5 Version Badge
Used to identify:
- version number
- active version
- historical version

### 23.6 Form Section Block
Used in:
- form definition management
- submission review

### 23.7 Empty State
Used when:
- no results
- no audience rules
- no versions
- no compliance data for selected filters

### 23.8 Confirmation Modal
Used for:
- publish
- archive
- invalidation if added later

---

## 24. Admin Navigation Rules

### Recommended Primary Navigation
- Dashboard
- Policies
- Acknowledgments
- Compliance
- Audit
- Settings

### Secondary Navigation
May exist within modules, such as:
- Policy Details
- Versions
- Audience
- Recurrence
- Form Definition

### Navigation Principles
- navigation must remain shallow
- major operational areas must be discoverable immediately
- avoid hidden critical pages
- keep “create” actions close to list pages

---

## 25. MVP Priority Pages

The highest-priority admin portal pages for Phase 1 are:

1. Admin Dashboard
2. Policy List
3. Policy Version Management
4. Acknowledgment List
5. Acknowledgment Version Details
6. Form Definition Management
7. Audience Targeting Management
8. Recurrence Configuration
9. User Requirement Monitoring
10. Compliance Dashboard
11. Audit Log Explorer

---

## 26. Admin Portal UX Constraints

The Admin Portal in Phase 1 must not include:

- visually heavy BI-style dashboards
- deep configuration trees
- drag-and-drop form builder
- complex workflow editors
- confusing side-by-side overloaded screens
- decorative graphics inside dense tables and operational forms

---

## 27. Summary

The Admin Portal is the operational and governance control layer of the platform.

Its purpose is to help authorized users:

- create and manage policies
- create and manage acknowledgments/disclosures
- configure audience and recurrence
- monitor compliance
- access history and audit evidence

The portal must be:
- clear
- structured
- table-friendly
- operationally efficient
- role-aware
- consistent with The Environment Fund identity
