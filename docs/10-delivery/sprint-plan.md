# Sprint Plan

## 1. Purpose

This document defines the sprint-based delivery plan for the Enterprise Acknowledgment Platform (EAP).

It breaks the MVP into structured implementation phases so that the team can:

- build in the correct order
- reduce delivery risk
- validate business assumptions early
- deliver usable value incrementally
- align business, design, and engineering work
- guide AI-assisted development in a controlled way

This plan is designed for Phase 1 (MVP).

---

## 2. Planning Principles

The sprint plan follows these principles:

- MVP-first delivery
- clear separation between foundation, core business logic, and user experience
- small but meaningful sprint scope
- versioning and auditability established early
- user-facing value delivered incrementally
- form-based disclosures included in Phase 1
- no advanced workflow or blocking logic in MVP
- no full visual form builder in MVP

---

## 3. Recommended Sprint Model

### Sprint Duration
- 2 weeks per sprint

### Recommended Phase 1 Plan
- 9 implementation sprints
- optional stabilization / release sprint if needed

### Why 9 Sprints
This structure is recommended because it:

- reduces scope overload in each sprint
- makes AI-assisted development easier to control
- separates high-risk components
- improves visibility of progress
- allows earlier testing of business-critical flows

---

## 4. MVP Delivery Goals

By the end of Phase 1, the platform should support:

- user authentication through LDAP / Active Directory
- user profile synchronization
- policy upload and versioning
- acknowledgment and disclosure definition
- simple acknowledgments
- form-based disclosures
- audience targeting
- recurrence models
- user portal
- admin portal
- compliance tracking
- Exchange notifications
- audit trail
- basic reporting

---

## 5. Sprint Overview

| Sprint | Name | Core Outcome |
|------|------|--------------|
| Sprint 0 | Foundation & Setup | Project skeleton and engineering foundation |
| Sprint 1 | Identity & Access Foundations | Authentication, users, roles, scopes |
| Sprint 2 | Policy Management | Policies, policy versions, document handling |
| Sprint 3 | Acknowledgment Core | Acknowledgment definitions and versions |
| Sprint 4 | Audience & Recurrence | Targeting and recurring action logic |
| Sprint 5 | Form-Based Disclosures | Dynamic disclosure forms and submission model |
| Sprint 6 | User Portal | Employee-facing completion flow |
| Sprint 7 | Admin Portal & Operations | Admin workflows and monitoring |
| Sprint 8 | Compliance, Notifications, Audit & Reports | Operational visibility and launch readiness |

---

## 6. Sprint 0 – Foundation & Setup

## Objective
Establish the technical and delivery foundation for the project.

## Scope
- repository structure
- solution setup
- project scaffolding
- coding conventions
- environment configuration
- package installation
- shared technical standards
- UI foundation

## Backend Tasks
- create ASP.NET Core solution structure
- create modular project structure
- establish vertical slice conventions
- configure MediatR
- configure FluentValidation
- configure AutoMapper
- configure Serilog
- configure Swagger
- prepare EF Core project setup
- create base API conventions
- create standardized API response/error structure

## Frontend Tasks
- create Next.js app structure
- configure TypeScript
- configure Tailwind CSS
- configure RTL-first setup
- configure React Query
- configure Axios client
- configure React Hook Form
- configure Zod
- establish app layout shell
- establish route shell

## UX / Design Tasks
- implement design tokens
- define base spacing system
- define base typography system
- define base components:
  - buttons
  - inputs
  - cards
  - badges
  - tables
  - modal/dialog

## Delivery Output
- working backend skeleton
- working frontend skeleton
- approved package setup
- shared coding and UI foundation

---

## 7. Sprint 1 – Identity & Access Foundations

## Objective
Deliver authentication, user creation, role model, and scoped access foundation.

## Scope
- LDAP / AD authentication
- user profile creation
- user synchronization
- roles
- scopes
- user-role assignments

## Backend Tasks
- implement LDAP / AD authentication flow
- implement login/session integration approach
- create User entity and persistence
- create Role entity
- create Scope entity
- create UserRoleAssignment entity
- implement user synchronization from AD
- implement access context foundation
- add audit hooks for authentication-related events where appropriate

## Frontend Tasks
- create authenticated app shell
- create login experience if required by deployment model
- create session-aware navigation shell
- create profile context foundation
- create role-aware route handling foundation

## Delivery Output
- authenticated users can access the platform
- user profiles are created/synced
- role and scope model is in place

---

## 8. Sprint 2 – Policy Management

## Objective
Deliver policy management, versioning, and document upload.

## Scope
- policy records
- policy versions
- document upload
- publish/archive behavior
- historical visibility

## Backend Tasks
- create Policy entity
- create PolicyVersion entity
- create PolicyDocument entity
- implement policy CRUD endpoints
- implement policy version endpoints
- implement policy document upload endpoints
- enforce one published version per policy
- implement publish/archive rules
- implement history retrieval
- log audit events for policy operations

## Frontend Tasks
- build policy list page
- build create/edit policy page
- build policy versions page
- build policy version details page
- build document upload UI
- build status badges
- build publish/archive actions

## Delivery Output
- admin users can manage policies and versions
- official policy documents can be uploaded and retrieved
- historical policy versions are visible

---

## 9. Sprint 3 – Acknowledgment Core

## Objective
Deliver core acknowledgment and disclosure master management.

## Scope
- acknowledgment definitions
- acknowledgment versions
- linkage to policy versions
- action types
- base lifecycle states

## Backend Tasks
- create AcknowledgmentDefinition entity
- create AcknowledgmentVersion entity
- implement acknowledgment definition CRUD
- implement acknowledgment version CRUD
- link acknowledgment versions to policy versions
- support action types:
  - Simple Acknowledgment
  - Acknowledgment with Commitment
  - Form-Based Disclosure
- enforce versioning rules
- implement publish/archive rules
- log audit events for acknowledgment operations

## Frontend Tasks
- build acknowledgment list page
- build create/edit acknowledgment page
- build acknowledgment versions page
- build acknowledgment version details page
- build action type selector
- build version status UI

## Delivery Output
- admin users can define and version actions
- actions are linked to exact policy versions
- action types are operationally recognized

---

## 10. Sprint 4 – Audience Targeting & Recurrence

## Objective
Deliver assignment logic for who receives an action and when it becomes required.

## Scope
- audience definitions
- audience rules
- exclusions
- recurrence models
- start/due date behavior
- user action requirement generation foundation

## Backend Tasks
- create AudienceDefinition entity
- create AudienceRule entity
- implement target audience storage
- implement targeting by:
  - all users
  - department
  - AD group
  - exclusions
- implement recurrence model storage
- support recurrence models:
  - Onboarding Only
  - Annual
  - Onboarding + Annual
  - On Change
  - Event-Driven
- create UserActionRequirement entity
- implement requirement generation logic foundation

## Frontend Tasks
- build audience targeting page
- build exclusions UI
- build recurrence configuration page
- build recurrence summary component
- build target audience preview summary

## Delivery Output
- actions can be targeted to the correct users
- actions can be scheduled and repeated correctly
- user obligation records can start to be generated

---

## 11. Sprint 5 – Form-Based Disclosures

## Objective
Deliver dynamic form-based disclosures for scenarios such as conflict of interest and gifts/hospitality.

## Scope
- form definition model
- JSON-driven form schema
- supported field types
- validation
- submission storage model

## Backend Tasks
- create FormDefinition entity or equivalent version-bound storage
- store form definition at acknowledgment version level
- support field types:
  - Short Text
  - Long Text
  - Number
  - Decimal
  - Date
  - Checkbox
  - Radio Group
  - Dropdown
  - Multi Select
  - Yes/No
  - Email
  - Phone Number
  - File Upload
  - Read-Only Display Field
  - Section Header / Static Text
- implement backend validation of submitted values
- create UserSubmission entity
- implement structured submission storage
- implement optional field-value flattening model if needed
- implement form snapshot support if included
- audit disclosure submission events

## Frontend Tasks
- build form definition management page
- build field editor UI
- build field ordering UI
- build section/group support
- build dynamic form renderer
- build inline validation behavior
- build file upload field experience

## Priority Business Use Cases
- conflict of interest disclosure
- gifts and hospitality disclosure

## Delivery Output
- admin users can configure structured disclosure forms
- end users can fill and submit dynamic forms
- submissions are version-aware and auditable

---

## 12. Sprint 6 – User Portal

## Objective
Deliver the employee-facing experience for viewing and completing actions.

## Scope
- dashboard
- required actions list
- action details
- policy viewer
- acknowledgment submission
- disclosure submission
- history

## Backend Tasks
- implement current user action retrieval APIs
- implement pending/completed/overdue retrieval
- implement action detail APIs
- implement policy viewing endpoint
- implement simple acknowledgment submission endpoint
- implement commitment acknowledgment submission endpoint
- implement disclosure submission endpoint
- update requirement status logic

## Frontend Tasks
- build user dashboard
- build my required actions page
- build action details page
- build policy viewer
- build simple acknowledgment page
- build commitment acknowledgment page
- build form-based disclosure page
- build submission confirmation page
- build my history page
- build submission history details page

## Delivery Output
- end users can see what is required
- end users can read policies
- end users can complete simple and form-based actions
- end users can review their history

---

## 13. Sprint 7 – Admin Portal & Operations

## Objective
Deliver the operational admin experience for managing and monitoring the platform.

## Scope
- admin dashboard
- policy management refinements
- acknowledgment management refinements
- user action monitoring
- historical detail pages

## Backend Tasks
- implement admin dashboard summary APIs
- implement user requirement monitoring queries
- implement historical retrieval APIs for admin use
- refine version detail endpoints
- refine audience and recurrence configuration APIs

## Frontend Tasks
- build admin dashboard
- refine policy management screens
- refine acknowledgment management screens
- build user action monitoring page
- build submission review page
- build operational summary cards

## Delivery Output
- business admins can manage platform operations clearly
- compliance owners can inspect user requirements and submissions
- operational visibility is now practical

---

## 14. Sprint 8 – Compliance, Notifications, Audit & Reports

## Objective
Deliver reporting, notification automation, audit visibility, and release readiness for MVP.

## Scope
- compliance dashboards
- reports
- Exchange notifications
- audit log explorer
- exports
- release stabilization

## Backend Tasks
- implement compliance summary APIs
- implement department-level compliance queries
- implement non-compliant user queries
- implement Exchange integration
- implement assignment notifications
- implement reminder notifications
- implement overdue notifications
- implement notification result logging
- implement audit log retrieval
- implement export endpoints for Excel/PDF where included in MVP
- optimize critical queries

## Frontend Tasks
- build compliance dashboard
- build compliance reports page
- build audit log explorer
- build export flows
- build notification-related operational indicators where useful

## QA / UAT Tasks
- test conflict of interest flow
- test gifts and hospitality flow
- test acceptable use acknowledgment flow
- test annual recurrence scenarios
- test user history
- test audit traceability
- test role/scope access boundaries

## Delivery Output
- compliance reporting is operational
- notifications are working
- audit visibility is usable
- platform is ready for UAT / controlled launch

---

## 15. Optional Sprint 9 – Stabilization & Launch Readiness

## Objective
Provide a buffer sprint for final hardening if required by delivery conditions.

## Scope
- defect fixes
- UAT fixes
- performance improvements
- deployment readiness
- documentation cleanup

## Use This Sprint If
- major UAT feedback remains open
- infrastructure/deployment readiness needs more work
- reports/notifications need final tuning
- permissions or recurrence logic need stabilization

## Delivery Output
- stabilized MVP
- production-like readiness
- release support package complete

---

## 16. Dependency Order

The recommended dependency order is:

1. Foundation
2. Identity & Access
3. Policy Management
4. Acknowledgment Core
5. Audience & Recurrence
6. Form-Based Disclosures
7. User Portal
8. Admin Portal & Operations
9. Compliance / Notifications / Reports
10. Stabilization if needed

This order should only change if there is a strong delivery reason.

---

## 17. Scope Discipline for MVP

The following must not be added to Phase 1 sprint scope:

- blocking enforcement
- exception management
- defer workflows
- advanced approval workflow
- committee workflow
- advanced rules engine
- visual drag-and-drop form builder
- advanced conditional branching in forms
- mobile application
- advanced analytics / BI platform

These are Phase 2 candidates.

---

## 18. Definition of Done

A sprint item is considered done when:

- backend implementation is completed
- frontend implementation is completed if relevant
- validation rules are applied
- role/scope behavior is respected
- audit behavior is applied where relevant
- testing is completed at functional level
- no critical blockers remain
- documentation is updated if the implemented behavior changed the agreed design

---

## 19. Recommended Release Approach

### Internal Release Candidate
After Sprint 8 (or Sprint 9 if used), release to a controlled internal audience.

### Suggested Pilot Group
- GRC
- Human Capital
- limited employee sample

### Initial Business Flows to Pilot
- conflict of interest disclosure
- gifts and hospitality disclosure
- acceptable use acknowledgment
- onboarding-only HR acknowledgments

---

## 20. Summary

This sprint plan is designed to deliver the EAP MVP in a controlled, low-risk, and practical sequence.

The 9-sprint structure is recommended because it:

- reduces complexity per sprint
- improves execution clarity
- makes AI-assisted delivery easier to control
- supports iterative review with business stakeholders
- balances speed with implementation quality
