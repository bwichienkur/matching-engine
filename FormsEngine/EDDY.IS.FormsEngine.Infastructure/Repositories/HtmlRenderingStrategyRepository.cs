using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class HtmlRenderingStrategyRepository : IHtmlRenderingStrategyRepository
    {
        public HtmlRenderingStrategy GetHtmlRenderingStrategy(string renderingStrategyName)
        {
            var formsEngine = new FormsEngine();
            var mapper = new HtmlRenderingStrategyMapper();

            var renderingStrategyDTO = formsEngine.GetHTMLRenderingStrategy(renderingStrategyName);
            var renderingStrategy = mapper.MapHtmlRenderingStrategy(renderingStrategyDTO);

            return renderingStrategy;
        }
    }
}
