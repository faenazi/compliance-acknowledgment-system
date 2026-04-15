# Project Overview

## System Name

Enterprise Acknowledgment Platform (EAP)

## Owner

The Environment Fund — Internal Compliance Function.

## Purpose

EAP is an internal compliance platform that governs the full lifecycle of organizational policies and the acknowledgment of those policies by employees. It provides a single source of truth for what each employee is required to read, what they have acknowledged, and when — backed by an immutable audit trail sufficient for internal and external audit review.

## Problem It Solves

The Environment Fund currently distributes policies through email, shared drives, and manual tracking sheets. This produces three failures:

1. No reliable record of who has read which version of a policy.
2. No enforcement that acknowledgments are tied to a specific, unmodified version.
3. No defensible audit trail when regulators or internal audit request evidence.

EAP replaces these manual processes with a controlled, versioned, auditable system.

## Core Domains (Phase 1)

1. **Policies** — authoring, categorization, and ownership of compliance documents.
2. **Versioning** — every policy change produces a new immutable version; published versions cannot be altered.
3. **Acknowledgments** — employee confirmation that a specific policy version has been read and accepted.
4. **User Compliance** — per-user status showing which required acknowledgments are complete, pending, or overdue.
5. **Audit Logs** — append-only record of every state-changing action in the system.
6. **Reporting** — compliance posture across the organization, by policy, department, and user.

## Primary Actors

- **Employee (User)** — receives assigned policies, reads them, submits acknowledgments, views personal compliance status.
- **Compliance Administrator** — authors policies, publishes versions, assigns audiences, monitors compliance, exports reports.
- **Auditor (read-only)** — reviews audit logs and compliance reports; cannot modify data.

## Portals

- **User Portal** — full-featured employee experience: assigned policies, reading, acknowledgment submission, personal compliance history.
- **Admin Portal** — lean operational console for compliance administrators: policy authoring, version publishing, audience assignment, reporting, audit log inspection.

## Language and Direction

Arabic-first, right-to-left (RTL) as the default UI direction. English is a secondary language.

## Phase 1 Scope

In scope:

- Policy authoring and versioning
- Publishing workflow (draft → published)
- Acknowledgment capture bound to a specific version
- Per-user compliance status
- Audit logging of all state changes
- Administrative reporting and export

Out of scope for Phase 1:

- E-signature integrations
- Automated training/quiz modules
- External (non-employee) acknowledgments
- Mobile native apps
- Third-party identity federation beyond the organization's internal directory
