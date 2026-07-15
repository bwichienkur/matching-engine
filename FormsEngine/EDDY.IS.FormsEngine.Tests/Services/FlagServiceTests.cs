using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;
using System.Collections.Specialized;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class FlagServiceTests
    {
        [Theory]
        [InlineData("flagOne")]
        [InlineData("flagTwo")]
        [InlineData("flagThree")]
        [InlineData("flagFour")]
        [InlineData("flagFive")]
        [InlineData("flagSix")]
        [InlineData("flagSeven")]
        public void GetFlagTest_FlagsThatExist(string flagName)
        {
            NameValueCollection settings = GetTestFlagCollection();

            var configurationServiceMock = new Mock<IConfigurationService>();
            configurationServiceMock.Setup(m => m.GetSettingsSection(It.IsAny<string>())).Returns(settings);

            var flagService = new FlagService(configurationServiceMock.Object);
            bool flagEnabled = flagService.IsFlagEnabled(flagName);

            var expectedResults = GetExpectedResult();
            var expectedResult = expectedResults[flagName];

            Assert.Equal(expectedResult, flagEnabled);
        }

        [Theory]
        [InlineData("missingFlag", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void GetFlagTest_FlagsThatDontExist(string flagName, bool expectedResult)
        {
            NameValueCollection settings = GetTestFlagCollection();

            var configurationServiceMock = new Mock<IConfigurationService>();
            configurationServiceMock.Setup(m => m.GetSettingsSection(It.IsAny<string>())).Returns(settings);

            var flagService = new FlagService(configurationServiceMock.Object);
            bool flagEnabled = flagService.IsFlagEnabled(flagName);

            Assert.Equal(expectedResult, flagEnabled);
        }

        [Fact]
        public void GetAllFlagsTest_EmptyDictionary()
        {
            NameValueCollection settings = null;

            var configurationServiceMock = new Mock<IConfigurationService>();
            configurationServiceMock.Setup(m => m.GetSettingsSection(It.IsAny<string>())).Returns(settings);

            var flagService = new FlagService(configurationServiceMock.Object);
            var flags = flagService.GetAllFlags();

            int expectedCount = 0;
            Assert.Equal(expectedCount, flags.Count);
        }

        [Fact]
        public void GetAllFlagsTest_ExpectedNumberOfFlagsReturned()
        {
            Dictionary<string, bool> flags = GetAllFlags();

            var expectedValues = GetExpectedResult();

            Assert.Equal(expectedValues.Count, flags.Count);
        }

        [Fact]
        public void GetAllFlagsTest_FlagsParsed()
        {
            Dictionary<string, bool> flags = GetAllFlags();

            var expectedValues = GetExpectedResult();

            foreach (var expected in expectedValues)
            {
                var flagValue = flags[expected.Key];
                Assert.Equal(expected.Value, flagValue);
            }
        }

        private Dictionary<string, bool> GetAllFlags()
        {
            NameValueCollection settings = GetTestFlagCollection();

            var configurationServiceMock = new Mock<IConfigurationService>();
            configurationServiceMock.Setup(m => m.GetSettingsSection(It.IsAny<string>())).Returns(settings);

            var flagService = new FlagService(configurationServiceMock.Object);
            var flags = flagService.GetAllFlags();
            return flags;
        }

        private NameValueCollection GetTestFlagCollection()
        {
            NameValueCollection settings = new NameValueCollection
            {
                { "flagOne", "true" },
                { "flagTwo", "false" },
                { "flagThree", "false" },
                { "flagFour", "true" },
                { "flagFive", "True" },
                { "flagSix", "False" },
                { "flagSeven", "jfdsj" }
            };
            return settings;
        }

        private Dictionary<string, bool> GetExpectedResult()
        {
            var expectedValues = new Dictionary<string, bool>
            {
                { "flagOne", true },
                { "flagTwo", false },
                { "flagThree", false },
                { "flagFour", true },
                { "flagFive", true },
                { "flagSix", false },
                { "flagSeven", false }
            };

            return expectedValues;
        }

    }
}