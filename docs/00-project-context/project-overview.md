# Project Overview

---

# 1. Introduction

The **Enterprise Acknowledgment Platform (EAP)** is an internal system for **The Environment Fund** designed to manage policies and user acknowledgments in a centralized, structured, and auditable way.

The platform enables the organization to ensure that employees are aware of policies and formally acknowledge them, while providing visibility into compliance status across departments.

---

# 2. Problem Statement

Currently, policy acknowledgment processes are:

- Manual or semi-manual
- Not centrally tracked
- Difficult to audit
- Lacking real-time visibility
- Dependent on email or external tools

This creates risks in:
- Compliance tracking
- Audit readiness
- Policy enforcement
- Organizational accountability

---

# 3. Solution Overview

The EAP platform provides:

- Centralized policy management
- Structured acknowledgment workflows
- User-level tracking of acknowledgments
- Organizational compliance visibility
- Integration with Active Directory for user data
- Email notification capabilities

---

# 4. Objectives

The system aims to:

- Ensure all users are aware of relevant policies
- Track acknowledgment completion across the organization
- Provide real-time compliance visibility
- Enable audit-ready records
- Reduce manual effort in policy tracking

---

# 5. Scope (MVP - Phase 1)

The initial version of the system includes:

## Included

- User authentication via LDAP / Active Directory
- Policy upload and version management
- Acknowledgment creation and publishing
- Basic audience targeting (department / AD groups / all users)
- User portal for viewing and completing acknowledgments
- Compliance tracking (Pending / Completed / Overdue)
- Email notifications via Exchange
- Basic reporting
- Audit logging

---

## Excluded (Deferred)

- Blocking / enforcement logic
- Exception and defer management
- Advanced rules engine
- Approval workflows
- Dynamic forms
- Mobile applications
- eSignature
- Advanced analytics

---

# 6. Core Modules

The system consists of the following modules:

## 6.1 Identity & User Management
- LDAP / Active Directory authentication
- User profile creation and synchronization

## 6.2 Policy Management
- Policy creation
- Document upload
- Version control

## 6.3 Acknowledgment Management
- Create and publish acknowledgments
- Link acknowledgments to policies

## 6.4 Audience Targeting
- Assign acknowledgments to users based on:
  - Department
  - AD Groups
  - All users

## 6.5 User Portal
- Dashboard
- View acknowledgments
- Submit acknowledgment
- View history

## 6.6 Compliance Tracking
- Track acknowledgment status per user
- Calculate compliance metrics

## 6.7 Notification System
- Email notifications via Exchange

## 6.8 Audit Logging
- Track key system actions

---

# 7. Users & Roles

## End User
- View assigned acknowledgments
- Complete acknowledgments
- View history

## Content Manager
- Create policies
- Upload documents

## Publisher
- Publish policies and acknowledgments

## Compliance Viewer
- View reports and compliance data

## Administrator
- Manage system configuration
- Manage users and access

---

# 8. Key Concepts

| Concept | Description |
|--------|------------|
| Policy | Official document requiring acknowledgment |
| Policy Version | Specific version of a policy document |
| Acknowledgment | User agreement to a policy |
| User Acknowledgment | Record of user action |
| Compliance | User completion status |
| Audit Log | Record of system actions |

---

# 9. System Principles

- All acknowledgments must be linked to a policy version
- Published content cannot be modified
- All actions must be logged
- System must be simple and fast for MVP
- No enforcement logic in Phase 1
- System must be expandable in future phases

---

# 10. Success Criteria

The system is considered successful if:

- Users can easily view and complete acknowledgments
- Management can track compliance in real-time
- Reports accurately reflect user status
- Notifications are delivered successfully
- System is audit-ready
