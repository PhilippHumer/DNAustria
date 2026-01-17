# Discover.DNAustria Prototype

## Stack
- .NET 8 Web API (Clean Architecture layers)
- Angular 21 Material SPA
- PostgreSQL 16
- Docker Compose for local run

## Quick Start

```bash
docker-compose up --build
```

Frontend: http://localhost:8080
Backend Swagger: http://localhost:5100/swagger
Public Export: http://localhost:5100/server/api/public/events
Health: http://localhost:5100/server/api/public/health

## API Endpoints
- CRUD: /server/api/events, /server/api/contacts, /server/api/organizations
- Import: POST /server/api/events/import
- Public Export: GET /server/api/public/events

## Environment Variables
- DATABASE_CONNECTION
- INTERNAL_API_KEY
- INTEGRATION_TESTS
