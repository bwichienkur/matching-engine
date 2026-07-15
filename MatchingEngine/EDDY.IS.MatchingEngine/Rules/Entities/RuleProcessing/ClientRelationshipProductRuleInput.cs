using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public class ClientRelationshipProductRuleInput : EntityRemovalReason
    {
        public int ClientRelationProductMappingId { get; set; }
        public int ClientRelationshipId { get; set; }
        public int InstitutionId { get; set; }
        public int ProductId { get; set; }
        public bool ExcludeMatch1plusForFinAid { get; set; }
        public bool RequireJournayaLeadId { get; set; }

        
    }

}
