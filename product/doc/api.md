# REST-API

## Organisationen

### Alle Organisationen
GET /api/organizations

### Einzelne Organisation
GET /api/organizations/{id}

### Organisation nach Name suchen
GET /api/organizations?name={name}

### Organisation erstellen
POST /api/organizations

### Organisation aktualisieren
PUT /api/organizations/{id}

### Organisation löschen
DELETE /api/organizations/{id}

## Kontakte
### Alle Kontakte
GET /api/contacts

### Einzelner Kontakt
GET /api/contacts/{id}

### Kontakt nach Name suchen
GET /api/contacts?name={name}

### Kontakt erstellen
POST /api/contacts

### Kontakt aktualisieren
PUT /api/contacts/{id}

### Kontakt löschen
DELETE /api/contacts/{id}

# Events
### Alle Events
GET /api/events

### Einzelnes Event
GET /api/events/{id}

### Event nach Name suchen
GET /api/events?name={name}

### Events nach Status filtern
GET /api/events?status={status}

### Events paginiert abrufen
GET /api/events?page={page}&pageSize={pageSize}

### Event erstellen
POST /api/events

### Event aktualisieren
PUT /api/events/{id}

### Event löschen
DELETE /api/events/{id}

### Eventstatus aktualisieren
PATCH /api/events/{id}/status

### Öffentliche Events abrufen
GET /api/public/events

### LLM generierung anstoßen
POST /api/events/llm

## Locations
### Alle Locations
GET /api/locations

### Einzelne Location
GET /api/locations/{id}

### Location erstellen
POST /api/locations

### Location aktualisieren
PUT /api/locations/{id}

### Location löschen
DELETE /api/locations/{id}


## Rückgabekonventionen
- Bei PUT, PATCH und POST: Rückgabe des aktualisierten/erstellten Objekts mit Statuscode 200 (OK) oder 201 (Created).

