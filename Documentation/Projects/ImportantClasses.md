# Important Classes Reference

## FormsEngine

### TemplateManagerControllerBase
- **File:** `FormsEngine/EDDY.IS.FormsEngine.Services/Controllers/Base/TemplateManagerControllerBase.cs`
- **Purpose:** Central controller for all form submission, wizard, cross-sell, and prospect flows
- **Collaborators:** `FormsEngine`, `LeadSaveManger`, `LeadEngine`, WCF clients
- **Lifecycle:** Per HTTP request (but uses static `LeadEngineService`)
- **Thread safety:** Not thread-safe (static state)
- **Issues:** God class (~1800 lines); mixes validation, business logic, WCF calls, session management
- **Complexity:** Very High

### FormsEngine (partial class)
- **Files:** `FormsEngine.RF/FormsEngine.cs`, `FormsEngineAPI.cs`, `FormsEngineOpenMail.cs`
- **Purpose:** Legacy business facade for templates, validation, submission, matching
- **Collaborators:** All `*DataService` classes, `FormsRelatedServices`, `ValidationEngine`
- **Patterns:** Facade
- **Issues:** Partial class across 3+ files; static methods; untestable without integration tests

### LeadSaveManger
- **File:** `FormsEngine.RF/LeadSaveManager.cs`
- **Purpose:** Orchestrates lead creation, submission persistence, async save
- **Methods:** `Execute()`, `SaveSubmissionAndUpdateLeadsAsync()`, `SendLeadsToEmsLeadService()`
- **Dependencies:** `LeadEngine`, `SubmissionDataService`, `EntityBuildHelper`
- **Issues:** Typo in name; static instance; fire-and-forget async

### FESession
- **File:** `FormsEngine.RF/FESession.cs`
- **Purpose:** Session emulation using `HttpRuntime.Cache`
- **Thread safety:** ASP.NET Cache is thread-safe per key
- **Issues:** No distributed session without Redis extension in SessionController

### SubmissionService
- **File:** `FormsEngine.Core/Services/SubmissionService.cs`
- **Purpose:** School Picker submission pipeline (DI path)
- **Methods:** `SubmitSchoolPickerWizard()` — validate → prospect → lead → session
- **Complexity:** Medium (well-structured)

---

## MatchingEngine

### MatchingEngine
- **File:** `MatchingEngine/EDDY.IS.MatchingEngine/MatchingEngine.cs`
- **Purpose:** Main match orchestrator
- **Methods:** `GetWizardMatches()`, `GetInstitutions()`, `GetProgramsForCrossSell()`, `ValidateProgram()`
- **Dependencies:** `RulesEngine`, `SchoolRankingEngine`, `MatchDatabase`, `StaticCacheProxyHost`
- **Complexity:** Very High

### RulesEngine
- **File:** `MatchingEngine/EDDY.IS.MatchingEngine/RulesEngine.cs`
- **Purpose:** Apply eligibility rules at CR/campus/program/product levels
- **Patterns:** Strategy (per-rule classes), Factory (via RulesEngineFactory)
- **Thread safety:** Uses `System.Runtime.Caching` references

### MatchingEngineCache
- **File:** `MatchingEngine/EDDY.IS.MatchingEngine/MatchingEngineCache.cs`
- **Purpose:** 60+ cache items with population delegates
- **Extends:** `LocalCacheBase` (EDDY.IS.LocalCache NuGet)
- **Methods:** `PreloadEntireCache()` — parallel load all items
- **Memory:** Likely GB-scale when fully loaded

### SchoolRankingEngine
- **File:** `MatchingEngine/EDDY.IS.MatchingEngine/SchoolRankingEngine.cs`
- **Purpose:** School Ranking Algorithm — eRPL, strategic weights, business model scoring

---

## LeadEngine

### LeadEngine
- **File:** `LeadEngine/EDDY.IS.LeadEngine-RF/LeadEngine.cs`
- **Purpose:** Lead facade — DTO mapping, status assignment, persistence
- **Key methods:**
  - `CreateLeadDTO()` — field normalization + status logic
  - `CreateLead()` — full create pipeline
  - `UpdateLeads()` — post-submission linking
  - `IsTestLead()` — test detection
- **Dependencies:** `LeadDataService`, `BetaLeadDataService`, `RawPostDataService`, `ValidationEngine`
- **Thread safety:** Static data services — not thread-safe for concurrent writes (EF contexts per call)

### LeadDataService
- **File:** `LeadEngine/EDDY.IS.LeadEngine-RF/DataModel/LeadDataService.cs`
- **Methods:** `SaveLead()` (EF insert), `UpdateLead()` (EF + SP), `SaveLeadCreative()` (SP)

---

## VendorWebAPI

### CampaignAuthorizationFilter
- **File:** `VendorWebAPI/EDDY.IS.Vendor.Web.API/Filters/CampaignAuthorizationFilter.cs`
- **Purpose:** Global API key authentication
- **Methods:** `OnAuthorization()` — parse apikey, validate campaign, check rate limits
- **Returns:** 401 with `VendorResponseBase` JSON

### MatchingServiceDAO
- **File:** `VendorWebAPI/EDDY.IS.Vendor.DataAccess/MatchingServiceDAO.cs`
- **Purpose:** WCF client wrapper for all MatchingEngine calls
- **Caching:** Redis for match results and microsite data

### FormsServiceDAO
- **File:** `VendorWebAPI/EDDY.IS.Vendor.DataAccess/FormsServiceDAO.cs`
- **Purpose:** WCF client wrapper for FormsEngine + External Match

### DataExchangeServiceDAO
- **File:** `VendorWebAPI/EDDY.IS.Vendor.DataAccess/DataExchangeServiceDAO.cs`
- **Purpose:** EMS lead lookup/update via HTTP + Nexus EF

---

## Method Complexity Summary

| Class | Estimated LOC | Cyclomatic Complexity | Priority for Refactoring |
|-------|--------------|----------------------|--------------------------|
| TemplateManagerControllerBase | ~1800 | Very High | P1 |
| FormsEngine (all partials) | ~3000+ | Very High | P2 |
| MatchingEngine | ~2000+ | High | P2 |
| LeadEngine | ~490 | Medium | P3 |
| SubmissionService | ~200 | Low | OK |
| CampaignAuthorizationFilter | ~180 | Medium | OK |
