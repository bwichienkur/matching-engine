using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public class ProgramRuleInput : EntityRemovalReason
    {
        public int ProgramId { get; set; }
        public int ProgramLevelId { get; set; }

        public int InstitutionId { get; set; }
    }
}
