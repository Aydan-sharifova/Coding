# Public Demo Environment

The demo runs as a separate deployment with its own PostgreSQL database,
Redis data, API environment, and frontend build. `DemoMode` fails closed unless:

- `ASPNETCORE_ENVIRONMENT=Demo`;
- `DemoMode:Enabled=true`; and
- the configured database name contains the configured `Demo` marker.

This prevents the seed and reset services from operating against the production
database.

## Start the demo

Create a local environment file and replace every example secret:

```bash
cp .env.demo.example .env.demo
```

Build, migrate, seed, and start the isolated stack:

```bash
docker compose --env-file .env.demo -f docker-compose.demo.yml up --build -d
```

Open `http://localhost:8081/demo` and choose Owner, Admin, or Member. The
frontend never receives or stores a demo password. The server maps the selected
role to a predefined demo identity and issues a short-lived session.

## Seed and reset commands

Run the idempotent seed:

```bash
docker compose --env-file .env.demo -f docker-compose.demo.yml run --rm demo-seed
```

Restore the original Nebula Commerce Platform data:

```bash
docker compose --env-file .env.demo -f docker-compose.demo.yml --profile operations run --rm demo-reset
```

The API also performs the same reset automatically at the configured interval.
Reset operations use a process semaphore, a PostgreSQL advisory transaction
lock, deterministic demo ownership IDs, and structured logs.

## Demo safeguards

- Demo users receive only the normal `User` system role. Owner/Admin/Member are
  project roles; the demo Admin cannot access platform administration.
- Registration, password and security changes, system administration, deletes,
  invitations, and project-member changes are rejected in Demo Mode.
- AI endpoints are rate limited.
- Demo uploads default to 1 MB and reject executable/package file types.
- Demo access tokens default to 20 minutes; refresh sessions default to 2 hours.
- The demo database and volumes are named separately from production resources.

To change limits, use the `DemoMode__*` environment variables in
`docker-compose.demo.yml`. Never enable `DemoMode` in a production deployment.
