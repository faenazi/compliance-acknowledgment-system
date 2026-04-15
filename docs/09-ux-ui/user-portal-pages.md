# User Portal Pages

## 1. Purpose

This document defines the pages, page objectives, and UX expectations for the User Portal of the Enterprise Acknowledgment Platform (EAP).

The User Portal is designed for employees and end users who need to:

- view required actions
- review linked policies
- submit acknowledgments
- submit disclosures
- track their own completion history

This document defines:
- required pages
- page purpose
- key content blocks
- main user actions
- UX notes for implementation

---

## 2. User Portal Design Goals

The User Portal must be:

- simple
- fast
- clear
- Arabic-first
- low-friction
- highly readable

The portal must prioritize:
- understanding what is required
- completing required actions quickly
- reducing confusion
- making status visible
- preserving trust and clarity

---

## 3. User Portal Information Architecture

The User Portal should include the following primary sections:

1. Dashboard
2. My Required Actions
3. Action Details
4. Policy Viewer
5. Submit Acknowledgment
6. Submit Disclosure Form
7. My History
8. Profile / User Context

---

## 4. Page 1: User Dashboard

### Page Name
User Dashboard

### Purpose
Provide the user with a clear summary of:
- what is pending
- what is overdue
- what is completed
- what requires immediate attention

### Main Content Blocks
- Welcome header
- Summary cards
- Pending actions list
- Overdue actions list
- Recently completed actions
- Quick action shortcuts

### Suggested Summary Cards
- Pending Actions
- Overdue Actions
- Completed Actions
- Annual Disclosures Due

### Main User Actions
- Open a pending action
- Open an overdue action
- View all required actions
- View history

### UX Notes
- This page must make priority items obvious
- Overdue items should appear above normal pending items
- Use simple summary cards with strong hierarchy
- Do not overload the dashboard with unnecessary charts in the end-user portal

---

## 5. Page 2: My Required Actions

### Page Name
My Required Actions

### Purpose
Display all currently applicable user actions.

### Main Content Blocks
- Page header
- Search/filter bar
- Status tabs or segmented filters
- Action list or table

### Suggested Filters
- All
- Pending
- Overdue
- Completed
- By action type
- By recurrence type

### Suggested Columns / Card Fields
- Action Title
- Linked Policy
- Action Type
- Due Date
- Status
- Owner Department
- Recurrence Type

### Main User Actions
- Open action details
- Filter actions
- Search actions

### UX Notes
- Pending and Overdue must be easily distinguishable
- Completed items should still be visible but visually secondary
- Mobile responsiveness is not primary for Phase 1, but layout should still collapse cleanly

---

## 6. Page 3: Action Details

### Page Name
Action Details

### Purpose
Provide the user with all information needed before submission.

### Main Content Blocks
- Action title
- Status badge
- Action type
- Owner department
- Due date
- Effective date / recurrence info
- Policy summary
- Action instructions
- Primary action area

### Main User Actions
- View linked policy
- Start acknowledgment
- Start disclosure form
- Return to actions list

### UX Notes
- This page must clearly answer:
  - What is this action?
  - Why am I seeing it?
  - What do I need to do?
  - By when?
- The primary call-to-action must be obvious
- Instructions should be concise and readable

---

## 7. Page 4: Policy Viewer

### Page Name
Policy Viewer

### Purpose
Allow the user to read or review the linked policy document before submitting.

### Main Content Blocks
- Policy title
- Policy version label
- Owner department
- Effective date
- Embedded PDF viewer or download/open experience
- Return to action

### Main User Actions
- View policy
- Return to action details
- Continue to submission

### UX Notes
- Reading experience must be clean and distraction-free
- Avoid unnecessary side content
- The user must always have a clear path back to submission
- If embedded viewing is used, it should not feel cramped

---

## 8. Page 5: Simple Acknowledgment Submission

### Page Name
Submit Acknowledgment

### Purpose
Allow the user to confirm they have read and/or accepted the relevant policy.

### Main Content Blocks
- Action title
- Policy reference
- Short instruction text
- Acknowledgment text
- Confirmation checkbox or explicit consent section
- Submit button

### Main User Actions
- Confirm acknowledgment
- Submit
- Cancel / go back

### UX Notes
- This page should be visually light and straightforward
- The form should not feel heavy
- The acknowledgment text should be readable and not buried
- The submit action should be disabled until required confirmation is provided, if applicable

---

## 9. Page 6: Commitment Acknowledgment Submission

### Page Name
Submit Commitment Acknowledgment

### Purpose
Allow the user to acknowledge and explicitly commit to policy requirements.

### Main Content Blocks
- Action title
- Policy reference
- Commitment text block
- Confirmation control
- Submit button

### Main User Actions
- Review commitment text
- Confirm commitment
- Submit

### UX Notes
- Commitment acknowledgments should feel slightly more formal than simple acknowledgment
- Use clear wording and visible confirmation language
- Avoid unnecessary visual clutter

---

## 10. Page 7: Form-Based Disclosure Submission

### Page Name
Submit Disclosure Form

### Purpose
Allow the user to submit structured disclosure information.

### Main Content Blocks
- Action header
- Policy reference
- Intro / instructions
- Form sections
- Field groups
- Validation messages
- Final declaration / consent block
- Submit button

### Required UX Behaviors
- support long forms
- support grouped sections
- support required field indicators
- support inline validation
- support file upload fields where applicable
- support reviewable structure

### Main User Actions
- fill fields
- upload files if required
- submit disclosure
- return to action details

### UX Notes
- Long forms must be broken into clear sections
- Related fields must be grouped logically
- The page must remain readable even for large disclosures such as conflict of interest
- The final declaration section should be visually distinct
- Required fields must be obvious

---

## 11. Page 8: Submission Success / Confirmation

### Page Name
Submission Confirmation

### Purpose
Confirm to the user that the acknowledgment or disclosure has been submitted successfully.

### Main Content Blocks
- Success message
- Submission timestamp
- Action title
- Status result
- Next-step note if relevant
- Return options

### Main User Actions
- Return to dashboard
- View history
- View action details

### UX Notes
- This page must reassure the user that submission is complete
- Use a clean confirmation state
- Do not overload it with unnecessary information

---

## 12. Page 9: My History

### Page Name
My History

### Purpose
Allow the user to review previously completed acknowledgments and disclosures.

### Main Content Blocks
- Page header
- Search/filter tools
- History list or table

### Suggested Fields
- Action title
- Action type
- Policy title
- Version
- Submitted date
- Status
- Recurrence cycle if applicable

### Main User Actions
- View past submission
- Filter by action type
- Search by title/policy

### UX Notes
- The history page should provide confidence and traceability
- Historical records should be easy to scan
- Completed disclosures should be distinguishable from simple acknowledgments

---

## 13. Page 10: Historical Submission Details

### Page Name
Submission History Details

### Purpose
Allow the user to review the details of a past submission.

### Main Content Blocks
- Action title
- Submitted date
- Related policy/version
- Submission summary
- Submitted form values if applicable

### Main User Actions
- View submission details
- Return to history

### UX Notes
- This page must preserve historical context
- If the submission was form-based, values should be presented in a readable, section-based way
- Users should not be able to edit past submissions in Phase 1 unless explicitly allowed later

---

## 14. Page 11: User Profile / User Context

### Page Name
My Profile

### Purpose
Allow the user to view their basic profile data used by the system.

### Main Content Blocks
- Full name
- Username
- Email
- Department
- Job title
- Role context if appropriate

### Main User Actions
- View profile only

### UX Notes
- This can be lightweight in Phase 1
- Editing is not required if identity data is sourced from AD

---

## 15. Common User Portal Components

The following reusable components should exist across user portal pages:

### 15.1 Status Badge
Used for:
- Pending
- Completed
- Overdue

### 15.2 Action Card / Action Row
Used in:
- dashboard
- required actions list
- history list

### 15.3 Policy Reference Block
Used in:
- action details
- submission pages
- historical details

### 15.4 Section Header
Used for:
- form grouping
- page content segmentation

### 15.5 Empty State
Used when:
- user has no pending actions
- user has no overdue actions
- history is empty

### 15.6 Success Message Block
Used after:
- successful acknowledgment
- successful disclosure submission

---

## 16. User Portal Navigation Rules

### Primary Navigation Items
Recommended user portal navigation:

- Dashboard
- My Actions
- My History
- My Profile

### Navigation Principles
- keep navigation minimal
- avoid deep nesting
- provide clear active state
- always make the dashboard easy to return to

---

## 17. Page Priority for MVP

The highest-priority user portal pages for Phase 1 are:

1. User Dashboard
2. My Required Actions
3. Action Details
4. Policy Viewer
5. Submit Acknowledgment
6. Submit Disclosure Form
7. Submission Confirmation
8. My History

Profile page is lower priority and may remain lightweight.

---

## 18. User Portal UX Constraints

The User Portal in Phase 1 must not include:

- blocking access experience
- complicated multi-step wizard unless required later
- advanced editable draft behavior unless explicitly approved
- overly dense dashboard analytics
- decorative graphics inside operational forms
- complex branching flows

---

## 19. Summary

The User Portal is the employee-facing execution layer of the platform.

Its main goal is to help users:

- understand what is required
- access the relevant policy
- complete the needed action
- verify that it was submitted
- review their own history

The portal must be:
- clear
- fast
- highly readable
- low-friction
- consistent with The Environment Fund identity
