using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.Enums;
using EDDY.IS.FormsEngine.Core.Interfaces;
using Moq;
using EDDY.IS.FormsEngine.Tests.TestData;
using EDDY.IS.FormsEngine.Tests.Mocks;
using FluentAssertions;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class ThankYouPageServiceTests
    {
        private readonly MockProgramTestData _mockProgramTestData;

        public ThankYouPageServiceTests()
        {
            _mockProgramTestData = new MockProgramTestData();
        }

        [Fact]
        public void GetThankYouPageTest()
        {
            var formRequest = new FormRequest
            {
                FESessionId = "8E43C61A-7B96-481F-B6E3-6A71A7ABBF95",
                Theme = "default",
                RenderingStrategy = "SchoolPickerWizard"
            };

            var mockPrograms = _mockProgramTestData.GetMockPrograms();
            var programIdsFromSession = mockPrograms.Select(p => p.ProgramId).ToList();
            var leadIdsFromSession = new List<decimal> { 100, 200, 300, 400 };

            var mockSessionService = new Mock<ISessionService>();
            mockSessionService.Setup(s => s.GetUserFullName(It.Is<string>(i => i.Equals(formRequest.FESessionId)))).Returns("Test Tester");
            mockSessionService.Setup(s => s.GetProgramIdsFromLeads(It.Is<string>(i => i.Equals(formRequest.FESessionId)))).Returns(programIdsFromSession);
            mockSessionService.Setup(s => s.GetLeadIds(It.Is<string>(i => i.Equals(formRequest.FESessionId)))).Returns(leadIdsFromSession);

            var mockProgramService = new Mock<IProgramService>();
            mockProgramService.Setup(s => s.GetPrograms(It.Is<FormRequest>(r => r.Equals(formRequest)), It.Is<List<int>>(p => p.Equals(programIdsFromSession)), It.IsAny<bool>())).Returns(mockPrograms);

            var logoFormatterService = MockHelper.GetLogoUrlFormattingService();

            var thankYouPageService = new ThankYouPageService(mockSessionService.Object, mockProgramService.Object, logoFormatterService);

            var actualResult = thankYouPageService.GetThankYouPage(formRequest);

            var expectedResult = GetExpectedResult();
            actualResult.Should().BeEquivalentTo(expectedResult);
            actualResult.LineItems.Should().BeInAscendingOrder(l => l.CampusType);
        }


        private ThankYouPage GetExpectedResult()
        {
            return new ThankYouPage
            {
                UserFullName = "Test Tester",
                SubmissionsFailed = false,
                LeadList = "100,200,300,400",
                SchoolPickerMatchProgramIdList = new List<int> { 1, 2, 3, 4 },
                LineItems = new List<ThankYouPageLineItem>
                {

                    new ThankYouPageLineItem
                    {
                        LogoUrl = "https://logo.educationdynamics.com/300/Logo_120x40.png?1531394702",
                        ShouldShowLogo = true,
                        CampusType = CampusType.Online,
                        CampusName = "Third Test Institution",
                        CampusDescription = "The Third Test Institution",
                        ProgramName = "Third Test Program",
                        ProgramDescription = "The Third Test Program"
                    },
                    new ThankYouPageLineItem
                    {
                        LogoUrl = "https://logo.educationdynamics.com/4000/Logo_120x40.png?1531394702",
                        ShouldShowLogo = true,
                        CampusType = CampusType.Online,
                        CampusName = "Fourth Test Institution",
                        CampusDescription = "The Fourth Test Institution",
                        ProgramName = "Fourth Test Program",
                        ProgramDescription = "The Fourth Test Program"
                    },
                    new ThankYouPageLineItem
                    {
                        LogoUrl = "https://logo.educationdynamics.com/100/Logo_120x40.png?1531394702",
                        ShouldShowLogo = false,
                        CampusType = CampusType.Ground,
                        CampusName = "First Test Institution",
                        CampusDescription = "The First Test Institution",
                        ProgramName = "First Test Program",
                        ProgramDescription = "The First Test Program"
                    },
                    new ThankYouPageLineItem
                    {
                        LogoUrl = "https://logo.educationdynamics.com/2000/Logo_120x40.png?1531394702",
                        ShouldShowLogo = true,
                        CampusType = CampusType.Ground,
                        CampusName = "Second Test Institution",
                        CampusDescription = "The Second Test Institution",
                        ProgramName = "Second Test Program",
                        ProgramDescription = "The Second Test Program"
                    }
                }
            };
        }

    }
}