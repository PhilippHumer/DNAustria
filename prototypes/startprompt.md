# 🧠 Project Specification – *Discover.DNAustria*

> Build this full-stack prototype end-to-end.
> Follow every rule below. Generate runnable backend, frontend, database, and Docker setup.

---

## 🎯 Purpose

Prototype a **complete event management system** for FH Upper Austria to collect, manage and export educational events to the national **DNAustria** platform.
Provide a runnable local demo with:

* Angular SPA frontend
* .NET backend
* PostgreSQL database

Architecture: **Clean Architecture (Domain, Application, Infrastructure, API)**

---

## ⚙️ Constraints

```json
{
  "frontend_framework": "Angular (latest stable)",
  "backend_framework": "dotnet (latest LTS or supported)",
  "database": "PostgreSQL (latest stable)",
  "packages": "Use latest stable package versions",
  "tests": "Optional; integration tests disabled by default via env var"
}
```

---

## 📦 Scope

### Must

* CRUD API for **Events**, **Contacts**, **Organizations**
* Public export endpoint returning **DNAustria-compatible JSON** for Approved/Transferred events
* Angular SPA with list + form UIs for all entities
* Local run descriptor (**docker-compose**) for API, frontend & DB
* Seed data: ≥ 2 events, 1 organization, 1 contact

### Should

* OpenAPI / Swagger documentation
* Client + server validation for required fields
* Map empty client dates to `null` on DTOs

### May

* Role separation (Admin / Editor)
* E2E smoke tests (Playwright / Cypress)
* AI-assisted import (text/HTML → event mapping via LLM/NLP)

---

## 🧱 Domain Model

```json
{
  "Event": {
    "id": "GUID",
    "title": "string (required)",
    "description": "string|null",
    "topics": "list<int>|null",
    "date_start": "datetime|null",
    "date_end": "datetime|null",
    "organization_id": "GUID|null",
    "contact_id": "GUID|null",
    "target_audience": "list<int>|null",
    "is_online": "bool",
    "event_link": "string|null",
    "status": "enum [Draft, Approved, Transferred]",
    "created_by": "string|null",
    "modified_by": "string|null",
    "modified_at": "datetime"
  },
  "Contact": {
    "id": "GUID",
    "name": "string (required)",
    "email": "string|null",
    "phone": "string|null",
    "organization_id": "GUID|null"
  },
  "Organization": {
    "id": "GUID",
    "name": "string (required)",
    "address_street": "string|null",
    "address_city": "string|null",
    "address_zip": "string|null",
    "region_id": "int|null"
  }
}
```

**Rules**

* Editable only when status ∈ [Draft, Approved]
* Auto-set timestamps
* Frontend sends `null` for empty dates

---

## 🔗 API Contract

```json
[
  { "name": "ListEvents", "method": "GET", "path": "/server/api/events", "params": ["filter","search","page","pageSize"], "auth": "internal" },
  { "name": "CreateEvent", "method": "POST", "path": "/server/api/events", "body": "Event", "auth": "internal" },
  { "name": "UpdateEvent", "method": "PUT", "path": "/server/api/events/{id}", "body": "Event", "auth": "internal" },
  { "name": "DeleteEvent", "method": "DELETE", "path": "/server/api/events/{id}", "auth": "internal" },
  { "name": "ImportEvent", "method": "POST", "path": "/server/api/events/import", "body": "raw text/html", "response": "Event", "auth": "internal" },
  { "name": "ListContacts", "method": "GET", "path": "/server/api/contacts", "auth": "internal" },
  { "name": "ListOrganizations", "method": "GET", "path": "/server/api/organizations", "auth": "internal" },
  { "name": "PublicExport", "method": "GET", "path": "/server/api/public/events", "response": "DNAustria export JSON", "auth": "public" }
]
```

Validation:

```json
{
  "required": {
    "Event": ["title"],
    "Contact": ["name"],
    "Organization": ["name"]
  },
  "date_format": "ISO-8601 when present; accept empty as null"
}
```

---

## 📤 Export Schema

Return essential fields for each approved/transferred event:

```json
{
  "event_title": "string",
  "event_description": "string",
  "event_start": "datetime",
  "event_end": "datetime",
  "event_link": "string",
  "event_topics": "array<int>",
  "event_target_audience": "array<int>",
  "event_is_online": "bool",
  "organization_name": "string",
  "event_contact_email": "string",
  "event_contact_phone": "string",
  "event_address_street": "string",
  "event_address_city": "string",
  "event_address_zip": "string",
  "event_address_state": "string",
  "location": "array<float>|null"
}
```

Filter rule: only events with status ∈ [Approved, Transferred].

---

## 🖥️ Frontend Requirements

* Angular Material UI (tables, dialogs, forms)
* List view with filter + search
* Editable reactive forms
* Client-side validation
* Confirmation dialog on delete

---

## ⚙️ Operational Setup

```json
{
  "local_run": {
    "command": "docker-compose up --build",
    "ports": { "frontend": 8080, "backend": 5000 },
    "env": {
      "POSTGRES_USER": "appuser",
      "POSTGRES_PASSWORD": "password",
      "POSTGRES_DB": "discoverdnaustria"
    }
  },
  "integration_tests": { "env_toggle": "INTEGRATION_TESTS", "default": "false" },
  "seed_data": "Include 2 sample events, 1 organization, 1 contact as JSON"
}
```

---

## 📈 Non-Functional Requirements

* API response < 2 s
* Maintainable and easily extendable schema
* Swagger enabled by default
* Internal endpoints require auth or same-origin
* Consistent naming across all layers

---

## 📦 Deliverables

* Backend source (controllers, services, DTOs, EF migrations)
* Frontend source (Angular SPA with forms + lists)
* PostgreSQL schema + seed data
* Docker Compose setup (API + Frontend + DB)
* Swagger/OpenAPI documentation
* Example DNAustria export JSON

---

## 🧠 Agent Guidelines

```json
{
  "design_freedom": "Agent may choose structure within constraints",
  "non_functional_musts": [
    "Use PostgreSQL for persistence",
    "Frontend must be Angular",
    "Backend must be .NET",
    "No secrets in code"
  ],
  "testing": "Optional; integration tests off unless INTEGRATION_TESTS=true",
  "output_format": {
    "code_edits": "return machine-readable diff",
    "run_steps": "return exact shell commands"
  }
}
```

---

## ✅ Success Criteria

* `POST /server/api/events` with minimal data → `201 Created` and GUID returned
* `GET /server/api/public/events` → array matching export schema
* Frontend accessible at `http://localhost:8080` listing seeded events

---

💡 **Usage Tip for Cursor / VS Code AI**

1. Open a new project folder.
2. Paste this prompt into `PROMPT.md`.
3. Run **“Build project from spec”** or `/new` in the composer.
4. The agent should scaffold backend, frontend, migrations, Docker setup & seed data automatically.

---