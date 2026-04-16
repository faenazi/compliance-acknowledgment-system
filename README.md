# Enterprise Acknowledgment Platform (EAP)

A centralized internal enterprise platform for managing policy acknowledgments, disclosures, and compliance tracking. Built for **The Environment Fund** to replace fragmented, manual processes with a structured, auditable, and scalable system.

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Core Concepts](#core-concepts)
- [User Roles](#user-roles)
- [Documentation](#documentation)
- [License](#license)

---

## Overview

The EAP platform enables the organization to:

- Publish and version-control official policies
- Create acknowledgments and form-based disclosures linked to policy versions
- Target specific audiences (all users, by department, by AD group, with exclusions)
- Track compliance status per user, department, and policy
- Send email notifications via Exchange (assignment, reminders, overdue alerts)
- Maintain a complete, immutable audit trail for governance and internal review
- Support recurring and event-driven action models (onboarding, annual, on-change, event-driven)

The platform provides two portals:

- **User Portal** -- Employees view pending actions, review policies, submit acknowledgments/disclosures, and track personal history
- **Admin Portal** -- Business teams manage policies, configure acknowledgments, define audiences, monitor compliance, and access reports

---

## Key Features

### Policy Management
- Create policy records with metadata (title, description, owner department)
- Upload PDF policy documents
- Version-controlled policies with Draft / Published / Superseded / Archived lifecycle
- Only one active published version per policy at any time

### Acknowledgment & Disclosure Management
- Create acknowledgment definitions linked to specific policy versions
- Support for three action types: **Simple Acknowledgment**, **Acknowledgment with Commitment**, and **Form-Based Disclosure**
- Version-controlled acknowledgment definitions with full lifecycle management
- Configure recurrence models: Onboarding Only, Annual, Onboarding + Annual, On Change, Event-Driven

### Form-Based Disclosures
- JSON-driven structured form definitions stored at the acknowledgment version level
- Dynamic rendering in the user portal
- Supported field types: Short Text, Long Text, Number, Decimal, Date, Checkbox, Radio Group, Dropdown, Multi Select, Yes/No, Email, Phone, File Upload, Read-Only Display, Section Header
- Server-side and client-side validation
- Submission data preserved with full historical traceability

### Audience Targeting
- Target all users, by department, by AD group, or with explicit exclusions
- Resolution based on current LDAP/Active Directory attributes
- Audience preview before publishing

### Compliance Tracking
- Track status per user per action: Pending, Completed, Overdue
- Department-level and organization-wide compliance views
- Non-compliant user identification
- Completion metrics and reporting

### Notifications
- Email notifications via Microsoft Exchange
- Assignment notifications, reminders before due date, overdue alerts
- Delivery outcome logging

### Audit Logging
- Immutable audit records for all critical administrative actions and user submissions
- Captures actor, action, timestamp, target entity, and version
- Separate from application logging

---

## Architecture

The platform follows a **Modular Vertical Slice Architecture** with clean separation of concerns:

```
┌──────────────────────────────────────────────────────────┐
│                   Presentation Layer                      │
│              Next.js + React + TypeScript                 │
│            (User Portal & Admin Portal)                   │
├──────────────────────────────────────────────────────────┤
│                   API / Application Layer                 │
│           ASP.NET Core + MediatR + FluentValidation       │
│              (Controllers, Commands, Queries)             │
├──────────────────────────────────────────────────────────┤
│                     Domain Layer                          │
│        (Entities, Enums, Business Rules, Value Objects)   │
├──────────────────────────────────────────────────────────┤
│                  Infrastructure Layer                     │
│     EF Core + SQL Server + LDAP + Exchange + File Storage │
└──────────────────────────────────────────────────────────┘
```

### Backend Projects

| Project | Responsibility |
|---------|---------------|
| `Eap.Api` | Controllers, authentication setup, middleware, Swagger configuration |
| `Eap.Application` | Commands, queries, validators, DTOs, mapping profiles, application services |
| `Eap.Domain` | Domain entities, enums, value objects, business rules |
| `Eap.Infrastructure` | EF Core persistence, repository implementations, LDAP integration, Exchange email, file storage, audit logging |

### Frontend Structure

| Directory | Responsibility |
|-----------|---------------|
| `src/app/(user)/` | User portal pages (dashboard, actions, history, profile) |
| `src/app/admin/` | Admin portal pages (policies, acknowledgments, compliance, audit, monitoring) |
| `src/components/` | Reusable UI components (forms, layout, policies, acknowledgments, audience) |
| `src/lib/api/` | API client layer (Axios-based service modules) |
| `src/lib/auth/` | Authentication context, session management, role utilities |
| `src/lib/*/hooks.ts` | Feature-specific React Query hooks |
| `src/lib/*/types.ts` | TypeScript type definitions per feature |

---

## Tech Stack

### Backend

| Technology | Version |
|-----------|---------|
| .NET SDK | 10.0 |
| ASP.NET Core | 10 |
| C# | Latest (with .NET 10) |
| Entity Framework Core | 10.x |
| SQL Server | Primary database |
| MediatR | 14.1.0 |
| FluentValidation | 12.1.1 |
| AutoMapper | 16.1.1 |
| Swashbuckle (Swagger) | 10.1.7 |
| Serilog | 10.0.0 |

### Frontend

| Technology | Version |
|-----------|---------|
| Next.js | 16.2.3 |
| React | 19.2.5 |
| TypeScript | 6.0.2 |
| Tailwind CSS | 4.2.2 |
| TanStack React Query | 5.97.0 |
| Axios | 1.15.0 |
| React Hook Form | 7.72.1 |
| Zod | 4.3.6 |
| Lucide React | 1.8.0 |

### Integrations

| System | Purpose |
|--------|---------|
| LDAP / Active Directory | Authentication, user profile sync, audience targeting data |
| Microsoft Exchange | Email notifications (SMTP) |

---

## Project Structure

```
compliance-acknowledgment-system/
├── backend/
│   ├── Eap.sln
│   ├── global.json
│   └── src/
│       ├── Eap.Api/                  # REST API layer
│       │   ├── Authentication/       # Cookie auth, current user service
│       │   ├── Conventions/          # API error conventions
│       │   ├── Extensions/           # Swagger setup
│       │   ├── Features/             # Feature-based controllers
│       │   │   ├── Acknowledgments/
│       │   │   ├── Admin/
│       │   │   ├── Audience/
│       │   │   ├── Audit/
│       │   │   ├── Auth/
│       │   │   ├── Compliance/
│       │   │   ├── Forms/
│       │   │   ├── Notifications/
│       │   │   ├── Policies/
│       │   │   ├── Requirements/
│       │   │   └── UserPortal/
│       │   ├── Middleware/           # Global exception handling
│       │   └── Program.cs           # Application entry point
│       ├── Eap.Application/          # Application logic (CQRS)
│       │   ├── Acknowledgments/      # Commands, queries, DTOs
│       │   ├── Admin/
│       │   ├── Audience/
│       │   ├── Audit/
│       │   ├── Common/              # Shared behaviors, exceptions, models
│       │   ├── Compliance/
│       │   ├── Forms/
│       │   ├── Identity/
│       │   ├── Notifications/
│       │   ├── Policies/
│       │   ├── Requirements/
│       │   └── UserPortal/
│       ├── Eap.Domain/               # Domain entities and enums
│       │   ├── Acknowledgment/
│       │   ├── Audience/
│       │   ├── Audit/
│       │   ├── Common/
│       │   ├── Forms/
│       │   ├── Identity/
│       │   ├── Notifications/
│       │   ├── Policy/
│       │   └── Requirements/
│       └── Eap.Infrastructure/       # Data access and integrations
│           ├── Persistence/          # EF Core DbContext & configurations
│           ├── Identity/             # LDAP auth, user repository, seeding
│           ├── Notifications/        # Exchange email sender
│           ├── Policies/             # Policy document storage
│           ├── Forms/                # Form upload storage
│           └── ...                   # Feature-specific repositories
├── frontend/
│   ├── package.json
│   ├── next.config.ts
│   ├── tsconfig.json
│   └── src/
│       ├── app/
│       │   ├── (user)/               # User portal routes
│       │   │   ├── dashboard/
│       │   │   ├── actions/
│       │   │   ├── history/
│       │   │   └── profile/
│       │   ├── admin/                # Admin portal routes
│       │   │   ├── dashboard/
│       │   │   ├── policies/
│       │   │   ├── acknowledgments/
│       │   │   ├── compliance/
│       │   │   ├── monitoring/
│       │   │   ├── audit/
│       │   │   └── notifications/
│       │   ├── login/
│       │   └── layout.tsx            # Root layout (Arabic-first, RTL)
│       ├── components/
│       │   ├── acknowledgments/
│       │   ├── admin/
│       │   ├── audience/
│       │   ├── forms/
│       │   ├── layout/
│       │   ├── policies/
│       │   ├── recurrence/
│       │   ├── ui/                   # Shared UI primitives (Button, Card, Badge)
│       │   └── user-portal/
│       ├── lib/
│       │   ├── api/                  # API service modules
│       │   ├── auth/                 # Session & role management
│       │   ├── */hooks.ts            # React Query hooks per feature
│       │   ├── */types.ts            # TypeScript types per feature
│       │   └── tokens/               # Design tokens
│       └── styles/
│           └── tokens.css
└── docs/                             # Project documentation
    ├── 00-project-context/
    ├── 01-business/
    ├── 03-functional-requirements/
    ├── 04-solution-design/
    ├── 05-data/
    ├── 09-ux-ui/
    ├── 10-delivery/
    └── 11-engineering/
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS recommended)
- [SQL Server](https://www.microsoft.com/sql-server) (local or remote instance)
- LDAP / Active Directory access (for authentication)

### Backend Setup

```bash
cd backend

# Restore dependencies
dotnet restore

# Configure connection string and LDAP settings
# Edit src/Eap.Api/appsettings.Development.json or use dotnet user-secrets

# Run the API
dotnet run --project src/Eap.Api
```

The API starts at `http://localhost:5100` by default with Swagger UI available at `/swagger` in development mode.

A health check endpoint is available at `GET /health`.

### Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Create environment file
cp .env.example .env.local

# Start development server
npm run dev
```

The frontend starts at `http://localhost:3000` by default.

### Available Frontend Scripts

| Script | Description |
|--------|-------------|
| `npm run dev` | Start Next.js development server |
| `npm run build` | Build for production |
| `npm run start` | Start production server |
| `npm run lint` | Run ESLint |
| `npm run type-check` | Run TypeScript type checking |

---

## Configuration

### Backend Configuration (`appsettings.json`)

| Section | Purpose |
|---------|---------|
| `ConnectionStrings.EapDatabase` | SQL Server connection string |
| `Ldap` | LDAP/AD host, port, SSL, bind credentials, user search filter, attribute mapping |
| `Identity.Provisioning` | Auto-assign End User role on provision, system administrator list |
| `Identity.Seed` | Enable/disable reference data seeding |
| `PolicyDocuments` | File storage root path, max file size (25 MB), allowed extensions (PDF) |
| `FormUploads` | File storage root path, max file size (10 MB), allowed extensions (PDF, images, Office docs) |
| `Exchange` | SMTP host, port, SSL, sender email/name, credentials |
| `Notifications` | Reminder days before due date |
| `Cors.AllowedOrigins` | Allowed frontend origins |
| `Serilog` | Structured logging configuration |

### Frontend Configuration (`.env.local`)

| Variable | Description |
|----------|-------------|
| `NEXT_PUBLIC_API_BASE_URL` | Backend API base URL (default: `http://localhost:5100`) |

---

## API Documentation

Swagger UI is available in development mode at:

```
http://localhost:5100/swagger
```

### Key API Endpoints

| Area | Base Path | Description |
|------|-----------|-------------|
| Authentication | `/api/auth` | Login/logout via LDAP |
| Policies | `/api/policies` | CRUD, versioning, document upload |
| Policy Versions | `/api/policies/{id}/versions` | Version management, publishing |
| Policy Documents | `/api/policies/versions/{id}/documents` | Document upload/download |
| Acknowledgments | `/api/acknowledgment-definitions` | CRUD for acknowledgment definitions |
| Acknowledgment Versions | `/api/acknowledgment-versions` | Version management, publishing |
| Audience | `/api/audience` | Audience configuration and preview |
| Forms | `/api/form-definitions` | Form schema configuration |
| Form Submissions | `/api/form-submissions` | Submission management |
| Requirements | `/api/requirements` | User action requirement management |
| User Portal | `/api/user-portal` | Dashboard, actions, history, submissions |
| Admin Dashboard | `/api/admin/dashboard` | KPIs and monitoring |
| Compliance | `/api/compliance` | Compliance dashboard and reports |
| Audit | `/api/audit` | Audit log listing and export |
| Notifications | `/api/notifications` | Notification listing |

---

## Core Concepts

| Concept | Description |
|---------|-------------|
| **Policy** | An official document that employees must review. Supports multiple versions over time. |
| **Policy Version** | An immutable snapshot of a policy. Published versions cannot be modified. |
| **Acknowledgment Definition** | A business action linked to a policy (e.g., "Annual COI Disclosure"). |
| **Acknowledgment Version** | A specific executable version of an acknowledgment, linked to a policy version, with configured audience, recurrence, and optional form. |
| **Form Definition** | A JSON-based structured form schema attached to an acknowledgment version for form-based disclosures. |
| **Audience Definition** | Configuration defining which users are targeted (all users, department, AD group, exclusions). |
| **User Action Requirement** | The assignment record for a specific user to complete a specific acknowledgment version in a specific cycle. |
| **User Submission** | The recorded completion by a user, including form field values for disclosures. |
| **Recurrence Model** | Defines how often an action recurs: Onboarding Only, Annual, Onboarding + Annual, On Change, Event-Driven. |

### Lifecycle States

**Policy/Acknowledgment Versions:** Draft &rarr; Published &rarr; Superseded &rarr; Archived

**User Action Requirements:** Pending &rarr; Completed | Overdue | Cancelled

---

## User Roles

| Role | Capabilities |
|------|-------------|
| **End User** | View assigned actions, review policies, submit acknowledgments/disclosures, view personal history |
| **Policy Manager** | Create and manage policy records and draft versions |
| **Acknowledgment Manager** | Create and manage acknowledgment definitions, configure audience and recurrence |
| **Publisher** | Publish and archive policy and acknowledgment versions |
| **Compliance Viewer** | Access compliance dashboards, reports, and non-compliant user lists |
| **Auditor** | Review historical records, audit trails, and submission evidence |
| **System Administrator** | Manage system configuration, roles, and operational settings |

Access control uses **Role-Based Access Control (RBAC)** with scoping (Global, Department, Owned Content).

---

## Documentation

Comprehensive project documentation is available in the `docs/` directory:

| Directory | Contents |
|-----------|----------|
| `docs/00-project-context/` | Project overview, technical preferences, implementation rules |
| `docs/01-business/` | Business Requirements Document (BRD) |
| `docs/03-functional-requirements/` | Functional requirements, business rules, user stories, lifecycle models |
| `docs/04-solution-design/` | Solution architecture overview, approved libraries and versions |
| `docs/05-data/` | Conceptual data model |
| `docs/09-ux-ui/` | UX principles, design system tokens, portal page specifications |
| `docs/10-delivery/` | Sprint plan and sprint status |
| `docs/11-engineering/` | Backend developer guide |

---

## UI Design

The platform follows an **Arabic-first, RTL-first** design direction with:

- Clean, institutional visual tone aligned with The Environment Fund brand identity
- Brand color palette anchored in Navy (`#2C3A82`), Dark (`#0F1822`), and White (`#FFFFFF`)
- Secondary accents: Blue (`#0051B1`), Green (`#C0CB6C`), Brown (`#A18E77`), Warm Grey (`#F1EEE8`)
- Tailwind CSS with custom design tokens
- Task-oriented, minimal cognitive load
- Consistent component behavior across User and Admin portals

---

## License

This is a private internal platform developed for The Environment Fund. All rights reserved.
