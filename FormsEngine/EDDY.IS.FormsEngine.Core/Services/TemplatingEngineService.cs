using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.Interfaces;
using RazorEngine;
using RazorEngine.Templating;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class TemplatingEngineService : ITemplatingEngineService
    {
        public void CompileTemplate(string template, string templateKey)
        {
            Engine.Razor.Compile(template, templateKey);
        }

        public string RenderTemplate(string templateKey, object model)
        {
            return Engine.Razor.Run(templateKey, null, model);
        }
    }
}
