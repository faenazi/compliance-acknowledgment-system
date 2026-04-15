# Technical Preferences

## 1. Purpose

This document defines the approved technology stack and technical direction for the Enterprise Acknowledgment Platform (EAP).

It serves as the official reference for:

- framework selection
- package decisions
- version constraints
- development direction

---

## 2. Technical Direction

The platform must be built as an internal enterprise web application with:

- ASP.NET Core backend
- Next.js frontend
- SQL Server database
- LDAP / Active Directory integration
- Exchange integration
- Arabic-first RTL-first UI

The design goal is:

- fast MVP delivery
- controlled complexity
- maintainable structure
- future scalability

---

## 3. Approved Backend Stack

### Framework
- ASP.NET Core 10

### Language
- C#

### Architecture
- Modular Architecture
- Vertical Slice Architecture

### API Style
- RESTful APIs
- JSON request/response contracts

### Data Access
- Entity Framework Core
- SQL Server

### Core Backend Packages
- MediatR 14.1.0
- FluentValidation 12.1.1
- AutoMapper 16.1.1
- Swashbuckle.AspNetCore 10.1.7
- Serilog.AspNetCore 10.0.0

### Optional Backend Package Family
- Microsoft.EntityFrameworkCore 10.x
- Microsoft.EntityFrameworkCore.SqlServer 10.x
- Microsoft.EntityFrameworkCore.Design 10.x

### Backend Package Policy
- use stable versions only
- pin every package version in `.csproj`
- do not use floating ranges
- do not upgrade packages without approval

---

## 4. Approved Frontend Stack

### Framework
- Next.js 16.2.3
- React 19.2.5
- React DOM 19.2.5

### Language
- TypeScript 6.0.2

### Styling
- Tailwind CSS 4.2.2
- @tailwindcss/postcss 4.2.2
- PostCSS 8.5.9
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

### Frontend Package Policy
- pin all versions in `package.json`
- do not use `latest`
- do not add alternative libraries for the same purpose without approval

---

## 5. Authentication & Identity

### Authentication Source
- LDAP / Active Directory

### Identity Data
AD is the source of truth for:
- username
- full name
- email
- department
- job title
- group memberships

### Local Profile Strategy
- create a local user profile on first successful login
- refresh AD-derived attributes on login or scheduled sync
- do not allow local overwrite of identity attributes sourced from AD

---

## 6. Email & Notification Technology

### Email Source
- Microsoft Exchange

### Delivery Options
- SMTP
- service-based Exchange integration when needed

### Rules
- email failures must be logged
- notification sending must support retries in application logic if required later

---

## 7. Data & Storage Strategy

### Database
- SQL Server is the primary transactional database

### Versioned Entities
The following entities are version-controlled:
- Policy
- Acknowledgment

### File Storage
Policy documents are stored as files, initially as uploaded PDFs

### Form-Based Acknowledgment Storage
For form-based acknowledgments:
- the form definition shall be stored at the acknowledgment version level
- user submissions shall be stored against the user acknowledgment record
- submitted data must remain historically traceable even if future versions are published later

---

## 8. Logging & Audit Strategy

### Application Logging
- use Serilog for structured logs
- collect:
  - errors
  - warnings
  - operational events
  - integration failures

### Audit Logging
- maintain audit records separately from application logs
- store audit records in the application database
- treat audit records as immutable

---

## 9. UI Direction

### Language
- Arabic-first

### Layout
- RTL-first

### UX Direction
- simple and task-focused
- minimal cognitive load
- optimized for internal enterprise use
- consistent behavior across User Portal and Admin Portal

---

## 10. Development Strategy

### MVP First
- implement only documented MVP requirements
- avoid enforcement logic in Phase 1

### Controlled Growth
- architecture must support future additions
- future phases may introduce:
  - enforcement
  - exceptions
  - advanced rules
  - advanced workflows

### Simplicity Principle
- choose the simplest solution that satisfies the requirement
- do not add complexity early

---

## 11. Version Strategy

### General Rules
- use only stable versions
- avoid preview and beta versions
- keep versions explicit and pinned

### Backend
- all NuGet packages must be version-pinned in `.csproj`
- no floating versions
- upgrades require approval and compatibility review

### Frontend
- all npm packages must be version-pinned in `package.json`
- no floating versions
- upgrades require approval and compatibility review

### AI Coding Constraint
When generating code:
- use only the approved stack
- use only the approved versions
- do not introduce additional packages
- do not upgrade package versions automatically

---

## 12. Constraints

- do not change the approved architecture
- do not introduce new frameworks without approval
- do not add package alternatives that duplicate existing responsibilities
- keep the stack controlled and predictable

---

## 13. Approved Versions

### Backend
- ASP.NET Core 10
- MediatR 14.1.0
- FluentValidation 12.1.1
- AutoMapper 16.1.1
- Swashbuckle.AspNetCore 10.1.7
- Serilog.AspNetCore 10.0.0

### Frontend
- Next.js 16.2.3
- React 19.2.5
- React DOM 19.2.5
- TypeScript 6.0.2
- Tailwind CSS 4.2.2
- @tailwindcss/postcss 4.2.2
- PostCSS 8.5.9
- react-hook-form 7.72.1
- zod 4.3.6
- @tanstack/react-query 5.97.0
- axios 1.15.0
- clsx 2.1.1
- tailwind-merge 3.5.0
- lucide-react 1.8.0

### Version Policy
- use stable versions only
- pin all versions explicitly
- do not use preview, beta, or floating versions
- do not upgrade versions without explicit approval
