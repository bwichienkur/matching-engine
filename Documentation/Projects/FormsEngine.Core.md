# FormsEngine — EDDY.IS.FormsEngine.Core

**Path:** `FormsEngine/EDDY.IS.FormsEngine.Core/`  
**Type:** Class Library (.NET Framework 4.8)

## Purpose

Newer domain layer introduced for School Picker and component rendering flows. Defines service interfaces, domain models, DTOs, and service implementations following a cleaner separation pattern.

## Responsibilities

- 25+ service interfaces (`Interfaces/I*Service.cs`)
- 11 repository interfaces (`Interfaces/Repositories/I*Repository.cs`)
- Service implementations (`Services/*.cs`)
- Domain models (`Models/`)
- Request/response DTOs (`DTO/`)
- Mappers (`Mappers/`)
- Constants and enums (`Constants.cs`, `Enums.cs`)

## Dependencies

- EDDY.IS.FormsEngine.RF (legacy delegation)
- EDDY.IS.LeadEngine
- Microsoft.AspNet.Mvc 5.3 (for `Controller` types in some services)
- RazorEngine 3.10.0
- Newtonsoft.Json 13.0.1

## Important Classes

| Class | Interface | Role |
|-------|-----------|------|
| `SubmissionService` | `ISubmissionService` | School Picker submission pipeline |
| `FailedMatchReplacementService` | `IFailedMatchReplacementService` | Replacement school suggestions |
| `ComponentCompilingService` | `IComponentCompilingService` | Pre-compile Razor components |
| `ComponentRenderingService` | `IComponentRenderingService` | Render compiled components |
| `TemplatingEngineService` | `ITemplatingEngineService` | RazorEngine wrapper |
| `LeadSubmissionService` | `ILeadSubmissionService` | Lead save orchestration |
| `MatchValidationService` | `IMatchValidationService` | Program match validation |
| `ThankYouPageService` | `IThankYouPageService` | Thank-you page assembly |
| `SessionService` | `ISessionService` | Session abstraction over FESession |

## Configuration

- `app.config` — assembly binding redirects only; runtime config from host `Web.config`

## Potential Improvements

1. Remove dependency on FormsEngine.RF (repositories should not delegate to legacy facade)
2. Extract pure domain models without MVC dependencies
3. Add comprehensive unit test coverage (currently only 1 test in CoreTests project)
