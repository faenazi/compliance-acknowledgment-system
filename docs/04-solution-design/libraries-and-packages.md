# Libraries and Packages

## Purpose

This document defines the approved libraries and exact versions for the EAP platform.

All generated code must follow this file.

---

## Backend (.NET)

- ASP.NET Core 10
- MediatR 14.1.0
- FluentValidation 12.1.1
- AutoMapper 16.1.1
- Swashbuckle.AspNetCore 10.1.7
- Serilog.AspNetCore 10.0.0

If Entity Framework Core is used:
- Microsoft.EntityFrameworkCore 10.x
- Microsoft.EntityFrameworkCore.SqlServer 10.x
- Microsoft.EntityFrameworkCore.Design 10.x

Rules:
- use exact versions where possible
- do not use preview packages
- do not use floating package ranges

---

## Frontend (Next.js)

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
- all versions must be pinned in `package.json`
- do not use `latest`
- do not add unapproved packages
- do not upgrade package versions without explicit approval
