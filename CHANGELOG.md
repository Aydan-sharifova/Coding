# Changelog

All notable changes follow [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and semantic versioning.

## [1.0.0] - 2026-07-28

### Added

- Production multi-stage, non-root API and frontend images
- Nginx SPA/API/WebSocket reverse proxy and security headers
- PostgreSQL, authenticated Redis, API, frontend, and one-shot migrator Compose services
- PostgreSQL, Redis, API, and frontend health checks
- Named volumes for PostgreSQL, Redis, and avatar persistence
- Explicit development seed data for admin/demo users, project files, tasks, and chat
- Pull-request/main CI and optional tagged GHCR publishing
- Environment, migration, architecture, sequence, security, and deployment documentation

### Changed

- Production migrations and development seeding are disabled by default
- CORS now fails closed when no allowed origin is configured
- Configuration files no longer contain credentials
- Frontend lint gate performs a strict TypeScript project check

### Security

- Added non-root containers, internal data networks, authenticated Redis, read-only edge filesystem, security headers, strict JWT startup validation, and secret-management guidance

[1.0.0]: ./docs/releases/v1.0.0.md
