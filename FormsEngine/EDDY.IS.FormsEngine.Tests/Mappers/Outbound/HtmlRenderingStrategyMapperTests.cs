using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Core.DTO;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Outbound.Tests
{
    public class HtmlRenderingStrategyMapperTests
    {
        private HTMLRenderingStrategyDTO _htmlRenderingStrategyDTO
        {
            get
            {
                return new HTMLRenderingStrategyDTO
                {
                    HTMLRenderingStrategyId = 1,
                    Name = "Test Rendering Strategy",
                    CSSPath = "Test/CSS/Path",
                    FormTemplateView = "FormTemplateView",
                    ManagedChoiceTemplateView = "ManagedChoiceTemplateView",
                    NoMatchTemplateView = "NoMatchTemplateView",
                    ThankYouTemplateView = "ThankYouTemplateView",
                    NoThankYouTemplateView = "NoThankYouTemplateView",
                    CrossSellTemplateView = "CrossSellTemplateView"
                };
            }
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_NotNull()
        {
            var mapper = new HtmlRenderingStrategyMapper();

            HtmlRenderingStrategy htmlRenderingStrategy = mapper.MapHtmlRenderingStrategy(null);

            Assert.NotNull(htmlRenderingStrategy);
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_HtmlRenderingStrategyIdIsMapped()
        {
            HtmlRenderingStrategy htmlRenderingStrategy = MapHtmlRenderingStrategy();
            Assert.Equal(_htmlRenderingStrategyDTO.HTMLRenderingStrategyId, htmlRenderingStrategy.HtmlRenderingStrategyId);
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_NameIsMapped()
        {
            HtmlRenderingStrategy htmlRenderingStrategy = MapHtmlRenderingStrategy();
            Assert.Equal(_htmlRenderingStrategyDTO.Name, htmlRenderingStrategy.Name);
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_CSSPathIsMapped()
        {
            HtmlRenderingStrategy htmlRenderingStrategy = MapHtmlRenderingStrategy();
            Assert.Equal(_htmlRenderingStrategyDTO.CSSPath, htmlRenderingStrategy.CSSPath);
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_FormTemplateViewIsMapped()
        {
            HtmlRenderingStrategy htmlRenderingStrategy = MapHtmlRenderingStrategy();
            Assert.Equal(_htmlRenderingStrategyDTO.FormTemplateView, htmlRenderingStrategy.FormTemplateView);
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_ManagedChoiceTemplateViewIsMapped()
        {
            HtmlRenderingStrategy htmlRenderingStrategy = MapHtmlRenderingStrategy();
            Assert.Equal(_htmlRenderingStrategyDTO.ManagedChoiceTemplateView, htmlRenderingStrategy.ManagedChoiceTemplateView);
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_NoMatchTemplateViewIsMapped()
        {
            HtmlRenderingStrategy htmlRenderingStrategy = MapHtmlRenderingStrategy();
            Assert.Equal(_htmlRenderingStrategyDTO.NoMatchTemplateView, htmlRenderingStrategy.NoMatchTemplateView);
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_ThankYouTemplateViewIsMapped()
        {
            HtmlRenderingStrategy htmlRenderingStrategy = MapHtmlRenderingStrategy();
            Assert.Equal(_htmlRenderingStrategyDTO.ThankYouTemplateView, htmlRenderingStrategy.ThankYouTemplateView);
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_NoThankYouTemplateViewIsMapped()
        {
            HtmlRenderingStrategy htmlRenderingStrategy = MapHtmlRenderingStrategy();
            Assert.Equal(_htmlRenderingStrategyDTO.NoThankYouTemplateView, htmlRenderingStrategy.NoThankYouTemplateView);
        }

        [Fact]
        public void MapHtmlRenderingStrategyTest_CrossSellTemplateViewIsMapped()
        {
            HtmlRenderingStrategy htmlRenderingStrategy = MapHtmlRenderingStrategy();
            Assert.Equal(_htmlRenderingStrategyDTO.CrossSellTemplateView, htmlRenderingStrategy.CrossSellTemplateView);
        }

        private HtmlRenderingStrategy MapHtmlRenderingStrategy()
        {
            var mapper = new HtmlRenderingStrategyMapper();
            return mapper.MapHtmlRenderingStrategy(_htmlRenderingStrategyDTO);
        }
    }
}