# Entity Documentation

## LeadEngine Entities

### Lead

| Property | Purpose |
|----------|---------|
| **Purpose** | Production education lead record — central entity for the lead delivery pipeline |
| **Table** | `dbo.Lead` |
| **PK** | `LeadId` (decimal, identity) |

**Key Relationships:**
- `SubmissionId` → `dbo.Submission` (set post-create via SP)
- `ProgramProductId` → program inventory (logical, not EF navigation)
- `MatchResponseGuid` → MatchingEngine match response (cross-system GUID)
- `ProspectId` → Prospect Service CRM record
- `RawPostDataId` → `dbo.RawPostData`

**Business Fields:**
- Contact: `FirstName`, `LastName`, `EmailAddress`, `Phone1`, `Address1`, `City`, `StateProvince`, `ZipCode`, `CountryCode`
- Campaign: `TrackId`, `AffiliateId`, `FormUniqueId`
- Tracking: `TrackingSessionGUID`, `SessionInternalId`, `VisitorInternalId`
- Delivery: `RealtimeDeliveryStatusId`, `DeliveryDefinitionId`, `DeliveryEngineMachineKey`
- Revenue: `UnaudittedRevenue`, `EstimatedRevShare`, `EstimatedLeadRev`
- Metadata: `AdditionalFields` (XML), `LeadCreationTypeId`, `IsCallCenter`, `IsCapProcessed`

**Validation:** Performed upstream in FormsEngine/ValidationEngine before `CreateLead` is called. LeadEngine assigns status codes based on validation outcome.

**Lifecycle:**
1. Created via `LeadDataService.SaveLead` (EF insert)
2. Updated via `LeadDataService.UpdateLead` (EF + `EDDY_FE_Lead_Update` SP)
3. Status transitions managed by delivery engine (outside this repo)

**Reference:** `LeadEngine/EDDY.IS.LeadEngine-RF/DataModel/Lead.cs`

---

### BetaLead

Identical schema to `Lead` but written to `dbo.BetaLead` for non-production testing. Selected when host app runs in beta mode.

---

### RawPostData

| Property | Purpose |
|----------|---------|
| **Purpose** | Captures raw HTTP POST payload for audit and replay |
| **Table** | `dbo.RawPostData` |
| **PK** | `RawPostDataID` (bigint) |

Created before lead update in `UpdateLeads` flow. Linked to leads via `RawPostDataId` on `Lead` row.

---

## FormsEngine Entities (Selected)

### Submission

| Property | Value |
|----------|-------|
| **Purpose** | Records a form submission event |
| **Relationships** | Has many `SubmissionDetail`, `SubmissionMatchResponse`, `SubmissionValidationError` |
| **Business meaning** | One user form submit action; may create multiple leads |

### Template

| Property | Value |
|----------|-------|
| **Purpose** | Defines form structure (fields, steps, sections) |
| **Relationships** | Has many `TemplateControl`, `TemplateStep`, `TemplateSection` |
| **Business meaning** | Configurable form blueprint assigned to campaigns |

### TemplateControl

Individual form field definition with validation rules, data types, and display properties. Links to `StandardControl` for reusable field types.

### Campaign

Campaign configuration including track GUID, status, and template assignments. Shared across FormsEngine and VendorWebAPI (different EF models mapping same table).

---

## MatchingEngine View Entities (Selected)

### VW_Matching_Campaign

Denormalized campaign configuration loaded into `MatchingEngineCache`. Contains track GUID, business model, application ID, channel settings.

### VW_Matching_ProgramProductWithApplicationSubject

Core inventory item for matching. Each row represents a program-product combination with application, subject, and category mappings. Used by `MatchDatabase` inverted indexes.

### VW_Matching_CapHierarchy

Volume cap configuration at campaign, channel, and client-relationship levels. Used by `LeadCap` and `CRChannelCap` rules.

---

## VendorWebAPI Entities (Selected)

### VendorCampaign

| Property | Purpose |
|----------|---------|
| **Purpose** | Maps API key (TrackGuid) to campaign with authorization metadata |
| **Key fields** | `TrackGuid`, `IsEnabled`, `CampaignStatus`, rate limit settings |
| **Used by** | `CampaignAuthorizationFilter` |

### ContactRequest

Primary API request DTO for lead/prospect submission. Contains contact fields, program selections, TCPA consent, lead source URL, and `apikey`.

### VendorResponseBase

Standard API response wrapper with `Success`, `MessageCode`, `Message`, and typed `Data` payload.

---

## Entity Documentation Index

For complete entity lists, see:
- FormsEngine: 40+ entities in `FormsEngine/EDDY.IS.FormsEngine.RF/DataModel/`
- LeadEngine: 3 entities in `LeadEngine/EDDY.IS.LeadEngine-RF/DataModel/`
- MatchingEngine: 72 view entities in `MatchingEngine/EDDY.IS.MatchingEngine/DataModel/Entity/`
- VendorWebAPI: 30+ EF entities in `VendorWebAPI/EDDY.IS.Vendor.DataAccess/DataModels/`
- VendorWebAPI DTOs: 64 files in `VendorWebAPI/EDDY.IS.Vendor.Entities/`
