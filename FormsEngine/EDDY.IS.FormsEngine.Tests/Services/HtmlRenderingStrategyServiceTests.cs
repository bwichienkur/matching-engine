using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.DTO;
using FluentAssertions;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class HtmlRenderingStrategyServiceTests
    {
        private const string _renderingStrategyName = "TestRenderingStrategy";

        private HtmlRenderingStrategy _mockHtmlRenderingStrategy
        {
            get
            {
                return new HtmlRenderingStrategy
                {
                    HtmlRenderingStrategyId = 1,
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

        [Theory]
        [InlineData(null)]
        [InlineData(_renderingStrategyName)]
        public void GetHtmlRenderingStrategyTest_NotNull(string renderingStrategyName)
        {
            HtmlRenderingStrategy mockHtmlRenderingStrategyResponse = null;

            var htmlRenderingStrategy = GetHtmlRenderingStrategy(renderingStrategyName, mockHtmlRenderingStrategyResponse);

            Assert.NotNull(htmlRenderingStrategy);
        }

        [Fact]
        public void GetHtmlRenderingStrategyTest()
        {
            var htmlRenderingStrategy = GetHtmlRenderingStrategy(_renderingStrategyName, _mockHtmlRenderingStrategy);

            htmlRenderingStrategy.Should().BeEquivalentTo(_mockHtmlRenderingStrategy);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(_renderingStrategyName)]
        public void GetHtmlRenderingStrategyThankYouTemplateTest_NotNull(string renderingStrategyName)
        {
            HtmlRenderingStrategy mockHtmlRenderingStrategyResponse = null;

            var thankYouTemplate = GetHtmlRenderingStrategyThankYouTemplate(renderingStrategyName, mockHtmlRenderingStrategyResponse);

            Assert.NotNull(thankYouTemplate);
        }

        [Fact]
        public void GetHtmlRenderingStrategyThankYouTemplateTest()
        {
            var thankYouTemplate = GetHtmlRenderingStrategyThankYouTemplate(_renderingStrategyName, _mockHtmlRenderingStrategy);

            Assert.Equal(_mockHtmlRenderingStrategy.ThankYouTemplateView, thankYouTemplate);
        }

        private HtmlRenderingStrategy GetHtmlRenderingStrategy(string renderingStrategyName, HtmlRenderingStrategy mockHtmlRenderingStrategy)
        {
            var htmlRenderingStrategyService = GetHtmlRenderingStrategyService(renderingStrategyName, mockHtmlRenderingStrategy);
            return htmlRenderingStrategyService.GetHtmlRenderingStrategy(renderingStrategyName);
        }

        private string GetHtmlRenderingStrategyThankYouTemplate(string renderingStrategyName, HtmlRenderingStrategy mockHtmlRenderingStrategy)
        {
            var htmlRenderingStrategyService = GetHtmlRenderingStrategyService(renderingStrategyName, mockHtmlRenderingStrategy);
            return htmlRenderingStrategyService.GetHtmlRenderingStrategyThankYouTemplate(renderingStrategyName);
        }

        private HtmlRenderingStrategyService GetHtmlRenderingStrategyService(string renderingStrategyName, HtmlRenderingStrategy mockHtmlRenderingStrategy)
        {
            var mockHtmlRenderingStrategyRepository = new Mock<IHtmlRenderingStrategyRepository>();
            mockHtmlRenderingStrategyRepository.Setup(r => r.GetHtmlRenderingStrategy(It.Is<string>(n => n.Equals(renderingStrategyName)))).Returns(mockHtmlRenderingStrategy);
            return new HtmlRenderingStrategyService(mockHtmlRenderingStrategyRepository.Object);
        }

    }
}