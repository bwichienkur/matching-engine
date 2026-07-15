using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class LeadScoringInput
    {
        [DataMember]
        public int? LeadScoringTierLevel { get; set; }

        [DataMember]
        public Guid LeadScoringGuid { get; set; }

        [DataMember]
        public LeadScoringProductAssignment ProductAssignment { get; set; }
    }
    
    [DataContract]
    public class LeadScoringProductAssignment
    {
        [DataMember]
        public int? ChosenModelProductId { get; set; }        

        [DataMember]
        public List<CrProductAssignment> CrProductAssignmentList { get; set; }
    }

    [DataContract]
    public class CrProductAssignment
    {
        [DataMember]
        public int CrId { get; set; }

        [DataMember]
        public int ProductId { get; set; }
    }

}
