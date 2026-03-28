# 🚀 Quickstart: SonarQube + Coverage (DNAustria Backend)

## Ziel

* Code-Analyse + Test Coverage in SonarQube
* Einfache lokale Ausführung für alle im Team
* Deterministischer Coverage-Report (`./coverage.opencover.xml`)

---

# 1. SonarQube lokal starten (einmalig)

Am einfachsten via Docker:

```bash
docker run -d --name sonarqube \
  -p 9000:9000 \
  sonarqube:lts
```

Dann im Browser öffnen:

```
http://localhost:9000
```

Login:

* user: `admin`
* password: `admin` → Passwort ändern

## Projekt anlegen

* Neues Projekt erstellen
* **Project Key merken** (z. B. `DNAustira-Vibecode`)
* **Token generieren** (wird später gebraucht)

---

# 2. Voraussetzung im Projekt

Im Testprojekt muss **Coverlet MSBuild** installiert sein:

```bash
dotnet add tests/DNAustria.Api.Tests/DNAustria.Api.Tests.csproj package coverlet.msbuild
```

---

# 3. Analyse + Coverage ausführen

## Schritt 1: Scan starten

```bash
dotnet sonarscanner begin \
  /k:"DNAustira-Vibecode" \
  /d:sonar.host.url="http://localhost:9000" \
  /d:sonar.token="DEIN_TOKEN" \
  /d:sonar.cs.opencover.reportsPaths="tests/DNAustria.Api.Tests/coverage.opencover.xml"
```

---

## Schritt 2: Build

```bash
dotnet build
```

---

## Schritt 3: Tests + Coverage

```bash
dotnet test tests/DNAustria.Api.Tests/DNAustria.Api.Tests.csproj \
  --no-build \
  /p:CollectCoverage=true \
  /p:CoverletOutput=tests/DNAustria.Api.Tests/coverage.opencover.xml \
  /p:CoverletOutputFormat=opencover
```

👉 Wichtig:

* Output ist bewusst **fix im Testprojekt**
* keine verschachtelten `TestResults/...` Pfade mehr

---

## Schritt 4: Scan beenden

```bash
dotnet sonarscanner end \
  /d:sonar.token="DEIN_TOKEN"
```

---

# 4. Ergebnis prüfen

Im SonarQube Dashboard:

* ✅ Coverage wird angezeigt (nicht mehr 0%)
* ✅ Issues / Code Smells sichtbar
* ❌ Kein Fehler mehr wie:

  ```
  Could not find any coverage report file
  ```

---

# 5. Optional: Einzeiler für den Alltag

```bash
dotnet sonarscanner begin \
  /k:"DNAustira-Vibecode" \
  /d:sonar.host.url="http://localhost:9000" \
  /d:sonar.token="$SONAR_TOKEN" \
  /d:sonar.cs.opencover.reportsPaths="tests/DNAustria.Api.Tests/coverage.opencover.xml" \
&& dotnet build \
&& dotnet test tests/DNAustria.Api.Tests/DNAustria.Api.Tests.csproj --no-build \
   /p:CollectCoverage=true \
   /p:CoverletOutput=tests/DNAustria.Api.Tests/coverage.opencover.xml \
   /p:CoverletOutputFormat=opencover \
&& dotnet sonarscanner end /d:sonar.token="$SONAR_TOKEN"
```
