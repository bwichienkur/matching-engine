# VendorWebAPI — Project Documentation

## EDDY.IS.Vendor.Web.API

**Path:** `VendorWebAPI/EDDY.IS.Vendor.Web.API/`  
**Type:** ASP.NET Web API 2 + MVC 5 (.NET Framework 4.8)

### Purpose
Partner-facing REST API for education lead generation: directory browsing, program details, lead/prospect submission, call-center and Five9 integrations.

### Key Components
- 12 Web API controllers + 2 MVC controllers
- 25+ authorization/validation action filters
- `CampaignAuthorizationFilter` — global API key auth
- Help Page area for API documentation
- Response models that invoke Business layer in constructors

### IIS Deployment
- App pool: `CheetahPool`
- Path: `F:\inetpub\wwwroot\EDDY.IS.Vendor.Web.API`

---

## EDDY.IS.Vendor.Business

**Path:** `VendorWebAPI/EDDY.IS.Vendor.Business/`

### Purpose
Business/service layer — orchestrates DAOs, no interfaces.

### Classes
`VendorBase`, `VendorCampaigns`, `Programs`, `Institutions`, `Campuses`, `Categories`, `Subjects`, `Specialties`, `Levels`, `Zips`, `Locations`, `MilitaryStatuses`, `Prospects`, `Leads`, `GpFive9`, `Timezones`, `Logs`, `VendorResponseMessages`

---

## EDDY.IS.Vendor.DataAccess

**Path:** `VendorWebAPI/EDDY.IS.Vendor.DataAccess/`

### Purpose
Data access: EF6 DbContexts, DAOs, WCF proxies, Redis, HTTP to EMS.

### DbContexts
- `APINexusEntities` — Nexus DB (campaigns, programs, institutions, leads)
- `EddyLoggingEntities` — API request/response logs

### DAOs (18 classes)
`MatchingServiceDAO`, `FormsServiceDAO`, `ProspectServiceDAO`, `GpFive9ServiceDAO`, `DataExchangeServiceDAO`, `VendorCampaignsDAO`, `ProgramDAO`, `InstitutionDAO`, `CampusDAO`, `CategoryDAO`, `SubjectDAO`, `ProgramLevelDAO`, `MilitaryStatusDAO`, `ZipDAO`, `LocationsDAO`, `TimezoneServiceDAO`, `LogsDAO`, `VendorResponseMessagesDAO`, `RedisHelper`

---

## EDDY.IS.Vendor.Entities

**Path:** `VendorWebAPI/EDDY.IS.Vendor.Entities/`  
64 entity/DTO files: `ContactRequest`, `DirectoryRequest`, `Program`, `Institution`, `VendorCampaign`, `LeadSubmissionResponse`, etc.

---

## EDDY.IS.Vendor.Utilities

**Path:** `VendorWebAPI/EDDY.IS.Vendor.Utilities/`

Cross-cutting: `CacheStore`, `InputValidation`, `HttpRequestExtensions`, constants, logging helpers.

---

## EDDY.IS.Vendor.Web.API.Tests

**Path:** `VendorWebAPI/EDDY.IS.Vendor.Web.API.Tests/`  
MSTest 4.2.3 + Moq 4.10.0 — 56 tests across 5 controller/filter test classes. Load test artifacts included.
