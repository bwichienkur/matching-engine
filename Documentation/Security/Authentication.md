# Authentication & Authorization

## FormsEngine

**Authentication:** None  
**Authorization:** None

- No `[Authorize]` attributes on any controller
- No `<authentication>` element in `Web.config`
- IIS anonymous authentication enabled
- Designed as public-facing JSONP API for embedded partner forms

**Service-to-service auth:**
- EMS Lead Engine: static `EmsLeadEngineAuthToken` GUID in config
- WCF clients: `security mode="None"` (network trust)

---

## MatchingEngine

**Authentication:** None  
**Authorization:** None

- WCF service metadata publicly accessible (`httpGetEnabled="true"`)
- `includeExceptionDetailInFaults="true"` in config
- `crossdomain.xml` / `clientaccesspolicy.xml` for legacy cross-domain access
- Security relies on network perimeter and IIS Windows auth for SQL

---

## VendorWebAPI

### Primary Auth: Campaign API Key

**Implementation:** `CampaignAuthorizationFilter` (global filter)

| Aspect | Detail |
|--------|--------|
| Credential | `apikey` (GUID) |
| Location | Query string (GET) or JSON body (POST) |
| Validation | Parse GUID → lookup `VendorCampaign` in Nexus |
| Checks | Enabled, status (expired/pending/inactive/terminated), rate limit |
| Failure | `401 Unauthorized` with `VendorResponseBase` JSON |
| Logging | Failed attempts logged to `EddyLogging` |

### Secondary Auth: Action Filters

Campaign-type-specific filters enforce business rules (not identity):

| Filter | Rule |
|--------|------|
| `HostAndPostCampaignFilter` | Campaign must support host-and-post |
| `ProspectCampaignFilter` | Campaign must support prospects |
| `APIDirectoryCampaignFilter` | Campaign must support directory API |
| `TCPAFilter` | TCPA consent required |
| `LeadSourceFilter` | Lead source URL required |

### Unprotected Endpoints

| Endpoint | Risk |
|----------|------|
| `Util/checkapikey` | Intentionally public (key validation) |
| `LeadDetails/Index` | **No auth** — lead details HTML view |
| `LeadDetails/SaveNote` | **No auth** — note saving |
| Help Page `/` | Public API documentation |

### Not Used (despite packages)

- ASP.NET Identity (packages referenced, not configured)
- Owin middleware (`owin:AutomaticAppStartup = false`)
- JWT / Bearer tokens
- OAuth

---

## Permission Model

No role-based or claims-based permission model exists. Access control is:

1. **Campaign-scoped:** API key grants access to one campaign's inventory and submission capabilities
2. **Campaign-type-scoped:** Filters enforce what operations a campaign type supports
3. **Rate-limited:** Per-campaign API rate limits in `CampaignAuthorizationFilter`

There is no admin role, no multi-campaign access per key, and no user identity concept.

---

## Claims

Not used. No `ClaimsPrincipal`, no `ClaimsIdentity`, no custom claim types.

---

## Recommendations

1. Add API key auth to FormsEngine (at minimum for admin/cache endpoints)
2. Add auth to `LeadDetailsController`
3. Move API key from query string to `Authorization` header
4. Implement mutual TLS for WCF inter-service communication
5. Add per-partner scopes (directory-only, lead-submit-only)
