# FormsEngine — EDDY.IS.FormsEngine.RF (Legacy Core)

**Path:** `FormsEngine/EDDY.IS.FormsEngine.RF/`  
**Type:** Class Library (.NET Framework 4.5.1)  
**Assembly name:** `EDDY.IS.FormsEngine`

## Purpose

Legacy core engine containing the original business logic facade, EF6 data access, WCF service clients, caching, and DTO generation. This is where the majority of FormsEngine business logic still lives.

## Responsibilities

- `FormsEngine` partial class — templates, validation, submission, matching orchestration
- `LeadSaveManger` — lead creation pipeline
- `FESession` — session emulation via ASP.NET Cache
- `*DataService` classes — EF6 data access for templates, campaigns, submissions
- WCF integration via `FormsRelatedServices` partial classes
- Auto-generated DTOs via T4 templates (`DTO.tt`)
- `FEEntitiesContainer` DbContext — 40+ DbSets

## Dependencies

- EDDY.IS.LeadEngine
- EDDY.IS.Base, EDDY.IS.Core.Logging, EDDY.IS.Util, EDDY.IS.Validation (NuGet)
- EDDY.IS.EmsLeadEngine.Entities 1.0.3

## Important Classes

| Class | Files | Lines (approx) | Role |
|-------|-------|----------------|------|
| `FormsEngine` | `FormsEngine.cs`, `FormsEngineAPI.cs`, `FormsEngineOpenMail.cs` | 3000+ | Main facade |
| `FormsEngineBase` | `FormsEngineBase.cs` | — | DAO + engine holders |
| `LeadSaveManger` | `LeadSaveManager.cs` | 500+ | Lead save orchestration |
| `FormsRelatedServices` | `FormsRelatedServices.*.cs` (7 files) | — | WCF client wrappers |
| `FEEntitiesContainer` | `DataModel/FEModel.Context.cs` | — | EF6 DbContext |
| `TemplateDataService` | `DataModel/TemplateDataService.cs` | — | Template CRUD |
| `SubmissionDataService` | `DataModel/SubmissionDataService.cs` | — | Submission persistence |
| `FormsEngineCache` | `FormsEngineCache.cs` | — | Local + Redis caching |

## Configuration

- `App.config` — `FEEntitiesContainer` connection, cache timeouts, WCF bindings

## External Services

WCF clients: MatchingEngine, ProspectService, ValidationService, LeadScoringService, PixelsService, Five9Service, LeadPingService. HTTP: EMS Lead Engine.

## Potential Improvements

1. Decompose `FormsEngine` partial into domain services
2. Extract WCF clients behind interfaces for testability
3. Migrate EF Database First to Code First or Dapper for hot paths
4. Eliminate static singletons (`LeadSaveManger`, `FormsEngineCache`)
