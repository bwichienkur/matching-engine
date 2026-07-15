using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.Models;
using FluentAssertions;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Tests
{
    public class CampusMapperTests
    {
        [Fact]
        public void MapCampusesTest_NotNull()
        {
            var mapper = new CampusMapper();
            var campuses = mapper.MapCampuses(null);

            Assert.NotNull(campuses);
        }

        [Fact]
        public void MapCampusesTest()
        {
            var matchingEngineCampuses = GetTestMatchingEngineCampuses();

            var mapper = new CampusMapper();
            List<Campus> actualRestult = mapper.MapCampuses(matchingEngineCampuses);

            List<Campus> expectedResult = GetExpectedResult();
            actualRestult.Should().BeEquivalentTo(expectedResult);
        }

        private MatchingEngine.CampusWithInstitution[] GetTestMatchingEngineCampuses()
        {
            return new MatchingEngine.CampusWithInstitution[]
            {
                new MatchingEngine.CampusWithInstitution
                {
                    InstitutionName = "QA National",
                    InstitutionDescription = "QA National Test Description",
                    InstitutionId = 8324,
                    CampusId = 83240,
                    CampusName = "QA National Main",
                    CampusLogoURL = "/83240/{FILENAME}?1531394702",
                    InstitutionLogoURL = "/8324/{FILENAME}?1531394702",
                    ProgramRankScore = 99.9m,
                    ProgramList = new MatchingEngine.Program[]
                    {
                        new MatchingEngine.Program
                        {
                            ProgramId = 1,
                            ProgramName = "Master Program",
                            ProgramProductId = 123,
                            TemplateId = 400,
                            ProgramRankScore = 98.9m,
                        },
                        new MatchingEngine.Program
                        {
                            ProgramId = 45,
                            ProgramName = "Doctor Program",
                            ProgramProductId = 1456,
                            TemplateId = 23,
                            ProgramRankScore = 98.0m,
                        }
                    }
                },
                new MatchingEngine.CampusWithInstitution
                {
                    InstitutionName = "Emerson College",
                    InstitutionDescription = "Emerson College Test Description",
                    InstitutionId = 1571,
                    CampusId = 15710,
                    CampusName = "Emerson College Main",
                    CampusLogoURL = "/15710/{FILENAME}?1424796370",
                    InstitutionLogoURL = "/1571/{FILENAME}?1424796370",
                    ProgramRankScore = 95.5m,
                    ProgramList = new MatchingEngine.Program[]
                    {
                        new MatchingEngine.Program
                        {
                            ProgramId = 3,
                            ProgramName = "Bachelor Program",
                            ProgramProductId = 898,
                            TemplateId = 2,
                            ProgramRankScore = 95.5m,
                        }
                    }
                }
            };
        }


        private List<Campus> GetExpectedResult()
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
    }
}