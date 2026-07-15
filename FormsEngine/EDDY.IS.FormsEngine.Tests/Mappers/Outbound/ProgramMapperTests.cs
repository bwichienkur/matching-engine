using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Tests.TestData;
using FluentAssertions;
using EDDY.IS.FormsEngine.Core.Enums;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Tests
{
    public class ProgramMapperTests
    {
        private readonly MockMatchingEngineProgramTestData _mockProgramTestData;

        public ProgramMapperTests()
        {
            _mockProgramTestData = new MockMatchingEngineProgramTestData();
        }

        [Fact]
        public void MapPrograms_NotNull()
        {
            var programs = MapPrograms(null);

            Assert.NotNull(programs);
        }

        [Fact]
        public void MapPrograms_ProgramIdMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEnginePrograms();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.ProgramId).Should().Equal(programs.Select(c => c.ProgramId));
        }

        [Fact]
        public void MapPrograms_ProgramNameMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEnginePrograms();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.ProgramName).Should().Equal(programs.Select(c => c.ProgramName));
        }


        [Fact]
        public void MapPrograms_ProgramDescriptionMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEnginePrograms();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.ProgramDescription).Should().Equal(programs.Select(c => c.ProgramDescription));
        }

        [Fact]
        public void MapPrograms_ProgramProductIdMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEnginePrograms();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.ProgramProductId).Should().Equal(programs.Select(c => c.ProgramProductId));
        }

        [Fact]
        public void MapPrograms_ProgramRankScoreMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEnginePrograms();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.ProgramRankScore).Should().Equal(programs.Select(c => c.ProgramRankScore));
        }

        [Fact]
        public void MapPrograms_ProgramTemplateIdMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEnginePrograms();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.TemplateId).Should().Equal(programs.Select(c => c.ProgramTemplateId));
        }

        [Fact]
        public void MapPrograms_ProductIdMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEnginePrograms();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.ProductId).Should().Equal(programs.Select(c => c.ProductId));
        }

        [Fact]
        public void MapPrograms_InsitutionIdMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEngineProgramsWithInstitutionCampus();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.InstitutionId).Should().Equal(programs.Select(c => c.InstitutionId));
        }

        [Fact]
        public void MapPrograms_InsitutionNameMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEngineProgramsWithInstitutionCampus();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.InstitutionName).Should().Equal(programs.Select(c => c.InstitutionName));
        }

        [Fact]
        public void MapPrograms_InsitutionLogoUrlMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEngineProgramsWithInstitutionCampus();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.InstitutionLogoURL).Should().Equal(programs.Select(c => c.InstitutionLogoUrl));
        }

        [Fact]
        public void MapPrograms_InsitutionDescriptionMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEngineProgramsWithInstitutionCampus();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.InstitutionDescription).Should().Equal(programs.Select(c => c.InstitutionDescription));
        }

        [Fact]
        public void MapPrograms_CampusIdMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEngineProgramsWithInstitutionCampus();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.CampusId).Should().Equal(programs.Select(c => c.CampusId));
        }

        [Fact]
        public void MapPrograms_CampusNameMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEngineProgramsWithInstitutionCampus();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.CampusName).Should().Equal(programs.Select(c => c.CampusName));
        }

        [Fact]
        public void MapPrograms_CampusLogoUrlMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEngineProgramsWithInstitutionCampus();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => c.CampusLogoURL).Should().Equal(programs.Select(c => c.CampusLogoUrl));
        }

        [Fact]
        public void MapPrograms_CampusTypeMapped()
        {
            var matchingEnginePrograms = _mockProgramTestData.GetMockMatchingEngineProgramsWithInstitutionCampus();

            var programs = MapPrograms(matchingEnginePrograms);

            matchingEnginePrograms.Select(c => MapCampusType(c.CampusType)).Should().Equal(programs.Select(c => c.CampusType));
        }

        private CampusType? MapCampusType(MatchingEngine.CampusType matchingEngineCampusType)
        {
            CampusType? campusType = null;

            if (matchingEngineCampusType == MatchingEngine.CampusType.Online)
            {
                campusType = CampusType.Online;
            }
            else if (matchingEngineCampusType == MatchingEngine.CampusType.Ground)
            {
                campusType = CampusType.Ground;
            }

            return campusType;
        }

        public static IEnumerable<object[]> GetTestParams()
        {
            return new List<object[]>
            {
                new object[] { MatchingEngine.CampusType.Ground },
                new object[] { MatchingEngine.CampusType.Online }
            };
        }

        private List<Program> MapPrograms(IEnumerable<MatchingEngine.Program> matchingEnginePrograms)
        {
            var mapper = new ProgramMapper();
            return mapper.MapPrograms(matchingEnginePrograms);
        }
    }
}