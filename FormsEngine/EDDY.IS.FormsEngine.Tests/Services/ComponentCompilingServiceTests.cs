using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class ComponentCompilingServiceTests
    {
        [Theory]
        [MemberData(nameof(GetTestParams))]
        public void CompileAllComponentsTest_TemplatePassedToCompileTemplate(string baseAppDirectory, string mockTemplate)
        {
            var componentCompilingService = GetComponentCompilingService(mockTemplate, out Mock<ITemplatingEngineService> mockTemplatingEngineService, out _, out _);

            componentCompilingService.CompileAllComponents(baseAppDirectory);

            mockTemplatingEngineService.Verify(s => s.CompileTemplate(It.Is<string>(t => t.Equals(mockTemplate)), It.IsAny<string>()));
        }

        [Theory]
        [MemberData(nameof(GetTestParams))]
        public void CompileAllComponentsTest_TemplateKeyPassedToCompileTemplate(string baseAppDirectory, string mockTemplate)
        {
            var componentCompilingService = GetComponentCompilingService(mockTemplate, out Mock<ITemplatingEngineService> mockTemplatingEngineService, out _, out Dictionary<string, string> mockComponentTemplates);

            componentCompilingService.CompileAllComponents(baseAppDirectory);

            mockTemplatingEngineService.Verify(s => s.CompileTemplate(It.IsAny<string>(), It.Is<string>(k => mockComponentTemplates.ContainsKey(k))));
        }

        [Theory]
        [MemberData(nameof(GetTestParams))]
        public void CompileAllComponentsTest_CompileTemplatesMethodIsCalledForHoweverManyComponentsAreReturnedFromTheComponentTemplateService(string baseAppDirectory, string mockTemplate)
        {
            var componentCompilingService = GetComponentCompilingService(mockTemplate, out Mock<ITemplatingEngineService> mockTemplatingEngineService, out _, out Dictionary<string, string> mockComponentTemplates);

            componentCompilingService.CompileAllComponents(baseAppDirectory);

            mockTemplatingEngineService.Verify(s => s.CompileTemplate(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(mockComponentTemplates.Count));
        }

        [Theory]
        [MemberData(nameof(GetTestParams))]
        public void CompileAllComponentsTest_PathPassedToReadAllTextFromFileStartsWithBaseDirectoryPath(string baseAppDirectory, string mockTemplate)
        {
            var componentCompilingService = GetComponentCompilingService(mockTemplate, out _, out Mock<IFileReaderService> mockFileReaderService, out _);

            componentCompilingService.CompileAllComponents(baseAppDirectory);

            mockFileReaderService.Verify(s => s.ReadAllTextFromFile(It.Is<string>(p => p.StartsWith(baseAppDirectory))));
        }

        [Theory]
        [MemberData(nameof(GetTestParams))]
        public void CompileAllComponentsTest_ReadAllTextFromFileMethodIsCalledForHoweverManyComponentsAreReturnedFromTheComponentTemplateService(string baseAppDirectory, string mockTemplate)
        {
            var componentCompilingService = GetComponentCompilingService(mockTemplate, out _, out Mock<IFileReaderService> mockFileReaderService, out Dictionary<string, string> mockComponentTemplates);

            componentCompilingService.CompileAllComponents(baseAppDirectory);

            mockFileReaderService.Verify(s => s.ReadAllTextFromFile(It.IsAny<string>()), Times.Exactly(mockComponentTemplates.Count));
        }

        public static IEnumerable<object[]> GetTestParams()
        {
            return new List<object[]>
            {
                new object[] { "~/TestBase", "<div>MockTemplate</div>" }
            };
        }

        private ComponentCompilingService GetComponentCompilingService(string mockTemplate, out Mock<ITemplatingEngineService> mockTemplatingEngineService, out Mock<IFileReaderService> mockFileReaderService, out Dictionary<string, string> mockComponentTemplates)
        {
            mockFileReaderService = GetMockFileReaderService(mockTemplate);
            mockTemplatingEngineService = new Mock<ITemplatingEngineService>();
            mockComponentTemplates = GetMockComponentTemplates();
            var mockComponentTemplateFilePathService = GetMockComponentTemplateService(mockComponentTemplates);
            return new ComponentCompilingService(mockFileReaderService.Object, mockComponentTemplateFilePathService.Object, mockTemplatingEngineService.Object);
        }

        private Dictionary<string, string> GetMockComponentTemplates()
        {
            string baseComponentDirectory = "/Templates/Common/Components/";

            return new Dictionary<string, string>
            {
                { "FailedMatchReplacement", $"{baseComponentDirectory}FailedMatchReplacement.cshtml" },
                { "SchoolPickerMatch", $"{baseComponentDirectory}SchoolPickerMatch.cshtml" }
            };
        }
        
        private Mock<IFileReaderService> GetMockFileReaderService(string mockTemplate)
        {
            var mockFileReaderService = new Mock<IFileReaderService>();
            mockFileReaderService.Setup(s => s.ReadAllTextFromFile(It.IsAny<string>())).Returns(mockTemplate);
            return mockFileReaderService;
        }

        private Mock<IComponentTemplateService> GetMockComponentTemplateService(Dictionary<string, string> mockComponentTemplates)
        {
            var mockComponentTemplateService = new Mock<IComponentTemplateService>();
            mockComponentTemplateService.Setup(s => s.GetAllComponentTemplates()).Returns(mockComponentTemplates);
            return mockComponentTemplateService;
        }
    }
}