# Feature Slice Template (Backend)

## Context

### Projekt

**DNAustria** ist eine Plattform zur Verwaltung von Veranstaltungen in Österreich.  
Das Projekt entsteht im Rahmen eines **FH-Semesterprojekts an der FH Hagenberg**.

Technologie-Stack:

* Frontend: Angular
* Backend: .NET
* Datenbank: PostgreSQL

Monorepo-Struktur:

```text
frontend   → Angular SPA
backend    → .NET Backend
infra      → Docker Compose
docs       → Dokumentation
```

### Backend Architektur

```text
src/Domain
src/Application
src/Infrastructure
src/Api
tests/Api.Tests
```

## Feature Kontext

Es wird ein **Feature Slice für Location und Address** implementiert.

Wichtig:

* `Location` referenziert `Address` über `address_id`
* `Address` wird eigenständig verwaltet
* Deduplication erfolgt auf Address-Ebene

## Domain Model

### Location

| Feld        | Typ      | Regeln          |
| ----------- | -------- | --------------- |
| id          | GUID     | PK              |
| name        | string   | Pflicht, max 50 |
| address_id  | GUID     | Pflicht, FK     |
| is_deleted  | bool     | default false   |
| created_at  | datetime | UTC             |
| modified_at | datetime | UTC             |

### Address

| Feld          | Typ      | Regeln          |
| ------------- | -------- | --------------- |
| id            | GUID     | PK              |
| location_name | string   | Pflicht, max 50 |
| street        | string   | Pflicht, max 50 |
| city          | string   | Pflicht, max 50 |
| zip           | string   | Pflicht, max 10 |
| state         | string   | Pflicht, max 50 |
| latitude      | numeric  | Pflicht         |
| longitude     | numeric  | Pflicht         |
| is_deleted    | bool     | default false   |
| created_at    | datetime | UTC             |
| modified_at   | datetime | UTC             |

## Fachliche Regeln

* `Location.name` ist Pflicht
* `Location.address_id` ist Pflicht
* `Location.name` darf doppelt vorkommen
* `Location` erhält nur `address_id`
* API liefert immer die vollständige `Address`
* gleiche `Address` darf mehrfach referenziert werden
* Strings werden getrimmt
* `Address` bleibt nach `Location`-Delete bestehen
* `Address` darf nicht gelöscht werden, wenn sie noch von mindestens einer aktiven `Location` referenziert wird

### Deduplication

`Address` wird fachlich eindeutig identifiziert durch:

```text
zip + latitude + longitude
```

Technische Regeln:

* Vergleich erfolgt auf exakten numerischen Werten
* keine implizite Rundung im Backend
* Speicherung als `NUMERIC/DECIMAL`, nicht als `FLOAT/DOUBLE`
* Dedup-Prüfung gilt nur für aktive Datensätze (`is_deleted = false`)

### Address Create

```text
1. Input trimmen und validieren
2. prüfen, ob aktive Address mit gleicher Kombination aus
   zip + latitude + longitude existiert
3. wenn ja → bestehende Address zurückgeben
4. wenn nein → neue Address anlegen
```

### Address Update

```text
1. Input trimmen und validieren
2. prüfen, ob eine andere aktive Address dieselbe Kombination aus
   zip + latitude + longitude besitzt
3. wenn ja → 400 BadRequest
4. wenn nein → Update durchführen
```

## Soft Delete

Physisches Löschen ist verboten.

```text
is_deleted = true
modified_at = now()
```

Regeln:

* GET liefert nur aktive Datensätze
* Zugriff auf gelöschte Datensätze → `404 NotFound`

# Intent

Es soll ein vollständiger **Backend Feature Slice für Location und Address** implementiert werden.

Der Slice umfasst:

### Location

* Domain
* Repository
* Service
* API
* Tests

### Address

* Domain
* Repository
* Service
* API
* Deduplication
* Delete-Regel
* Tests

Nicht Teil dieses Slice:

* Authentifizierung
* Pagination
* komplexe Businesslogik außerhalb des Entity-Kontexts

# Constraints

## Architektur

```text
Domain
Application
Infrastructure
API
```

Keine unnötigen Abstraktionen.

## API

### Location

```text
GET    /api/locations
GET    /api/locations/{id}
POST   /api/locations
PUT    /api/locations/{id}
DELETE /api/locations/{id}
```

### Address

```text
GET    /api/addresses
GET    /api/addresses/{id}
POST   /api/addresses
PUT    /api/addresses/{id}
DELETE /api/addresses/{id}
```

## HTTP Semantik

```text
GET    → 200 OK
POST   → 201 Created + Body
PUT    → 200 OK + Body
DELETE → 204 NoContent
```

Fehlercodes:

```text
400 BadRequest
404 NotFound
```

## Validierung

### Location

* `name` required, max 50
* `address_id` required
* `address_id` muss existieren und aktiv sein, sonst `404`

### Address

* `location_name` required, max 50
* `street` required, max 50
* `city` required, max 50
* `zip` required, max 10
* `state` required, max 50
* `latitude` required
* `longitude` required

## Datenbank

### locations

```sql
id UUID PRIMARY KEY
name VARCHAR(50) NOT NULL
address_id UUID NOT NULL
is_deleted BOOLEAN NOT NULL DEFAULT FALSE
created_at TIMESTAMP NOT NULL DEFAULT now()
modified_at TIMESTAMP NOT NULL DEFAULT now()
```

### addresses

```sql
id UUID PRIMARY KEY
location_name VARCHAR(50) NOT NULL
street VARCHAR(50) NOT NULL
city VARCHAR(50) NOT NULL
zip VARCHAR(10) NOT NULL
state VARCHAR(50) NOT NULL
latitude NUMERIC(9,6) NOT NULL
longitude NUMERIC(9,6) NOT NULL
is_deleted BOOLEAN NOT NULL DEFAULT FALSE
created_at TIMESTAMP NOT NULL DEFAULT now()
modified_at TIMESTAMP NOT NULL DEFAULT now()
```

### Constraints

```sql
FOREIGN KEY (address_id) REFERENCES addresses(id)

UNIQUE (zip, latitude, longitude) WHERE is_deleted = false
```

# Examples

## POST /api/addresses

```json
{
  "location_name": "FH Hagenberg",
  "street": "Softwarepark 11",
  "city": "Hagenberg",
  "zip": "4232",
  "state": "Oberösterreich",
  "latitude": 48.368000,
  "longitude": 14.513000
}
```

Verhalten:

* existiert bereits eine aktive Address mit gleicher Kombination → bestehende Address zurückgeben
* sonst neue Address erstellen

## PUT /api/addresses/{id}

Wenn die neue Kombination `zip + latitude + longitude` bereits bei einer anderen aktiven `Address` existiert:

```text
400 BadRequest
```

## POST /api/locations

```json
{
  "name": "FH Hagenberg",
  "address_id": "guid"
}
```

## Response DTO

```json
{
  "id": "guid",
  "name": "FH Hagenberg",
  "address": {
    "id": "guid",
    "location_name": "FH Hagenberg",
    "street": "Softwarepark 11",
    "city": "Hagenberg",
    "zip": "4232",
    "state": "Oberösterreich",
    "latitude": 48.368000,
    "longitude": 14.513000
  },
  "created_at": "datetime",
  "modified_at": "datetime"
}
```

## DELETE /api/addresses/{id}

* `400 BadRequest`, wenn mindestens eine aktive `Location` darauf referenziert
* `404 NotFound`, wenn `Address` nicht existiert oder bereits gelöscht ist
* sonst Soft Delete und `204 NoContent`

# Verification

## Build

* Backend kompiliert ohne Fehler

## Datenbank

* Tabellen korrekt erstellt
* Foreign Key funktioniert
* Unique Constraint funktioniert
* `latitude` und `longitude` sind `NUMERIC(9,6)` und `NOT NULL`
* Soft Delete wird korrekt gespeichert

## API

* CRUD für `Location` funktioniert
* CRUD für `Address` funktioniert
* GET liefert keine gelöschten Datensätze
* `Location` liefert `Address` eingebettet zurück
* `address_id` auf gelöschte oder nicht vorhandene `Address` → `404`
* Address Dedup funktioniert bei Create
* Address Update liefert `400`, wenn Dedup verletzt würde
* `Address` bleibt nach `Location`-Delete bestehen

## Tests

```text
Create Address returns existing address when duplicate exists
Create Address creates new address when no duplicate exists
Update Address returns 400 when duplicate zip + latitude + longitude would occur
Update Address succeeds when combination stays unique
Create Location
Read Location
Update Location
Delete Location
Delete Address blocked when referenced
Delete Address allowed when not referenced
404 for deleted Address
404 for deleted Location
```

## Commit Strategie

```text
feat: add location domain model
feat: add address domain model
feat: add location and address migration
feat: implement address deduplication
feat: implement address endpoints
feat: implement location endpoints
test: add location and address integration tests
```
