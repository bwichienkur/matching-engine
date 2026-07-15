using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Tests.TestData;
using FluentAssertions;
using Match = EDDY.IS.FormsEngine.Core.Models.Match;
using EDDY.IS.FormsEngine.Core.DTO.Responses;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class FailedMatchReplacementServiceTests
    {
        private readonly MockMatchesTestData _mockMatchesTestData;
        private readonly FailedMatchReplacementServiceTestData _testDataHelper;

        public FailedMatchReplacementServiceTests()
        {
            _mockMatchesTestData = new MockMatchesTestData();
            _testDataHelper = new FailedMatchReplacementServiceTestData();
        }

        [Fact]
        public void GetReplacementsForFailedMatchesTest_FormInputNull()
        {
            FormInput formInput = null;
            var failedProgramIds = new List<int>();

            FailedMatchReplacements failedMatchReplacements = GetReplacementsForFailedMatches(formInput, failedProgramIds, GetMockUserSelectionResponse());

            Assert.NotNull(failedMatchReplacements);
            Assert.True(failedMatchReplacements.ReplacementMatches.Count() == 0);
            Assert.True(failedMatchReplacements.FailedMatches.Count() == 0);
        }

        [Fact]
        public void GetReplacementsForFailedMatchesTest_MatchesNull()
        {
            FormInput formInput = new FormInput();
            formInput.Matches = null;

            var failedProgramIds = new List<int>();

            FailedMatchReplacements failedMatchReplacements = GetReplacementsForFailedMatches(formInput, failedProgramIds, GetMockUserSelectionResponse());

            Assert.NotNull(failedMatchReplacements);
            Assert.True(failedMatchReplacements.ReplacementMatches.Count() == 0);
            Assert.True(failedMatchReplacements.FailedMatches.Count() == 0);
        }

        [Fact]
        public void GetReplacementsForFailedMatchesTest_NoProgramFailures()
        {
            var formInput = new FormInput();
            formInput.Matches = _mockMatchesTestData.GetMockMatches(3);

            var failedProgramIds = new List<int>();

            FailedMatchReplacements failedMatchReplacements = GetReplacementsForFailedMatches(formInput, failedProgramIds, GetMockUserSelectionResponse());

            var expectedReplacementMatches = new List<Match>();

            failedMatchReplacements.ReplacementMatches.Should().BeEquivalentTo(expectedReplacementMatches);
            failedMatchReplacements.FailedMatches.Should().BeEquivalentTo(GetExpectedFailedMatches(formInput, failedProgramIds));
            Assert.Equal(string.Empty, failedMatchReplacements.Message);
        }

        [Fact]
        public void GetReplacementsForFailedMatchesTest_OneProgramFailure()
        {
            var formInput = new FormInput();
            formInput.Matches = _mockMatchesTestData.GetMockMatches(3);

            var failedProgramIds = new List<int> { 302601 };

            FailedMatchReplacements failedMatchReplacements = GetReplacementsForFailedMatches(formInput, failedProgramIds, GetMockUserSelectionResponse());

            var expectedReplacementMatches = new List<Match>
            {
                new Match
                {
                    InstitutionId = 8324,
                    InstitutionName = "QA National",
                    InstitutionLogoUrl = "/8324/{FILENAME}?1531394702",
                    ProgramId = 1,
                    ProgramName = "Master Program",
                    ProgramProductId = 123,
                    ProgramTemplateId = 400
                }
            };

            failedMatchReplacements.ReplacementMatches.Should().BeEquivalentTo(expectedReplacementMatches);
            failedMatchReplacements.FailedMatches.Should().BeEquivalentTo(GetExpectedFailedMatches(formInput, failedProgramIds));
            Assert.Equal("The selections made for Paid_Category have failed validation. You may be interested in the selections below instead.", failedMatchReplacements.Message);
        }

        [Fact]
        public void GetReplacementsForFailedMatchesTest_TwoProgramFailures()
        {
            var formInput = new FormInput();
            formInput.Matches = _mockMatchesTestData.GetMockMatches(3);

            var failedProgramIds = new List<int> { 302601, 302603 };

            FailedMatchReplacements failedMatchReplacements = GetReplacementsForFailedMatches(formInput, failedProgramIds, GetMockUserSelectionResponse());

            var expectedReplacementMatches = new List<Match>
            {
                new Match
                {
                    InstitutionId = 8324,
                    InstitutionName = "QA National",
                    InstitutionLogoUrl = "/8324/{FILENAME}?1531394702",
                    ProgramId = 1,
                    ProgramName = "Master Program",
                    ProgramProductId = 123,
                    ProgramTemplateId = 400
                },
                new Match
                {
                    InstitutionId = 1571,
                    InstitutionName = "Emerson College",
                    InstitutionLogoUrl = "/1571/{FILENAME}?1424796370",
                    ProgramId = 3,
                    ProgramName = "Bachelor Program",
                    ProgramProductId = 898,
                    ProgramTemplateId = 2,
                }
            };

            failedMatchReplacements.ReplacementMatches.Should().BeEquivalentTo(expectedReplacementMatches);
            failedMatchReplacements.FailedMatches.Should().BeEquivalentTo(GetExpectedFailedMatches(formInput, failedProgramIds));
            Assert.Equal("The selections made for Paid_Category, and Paid_Specialty_A have failed validation. You may be interested in the selections below instead.", failedMatchReplacements.Message);
        }

        [Fact]
        public void GetReplacementsForFailedMatchesTest_ThreeProgramFailures()
        {
            var formInput = new FormInput();
            formInput.Matches = _mockMatchesTestData.GetMockMatches(3);

            var failedProgramIds = new List<int> { 302601, 302603, 302597 };

            FailedMatchReplacements failedMatchReplacements = GetReplacementsForFailedMatches(formInput, failedProgramIds, GetMockUserSelectionResponse());

            var expectedReplacementMatches = new List<Match>
            {
                new Match
                {
                    InstitutionId = 8324,
                    InstitutionName = "QA National",
                    InstitutionLogoUrl = "/8324/{FILENAME}?1531394702",
                    ProgramId = 1,
                    ProgramName = "Master Program",
                    ProgramProductId = 123,
                    ProgramTemplateId = 400
                },
                new Match
                {
                    InstitutionId = 1571,
                    InstitutionName = "Emerson College",
                    InstitutionLogoUrl = "/1571/{FILENAME}?1424796370",
                    ProgramId = 3,
                    ProgramName = "Bachelor Program",
                    ProgramProductId = 898,
                    ProgramTemplateId = 2,
                },
                new Match
                {
                    InstitutionId = 4462,
                    InstitutionName = "University of Miami",
                    InstitutionLogoUrl = "/4462/{FILENAME}?1424796370",
                    ProgramId = 244982,
                    ProgramName = "Master of Arts in International Administration (MAIA)",
                    ProgramProductId = 626396,
                    ProgramTemplateId = 2
                }
            };

            failedMatchReplacements.ReplacementMatches.Should().BeEquivalentTo(expectedReplacementMatches);
            failedMatchReplacements.FailedMatches.Should().BeEquivalentTo(GetExpectedFailedMatches(formInput, failedProgramIds));
            Assert.Equal("The selections made for Paid_Category, Paid_Specialty_A, and GSMatch1 have failed validation. You may be interested in the selections below instead.", failedMatchReplacements.Message);
        }

        private List<Match> GetExpectedFailedMatches(FormInput formInput, List<int> failedProgramIds)
        {
            return formInput.Matches.Where(m => failedProgramIds.Contains(m.ProgramId)).Select(m => m).ToList();
        }

        private FailedMatchReplacements GetReplacementsForFailedMatches(FormInput formInput, List<int> failedProgramIds, UserSelectionResponse userSelectionResponse)
        {
            Mock<IMatchValidationService> mockMatchValidationService = GetMockMatchValidationService(formInput, failedProgramIds);
            Mock<IUserSelectionService> mockUserSelectionService = new Mock<IUserSelectionService>();
            mockUserSelectionService.Setup(s => s.GetUserSelectionsForSchoolPicker(It.IsAny<FormInput>(), It.IsAny<IEnumerable<int>>())).Returns(userSelectionResponse);

            Mock<IMetaDataService> mockMetaDataService = new Mock<IMetaDataService>();
            mockMetaDataService.Setup(s => s.GetMetaDataMessageByKey(It.Is<string>(k => k.Equals("SCHOOLPICKERWIZARD.MATCHREPLACEMENTS.MESSAGE")))).Returns("The selections made for {schools} have failed validation. You may be interested in the selections below instead.");

            var failedMatchReplacementService = new FailedMatchReplacementService(mockMatchValidationService.Object, mockUserSelectionService.Object, mockMetaDataService.Object);

            return failedMatchReplacementService.GetReplacementsForFailedMatches(formInput);
        }

        private Mock<IMatchValidationService> GetMockMatchValidationService(FormInput formInput, List<int> failedProgramIds)
        {
            var mockMatchValidationService = new Mock<IMatchValidationService>();

            HashSet<int> failedProgramIdsSet = new HashSet<int>(failedProgramIds);
            List<Match> failedMatches = new List<Match>();

            if (formInput?.Matches?.Count() > 0)
            {
                foreach (var match in formInput.Matches)
                {
                    if (failedProgramIdsSet.Contains(match.ProgramId))
                    {
                        failedMatches.Add(match);
                    }
                }
            }

            mockMatchValidationService.Setup(s => s.GetMatchesThatFailedValidation(It.IsAny<FormInput>())).Returns(failedMatches);
            return mockMatchValidationService;

        }


        private UserSelectionResponse GetMockUserSelectionResponse()
        {
            return new UserSelectionResponse
            {
                UserSelections = GetCampuses(),
                MatchResponseGuid = Guid.Empty
            };
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
                },
                new Campus
                {
                    InstitutionName = "University of Miami",
                    InstitutionDescription = "University of Miami Test Description",
                    InstitutionId = 4462,
                    CampusId = 44620,
                    CampusName = "University Of Miami Main",
                    CampusLogoUrl = "/44620/{FILENAME}?1424796370",
                    InstitutionLogoUrl = "/4462/{FILENAME}?1424796370",
                    ProgramRankScore = 0.575497943m,
                    Programs = new List<Program>
                    {
                        new Program
                        {
                            ProgramId = 244982,
                            ProgramName = "Master of Arts in International Administration (MAIA)",
                            ProgramProductId = 626396,
                            ProgramRankScore = 0.575498082m,
                            ProgramTemplateId = 2
                        },
                        new Program
                        {
                            ProgramId = 243996,
                            ProgramName = "Master of Arts in Liberal Studies (MALS)",
                            ProgramProductId = 359259,
                            ProgramRankScore = 0.575497804m,
                            ProgramTemplateId = 2
                        }
                    }
                }
            };
        }
    }
}