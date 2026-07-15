using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public class ClientRelationshipCampusProductRuleInput : EntityRemovalReason
    {
        public int ClientCampusProductMappingId { get; set; }
        public int ClientRelationProductMappingId { get; set; }
        public int ClientCampusRelationshipId { get; set; }

        public int ClientRelationshipId { get; set; }
        public int InstitutionId { get; set; }
        public int ProductId { get; set; }
        public int CampusTypeId { get; set; }
        public string CampusPostalCode { get; set; }
        public DTO.CampusType CampusType { get; set; }
    }

}
