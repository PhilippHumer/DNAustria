# Prozess der Gemini-Frontend-Entwicklung
Es wurde versucht, die Frontend- und Backend-Entwicklung zu trennen, um den KI-Modellen die Arbeit zu erleichtern, indem der Kontext reduziert wurde.

## Aufbau des Versuchs
Diese Frontend-Entwicklung gehört zu der Backend-implementierung mit Raptor. 
Das Frontend wurde mit dem Copilot-Modell Gemini 3 Pro entwickelt. 
Als Startpunkt wurde der Startprompt verwendet, mit der Bemerkung, nur den Frontend-Code zu generieren.

## Erkenntnisse
- Laufende Angular-App ist relativ sschnell erstellt. Die Benutzbarkeit ist jedoch in keinem Fall gegeben, da es den Anschein hat, als ob das Modell keine visuellen Aspekte ohne Referenz versteht. Nach dem ersten Entwurf wurde mit Screenshots gearbeitet, um dem Modell die Fehler zu verdeutlichen. Diese wurde mehr oder weniger gut ausgebessert.
- Nach ein paar Iterationen, wurde die UI verbessert und es entstand eine benutzbare App. Die Funktionalität ist rudimentär, viele Features wie Validierung, Fehlermeldungen, Ladeanzeigen, etc. fehlen.
- Die API wurde zunächst ganz anders interpretiert als beim Backend-Projekt. Es wurden falsche Endpunkte und Datenstrukturen verwendet. 
- Codequalität ist akzeptabel, aber vieles (zb Desgin) ist hart ins Template codiert und bei weitem nicht so modular und wartbar wie von einem erfahrenen Entwickler erstellt. 

## Fazit
Zusammenfassend lässt sich sagen, dass die Trennung von Frontend- und Backend-Entwicklung den Prozess vereinfacht hat. Die ersten Versionen sind immer fehlerhaft und unbenutzbar. Durch Verkleinerung des Kontextes können spezifische Wünsche und Fehlerbehebungen ohne tiefes Angular-Wissen umgesetzt werden.
