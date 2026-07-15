# FormsEngine — EDDY.IS.FormsEngine.Services

**Path:** `FormsEngine/EDDY.IS.FormsEngine.Services/`  
**Type:** ASP.NET MVC 5 + Web API 2 Web Application (.NET Framework 4.5.1)  
**Role:** HTTP host for all FormsEngine endpoints

## Purpose

Serves embedded education inquiry forms to partner websites via JSONP/MVC endpoints. Hosts Razor templates, JavaScript/CSS bundles, and bootstraps SimpleInjector DI for newer School Picker flows.

## Responsibilities

- HTTP request routing (`{controller}/{action}`)
- Controller orchestration (17 controller classes)
- Razor template rendering (`Templates/` folder)
- Static asset bundling (wizard JS, QDF JS/CSS)
- Application startup: DI, cache warmup, component pre-compilation
- Classic WCF endpoint: `Classic/FormsEngineAPI.svc`

## Dependencies

| Reference | Purpose |
|-----------|---------|
| EDDY.IS.FormsEngine.Core | DI services, interfaces, models |
| EDDY.IS.FormsEngine.Infastructure | Repository implementations |
| EDDY.IS.FormsEngine.RF | Legacy FormsEngine facade |
| EDDY.IS.LeadEngine | Lead persistence |

## Important Classes

| Class | File | Role |
|-------|------|------|
| `MvcApplication` | `Global.asax.cs` | Application startup, DI |
| `TemplateManagerController` | `Controllers/TemplateManagerController.cs` | Primary form submission surface |
| `TemplateManagerControllerBase` | `Controllers/Base/TemplateManagerControllerBase.cs` | Core submission logic (~1800 lines) |
| `SessionController` | `Controllers/SessionController.cs` | FESession management |
| `SubmissionController` | `Controllers/SubmissionController.cs` | School Picker DI submission |
| `StaticController` | `Controllers/StaticController.cs` | JS/CSS delivery |
| `FormsEngineAPI` | `Classic/FormsEngineAPI.svc.cs` | WCF API for VendorWebAPI |

## Configuration

- `Web.config` — connection strings, WCF clients, appSettings, Redis, feature flags
- `App_Start/WebApiConfig.cs`, `RouteConfig.cs`, `FilterConfig.cs`

## External Services

All WCF clients configured in `Web.config` `system.serviceModel/client` (MatchingEngine, Prospect, Validation, LeadScoring, Pixels, Five9, LeadPing, Targus).

## NuGet Packages (Key)

SimpleInjector 4.6, Microsoft.AspNet.Mvc 5.3, EntityFramework 6.2, StackExchange.Redis 2.5, RazorEngine (via Core), Newtonsoft.Json 13.0.1, EDDY.IS.Validation 2.0.0

## Potential Improvements

1. Migrate to ASP.NET Core with proper REST API (eliminate JSONP/GET mutations)
2. Register all controllers in DI container
3. Split `TemplateManagerControllerBase` into application services
4. Add authentication middleware for admin/cache endpoints
5. Replace `Task.Run` with `IHostedService` or queue-based processing
