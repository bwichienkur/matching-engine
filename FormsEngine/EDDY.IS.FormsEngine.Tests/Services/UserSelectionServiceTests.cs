using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.Models;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Interfaces;
using FluentAssertions;
using EDDY.IS.FormsEngine.Tests.Mocks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.DTO.Responses;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class MatchServiceTests
    {
        private readonly Guid _mockMatchResponseGuid = new Guid("641d2ee4-8d3e-4c04-8d60-7e651eb72f77");

        [Fact]
        public void GetUserSelectionsForSchoolPickerTest_NotNull()
        {
            var formInput = new FormInput();

            UserSelectionResponse userSelectionResponse = GetUserSelections(formInput, new UserSelectionResponse());

            Assert.NotNull(userSelectionResponse);
        }

        [Fact]
        public void GetUserSelectionsForSchoolPickerTest()
        {
            var formInput = new FormInput();

            var actualResult = GetUserSelections(formInput, GetMockRepositoryResponse());

            actualResult.Should().BeEquivalentTo(GetExpectedResult());
        }

        private UserSelectionResponse GetUserSelections(FormInput formInput, UserSelectionResponse mockRepositoryResponse)
        {
            var mockUserSelectionRepository = new Mock<IUserSelectionRepository>();
            mockUserSelectionRepository.Setup(r => r.GetUserSelectionsForSchoolPicker(It.IsAny<FormInput>(), It.IsAny<IEnumerable<int>>())).Returns(mockRepositoryResponse);

            var configServiceMock = MockHelper.GetConfigurationServiceMock();
            var logoFormatterService = new LogoUrlFormattingService(configServiceMock.Object);

            var userSelectionService = new UserSelectionService(mockUserSelectionRepository.Object, logoFormatterService);
            return userSelectionService.GetUserSelectionsForSchoolPicker(formInput);
        }

        private List<Campus> GetCampuses()
        {
            return new List<Campus>
            {
                new Campus
                {
                    InstitutionName = "QA National",
                    InstitutionDescription = "QA National Test Description",
                    InstitutionId = 8324,
                    CampusId = 83240,
                    CampusName = "QA National Main",
                    CampusLogoUrl = "/83240/{FILENAME}?1531394702",
                    InstitutionLogoUrl = "/8324/{FILENAME}?1531394702",
                    ProgramRankScore = 99.9m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 1,
                            ProgramName = "Master Program",
                            ProgramProductId = 123,
                            ProgramTemplateId = 400,
                            ProgramRankScore = 98.9m,
                        },
                        new Program
                        {
                            ProgramId = 45,
                            ProgramName = "Doctor Program",
                            ProgramProductId = 1456,
                            ProgramTemplateId = 23,
                            ProgramRankScore = 98.0m,
                        }
                    }
                },
                new Campus
                {
                    InstitutionName = "Emerson College",
                    InstitutionDescription = "Emerson College Test Description",
                    InstitutionId = 1571,
                    CampusId = 15710,
                    CampusName = "Emerson College Main",
                    CampusLogoUrl = "/15710/{FILENAME}?1424796370",
                    InstitutionLogoUrl = "/1571/{FILENAME}?1424796370",
                    ProgramRankScore = 95.5m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 3,
                            ProgramName = "Bachelor Program",
                            ProgramProductId = 898,
                            ProgramTemplateId = 2,
                            ProgramRankScore = 95.5m,
                        }
                    }
                }
            };
        }


        private UserSelectionResponse GetMockRepositoryResponse()
        {
            return new UserSelectionResponse
            {
                UserSelections = GetCampusesWithLogoUrls("/{0}/{{FILENAME}}?1424796370"),
                MatchResponseGuid = _mockMatchResponseGuid
            };
        }

        private UserSelectionResponse GetExpectedResult()
        {
            return new UserSelectionResponse
            {
                UserSelections = GetCampusesWithLogoUrls("https://logo.educationdynamics.com/{0}/Logo_240x80.gif?1424796370"),
                MatchResponseGuid = _mockMatchResponseGuid
            };
        }

        private List<Campus> GetCampusesWithLogoUrls(string urlFormat)
        {
            List<Campus> campuses = GetCampuses();

            foreach (var campus in campuses)
            {
                campus.CampusLogoUrl = string.Format(urlFormat, campus.CampusId);
                campus.InstitutionLogoUrl = string.Format(urlFormat, campus.InstitutionId);
            }

            return campuses;
        }

    }
}