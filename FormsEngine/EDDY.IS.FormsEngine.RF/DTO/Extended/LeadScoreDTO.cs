using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class LeadScoreDTO
    {
        [DataMember]
        public LeadScoringService.ScoringRequest Request { get; set; }

        [DataMember]
        public LeadScoringService.ScoringResponse Response { get; set; }
    }
}
