# Solution Overview

This repository contains **three Visual Studio solutions** plus a **shared class library** (LeadEngine) and several **orphan/utility projects** not included in any solution.

---

## Solution Inventory

| Solution | Path | Projects in Solution | Deploy Target |
|----------|------|-------------------|---------------|
| FormsEngine | `FormsEngine/EDDY.IS.FormsEngine.sln` | 6 | `F:\inetpub\wwwroot\EDDY.IS.FormsEngine.Service` |
| MatchingEngine | `MatchingEngine/EDDY.IS.MatchingEngine.sln` | 6 | `F:\inetpub\wwwroot\EDDY.IS.MatchingEngine.Service` |
| VendorWebAPI | `VendorWebAPI/EDDY.IS.Vendor.Web.API.sln` | 6 | `F:\inetpub\wwwroot\EDDY.IS.Vendor.Web.API` |
| LeadEngine | *(no .sln)* | 1 | Embedded library |
| SQL CLR | `MatchingEngine/EDDY.IS.Core.SqlCLR/EDDY.IS.Core.SqlCLR.sln` | 1 | SQL Server UDFs |

---

## All Projects (Complete Inventory)

### FormsEngine Solution (6 projects)

| Project | Path | Type | In Solution |
|---------|------|------|-------------|
| EDDY.IS.FormsEngine.Services | `FormsEngine/EDDY.IS.FormsEngine.Services/` | ASP.NET MVC Web App | Yes |
| EDDY.IS.FormsEngine (RF) | `FormsEngine/EDDY.IS.FormsEngine.RF/` | Class Library | Yes |
| EDDY.IS.FormsEngine.Core | `FormsEngine/EDDY.IS.FormsEngine.Core/` | Class Library | Yes |
| EDDY.IS.FormsEngine.Infastructure | `FormsEngine/EDDY.IS.FormsEngine.Infastructure/` | Class Library | Yes |
| EDDY.IS.FormsEngine.Tests | `FormsEngine/EDDY.IS.FormsEngine.Tests/` | xUnit Tests | Yes |
| EDDY.IS.LeadEngine | `LeadEngine/EDDY.IS.LeadEngine-RF/` | Class Library (external ref) | Yes |

### FormsEngine Orphan Projects (not in solution)

| Project | Path | Status |
|---------|------|--------|
| EDDY.IS.FormsEngine.Business | `FormsEngine/EDDY.IS.FormsEngine.Business/` | Thin wrapper; likely abandoned |
| EDDY.IS.FormsEngine.Entities | `FormsEngine/EDDY.IS.FormsEngine.Entities/` | 2 request DTOs only |
| EDDY.IS.FormsEngine.Services.Tests | `FormsEngine/EDDY.IS.FormsEngine.Services.Tests/` | Legacy MSTest |
| EDDY.IS.FormsEngine.Services.Test | `FormsEngine/EDDY.IS.FormsEngine.Services.Test/` | Empty test shell |
| EDDY.IS.FormsEngine.CoreTests | `FormsEngine/EDDY.IS.FormsEngine.CoreTests/` | Single test file |
| EDDY.IS.ProspectResubmit | `FormsEngine/EDDY.IS.ProspectResubmit/` | Empty console stub |
| TestLead | `FormsEngine/TestLead/` | Manual harness |

### MatchingEngine Solution (6 projects)

| Project | Path | Type |
|---------|------|------|
| EDDY.IS.MatchingEngine.Service | `MatchingEngine/EDDY.IS.MatchingEngine.Service/` | WCF Web App |
| EDDY.IS.MatchingEngine | `MatchingEngine/EDDY.IS.MatchingEngine/` | Class Library |
| EDDY.IS.MatchingEngine.Logging | `MatchingEngine/EDDY.IS.MatchingEngine.Logging/` | Class Library |
| EDDY.IS.MatchingEngine.UnitTests | `MatchingEngine/EDDY.IS.MatchingEngine.UnitTests/` | Tests |
| EDDY.IS.SEOAllocation.Console | `MatchingEngine/EDDY.IS.SEOAllocation.Console/` | Console |
| EDDY.IS.MatchResponseReplay.Console | `MatchingEngine/EDDY.IS.MatchResponseReplay.Console/` | Console |

### MatchingEngine Orphan Projects

| Project | Path | Status |
|---------|------|--------|
| EDDY.IS.MatchingEngine.MongoDB | `MatchingEngine/EDDY.IS.MatchingEngine.MongoDB/` | Unreferenced experiment |
| EDDY.IS.Core.SqlCLR | `MatchingEngine/EDDY.IS.Core.SqlCLR/` | Separate SQL CLR solution |
| EDDY.IS.MatchingEngine.Test | Legacy MSTest | Not in sln |
| EDDY.IS.MatchingEngine.TestSplitSimulator | WinForms | Manual tool |
| PocketME | WinForms | Manual tool |
| ME_Troubleshoot | WinForms WCF client | Troubleshooting |
| EDDY.IS.SEOAllocation.UnitTests | MSTest | Not in sln |

### VendorWebAPI Solution (6 projects)

| Project | Path | Type |
|---------|------|------|
| EDDY.IS.Vendor.Web.API | `VendorWebAPI/EDDY.IS.Vendor.Web.API/` | Web API + MVC |
| EDDY.IS.Vendor.Business | `VendorWebAPI/EDDY.IS.Vendor.Business/` | Class Library |
| EDDY.IS.Vendor.DataAccess | `VendorWebAPI/EDDY.IS.Vendor.DataAccess/` | Class Library |
| EDDY.IS.Vendor.Entities | `VendorWebAPI/EDDY.IS.Vendor.Entities/` | Class Library |
| EDDY.IS.Vendor.Utilities | `VendorWebAPI/EDDY.IS.Vendor.Utilities/` | Class Library |
| EDDY.IS.Vendor.Web.API.Tests | `VendorWebAPI/EDDY.IS.Vendor.Web.API.Tests/` | MSTest |

---

## Project Reference Graph

See individual project docs in [Projects/](./Projects/) and [Diagrams/DependencyGraph.md](./Diagrams/DependencyGraph.md).

---

## Startup Sequence Summary

| Application | Entry Point | First Actions |
|-------------|-------------|---------------|
| FormsEngine | `Global.asax.cs` → `MvcApplication.Application_Start` | DI registration, cache warmup (async) |
| MatchingEngine | `MatchingService.svc` + `MatchingEngineStartup.AppInitialize` | Preload entire ME cache |
| VendorWebAPI | `Global.asax.cs` → `WebApiApplication.Application_Start` | Reference data cache warmup |
| SEO Allocation | `Program.Main` | Manual DI, `ProcessAllocation()` |
| LeadEngine | *(none)* | Loaded by host |

---

## Application Lifecycle

All three web applications are **IIS-hosted, request-scoped** with no graceful shutdown hooks beyond empty `Application_End` in FormsEngine. Background work uses fire-and-forget `Task.Run` / `ThreadPool.QueueUserWorkItem` without cancellation token support.

Configuration is loaded at process start from `Web.config` / `App.config` and is not hot-reloaded without app pool recycle.
