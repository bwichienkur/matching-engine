using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public class ClickRuleInput : EntityRemovalReason
    {
        public int ClientRelationProductMappingId { get; set; }
        public int ClientCampusRelationshipId { get; set; }
        public int PsiId { get; set; }
        public int ProductId { get; set; }
    }

}
