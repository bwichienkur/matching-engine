using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class ComponentTemplateServiceTests
    {

        [Fact]
        public void SchoolPickerMatchComponentTemplatePathTest()
        {
            var componentTemplateService = GetComponentTemplateService();

            var schoolPickerMatchComponentFilePath = componentTemplateService.SchoolPickerMatchComponentTemplatePath;

            Assert.Equal("~/Templates/Common/Components/SchoolPickerMatch.cshtml", schoolPickerMatchComponentFilePath);
        }

        [Fact]
        public void FailedMatchReplacementComponentTemplatePathTest()
        {
            var componentTemplateService = GetComponentTemplateService();

            var failedMatchReplacementComponentFilePath = componentTemplateService.FailedMatchReplacementComponentTemplatePath;

            Assert.Equal("~/Templates/Common/Components/FailedMatchReplacement.cshtml", failedMatchReplacementComponentFilePath);
        }

        [Fact]
        public void GetAllComponentTemplatePathsTest()
        {
            var expectedResult = GetAllComponentTemplatePathsExpectedResult();
            var componentTemplateService = GetComponentTemplateService();

            var allTemplateFilePaths = componentTemplateService.GetAllComponentTemplates();

            Assert.Equal(expectedResult, allTemplateFilePaths);
        }

        [Fact]
        public void FailedMatchReplacementComponentTemplateKeyTest()
        {
            string expectedResult = "FailedMatchReplacement";
            var componentTemplateService = GetComponentTemplateService();

            string actualResult = componentTemplateService.FailedMatchReplacementComponentTemplateKey;

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void SchoolPickerMatchComponentTemplateKeyTest()
        {
            string expectedResult = "SchoolPickerMatch";
            var componentTemplateService = GetComponentTemplateService();

            string actualResult = componentTemplateService.SchoolPickerMatchComponentTemplateKey;

            Assert.Equal(expectedResult, actualResult);
        }

        private ComponentTemplateService GetComponentTemplateService()
        {
            var mockComponentTemplateSettings = GetTestTemplateComponentSettingsCollection();

            var mockConfigurationService = new Mock<IConfigurationService>();
            mockConfigurationService.Setup(s => s.GetSettingsSection(It.IsAny<string>())).Returns(mockComponentTemplateSettings);

            return new ComponentTemplateService(mockConfigurationService.Object);
        }

        private Dictionary<string, string> GetAllComponentTemplatePathsExpectedResult()
        {
            var expectedResult = new Dictionary<string, string>();

            Dictionary<string, string> componentTemplateSettings = GetTestTemplateComponentSettingsDictionary();

            string componentDirectoryPathKey = "ComponentDirectory";
            if (componentTemplateSettings.TryGetValue(componentDirectoryPathKey, out string directory))
            {
                componentTemplateSettings.Remove(componentDirectoryPathKey);
                foreach (var componentTemplateSetting in componentTemplateSettings)
                {
                    expectedResult[componentTemplateSetting.Key] = directory + componentTemplateSetting.Value;
                }
            }

            return expectedResult;
        }

        private Dictionary<string, string> GetTestTemplateComponentSettingsDictionary()
        {
            var result = new Dictionary<string, string>();

            var componentTemplateSettings = GetTestTemplateComponentSettingsCollection();

            for (int i = 0; i < componentTemplateSettings?.Count; i++)
            {
                string templateKey = componentTemplateSettings.AllKeys[i];
                string templatePath = componentTemplateSettings[i];
                result[templateKey] = templatePath;
            }

            return result;
        }

        private NameValueCollection GetTestTemplateComponentSettingsCollection()
        {
            NameValueCollection settings = new NameValueCollection
            {
                { "ComponentDirectory", "~/Templates/Common/Components/" },
                { "FailedMatchReplacement", "FailedMatchReplacement.cshtml" },
                { "SchoolPickerMatch", "SchoolPickerMatch.cshtml" }
            };

            return settings;
        }
    }
}