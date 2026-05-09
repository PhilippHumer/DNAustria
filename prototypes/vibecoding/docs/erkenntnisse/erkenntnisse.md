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

Auch bei rückfragen der Modelle gibt es Unterschiede - die Gratis-Modelle brauchen sehr viele revisionen, weil die Fragen tlw nicht zielgerichtet gestellt werden, informationen verschluckt oder nicht richtig interpretiert werden.

## 2026-03-27

Wir haben die swagger.json aus den beiden Projekten mit ChatGPT vergleichen und analysieren lassen um konkrete unterscheide zwischen den Endpunkten zu finden. Nachdem wir die Endpunkte im vorhinein schon definiert hatten sollten diese zwar recht gering ausfallen, allerdings gibt es schon einige Unterschiede.

### Vergleich API-Endpunkte:

- **Dateien:** swagger-vibe.json, swagger-diy.json

**Hauptunterschiede**
- **Pfad-/Konsistenz:** vibe verwendet gemischte Groß-/Kleinschreibung (z.B. /api/Contacts, /Locations), diy nutzt durchgängig Kleinbuchstaben und `/api/`-Prefix (z.B. /api/contacts, /api/locations). In vibe sind Locations unter `/Locations` (ohne `/api/`) — strukturell abweichend.
- **ID-Typen:** vibe -> integer (`int32`) IDs; diy -> string UUIDs (`format: "uuid"`).
- **Payload-/Schema-Namen:** vibe verwendet `CreateXDto` / `XDto` / `InsertEventDto` / `UpdateXDto`; diy verwendet `CreateXRequest` / `UpdateXRequest`. Feldnamen für Events/Addresses/Contacts unterscheiden sich (z.B. vibe `name`, `link`, `startDate` / diy `title`, `eventLink`, `dateStart`).
- **Validation & Anforderungen:** vibe-Schemas listen viele `required`-Felder und enthalten z.B. Pattern-Validierung für `state`; diy-Schemas setzen viele Felder als `nullable` und `additionalProperties:false`.
- **Response-Codes:** vibe verwendet 201 / 204 / 200 je nach Aktion; diy verwendet überwiegend 200 (einheitlicher).
- **Zusätzliche Endpunkte:** vibe hat `/api/events/llm` (LLM-Endpoint); diy hat `/api/public/events` (öffentliche Events) — beide haben Endpoints, die der andere nicht enthält.
- **Error-Modelle:** vibe enthält `ProblemDetails`-Schema und referenziert es bei 404/400; diy hat keine `ProblemDetails`-Referenzen in den gezeigten Responses.
- **Organisationen / Adressen:** vibe `OrganizationDto` enthält `adress` (Schreibfehler) und eingebettetes `AddressDto`; diy trennt über `addressId` (Referenz-Id) in Create/Update.
- **Query-Parameter:** Suche/Filter-Parameter differieren (z.B. vibe `/api/events` query `name`, diy uses `title`).
- **Semantik der Events:** vibe hat starke, typed EventDto mit integer IDs und many required fields; diy ist weniger restriktiv, verwendet andere Feldnamen und nullable Werte.


## 2026-04-11
Für die Code-Qualität haben wir uns für Stryker (Mutation Testing) und 

# Mutation Testing Analyse (Stryker)

## Überblick

Im Rahmen der Qualitätssicherung wurde das Projekt mittels Mutation Testing mit Stryker analysiert. Ziel ist es, die Effektivität der bestehenden Tests zu bewerten, indem gezielt kleine Codeveränderungen (Mutationen) eingeführt und überprüft wird, ob diese durch Tests erkannt werden.

### Ergebniszusammenfassung

* **Gesamtanzahl Mutants:** 1010
* **Killed:** 407
* **Survived:** 193
* **No Coverage:** 319
* **Ignored:** 87
* **Compile Errors:** 4

→ **Mutation Score: ~44,3 %**

Der definierte Mindestwert (Threshold) von 60 % wurde somit **nicht erreicht**.

---

## Interpretation der Ergebnisse

Der Mutation Score liegt im unteren Bereich und zeigt Verbesserungspotenzial. Wichtig ist jedoch die differenzierte Betrachtung:

### 1. Fehlende Testabdeckung

Ein signifikanter Anteil der Mutanten (**319 von 1010**) wurde nicht ausgeführt, da entsprechende Codebereiche **nicht durch Tests abgedeckt sind**.

➡️ Interpretation:
Ein Teil des niedrigen Scores ist nicht auf fehlerhafte Tests zurückzuführen, sondern auf **fehlende Tests**.

---

### 2. Unzureichende Testtiefe

Zusätzlich überleben **193 Mutanten**, obwohl sie ausgeführt wurden.

➡️ Interpretation:
Vorhandene Tests prüfen oft nur oberflächlich:

* Methoden werden aufgerufen, aber Ergebnisse nicht ausreichend verifiziert
* Seiteneffekte (z. B. Datenbankoperationen) werden nicht überprüft
* Grenzwerte (Boundary Conditions) werden nicht getestet

---

## Analyse nach Komponenten

Die meisten überlebenden Mutanten konzentrieren sich auf wenige zentrale Klassen:

### Kritische Bereiche

* **AppDbContext.cs**

  * 162 Mutanten, davon 85 survived
  * hoher Anteil infrastruktureller Logik

* **EventService.cs**

  * 126 Mutanten, davon 19 survived

* **AddressService.cs**

  * 95 Mutanten, davon 17 survived

* **ContactService.cs**

  * 84 Mutanten, davon 17 survived

* **LocationService.cs**

  * 55 Mutanten, davon 12 survived

* **Program.cs**

  * 27 Mutanten, davon 12 survived

---

## Typische Schwachstellen

Die Analyse der Mutationen zeigt wiederkehrende Muster:

### 1. Boundary-Tests fehlen

Beispiel:

* `Length > 50` wird zu `Length >= 50` mutiert → Test schlägt nicht fehl

➡️ Problem:
Grenzwerte werden nicht explizit getestet (z. B. genau 50 Zeichen).

---

### 2. Exception-Inhalte werden nicht geprüft

Mutationen von String-Werten (z. B. Fehlermeldungen) bleiben oft unentdeckt.

➡️ Problem:
Tests prüfen nur, **dass** eine Exception geworfen wird, aber nicht **welche**.

---

### 3. Seiteneffekte werden nicht verifiziert

Beispiele:

* `SaveChangesAsync()` wird entfernt → Test erkennt es nicht
* Properties werden nicht gesetzt → Test erkennt es nicht

➡️ Problem:
Tests validieren nicht:

* Persistierung
* Zustandsänderungen
* Mapping-Ergebnisse

---

### 4. Infrastruktur-Code nur teilweise sinnvoll testbar

Insbesondere:

* `AppDbContext`
* `Program.cs`

➡️ Problem:
Ein Teil der Mutationen betrifft Code, der:

* schwer isoliert testbar ist
* oder nur begrenzten fachlichen Mehrwert hat

---

## Bewertung

Der aktuelle Zustand lässt sich wie folgt einordnen:

| Bereich        | Bewertung             |
| -------------- | --------------------- |
| Testabdeckung  | unvollständig         |
| Testqualität   | ausbaufähig           |
| Architektur    | grundsätzlich testbar |
| Mutation Score | unter Zielwert        |

➡️ Wichtig:
Die Tests sind **nicht grundsätzlich schlecht**, aber:

* zu wenig präzise
* nicht vollständig
* nicht ausreichend auf Randfälle ausgelegt

---

## Maßnahmen zur Verbesserung

### Priorität 1 – Service-Logik absichern

* Boundary-Tests ergänzen (z. B. exakt 50 Zeichen)
* Validierungslogik vollständig abdecken
* Rückgabewerte exakt prüfen

---

### Priorität 2 – Seiteneffekte testen

* Verifizieren von:

  * `SaveChangesAsync`
  * Änderungen an Entities
  * Statusfeldern (z. B. `ModifiedAt`, `IsDeleted`)

---

### Priorität 3 – Testabdeckung erhöhen

* Fokus auf:

  * Services
  * Repositories
* weniger Fokus auf:

  * `Program.cs`
  * rein technischen Boilerplate-Code

---

### Priorität 4 – Exception-Handling präzisieren

* gezielt prüfen:

  * Exception-Typ
  * relevante Fehlermeldungen (falls Teil des Vertrags)

---

### Priorität 5 – Infrastruktur bewusst behandeln

* entscheiden:

  * welche Teile sinnvoll getestet werden
  * welche ggf. ignoriert werden können (z. B. via Stryker-Config)

---

## Fazit

Der Mutation-Test zeigt klar:

* Die bestehende Testbasis deckt grundlegende Funktionalität ab
* Es fehlen jedoch:

  * präzise Assertions
  * Grenzwerttests
  * vollständige Abdeckung zentraler Logik

Der größte Hebel zur Verbesserung liegt **nicht in der Menge der Tests**, sondern in deren **Qualität und Zielgerichtetheit**.

---

Wenn du willst, kann ich dir als nächsten Schritt:

* konkrete Testfälle für z. B. `AddressService` formulieren
* oder dir zeigen, wie du gezielt von ~44 % auf >70 % kommst ohne unnötigen Overhead 🚀

## 2026-04-25

Das reine Vibe-Coding ist relativ monoton, weil hier grundsätzlich nur Anforderungen in ein passendes Schema gebracht werden, dann geprüft und schlussendlich wieder im Chat-Fenster vom Copilot/Claude landen. Als "Entwickler" sieht man die meiste Zeit nur zu. Aus Sicht eines Product Owner würde das schon etwas mehr sinn machen, allerdings ist auch die Implementierung ohne technisches Verständnis eher eine Blackbox. Debugging wird da sehr repetitiv und ohne konkrete Inputs haben die KI-Modelle teilweise Schwierigkeiten den Fehler aus den gegebenen Fehlermeldungen bzw. Logs herauszulesen.