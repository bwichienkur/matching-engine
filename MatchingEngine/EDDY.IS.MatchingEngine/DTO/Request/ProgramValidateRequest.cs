using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    public class ProgramValidateRequest
    {
        [DataMember]
        public int ProgramProductId { get; set; }

        [DataMember]
        public IS.Base.ISApplication Application { get; set; }

        [DataMember]
        public Guid TrackGuid { get; set; }

        [DataMember]
        public ProspectInput ProspectInput { get; set; }

        [DataMember]
        public bool BreakOnFirstValidationFailure { get; set; }

        [DataMember]
        public bool IgnoreCaps { get; set; }

        [DataMember]
        public LeadCreationType LeadCreationType { get; set; }

        [DataMember]
        public int? ProgramId { get; set; }

		[DataMember]
		public bool? LookForEPUpsell { get; set; }

        [DataMember]
        public int? AgentId { get; set; }

        [DataMember]
        public int? CampusId { get; set; }
    }
}
