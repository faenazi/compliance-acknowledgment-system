# EAP Backend

Enterprise Acknowledgment Platform - Backend API

## Technology Stack

- ASP.NET Core 8.0
- C#
- SQL Server (via Entity Framework Core 8.0.26)
- MediatR 14.1.0
- FluentValidation 12.1.1
- Serilog.AspNetCore 10.0.0
- Swashbuckle.AspNetCore 10.1.7

## Solution Structure

```
backend-claude/
├── EAP.sln
├── src/
│   ├── EAP.Api/              # Presentation layer (controllers, middleware, config)
│   ├── EAP.Application/      # Application layer (handlers, behaviors, validators)
│   ├── EAP.Domain/           # Domain layer (entities, business rules)
│   └── EAP.Infrastructure/   # Infrastructure layer (persistence, integrations)
└── README.md
```

## Architecture

- **Modular Architecture** with clear layer separation
- **Vertical Slice Architecture** for feature organization
- **MediatR** for command/query handling (no business logic in controllers)
- **FluentValidation** with pipeline behavior for automatic validation
- **Serilog** for structured logging
- **Global exception handling** with standardized API error responses

## Prerequisites

- .NET 8.0 SDK
- SQL Server (for database features; not required for foundation endpoints)

## Build and Run

```bash
cd backend-claude

# Restore packages
dotnet restore EAP.sln

# Build
dotnet build EAP.sln

# Run (starts on http://localhost:5100)
cd src/EAP.Api
dotnet run
```

## Verification Commands

```bash
# Restore
dotnet restore EAP.sln

# Build
dotnet build EAP.sln

# Check for vulnerable packages
dotnet list package --vulnerable

# Run
cd src/EAP.Api
dotnet run
```

## Endpoints

| Method | Path               | Description          |
|--------|--------------------|----------------------|
| GET    | /health            | Health check         |
| GET    | /api/systeminfo    | System information   |

## Swagger

When running in Development mode, Swagger UI is available at:

```
http://localhost:5100/swagger
```

## Configuration

- `appsettings.json` - Base configuration (connection string, Serilog, application settings)
- `appsettings.Development.json` - Development overrides

## Package Versions

All NuGet package versions are pinned in `.csproj` files. No floating version ranges are used.
