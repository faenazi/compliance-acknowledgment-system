# Solution Overview

## 1. Purpose

This document provides a high-level overview of the proposed solution for the Enterprise Acknowledgment Platform (EAP).

It defines:

- the overall solution structure
- the major components of the platform
- how business capabilities are mapped into system modules
- how integrations fit into the architecture
- how the MVP should be implemented in a controlled and scalable way

This document is not intended to be a detailed technical design or code-level architecture.
It is a high-level solution reference for analysis, design, and development planning.

---

## 2. Solution Objectives

The solution must achieve the following objectives:

- centralize policy acknowledgments and disclosures
- support both simple and form-based actions
- integrate with LDAP / Active Directory for identity and audience data
- integrate with Exchange for email notifications
- provide user and admin portals
- preserve versioning and auditability
- support recurring and event-driven actions
- remain simple enough for fast MVP delivery
- remain extensible for future phases

---

## 3. Solution Scope

The solution includes:

- Backend API layer
- Frontend web application
- Database
- File/document handling
- LDAP / Active Directory integration
- Exchange integration
- Compliance and reporting functions
- Audit logging

The solution excludes in Phase 1:

- mobile apps
- advanced workflow engine
- visual form builder
- blocking enforcement
- advanced analytics engine
- external third-party integrations beyond LDAP / AD and Exchange

---

## 4. High-Level Solution Architecture

The proposed solution consists of the following layers:

### 4.1 Presentation Layer
This layer includes the web application interfaces used by:
- End Users
- Policy Managers
- Acknowledgment Managers
- Publishers
- Compliance Viewers
- Auditors
- Administrators

It is implemented using:
- Next.js
- React
- TypeScript
- Tailwind CSS

### 4.2 API / Application Layer
This layer exposes backend APIs and application use cases.
It handles:
- requests from the frontend
- validation
- application workflows
- orchestration between domain, database, and integrations

It is implemented using:
- ASP.NET Core
- MediatR
- FluentValidation
- AutoMapper
- Swagger
- Serilog

### 4.3 Domain Layer
This layer contains business concepts and rules such as:
- policies
- policy versions
- acknowledgments
- disclosure forms
- user requirements
- submissions
- recurrence logic
- compliance status rules

### 4.4 Infrastructure Layer
This layer handles:
- SQL Server persistence
- LDAP / Active Directory access
- Exchange email delivery
- file storage
- audit persistence
- operational logging

---

## 5. Core Solution Components

## 5.1 Identity & Access Component

Responsible for:
- authenticating users against LDAP / Active Directory
- retrieving user identity attributes
- creating/updating local user profile
- enabling scoped authorization in the application

Key outputs:
- authenticated user session
- internal user record
- AD-derived attributes for targeting

---

## 5.2 Policy Management Component

Responsible for:
- creating policy records
- uploading policy documents
- managing policy versions
- publishing and archiving policy versions

Key business significance:
- establishes the official document base
- ensures every action is linked to an official policy version

---

## 5.3 Acknowledgment & Disclosure Management Component

Responsible for:
- creating acknowledgments/disclosures
- linking them to policy versions
- defining action type
- defining recurrence behavior
- defining dates
- publishing versions

Supported action models:
- Simple Acknowledgment
- Acknowledgment with Commitment
- Form-Based Disclosure

---

## 5.4 Form-Based Disclosure Component

Responsible for:
- storing structured form definitions at the acknowledgment version level
- rendering form-based disclosures in the frontend
- validating field values
- storing submitted data

MVP design direction:
- controlled configurable forms
- no visual form builder
- JSON-driven form structure
- version-bound form definitions

---

## 5.5 Audience Targeting Component

Responsible for:
- resolving target users
- using AD attributes such as department and group membership
- supporting:
  - all users
  - department targeting
  - AD group targeting
  - user exclusions

This component determines which users receive which actions.

---

## 5.6 User Action Assignment Component

Responsible for:
- generating or resolving applicable actions for users
- applying recurrence logic
- tracking assigned states
- setting Pending / Completed / Overdue

This component bridges business action definitions with actual user obligations.

---

## 5.7 User Portal Component

Responsible for:
- displaying the user dashboard
- showing pending/completed/overdue items
- viewing action details
- viewing linked policy document
- submitting acknowledgments and disclosures
- showing personal history

Primary goal:
- make completion simple and clear for employees

---

## 5.8 Admin Portal Component

Responsible for:
- policy management
- acknowledgment/disclosure management
- audience configuration
- recurrence configuration
- compliance viewing
- report access
- historical record access

Primary goal:
- give business teams controlled self-service capability

---

## 5.9 Compliance Tracking Component

Responsible for:
- status tracking per user
- identifying non-compliant users
- providing completion metrics
- enabling operational follow-up

Outputs include:
- compliance views
- summary counts
- completion ratios
- overdue views

---

## 5.10 Notification Component

Responsible for:
- preparing outbound notifications
- sending email via Exchange
- sending:
  - assignment notifications
  - reminders
  - overdue alerts
- logging delivery result

---

## 5.11 Reporting Component

Responsible for:
- compliance reports
- non-compliant user reports
- completion reports
- history views
- exports

MVP scope:
- operational reporting only
- no advanced analytics engine

---

## 5.12 Audit Component

Responsible for:
- capturing critical administrative actions
- capturing user submissions
- preserving historical traceability
- supporting governance and audit review

The audit component must remain separate from normal application logging.

---

## 6. Key Business Objects in the Solution

The solution is centered around the following core business objects:

- User
- Policy
- PolicyVersion
- AcknowledgmentDefinition
- AcknowledgmentVersion
- FormDefinition
- UserActionRequirement
- UserSubmission
- Notification
- AuditLog

These objects will be elaborated further in the data model documentation.

---

## 7. High-Level Process Flow

## 7.1 Policy-to-Action Flow

1. Business owner creates policy
2. Policy document is uploaded
3. Policy version is published
4. Acknowledgment/disclosure is created
5. Acknowledgment is linked to policy version
6. Audience and recurrence are configured
7. Acknowledgment version is published

Result:
- the action becomes available to the intended users

---

## 7.2 User Completion Flow

1. User logs in using LDAP / Active Directory
2. System resolves applicable actions for the user
3. User views pending actions
4. User opens action details
5. User reviews linked policy
6. User submits acknowledgment or disclosure
7. System records submission
8. System updates compliance status
9. System logs the action for audit

---

## 7.3 Recurrence Flow

1. Action is configured with a recurrence model
2. System evaluates recurrence schedule
3. New user requirement becomes active for the correct cycle
4. User completes or misses the cycle
5. Status becomes Completed or Overdue
6. Future cycle is generated if recurrence requires renewal

---

## 7.4 Event-Driven Disclosure Flow

1. Business policy defines an event-driven disclosure
2. User experiences a relevant event
3. User opens the disclosure form
4. User submits the disclosure
5. System records the disclosure under the current acknowledgment version
6. Compliance and audit data are updated

---

## 8. Integration Overview

## 8.1 LDAP / Active Directory Integration

Used for:
- authentication
- retrieving user attributes
- audience targeting data
- access scope context

Expected attributes:
- username
- full name
- email
- department
- job title
- AD groups

---

## 8.2 Exchange Integration

Used for:
- assignment notifications
- reminders
- overdue notifications

MVP delivery mode:
- SMTP or service integration

---

## 9. Data Storage Overview

## 9.1 Transactional Data

Stored in SQL Server:

- users
- policies
- versions
- actions
- audience definitions
- user requirements
- submissions
- compliance states
- notifications
- audit records

## 9.2 Documents

Policy documents are stored as uploaded files.
The exact storage mechanism may be:
- file system
- database-backed storage
- object storage in future

## 9.3 Form Definitions

Form definitions are stored at the acknowledgment version level.

## 9.4 Submission Data

Submission data is stored against the user submission / acknowledgment record in a structured format.

---

## 10. Versioning Strategy in the Solution

Versioning is a core design principle.

### 10.1 Policy Versioning
Every action must point to a specific policy version.

### 10.2 Acknowledgment Versioning
Every user action requirement and submission must point to a specific acknowledgment version.

### 10.3 Form Versioning
If a form changes materially, it must be treated as part of a new acknowledgment version.

### 10.4 Historical Integrity
Historical submissions must remain linked to the exact version used at the time of submission.

---

## 11. Access Control Model

The solution must use:

- Role-Based Access Control (RBAC)
- Scoped access

Access decisions are based on:
- role
- permission set
- organizational scope

Supported scopes include:
- Global
- Department Scope
- Owned Content Scope

This prevents permissions from being hardcoded by department name.

---

## 12. MVP Design Principles

The MVP implementation must follow these principles:

### 12.1 Keep Business Value First
Focus on operational value:
- publish actions
- complete actions
- track compliance
- notify users
- support audit

### 12.2 Avoid Overengineering
Do not build:
- workflow engines
- complex rule engines
- visual form builders
- enforcement logic

### 12.3 Preserve Expandability
Although simple, the architecture must support later growth in:
- enforcement
- exception flows
- approval workflow
- richer reporting
- broader departmental adoption

---

## 13. Phase 1 Solution Boundaries

Phase 1 must deliver a complete operational solution for:

- onboarding acknowledgments
- annual disclosures
- change-driven disclosures
- event-driven disclosures such as gifts and hospitality
- user and admin web experiences
- notifications
- reports
- audit trail

Phase 1 must not attempt to solve all future governance workflows.

---

## 14. Success Criteria for the Solution

The solution is considered successful when:

- business owners can publish policies and actions
- users can complete both simple and form-based actions
- AD-backed identity and targeting work correctly
- Exchange notifications are sent correctly
- compliance status is visible
- audit evidence is preserved
- the MVP is deployable and usable without major manual workarounds

---

## 15. Summary

The proposed solution for EAP is a web-based internal enterprise platform built on:

- ASP.NET Core backend
- Next.js frontend
- SQL Server
- LDAP / Active Directory
- Exchange

It is designed to support policy acknowledgments and disclosures in a centralized, versioned, auditable, and scalable way.

The architecture prioritizes:

- MVP speed
- business usability
- auditability
- future extensibility
