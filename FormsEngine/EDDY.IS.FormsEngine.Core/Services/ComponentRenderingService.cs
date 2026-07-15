using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class ComponentRenderingService : IComponentRenderingService
    {
        private readonly ITemplatingEngineService _templatingEngineService;

        public ComponentRenderingService(ITemplatingEngineService templatingEngineService)
        {
            _templatingEngineService = templatingEngineService;
        }
        
        public List<string> RenderComponents(string templateKey, IEnumerable<object> models)
        {
            List<string> renderedComponents = new List<string>();

            foreach (var model in models)
            {
                string renderedComponent = _templatingEngineService.RenderTemplate(templateKey, model);
                renderedComponents.Add(renderedComponent);
            }

            return renderedComponents;
        }
    }
}
