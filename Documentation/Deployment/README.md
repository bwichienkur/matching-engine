# Deployment Documentation

## Build Process

### CI/CD Pipelines

| Pipeline | Trigger | Scope |
|----------|---------|-------|
| `azure-pipelines.yaml` | `development`, `qa`, `uat`, `main` | Full CI/CD with deploy + rollback |
| `ci-pipelines.yaml` | `feature/*` PRs to main branches | CI build only (no deploy) |
| `MatchingEngine/build_ci_matching_engine.yaml` | `main`, paths `MatchingEngine/` | ME-specific CI on `windows-latest` |

### Build Configuration

| Setting | Value |
|---------|-------|
| Agent pool | `Back Office` (deploy) / `windows-latest` (ME CI) |
| OS | Windows_NT |
| Configuration | Release |
| NuGet config | `nuget.config` (root) / `VendorWebAPI/nuget.config` |
| Publish profile | `NonProd` |
| Solution paths | See table below |

### Solutions Built

| App | Solution | Output |
|-----|----------|--------|
| FormsEngine | `FormsEngine\EDDY.IS.FormsEngine.sln` | IIS web deploy package |
| MatchingEngine | `MatchingEngine\EDDY.IS.MatchingEngine.sln` | IIS web deploy package (`WebApp.zip`) |
| VendorWebAPI | `VendorWebAPI\EDDY.IS.Vendor.Web.API.sln` | IIS web deploy package |

### External Dependencies (CI)

- `ExternalMatch/EDDY.IS.ExternalMatch.sln` restored before MatchingEngine build
- Azure DevOps NuGet feed: `0463f636-c458-4488-987d-6723a95d4270`
- Pipeline templates: `Technology Management/AzurePipelines` repo, branch `V1.0`

### Unit Tests in CI

**MatchingEngine unit tests are commented out** in `build_ci_matching_engine.yaml` (`VSTest@3` disabled). FormsEngine and VendorWebAPI CI templates do not explicitly run tests in the YAML visible in-repo.

---

## Infrastructure

### Hosting Model

```
Windows Server
└── IIS
    ├── App Pool: EDDY.IS.FormsEngine.Service
    │   └── Site: F:\inetpub\wwwroot\EDDY.IS.FormsEngine.Service
    ├── App Pool: EDDY.IS.MatchingEngine.Service
    │   └── Site: F:\inetpub\wwwroot\EDDY.IS.MatchingEngine.Service
    └── App Pool: CheetahPool
        └── Site: F:\inetpub\wwwroot\EDDY.IS.Vendor.Web.API
```

### No Container Support

- No Dockerfile in repository
- No Kubernetes manifests
- No Azure App Service configuration

### Batch Jobs (External Scheduler)

| Console App | Purpose | Scheduling |
|-------------|---------|------------|
| `EDDY.IS.SEOAllocation.Console` | GradSchools SEO allocation | External Windows Task Scheduler (implied) |
| `EDDY.IS.MatchResponseReplay.Console` | Match response debugging | Manual execution |
| `EDDY.IS.ProspectResubmit` | Prospect resubmission | Not implemented (empty Main) |

---

## Environment Promotion

| Branch | Pipeline | Deploy |
|--------|----------|--------|
| `feature/*` | `ci-pipelines.yaml` | Build only |
| `development` | `azure-pipelines.yaml` | Deploy to NonProd |
| `qa` | `azure-pipelines.yaml` | Deploy to QA |
| `uat` | `azure-pipelines.yaml` | Deploy to UAT |
| `main` | `azure-pipelines.yaml` | Deploy to production (implied) |

### Rollback

Each app has a dedicated rollback stage in `azure-pipelines.yaml`:
- Uses `pipelines/templates/rollback.yaml@PipelineTemplates`
- Parameters: `app`, `appPool`, `serverDestPath`, `Revision`

---

## Configuration Differences by Environment

### Token Replacement (`Master.config`)

Environment-specific overrides via token replacement:

| Token | Example Values |
|-------|---------------|
| DB server | `ping-sql1` (dev), production server (prod) |
| LeadPing URL | `leadping.gpm.educationdynamics.local` vs production |
| Service endpoints | `tcpa-issvc1` vs `tag-ISSVC1` |

### VendorWebAPI Build Configurations

The solution supports 10 build configurations: `Debug`, `Release`, `DEMO`, `DEVSTAGE`, `MIKETESTENV`, `PRODUCTIONSUPORT`, `QA`, `QASTAGE`, `QASTAGE02`, `UAT`.

Each maps to potentially different `Web.config` transforms.

### FormsEngine Configuration

- `Web.config` + `Web.Debug.config` / `Web.Release.config`
- `appSettings`: Redis, session timeouts, validation levels, feature toggles
- `connectionStrings`: FEEntitiesContainer, Nexus_Validation_Entities, Nexus_Lead_Entities, EddyLoggingEntities

### MatchingEngine Configuration

- `Web.config` with 40+ `ME_*_ExpirationMinutes` cache TTL settings
- `IsBeta` flag suppresses match response logging
- `EntityFrameworkCommandTimeout=500`

---

## Deployment Checklist

1. Build solution in Release configuration
2. Run config transform for target environment (`Master.config` token replacement)
3. Deploy to IIS app pool path
4. Verify app pool starts and cache warmup completes
5. Smoke test: MatchingEngine `CacheInfo.aspx`, VendorWebAPI `Util/checkapikey`
6. Monitor `EddyLogging` for startup exceptions

---

## Missing Deployment Artifacts

- No infrastructure-as-code (Terraform, ARM, Bicep)
- No health check endpoints (except `CacheInfo.aspx`)
- No blue-green or canary deployment configuration
- No automated smoke tests post-deploy
