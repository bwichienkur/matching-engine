using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Tests.TestData
{
    public class MockMatchesTestData
    {
        private readonly List<Match> _mockMatches;

        public MockMatchesTestData()
        {
            _mockMatches = new List<Match>
            {
                new Match { InstitutionId = 8618, InstitutionName = "Paid_Category", ProgramName = "Paid_Category_Program", ProgramId = 302601, ProgramProductId = 634949, ProgramTemplateId = 2, PaidStatusType = 1 },
                new Match { InstitutionId = 8620, InstitutionName = "Paid_Specialty_A", ProgramName = "Paid_Specialty_A_Program", ProgramId = 302603, ProgramProductId = 634951, ProgramTemplateId = 2, PaidStatusType = 2 },
                new Match { InstitutionId = 8614, InstitutionName = "GSMatch1", ProgramName = "Program_GSMatch1", ProgramId = 302597, ProgramProductId = 634945, ProgramTemplateId = 1, PaidStatusType = 3 },
                new Match { InstitutionId = 8617, InstitutionName = "Match1Plus", ProgramName = "Program_Match1Plus", ProgramId = 302600, ProgramProductId = 634948, ProgramTemplateId = 1, PaidStatusType = 4 },
                new Match { InstitutionId = 8616, InstitutionName = "Match1", ProgramName = "Program_Match1", ProgramId = 302599, ProgramProductId = 634947, ProgramTemplateId = 1, PaidStatusType = 4 },
                new Match { InstitutionId = 8324, InstitutionName = "QA National", ProgramName = "Years_Teaching_Experience_Minimum 2 years", ProgramId = 302512, ProgramProductId = 634862, ProgramTemplateId = 4, PaidStatusType = 3 },
                new Match { InstitutionId = 8635, InstitutionName = "Eplite ", ProgramName = "Accounting Premier", ProgramId = 302658, ProgramProductId = 635010, ProgramTemplateId = 2, PaidStatusType = 2 },
                new Match { InstitutionId = 4462, InstitutionName = "University of Miami", ProgramName = "Master of Arts in International Administration (MAIA)", ProgramId = 244982, ProgramProductId = 626396, ProgramTemplateId = 2, PaidStatusType = 1 },
                new Match { InstitutionId = 272, InstitutionName = "Northcentral University", ProgramName = "Master of Education - International Education", ProgramId = 177449, ProgramProductId = 633838, ProgramTemplateId = 4, PaidStatusType = 1 },
                new Match { InstitutionId = 1571, InstitutionName = "Emerson College", ProgramName = "Master of Science in Communication Disorders", ProgramId = 298453, ProgramProductId = 617425, ProgramTemplateId = 2, PaidStatusType = 2 }
            };
        }

        public List<Match> GetMockMatches(int maxMatches)
        {
            return _mockMatches.Take(maxMatches).ToList();
        }
        
    }
}
