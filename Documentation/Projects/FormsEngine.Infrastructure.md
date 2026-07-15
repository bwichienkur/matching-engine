# FormsEngine — EDDY.IS.FormsEngine.Infastructure

**Path:** `FormsEngine/EDDY.IS.FormsEngine.Infastructure/`  
**Type:** Class Library (.NET Framework 4.8)  
**Note:** Project name contains typo ("Infastructure" instead of "Infrastructure")

## Purpose

Repository implementations and inbound/outbound mappers bridging Core interfaces to legacy RF layer and WCF clients.

## Responsibilities

- 11 repository implementations (`Repositories/*.cs`)
- Inbound mappers: Core models → LeadEngine DTOs (`Mappers/Inbound/`)
- Outbound mappers: External responses → Core models (`Mappers/Outbound/`)

## Dependencies

- EDDY.IS.FormsEngine.Core
- EDDY.IS.FormsEngine.RF
- EDDY.IS.LeadEngine
- Newtonsoft.Json 13.0.1

## Important Classes

| Repository | Delegates To |
|------------|-------------|
| `LeadRepository` | `LeadSaveManger.Execute` |
| `ProgramRepository` | `MatchingServiceClient` WCF |
| `ProspectRepository` | `ProspectServiceClient` WCF |
| `FormValidationRepository` | `FormsEngine.QuickCheckValidateForm` |
| `SessionRepository` | `FESession` cache |
| `InstitutionRepository` | Matching Engine WCF |
| `ProgramValidationRepository` | Matching Engine validation |
| `UserSelectionRepository` | Matching Engine |
| `HtmlRenderingStrategyRepository` | `RenderingStrategyDataService` |
| `MetaDataRepository` | `ResourceMetaDataService` |
| `LocationValidationRepository` | Validation service |

## Mappers

- `LeadCreateRequestMapper` — `FormInput` → `LeadCreateRequest`
- `LeadMapper` — `LeadDTO` → Core `Lead` model
- `RawPostDataMapper` — form data → `RawPostDataDTO`
- `ProgramMapper`, `InstitutionMapper`, `MatchMapper`, etc.

## Potential Improvements

1. Rename project to `Infrastructure` (breaking change)
2. Replace WCF delegation with HttpClient-based adapters
3. Add resilience patterns (retry, circuit breaker) on external calls
