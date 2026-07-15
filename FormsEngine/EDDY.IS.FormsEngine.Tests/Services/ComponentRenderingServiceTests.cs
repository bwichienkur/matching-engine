using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.Models;
using System.IO;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;
using Match = EDDY.IS.FormsEngine.Core.Models.Match;
using EDDY.IS.FormsEngine.Tests.TestData;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class ComponentRenderingServiceTests
    {
        private static readonly MockMatchesTestData _mockMatchesTestData = new MockMatchesTestData();
        private static readonly string _fakeKey = "fakeKey";

        [Theory]
        [MemberData(nameof(GetTestParams))]
        public void RenderListComponentTest(List<Match> matches)
        {
            var renderingService = GetComponentRenderingService(matches, out List<string> expectedResult, out _);

            var actualResult = renderingService.RenderComponents(_fakeKey, matches);

            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [MemberData(nameof(GetTestParams))]
        public void RenderListComponentTest_TemplateKeyPassedToRenderTemplateMethod(List<Match> matches)
        {
            var renderingService = GetComponentRenderingService(matches, out List<string> expectedResult, out Mock<ITemplatingEngineService> mockTemplatingEngineService);

            var actualResult = renderingService.RenderComponents(_fakeKey, matches);

            mockTemplatingEngineService.Verify(s => s.RenderTemplate(It.Is<string>(k => k.Equals(_fakeKey)), It.IsAny<Match>()));
        }

        [Theory]
        [MemberData(nameof(GetTestParams))]
        public void RenderListComponentTest_ModelsPassedToRenderTemplateMethod(List<Match> matches)
        {
            var renderingService = GetComponentRenderingService(matches, out List<string> expectedResult, out Mock<ITemplatingEngineService> mockTemplatingEngineService);

            var actualResult = renderingService.RenderComponents(_fakeKey, matches);

            mockTemplatingEngineService.Verify(s => s.RenderTemplate(It.IsAny<string>(), It.Is<Match>(m => matches.Contains(m))));
        }

        public static IEnumerable<object[]> GetTestParams()
        {
            var testParams = new List<object[]>();

            for (int i = 1; i <= 5; i++)
            {
                var matches = _mockMatchesTestData.GetMockMatches(i);
                testParams.Add(new[] { matches });
            }

            return testParams;
        }

        private ComponentRenderingService GetComponentRenderingService(List<Match> matches, out List<string> expectedResult, out Mock<ITemplatingEngineService> mockTemplatingEngineService)
        {
            expectedResult = GetRenderListComponentExpectedResult(matches);
            mockTemplatingEngineService = GetMockTemplatingEngineService(expectedResult);
            return new ComponentRenderingService(mockTemplatingEngineService.Object);
        }

        private Mock<ITemplatingEngineService> GetMockTemplatingEngineService(List<string> expectedResult)
        {
            Queue<string> mockResults = new Queue<string>(expectedResult);

            var mockTemplateEngineService = new Mock<ITemplatingEngineService>();
            mockTemplateEngineService.Setup(s => s.RenderTemplate(It.IsAny<string>(), It.IsAny<object>())).Returns(mockResults.Dequeue);

            return mockTemplateEngineService;
        }

        private List<string> GetRenderListComponentExpectedResult(IEnumerable<Match> matches)
        {
            return matches.Select(match => $"<div><h2>{match.InstitutionName}<h2><h4>{match.ProgramName}<h4></div>").ToList();
        }
    }
}