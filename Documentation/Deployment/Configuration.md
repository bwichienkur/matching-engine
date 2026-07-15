# Configuration Reference

## FormsEngine (`Web.config`)

### Connection Strings

| Name | Database | Purpose |
|------|----------|---------|
| `FEEntitiesContainer` | Nexus | Templates, submissions, campaigns |
| `Nexus_Validation_Entities` | Nexus | Validation rules |
| `Nexus_Lead_Entities` | Nexus | Lead persistence |
| `EddyLoggingEntities` | EddyLogging | Exception logging |

### Key appSettings

| Key | Purpose |
|-----|---------|
| `RedisServer`, `RedisCachePrefix` | Redis session cache |
| `FormsEngineAjaxDomain` | Allowed AJAX origin domain |
| `WebLeadsServerSideValidationEnabled` | Toggle server-side validation |
| `EmailVerificationLevel` | Email validation strictness |
| `EmsLeadEngineCreateFromISUrl` | EMS lead delivery URL |
| `EmsLeadEngineAuthToken` | EMS auth token (SECRET) |
| `GooglePlacesAPIKey` | Geo autocomplete (SECRET) |
| `CreativePortalUrl` | Creative asset URLs |
| `IsSpamAllowedForDelivery` | Allow spam leads through delivery |

### Config Sections

| Section | Purpose |
|---------|---------|
| `featureFlags` | Feature toggle extension point |
| `componentTemplates` | Razor component template paths |
| `system.serviceModel` | WCF client endpoints and bindings |

---

## MatchingEngine (`Web.config`)

### Connection Strings

| Name | Database |
|------|----------|
| `Nexus` | Nexus (raw ADO.NET) |
| `MatchingEngineModelContainer` | Nexus (EF6 views) |
| `MatchLoggingModelContainer` | EddyTracking |
| `EddyLoggingEntities` | EddyLogging |

### Key appSettings

| Key | Purpose |
|-----|---------|
| `ME_*_ExpirationMinutes` | Per-cache-item TTL (40+ keys) |
| `SM_MaxDistanceInMiles` | Smart match max distance |
| `IsBeta` | Suppress match response logging |
| `LeadScoringPremierTiers` | Premier tier configuration |
| `ExternalMatchProviderServiceURL` | External match HTTP endpoint |
| `SpamCheckServiceURL` | Spam check HTTP endpoint |
| `PerformanceLoggingEnabled` | Performance timing toggle |
| `MatchResponseLoggingEnabled` | Match logging toggle |

### Environment Transforms

`Master.config` — token replacement for DB server, LeadPing URL per environment.

---

## VendorWebAPI (`Web.config`)

### Connection Strings

| Name | Database |
|------|----------|
| `APINexusEntities` / `Nexus` | Nexus |
| `EddyLoggingEntities` / `EddyLogging` | EddyLogging |
| `ContactCenter` | ContactCenter |
| `NexusEntities` | External Match EF metadata |

### Key appSettings

| Key | Purpose |
|-----|---------|
| `SupportingDataCacheHourDurationValue` | Reference cache TTL (24h) |
| `ResponseDataCacheMinuteDurationValue` | Response cache TTL (15min) |
| `MatchingEngineMaxRecords` | Match result cap (1000) |
| `RedisPrefix`, `RedisConnection`, `UseRedisCaching` | Redis config |
| `EmsLeadEngineAuthToken` | EMS auth (SECRET) |
| `EmsLeadEngineBaseUrl` | EMS API base URL |
| `RequireContactSource` | Validation flag |
| `DefaultLeadSource*` | Default lead source URLs |
| `EduMaxLenexaDataProductId` | EduMax product IDs |
| `LogoBaseURL` | Institution logo CDN |

### Build Configurations

10 configurations with potentially different transforms: `Debug`, `Release`, `DEMO`, `DEVSTAGE`, `MIKETESTENV`, `PRODUCTIONSUPORT`, `QA`, `QASTAGE`, `QASTAGE02`, `UAT`.

---

## LeadEngine (`App.config`)

| Setting | Value |
|---------|-------|
| `Nexus_Lead_Entities` | SQL Server connection to Nexus |

Runtime settings read from host: `IsSpamAllowedForDelivery`, `CreativePortalUrl`.

---

## Secrets Management

**Current state:** All secrets in plaintext `Web.config` / `appSettings`.  
**Recommended:** Azure Key Vault with `{Environment}.config` overrides.

## Feature Flags

| Location | Status |
|----------|--------|
| FormsEngine `featureFlags` config section | Empty (extensible) |
| FormsEngine `FlagService` / `IFlagService` | Implemented in Core |
| MatchingEngine `FeatureProcessor` | DB-driven feature cache |
