using EDDY.IS.FormsEngine.MatchingEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Tests.TestData
{
    public class MockMatchingEngineProgramTestData
    {
        public List<Program> GetMockMatchingEnginePrograms()
        {
            return new List<Program>
            {
                new Program
                {
                    ProgramId = 10,
                    ProgramName = "First Test Program",
                    ProgramDescription = "The First Test Program",
                    ProgramProductId = 300,
                    ProgramRankScore = 67,
                    ProductId = 200,
                    TemplateId = 400
                },
                new Program
                {
                    ProgramId = 20,
                    ProgramName = "Second Test Program",
                    ProgramDescription = "The Second Test Program",
                    ProgramProductId = 600,
                    ProgramRankScore = 77,
                    ProductId = 700,
                    TemplateId = null
                },
                new Program
                {
                    ProgramId = 30,
                    ProgramName = "Third Test Program",
                    ProgramDescription = "The Third Test Program",
                    ProgramProductId = 900,
                    ProgramRankScore = 87,
                    ProductId = null,
                    TemplateId = null
                }
            };
        }


        public List<ProgramWithInstitutionCampus> GetMockMatchingEngineProgramsWithInstitutionCampus()
        {
            return new List<ProgramWithInstitutionCampus>
            {
                new ProgramWithInstitutionCampus
                {
                    ProgramId = 100,
                    ProgramName = "First Test Program",
                    ProgramDescription = "The First Test Program",
                    ProductId = 200,
                    CampusId = 300,
                    CampusName = "First Test Campus",
                    CampusType = CampusType.Ground,
                    CampusLogoURL = "www.firsttestcampuslogourl.com",
                    InstitutionId = 400,
                    InstitutionName = "First Test Institution",
                    InstitutionLogoURL = "www.testinstitutionlogourl.com",
                    InstitutionDescription = "The First Test Institution"
                },
                new ProgramWithInstitutionCampus
                {
                    ProgramId = 500,
                    ProgramName = "Second Test Program",
                    ProgramDescription = "The Second Test Program",
                    ProductId = 600,
                    CampusId = 700,
                    CampusName = "Second Test Campus",
                    CampusType = CampusType.Online,
                    CampusLogoURL = "www.secondtestcampuslogourl.com",
                    InstitutionId = 800,
                    InstitutionName = "Test Institution",
                    InstitutionLogoURL = "www.testinstitutionlogourl.com",
                    InstitutionDescription = "Just A Test Institution"
                }
            };
        }

    }
}
