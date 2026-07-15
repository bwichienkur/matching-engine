# MatchingEngine — EDDY.IS.MatchingEngine

**Path:** `MatchingEngine/EDDY.IS.MatchingEngine/`  
**Type:** Class Library (.NET Framework 4.8)

## Purpose

Core matching engine: rules evaluation, school ranking algorithm (SRA), in-memory caching, and data access for campaign/program inventory.

## Responsibilities

- `MatchingEngine` — orchestrates wizard, directory, cross-sell, form, neo, apollo matches
- `RulesEngine` + 25+ standard rules — eligibility filtering
- `SchoolRankingEngine` — business model scoring, eRPL, strategic weights
- `MatchDatabase` — inverted index for program-product filtering
- `MatchingEngineCache` — 60+ cache items with TTL configuration
- `MatchAggregator` — maps internal results to response DTOs
- `MatchPersister` — async logging to EddyTracking
- 72 `VW_Matching_*` EF view entities
- 83 DTO request/response types

## Dependencies

- EDDY.IS.MatchingEngine.Logging
- EDDY.IS.Base 2.0.4, EDDY.IS.Core 2.0.4, EDDY.IS.Core.Logging 2.0.4
- EDDY.IS.ExternalMatch.Base 1.0.0-ci
- EDDY.IS.LocalCache 2.0.0
- EntityFramework 6.5.1

## Important Classes

| Class | Role |
|-------|------|
| `MatchingEngine` | Main orchestrator |
| `RulesEngine` | Rule pipeline execution |
| `RulesEngineFactory` | Rule discovery and bucketing by entity level |
| `SchoolRankingEngine` | SRA scoring and sorting |
| `MatchingEngineCache` | Cache population (extends `LocalCacheBase`) |
| `MatchDatabase` | In-memory inverted indexes |
| `RuleCacheProcessor` | Caps, geo, schedules, warm transfer rules |
| `CrossSellProcessor` | Cross-sell mapping cache |
| `LeadScoringProcessor` | Lead scoring products and tiers |
| `ThirdPartyMatchProcessor` | External API smart-match |
| `ExternalMatchServiceClient` | HTTP client to external match provider |
| `StaticCacheProxyHost` | Lazy singleton cache proxy |

## Data Access

- `DataHelper` — `SqlDataReader` from Nexus views and stored procedures
- `MatchingEngineModelContainer` — EF6 DbContext for 60+ views
- 11 internal `*DataService` classes

## Configuration

- `App.Config` — `MatchingEngineModelContainer` connection
- Cache TTLs: `ME_*_ExpirationMinutes` (40+ keys in host Web.config)
- Behavior: `SM_MaxDistanceInMiles`, `IsBeta`, `LeadScoringPremierTiers`

## External Services

- LeadPing WCF (rule: `LeadPing`)
- External Match HTTP API
- Spam Check HTTP API

## Potential Improvements

1. Introduce DI container for testability
2. Split god-class `MatchingEngine.cs` by match type
3. Add structured logging with correlation IDs
4. Evaluate Redis/distributed cache (MongoDB project was abandoned)
