# Security Review

## Executive Summary

The platform has **significant security gaps** typical of legacy .NET Framework applications deployed behind a corporate network perimeter. VendorWebAPI has API key authentication; FormsEngine and MatchingEngine have **no application-level authentication**.

**Overall risk:** High for internet-exposed endpoints; Medium if properly network-segmented.

---

## Secrets and Credentials

| Finding | Location | Risk | Recommendation |
|---------|----------|------|----------------|
| EMS auth token in config | `EmsLeadEngineAuthToken` in Web.config | High | Move to Azure Key Vault / IIS environment variables |
| Google Places API key | FormsEngine `Web.config` appSettings | Medium | Restrict key to server IPs; use Key Vault |
| Redis connection string | VendorWebAPI `Web.config` | Medium | Use managed identity where possible |
| SQL connection strings | All Web.config files | Medium | Use Windows Auth (already SSPI); restrict DB permissions |
| WCF endpoints with `security mode="None"` | FormsEngine Web.config | High if exposed | Enable transport security or migrate to HTTPS + auth |

**No hardcoded passwords found in source code.** Connection strings use Integrated Security.

---

## Authentication Gaps

| Application | Auth Mechanism | Gap |
|-------------|---------------|-----|
| FormsEngine | None | All 80+ endpoints publicly accessible |
| MatchingEngine WCF | None | Full matching API exposed; metadata enabled |
| VendorWebAPI | API key (GUID) | Key in query string (logged in access logs) |
| LeadDetails MVC | **None** | Lead details viewable without auth |
| CacheHelper | None | Cache manipulation endpoint unprotected |

---

## Authorization Gaps

- No role-based access control in any application
- No resource-level authorization (e.g., partner A cannot access partner B's leads)
- `CampaignAuthorizationFilter` validates campaign status but not data scope
- Admin/diagnostic endpoints (`CacheInfo.aspx`, `CacheHelper/ResetTemplateCache`) unprotected

---

## Injection Risks

| Vector | Assessment | Evidence |
|--------|------------|----------|
| SQL Injection | Low-Medium | EF6 parameterized queries used; `DataHelper.GetDataReader` uses view names from code (not user input). SP parameters are typed. |
| XSS | Medium | FormsEngine renders user input in Razor templates; JSONP responses may reflect input. No explicit output encoding audit performed. |
| Command Injection | Low | No shell execution found |

---

## CSRF

| Application | Risk | Reason |
|-------------|------|--------|
| FormsEngine | **High** | State-changing operations via GET (ProcessSubmit, SaveProspect, etc.) |
| VendorWebAPI | Low | POST-only mutations with JSON body |
| MatchingEngine | N/A | Read-only matching (no mutations via API) |

---

## Input Validation

| Layer | Coverage |
|-------|----------|
| FormsEngine client validation | Email, phone, zip, profanity, birthdate |
| FormsEngine server validation | Configurable via `WebLeadsServerSideValidationEnabled` |
| VendorWebAPI | 25+ action filters (TCPA, campus, category, age, etc.) |
| MatchingEngine | Request DTOs validated in engine; no input sanitization layer |

**Gap:** No request size limiting beyond IIS `maxRequestLength`. No rate limiting on FormsEngine/MatchingEngine.

---

## PII Handling

| Data | Storage | Concern |
|------|---------|---------|
| Name, email, phone, address | `dbo.Lead`, `dbo.SubmissionDetail` | Standard PII; no encryption at rest in app layer |
| SSN/financial | Not observed in entities | — |
| TCPA consent text | Stored on lead/submission | Compliance requirement |
| Raw POST data | `dbo.RawPostData` | Full form payload stored; may contain sensitive fields |
| API logs | `EddyApiLog` | May contain PII from request bodies |

**Recommendation:** Implement PII field masking in logs; audit data retention policies.

---

## Rate Limiting

| Application | Rate Limiting |
|-------------|---------------|
| VendorWebAPI | Campaign-level rate limit in `CampaignAuthorizationFilter` (`APIRateExceeded`) |
| FormsEngine | None |
| MatchingEngine | None |

---

## Dependency Vulnerabilities

| Package | Version | Notes |
|---------|---------|-------|
| EntityFramework | 6.1.3 – 6.5.1 | Legacy; no critical CVEs in range but unsupported |
| Newtonsoft.Json | 13.0.1 – 13.0.4 | Current; monitor advisories |
| jQuery | 1.10.2 – 3.x | Older versions in some projects; known XSS CVEs in 1.x |
| SimpleInjector | 4.6.0 | Stable |
| StackExchange.Redis | 1.2.6 – 2.5.61 | Wide version range across projects |

**Recommendation:** Run `dotnet list package --vulnerable` after migration to SDK-style projects.

---

## Open Redirects

No explicit open redirect vulnerabilities identified. Thank-you page redirects use configured rendering strategies from database.

**Confidence:** Medium — not all redirect paths were audited.

---

## SSRF

| Vector | Assessment |
|--------|------------|
| `ExternalMatchServiceClient` | URL from config (not user input) — Low risk |
| EMS Lead Engine HTTP calls | URL from config — Low risk |
| `LocationController.GetPostalCode` | IP override parameter — **Medium risk** if attacker can control `ipOverride` |

---

## Privilege Escalation

- No user role system exists
- API key grants access to all campaign data for that key
- No mechanism to escalate beyond campaign scope (no admin API found)

---

## Recommendations (Prioritized)

1. **Add authentication to FormsEngine admin/cache endpoints** (High)
2. **Migrate GET mutations to POST** in FormsEngine (High)
3. **Move secrets to Key Vault** (High)
4. **Add auth to LeadDetails MVC controller** (High)
5. **Enable WCF transport security** or migrate off WCF (High)
6. **Add rate limiting to FormsEngine and MatchingEngine** (Medium)
7. **Implement correlation IDs and PII masking in logs** (Medium)
8. **Disable WCF metadata in production** (Medium)
9. **Upgrade jQuery in legacy projects** (Low)
10. **Add CORS restrictions** (replace `origins: "*"` in VendorWebAPI) (Medium)
