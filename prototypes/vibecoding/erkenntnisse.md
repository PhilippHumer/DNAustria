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