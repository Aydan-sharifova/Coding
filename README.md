# Coding Platform

Production-oriented Onion Architecture foundation for the Coding API. Business features are intentionally outside the scope of this infrastructure setup.

## Structure

- `src/Coding.Domain` — entities and domain types
- `src/Coding.Application` — DTOs and service contracts
- `src/Coding.Infrastructure` — EF Core, PostgreSQL, migrations, and service implementations
- `src/Coding.Api` — HTTP host, authentication, Swagger, health checks, and logging
- `frontend` — frontend folder architecture without feature implementation

## Local infrastructure

Copy `.env.example` to `.env`, replace all placeholder secrets, then start the services:

```bash
docker compose up --build
```

The API health endpoint is available at `http://localhost:8080/health`.

## Entity Framework

Create migrations from the repository root:

```bash
dotnet ef migrations add MigrationName \
  --project src/Coding.Infrastructure/Coding.Infrastructure.csproj \
  --startup-project src/Coding.Api/Coding.Api.csproj
```

Apply migrations:

```bash
dotnet ef database update \
  --project src/Coding.Infrastructure/Coding.Infrastructure.csproj \
  --startup-project src/Coding.Api/Coding.Api.csproj
```

Secrets must be supplied through environment variables or a production secret manager. Do not commit `.env`.
