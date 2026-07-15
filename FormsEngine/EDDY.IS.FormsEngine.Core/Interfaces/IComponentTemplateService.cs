using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface IComponentTemplateService
    {
        string SchoolPickerMatchComponentTemplateKey { get; }
        string FailedMatchReplacementComponentTemplateKey { get; }
        string SchoolPickerMatchComponentTemplatePath { get; }
        string FailedMatchReplacementComponentTemplatePath { get; }
        Dictionary<string, string> GetAllComponentTemplates();
    }
}
