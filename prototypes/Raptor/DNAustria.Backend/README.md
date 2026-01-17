# DNAustria Backend

.NET 8 Web API for the DNAustria event management system.

## Requirements
- .NET 8 SDK
- PostgreSQL

## Quick start
1. Configure the connection string in `appsettings.json` (or set `ConnectionStrings__DefaultConnection`).
2. Run migrations / create database:
   - dotnet tool install --global dotnet-ef --version 8.0.0
   - cd src\DNAustria.Backend
   - dotnet ef migrations add Initial
   - dotnet ef database update
3. Run the app
   - dotnet run --project src\DNAustria.Backend

## Import example (curl)
POST /api/events/import

curl -X POST "https://localhost:5001/api/events/import" -H "Content-Type: application/json" -d "{\"content\": \"<h1>Sample Event</h1>\n<div>12.01.2026 - 12.01.2026</div>\n<p>Details...</p>\", \"isHtml\": true }"

This will return a parsed `EventCreateDto` (draft). Use the result to POST to `/api/events` to create the event.
## Notes
- A stub LLM parser `StubLLMService` is provided and should be replaced with an integration to a real LLM provider (OpenAI/Azure) in production. Use the `POST /api/events/import` endpoint with a JSON body `{ "content": "...text or html...", "isHtml": true|false }` to parse content into an event draft.
- Address reuse behavior: if `EventCreateDto.LocationId` is specified, the service will attach the existing address.
- The API supports inline creation of `Address` and `Contact` when creating/updating an event using `EventCreateDto.Address` or `EventCreateDto.Contact`.
- Searching events: use `GET /api/events?q=searchTerm` to filter by title or description.
