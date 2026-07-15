using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class CrossSellMatchRequest : BaseMatchRequest
    {
        [DataMember]
        public int FormProgramProductId { get; set; }

        [DataMember]
        public int FormTemplateId { get; set; }

        [DataMember]
        public int FormInstitutionId { get; set; }

        [DataMember]
        public int FormDefaultTemplateId { get; set; }

        [DataMember]
        public LeadScoringInput LeadScoringInput { get; set; }

        [DataMember]
        public LeadCreationType LeadCreationType { get; set; }

        [DataMember]
        public bool InitialLeadSuccess { get; set; }
    }
}
