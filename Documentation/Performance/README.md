# Performance Review

## Executive Summary

MatchingEngine is optimized for **read-heavy, sub-second match responses** via aggressive in-memory caching. FormsEngine and VendorWebAPI have **moderate performance concerns** around fire-and-forget async, N+1 patterns in legacy code, and large controller methods.

---

## MatchingEngine (Strengths)

### In-Memory Cache Architecture

- 60+ cache items preloaded at startup via `MatchingEngineCache.PreloadEntireCache()`
- `Parallel.ForEach` for parallel cache item loading
- Configurable TTL per cache item (1–1500 minutes)
- `MatchDatabase` inverted indexes avoid full-table scans

**Hot path:** `GetInstitutions` → cache lookup → `MatchDatabase.FilterProgramProducts` → `RulesEngine` → `SchoolRankingEngine` → response. No DB round-trip on hot path after cache warmup.

### Response Caching

- 15-minute `HttpRuntime.Cache` for navigation/category/subject endpoints
- ASP.NET cache for facet endpoints in `MatchingService.svc.cs`

### Async Logging

- `Task.Run` / `ThreadPool.QueueUserWorkItem` for match response persistence
- Logging does not block response delivery

---

## MatchingEngine (Concerns)

| Issue | Location | Impact |
|-------|----------|--------|
| Memory footprint | 60+ full cache loads | Large worker process (likely GB-scale); app pool recycle clears cache |
| `Parallel.ForEach` in SEO allocation | `AllocationProcessManager` | Max 2 threads — conservative but slow for large sitemaps |
| No cache stampede protection | `MatchingEngineCache` | Concurrent requests during cache refresh may hit DB |
| Synchronous WCF | All operations | Thread pool exhaustion under high concurrency |
| `EntityFrameworkCommandTimeout=500` | Web.config | Very high timeout masks slow queries |

---

## FormsEngine (Concerns)

| Issue | Location | Impact |
|-------|----------|--------|
| God class controller | `TemplateManagerControllerBase` (~1800 lines) | Hard to optimize; likely contains sequential WCF calls |
| Fire-and-forget `Task.Run` | `LeadSaveManger`, `Global.asax.cs` | Lead save failures invisible to caller; thread pool pressure |
| JSONP GET requests | All controllers | No HTTP caching benefits; large query strings |
| Dual cache (ASP.NET + Redis) | `FESession`, `SessionController` | Cache coherence complexity |
| Startup warmup | `Global.asax.cs` | Two `Task.Run` on startup; may delay first requests |
| RazorEngine compile at startup | `ComponentCompilingService` | CPU spike on app pool recycle |
| No async/await | Most of RF layer | Thread blocking during WCF calls |

### Potential N+1 Queries

- `TemplateDataService` may load template controls individually (not verified in hot path)
- `SubmissionDataService` saves submission details in loops

**Confidence:** Medium — requires runtime profiling to confirm.

---

## VendorWebAPI (Concerns)

| Issue | Location | Impact |
|-------|----------|--------|
| Response model constructors call Business layer | `Models/*ResponseModel.cs` | Business logic in model construction; hard to cache |
| `new` instantiation everywhere | Controllers, filters | No object pooling; GC pressure |
| Redis + MemoryCache dual caching | `RedisHelper`, `CacheStore` | Cache invalidation complexity |
| WCF calls per request | `MatchingServiceDAO`, `FormsServiceDAO` | Network latency per API call |
| Large filter chain | 25+ action filters per request | Filter execution overhead |

### Caching Opportunities

- Directory endpoints already cached in MatchingEngine (15 min)
- Microsite endpoints cached in Redis
- Reference data cached 24 hours in MemoryCache
- **Opportunity:** Cache `GetProgramDetails` and `GetProgramFormTemplate` responses

---

## LeadEngine (Concerns)

| Issue | Location | Impact |
|-------|----------|--------|
| Synchronous EF inserts | `LeadDataService.SaveLead` | Blocks calling thread |
| Static data service instances | `LeadEngine.cs` | No connection pooling optimization |
| XML serialization for AdditionalFields | `CreateLeadDTO` | Allocation overhead for large dictionaries |
| No batch insert | `UpdateLeads` loops per lead | N round-trips for multi-lead submissions |

---

## Database Performance

| Pattern | Assessment |
|---------|------------|
| MatchingEngine reads views | Views are pre-computed cache tables — fast reads |
| LeadEngine writes | Direct EF insert — acceptable for single leads |
| Match logging bulk insert | `Z.EntityFramework.Extensions.BulkInsert` — optimized |
| EF6 Database First | No query compilation caching issues; but no query splitting |

### Index Recommendations (Inferred)

- `Lead.TrackId`, `Lead.MatchResponseGuid` — likely indexed (high-cardinality lookups)
- `Lead.SubmissionId` — FK index for submission linkage
- `VendorCampaign.TrackGuid` — API key lookup on every VendorWebAPI request

**Confidence:** Low for indexes — requires DBA schema review.

---

## Async Correctness

| Pattern | Correct? | Issue |
|---------|----------|-------|
| `Task.Run` for cache warmup | Acceptable | Exceptions caught and logged |
| `Task.Run` for lead save | **Problematic** | No error propagation to caller; no `ConfigureAwait` |
| `Task.Run` for match logging | Acceptable | Fire-and-forget by design |
| `ThreadPool.QueueUserWorkItem` | Legacy | Same issues as Task.Run |
| No `async/await` in hot paths | **Problematic** | Thread pool blocking under load |

---

## Parallelization Opportunities

1. **FormsEngine submission:** Validate + match + prospect save could be parallelized where independent
2. **VendorWebAPI directory:** Batch WCF calls for multi-entity requests
3. **SEO allocation:** Increase `Parallel.ForEach` max threads beyond 2
4. **LeadEngine UpdateLeads:** Batch SP calls or use table-valued parameters

---

## Hot Paths (Ranked by Expected Traffic)

1. `MatchingEngine.GetInstitutions` / `GetPrograms` — directory browsing
2. `FormsEngine.TemplateManager.ProcessSubmit` — form submission
3. `VendorWebAPI.Lead/save` — partner lead API
4. `FormsEngine.FormValidation.*` — real-time field validation
5. `FormsEngine.Session.*` — session read/write per form interaction
6. `MatchingEngine.GetWizardMatches` — wizard smart match
7. `MatchingEngine.ValidateProgram` — pre-submission validation

---

## Monitoring Recommendations

1. Add Application Insights or OpenTelemetry (VendorWebAPI tests already reference OpenTelemetry packages)
2. Track WCF call latency per downstream service
3. Monitor cache hit rates for MatchingEngine cache items
4. Alert on app pool recycle frequency (cache cold-start impact)
5. Profile `TemplateManagerControllerBase.ProcessSubmit` end-to-end latency
