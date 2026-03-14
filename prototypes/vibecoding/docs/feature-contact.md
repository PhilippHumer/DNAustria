# Feature Slice Template (Backend)

## Context

### Projekt

**DNAustria** ist eine Plattform zur Verwaltung von Veranstaltungen in Österreich.
Das Projekt entsteht im Rahmen eines **FH-Semesterprojekts an der FH Hagenberg**.

Technologie-Stack:

* Frontend: **Angular**
* Backend: **.NET**
* Datenbank: **PostgreSQL**

Das Projekt verwendet ein **Monorepo** mit folgender Struktur:

```
frontend   → Angular SPA
backend    → .NET Backend
infra      → Docker Compose für lokale Services
docs       → Projektdokumentation
```

### Backend Architektur

Das Backend folgt einer klaren Schichtung:

```
src/Domain
    Domainmodelle und Domänenlogik

src/Application
    Use-Case-orientierte Services

src/Infrastructure
    Technische Implementierungen (DB, externe Services)

src/Api
    HTTP Endpunkte und API Konfiguration

tests/Api.Tests
    API-nahe Integrationstests
```

### Frontend Struktur

Angular verwendet folgende Struktur:

```
core
shared
features
```

### Infrastruktur

Lokale Services werden über Docker Compose gestartet.

```
infra/docker-compose.yml
```

Beispiel:

* PostgreSQL
* optionale Services über Compose Profiles (z. B. Mailhog)

---

# Feature Kontext

In diesem Prompt soll ein **neuer Backend Feature Slice** implementiert werden.

Der Slice:

* implementiert eine neue **contact**-Entität
* integriert sich in die bestehende Architektur
* dient als **Referenzstruktur für zukünftige Entitäten**

---

# Domain Model

Entity: **contact**

| Feld        | Typ      | Regeln                                                     |
| ----------- | -------- | ---------------------------------------------------------- |
| id          | GUID     | Primärschlüssel                                            |
| name        | string   | Pflichtfeld, max 50 Zeichen, **unique (case-insensitive)** |
| email       | string   | Pflichtfeld, max 100 Zeichen                               |
| phone       | string   | Pflichtfeld, max 50 Zeichen                                |
| created_at  | datetime | UTC                                                        |
| modified_at | datetime | UTC                                                        |
| is_deleted  | bool     | Default false                                              |

Optionale Felder:

| Feld            | Typ    | Regeln                                  |
| --------------- | ------ | --------------------------------------- |
| organization_id | GUID   | optional, Referenz auf Organization     |
| org             | string | optional, Anzeige-Text der Organisation |
| created_by      | string | wird automatisch gesetzt                |
| modified_by     | string | wird automatisch gesetzt                |

---

# Fachliche Regeln

Für **contact** gelten folgende Regeln:

* `name` darf nicht leer sein
* `name` maximal 50 Zeichen
* `name` muss **eindeutig sein (case-insensitive)** für `is_deleted = false`
* `email` ist Pflichtfeld
* `email` muss gültiges E-Mail Format haben
* `email` maximal 100 Zeichen
* `phone` maximal 50 Zeichen
* Strings werden vor Speicherung **getrimmt**
* `organization_id` ist optional
* `org` ist optional und dient **nur als Anzeige-Text**

Konsistenzregel:

```
organization_id und org müssen immer zusammen passen.
Wenn organization_id gesetzt ist, muss auch org gesetzt sein.
```

Audit Felder:

```
created_by und modified_by werden automatisch gesetzt.
```

---

# Soft Delete

Physisches Löschen ist **verboten**.

DELETE führt zu:

```
is_deleted = true
modified_at = now()
```

Verhalten:

* Soft-deleted Datensätze erscheinen in keinen GET-Endpunkten
* Zugriff auf eine gelöschte Ressource liefert `404`
* DELETE auf eine bereits gelöschte Ressource liefert ebenfalls `404`

---

# Intent

Es soll ein vollständiger **Backend Feature Slice für contact** implementiert werden.

Der Slice umfasst:

* Domain Modell
* Datenbankstruktur
* CRUD API
* Validierung
* Soft Delete Verhalten
* Integrationstests

Der Slice soll:

* sauber in die bestehende Architektur integriert sein
* als Referenzstruktur für weitere Entitäten dienen
* klar strukturiert und wartbar sein
* keine unnötige Komplexität enthalten

Nicht Teil dieses Slice:

* Authentifizierung
* Pagination
* komplexe Businesslogik außerhalb des Entity-Kontexts

---

# Constraints

## Architektur

Halte dich strikt an die bestehende Projektstruktur.

Klare Trennung zwischen:

```
Domain
Application
Infrastructure
API
```

Keine unnötige Abstraktion.

---

# API Regeln

Verfügbare Endpunkte:

```
GET    /api/contacts
GET    /api/contacts/{id}
POST   /api/contacts
PUT    /api/contacts/{id}
DELETE /api/contacts/{id}
```

Optionaler Query Parameter:

```
GET /api/contacts?name={filter}
```

HTTP Semantik:

```
POST   → 201 Created + Body
PUT    → 200 OK + Body
DELETE → 204 NoContent
GET    → 200 OK
```

Fehlercodes:

```
400 BadRequest
404 NotFound
409 Conflict
```

Zusätzliche Regeln:

```
GET by id auf soft-deleted entity → 404
DELETE auf nicht existierende Ressource → 404
DELETE auf bereits gelöschte Ressource → 404
```

---

# Validierung

Allgemeine Regeln:

* Strings werden **getrimmt**
* Pflichtfelder dürfen nicht leer sein
* maximale Länge muss eingehalten werden

Spezifisch für contact:

* `name` max 50 Zeichen
* `name` unique (case-insensitive)
* `email` gültiges E-Mail Format
* `email` max 100 Zeichen
* `phone` max 50 Zeichen
* `org` max 50 Zeichen

---

# Name / Identifier Regeln

Name-Vergleiche sind:

```
case-insensitive
```

Uniqueness gilt nur für:

```
is_deleted = false
```

---

# Datenbank

Tabellenname:

```
contacts
```

Primary Key:

```
id UUID
```

Tabellenstruktur:

```
id UUID PRIMARY KEY
name VARCHAR(50) NOT NULL
organization_id UUID NULL
org VARCHAR(50) NULL
email VARCHAR(100) NOT NULL
phone VARCHAR(50) NOT NULL
created_by TEXT NULL
modified_by TEXT NULL
created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
modified_at TIMESTAMP WITH TIME ZONE NULL
is_deleted BOOLEAN DEFAULT false
```

Defaults:

```
created_at default now()
is_deleted default false
```

Empfohlene Indizes:

```
index on name
index on is_deleted
```

Unique Constraint:

```
unique(name) where is_deleted = false
```

Optional:

```
foreign key (organization_id) -> organizations(id)
```

---

# Commit Strategie

Commits müssen logisch getrennt sein.

Beispiele:

```
feat: add contact domain model
feat: add contact database migration
feat: implement contact repository
feat: implement contact API endpoints
test: add contact integration tests
```

Große Sammelcommits sind zu vermeiden.

---

# Examples

## GET /api/contacts

Liefert alle aktiven Datensätze.

Optionaler Query Parameter:

```
?name={filter}
```

Suchregeln:

* case-insensitive
* Teilstringsuche (`contains`)
* trim whitespace
* nur `is_deleted = false`

---

## GET /api/contacts/{id}

```
200 OK
```

wenn Ressource existiert.

```
404 Not Found
```

wenn:

* Ressource nicht existiert
* Ressource soft-deleted ist

---

## POST /api/contacts

Request Beispiel:

```json
{
  "name": "Max Mustermann",
  "organization_id": "guid-or-null",
  "org": "DNAustria",
  "email": "max.mustermann@example.com",
  "phone": "+43 123 456789"
}
```

Response:

```
201 Created
```

Fehler:

```
400 BadRequest
409 Conflict
```

`409 Conflict` wenn:

```
name bereits existiert (case-insensitive)
```

---

## PUT /api/contacts/{id}

PUT erwartet **immer alle Felder**.

Updatebare Felder:

```
name
organization_id
org
email
phone
```

Audit Felder werden automatisch gesetzt:

```
modified_by
modified_at
```

Response:

```
200 OK
```

Fehler:

```
404 NotFound
409 Conflict
400 BadRequest
```

---

## DELETE /api/contacts/{id}

Soft Delete:

```
is_deleted = true
modified_at = now()
```

Response:

```
204 NoContent
```

---

# Response DTO

```
ContactDto
```

```
{
  "id": "guid",
  "name": "string",
  "organization_id": "guid | null",
  "org": "string | null",
  "email": "string",
  "phone": "string",
  "created_at": "datetime",
  "modified_at": "datetime"
}
```

Nicht enthalten:

```
is_deleted
created_by
modified_by
```

---

# Verification

Der Feature Slice gilt als abgeschlossen wenn:

### Build

* Backend kompiliert ohne Fehler

---

### Datenbank

* Migration erstellt Tabelle korrekt
* Indizes existieren
* Unique Constraint funktioniert
* Soft Delete wird korrekt gespeichert

---

### API

* alle CRUD Endpunkte funktionieren
* GET liefert keine gelöschten Datensätze
* Zugriff auf gelöschte Datensätze liefert `404`

---

### Tests

Integrationstests vorhanden für:

```
Create
Read
Update
Delete
Duplicate Constraint (name)
Soft Delete Verhalten
GET by id auf gelöschte Ressource -> 404
GET list liefert keine gelöschten Datensätze
```

---

### Dokumentation

* README ggf. um neue Endpunkte ergänzt

