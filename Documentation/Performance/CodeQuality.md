# Logging, Exception Handling & Code Quality

## Logging

### FormsEngine & MatchingEngine & LeadEngine

| Component | Package | Usage |
|-----------|---------|-------|
| `ISException` | EDDY.IS.Core.Logging | `new ISException(ISApplication.FormsEngine, ex).Save()` |
| `PerformanceLog` | EDDY.IS.Core.Logging | Request-level timing with detail steps |

**Pattern:** Try/catch → `ISException.Save()` → return error to caller or swallow in `Task.Run`.

**No Serilog, NLog, or Application Insights** in production code (OpenTelemetry referenced only in VendorWebAPI.Tests).

### VendorWebAPI

| Component | Package | Usage |
|-----------|---------|-------|
| Enterprise Library Logging | EDDY.IS.Common.Logging | Structured logging |
| `LogsDAO` | VendorWebAPI | Writes `APILog`, `EddyApiLog` to EddyLogging DB |
| `Logs` business class | VendorWebAPI | Called from filters and controllers |

### Structured Logging

**Not implemented.** Log messages are exception-centric without correlation IDs, request IDs, or structured fields.

### Correlation IDs

**Not found** in any subsystem. Cross-service WCF calls have no tracing headers.

### Telemetry

VendorWebAPI.Tests references `Microsoft.ApplicationInsights` and `OpenTelemetry` packages but production Web.API project does not use them.

**Confidence:** High.

---

## Exception Handling

### Global Handlers

| Application | Handler | Behavior |
|-------------|---------|----------|
| FormsEngine MVC | `HandleErrorAttribute` (global filter) | Redirect to error view |
| VendorWebAPI | `EDDY.IS.Common.ExceptionHandler` | Enterprise Library exception handling |
| MatchingEngine WCF | WCF fault contracts | `includeExceptionDetailInFaults=true` |

### Middleware

No custom exception middleware (not ASP.NET Core). Exception handling is via MVC filters and try/catch blocks.

### Error Responses

| Application | Format |
|-------------|--------|
| FormsEngine | JSONP with error fields in response DTOs |
| MatchingEngine | WCF faults (JSON-wrapped) |
| VendorWebAPI | `VendorResponseBase` with `MessageCode` and localized `Message` |

### Retry Logic

**No formal retry policies** (no Polly, no WCF reliable sessions). Fire-and-forget `Task.Run` for non-critical operations.

### Compensation

No saga or compensation patterns. Failed lead saves after successful validation are not rolled back (prospect may exist without lead).

---

## Code Quality Assessment

### Code Smells

| Smell | Location | Severity |
|-------|----------|----------|
| God class | `TemplateManagerControllerBase` | High |
| God class | `FormsEngine` partial (7+ files) | High |
| Static singletons | `LeadSaveManger`, `LeadEngine` data services | Medium |
| Feature envy | Infrastructure repos delegate to legacy facade | Medium |
| Long parameter lists | `CreateLeadDTO`, `LeadCreateRequest` builders | Medium |
| Magic numbers | Delivery status codes, product IDs | Low |

### SOLID Violations

| Principle | Violation |
|-----------|-----------|
| SRP | Controllers contain validation, mapping, WCF calls, session management |
| OCP | New match types require modifying `MatchingEngine.cs` |
| LSP | N/A (minimal inheritance) |
| ISP | No interfaces in VendorWebAPI/MatchingEngine |
| DIP | Direct `new` instantiation everywhere except FormsEngine DI path |

### DRY Violations

- WCF client configuration duplicated across FormsEngine and VendorWebAPI
- Cache TTL configuration duplicated in SEO Allocation `App.config`
- Validation logic in both FormsEngine controllers and VendorWebAPI filters

### Dead Code

| Item | Evidence |
|------|----------|
| `EDDY.IS.FormsEngine.Business` | Not in solution; 2 classes only |
| `EDDY.IS.FormsEngine.Entities` | Not in solution; 2 DTOs only |
| `EDDY.IS.ProspectResubmit` | Empty `Main()` |
| `EDDY.IS.MatchingEngine.MongoDB` | Not referenced by any solution project |
| `FormsEngine.Services.Test` | No test classes |
| Owin packages in VendorWebAPI | `AutomaticAppStartup = false` |

### Circular Dependencies

No compile-time circular dependencies detected. Runtime circularity: FormsEngine → MatchingEngine → (no back-reference). VendorWebAPI → FormsEngine (WCF) + MatchingEngine (WCF).

### Naming Issues

- `Infastructure` (typo)
- `LeadSaveManger` (typo: Manager)
- `RF` suffix meaning unclear (possibly "Refactored" or legacy naming)
- `GpFive9` vs `Five9` — two different Five9 integrations

---

## Design Patterns

| Pattern | Implementation | Location |
|---------|---------------|----------|
| **Facade** | `FormsEngine`, `LeadEngine`, `MatchingService` | RF layer, LeadEngine, ME Service |
| **Repository** | `I*Repository` + implementations | FormsEngine Infrastructure |
| **DAO** | `*DAO` classes | VendorWebAPI DataAccess |
| **Strategy** | Rule classes implement `IMatchingRule` | MatchingEngine Rules |
| **Factory** | `RulesEngineFactory` | MatchingEngine |
| **Chain of Responsibility** | SEO allocation steps | SEOAllocation.Console |
| **Singleton** | `StaticCacheProxyHost`, static data services | MatchingEngine, LeadEngine |
| **Adapter** | Infrastructure repos wrap WCF clients | FormsEngine Infrastructure |
| **Template Method** | `Engine` base class with `PerformanceLog` | MatchingEngine |
| **DTO** | Extensive DTO layers | All projects |
| **Service Locator** | `DependencyResolver` (partial) | FormsEngine Services |
| **Unit of Work** | EF6 `DbContext` per `using` block | All EF consumers |

Patterns **not** used: Mediator, Observer, Specification (formal), Builder (formal), Decorator, Unit of Work (explicit).

---

## Testing

### Test Projects

| Project | Framework | Tests | In CI? |
|---------|-----------|-------|--------|
| EDDY.IS.FormsEngine.Tests | xUnit + Moq + FluentAssertions | 40+ classes | Unknown |
| EDDY.IS.FormsEngine.Services.Tests | MSTest | 4 classes | No |
| EDDY.IS.FormsEngine.CoreTests | xUnit | 1 class | No |
| EDDY.IS.MatchingEngine.UnitTests | xUnit + MSTest | 8 fixtures | **Commented out** |
| EDDY.IS.SEOAllocation.UnitTests | MSTest | 1 fixture | No |
| EDDY.IS.Vendor.Web.API.Tests | MSTest + Moq | 56 tests | Unknown |

### Coverage Gaps

- `TemplateManagerControllerBase` — no unit tests (integration only)
- `LeadSaveManger` — no dedicated tests
- `MatchingEngine.RulesEngine` — no unit tests for individual rules
- VendorWebAPI DAOs — minimal mocking
- WCF integration — tested via full-stack fixtures only

### Mocking Strategy

- FormsEngine Tests: Moq for repository interfaces
- VendorWebAPI Tests: Moq for DAOs in filter tests
- MatchingEngine Tests: Integration-style against real cache-backed engine

### Recommendations

1. Enable ME unit tests in CI immediately
2. Add unit tests for `LeadEngine.CreateLeadDTO` status assignment logic
3. Add rule-level unit tests for MatchingEngine standard rules
4. Add integration tests for VendorWebAPI with test API keys
5. Target 60%+ coverage on Core/Infrastructure services
