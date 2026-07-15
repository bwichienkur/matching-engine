using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.Util.HTMLExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class ComponentsController : Controller
    {
        public string GetFailedMatchReplacementComponents(IEnumerable<Match> matches)
        {
            string components = string.Empty;

            foreach (var match in matches)
            {
                var component = this.RazorViewToString("~/Views/Components/FailedMatchReplacement.cshtml", match, false, false);
                components += component;
            }

            

            return components;
        }
    }
}