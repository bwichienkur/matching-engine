using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class HtmlRenderingStrategyService : IHtmlRenderingStrategyService
    {
        private readonly IHtmlRenderingStrategyRepository _htmlRenderingStrategyRepository;

        public HtmlRenderingStrategyService(IHtmlRenderingStrategyRepository htmlRenderingStrategyRepository)
        {
            _htmlRenderingStrategyRepository = htmlRenderingStrategyRepository;
        }

        public string GetHtmlRenderingStrategyThankYouTemplate(string renderingStrategyName)
        {
            string thankYouTemplateView = null;

            if (!string.IsNullOrWhiteSpace(renderingStrategyName))
            {
                var renderingStrategy = GetHtmlRenderingStrategy(renderingStrategyName);
                thankYouTemplateView = renderingStrategy?.ThankYouTemplateView;
            }

            return thankYouTemplateView ?? string.Empty;
        }

        public HtmlRenderingStrategy GetHtmlRenderingStrategy(string renderingStrategyName)
        {
            HtmlRenderingStrategy renderingStrategy = null;

            if (!string.IsNullOrWhiteSpace(renderingStrategyName))
            {
                renderingStrategy = _htmlRenderingStrategyRepository.GetHtmlRenderingStrategy(renderingStrategyName);
            }

            return renderingStrategy ?? new HtmlRenderingStrategy();
        }
    }
}
