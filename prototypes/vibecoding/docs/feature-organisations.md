# Feature Slice — Organizations

# Context

## Projekt

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

---

## Backend Architektur

Das Backend folgt einer klaren Schichtung:

```
src/Domain
    Domainmodelle und Domänenlogik

src/Application
    Use-Case-orientierte Services

src/Infrastructure
    Technische Implementierungen (z. B. Datenbank)

src/Api
    HTTP Endpunkte und API Konfiguration

tests/Api.Tests
    API-nahe Integrationstests
```

---

## Frontend Struktur

Angular verwendet folgende Struktur:

```
core
shared
features
```

---

## Infrastruktur

Lokale Services werden über Docker Compose gestartet.

```
infra/docker-compose.yml
```

Beispiel Services:

* PostgreSQL
* optionale Services über Compose Profiles (z. B. Mailhog)

---

## Feature Kontext

Das Projektsetup existiert bereits.
Nun wird der erste fachliche Backend-Slice implementiert:

**Organization Management**

Dieser Slice dient als **Referenzstruktur für zukünftige Entitäten** und soll daher:

* klar strukturiert
* konsistent umgesetzt
* einfach erweiterbar

sein.

---

## Domain Model

Entity: **Organization**

| Feld        | Typ      | Regeln                       |
| ----------- | -------- | ---------------------------- |
| id          | GUID     | Primärschlüssel              |
| name        | string   | Pflichtfeld, max. 50 Zeichen |
| address_id  | GUID     | optional                     |
| is_deleted  | bool     | Default false                |
| created_at  | datetime | UTC                          |
| modified_at | datetime | UTC                          |

---

## Fachliche Regeln

* `name` muss eindeutig sein **für aktive Organisationen**
* Soft-deleted Datensätze dürfen denselben Namen wiederverwenden
* `created_at` wird beim Erstellen gesetzt
* `modified_at` wird bei jeder Änderung aktualisiert
* Alle Timestamps sind **UTC**

---

## Soft Delete

Physisches Löschen ist **verboten**.

DELETE führt zu:

```
is_deleted = true
modified_at = now()
```

Verhalten:

* GET-Endpunkte dürfen nur Datensätze mit `is_deleted = false` zurückgeben
* Gelöschte Organisationen erscheinen in keinen Listen
* Zugriff auf eine gelöschte Organisation liefert `404`

---

# Intent

Es soll ein vollständiger **Backend Feature Slice für Organization** implementiert werden.

Der Slice umfasst:

* Domain Modell
* Datenbankstruktur
* CRUD API
* Validierung
* Soft Delete Verhalten
* Integrationstests

Der Slice soll:

* als Referenzstruktur für zukünftige Entitäten dienen
* eine konsistente API bereitstellen
* sauber in die bestehende Architektur integriert sein
* einfach erweiterbar bleiben

Nicht Teil dieses Slice:

* Authentifizierung
* Pagination
* komplexe Businesslogik außerhalb des Organization-Kontexts

---

# Constraints

## Architektur

* Halte dich strikt an die bestehende Projektstruktur
* klare Trennung zwischen

```
Domain
Application
Infrastructure
API
```

* keine unnötige Komplexität oder Overengineering

---

## API Regeln

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

---

## Validierung

Name:

* Pflichtfeld
* whitespace wird getrimmt
* darf nicht leer sein
* maximal **50 Zeichen**

Vergleich:

* case-insensitive
* uniqueness gilt nur für `is_deleted = false`

---

## Soft Delete

Physisches Löschen ist verboten.

DELETE setzt:

```
is_deleted = true
modified_at = now()
```

Verhalten:

* Gelöschte Datensätze erscheinen in keinen GET-Endpunkten
* Zugriff auf gelöschte Ressourcen liefert `404`

---

## Datenbank

Tabelle:

```
organizations
```

Primary Key:

```
id UUID
```

Partial Unique Constraint:

```
unique(name) where is_deleted = false
```

Defaults:

```
is_deleted default false
created_at default now()
```

Empfohlene Indizes:

```
index on name
index on is_deleted
```

---

## Commit Strategie

Commits müssen logisch getrennt sein.

Beispiel:

```
feat: add organization domain model
feat: add organizations database migration
feat: implement organizations repository
feat: implement organizations API endpoints
test: add organization integration tests
```

Große Sammelcommits sind zu vermeiden.

---

# Examples

## GET /api/organizations

Liefert alle **aktiven Organisationen**.

Optionaler Query Parameter:

```
?name={filter}
```

Suchregeln:

* case-insensitive
* Teilstringsuche (`contains`)
* whitespace wird getrimmt

Beispiel:

```
GET /api/organizations?name=hagen
```

Ergebnis:

```
FH Hagenberg
Hagenberg Events
```

---

## GET /api/organizations/{id}

```
200 OK
```

wenn Organisation existiert.

```
404 Not Found
```

wenn:

* Organisation nicht existiert
* Organisation soft-deleted ist

---

## POST /api/organizations

Request:

```
{
  "name": "FH Hagenberg",
  "address_id": "optional-guid"
}
```

Response:

```
201 Created
```

Fehler:

```
400 BadRequest
```

wenn:

* name fehlt
* name leer ist
* name länger als 50 Zeichen ist

```
409 Conflict
```

wenn:

* name bereits vergeben ist

---

## PUT /api/organizations/{id}

Updatebare Felder:

```
name
address_id
```

Verhalten:

```
modified_at wird aktualisiert
```

Response:

```
200 OK
```

Fehler:

```
404 NotFound
```

wenn:

* Organisation nicht existiert
* Organisation soft-deleted ist

```
409 Conflict
```

wenn:

* Name bereits vergeben ist

---

## DELETE /api/organizations/{id}

Soft Delete:

```
is_deleted = true
modified_at = now()
```

Response:

```
204 NoContent
```

Fehler:

```
404 NotFound
```

wenn:

* Organisation nicht existiert
* Organisation bereits gelöscht ist

---

## Response DTO

```
OrganizationDto
```

```
{
  "id": "guid",
  "name": "string",
  "address_id": "guid | null",
  "created_at": "datetime",
  "modified_at": "datetime"
}
```

Nicht enthalten:

```
is_deleted
```

---

# Verification

Der Feature Slice gilt als abgeschlossen wenn:

## Build

* Backend kompiliert erfolgreich
* keine Compilerfehler

---

## Datenbank

* Tabelle `organizations` existiert
* Unique Constraint funktioniert korrekt
* Soft Delete wird korrekt gespeichert

---

## API

* alle CRUD Endpunkte funktionieren
* GET liefert keine gelöschten Organisationen
* Zugriff auf gelöschte Organisationen liefert `404`

---

## Tests

Integrationstests vorhanden für:

```
Create
Read
Update
Delete
Duplicate Name → 409
Soft Delete Verhalten
```

---

## Dokumentation

* README ggf. um neue Endpunkte ergänzt