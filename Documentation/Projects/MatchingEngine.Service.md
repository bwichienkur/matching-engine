# MatchingEngine — EDDY.IS.MatchingEngine.Service

**Path:** `MatchingEngine/EDDY.IS.MatchingEngine.Service/`  
**Type:** ASP.NET WCF Web Application (.NET Framework 4.8)

## Purpose

WCF/JSON HTTP host exposing the matching engine as a service to FormsEngine, VendorWebAPI, and SEO allocation console.

## Responsibilities

- `MatchingService.svc` — WCF service implementation (30+ operations)
- `IMatchingService` — service contract with `[WebInvoke]` JSON endpoints
- `MatchingEngineStartup.AppInitialize` — cache preload on startup
- `CacheInfo.aspx` — diagnostic page for HttpRuntime.Cache keys
- Cross-domain policy files (`crossdomain.xml`, `clientaccesspolicy.xml`)

## Dependencies

- EDDY.IS.MatchingEngine
- EDDY.IS.MatchingEngine.Logging
- EDDY.IS.Base 2.0.4, EDDY.IS.LocalCache 2.0.0
- EntityFramework 6.5.1, Newtonsoft.Json 13.0.1

## WCF Operations (from IMatchingService)

| Category | Operations |
|----------|------------|
| Directory | `GetInstitutions`, `GetPrograms`, `GetCampuses`, `GetCategories`, `GetSubjects`, `GetSpecialties`, `GetProgramLevels`, `GetCampusTypes`, `GetFacetedNavigation` |
| Wizard | `GetWizardMatches`, `GetFormPrograms` |
| Cross-sell | `GetProgramsForCrossSell` |
| Validation | `ValidateProgram`, `ValidateAPIProgram` |
| Details | `GetInstitutionDetails`, `GetProgramDetails`, `GetProgramDisplayGroupDetails` |
| Special | `GetNeoResponse`, `GetApolloCampuses`, `GetSiteMapGeoInfo`, `GetCampaignDetailByTrackId` |
| Fallbacks | `GetCategoriesAllIfNone`, `GetSubjectsAllIfNone`, etc. |

## Configuration

- `Web.config` — Nexus, EddyLogging, MatchLoggingModelContainer connections
- WCF LeadPing client endpoint
- `PerformanceLogging*`, `MatchResponseLogging*` toggles

## IIS Deployment

- App pool: `EDDY.IS.MatchingEngine.Service`
- Path: `F:\inetpub\wwwroot\EDDY.IS.MatchingEngine.Service`

## Potential Improvements

1. Add authentication (API key or mutual TLS)
2. Migrate WCF to ASP.NET Core minimal API or gRPC
3. Disable `includeExceptionDetailInFaults` in production
4. Wire `AppInitialize` explicitly in application startup
