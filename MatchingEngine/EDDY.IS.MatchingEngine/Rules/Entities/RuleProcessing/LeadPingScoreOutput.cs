using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public class LeadPingScoreOutput
    {
        public Int32 InstitutionId { get; set; }
        public List<Int32> ProductIDs { get; set; }
        public Int32? CampusID { get; set; }
        public String Score { get; set; }
        public Int32? ScoreId { get; set; }
    }
}
