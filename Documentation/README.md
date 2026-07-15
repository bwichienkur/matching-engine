# EDDY.IS Platform Documentation

Enterprise-level reverse-engineered documentation for the EDDY Information Systems education lead-generation platform.

**Generated:** 2026-07-15  
**Repository:** `/workspace`  
**Subsystems:** FormsEngine, MatchingEngine, LeadEngine, VendorWebAPI

---

## Quick Start for New Engineers

1. Read [ExecutiveSummary.md](./ExecutiveSummary.md) for business context
2. Read [Architecture.md](./Architecture.md) for system design
3. Read [AI_CONTEXT.md](./AI_CONTEXT.md) for machine-readable context
4. Dive into subsystem docs based on your task

---

## Documentation Index

### Core Documents

| # | Document | Description |
|---|----------|-------------|
| 1 | [ExecutiveSummary.md](./ExecutiveSummary.md) | Business purpose, users, workflows, strengths/weaknesses |
| 2 | [Architecture.md](./Architecture.md) | Architecture style, violations, startup sequence |
| 3 | [BusinessProcesses.md](./BusinessProcesses.md) | 14 major business processes with sequence diagrams |
| 31 | [AI_CONTEXT.md](./AI_CONTEXT.md) | AI agent context summary |

### Subsystem Documentation

| Folder | Contents |
|--------|----------|
| [Projects/](./Projects/) | Per-project documentation (all 20+ projects) |
| [APIs/](./APIs/) | All HTTP/WCF endpoints (FormsEngine, MatchingEngine, VendorWebAPI) |
| [Database/](./Database/) | DbContexts, entities, tables, views, stored procedures, ER diagram |
| [Entities/](./Entities/) | Key entity documentation with business meaning |
| [Services/](./Services/) | All services, integrations, caching, logging |
| [Security/](./Security/) | Security review, authentication, recommendations |
| [Performance/](./Performance/) | Performance review, code quality, design patterns, testing |
| [Deployment/](./Deployment/) | CI/CD, IIS deployment, configuration reference |
| [Diagrams/](./Diagrams/) | Mermaid dependency, sequence, class, and flow diagrams |
| [Refactoring/](./Refactoring/) | Ranked refactoring recommendations |

---

## Project Coverage

| Solution | Projects Documented | Status |
|----------|-------------------|--------|
| FormsEngine (6 in sln + 7 orphan) | 13/13 | Complete |
| MatchingEngine (6 in sln + 7 orphan) | 13/13 | Complete |
| VendorWebAPI (6 in sln) | 6/6 | Complete |
| LeadEngine (1, no sln) | 1/1 | Complete |
| SQL CLR (separate sln) | 1/1 | Complete |

---

## Key Findings

- **Legacy .NET Framework 4.x** monolith with WCF inter-service communication
- **FormsEngine** is in transition: legacy `FormsEngine` facade + newer Core/Infrastructure DI layer
- **MatchingEngine** is performance-optimized with 60+ in-memory cache items
- **VendorWebAPI** is the only subsystem with API key authentication
- **LeadEngine** is a focused persistence library with no HTTP surface
- **Significant security gaps** on FormsEngine and MatchingEngine (no auth)
- **Technical debt** concentrated in god classes and WCF dependencies

---

## Diagrams

All diagrams use Mermaid syntax and are embedded in:
- [Architecture.md](./Architecture.md) — system architecture
- [BusinessProcesses.md](./BusinessProcesses.md) — process sequence diagrams
- [Database/README.md](./Database/README.md) — ER diagram
- [Diagrams/README.md](./Diagrams/README.md) — dependency graphs, class diagrams, flowcharts
