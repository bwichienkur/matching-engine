using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Tests.Mocks
{
    public class MockHelper
    {
        public static Mock<IConfigurationService> GetConfigurationServiceMock()
        {
            var configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(m => m.EddyLogoImagePathDomain).Returns("https://logo.educationdynamics.com");
            configServiceMock.Setup(m => m.EddyLogoImageFileName).Returns("Logo_{0}");
            configServiceMock.Setup(m => m.EddyLogoImageSizeSmall).Returns("120x40.png");
            configServiceMock.Setup(m => m.EddyLogoImageSizeMedium).Returns("150x50.gif");
            configServiceMock.Setup(m => m.EddyLogoImageSizeLarge).Returns("240x80.gif");
            return configServiceMock;
        }

        public static ILogoUrlFormattingService GetLogoUrlFormattingService()
        {
            var configServiceMock = MockHelper.GetConfigurationServiceMock();
            return new LogoUrlFormattingService(configServiceMock.Object);
        }
    }
}
