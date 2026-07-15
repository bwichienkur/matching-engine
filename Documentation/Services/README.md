# Services Documentation

## FormsEngine Core Services

All registered as **Scoped** (per HTTP request) in SimpleInjector. See `Global.asax.cs` for full registration list.

| Service | Interface | Responsibilities | Dependencies | Consumers |
|---------|-----------|------------------|--------------|-----------|
| `SubmissionService` | `ISubmissionService` | School Picker submission pipeline | MatchValidation, FormValidation, Prospect, LeadSubmission, Session services | `SubmissionController` |
| `LeadSubmissionService` | `ILeadSubmissionService` | Lead save orchestration | `ILeadRepository` | `SubmissionService` |
| `FailedMatchReplacementService` | `IFailedMatchReplacementService` | Find replacement schools for failed validation | Program repository, configuration | `SchoolPickerWizardController` |
| `MatchValidationService` | `IMatchValidationService` | Validate program matches against rules | `IProgramValidationRepository` | `SubmissionService` |
| `FormValidationService` | `IFormValidationService` | Delegate form validation | `IFormValidationRepository` | `SubmissionService` |
| `ProspectService` | `IProspectService` | Prospect save (sync/async) | `IProspectRepository` | `SubmissionService` |
| `SessionService` | `ISessionService` | FESession read/write abstraction | `ISessionRepository` | `SubmissionService`, controllers |
| `ThankYouPageService` | `IThankYouPageService` | Thank-you page data assembly | HtmlRenderingStrategy, MetaData repos | `ThankYouController` |
| `ProgramService` | `IProgramService` | Program retrieval via ME | `IProgramRepository` | Various |
| `InstitutionService` | `IInstitutionService` | Institution details | `IInstitutionRepository` | `InstitutionController` |
| `ComponentCompilingService` | `IComponentCompilingService` | Pre-compile Razor components at startup | FileReader, ComponentTemplate, TemplatingEngine | `Global.asax.cs` warmup |
| `ComponentRenderingService` | `IComponentRenderingService` | Render compiled components | ComponentCompiling | `ComponentsController` |
| `TemplatingEngineService` | `ITemplatingEngineService` | RazorEngine wrapper | — | Component services |
| `ConfigurationService` | `IConfigurationService` | Read appSettings / componentTemplates | — | Multiple services |
| `MetaDataService` | `IMetaDataService` | Resource metadata text | `IMetaDataRepository` | ThankYou, DataBind |
| `LocationValidationService` | `ILocationValidationService` | Postal code / geo validation | `ILocationValidationRepository` | Location flows |
| `CCPAMessageService` | `ICCPAMessageService` | CCPA compliance message | — | `CCPAController` |
| `FlagService` | `IFlagService` | Feature flag lookups | — | Various |
| `IPAddressService` | `IPAddressService` | Client IP resolution | — | Submission flows |
| `LogoUrlFormattingService` | `ILogoUrlFormattingService` | Institution logo URL formatting | — | Institution display |
| `HtmlRenderingStrategyService` | `IHtmlRenderingStrategyService` | HTML rendering strategy | `IHtmlRenderingStrategyRepository` | ThankYou |
| `UserSelectionService` | `IUserSelectionService` | User school selections | `IUserSelectionRepository` | School Picker |
| `ProgramValidationService` | `IProgramValidationService` | Per-program validation | `IProgramValidationRepository` | Submission |
| `FileReaderService` | `IFileReaderService` | Read template files from disk | — | Component services |
| `ComponentTemplateService` | `IComponentTemplateService` | Resolve component template paths | `IConfigurationService` | Component services |
| `ShortFormSubmissionService` | `IShortFormSubmissionService` | Short-form submission flow | — | Short form endpoints |

### Legacy Services (FormsEngine.RF — not DI registered)

| Service | Responsibilities | External APIs |
|---------|------------------|---------------|
| `FormsEngine` (facade) | All legacy business logic | Matching, Prospect, Validation, LeadScoring, Pixels, Five9, EMS WCF/HTTP |
| `LeadSaveManger` | Lead creation pipeline | LeadEngine, SubmissionDataService |
| `TemplateDataService` | Template CRUD | Nexus EF |
| `SubmissionDataService` | Submission persistence | Nexus EF |
| `CampaignDataService` | Campaign data | Nexus EF |
| `RenderingStrategyDataService` | Rendering strategies | Nexus EF |
| `ResourceMetaDataService` | Resource metadata | Nexus EF |
| `OpenMailDataService` | OpenMail profiles | Nexus EF |
| `LandingPageDataService` | Landing page settings | Nexus EF view |
| `EMSTCPAMessageDataService` | EMS TCPA messages | Nexus EF view |
| `FormsEngineCache` | Local + Redis caching | Redis, HttpRuntime.Cache |

---

## MatchingEngine Services

No DI container. Services are instantiated via `new` or static singletons.

| Class | Responsibilities | Caching | Logging |
|-------|------------------|---------|---------|
| `MatchingEngine` | Match orchestration | Uses `StaticCacheProxyHost` | `PerformanceLog` |
| `RulesEngine` | Rule pipeline | Rule-level memory cache | Per-rule timing |
| `SchoolRankingEngine` | SRA scoring | Business model cache | Factor scores logged |
| `MatchAggregator` | DTO mapping | — | — |
| `MatchPersister` | Async match logging | — | EddyTracking DB |
| `MatchingEngineCache` | 60+ cache items | `LocalCacheBase` + `HttpRuntime.Cache` | Cache load timing |
| `MatchDatabase` | Inverted index filtering | In-memory indexes | — |
| `CrossSellProcessor` | Cross-sell mappings | Cache | — |
| `LeadScoringProcessor` | Lead scoring config | Cache | — |
| `ThirdPartyMatchProcessor` | External match integration | Cache | — |
| `ExternalMatchServiceClient` | HTTP to external match | — | Error logging |
| `GeoCodeProcessor` | Zip distance calculation | Cache | — |
| `Campaign` | Campaign lookup, cap checks | Cache | — |

### Retry Policies

**None formalized.** External HTTP calls in `ExternalMatchServiceClient` have no Polly/retry wrapper. WCF calls rely on default binding retry behavior.

---

## VendorWebAPI Business Services

Manual instantiation (`new`) in controllers and filters.

| Class | Responsibilities | DAO Dependencies | External APIs |
|-------|------------------|------------------|---------------|
| `VendorCampaigns` | API key validation | `VendorCampaignsDAO` | — |
| `Programs` | Program directory, details, form, matches | `MatchingServiceDAO`, `FormsServiceDAO`, `ProgramDAO` | Matching WCF, Forms WCF |
| `Institutions` | Institution directory | `InstitutionDAO`, `MatchingServiceDAO` | Matching WCF |
| `Campuses` | Campus directory, microsites | `CampusDAO`, `MatchingServiceDAO` | Matching WCF |
| `Categories` | Category/subject directory | `CategoryDAO`, `SubjectDAO` | Matching WCF |
| `Leads` | Lead save, call center, EduMax, details | `FormsServiceDAO`, `ProspectServiceDAO`, `DataExchangeServiceDAO` | Forms WCF, Prospect WCF, EMS HTTP |
| `Prospects` | Prospect save/validate | `ProspectServiceDAO` | Prospect WCF |
| `GpFive9` | Dialer routing | `GpFive9ServiceDAO` | GpFive9 WCF |
| `Logs` | API logging | `LogsDAO` | EddyLogging DB |
| `VendorResponseMessages` | Localized messages | `VendorResponseMessagesDAO` | — |
| `Timezones` | Timezone lookup | `TimezoneServiceDAO` | Nexus zip/area code data |
| `Zips` | Zip by campus | `ZipDAO` | Nexus |
| `Locations` | States, countries, postal codes | `LocationsDAO` | Nexus |
| `MilitaryStatuses` | Military status reference | `MilitaryStatusDAO` | Nexus |

### Caching (VendorWebAPI)

| Cache | TTL | Mechanism |
|-------|-----|-----------|
| Reference data (vendors, categories, states, programs) | 24 hours | `System.Runtime.Caching.MemoryCache` via `CacheStore` |
| Match results (microsites) | 15 minutes (configurable) | Redis via `RedisHelper` |
| Response data | 15 minutes | `MemoryCache` |

---

## LeadEngine Services

| Class | Responsibilities | Consumers |
|-------|------------------|-----------|
| `LeadEngine` | Facade: DTO mapping, status assignment, orchestration | FormsEngine `LeadSaveManger` |
| `LeadDataService` | Production lead CRUD + SP calls | `LeadEngine` |
| `BetaLeadDataService` | Beta lead inserts | `LeadEngine` |
| `RawPostDataService` | Raw post data inserts | `LeadEngine` |
| `Converter` | Entity ↔ DTO mapping | All data services |

**No retry, no caching, no async.** All synchronous EF operations.

---

## Cross-Cutting Logging Services

| Component | Package | Usage |
|-----------|---------|-------|
| `ISException` | EDDY.IS.Core.Logging | Exception capture + DB save |
| `PerformanceLog` | EDDY.IS.Core.Logging | Request timing detail |
| Enterprise Library Logging | EDDY.IS.Common.Logging | VendorWebAPI structured logging |
| `LogsDAO` / `Logs` | VendorWebAPI | API request/response persistence |

**No correlation IDs** found in any subsystem. **Confidence:** High.
