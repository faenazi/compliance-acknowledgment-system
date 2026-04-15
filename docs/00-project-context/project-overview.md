# Project Overview

## 1. Introduction

The **Enterprise Acknowledgment Platform (EAP)** is an internal system for **The Environment Fund** designed to manage policies, acknowledgments, and disclosures in a centralized, structured, and auditable way.

The platform enables the organization to ensure that employees:

- review assigned policies
- submit acknowledgments
- submit required disclosures and declarations
- can be tracked for compliance purposes
- can be audited through a centralized record

---

## 2. Problem Statement

Currently, policy acknowledgment and disclosure processes are:

- manual or semi-manual
- fragmented across email, files, and disconnected tools
- difficult to track centrally
- difficult to audit
- lacking real-time visibility
- not scalable across multiple departments and policy types

This creates risks in:

- compliance tracking
- governance
- audit readiness
- policy version control
- organizational accountability
- operational efficiency

---

## 3. Solution Overview

The EAP platform provides a centralized solution that supports:

- policy document management
- policy versioning
- simple acknowledgments
- form-based acknowledgments and disclosures
- audience targeting
- compliance tracking
- user-level history
- reporting
- audit logging
- integration with LDAP / Active Directory and Exchange

---

## 4. Objectives

The system aims to:

- centralize policy acknowledgment and disclosure processes
- improve compliance visibility across the organization
- provide auditable records
- reduce manual effort for business teams
- support both onboarding and recurring policy actions
- provide a scalable base for future workflow and enforcement features

---

## 5. Scope (MVP - Phase 1)

### Included

- User authentication via LDAP / Active Directory
- User profile creation and synchronization from AD
- Policy upload and version management
- Acknowledgment creation and publishing
- Support for simple acknowledgments
- Support for form-based acknowledgments and disclosures
- Basic audience targeting (all users / department / AD groups / exclusions)
- User portal for viewing and completing acknowledgments
- Compliance tracking (Pending / Completed / Overdue)
- Email notifications via Exchange
- Basic reporting
- Audit logging

### Excluded (Deferred)

- Blocking / enforcement logic
- Exception and defer workflows
- Advanced rules engine
- Approval workflows
- Full visual form builder
- Mobile application
- eSignature
- Advanced analytics

---

## 6. Core Modules

### 6.1 Identity & User Management
- LDAP / Active Directory authentication
- user profile creation and synchronization
- AD attribute retrieval

### 6.2 Policy Management
- policy creation
- document upload
- version control
- publish / archive behavior

### 6.3 Acknowledgment Management
- create and publish acknowledgments
- link acknowledgments to policy versions
- manage dates and audience

### 6.4 Form-Based Disclosure Support
- support structured user input forms
- support predefined field types
- store submitted values with acknowledgment record

### 6.5 Audience Targeting
- all users
- by department
- by AD groups
- explicit exclusions

### 6.6 User Portal
- dashboard
- pending acknowledgments
- completed acknowledgments
- overdue acknowledgments
- acknowledgment details
- form submission
- user history

### 6.7 Admin Portal
- manage policies
- manage acknowledgments
- manage audiences
- access reports
- operational oversight

### 6.8 Compliance Tracking
- track completion per user
- track completion per department
- identify overdue users

### 6.9 Notification System
- email notifications through Exchange
- reminders
- overdue alerts

### 6.10 Audit Logging
- track key administrative actions
- track user submissions
- preserve evidence for governance and audit

---

## 7. Users & Roles

### End User
- view assigned acknowledgments
- review linked policies
- submit acknowledgment or disclosure
- view personal history

### Policy Manager
- create and maintain policy drafts

### Acknowledgment Manager
- create and maintain acknowledgment drafts
- define audience and schedule

### Publisher
- publish policies and acknowledgments

### Compliance Viewer
- view compliance dashboards and reports

### Auditor
- review historical records and audit trails

### System Administrator
- manage system configuration, roles, and operational settings

---

## 8. Key Concepts

| Concept | Description |
|--------|------------|
| Policy | Official document that users must review |
| Policy Version | Specific immutable version of a policy |
| Acknowledgment | User-facing action linked to a policy version |
| Form-Based Acknowledgment | Acknowledgment requiring structured user input |
| User Acknowledgment | Recorded completion by a specific user |
| Compliance Status | Current status of a user action |
| Audit Log | Immutable record of significant system actions |

---

## 9. System Principles

- every acknowledgment must be linked to a policy version
- published content must be version-controlled
- form-based disclosures are supported in Phase 1
- auditability is mandatory
- AD is the source of truth for identity data
- the MVP must remain simple and fast to implement
- no enforcement logic is included in Phase 1
- the platform must support future expansion

---

## 10. Success Criteria

The system is successful when:

- users can sign in and access their assigned actions
- users can complete simple and form-based acknowledgments
- business teams can publish policies and acknowledgments successfully
- compliance status is visible to business owners
- notifications are delivered successfully
- reports provide accurate completion information
- the system is audit-ready
