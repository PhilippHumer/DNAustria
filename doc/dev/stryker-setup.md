# 🧪 Stryker Quickstart (.NET)

## 1️⃣ Installation

Global installieren (einmalig):

```bash
dotnet tool install -g dotnet-stryker
```

Optional prüfen:

```bash
dotnet stryker --version
```

---

## 2️⃣ Initialisieren

Im **Projekt-Root (Backend)** ausführen:

```bash
dotnet stryker init
```

👉 erzeugt eine `stryker-config.json`

---

## 3️⃣ Wichtige Config (empfohlen)

Passe die Config minimal an:

```json
{
  "stryker-config": {
    "solution": "DNAustria.sln",
    "test-projects": [
      "tests/DNAustria.Api.Tests/DNAustria.Api.Tests.csproj"
    ],
    "reporters": [
      "Progress",
      "Html",
      "Json"
    ],
    "report-file-name": "mutation-report",
    "thresholds": {
      "high": 80,
      "low": 60,
      "break": 50
    }
  }
}
```

👉 Wichtig:

* `test-projects` MUSS gesetzt sein (sonst passiert oft „nichts“)

---

## 4️⃣ Ausführen

```bash
dotnet stryker
```

👉 dauert deutlich länger als normale Tests (je nach Projekt mehrere Minuten)

---

## 5️⃣ Ergebnisse ansehen

Nach dem Run:

```
StrykerOutput/<timestamp>/reports/
```

Dort findest du:

* `mutation-report.html` → **visueller Report**
* `mutation-report.json` → **für Auswertung / Doku**

👉 HTML öffnen:

```bash
open StrykerOutput/.../reports/mutation-report.html
```

---

## 6️⃣ Interpretation (ultrakurz)

* ✅ **Killed** → Test gut
* ❌ **Survived** → Test fehlt / zu schwach
* ⚠️ **NoCoverage** → Code nicht getestet

👉 Ziel:

* Mutation Score > **60% (ok)**
* > **80% (gut)**

---

## 7️⃣ Typischer Workflow im Projekt

```bash
dotnet build
dotnet test
dotnet stryker
```

👉 Danach:

* HTML anschauen
* gezielt Tests verbessern
* erneut laufen lassen

---

## 8️⃣ Best Practices (aus Erfahrung)

* Erst **Coverage fixen**, dann Stryker
* Fokus auf:

  * Services (Business Logic)
  * Validierung
  * Edge Cases
* Infra / EF / Migrations eher ignorieren (optional)

