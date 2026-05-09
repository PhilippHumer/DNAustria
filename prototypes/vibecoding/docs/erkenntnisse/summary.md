# Erkenntnisse aus dem Vibecoding-Team

## Zusmmenfassung der ausarbeitung vom DNAustria Portal mit reinem Vibecoding

### 1. Einleitung

Im Rahmen der Lehrveranstaltung wurde untersucht, wie sich moderne KI-gestützte Entwicklungswerkzeuge in einem Softwareprojekt praktisch einsetzen lassen. Der Fokus lag dabei insbesondere auf sogenanntem „Vibecoding“, also einer Arbeitsweise, bei der Anforderungen durch den Menschen formuliert und anschließend durch KI-Modelle bzw. Coding-Agenten in Code überführt werden.

Ziel war es nicht nur, funktionierende Softwareartefakte zu erzeugen, sondern auch die Grenzen, Stärken und Schwächen dieser Vorgehensweise besser zu verstehen. Dabei wurden verschiedene Modelle und Agenten getestet, Anforderungen in Feature-Slices strukturiert, API-Definitionen verglichen und die Qualität des erzeugten Codes unter anderem mithilfe von Mutation Testing analysiert.

Die folgenden Abschnitte fassen die wichtigsten Erkenntnisse des Teams zusammen und ordnen diese hinsichtlich praktischer Softwareentwicklung, Anforderungsmanagement, Architekturentscheidungen und Qualitätssicherung ein.

---

### 2. Kontextfenster und Arbeitsweise von Large Language Models

Eine zentrale Erkenntnis zu Beginn des Projekts war, dass Large Language Models nicht über ein unbegrenztes Arbeitsgedächtnis verfügen. Stattdessen arbeiten sie innerhalb eines sogenannten Context Windows. In diesem Kontextfenster konkurrieren unterschiedliche Informationsquellen um denselben begrenzten Platz, unter anderem:

* System- und Entwickleranweisungen,
* Benutzereingaben,
* bisherige Chat-Historie,
* Ausgaben von Tools,
* Fehlermeldungen,
* Codeausschnitte und Dokumentation.

Dadurch wurde deutlich, dass die Qualität der Eingaben einen direkten Einfluss auf die Qualität der Ergebnisse hat. Besonders relevant sind dabei die semantische Dichte der Informationen, die Reihenfolge der Angaben, der Anteil redundanter oder irrelevanter Informationen sowie die klare Abgrenzung einzelner Aufgaben.

Ein unstrukturierter Prompt mit vielen gemischten Anforderungen führt häufig dazu, dass das Modell Details übersieht oder Annahmen trifft, die fachlich oder technisch nicht korrekt sind. Umgekehrt verbessern klar strukturierte, präzise und kontextuell gut vorbereitete Eingaben die Qualität der Antworten deutlich.

---

### 3. Optimierung der Kontextnutzung

Aus den ersten Experimenten ergaben sich mehrere Maßnahmen, mit denen die Nutzung des Kontextfensters verbessert werden konnte.

#### 3.1 Projektmanifest

Als hilfreich erwies sich ein zentrales Projektmanifest. Dieses Dokument beschreibt grundlegende Informationen zum Projekt, etwa Architektur, Features, Coding-Conventions, Projektstruktur und technische Rahmenbedingungen. Dadurch muss nicht in jedem Prompt erneut die gesamte Projektlogik erklärt werden. Gleichzeitig reduziert ein solches Manifest das Risiko, dass das Modell bei späteren Aufgaben von falschen Annahmen ausgeht.

#### 3.2 Feature-Scoped Prompts

Statt eine gesamte Applikation über einen großen Prompt beschreiben zu lassen, wurden Anforderungen zunehmend in kleinere Feature-Slices zerlegt. Diese Vorgehensweise war deutlich besser handhabbar. Einzelne Features konnten gezielter beschrieben, umgesetzt und überprüft werden.

Auf Feature-Ebene wurden dabei typischerweise folgende Informationen definiert:

* fachlicher Kontext,
* technische Rahmenbedingungen,
* Schnittstellen,
* harte Einschränkungen,
* erwarteter Zielzustand,
* Beispiele,
* Erfolgskriterien.

Diese stärkere Eingrenzung reduzierte Interpretationsspielräume und führte insgesamt zu nachvollziehbareren Ergebnissen.

#### 3.3 State-Compression

Ein weiterer hilfreicher Ansatz war die regelmäßige Zusammenfassung des aktuellen Projektzustands. Nach einigen Iterationen wurde der Status strukturiert festgehalten, um die relevanten Informationen für die weitere Arbeit kompakt verfügbar zu machen. Diese sogenannte State-Compression verhindert, dass wichtige Entscheidungen in der Chat-Historie verloren gehen oder durch späteren Kontext verdrängt werden.

---

### 4. Rollenverteilung zwischen Mensch und KI

Im Projekt zeigte sich, dass eine klare Rollenverteilung zwischen Mensch und KI entscheidend ist. Die KI kann viele Tätigkeiten unterstützen, sollte aber nicht in allen Bereichen die alleinige Verantwortung übernehmen.

#### 4.1 Product Owner und Requirements Engineer

Die Rolle des Product Owners bzw. Requirements Engineers sollte primär beim Menschen bleiben. Fachliche Anforderungen, Prioritäten und Zielbilder müssen durch das Projektteam definiert werden. Die KI kann hier unterstützend eingesetzt werden, etwa um Formulierungen zu schärfen, Inkonsistenzen aufzudecken oder Anforderungen verständlicher zu strukturieren.

Gerade bei der Formulierung von Anforderungen zeigte sich, dass KI-Modelle hilfreich sein können, wenn sie gezielt nach unklaren Punkten, fehlenden Edge-Cases oder möglichen Widersprüchen gefragt werden. Besonders wirksam waren Fragestellungen wie:

* „Welche Aspekte wurden hier noch nicht betrachtet?“
* „Welche Edge-Cases können auftreten?“
* „Welche Annahmen sind in dieser Anforderung enthalten?“

Solche Fragen führen dazu, dass das Modell weniger stark dem offensichtlichen Happy Path folgt und Anforderungen kritischer betrachtet.

#### 4.2 Softwarearchitektur

Architekturentscheidungen sollten ebenfalls nicht vollständig an KI-Modelle ausgelagert werden. Die Modelle können als Sparringspartner dienen, Vorschläge bewerten oder technische Alternativen aufzeigen. Die finale Entscheidung über Abhängigkeiten, Entitäten, technische Standards und nicht-funktionale Anforderungen sollte jedoch durch das Projektteam erfolgen.

Im Projekt zeigte sich, dass KI-Modelle insbesondere bei locker definierten Funktionalitäten oder unklaren Endpunktstrukturen zu inkonsistenten Lösungen neigen. Auch fachliche Annahmen über Beziehungen zwischen Entitäten wurden teilweise falsch interpretiert. Daher ist menschliche Kontrolle in der Architekturarbeit weiterhin wesentlich.

#### 4.3 Code-Review

Für Code-Reviews erwies sich KI dagegen als besonders nützlich. Modelle können Code schnell analysieren, potenzielle Fehlerstellen identifizieren, alternative Implementierungen vorschlagen und auf Inkonsistenzen hinweisen. Die Geschwindigkeit ist hier ein klarer Vorteil. Dennoch bleibt auch in diesem Bereich eine fachliche Bewertung durch Menschen notwendig, insbesondere wenn es um Architekturkonformität oder fachliche Korrektheit geht.

---

### 5. Strukturierung von Feature-Slices

Im weiteren Projektverlauf wurde eine wiederverwendbare Struktur für Feature-Slices entwickelt. Diese Struktur erwies sich als praktikabler Kompromiss zwischen Flexibilität und Verbindlichkeit.

#### 5.1 Context

Der Abschnitt „Context“ beschreibt den allgemeinen und technischen Projektkontext. Dazu gehören die fachliche Beschreibung des Projekts, verwendete Frameworks und Versionen sowie die Projektstruktur. Besonders bei paralleler Entwicklung mehrerer Features ist dieser Abschnitt wichtig, damit sich die Implementierungen nicht in unterschiedliche Richtungen entwickeln.

#### 5.2 Intent

Der Abschnitt „Intent“ definiert das Ziel des jeweiligen Feature-Slices. Hier sollte möglichst klar beschrieben werden, was umgesetzt werden soll. Je präziser der Zielzustand formuliert ist, desto geringer ist der Interpretationsspielraum für das Modell.

#### 5.3 Constraints

Constraints beschreiben harte Grenzen und nicht verhandelbare Entscheidungen. Dazu zählen beispielsweise Architekturvorgaben, Datenbankentscheidungen, Framework-Versionen oder verbotene Workarounds. Dieser Abschnitt wurde besonders wichtig, da KI-Modelle bei Problemen gelegentlich versuchen, den einfachsten Weg zu wählen, etwa durch Framework-Upgrades, Downgrades oder temporäre In-Memory-Lösungen.

Gerade im Debugging zeigte sich, dass Modelle ohne klare Constraints schnell kreativ werden. Das kann hilfreich sein, führt aber auch dazu, dass ursprüngliche Projektentscheidungen umgangen werden.

#### 5.4 Examples

Beispiele helfen dabei, erwartete Ergebnisse konkreter zu machen. Besonders gut funktionierten Beispieldefinitionen für API-Endpunkte, inklusive erwartetem Response-Objekt, HTTP-Statuscode und Fehlerverhalten. Dadurch wurde die gewünschte Semantik für das Modell greifbarer.

#### 5.5 Verification

Der Abschnitt „Verification“ definiert messbare Erfolgskriterien. Ein Beispiel dafür ist: „Das Backend-Setup gilt als abgeschlossen, wenn `dotnet build` ohne Fehler durchläuft.“ Weitere mögliche Kriterien betreffen Datenbankmigrationen, Docker-Setup oder ausführbare Binaries.

Solche Kriterien sind wichtig, weil sie nicht nur die Implementierung beschreiben, sondern auch festlegen, wann ein Ergebnis tatsächlich als abgeschlossen gelten kann.

#### 5.6 Documentation

Abschließend wurde das Modell aufgefordert, die jeweilige Implementierung zu dokumentieren. Bei offenen Formulierungen erstellten oder ergänzten die Modelle meist eine README-Datei. Diese Dokumentation ist hilfreich, sollte aber ebenfalls überprüft werden, da Modelle gelegentlich Funktionen dokumentieren, die nur teilweise oder anders implementiert wurden.

---

### 6. Erfahrungen mit verschiedenen Modellen und Agenten

Im Projekt wurden mehrere Modelle bzw. Agenten miteinander verglichen. Dabei zeigte sich, dass die Qualität und Geschwindigkeit der Ergebnisse deutlich variieren kann.

Einige kleinere bzw. günstigere Modelle konnten gut definierte Features nach mehreren Iterationen grundsätzlich umsetzen. Die Ergebnisse waren jedoch inkonsistenter und benötigten mehr Nacharbeit. Teilweise schrieben die Modelle zunächst nicht funktionsfähigen Code, analysierten anschließend die Fehler und korrigierten diese in weiteren Iterationen.

Besonders schwierig waren längere Debugging-Zyklen mit Compiler-Output. Häufig mussten mehrere Ansätze ausprobiert werden, bevor ein Fehler tatsächlich behoben war. Dabei wurden Fehlermeldungen nicht immer korrekt interpretiert oder relevante Informationen übersehen.

Leistungsfähigere Modelle und spezialisierte Coding-Agenten machten während der Entwicklung tendenziell weniger Fehler und erreichten schneller brauchbare Ergebnisse. Der zeitliche Vorteil war allerdings nicht immer konstant. Auch bei gleichem Input wählten Modelle nicht zwingend denselben Lösungsweg. Das zeigt, dass KI-gestützte Entwicklung trotz strukturierter Eingaben eine gewisse Varianz aufweist.

Ein weiterer Unterschied zeigte sich bei Rückfragen. Schwächere Modelle stellten häufiger unscharfe oder wenig zielgerichtete Fragen. Teilweise wurden bereits gegebene Informationen nicht berücksichtigt oder falsch interpretiert. Dadurch stieg der Aufwand für Revisionen.

---

### 7. Vergleich der API-Endpunkte

Ein praktischer Vergleich zweier `swagger.json`-Dateien zeigte, dass trotz vorheriger Definition der Endpunkte relevante Unterschiede zwischen den Projekten entstanden sind. Die Abweichungen betrafen weniger einzelne Implementierungsdetails, sondern vor allem Konsistenz, Namenskonventionen und Datenmodelle.

Zu den wichtigsten Unterschieden gehörten:

* unterschiedliche Pfadkonventionen,
* gemischte Groß- und Kleinschreibung in einem Projekt,
* uneinheitliche Verwendung des `/api/`-Präfixes,
* verschiedene ID-Typen,
* abweichende Schema- und DTO-Namen,
* unterschiedliche Feldnamen bei ähnlicher fachlicher Bedeutung,
* verschiedene Response-Codes,
* unterschiedliche Fehlerrepräsentationen,
* zusätzliche Endpunkte, die jeweils nur in einem Projekt vorhanden waren.

Besonders auffällig war, dass ein Projekt integerbasierte IDs verwendete, während das andere UUIDs nutzte. Auch bei Events unterschieden sich Feldnamen und Semantik, etwa durch Begriffe wie `name` und `link` im Vergleich zu `title` und `eventLink`.

Diese Analyse verdeutlichte, dass KI-gestützte Entwicklung zwar schnell zu funktionierenden Endpunkten führen kann, Konsistenz über mehrere Features oder Projektteile hinweg aber explizit abgesichert werden muss. API-Konventionen sollten daher früh dokumentiert und regelmäßig überprüft werden.

---

### 8. Qualitätssicherung durch Mutation Testing

Zur Bewertung der Codequalität wurde Mutation Testing mit Stryker eingesetzt. Ziel dieser Methode ist es, die Aussagekraft bestehender Tests zu überprüfen. Dabei werden kleine Veränderungen am Code vorgenommen, sogenannte Mutationen. Anschließend wird geprüft, ob die Tests diese Veränderungen erkennen.

Die Analyse ergab folgende Ergebnisse:

| Kennzahl             |   Ergebnis |
| -------------------- | ---------: |
| Gesamtanzahl Mutants |       1010 |
| Killed               |        407 |
| Survived             |        193 |
| No Coverage          |        319 |
| Ignored              |         87 |
| Compile Errors       |          4 |
| Mutation Score       | ca. 44,3 % |

Der definierte Mindestwert von 60 % wurde damit nicht erreicht. Dieses Ergebnis zeigt, dass die Tests zwar grundlegende Funktionalitäten abdecken, aber noch deutlicher Verbesserungsbedarf besteht.

#### 8.1 Fehlende Testabdeckung

Ein erheblicher Anteil der Mutationen wurde gar nicht ausgeführt, weil die entsprechenden Codebereiche nicht durch Tests abgedeckt waren. Das bedeutet, dass ein Teil des niedrigen Mutation Scores nicht durch schlechte Tests verursacht wurde, sondern durch fehlende Tests.

#### 8.2 Unzureichende Testtiefe

Zusätzlich überlebten zahlreiche Mutationen, obwohl die betroffenen Codebereiche ausgeführt wurden. Das weist darauf hin, dass manche Tests zwar Methoden aufrufen, aber Ergebnisse, Seiteneffekte oder Grenzfälle nicht präzise genug prüfen.

Typische Schwächen waren:

* fehlende Boundary-Tests,
* ungenaue Prüfung von Exception-Inhalten,
* fehlende Verifikation von Seiteneffekten,
* unvollständige Prüfung von Mapping-Ergebnissen,
* geringe Testtiefe bei Service-Logik.

#### 8.3 Kritische Komponenten

Die meisten überlebenden Mutationen konzentrierten sich auf zentrale Klassen wie `AppDbContext.cs`, `EventService.cs`, `AddressService.cs`, `ContactService.cs`, `LocationService.cs` und `Program.cs`.

Dabei ist jedoch zu berücksichtigen, dass Infrastrukturcode wie `AppDbContext` oder `Program.cs` nur bedingt denselben fachlichen Testwert besitzt wie Service-Logik. Hier sollte bewusst entschieden werden, welche Teile sinnvoll getestet und welche gegebenenfalls über die Stryker-Konfiguration ausgeschlossen werden.

#### 8.4 Maßnahmen zur Verbesserung

Der größte Hebel liegt nicht in einer bloßen Erhöhung der Testanzahl, sondern in zielgerichteteren Tests. Besonders wichtig sind:

* Ergänzung von Grenzwerttests,
* präzisere Assertions,
* Prüfung von Rückgabewerten,
* Verifikation von Persistierung und Zustandsänderungen,
* gezielte Tests für Validierungslogik,
* bewusster Umgang mit Infrastrukturcode.

Die Analyse zeigt somit, dass Mutation Testing nicht nur eine Metrik liefert, sondern konkrete Hinweise darauf gibt, wo Tests fachlich zu schwach oder technisch unvollständig sind.

---

### 9. Reflexion zur Arbeitsweise „Vibecoding“

Das reine Vibecoding wurde im Projektverlauf zunehmend als monoton wahrgenommen. Viele Tätigkeiten bestanden darin, Anforderungen in ein geeignetes Schema zu bringen, diese von einem Modell prüfen zu lassen und anschließend in ein Coding-Tool zu übertragen. Während der eigentlichen Implementierung nimmt der Mensch häufig eine beobachtende Rolle ein.

Aus Sicht eines Product Owners kann diese Arbeitsweise durchaus sinnvoll sein, da fachliche Anforderungen stärker in den Vordergrund rücken. Gleichzeitig bleibt die Implementierung ohne technisches Verständnis weitgehend eine Blackbox. Besonders im Debugging wird deutlich, dass technische Grundlagen weiterhin wichtig sind. Ohne Verständnis für Fehlermeldungen, Logs, Architektur oder Frameworks ist es schwierig, die Vorschläge der KI kritisch zu bewerten oder gezielt zu korrigieren.

Im Rückblick wäre mit mehr praktischer Entwicklungserfahrung vermutlich ein besseres Ergebnis möglich gewesen. Teilweise fehlte das Gefühl für sinnvolle Umsetzungen, Implementierungsdetails und potenzielle Fehlerquellen. Gerade diese Erfahrung ist wichtig, um KI-generierte Vorschläge nicht nur anzunehmen, sondern aktiv zu steuern.

---

### 10. Fazit

Die Arbeit mit KI-gestützten Entwicklungswerkzeugen zeigte deutlich, dass solche Modelle ein großes Potenzial für Softwareprojekte besitzen. Sie können Anforderungen strukturieren, Code generieren, Reviews unterstützen, Tests vorschlagen und Dokumentation erstellen. Besonders bei klar abgegrenzten Feature-Slices liefern leistungsfähige Modelle brauchbare Ergebnisse und können Entwicklungsprozesse beschleunigen.

Gleichzeitig wurde sichtbar, dass KI-Modelle keine vollständige Verantwortung für fachliche, architektonische oder qualitätsbezogene Entscheidungen übernehmen sollten. Ohne klare Vorgaben neigen sie zu Annahmen, Inkonsistenzen oder pragmatischen Workarounds, die nicht immer zum Projektziel passen. Der Mensch bleibt daher insbesondere in den Rollen Requirements Engineering, Architektur, Qualitätssicherung und fachliche Bewertung zentral.

Eine wesentliche Erkenntnis ist, dass erfolgreiche KI-gestützte Entwicklung weniger von einzelnen Prompts abhängt, sondern von einem strukturierten Arbeitsprozess. Dazu gehören ein gepflegtes Projektmanifest, sauber definierte Feature-Slices, klare Constraints, regelmäßige State-Compression und konkrete Verifikationskriterien.

Insgesamt kann Vibecoding als produktive Ergänzung zur klassischen Softwareentwicklung betrachtet werden, nicht jedoch als vollständiger Ersatz für technisches Verständnis und methodisches Vorgehen. Die besten Ergebnisse entstehen dort, wo Mensch und KI klar abgegrenzte Rollen einnehmen: Der Mensch definiert Ziel, Kontext und Qualitätsanspruch; die KI unterstützt bei Umsetzung, Analyse und Iteration.
