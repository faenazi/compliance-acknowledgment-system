# Implementation Rules

---

# 1. Purpose

This document defines the **mandatory implementation rules** for building the Enterprise Acknowledgment Platform (EAP).

All developers and AI coding assistants MUST follow these rules to ensure:

- consistency
- maintainability
- scalability
- correctness

---

# 2. Architecture Principles

## 2.1 Architecture Style

The system MUST follow:

- Modular Architecture
- Vertical Slice Architecture (feature-based)

---

## 2.2 Separation of Concerns

The system MUST be structured into:

- Presentation Layer (API / UI)
- Application Layer (Use Cases)
- Domain Layer (Business Logic)
- Infrastructure Layer (Database, LDAP, Email)

---

## 2.3 Feature-Based Structure

Each feature MUST be self-contained.

Each feature SHOULD include:

- Endpoint / Controller
- Request / Response models
- Handler (Use Case)
- Validation
- Data access

---

# 3. Backend Rules (.NET)

## 3.1 API Design

- Use RESTful APIs
- Use DTOs for all requests and responses
- Do NOT expose database entities directly

---

## 3.2 Request Handling

- Use MediatR for all commands and queries
- Each feature MUST follow:
  - Command / Query
  - Handler

---

## 3.3 Validation

- Use FluentValidation
- Validation MUST be separate from controllers
- All inputs MUST be validated

---

## 3.4 Mapping Strategy

- Prefer manual mapping for critical domain logic
- Use AutoMapper ONLY for simple DTO mapping
- Do NOT hide business logic inside mapping profiles

---

## 3.5 Business Logic

- MUST NOT exist in controllers
- MUST be inside application/domain layers

---

## 3.6 Error Handling

- Use centralized error handling
- Return standardized error responses
- Do NOT expose internal exceptions

---

## 3.7 Logging

- Use Serilog for structured logging
- Log:
  - Errors
  - Important actions
  - Integration failures

---

# 4. Data & Database Rules

## 4.1 Database

- SQL Server MUST be used
- Entity Framework Core MUST be used

---

## 4.2 Data Integrity

- Enforce referential integrity
- Prevent duplicate records

---

## 4.3 Versioning

- Only ONE active version per entity
- Historical versions MUST be preserved
- Published data MUST NOT be modified

---

## 4.4 Audit Logging

- All critical actions MUST be logged
- Audit data MUST be immutable

---

# 5. Integration Rules

## 5.1 LDAP / Active Directory

- Authentication MUST be via LDAP / AD
- AD is the source of truth
- User data MUST be retrieved from AD

---

## 5.2 Exchange (Email)

- Email MUST be sent via Exchange
- Use SMTP or service integration
- Failures MUST be logged

---

# 6. Security Rules

## 6.1 Authentication

- All users MUST authenticate via LDAP / AD

---

## 6.2 Authorization

- Use Role-Based Access Control (RBAC)

---

## 6.3 Data Protection

- Do NOT expose sensitive data
- Secure all endpoints

---

# 7. Frontend Rules (Next.js)

## 7.1 Structure

- Use modular structure
- Separate:
  - pages
  - components
  - services

---

## 7.2 API Layer

- All API calls MUST go through a centralized service layer

---

## 7.3 Forms

- Use React Hook Form
- Use Zod for validation

---

## 7.4 UI Consistency

- Follow RTL (Arabic-first)
- Use consistent components

---

# 8. Simplicity Rules (MVP Critical)

## 8.1 No Overengineering

- Do NOT introduce unnecessary abstractions
- Do NOT build future features

---

## 8.2 MVP First

- Build only required features
- Follow requirements strictly

---

## 8.3 Keep It Simple

- Prefer simple solutions
- Optimize later

---

# 9. Performance Guidelines

- Avoid unnecessary DB calls
- Use pagination
- Optimize queries

---

# 10. Coding Standards

- Use clean naming conventions
- Avoid duplication
- Write readable code

---

# 11. AI Coding Rules

- Do NOT invent architecture
- Do NOT introduce new libraries
- Follow requirements strictly
- Follow this document exactly

---

# 12. Non-Negotiable Rules

- No business logic in controllers
- No direct DB access from frontend
- No modification of published data
- No skipping validation
- No skipping audit logging

---

# 13. Backend Libraries (Approved)

The backend MUST use the following libraries:

## Core Libraries

- MediatR
- FluentValidation
- AutoMapper
- Swashbuckle (Swagger)
- Serilog
- Entity Framework Core

---

## Rules

- Do NOT introduce additional libraries without approval
- Do NOT use multiple libraries for the same purpose
- Keep dependencies minimal

---

# 14. Frontend Libraries (Approved)

The frontend MUST use the following libraries:

---

## Core

- next
- react
- react-dom
- typescript

---

## Styling

- tailwindcss
- postcss
- autoprefixer
- clsx
- tailwind-merge

---

## Forms & Validation

- react-hook-form
- zod

---

## Data Fetching

- @tanstack/react-query
- axios

---

## UI Utilities

- lucide-react

---

## Optional (If Needed)

- @tanstack/react-table
- react-hot-toast

---

## Rules

- Do NOT add UI frameworks
- Do NOT mix multiple form libraries
- Keep dependencies minimal
