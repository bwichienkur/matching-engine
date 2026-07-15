using EDDY.IS.FormsEngine.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces.Repositories
{
    public interface IHtmlRenderingStrategyRepository
    {
        HtmlRenderingStrategy GetHtmlRenderingStrategy(string renderingStrategyName);
    }
}
