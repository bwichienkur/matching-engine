# Diagrams — Dependency Graph

## Solution-Level Dependencies

```mermaid
flowchart TB
    subgraph external [External NuGet Packages]
        BASE[EDDY.IS.Base]
        CORE[EDDY.IS.Core]
        LOGGING[EDDY.IS.Core.Logging]
        UTIL[EDDY.IS.Util]
        VALID[EDDY.IS.Validation]
        LOCALCACHE[EDDY.IS.LocalCache]
        EMS_ENT[EDDY.IS.EmsLeadEngine.Entities]
        EXT_MATCH[EDDY.IS.ExternalMatch]
        EF[EntityFramework 6.x]
        JSON[Newtonsoft.Json]
        REDIS[StackExchange.Redis]
        SIMPLE[SimpleInjector]
    end

    subgraph leadengine [LeadEngine]
        LE[EDDY.IS.LeadEngine]
    end

    subgraph formsengine [FormsEngine]
        FE_SVC[Services - Web Host]
        FE_RF[RF - Legacy Core]
        FE_CORE[Core]
        FE_INFRA[Infrastructure]
    end

    subgraph matchingengine [MatchingEngine]
        ME_SVC[Service - WCF Host]
        ME_CORE[MatchingEngine]
        ME_LOG[Logging]
    end

    subgraph vendorwebapi [VendorWebAPI]
        VA_WEB[Web.API]
        VA_BIZ[Business]
        VA_DAL[DataAccess]
        VA_ENT[Entities]
        VA_UTIL[Utilities]
    end

    FE_SVC --> FE_CORE & FE_INFRA & FE_RF & LE
    FE_INFRA --> FE_CORE & FE_RF & LE
    FE_CORE --> FE_RF & LE
    FE_RF --> LE & BASE & LOGGING & UTIL & VALID & EMS_ENT & EF & JSON

    ME_SVC --> ME_CORE --> ME_LOG & BASE & CORE & LOCALCACHE & EXT_MATCH & EF & JSON
    ME_LOG --> EF

    VA_WEB --> VA_BIZ --> VA_DAL --> VA_ENT
    VA_BIZ --> VA_UTIL --> VA_ENT
    VA_DAL --> EXT_MATCH & EMS_ENT & EF & REDIS & JSON

    LE --> BASE & LOGGING & UTIL & VALID & EF
    FE_SVC --> SIMPLE & REDIS & EF
```

## Runtime Service Dependencies

```mermaid
flowchart LR
    subgraph clients [Clients]
        Browser[Partner Websites]
        PartnerAPI[Partner API]
    end

    subgraph iis [IIS Applications]
        FE[FormsEngine]
        ME[MatchingEngine]
        VA[VendorWebAPI]
    end

    subgraph wcf [WCF Services - External to Repo]
        Prospect[Prospect Service]
        Validation[Validation Engine]
        LeadScoring[Lead Scoring]
        Pixels[Pixels Service]
        Five9[Five9 / GpFive9]
        LeadPing[LeadPing]
    end

    subgraph http [HTTP Services]
        EMS[EMS Lead Engine]
        ExtMatch[External Match API]
        SpamCheck[Spam Check API]
        Targus[Targus Phone Validation]
        xVerify[xVerify Email]
    end

    subgraph data [Data Stores]
        Nexus[(Nexus SQL)]
        Tracking[(EddyTracking)]
        EddyLog[(EddyLogging)]
        RedisCache[(Redis)]
    end

    Browser --> FE
    PartnerAPI --> VA
  FE --> ME
    FE --> Prospect & Validation & LeadScoring & Pixels & Five9 & EMS
    VA --> FE & ME & Prospect & Five9 & EMS & ExtMatch
    ME --> LeadPing & ExtMatch & SpamCheck
    FE --> Nexus & EddyLog & RedisCache
    ME --> Nexus & Tracking
    VA --> Nexus & EddyLog & RedisCache
    FE --> Targus & xVerify
```

## Data Flow — Request Lifecycle

```mermaid
sequenceDiagram
    participant C as Client
    participant FE as FormsEngine
    participant ME as MatchingEngine
    participant LE as LeadEngine
    participant PS as Prospect Service
    participant DB as Nexus SQL

    C->>FE: Form submission
    FE->>FE: Session read (FESession)
    FE->>ME: ValidateProgram (WCF)
    ME-->>FE: Validation result + MatchResponseGuid
    FE->>PS: SaveProspect (WCF)
    PS-->>FE: ProspectId
    FE->>LE: CreateLead(LeadCreateRequest)
    LE->>DB: INSERT Lead
    FE->>DB: Save Submission
    FE->>LE: UpdateLeads (link submission)
    LE->>DB: EXEC EDDY_FE_Lead_Update
    FE-->>C: JSONP response
```

## Authentication Flow (VendorWebAPI)

```mermaid
sequenceDiagram
    participant P as Partner
    participant F as CampaignAuthorizationFilter
    participant VC as VendorCampaigns
    participant DB as Nexus SQL
    participant API as Controller

    P->>F: POST /api/Lead/save (apikey in body)
    F->>F: Parse apikey as GUID
    F->>VC: GetVendorCampaignByTrackId
    VC->>DB: Query VendorCampaign
    alt invalid key
        F-->>P: 401 Unauthorized
    else campaign disabled
        F-->>P: 401 Campaign Expired/Inactive
    else rate limit exceeded
        F-->>P: 401 APIRateExceeded
    else valid
        F->>API: Continue to action filters
        API-->>P: 200 Lead response
    end
```

## Background Processing Flow

```mermaid
flowchart TD
    subgraph startup [Application Startup]
        A1[FormsEngine: Task.Run WarmupCache]
        A2[FormsEngine: Task.Run CompileComponents]
        A3[MatchingEngine: PreloadEntireCache]
        A4[VendorWebAPI: LoadSupportingCache]
    end

    subgraph request [Per Request - Fire and Forget]
        B1[FormsEngine: Task.Run SaveSubmissionAsync]
        B2[FormsEngine: Task.Run EmailXverify]
        B3[FormsEngine: Task.Run SaveProspectAsync]
        B4[MatchingEngine: Task.Run MatchPersister]
        B5[MatchingEngine: ThreadPool QueueUserWorkItem]
    end

    subgraph batch [Scheduled Batch]
        C1[SEO Allocation Console]
        C2[Match Response Replay Console]
    end

    A1 & A2 & A3 & A4 --> Ready[Application Ready]
    Ready --> B1 & B2 & B3 & B4 & B5
    C1 --> ME[MatchingEngine WCF]
    C2 --> ME
```

## Class Diagram — Lead Creation Pipeline

```mermaid
classDiagram
    class TemplateManagerControllerBase {
        +ProcessSubmit()
        +ManagedChoiceLeadSubmission()
        +CrossSellLeadSubmission()
    }
    class SubmissionService {
        +SubmitSchoolPickerWizard()
    }
    class LeadSaveManger {
        +Execute()
        +SaveSubmissionAndUpdateLeadsAsync()
    }
    class LeadEngine {
        +CreateLeadDTO()
        +CreateLead()
        +UpdateLeads()
    }
    class LeadDataService {
        +SaveLead()
        +UpdateLead()
    }
    class EntityBuildHelper {
        +BuildLeadCreateRequest()
    }
    class Nexus_Lead_Entities {
        +DbSet~Lead~
        +DbSet~BetaLead~
        +DbSet~RawPostData~
    }

    TemplateManagerControllerBase --> LeadSaveManger
    SubmissionService --> LeadSaveManger
    LeadSaveManger --> LeadEngine
    LeadSaveManger --> EntityBuildHelper
    LeadEngine --> LeadDataService
    LeadDataService --> Nexus_Lead_Entities
```

## Class Diagram — MatchingEngine Core

```mermaid
classDiagram
    class MatchingService {
        +GetInstitutions()
        +GetWizardMatches()
        +ValidateProgram()
    }
    class MatchingEngine {
        +GetWizardMatches()
        +GetInstitutions()
        +GetProgramsForCrossSell()
    }
    class RulesEngine {
        +GetRulesResult()
        +GetValidateProgramMatchResult()
    }
    class SchoolRankingEngine {
        +SortMatchItems()
    }
    class MatchDatabase {
        +FilterProgramProducts()
    }
    class MatchingEngineCache {
        +PreloadEntireCache()
        +GetCacheItem()
    }
    class MatchAggregator {
        +BuildInstitutionResponse()
        +BuildWizardMatchResponse()
    }
    class RulesEngineFactory {
        +CreateRulesEngine()
    }

    MatchingService --> MatchingEngine
    MatchingEngine --> RulesEngine
    MatchingEngine --> SchoolRankingEngine
    MatchingEngine --> MatchDatabase
    MatchingEngine --> MatchingEngineCache
    MatchingService --> MatchAggregator
    RulesEngine --> RulesEngineFactory
```

## Flowchart — Program Validation Decision

```mermaid
flowchart TD
    A[ValidateProgram Request] --> B{Campaign valid?}
    B -->|No| Z[Return error]
    B -->|Yes| C[Load program from cache]
    C --> D[RulesEngine.GetValidateProgramMatchResult]
    D --> E{Rules pass?}
    E -->|Yes| F{Lead scoring check}
    F -->|Pass| G[Return valid + CPL]
    F -->|Fail| H[Return tier restriction]
    E -->|No| I{Alternate program available?}
    I -->|Yes| J[Return alternate ProgramProductId]
    I -->|No| K{EPLite upsell?}
    K -->|Yes| L[Return EPLite offer]
    K -->|No| M{Warm Transfer Titanium?}
    M -->|Yes| N[Return WT upsell]
    M -->|No| O[Return validation failure]
```
