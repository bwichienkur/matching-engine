# LeadEngine — EDDY.IS.LeadEngine

**Path:** `LeadEngine/EDDY.IS.LeadEngine-RF/`  
**Type:** Class Library (.NET Framework 4.8)  
**No solution file** — referenced by FormsEngine solution

## Purpose

Focused persistence and normalization library for education leads. Maps form/API input to `LeadDTO`, assigns realtime delivery status codes, and writes to Nexus SQL Server.

## Responsibilities

- `LeadEngine` facade — `CreateLead`, `UpdateLead`, `UpdateLeads`, `CreateLeadDTO`
- Field normalization (phone, zip, TCPA text, city/state from zip)
- Realtime delivery status assignment (20+ status codes)
- Test lead detection
- EF6 persistence to `dbo.Lead`, `dbo.BetaLead`, `dbo.RawPostData`
- Stored procedure calls for post-save linking

## Dependencies

- EDDY.IS.Base 2.0.3, EDDY.IS.Core.Logging 2.0.3
- EDDY.IS.Util 2.0.0, EDDY.IS.Validation 2.0.0
- EntityFramework 6.1.3

## Important Classes

| Class | Role |
|-------|------|
| `LeadEngine` | Public facade (~490 lines) |
| `LeadDataService` | Production lead CRUD + SP calls |
| `BetaLeadDataService` | Beta environment inserts |
| `RawPostDataService` | Raw HTTP post capture |
| `Converter` | Entity ↔ DTO mapping |
| `Nexus_Lead_Entities` | EF6 DbContext |

## Configuration

- `App.config` — `Nexus_Lead_Entities` connection string
- Runtime settings from host: `IsSpamAllowedForDelivery`, `CreativePortalUrl`

## External Services

- `ValidationEngine.GetCityStateCountry` (zip lookup via EDDY.IS.Validation NuGet)
- SQL stored procedures: `Prod.EDDY_FE_Lead_Update`, `dbo.EDDY_FE_Lead_CreativeInsert`

## Consumers

- FormsEngine (primary): `LeadSaveManger`, `LeadRepository`, `TemplateManagerControllerBase`
- FormsEngine WCF: `FormsEngineAPI.svc.cs` (Apollo/APILead)

## Potential Improvements

1. Add interface (`ILeadEngine`) for testability
2. Replace static data service instances with injected dependencies
3. Migrate to async/await throughout
4. Extract status assignment logic into strategy pattern
