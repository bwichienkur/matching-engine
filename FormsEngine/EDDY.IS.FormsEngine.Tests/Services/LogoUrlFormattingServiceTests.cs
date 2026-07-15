using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;
using Xunit.Extensions;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class LogoUrlFormattingServiceTests
    {
        const string _smallImageSize = "120x40.png";
        const string _mediumImageSize = "150x50.gif";
        const string _largeImageSize = "240x80.gif";

        [Theory, MemberData(nameof(FormattedUrlTestData), _smallImageSize)]
        public void GetSmallLogoFormattedUrlTest(string unformattedPath, string expectedResult, string domain, string fileName, string fileSize)
        {
            Mock<IConfigurationService> configServiceMock = GetConfigurationServiceMock(domain, fileName);
            configServiceMock.Setup(m => m.EddyLogoImageSizeSmall).Returns(fileSize);

            var formatter = new LogoUrlFormattingService(configServiceMock.Object);
            string actualResult = formatter.GetSmallLogoFormattedUrl(unformattedPath);

            Assert.Equal(expectedResult, actualResult);
        }

        [Theory, MemberData(nameof(FormattedUrlTestData), _mediumImageSize)]
        public void GetMediumLogoFormattedUrlTest(string unformattedPath, string expectedResult, string domain, string fileName, string fileSize)
        {
            Mock<IConfigurationService> configServiceMock = GetConfigurationServiceMock(domain, fileName);
            configServiceMock.Setup(m => m.EddyLogoImageSizeMedium).Returns(fileSize);

            var formatter = new LogoUrlFormattingService(configServiceMock.Object);
            string actualResult = formatter.GetMediumLogoFormattedUrl(unformattedPath);

            Assert.Equal(expectedResult, actualResult);
        }

        [Theory, MemberData(nameof(FormattedUrlTestData), _largeImageSize)]
        public void GetLargeLogoFormattedUrlTest(string unformattedPath, string expectedResult, string domain, string fileName, string fileSize)
        {
            Mock<IConfigurationService> configServiceMock = GetConfigurationServiceMock(domain, fileName);
            configServiceMock.Setup(m => m.EddyLogoImageSizeLarge).Returns(fileSize);

            var formatter = new LogoUrlFormattingService(configServiceMock.Object);
            string actualResult = formatter.GetLargeLogoFormattedUrl(unformattedPath);

            Assert.Equal(expectedResult, actualResult);
        }

        private Mock<IConfigurationService> GetConfigurationServiceMock(string domain, string fileName)
        {
            var configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(m => m.EddyLogoImagePathDomain).Returns(domain);
            configServiceMock.Setup(m => m.EddyLogoImageFileName).Returns(fileName);
            return configServiceMock;
        }

        public static IEnumerable<object[]> FormattedUrlTestData(string fileSize)
        {
            return new[]
            {
                new object[] { "/8324/{FILENAME}?1531394702", $"https://logo.educationdynamics.com/8324/Logo_{fileSize}?1531394702", "https://logo.educationdynamics.com", "Logo_{0}", fileSize },
                new object[] { "/8324/?1531394702", "", "https://logo.educationdynamics.com", "Logo_{0}", fileSize },
                new object[] { "", "", "https://logo.educationdynamics.com", "Logo_{0}", fileSize },
                new object[] { "   ", "", "https://logo.educationdynamics.com", "Logo_{0}", fileSize },
                new object[] { null, "", "https://logo.educationdynamics.com", "Logo_{0}", fileSize },
                new object[] { "/8324/{FILENAME}?1531394702", "", "", "", "" },
                new object[] { "/8324/?1531394702", "", "", "", "" },
                new object[] { "", "", "", "", "" },
                new object[] { "   ", "", "", "", "" },
                new object[] { null, "", "", "", "" },
                new object[] { "/8324/{FILENAME}?1531394702", "", "", "Logo_{0}", fileSize },
                new object[] { "/8324/?1531394702", "", "", "Logo_{0}", fileSize },
                new object[] { "", "", "", "Logo_{0}", fileSize },
                new object[] { "   ", "", "", "Logo_{0}", fileSize },
                new object[] { null, "", "", "Logo_{0}", fileSize },
                new object[] { "/8324/{FILENAME}?1531394702", "", "https://logo.educationdynamics.com", "", fileSize },
                new object[] { "/8324/?1531394702", "", "https://logo.educationdynamics.com", "", fileSize },
                new object[] { "", "", "https://logo.educationdynamics.com", "", fileSize },
                new object[] { "   ", "", "https://logo.educationdynamics.com", "", fileSize },
                new object[] { null, "", "https://logo.educationdynamics.com", "", fileSize },
            };
        }
    }
}