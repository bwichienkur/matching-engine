using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDDY.IS.FormsEngine.Services.Models;

namespace EDDY.IS.FormsEngine.Services.Caching
{
    [Serializable]
    public class TemplateCache
    {
        public string RenderedTemplate { get; set; }
        public FormEngineModel TemplateModel { get; set; }
    }
}