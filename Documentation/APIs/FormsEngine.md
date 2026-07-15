# FormsEngine API Documentation

**Base URL pattern:** `/{Controller}/{Action}` (ASP.NET MVC convention)  
**Response format:** JSONP (`JsonpResult`) for most endpoints  
**Authentication:** None (public API)  
**HTTP methods:** Almost exclusively GET (one POST endpoint)

> **Security note:** State-changing operations use GET, which is vulnerable to CSRF and unintended caching. This is a legacy design choice for JSONP cross-origin embedding.

---

## CacheHelperController

| Method | URL | Auth | Description |
|--------|-----|------|-------------|
| GET | `/CacheHelper/ResetTemplateCache?TemplateId={id}` | None | Clears template cache for given ID |

---

## CCPAController

| Method | URL | Auth | Description |
|--------|-----|------|-------------|
| GET | `/CCPA/CCPAMessage` | None | Returns CCPA compliance partial view |

---

## ComponentsController

| Method | URL | Auth | Request | Response |
|--------|-----|------|---------|----------|
| GET | `/Components/GetFailedMatchReplacementComponents` | None | `IEnumerable<Match> matches` | Rendered component HTML |

---

## DataBindController

Reference data endpoints for form dropdowns. All GET, JSONP.

| Method | URL | Parameters | Business Logic |
|--------|-----|------------|----------------|
| GET | `/DataBind/GetStates` | `DataBindFilter` | States by country |
| GET | `/DataBind/GetCountries` | `DataBindFilter` | Country list |
| GET | `/DataBind/GetDesiredCountries` | `DataBindFilter` | Desired study countries |
| GET | `/DataBind/GetDesiredCities` | `DataBindFilter` | Desired study cities |
| GET | `/DataBind/GetCity` | `DataBindFilter` | City lookup |
| GET | `/DataBind/GetProgramLevels` | `DataBindFilter` | Program levels |
| GET | `/DataBind/GetPrograms` | `DataBindFilter` | Programs for filters |
| GET | `/DataBind/GetCampusTypes` | `DataBindFilter` | Campus types |
| GET | `/DataBind/GetCategories` | `DataBindFilter` | Categories |
| GET | `/DataBind/GetSubCategories` | `DataBindFilter` | Subcategories/subjects |
| GET | `/DataBind/GetSpecialties` | `DataBindFilter` | Specialties |
| GET | `/DataBind/GetResourceMetaDataTextForKey` | `Keys` (string) | Resource metadata text |
| GET | `/DataBind/GetResourceMetaDataTextForTCPA` | — | TCPA consent text |
| GET | `/DataBind/GetLandingPageSettings` | `formLeadUrl` | Landing page config |
| GET | `/DataBind/GetEMSInstitutionTCPAText` | `InstitutionId` | EMS institution TCPA |
| GET | `/DataBind/SearchPreDefinedValueList` | `term`, `standardControlCode` | Autocomplete search |
| GET | `/DataBind/GetKVCodeData` | `DataBindFilter` | Key-value code data |
| GET | `/DataBind/GetCampuses` | `DataBindFilter` | Campuses |
| GET | `/DataBind/GetCampusLocations` | `DataBindFilter` | Campus locations |

**Dependencies:** `DataBindControllerBase` → `FormsEngine` facade, EF data services

---

## FormValidationController

| Method | URL | Parameters | Validation |
|--------|-----|------------|------------|
| GET | `/FormValidation/ProfanityCheck` | `Value` | Profanity filter |
| GET | `/FormValidation/PhoneNumberCheck` | `PhoneNumber`, `CountryCode` | Phone format + Targus |
| GET | `/FormValidation/EmailCheck` | `EmailAddress`, `EmailVerificationLevel` | DNS/MX verification |
| GET | `/FormValidation/EmailCheckEx` | Same + async | xVerify async path |
| GET | `/FormValidation/GetCityStateCountry` | `ZipCode` | Zip → city/state/country |
| GET | `/FormValidation/ZipCodeStateCountryCheck` | zip, state, country | Consistency check |
| GET | `/FormValidation/BirthDateCheck` | `BirthDate` | Age validation |
| GET | `/FormValidation/GetStatesByCountry` | `CountryCode` | States list |
| GET | `/FormValidation/IsMobilePhone` | `PhoneNumber` | Mobile detection |
| GET | `/FormValidation/GetISMapping` | `set`, `category`, `source` | IS mapping lookup |

**Error handling:** Returns JSONP with validation result; exceptions logged via `ISException`

---

## InstitutionController

| Method | URL | Request | Response |
|--------|-----|---------|----------|
| GET | `/Institution/GetInstitution` | `FormRequest` | Institution details for widget |

**DI:** Uses `IInstitutionService` (SimpleInjector)

---

## LocationController

| Method | URL | Parameters | Description |
|--------|-----|------------|-------------|
| GET | `/Location/GetPostalCode` | `ipOverride` (optional) | Geo lookup from IP |

---

## MatchingController

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/Matching/GetFormPrograms` | Programs for form display |
| GET | `/Matching/GetCategories` | Categories for matching |
| GET | `/Matching/GetProgramLevels` | Program levels |
| GET | `/Matching/GetSubCategories` | Subcategories |
| GET | `/Matching/CountryCheck` | Country eligibility |
| GET | `/Matching/GetProgramDetail` | Single program detail |
| GET | `/Matching/GetProgram` | Program by ID |
| GET | `/Matching/GetProgramsForCounter` | Programs for counter widget |
| GET | `/Matching/GetCampaignDetailByTrackId` | Campaign by track ID |

**Dependencies:** WCF `MatchingServiceClient`

---

## SessionController

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/Session/GetSessionId` | Create new FESession |
| GET | `/Session/GetClonedSessionId` | Clone existing session |
| GET | `/Session/GetObject` | Read session key |
| GET | `/Session/PingSession` | Keep-alive |
| GET | `/Session/SetObject` | Write session key |
| GET | `/Session/GetAdditionalQAObject` | Additional Q&A data |
| GET | `/Session/SetAdditionalQAObject` | Set additional Q&A |
| GET | `/Session/SetWorkflowStatus` | Update workflow state |
| GET | `/Session/GetWorkflowStatus` | Read workflow state |
| GET | `/Session/GetFormSessionValues` | All form values |
| GET | `/Session/GetIframePixels` | Tracking pixels |

**Storage:** `HttpRuntime.Cache` + Redis extension

---

## SubmissionController

| Method | URL | Request | Business Logic |
|--------|-----|---------|----------------|
| GET | `/Submission/SubmitSchoolPickerWizard` | `FormRequest` | Full School Picker pipeline via `ISubmissionService` |

**DI:** Constructor-injected `ISubmissionService`, `IThankYouPageService`

---

## TemplateManagerController (Primary Surface)

| Method | URL | Business Process |
|--------|-----|------------------|
| GET | `/TemplateManager/GetProgramTemplate` | Render program form HTML |
| GET | `/TemplateManager/GetQDFTemplate` | QDF template |
| GET | `/TemplateManager/GetWizardTemplate` | Wizard template |
| GET | `/TemplateManager/GetWizardTemplateJS` | Wizard JavaScript |
| GET | `/TemplateManager/GetAdditionalTemplateQuestions` | Extra wizard questions |
| GET | `/TemplateManager/GetAdditionalTemplateQuestionCollection` | Question collection |
| GET | `/TemplateManager/GetMatchingEngineWizardResponse` | Wizard match results |
| GET | `/TemplateManager/AnySchoolMatches` | Pre-check for matches |
| GET | `/TemplateManager/GetManagedChoice` | Managed choice UI |
| GET | `/TemplateManager/SaveProspectJobContactMe` | Job contact-me prospect |
| GET | `/TemplateManager/GetThankYou` | Thank-you page (legacy) |
| GET | `/TemplateManager/GetNoMatch` | No-match page |
| GET | `/TemplateManager/CheckMobileNumbers` | Mobile validation |
| GET | `/TemplateManager/GetRenderingStrategies` | HTML rendering strategies |
| GET | `/TemplateManager/GetRenderingStrategiesByType` | Strategies by type |
| GET | `/TemplateManager/GetThemes` | Theme list |
| GET | `/TemplateManager/GetTemplateListByType` | Templates by type |
| GET | `/TemplateManager/GetTemplateList` | All templates |
| GET | `/TemplateManager/ValidateForm` | Server-side form validation |
| GET | `/TemplateManager/ProcessHostAndPostLead` | Host-and-post submission |
| GET | `/TemplateManager/GetProgramMatches` | Program match list |
| GET | `/TemplateManager/SaveProspect` | Save prospect |
| GET | `/TemplateManager/SaveProspectAdditionalInfo` | Additional prospect info |
| GET | `/TemplateManager/ProcessSubmit` | **Primary form submission** |
| GET | `/TemplateManager/GetOptimizelyCrossSell` | Optimizely cross-sell |
| GET | `/TemplateManager/CrossSellLeadSubmission` | Cross-sell lead save |
| GET | `/TemplateManager/ManagedChoiceLeadSubmission` | Managed choice lead save |
| GET | `/TemplateManager/GetProgramTemplateModel` | Template model JSON |
| GET | `/TemplateManager/ResubmitProspects` | Prospect resubmission |
| GET | `/TemplateManager/GetMatchedCampusDetail` | Campus detail |
| GET | `/TemplateManager/LogClientException` | Client error logging |

---

## ThankYouController

| Method | URL | Request |
|--------|-----|---------|
| GET | `/ThankYou/GetThankYouPage` | `FormRequest` |

**DI:** `IThankYouPageService`

---

## SchoolPickerWizardController

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/SchoolPickerWizard/GetFailedMatchReplacements` | Replacement schools for failed validation |
| GET | `/SchoolPickerWizard/GetSchoolPickerCarouselComponents` | Carousel UI components |

---

## StaticController

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/Static/GetCss` | Form CSS |
| GET | `/Static/GetCommonCss` | Common CSS |
| GET | `/Static/GetJs` | Form JavaScript |
| GET | `/Static/GetBundledWizardJs` | Wizard JS bundle |
| GET | `/Static/GetBundledQDFJs` | QDF JS bundle |
| GET | `/Static/GetBundledQDFPluginJs` | QDF plugin JS |
| GET | `/Static/GetBundledWizardCSS` | Wizard CSS bundle |
| GET | `/Static/GetBundledQDFCSS` | QDF CSS bundle |
| GET | `/Static/GetBundledQDFPluginCSS` | QDF plugin CSS |
| GET | `/Static/GetClient` | Client JS |
| GET | `/Static/GetWizardClient` | Wizard client JS |
| GET | `/Static/GetGlobal` | Global config JS |
| GET | `/Static/GetDFPScript` | Google DFP ad script |

**DI:** Constructor-injected services

---

## WidgetSupportController

| Method | URL | Auth | Request |
|--------|-----|------|---------|
| POST | `/WidgetSupport/GetInstitutionDetails` | None | `FormRequest` |

**Note:** Only POST endpoint in FormsEngine MVC controllers.

---

## Classic WCF API (FormsEngineAPI.svc)

Consumed by VendorWebAPI via `FormsServiceDAO`.

**Endpoint:** `http://{server}/EDDY.IS.FormsEngine.Service/Classic/FormsEngineAPI.svc`  
**Contract:** `IFormsEngine` — lead save, program matches, form templates, EduMax, external match

**Reference:** `FormsEngine/EDDY.IS.FormsEngine.Services/Classic/FormsEngineAPI.svc.cs`
