using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public class LeadPingRuleOutput : EntityRemovalReason
    {
        public int InstitutionId { get; set; }
        public int ClientRelationshipId { get; set; }

        public HashSet<int> Products { get; set; }
        public int? CampusId { get; set; }
        public Boolean IsWtEdmcDupe { get; set; }
        public Boolean? IsOnline { get; set; }
        public List<Int32> ProgramLevelIDs { get; set; }
    }

    public class LeadPingRemoval : EntityRemovalReason
    {
        public int InstitutionId { get; set; }
        public int ProductId { get; set; }
        public int ClientRelationshipId { get; set; }
        public int ClientRelationProductMappingId { get; set; }
    }
}
