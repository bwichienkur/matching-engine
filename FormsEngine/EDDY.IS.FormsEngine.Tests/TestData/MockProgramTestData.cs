using EDDY.IS.FormsEngine.Core.Enums;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Tests.TestData
{
    public class MockProgramTestData
    {
        public List<Program> GetMockPrograms()
        {
            return new List<Program>
            {
                new Program
                {
                    ProgramId = 1,
                    ProgramName = "First Test Program",
                    ProgramDescription = "<h2>The First Test Program<h2>",
                    ProductId = 19,
                    CampusId = 100,
                    CampusName = "First Test Campus",
                    CampusType = CampusType.Ground,
                    CampusLogoUrl = "/100/{FILENAME}?1531394702",
                    InstitutionId = 1000,
                    InstitutionName = "First Test Institution",
                    InstitutionLogoUrl = "/1000/{FILENAME}?1531394702",
                    InstitutionDescription = "The First Test Institution"
                },
                new Program
                {
                    ProgramId = 2,
                    ProgramName = "Second Test Program",
                    ProgramDescription = "<h2>The Second Test Program<h2>",
                    ProductId = 20,
                    CampusId = 200,
                    CampusName = "Second Test Campus",
                    CampusType = null,
                    CampusLogoUrl = "",
                    InstitutionId = 2000,
                    InstitutionName = "Second Test Institution",
                    InstitutionLogoUrl = "/2000/{FILENAME}?1531394702",
                    InstitutionDescription = "The Second Test Institution"
                },
                new Program
                {
                    ProgramId = 3,
                    ProgramName = "Third Test Program",
                    ProgramDescription = "<h2>The Third Test Program<h2>",
                    ProductId = 30,
                    CampusId = 300,
                    CampusName = "Third Test Campus",
                    CampusType = CampusType.Online,
                    CampusLogoUrl = "/300/{FILENAME}?1531394702",
                    InstitutionId = 3000,
                    InstitutionName = "Third Test Institution",
                    InstitutionLogoUrl = "/3000/{FILENAME}?1531394702",
                    InstitutionDescription = "The Third Test Institution"
                },
                new Program
                {
                    ProgramId = 4,
                    ProgramName = "Fourth Test Program",
                    ProgramDescription = "<h2>The Fourth Test Program<h2>",
                    ProductId = 40,
                    CampusId = 400,
                    CampusName = "Fourth Test Campus",
                    CampusType = CampusType.Online,
                    CampusLogoUrl = "",
                    InstitutionId = 4000,
                    InstitutionName = "Fourth Test Institution",
                    InstitutionLogoUrl = "/4000/{FILENAME}?1531394702",
                    InstitutionDescription = "The Fourth Test Institution"
                }
            };
        }
    }
}
