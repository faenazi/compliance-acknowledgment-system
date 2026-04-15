# Implementation Rules

## 1. Purpose

This document defines the mandatory implementation rules for building the Enterprise Acknowledgment Platform (EAP).

All developers and AI coding assistants must follow these rules to ensure:

- consistency
- maintainability
- scalability
- correctness
- controlled technology usage

---

## 2. Architecture Principles

### 2.1 Architecture Style

The backend must follow:

- Modular Architecture
- Vertical Slice Architecture

### 2.2 Separation of Concerns

The solution must separate:

- Presentation Layer
- Application Layer
- Domain Layer
- Infrastructure Layer

### 2.3 Feature-Based Structure

Each feature must be self-contained.

Examples:

- Policy
- Acknowledgment
- User
- Compliance
- Notification
- Audit

Each feature should include:

- endpoints
- requests and responses
- validators
- handlers
- domain logic
- persistence logic

---

## 3. Backend Rules (.NET)

### 3.1 Framework

- Use ASP.NET Core 10 only
- Do not use preview or beta .NET packages

### 3.2 Request Handling

- Use MediatR for commands and queries
- Each use case must be modeled explicitly
- Do not put application logic directly in controllers

### 3.3 Validation

- Use FluentValidation for request validation
- Every command and query input model must have validation where applicable
- Validation must be executed before business logic

### 3.4 Mapping

- Use AutoMapper for simple DTO and response mapping only
- Do not hide business rules inside AutoMapper profiles
- For domain-critical transformations, prefer explicit mapping

### 3.5 API Design

- Use RESTful APIs
- Use DTOs for all request and response contracts
- Do not expose EF entities directly
- Return standardized error responses

### 3.6 Error Handling

- Use centralized exception handling
- Do not expose internal stack traces
- Log all unhandled exceptions

### 3.7 Logging

- Use Serilog for structured application logging
- Log:
  - errors
  - warnings
  - important operational events
  - integration failures

### 3.8 OpenAPI

- Use Swagger / Swashbuckle for API documentation
- API documentation must be enabled in development
- API contracts must remain aligned with DTOs and validation rules

---

## 4. Data & Database Rules

### 4.1 Database

- Use SQL Server as the primary database
- Use Entity Framework Core for data access

### 4.2 Integrity

- Enforce referential integrity
- Prevent duplicate active records where the domain requires uniqueness
- Preserve historical records

### 4.3 Versioning

- Only one active published version is allowed per policy
- Only one active published version is allowed per acknowledgment definition
- Published versions must not be modified in place
- Changes to published content require a new version

### 4.4 Audit Logging

- All critical actions must be recorded
- Audit logs must be immutable from the application layer
- Application logging and audit logging are separate concerns

### 4.5 Form-Based Acknowledgments

- Form definitions must be version-bound
- The form definition must be stored at the acknowledgment version level
- If a form changes materially, a new acknowledgment version must be created
- Submissions must remain linked to the exact version used at the time of submission
- The first release must support controlled JSON-based form definitions
- The first release must not include a full visual form builder

---

## 5. Integration Rules

### 5.1 LDAP / Active Directory

- Authentication must be performed against LDAP / Active Directory
- AD is the source of truth for:
  - username
  - display name
  - email
  - department
  - job title
  - group memberships
- Local user records may cache AD attributes but must not override AD identity data

### 5.2 Exchange

- Email delivery must use Exchange integration
- Delivery failures must be logged
- Notification sending must not fail silently

---

## 6. Security Rules

### 6.1 Authentication

- All users must authenticate through LDAP / Active Directory

### 6.2 Authorization

- Use role-based access control
- Users must access only the features and actions assigned to their role

### 6.3 Data Protection

- Do not expose sensitive internal data in API responses
- Validate all inputs
- Secure all endpoints

---

## 7. Frontend Rules (Next.js)

### 7.1 Framework

- Use Next.js 16.2.3
- Use React 19.2.5
- Use React DOM 19.2.5
- Use TypeScript 6.0.2

### 7.2 Structure

- Use feature-oriented structure where possible
- Separate:
  - routes/pages
  - reusable components
  - form schemas
  - API service layer
  - feature-specific hooks

### 7.3 Styling

- Use Tailwind CSS 4.2.2
- Use @tailwindcss/postcss 4.2.2
- Use PostCSS 8.5.9
- Use clsx 2.1.1 and tailwind-merge 3.5.0 for class composition
- Follow Arabic-first RTL-first layout rules

### 7.4 Forms & Validation

- Use React Hook Form 7.72.1
- Use Zod 4.3.6
- Form validation rules should align with backend validation rules as closely as possible

### 7.5 Data Fetching

- Use TanStack Query 5.97.0 for server state
- Use a centralized HTTP client based on Axios 1.15.0

### 7.6 UI Libraries

- Use lucide-react 1.8.0 for icons
- Do not introduce additional UI frameworks without approval

---

## 8. Simplicity Rules (MVP Critical)

### 8.1 No Overengineering

- Do not introduce unnecessary abstractions
- Do not build future features before they are needed

### 8.2 MVP First

- Build only the approved MVP scope
- Do not implement enforcement logic in Phase 1

### 8.3 Keep It Simple

- Prefer clear and explicit code
- Optimize later when there is a proven need

---

## 9. Performance Guidelines

- Avoid unnecessary database round trips
- Use pagination for list endpoints
- Keep dashboard and list queries efficient
- Avoid loading large documents into memory unnecessarily

---

## 10. Coding Standards

- Use consistent naming conventions
- Write readable code
- Avoid duplication
- Keep handlers focused and small
- Keep domain logic explicit

---

## 11. AI Coding Rules

When using AI coding assistants:

- do not invent architecture
- do not add libraries outside the approved list
- do not change package versions without explicit approval
- do not implement features outside documented scope
- do not add hidden assumptions that are not documented

---

## 12. Approved Backend Libraries and Versions

Use only the following backend libraries unless explicitly approved otherwise:

- ASP.NET Core 10
- MediatR 14.1.0
- FluentValidation 12.1.1
- AutoMapper 16.1.1
- Swashbuckle.AspNetCore 10.1.7
- Serilog.AspNetCore 10.0.0

If Entity Framework Core is used, use:

- Microsoft.EntityFrameworkCore 10.x
- Microsoft.EntityFrameworkCore.SqlServer 10.x
- Microsoft.EntityFrameworkCore.Design 10.x

Rules:

- all package versions must be pinned in `.csproj`
- no floating versions allowed
- no preview packages allowed
- package versions must not be upgraded without explicit approval

---

## 13. Approved Frontend Libraries and Versions

Use only the following frontend libraries unless explicitly approved otherwise:

### Core
- next 16.2.3
- react 19.2.5
- react-dom 19.2.5
- typescript 6.0.2

### Styling
- tailwindcss 4.2.2
- @tailwindcss/postcss 4.2.2
- postcss 8.5.9
- clsx 2.1.1
- tailwind-merge 3.5.0

### Forms & Validation
- react-hook-form 7.72.1
- zod 4.3.6

### Data Fetching
- @tanstack/react-query 5.97.0
- axios 1.15.0

### UI Utilities
- lucide-react 1.8.0

Rules:

- all package versions must be pinned in `package.json`
- do not use `latest`
- do not add another form library
- do not add another state management library for MVP
- do not add another icon library
- package versions must not be upgraded without explicit approval

---

## 14. Non-Negotiable Rules

- no business logic in controllers
- no direct database access from the frontend
- no modification of published versions
- no skipping validation
- no skipping audit logging
- no unapproved package additions
- no unapproved version upgrades
