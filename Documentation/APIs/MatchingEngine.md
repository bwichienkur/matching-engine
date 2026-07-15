# MatchingEngine WCF API Documentation

**Service:** `MatchingService.svc`  
**Contract:** `IMatchingService` (`MatchingEngine/EDDY.IS.MatchingEngine.Service/IMatchingService.cs`)  
**Protocol:** WCF with `[WebInvoke]` JSON (wrapped body style)  
**Authentication:** None (network perimeter)  
**Base URL:** `http://{server}/EDDY.IS.MatchingEngine.Service/MatchingService.svc`

---

## Directory / Listing Operations

| Operation | Request | Response | Description |
|-----------|---------|----------|-------------|
| `GetInstitutions` | `DirectoryMatchRequest`, `GetInstitutionCampusOption` | `InstitutionResponse` | Paginated institution list with SRA ranking |
| `GetPrograms` | `DirectoryMatchRequest`, `includeProgramDetail` | `ProgramResponse` | Program listing |
| `GetCampuses` | `DirectoryMatchRequest` | `CampusResponse` | Campus listing |
| `GetCategories` | `DirectoryMatchRequest` | `CategoryResponse` | Category facets |
| `GetSubjects` | `DirectoryMatchRequest` | `SubjectResponse` | Subject facets |
| `GetSpecialties` | `DirectoryMatchRequest` | `SpecialtyResponse` | Specialty facets |
| `GetProgramLevels` | `DirectoryMatchRequest` | `ProgramLevelResponse` | Program level facets |
| `GetCampusTypes` | `DirectoryMatchRequest` | `CampusTypeMatchResponse` | Campus type facets |
| `GetFacetedNavigation` | `DirectoryMatchRequest` | `NavigationResponse` | Combined facet navigation |
| `GetSiteMapGeoInfo` | `DirectoryMatchRequest` | `SiteMapResponse` | Geo info for sitemaps |
| `GetFormPrograms` | `DirectoryMatchRequest` | `FormProgramResponse` | Programs for form display |

### Fallback Operations (return all if none match filters)

| Operation | Request | Response |
|-----------|---------|----------|
| `GetCategoriesAllIfNone` | `DirectoryMatchRequest` | `CategoryResponse` |
| `GetSubjectsAllIfNone` | `DirectoryMatchRequest` | `SubjectResponse` |
| `GetSpecialtiesAllIfNone` | `DirectoryMatchRequest` | `SpecialtyResponse` |
| `GetProgramLevelsAllIfNone` | `DirectoryMatchRequest` | `ProgramLevelResponse` |

---

## Wizard Operations

| Operation | Request | Response | Description |
|-----------|---------|----------|-------------|
| `GetWizardMatches` | `WizardMatchRequest` | `WizardMatchResponse` | Smart match + school selection + limbo |
| `GetFormPrograms` | `DirectoryMatchRequest` | `FormProgramResponse` | Programs for wizard forms |

---

## Cross-Sell

| Operation | Request | Response |
|-----------|---------|----------|
| `GetProgramsForCrossSell` | `CrossSellMatchRequest` | `CrossSellProgramResponse` |

---

## Validation

| Operation | Request | Response | Description |
|-----------|---------|----------|-------------|
| `ValidateProgram` | `ProgramValidateRequest` | `ProgramValidateResponse` | Single program validation with alternates |
| `ValidateAPIProgram` | `APIProgramValidateRequest` | `APIProgramValidateResponse` | API-specific validation path |

---

## Detail Operations

| Operation | Parameters | Response |
|-----------|------------|----------|
| `GetInstitutionDetails` | `applicationId`, `institutionId`, `TrackGuid` | `InstitutionDetailResponse` |
| `GetProgramDetails` | `applicationId`, `programId`, `TrackGuid`, `includeProgramGroupRollup`, `campusId` | `ProgramDetailResponse` |
| `GetProgramDisplayGroupDetails` | `applicationId`, `programDisplayGroupId`, `TrackGuid` | `ProgramDetailResponse` |
| `GetCampaignDetailByTrackId` | `trackId` (string) | `CampaignDetailResponse` |

---

## Special Match Types

| Operation | Request | Response | Description |
|-----------|---------|----------|-------------|
| `GetNeoResponse` | `NeoMatchRequest` | `NeoResponse` | Neo match flow |
| `GetApolloCampuses` | `ApolloCampusRequest` | `ApolloCampusResponse` | Apollo campus matching |

---

## Common Request Fields (DirectoryMatchRequest)

| Field | Type | Purpose |
|-------|------|---------|
| `TrackGuid` | Guid | Campaign track identifier |
| `ApplicationId` | int | Application context (e.g., 7 = GradSchools) |
| `ZipCode` | string | Geographic filter |
| `StateCode` | string | State filter |
| `CountryCode` | string | Country filter |
| `CategoryId` | int? | Category filter |
| `SubjectId` | int? | Subject filter |
| `SpecialtyId` | int? | Specialty filter |
| `ProgramLevelId` | int? | Level filter |
| `EducationLevelId` | int? | Education level |
| `PageNumber` / `PageSize` | int | Pagination |

---

## Error Handling

- WCF faults with `includeExceptionDetailInFaults=true` in debug config
- Empty results returned as valid responses (not errors)
- Async logging failures do not affect response (fire-and-forget `Task.Run`)
- Beta mode (`IsBeta=true`) suppresses match response logging

---

## Caching

- 15-minute `HttpRuntime.Cache` for navigation/category/subject endpoints
- Full engine cache preloaded at startup (60+ items, configurable TTL per item)

---

## Consumers

| Consumer | Client Class | File |
|----------|-------------|------|
| FormsEngine | `MatchingServiceClient` | `FormsRelatedServices.MatchingEngine.cs` |
| VendorWebAPI | WCF proxy | `MatchingServiceDAO.cs` |
| SEO Allocation | `MatchingService` direct | `DrupalNodeProcessStep.cs` |
