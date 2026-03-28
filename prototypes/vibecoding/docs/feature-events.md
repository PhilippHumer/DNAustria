# Feature Slice Template (Backend)

## Context

### Projekt

DNAustria ist eine Plattform zur Verwaltung von Veranstaltungen in Österreich.

Technologie-Stack:

* Frontend: Angular
* Backend: .NET
* Datenbank: PostgreSQL

Das Projekt verwendet ein Monorepo:

```text
frontend   → Angular SPA
backend    → .NET Backend
infra      → Docker Compose
docs       → Dokumentation
```

---

### Backend Architektur

```text
src/Domain
src/Application
src/Infrastructure
src/Api
tests/Api.Tests
```

---

### Frontend Struktur

```text
core
shared
features
```

---

### Infrastruktur

```text
infra/docker-compose.yml
```

---

## Feature Kontext

Implementierung eines neuen Backend Feature Slice für die Entität **Event**.

Der Slice dient als Referenzimplementierung für zukünftige Entitäten.

---

## Domain Model

Entity: Event

### Felder

| Feld            | Typ                   |
| --------------- | --------------------- |
| id              | GUID                  |
| title           | string (max 255)      |
| description     | string                |
| event_link      | string (optional)     |
| target_audience | list[enum]            |
| topics          | list[enum]            |
| date_start      | datetime              |
| date_end        | datetime              |
| classification  | enum                  |
| fees            | bool                  |
| is_online       | bool                  |
| organization_id | GUID                  |
| program_name    | string (optional)     |
| format          | string (optional)     |
| school_bookable | bool (optional)       |
| age_minimum     | int (optional)        |
| age_maximum     | int (optional)        |
| location_id     | GUID (optional)       |
| contact_id      | GUID (optional)       |
| status          | enum (Default: Draft) |
| created_by      | string (optional)     |
| modified_by     | string (optional)     |
| created_at      | datetime              |
| modified_at     | datetime (optional)   |
| is_deleted      | bool (Default: false) |

---

### Enums

TargetAudience:
10 Preschool
20 PrimarySchool
30 SecondaryI
40 Vocational
50 SecondaryII
60 Adults
70 Families
80 GirlsWomenOnly

EventTopic:
100 DigitalizationAI
200 ArtsCulture
300 LanguagesLiterature
400 MedicineHealth
500 HistorySociety
600 EconomyLaw
700 ScienceEnvironment
800 MathematicsData

Classification:
Scheduled
OnDemand

EventStatus:
Draft
Approved
Transferred

---

## Fachliche Regeln

* Pflichtfelder sind laut XML definiert
* `created_at` wird automatisch gesetzt
* `modified_at` wird bei Änderungen gesetzt

### Statusübergänge

* Draft → Approved
* Approved → Draft
* Approved → Transferred

### Public Definition

Ein Event ist öffentlich, wenn:

```text
status = Approved
```

---

## Soft Delete

```text
is_deleted = true
modified_at = now()
```

Regeln:

* keine physische Löschung
* gelöschte Events erscheinen in keinem GET
* Zugriff → 404

---

# Intent

Implementierung eines vollständigen Feature Slice:

* Domain Model
* Datenbank
* CRUD API
* Status Update
* Public Endpoint
* Validierung
* Tests

---

# Constraints

## Architektur

```text
Domain
Application
Infrastructure
API
```

---

## API Regeln

```text
GET    /api/events
GET    /api/events/{id}
GET    /api/events?title={title}
POST   /api/events
PUT    /api/events/{id}
DELETE /api/events/{id}
PATCH  /api/events/{id}/status
GET    /api/public/events
```

---

## HTTP Codes

```text
200 OK
201 Created
204 NoContent
400 BadRequest
404 NotFound
409 Conflict
```

---

## Validierung

* Strings trimmen
* Pflichtfelder prüfen
* Enum-Werte validieren
* `date_end >= date_start`

---

## Datenbank

```text
events
```

```text
id UUID PK
title
description
event_link
target_audience
topics
date_start
date_end
classification
fees
is_online
organization_id
program_name
format
school_bookable
age_minimum
age_maximum
location_id
contact_id
status default 'Draft'
created_by
modified_by
created_at
modified_at
is_deleted default false
```

Hinweis:

* `target_audience` und `topics` sind Enum-Werte (vorgegeben)

---

## Commit Strategie

```text
feat: add event entity
feat: add migration
feat: add endpoints
feat: add status endpoint
test: add integration tests
```

---

# Examples

## GET /api/events

```text
liefert alle Events mit is_deleted = false
```

---

## GET /api/events/{id}

```text
200 OK
404 NotFound (auch bei soft delete)
```

---

## POST /api/events

```json
{
  "title": "Event",
  "description": "Beschreibung",
  "target_audience": [20],
  "topics": [100],
  "date_start": "2026-01-01T10:00:00Z",
  "date_end": "2026-01-01T12:00:00Z",
  "classification": "Scheduled",
  "fees": false,
  "is_online": true,
  "organization_id": "guid"
}
```

---

## PUT /api/events/{id}

```text
200 OK
404 NotFound
400 BadRequest
409 Conflict
```

---

## DELETE /api/events/{id}

```text
204 NoContent
```

---

## PATCH /api/events/{id}/status

```text
200 OK
```

---

## GET /api/public/events

```text
status = Approved AND is_deleted = false
```

---

## Response DTO

```json
{
  "id": "guid",
  "title": "string",
  "description": "string",
  "target_audience": [20],
  "topics": [100],
  "date_start": "datetime",
  "date_end": "datetime",
  "classification": "Scheduled",
  "fees": false,
  "is_online": true,
  "organization_id": "guid",
  "status": "Approved",
  "created_at": "datetime",
  "modified_at": "datetime"
}
```

---

# Verification

### Build

* kompiliert ohne Fehler

### Datenbank

* Tabelle erstellt
* Defaults gesetzt
* Soft Delete funktioniert

### API

* CRUD funktioniert
* gelöschte → 404
* Public Endpoint korrekt

### Tests

* Create
* Read
* Update
* Delete
* Status
* Soft Delete

---

## Kontrolle

* gleiche Sections wie Template
* gleiche Reihenfolge
* keine neuen Sections erfunden
* nur Inhalte ergänzt
* deine Regeln umgesetzt
