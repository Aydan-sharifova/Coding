# Database startup and migrations

## Development

Start PostgreSQL, configure `ConnectionStrings__Default`, and run:

```bash
dotnet ef database update \
  --project src/Coding.Infrastructure \
  --startup-project src/Coding.Api
```

Development-only sample data is opt-in. It requires `ASPNETCORE_ENVIRONMENT=Development`, `Database__SeedDevelopmentData=true`, and passwords supplied through `DevelopmentSeed__AdminPassword` and `DevelopmentSeed__DemoPassword`.

## Production strategy

Normal API startup never migrates the database. Deployments use a single, controlled migration job:

```bash
docker compose --profile operations run --rm migrate
```

The `--migrate` API mode applies pending EF migrations, seeds only built-in non-secret roles, logs success, and exits. It never creates development users in Production.

Before executing:

1. Generate and review an idempotent SQL script.
2. Identify locks, table rewrites, backfills, and compatibility requirements.
3. Test against a recent sanitized backup at production scale.
4. Take and verify a recoverable backup.
5. Stop if the migration is not backward-compatible with currently running API instances.

After executing, record the migration IDs, deployment version, operator, timestamps, and result. Prefer expand/migrate/contract changes for zero-downtime releases. Rollback usually means deploying compatible code and restoring data or applying a reviewed forward-fix; do not assume `dotnet ef database update <old>` is data-safe.

## Backup and recovery

Automate encrypted PostgreSQL backups, test restoration regularly, and define recovery-point/recovery-time objectives. The Compose volume is persistence, not a backup. Redis contains realtime/cache state and can normally be recreated, while PostgreSQL and avatar storage require backup policies.
