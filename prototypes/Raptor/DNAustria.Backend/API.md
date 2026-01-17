# API Reference — DNAustria Backend 🚀

Base URL: `https://{host}/api`

## Events

### GET /api/events
List events with optional filters.

Query parameters:
- `status` (optional): `Draft` | `Approved` | `Transferred`
- `q` (optional): search term (title or description)

Response: 200 OK
```json
[
  { "id": "guid", "title": "string", "dateStart": "2026-01-12T...Z", "dateEnd": "2026-01-12T...Z", "status": "Draft" }
]
```

---

### GET /api/events/{id}
Get event details.

Response: 200 OK
`EventDetailDto` (see DTOs section)

---

### POST /api/events
Create an event. Accepts `EventCreateDto` (JSON). Supports inline creation of Address and Contact.

Example payload:
```json
{
  "title": "Sample Event",
  "description": "Details...",
  "dateStart": "2026-01-12T10:00:00Z",
  "dateEnd": "2026-01-12T12:00:00Z",
  "address": { "locationName": "Main Hall", "city": "Linz", "zip": "4020", "street": "Universitaetsstrasse 1" },
  "contact": { "name": "John Doe", "email": "john@example.com" }
}
```

Response: 201 Created with `EventDetailDto`.

---

### PUT /api/events/{id}
Update an event (not allowed if status == `Transferred`). Body: `EventCreateDto`.

Response: 200 OK with updated `EventDetailDto` or 404 if not found.

---

### DELETE /api/events/{id}
Delete an event.

Response: 204 No Content or 404 if not found.

---

### PATCH /api/events/{id}/status?status=Approved
Update event status. Returns 204 No Content. Will return 400 if update is not allowed (already transferred).

---

### POST /api/events/import
Parse unstructured text/HTML into `EventCreateDto` using LLM parser.

Request body:
```json
{ "content": "<h1>Event</h1><p>12.01.2026 - 12.01.2026</p>", "isHtml": true }
```

Response: 200 OK — parsed `EventCreateDto` (draft)

---

## Addresses

Standard CRUD at `/api/organization`.
- GET `/api/organization` — list
- GET `/api/organization/{id}` — get
- POST `/api/organization` — create
- PUT `/api/organization/{id}` — update
- DELETE `/api/organization/{id}` — delete

---

## Contacts

Standard CRUD at `/api/contacts`.
- GET `/api/contacts`
- GET `/api/contacts/{id}`
- POST `/api/contacts`
- PUT `/api/contacts/{id}`
- DELETE `/api/contacts/{id}`

---

## DTOs (summary)

EventCreateDto highlights:
- `title`, `description`, `dateStart`, `dateEnd`, `classification`, `fees`, `isOnline`, `targetAudience[]`, `topics[]`
- `locationId` (link existing Address) OR `address` (inline AddressCreateDto)
- `contactId` OR `contact` (inline ContactCreateDto)
- `status` (Draft by default)

EventDetailDto: full event representation with embedded `Address` and `Contact` when present.

AddressCreateDto:
- `locationName`, `city`, `zip`, `state`, `street`, `latitude`, `longitude`

ContactCreateDto:
- `name`, `org`, `email`, `phone`

---

## Error handling
- 400 Bad Request: invalid payload or business rule (e.g., status change not allowed)
- 404 Not Found: resource missing
- 500 Internal Server Error: unhandled exceptions (improve with ProblemDetails and better logging)

---

## Notes for frontend integrators 💡
- Use `GET /api/events?q=term` for search-as-you-type behavior.
- Use import endpoint to prefill the create form from an event page (HTML) fetched by frontend or uploaded by user.
- Confirm delete and status changes in the UI before calling API.

---

If you want, I can also generate a Postman collection or OpenAPI spec file for direct consumption by frontend tooling. — GitHub Copilot