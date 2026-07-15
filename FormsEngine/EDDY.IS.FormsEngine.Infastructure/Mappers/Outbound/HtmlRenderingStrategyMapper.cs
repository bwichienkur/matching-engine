using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class HtmlRenderingStrategyMapper
    {
        public HtmlRenderingStrategy MapHtmlRenderingStrategy(HTMLRenderingStrategyDTO htmlRenderingStrategyDTO)
        {
            var htmlRenderingStrategy = new HtmlRenderingStrategy();

            if (htmlRenderingStrategyDTO != null)
            {
                htmlRenderingStrategy.HtmlRenderingStrategyId = htmlRenderingStrategyDTO.HTMLRenderingStrategyId;
                htmlRenderingStrategy.Name = htmlRenderingStrategyDTO.Name;
                htmlRenderingStrategy.CSSPath = htmlRenderingStrategyDTO.CSSPath;
                htmlRenderingStrategy.FormTemplateView = htmlRenderingStrategyDTO.FormTemplateView;
                htmlRenderingStrategy.ManagedChoiceTemplateView = htmlRenderingStrategyDTO.ManagedChoiceTemplateView;
                htmlRenderingStrategy.NoMatchTemplateView = htmlRenderingStrategyDTO.NoMatchTemplateView;
                htmlRenderingStrategy.ThankYouTemplateView = htmlRenderingStrategyDTO.ThankYouTemplateView;
                htmlRenderingStrategy.NoThankYouTemplateView = htmlRenderingStrategyDTO.NoThankYouTemplateView;
                htmlRenderingStrategy.CrossSellTemplateView = htmlRenderingStrategyDTO.CrossSellTemplateView;
            }

            return htmlRenderingStrategy;
        }
    }
}
