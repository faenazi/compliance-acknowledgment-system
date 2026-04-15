# EAP Backend (Sprint 0)

Foundation skeleton for the Enterprise Acknowledgment Platform backend.

## Stack

- ASP.NET Core 10
- C# / .NET 10
- MediatR 14.1.0
- FluentValidation 12.1.1
- AutoMapper 16.1.1
- Swashbuckle.AspNetCore 10.1.7
- Serilog.AspNetCore 10.0.0
- Entity Framework Core 10.0.0 (SQL Server provider)

All package versions are pinned per `docs/04-solution-design/libraries-and-packages.md`.

## Architecture

Modular + Vertical Slice. Layer separation:

```
Eap.Api              ASP.NET Core host, controllers, API conventions, Swagger, Serilog
Eap.Application      MediatR handlers, FluentValidation, AutoMapper, cross-cutting behaviors
Eap.Domain           Domain entities and business rules
Eap.Infrastructure   EF Core DbContext, SQL Server, AD/Exchange clients (wired in later sprints)
Eap.Modules          Feature modules (empty in Sprint 0; one folder per business module later)
```

## Run

```
dotnet restore
dotnet build
dotnet run --project src/Eap.Api
```

Swagger UI: `https://localhost:7100/swagger` (Development only).
Health probe: `GET /health`.

## Sprint 0 Scope

Sprint 0 delivers only the skeleton. No business features, entities, or
endpoints are implemented yet. See `docs/10-delivery/sprint-plan.md`.
