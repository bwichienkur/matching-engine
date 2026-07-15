# VendorWebAPI REST Documentation

**Base URL pattern:** `api/{controller}/{action}`  
**Response format:** JSON (`VendorResponseBase` hierarchy)  
**Authentication:** Campaign API key (`apikey` GUID in query string or JSON body)  
**CORS:** Enabled (`origins: "*"`) on most controllers

---

## Authentication

All endpoints (except `Util/checkapikey`) require `CampaignAuthorizationFilter`:

1. Extract `apikey` from query string (GET) or JSON body (POST)
2. Parse as GUID
3. Load `VendorCampaign` from Nexus DB
4. Validate: enabled, not expired/pending/inactive/terminated, rate limit not exceeded
5. Return `401 Unauthorized` with `VendorResponseBase` JSON on failure

**Reference:** `VendorWebAPI/EDDY.IS.Vendor.Web.API/Filters/CampaignAuthorizationFilter.cs`

---

## DirectoryController

All POST. Route prefix: `/api/Directory/`

| Action | Route | Request | Response | Filters |
|--------|-------|---------|----------|---------|
| `GetPrograms` | `programs` | `DirectoryRequest` | `GetProgramsResponseModel` | Paging, category, subject filters |
| `GetProgramMatches` | `programmatches` | `ContactRequest` | Program matches | `PostProgramMatchesActionFilter`, `ProgramMatchesLeadSourceFilter` |
| `GetInstitutions` | `institutions` | `DirectoryRequest` | Institution list | `PostInstitutionActionFilter`, paging |
| `GetCampuses` | `campuses` | `DirectoryRequest` | Campus list | Campus filters |
| `GetCategories` | `categories` | `DirectoryRequest` | Category list | `CategoryFilter` |
| `GetSubCategories` | `subjects` | `DirectoryRequest` | Subject list | `SubjectFilter` |
| `GetSpecialties` | `specialties` | `DirectoryRequest` | Specialty list | — |
| `GetLevels` | `programlevels` | `DirectoryRequest` | Level list | `ProgramLevelFilter` |
| `GetAllocation` | `allocation` | `DirectoryRequest` | Allocation status | — |
| `GetCampusesForMicrosites` | `campusesformicrosites` | `DirectoryRequest` | Microsite campuses | Redis cached |
| `GetZipsByCampusId` | `zipcodes` | `DirectoryRequest` | Zip codes | — |
| `GetProgramsForMicrosites` | `programsformicrosites` | `DirectoryRequest` | Microsite programs | Redis cached |
| `DeleteProgramsCacheForMicrosites` | `clearcacheprogramsformicrosites` | `DirectoryRequest` | Cache clear | Missing `[HttpPost]` attribute |
| `DeleteCampusesCacheForMicrosites` | `clearcampusesformicrosites` | `DirectoryRequest` | Cache clear | — |

**Business logic:** `MatchingServiceDAO` → WCF MatchingEngine  
**Caching:** Redis for microsite endpoints (`RedisHelper`)

---

## ProgramController

| Action | Route | Request | Response | Filters |
|--------|-------|---------|----------|---------|
| `GetProgramDetails` | `details` | `DirectoryRequest` | `GetProgramDetailsResponseModel` | `PostProgramDetailsActionFilter`, `APIDirectoryCampaignFilter` |
| `GetProgramFormTemplate` | `form` | `ContactRequest` | Form template | `PostProgramFormActionFilter` |

**Business logic:** `Programs` business class → `MatchingServiceDAO` / `FormsServiceDAO`

---

## LeadController

| Action | Route | Request | Response | Filters |
|--------|-------|---------|----------|---------|
| `PostLead` | `save` | `ContactRequest` | `PostLeadResponseModel` | `PostLeadActionFilter`, `TCPAFilter`, `LeadSourceFilter`, campus/category/subject/institution/program filters |

**Business logic:** `Leads.SaveLead` → `FormsServiceDAO.SaveLead` → FormsEngine WCF  
**Logging:** `EddyApiLog` table

---

## InstitutionsController

| Action | Route | Request | Response | Filters |
|--------|-------|---------|----------|---------|
| `PostLeadBasic` | `lead-save` | `ContactRequest` | Lead response | `EMSPostLeadActionFilter`, `HostAndPostCampaignFilter` |
| `PostLeadUpdate` | `lead-update` | `LeadUpdateRequest` | Update response | `LeadUpdateValidationFilter` |

**Business logic:** EMS lead save via FormsEngine; update via `DataExchangeServiceDAO` → EMS HTTP API

---

## MarketingController

| Action | Route | Request | Filters |
|--------|-------|---------|---------|
| `PostLeadBasic` | `lead-save` | `ContactRequest` | `EMSPostLeadActionFilter` |

---

## CallCenterLeadController

| Action | Route | Request | Filters |
|--------|-------|---------|---------|
| `PostLead` | `save` | `ContactRequest` | `PostLeadActionFilter` |
| `PostEduMaxLead` | `edumaxsave` | `ContactRequest` | `EDUMAXPostLeadActionFilter` |

**Business logic:** `Leads.SaveCallCenterLead`, `Leads.SaveEduMaxLead` → FormsEngine + External Match

---

## ProspectController

| Action | Route | Request | Filters |
|--------|-------|---------|---------|
| `PostProspect` | `save` | `ContactRequest` | `PostProspectActionFilter`, `ProspectCampaignFilter`, `ProspectSourceFilter` |
| `ValidateProspect` | `validate` | `ContactRequest` | `PostValidateProspectActionFilter` |
| `PostProspectSaveByProspectId` | `savebyProspectid` | `ContactRequest` | Prospect filters |

**Business logic:** `Prospects` → `ProspectServiceDAO` → WCF Prospect Service

---

## GpFive9Controller

| Action | Route | Request | Description |
|--------|-------|---------|-------------|
| `PostLeadNote` | `routelead` | `ContactRequest` | Route lead to Five9 dialer |

**Business logic:** `GpFive9.RouteLead` → `GpFive9ServiceDAO` → WCF

---

## UtilController

| Action | Route | Method | Auth | Description |
|--------|-------|--------|------|-------------|
| `CheckApiKey` | `checkapikey` | GET | None | Validate API key |
| `GetTimezone` | `gettimezone` | GET | Campaign auth | Timezone by zip/phone |

---

## MVC Endpoints (non-API)

| Controller | Route | Method | Auth | Description |
|------------|-------|--------|------|-------------|
| `LeadDetailsController` | `/LeadDetails` | GET | **None** | Lead details HTML view |
| `LeadDetailsController` | `/LeadDetails/SaveNote` | POST | **None** | Save lead note |
| `HelpController` | `/` | GET | None | API Help Page |

> **Security gap:** `LeadDetailsController` has no `CampaignAuthorizationFilter`.

---

## Standard Response Structure

```json
{
  "Success": true,
  "MessageCode": "OK",
  "Message": "Success message",
  "Data": { }
}
```

Error responses use `VendorResponseMessage` with localized message codes from `VendorResponseMessagesDAO`.

---

## Validation Filter Summary

| Filter | Validates |
|--------|-----------|
| `TCPAFilter` | TCPA consent present |
| `LeadSourceFilter` | Lead source URL required |
| `AgentNameFilter` | Agent name required |
| `AgeFilter` | Age within campaign rules |
| `CampusFilter` | Campus ID exists and enabled |
| `CategoryFilter` | Category ID exists |
| `SubjectFilter` | Subject ID exists |
| `InstitutionFilter` | Institution ID valid |
| `ProgramFilter` | Program ID valid |
| `PagingActionFilter` | Page number/size valid |
| `HostAndPostCampaignFilter` | Campaign supports host-and-post |
