# Erkenntnisse aus dem Vibecoding-Team

## 2026/02/28

Wir haben uns mit Hilfe von ChatGPT Context-Windows und Limitierungen von LLM-Chats erklären lassen. Erkenntnisse daraus:

---

LLMs haben ein begrenztes Context Window (Token-Limit). Dabei konkurieren:

- System Prompt
- Entwickler-Instruction
- User Input
- Tool-Outputs
- Chat-Historie

um denselben Platz.

---

Wichtige Faktoren, die sich auf das Arbeitsgedächtnis (context-windows) auswirken sind Token-Limit des Modells, Token-Verteilung (wie viel geht für Rauschen drauf?), Reihenfolge der Informationen, semantische Dichte des Textes und Redundanz von Informationen.

---

Die Kontext-Nutzung lässt sich Optimieren durch:

**Projekt-Manifest fürhen**
Hierbei handelt es sich um ein zentrales Dokument, das Informationen zu Architektur, Features, Coding-Conventions und Informationen zur gesamtstruktur beinhaltet.

**Feature-Scoped Prompts**
In dem die Features einzeln geschnitten und nicht eine gesamte Applikation aus einem einzelnen Prompt entsteht, werden Anforderungen deutlich genauer verarbeitet.

Dabei werden Informationen wie Modulbeschreibung, Schnittstellen, Constraints und Zielzustand auf Feature-Ebene definiert.

**State-Compression**
Dabei wird alle 2-3 Iterationen der aktuelle Projektzustand zusammengefasst und für die weitere Verwendung festgehalten.

> Fasse den aktuellen Projektzustand strukturiert zusammen.

---

## Rollendefinition

Es ist zusätzlich wichtig im Vorhinein zu definieren, welche Rolle der Mensch und welche das KI-Modell einnimmt.

In unserem Fall ist empfehlenswert in diese Rollen zu unterscheiden:

- Product Owner/Requirements Engineer: Mensch
  - Hier kann die KI aber sehr gut Unterstützen, in dem z.B. Formulierungen geprüft werden können, um sie besser verständlich zu schreiben.
- Software-Architekt: Mensch (mit KI-unterstützung)
  - Die anforderungen aus dem Dokument und die technischen und nicht-technischen Anforderungen sollten von Projektteam entschieden werden. Dazu gehören z.B. Definition von Abhängigkeiten zwischen Entitäten. Hier zeigt die Erfahrung, dass die Annahmen von KI-Modellen teilweise fachlich falsch abgebildet werden.
  - Für technische Entscheidungen kann das KI-Modell sehr gut unterstützen, speziell als Partner für Ideen-Ping-Pong oder für "Sanity-Checks". Vollständig überlassen kann man dem Modell i.d.R. die Architektur der Software aber nicht. große Schwierigkeiten gibt es bspw. bei der Konsistenz von Endpunkten und locker definierten Funktionalitäten.
  - Code-Review: KI
    - Hier kann die KI lt. ChatGPT mit höherer Geschwindigkeit punkten.

## 2026/03/07

Feature-Slices sind deutlich besser zu handhaben, als pauschale Projektanforderungen mit vielen, semi- oder unstrukturierten Informationen. 

Test mit verscheidenen Agenten. GPT 5.2-mini ist etwas hit-or-miss. Qualität des Outputs ist
unterschiedlich, scheint aber nach einigen (wenigen) Iterationen gut definierte Features funktioniered umsetzen zu können. 

Raptor-Mini-Preview  ist ähnlich, ergebnisse sind aber manchmal sehr schlecht und brauchen vergleichsweise viel nacharbeit.

GPT-5.2 setzt features durchgehend deutlich besser um, ist aber im Vergleich nicht viel schneller

## 2026-03-14?

Feature Slices funktionieren sehr gut, sind aber aufwendig zu definieren. Wir verwenden diese Struktur als halbwegs bindende vorgabe:


`Context`: Hier wird der Projektkontext definiert. Dazu gehören: 
- der Allgemeine Projektkontext, wie eine Beschreibung des Projektes
- der Technische Kontext, damit bspw. Versionen von Frameworks konsistent bleiben
- und die Projektstruktur (aufteilung in solutions im Backend) damit es hier keine Abweichungen bei der unabhängigen Entwicklung von mehreren Features gibt.

> Der Kontext in den Grundzügen sollte bei allen Slices gleich sein. 

`Intent` definiert, was in dem Slice umgesetzt werden sollte. Hier ist es wichtig die Ziele möglichst genau zu definieren, damit möglichst wenig Interpretationsspielraum entstehen kann

`Constraints` definieren harte Limits für den Chatbot. Hier ist es bspw. wichtig explizite Designentscheidungen (Architektur, Datenbank etc.) zu definieren. Der Chatbot neigt gelegntlich dazu sich "das Leben einfach zu machen" in dem er Konflikte in Bibliotheken mit Framework Up- und Downgrades zu umschiffen, was unnötig Tokens verbrennt. Bei Problemen mit der Datenbank greift er tlw. zu In-Memory Alternativen während der Entwicklung. 

Speziell beim debugging scheinen die Chatbots/Agenten hier schnell "kreativ" zu werden.

`Examples` sind hilfreich, um dem Chatbot/Agenten zu zeigen, welche Ergebnisse man erwartet. Gut funktionieren hier API Endpunkte mit dem erwarteten Antwortobjekt, dem Response-Code und Fehlerverhalten

`Verification` definiert dann die Erfolgskriterien, die der Chatbot/Agenten erfüllen muss. Gut funktioniert bspw. `Das Backend Setup gilt als abgeschlossen wenn dotnet build ohne Fehler durchläuft`. Hier kann man ebenfalls noch kriterien zu DB-Migrationen, docker setup oder ausführung der binaries (ggf. mit abstrichen)

`Documentation` fordert den Chatbot/Agenten dann noch auf eine entsprechende Dokumentation für das Feature zu erstellen. Bei offenen Formulierungen erstellt er i.d.R. eine README bzw. überarbeitet diese.

Hier ist es besonders Hilfreich, wenn man die Definietion nochmal von einem Chatbot prüfen lässt. Besonders Effektiv sind formulierungen wie "Welche Aspekte haben wir hier nicht betrachtet?" oder "Welche Edge-Cases können hier Auftreten und sollten berücksichtigt werden?", da diese den Chatbot von seinem "Happy Path" abbringen und zu kritischem Hinterfragen der Eingaben aufgefordert wird.

---
**Implementierung von Features**

**GPT5-mini und Raptor mini (Preview)** brauchen relativ Lange um zu eingem Ergebnis zu kommen und neigen dazu nicht-funktionalen Code zu schreiben, diesen dann zu Analysieren und dann zu Fixen - das Ergebnis funktioniert im Normalfall wie gefordert - in einigen Fällen war aber nach längeren Implementierungsphasen zusätzliche Informationen notwnendig. Besonders schwierig scheinen aber debugging Zyklen mit Compiler-Output zu sein. Meistens mussten mehrere Ansätze probiert werden, bevor ein Compiler Error verschwunden war.

**GPT-5.3-Codex und Claude Code** machen während der Entwicklung deutlich weniger Fehler, was schneller zu einem Ergebnis führt. Der zeitliche Unterschied ist aber sehr inkonsistent, bei gleichem Input scheinen auch diese Modelle nicht immer das gleiche Ergebnis zu liefern, bzw. nicht den gleichen Weg zu wählen. 

Auch bei rückfragen der Modelle gibt es Unterschiede - die Gratis-Modelle brauchen sehr viele 




```

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


```