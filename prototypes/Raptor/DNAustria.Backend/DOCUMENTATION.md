# DNAustria Backend — Documentation 📚

## Overview ✨
This repository contains the .NET 8 Web API backend for the DNAustria event management system. It provides CRUD operations for Events, Addresses, Contacts, and a text/HTML import endpoint that uses a pluggable LLM parser service.

## Quick start (local) ▶️
1. Ensure .NET 8 SDK is installed.
2. Start PostgreSQL (docker recommended):

```bash
# from repo root
docker compose up -d
```

3. Configure connection string (optional): edit `src/DNAustria.Backend/appsettings.json` or set env var `ConnectionStrings__DefaultConnection`.
4. Apply migrations and run:

```powershell
cd src\DNAustria.Backend
dotnet tool install --global dotnet-ef --version 8.0.0 # if not installed
dotnet ef migrations add Initial
dotnet ef database update
dotnet run
```

> Note: On startup the app will automatically apply pending migrations and seed sample data when using a relational database (this is idempotent and only runs when the tables are empty).
> 
> - To disable automatic seeding, set the environment variable `SEED_SAMPLE_DATA=false` or add `SeedSampleData: false` to your configuration (e.g., `appsettings.json`).
> - In Docker, set the same env var for the `app` service in `docker-compose.yml` (see below) to skip seeding when the container starts.

5. Open Swagger UI: https://localhost:<port>/swagger (Development only).

---

## Project layout 🗂️
- `src/DNAustria.Backend/` - main Web API project
  - `Controllers/` - API controllers
  - `Data/` - EF Core `AppDbContext`
  - `Models/` - domain models and enums
  - `Dtos/` - request/response DTOs
  - `Services/` - business logic and LLM parser (stub)
  - `Mapping/` - AutoMapper profiles

---

## Running with Docker Compose (DB only) 🐳
The included `docker-compose.yml` starts PostgreSQL 17. The app runs locally and connects to the DB using the connection string in `appsettings.json`.

---

## LLM import endpoint & parser 🧠
The import endpoint (`POST /api/events/import`) accepts unstructured text or HTML and returns a parsed `EventCreateDto` (draft). A stub implementation `StubLLMService` is provided for prototyping and should be replaced with a production LLM integration (OpenAI/Azure, etc.).

Integration checklist:
- Add secrets via environment variables or key vault.
- Implement `ILLMService` to call the provider and map structured JSON to `EventCreateDto`.
- Add retry logic and rate-limiting safeguards.
- Validate parsed output before saving to DB.

---

## Data-model notes & behavior 🔧
- Event stores lists `TargetAudience` and `Topics` as JSON columns (EF conversions).
- Address reuse: when creating/updating an Event, the system:
  - Uses `LocationId` if provided to link to an existing address.
  - If inline address is provided, tries to find an existing address by `Zip + Latitude + Longitude`. If not found, creates a new Address entity.
- Contact reuse: similar to address—`ContactId` takes precedence; inline contact creates a new Contact.
- Events with status `Transferred` are immutable (cannot update or change status).

---

## Validation & Security (recommendations) ⚠️
- Add FluentValidation or similar to validate DTOs thoroughly (dates, required fields, email format, etc.).
- Add authentication (JWT / IdentityServer) and authorization to protect modifying endpoints.
- Sanitize and limit uploaded content for the import endpoint to mitigate injection risks.

---

## Tests & CI suggestions ✅
- Unit tests for `EventService`, `AddressService`, `ContactService`, and `StubLLMService` logic.
- Integration tests using a test PostgreSQL instance (docker) and EF migrations.
- Add a GitHub Actions pipeline that runs `dotnet build`, tests, and optionally `dotnet ef migrations` checks.

---

## Next steps I can help with 👇
- Integrate a real LLM provider (OpenAI/Azure) with examples and env var configuration.
- Add test coverage and sample CI pipeline.
- Create Postman collection or OpenAPI client snippets for frontend integration.

---

If you want any of the next steps implemented, tell me which one and I'll add it. — GitHub Copilot

---

## Docker & ports
- When running locally with `dotnet run` the app listens on **HTTP 5000** and **HTTPS 5001** by default (HTTPS requires a valid developer certificate).
- `docker compose up -d` starts the `db` and `app` services. The `app` service listens on **HTTP 5000** (mapped to host port 5000) by default.
- To skip seeding sample data when running in Docker, add the environment variable to the `app` service in `docker-compose.yml`:

```yaml
services:
  app:
    environment:
      - SEED_SAMPLE_DATA=false
```

- HTTPS inside the container is disabled by default because containers don't have the developer cert. To enable HTTPS in Docker:
  1. Generate a PFX certificate and a password (or reuse an existing one).
  2. Mount it into the container (example):
     - Add a bind mount in `docker-compose.yml`:
       `- ./certs/aspnet/https.pfx:/secrets/aspnet/https.pfx:ro`
     - Set these environment variables for `app` in `docker-compose.yml`:
       - `ENABLE_HTTPS=true`
       - `ASPNETCORE_Kestrel__Certificates__Default__Path=/secrets/aspnet/https.pfx`
       - `ASPNETCORE_Kestrel__Certificates__Default__Password=yourpassword`
     3. Add port mapping for 5001 (`- "5001:5001"`) and restart the stack.

## Notes about dev certs
- For local development you can generate and trust the ASP.NET Core dev certificate on your host (`dotnet dev-certs https --trust`) and run `dotnet run` (not inside Docker) to get HTTPS on 5001.
