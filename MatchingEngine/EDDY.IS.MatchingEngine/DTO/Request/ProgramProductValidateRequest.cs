using EDDY.IS.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    public class ProgramProductValidateRequest
    {
        [DataMember]
        public List<int> ProgramProductIds { get; set; }

        [DataMember]
        public ProspectInput ProspectInput { get; set; }

        [DataMember]
        public ISApplication Application { get; set; }

        [DataMember]
        public Guid TrackGuid { get; set; }

        [DataMember]
        public LeadCreationType LeadCreationType { get; set; }
    }
}