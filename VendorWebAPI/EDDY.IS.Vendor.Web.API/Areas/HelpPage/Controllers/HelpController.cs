using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using EDDY.IS.Vendor.Web.API.Areas.HelpPage.ModelDescriptions;
using EDDY.IS.Vendor.Web.API.Areas.HelpPage.Models;
using EDDY.IS.Vendor.Web.API.Controllers;

namespace EDDY.IS.Vendor.Web.API.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";

        public HelpController()
            : this(GlobalConfiguration.Configuration)
        {
        }

        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        public HttpConfiguration Configuration { get; private set; }

        public ActionResult Index()
        {
            var displayOrder = new Dictionary<Type, int>();
            displayOrder.Add(typeof(DirectoryController), 1);
            displayOrder.Add(typeof(ProgramController), 2);
            displayOrder.Add(typeof(LeadController), 3);
            displayOrder.Add(typeof(ProspectController), 4);
            displayOrder.Add(typeof(MarketingController), 5);
            displayOrder.Add(typeof(InstitutionsController), 6);
            ViewBag.DisplayOrder = displayOrder;
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }

        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}