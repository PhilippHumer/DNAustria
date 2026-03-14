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

## Feature Kontext

In diesem Prompt soll ein **neuer Backend Feature Slice** implementiert werden.

Der Slice:

* implementiert eine neue **Domänenentität**
* integriert sich in die bestehende Architektur
* dient als **Referenzstruktur für zukünftige Entitäten**

---

## Domain Model

Entity: **[ENTITY_NAME]**

| Feld        | Typ      | Regeln                      |
| ----------- | -------- | --------------------------- |
| id          | GUID     | Primärschlüssel             |
| name        | string   | Pflichtfeld, max 50 Zeichen |
| created_at  | datetime | UTC                         |
| modified_at | datetime | UTC                         |
| is_deleted  | bool     | Default false               |

Optionale Felder:

| Feld           | Typ  | Regeln   |
| -------------- | ---- | -------- |
| [custom_field] | type | optional |

---

## Fachliche Regeln

Hier werden die **domänenspezifischen Regeln** beschrieben.

Beispiele:

* `[field]` muss eindeutig sein
* Werte müssen validiert werden
* bestimmte Felder sind optional
* bestimmte Felder dürfen nach Erstellung nicht geändert werden

---

## Soft Delete

Physisches Löschen ist **verboten**.

DELETE führt zu:

```
is_deleted = true
modified_at = now()
```

Verhalten:

* Soft-deleted Datensätze erscheinen in keinen GET-Endpunkten
* Zugriff auf eine gelöschte Ressource liefert `404`

---

# Intent

Es soll ein vollständiger **Backend Feature Slice für [ENTITY_NAME]** implementiert werden.

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

* Halte dich strikt an die bestehende Projektstruktur
* klare Trennung zwischen

```
Domain
Application
Infrastructure
API
```

* keine unnötige Abstraktion

---

## API Regeln

HTTP Semantik:

```
POST   → 201 Created
PUT    → 200 OK
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

Allgemeine Regeln:

* Strings werden **getrimmt**
* Pflichtfelder dürfen nicht leer sein
* maximale Länge muss eingehalten werden

---

## Name / Identifier Regeln

Falls ein Name existiert:

* Vergleich ist **case-insensitive**
* Uniqueness gilt nur für `is_deleted = false`

---

## Datenbank

Tabellenname:

```
[entity_plural]
```

Beispiel:

```
organizations
```

Primary Key:

```
id UUID
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

Optional:

```
unique(name) where is_deleted = false
```

---

## Commit Strategie

Commits müssen logisch getrennt sein.

Beispiele:

```
feat: add [entity] domain model
feat: add [entity] database migration
feat: implement [entity] repository
feat: implement [entity] API endpoints
test: add [entity] integration tests
```

Große Sammelcommits sind zu vermeiden.

---

# Examples

## GET /api/[entities]

Liefert alle aktiven Datensätze.

Optionaler Query Parameter:

```
?name={filter}
```

Suchregeln:

* case-insensitive
* Teilstringsuche (`contains`)
* trim whitespace

---

## GET /api/[entities]/{id}

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

## POST /api/[entities]

Request Beispiel:

```
{
  "name": "Example Name"
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

bei Validierungsfehlern

```
409 Conflict
```

bei Unique Constraint Verletzung

---

## PUT /api/[entities]/{id}

Updatebare Felder:

* definierte mutable Felder

Response:

```
200 OK
```

Fehler:

```
404 NotFound
409 Conflict
```

---

## DELETE /api/[entities]/{id}

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

## Response DTO

```
[Entity]Dto
```

```
{
  "id": "guid",
  "name": "string",
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

### Build

* Backend kompiliert ohne Fehler

---

### Datenbank

* Migration erstellt Tabelle korrekt
* Indizes existieren
* Unique Constraints funktionieren
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
Duplicate Constraint
Soft Delete Verhalten
```

---

### Dokumentation

* README ggf. um neue Endpunkte ergänzt
