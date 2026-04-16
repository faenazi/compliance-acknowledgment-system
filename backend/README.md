# EAP Backend Foundation

This folder contains a fresh backend foundation for the Enterprise Acknowledgment Platform (EAP).

## Architecture and Scope

The backend is intentionally minimal and focuses on a clean, runnable baseline:

- ASP.NET Core Web API (`Eap.Api`)
- Application layer with MediatR + FluentValidation (`Eap.Application`)
- Domain layer for core entities (`Eap.Domain`)
- Infrastructure layer for EF Core SQL Server persistence (`Eap.Infrastructure`)

A single small sample feature is included:

- `GET /api/v1/system/echo?message=...`

This proves end-to-end wiring:

- endpoint -> MediatR request -> validation pipeline -> handler -> JSON response

## Solution Structure

```text
backend/
  Eap.sln
  README.md
  src/
    Eap.Api/
    Eap.Application/
    Eap.Domain/
    Eap.Infrastructure/
```

## Prerequisites

- .NET SDK 10.x
- SQL Server instance (local or remote)

## Configuration

Edit `src/Eap.Api/appsettings.json` as needed:

- `ConnectionStrings:DefaultConnection`
- `Infrastructure:EnableSensitiveDataLogging`
- `Serilog` section

## Build and Run

From `backend/`:

```bash
dotnet restore Eap.sln
dotnet build Eap.sln
dotnet run --project src/Eap.Api/Eap.Api.csproj
```

## Verification Commands

From `backend/`:

```bash
dotnet restore Eap.sln
dotnet build Eap.sln
dotnet run --project src/Eap.Api/Eap.Api.csproj
dotnet list Eap.sln package --vulnerable
```

## Runtime Checks

When the API is running in Development:

- Swagger UI: `http://localhost:5000/swagger` or `https://localhost:5001/swagger`
- Health endpoint: `GET /health`
- Sample endpoint: `GET /api/v1/system/echo?message=hello`
