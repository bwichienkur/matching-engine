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
    public class InstitutionMapperTests
    {
        [Fact]
        public void MapInstitutionTest_NotNull()
        {
            var mapper = new InstitutionMapper();
            var institution = mapper.MapInstitution(null);

            Assert.NotNull(institution);
        }

        [Fact]
        public void MapInstitutionTest()
        {
            var institutionDetail = new MatchingEngine.InstitutionDetail
            {
                InstitutionId = 1,
                InstitutionName = "Test Institution",
                InstitutionDescription = "Test Institution Description",
                AccreditationOrganization = "Test Institution Accreditation",
                InstitutionLogoURL = "/8324/{FILENAME}?1531394702"
            };

            var expectedResult = new Institution
            {
                InstitutionId = institutionDetail.InstitutionId,
                InstitutionName = institutionDetail.InstitutionName,
                InstitutionDescription = institutionDetail.InstitutionDescription,
                InstitutionAccreditation = institutionDetail.AccreditationOrganization,
                InstitutionLogoUrl = institutionDetail.InstitutionLogoURL
            };

            var mapper = new InstitutionMapper();
            Institution actualResult = mapper.MapInstitution(institutionDetail);

            actualResult.Should().BeEquivalentTo(expectedResult);
        }


        [Fact]
        public void MapInstitutionsTest_NotNull()
        {
            var mapper = new InstitutionMapper();
            var institutions = mapper.MapInstitutions(null);

            Assert.NotNull(institutions);
        }

        [Fact]
        public void MapInstitutionsTest()
        {
            var matchingEngineInstitutions = GetTestMatchingEngineInstitutions();

            var mapper = new InstitutionMapper();
            List<Institution> actualResult = mapper.MapInstitutions(matchingEngineInstitutions);

            List<Institution> expectedResult = GetExpectedResult();

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        private MatchingEngine.InstitutionWithProgram[] GetTestMatchingEngineInstitutions()
        {
            return new MatchingEngine.InstitutionWithProgram[]
            {
                new MatchingEngine.InstitutionWithProgram
                {
                    InstitutionName = "QA National",
                    InstitutionDescription = "QA National Test Description",
                    InstitutionId = 8324,
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
                new MatchingEngine.InstitutionWithProgram
                {
                    InstitutionName = "Emerson College",
                    InstitutionDescription = "Emerson College Test Description",
                    InstitutionId = 1571,
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


        private List<Core.Models.Institution> GetExpectedResult()
        {
            return new List<Core.Models.Institution>
            {
                new Core.Models.Institution
                {
                    InstitutionName = "QA National",
                    InstitutionDescription = "QA National Test Description",
                    InstitutionId = 8324,
                    InstitutionLogoUrl = "/8324/{FILENAME}?1531394702",
                    ProgramRankScore = 99.9m,
                    Programs = new List<Core.Models.Program>
                    {
                        new Core.Models.Program
                        {
                            ProgramId = 1,
                            ProgramName = "Master Program",
                            ProgramProductId = 123,
                            ProgramTemplateId = 400,
                            ProgramRankScore = 98.9m,
                        },
                        new Core.Models.Program
                        {
                            ProgramId = 45,
                            ProgramName = "Doctor Program",
                            ProgramProductId = 1456,
                            ProgramTemplateId = 23,
                            ProgramRankScore = 98.0m,
                        }
                    }
                },
                new Core.Models.Institution
                {
                    InstitutionName = "Emerson College",
                    InstitutionDescription = "Emerson College Test Description",
                    InstitutionId = 1571,
                    InstitutionLogoUrl = "/1571/{FILENAME}?1424796370",
                    ProgramRankScore = 95.5m,
                    Programs = new List<Core.Models.Program>
                    {
                        new Core.Models.Program
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