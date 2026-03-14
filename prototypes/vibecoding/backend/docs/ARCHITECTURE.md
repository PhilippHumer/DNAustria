# Architecture Overview

Projects:
- `EventApp.Api` — presentation layer (Controllers, Mapping, Middleware)
- `EventApp.Core` — core interfaces and domain placeholders


Guiding rules: minimal invasive changes, no business logic in initial setup, tests required for health endpoint.- `EventApp.Infrastructure` — DbContext skeleton, export impl skeleton
