using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface ITemplatingEngineService
    {
        void CompileTemplate(string template, string templateKey);

        string RenderTemplate(string templateKey, object model);
    }
}
