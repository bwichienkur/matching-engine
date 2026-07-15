# AI_CONTEXT.md

> Machine-readable context for AI agents maintaining, extending, or modernizing the EDDY.IS platform.  
> Generated: 2026-07-15 from repository reverse engineering.

---

## What This System Is

**EDDY.IS** is an education lead-generation platform by Education Dynamics. It matches prospective students to school/program inventory under campaign rules, captures leads via web forms and partner APIs, and delivers them to institutions.

**Four subsystems in one repo:**
1. **FormsEngine** — embedded web forms (IIS, ASP.NET MVC 5, JSONP)
2. **MatchingEngine** — program/school matching (IIS, WCF JSON service)
3. **LeadEngine** — lead persistence library (SQL Server)
4. **VendorWebAPI** — partner REST API (IIS, ASP.NET Web API 2)

---

## Architecture Summary

- **Style:** Hybrid layered monolith, .NET Framework 4.5.1–4.8, IIS-hosted
- **NOT:** ASP.NET Core, microservices, event-driven, CQRS
- **Communication:** WCF between apps; HTTP for EMS/External Match
- **Data:** SQL Server (Nexus, EddyTracking, EddyLogging); Redis for caching
- **DI:** SimpleInjector in FormsEngine only (partial — 30 services, 2 controllers)
- **Auth:** API key (VendorWebAPI only); FormsEngine and MatchingEngine are unauthenticated

---

## Solution Map

```
FormsEngine/EDDY.IS.FormsEngine.sln
  ├── EDDY.IS.FormsEngine.Services     (web host)
  ├── EDDY.IS.FormsEngine.RF           (legacy core — WHERE MOST LOGIC LIVES)
  ├── EDDY.IS.FormsEngine.Core         (newer services/interfaces)
  ├── EDDY.IS.FormsEngine.Infastructure (repos — typo in name)
  ├── EDDY.IS.FormsEngine.Tests
  └── EDDY.IS.LeadEngine               (../LeadEngine/)

MatchingEngine/EDDY.IS.MatchingEngine.sln
  ├── EDDY.IS.MatchingEngine.Service   (WCF host)
  ├── EDDY.IS.MatchingEngine           (engine + rules + cache)
  ├── EDDY.IS.MatchingEngine.Logging
  ├── EDDY.IS.SEOAllocation.Console
  ├── EDDY.IS.MatchResponseReplay.Console
  └── EDDY.IS.MatchingEngine.UnitTests

VendorWebAPI/EDDY.IS.Vendor.Web.API.sln
  ├── EDDY.IS.Vendor.Web.API
  ├── EDDY.IS.Vendor.Business
  ├── EDDY.IS.Vendor.DataAccess
  ├── EDDY.IS.Vendor.Entities
  ├── EDDY.IS.Vendor.Utilities
  └── EDDY.IS.Vendor.Web.API.Tests

LeadEngine/EDDY.IS.LeadEngine-RF/      (no .sln — shared library)
```

---

## Critical Business Rules

### Lead Creation Types (stored in Lead.LeadCreationTypeId)
| ID | Name | Flow |
|----|------|------|
| 1 | InstitutionFormInitial | Standard program form |
| 2 | InstitutionFormCrossSell | Cross-sell after initial |
| 3 | WizardSmartMatch | Wizard auto-match |
| 4 | WizardUserSelection | Wizard user picks schools |
| 5 | HostAndPost | API direct post |
| 6 | Apollo | Apollo integration |
| 12 | SchoolPickerUserSelection | New School Picker flow |

### Lead Delivery Status (LeadEngine.RealtimeDeliveryStatusValue)
- `100` NEW, `101` REALTIME, `110` DELIVERED
- `430` EDDY_FORM_VALIDATION_FAILED
- `470` EDDY_FORM_TEST_LEAD
- `1003` SPAM
- Product 52 → WARM_TRANSFER_TITANIUM_HOLD

### Test Lead Detection
`LastName == "test"` (case-insensitive) OR email contains `@test.com`

### Matching Flow
1. Resolve campaign by TrackGuid
2. Filter program-products via inverted index (MatchDatabase)
3. Apply rules (geo, caps, age, education, military, lead scoring)
4. Rank via School Ranking Algorithm (eRPL + strategic weights)
5. Return paginated response; async log to EddyTracking

### VendorWebAPI Auth
`apikey` (GUID) in query string or JSON body → `VendorCampaign` lookup → status/rate-limit checks

---

## Naming Conventions

| Pattern | Example | Meaning |
|---------|---------|---------|
| `EDDY.IS.{Domain}` | EDDY.IS.FormsEngine | Namespace/assembly prefix |
| `*Controller` | TemplateManagerController | MVC controller |
| `*ControllerBase` | TemplateManagerControllerBase | Shared controller logic |
| `*DataService` | TemplateDataService | Legacy EF data access |
| `*Repository` | LeadRepository | Core/Infrastructure data access |
| `*DAO` | MatchingServiceDAO | VendorWebAPI data access |
| `*DTO` | LeadDTO | Data transfer object |
| `FormsRelatedServices.*` | FormsRelatedServices.MatchingEngine.cs | WCF client partial classes |
| `VW_Matching_*` | VW_Matching_Campaign | SQL view entity in MatchingEngine |
| `FE*` | FEEntitiesContainer | FormsEngine prefix |
| `RF` suffix | FormsEngine.RF | Legacy/refactored project folder |

---

## Coding Standards (Observed)

- .NET Framework 4.8 target (varies 4.5.1–4.8)
- Entity Framework 6 Database First (EDMX) — never Code First
- `OnModelCreating` throws `UnintentionalCodeFirstException`
- WCF service references for inter-app communication
- `ISException(app, ex).Save()` for error logging
- `PerformanceLog` for timing detail
- Case-insensitive dictionary keys for form data (`first_name`, `email`, etc.)
- JSONP responses in FormsEngine (`JsonpResult`)
- Manual `new` instantiation (except FormsEngine DI path)

---

## Key Workflows

### Form Submission (Legacy — 90% of traffic)
```
Browser → GET /TemplateManager/ProcessSubmit
  → TemplateManagerControllerBase.ProcessSubmit
  → FormsEngine.ValidateForm + MatchingEngine.ValidateProgram
  → ProspectService.SaveProspect
  → LeadSaveManger.Execute → LeadEngine.CreateLead
  → SubmissionDataService.Save + LeadEngine.UpdateLeads
  → JSONP response
```

### Form Submission (School Picker — DI path)
```
Browser → GET /Submission/SubmitSchoolPickerWizard
  → SubmissionService.SubmitSchoolPickerWizard
  → MatchValidation → FormValidation → Prospect → LeadSubmission
  → SessionService update
```

### Partner Lead API
```
Partner → POST /api/Lead/save (apikey)
  → CampaignAuthorizationFilter
  → PostLeadActionFilter + TCPAFilter
  → Leads.SaveLead → FormsServiceDAO → FormsEngine WCF
  → LeadEngine.CreateLead
```

### Directory Browse
```
Partner → POST /api/Directory/programs (apikey)
  → MatchingServiceDAO → MatchingEngine WCF GetPrograms
  → Cached rules engine response
```

---

## Database Relationships

| DbContext | Connection | Key Tables/Views |
|-----------|------------|------------------|
| FEEntitiesContainer | Nexus | Submission, Template, TemplateControl, Campaign |
| Nexus_Lead_Entities | Nexus | Lead, BetaLead, RawPostData |
| MatchingEngineModelContainer | Nexus | 72 VW_Matching_* views |
| MatchLoggingModelContainer | EddyTracking | MatchResponse, MatchResponseDisplayed |
| APINexusEntities | Nexus | Campaign, Vendor, VendorCampaign, Program |
| EddyLoggingEntities | EddyLogging | APILog, EddyApiLog |

**Cross-system link:** `Lead.MatchResponseGuid` ties leads to MatchingEngine results (no compile-time dependency).

**Stored procedures:** `Prod.EDDY_FE_Lead_Update`, `dbo.EDDY_FE_Lead_CreativeInsert`

---

## External Systems

| System | How Called | Config |
|--------|-----------|--------|
| MatchingEngine | WCF | system.serviceModel/client |
| ProspectService | WCF | system.serviceModel/client |
| ValidationEngine | WCF + NuGet | system.serviceModel/client |
| EMS Lead Engine | HTTP + token | EmsLeadEngineAuthToken |
| LeadPing | WCF | LeadPingService endpoint |
| External Match | HTTP | ExternalMatchProviderServiceURL |
| Redis | StackExchange.Redis | RedisServer/RedisConnection |
| Targus | SOAP | applicationSettings |
| Five9/GpFive9 | WCF | system.serviceModel/client |

---

## Entry Points

| App | Start | File |
|-----|-------|------|
| FormsEngine | IIS → Global.asax | `FormsEngine/EDDY.IS.FormsEngine.Services/Global.asax.cs` |
| MatchingEngine | IIS → MatchingService.svc | `MatchingEngine/EDDY.IS.MatchingEngine.Service/MatchingService.svc.cs` |
| VendorWebAPI | IIS → Global.asax | `VendorWebAPI/EDDY.IS.Vendor.Web.API/Global.asax.cs` |
| LeadEngine | (library) | `LeadEngine/EDDY.IS.LeadEngine-RF/LeadEngine.cs` |

---

## DI Registration (FormsEngine only)

File: `FormsEngine/EDDY.IS.FormsEngine.Services/Global.asax.cs` → `RegisterDIContainer()`  
Container: SimpleInjector 4.6, WebRequestLifestyle (scoped)  
30 service/repo pairs registered. Only `StaticController` and `DataBindController` injected.

---

## Configuration Keys (Most Important)

| Key | App | Purpose |
|-----|-----|---------|
| FEEntitiesContainer | FormsEngine | Main DB |
| Nexus_Lead_Entities | LeadEngine | Lead DB |
| Nexus | MatchingEngine | Raw SQL |
| EmsLeadEngineAuthToken | FormsEngine, VendorWebAPI | EMS auth (SECRET) |
| WebLeadsServerSideValidationEnabled | FormsEngine | SSV toggle |
| IsBeta | MatchingEngine | Suppress logging |
| RedisConnection | VendorWebAPI | Redis |
| ME_*_ExpirationMinutes | MatchingEngine | Cache TTLs |

---

## Patterns Used

Facade, Repository (informal), DAO, Strategy (rules), Factory (RulesEngineFactory), Chain of Responsibility (SEO), Singleton (caches), Adapter (infra repos), DTO, Service Locator (partial DI)

---

## Known Technical Debt

1. **No auth** on FormsEngine/MatchingEngine
2. **GET mutations** in FormsEngine (CSRF risk)
3. **God classes:** TemplateManagerControllerBase, FormsEngine partial
4. **WCF everywhere** — modernization blocker
5. **Fire-and-forget Task.Run** — silent failures
6. **Secrets in Web.config** plaintext
7. **Tests disabled** in MatchingEngine CI
8. **Orphan projects** not in solutions
9. **No correlation IDs** in logging
10. **Dual architecture** in FormsEngine (legacy + DI)

---

## Files to Read First

| Priority | File | Why |
|----------|------|-----|
| 1 | `FormsEngine/.../TemplateManagerControllerBase.cs` | Main submission logic |
| 2 | `FormsEngine/.../LeadSaveManager.cs` | Lead save pipeline |
| 3 | `LeadEngine/.../LeadEngine.cs` | Lead creation + status |
| 4 | `MatchingEngine/.../MatchingEngine.cs` | Match orchestration |
| 5 | `MatchingEngine/.../RulesEngine.cs` | Rule pipeline |
| 6 | `VendorWebAPI/.../CampaignAuthorizationFilter.cs` | API auth |
| 7 | `FormsEngine/.../Global.asax.cs` | DI + startup |
| 8 | `FormsEngine/.../SubmissionService.cs` | New DI submission path |
| 9 | `MatchingEngine/.../IMatchingService.cs` | WCF contract |
| 10 | `VendorWebAPI/.../Leads.cs` | Partner lead business logic |

---

## Deployment

- **IIS** on Windows Server, `F:\inetpub\wwwroot\`
- **CI/CD:** Azure DevOps, `azure-pipelines.yaml` (deploy) + `ci-pipelines.yaml` (PR build)
- **Branches:** development → qa → uat → main
- **No Docker/K8s**

---

## Test Commands

```bash
# FormsEngine tests (xUnit)
dotnet test FormsEngine/EDDY.IS.FormsEngine.Tests/

# MatchingEngine tests (xUnit + MSTest)
dotnet test MatchingEngine/EDDY.IS.MatchingEngine.UnitTests/

# VendorWebAPI tests (MSTest)
dotnet test VendorWebAPI/EDDY.IS.Vendor.Web.API.Tests/
```

Note: Projects use .NET Framework — may require `msbuild` on Windows rather than `dotnet test`.

---

## Documentation Index

| Document | Path |
|----------|------|
| Executive Summary | Documentation/ExecutiveSummary.md |
| Architecture | Documentation/Architecture.md |
| Business Processes | Documentation/BusinessProcesses.md |
| APIs | Documentation/APIs/ |
| Database | Documentation/Database/ |
| Entities | Documentation/Entities/ |
| Services | Documentation/Services/ |
| Projects | Documentation/Projects/ |
| Security | Documentation/Security/ |
| Performance | Documentation/Performance/ |
| Deployment | Documentation/Deployment/ |
| Diagrams | Documentation/Diagrams/ |
| Refactoring | Documentation/Refactoring/ |
