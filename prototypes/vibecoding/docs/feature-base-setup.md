Hier ist ein **Projekt-Setup-Prompt im exakt gleichen Stil** (Context → Intent → Constraints → Examples → Verification).
Der Zweck ist: **LLMs oder Copilot sollen das Backend-Grundgerüst deterministisch erzeugen**, bevor Feature Slices implementiert werden.

---

# Backend Project Setup — DNAustria

# Context

## Projekt

**DNAustria** ist eine Plattform zur Verwaltung von Veranstaltungen in Österreich.
Das Projekt entsteht im Rahmen eines **FH-Semesterprojekts an der FH Hagenberg**.

Technologie-Stack:

* Frontend: **Angular**
* Backend: **.NET**
* Datenbank: **PostgreSQL**

Das Projekt verwendet ein **Monorepo**.

Projektstruktur:

```
frontend   → Angular SPA
backend    → .NET Backend
infra      → Docker Compose für lokale Services
docs       → Projektdokumentation
```

Dieser Prompt betrifft ausschließlich das **Backend Setup**.

---

## Ziel des Setups

Das Backend soll eine **klare, einfache und erweiterbare Architektur** bereitstellen, die:

* zukünftige Feature Slices sauber aufnehmen kann
* eine klare Trennung zwischen Domain, Application und Infrastruktur bietet
* für ein Semesterprojekt **nicht overengineered** ist
* Integrationstests ermöglicht
* PostgreSQL verwendet

Dieses Setup dient als **Basis für alle zukünftigen Feature Slices**.

---

## Backend Architektur

Das Backend folgt einer einfachen **Layered Architecture**.

Struktur:

```
backend
 └─ src
     ├─ Domain
     ├─ Application
     ├─ Infrastructure
     └─ Api

tests
 └─ Api.Tests
```

### Domain

Enthält:

* Domain Entities
* Value Objects
* Domain Logik

Keine Abhängigkeiten zu:

* Datenbank
* Web Frameworks
* Infrastructure

---

### Application

Enthält:

* Use Cases
* Application Services
* Interfaces für Repositories
* DTOs (falls notwendig)

Application kennt:

* Domain

Application kennt **nicht**:

* konkrete Datenbankimplementierungen

---

### Infrastructure

Enthält technische Implementierungen:

* Entity Framework Core
* PostgreSQL Konfiguration
* Repository Implementierungen
* Migrations

Infrastructure implementiert Interfaces aus **Application**.

---

### API

Enthält:

* HTTP Controller
* Routing
* Dependency Injection
* API Konfiguration

API kennt:

* Application
* Infrastructure

---

### Tests

Integrationstests befinden sich in:

```
tests/Api.Tests
```

Tests greifen über HTTP auf die API zu.

---

# Intent

Es soll das **vollständige Backend-Grundgerüst** implementiert werden.

Das Setup umfasst:

* .NET Solution
* Projekte für Domain, Application, Infrastructure und Api
* Dependency Injection Setup
* PostgreSQL Integration über Entity Framework Core
* Datenbank Migration Setup
* Docker Compose Integration
* Integrationstest Projekt

Das Setup soll:

* minimalistisch
* verständlich
* stabil
* erweiterbar

sein.

Das Setup muss so gestaltet sein, dass zukünftige **Feature Slices problemlos ergänzt werden können**.

---

# Constraints

## .NET Version

Verwende:

```
.NET 8
```

---

## Projektstruktur

Die Solution muss folgende Projekte enthalten:

```
DNAustria.Domain
DNAustria.Application
DNAustria.Infrastructure
DNAustria.Api
DNAustria.Api.Tests
```

Ordnerstruktur:

```
backend
 ├─ src
 │   ├─ Domain
 │   ├─ Application
 │   ├─ Infrastructure
 │   └─ Api
 │
 └─ tests
     └─ Api.Tests
```

---

## Projektabhängigkeiten

Abhängigkeiten müssen strikt eingehalten werden.

```
Domain
   ↑
Application
   ↑
Infrastructure
   ↑
API
```

Tests referenzieren:

```
Api
```

---

## Entity Framework

Entity Framework Core wird für Persistence verwendet.

NuGet Packages:

```
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Design
Microsoft.EntityFrameworkCore.Tools
Npgsql.EntityFrameworkCore.PostgreSQL
```

Infrastructure enthält:

```
AppDbContext
```

---

## Datenbank

Datenbank:

```
PostgreSQL
```

Connection String kommt aus:

```
appsettings.json
```

Beispiel:

```
Host=localhost
Port=5432
Database=dnaustria
Username=postgres
Password=postgres
```

---

## Migration Setup

Migrations werden im **Infrastructure Projekt** gespeichert.

Beispiel:

```
dotnet ef migrations add InitialCreate
```

---

## Dependency Injection

Registrierung erfolgt in:

```
Api/Program.cs
```

Infrastructure registriert:

```
DbContext
Repositories
```

---

## Docker Integration

Lokale Datenbank wird über Docker Compose gestartet.

Pfad:

```
infra/docker-compose.yml
```

PostgreSQL Beispiel:

```
postgres:
  image: postgres:16
  environment:
    POSTGRES_DB: dnaustria
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: postgres
  ports:
    - "5432:5432"
```

---

## Commit Strategie

Commits müssen logisch getrennt sein.

Beispiel:

```
feat: create backend solution structure
feat: add domain and application projects
feat: add infrastructure with ef core
feat: configure postgresql database
feat: add api project with dependency injection
test: add api integration test project
```

Große Sammelcommits vermeiden.

---

# Examples

## Solution Struktur

```
DNAustria.sln

backend/
 ├─ src
 │   ├─ DNAustria.Domain
 │   ├─ DNAustria.Application
 │   ├─ DNAustria.Infrastructure
 │   └─ DNAustria.Api
 │
 └─ tests
     └─ DNAustria.Api.Tests
```

---

## Minimaler API Endpoint

Beispiel:

```
GET /health
```

Response:

```
200 OK
```

Body:

```
{
  "status": "ok"
}
```

---

## DbContext Beispiel

```
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
```

---

## Program.cs Beispiel

```
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
```

---

# Verification

Das Backend Setup gilt als abgeschlossen wenn:

## Build

```
dotnet build
```

läuft ohne Fehler.

---

## Projektstruktur

Solution enthält:

```
Domain
Application
Infrastructure
Api
Api.Tests
```

---

## Datenbank

PostgreSQL startet über:

```
docker compose up
```

---

## Migration

Migration kann erstellt werden:

```
dotnet ef migrations add InitialCreate
```

---

## API

API startet erfolgreich:

```
dotnet run
```

Health Endpoint funktioniert:

```
GET /health → 200 OK
```

---

## Tests

Integrationstestprojekt:

```
DNAustria.Api.Tests
```

Tests können ausgeführt werden:

```
dotnet test
```
