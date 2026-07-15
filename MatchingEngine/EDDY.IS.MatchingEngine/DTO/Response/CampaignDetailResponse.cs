using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class CampaignDetailResponse
    {
        [DataMember]
        public Guid TrackID { get; set; }

        [DataMember]
        public int? MaxSmartMatchCount { get; set; }

        [DataMember]
        public int? MaxSubmissionCount { get; set; }

        [DataMember]
        public bool AllowExitPops { get; set; }

        [DataMember]
        public bool IsCallCenter { get; set; }

        [DataMember]
        public Base.AdditionalQuestionsFlowType AdditionalQuestionsFlowType { get; set; }

        [DataMember]
        public Base.AdditionalQuestionsFlowType ProgramWizardAdditionalQuestionsFlowType { get; set; }

        [DataMember]
        public string CampaignTCPAMessageName { get; set; }

        [DataMember]
        public bool AllowRemonetization { get; set; }

        [DataMember]
        public bool UseInternationalTemplate { get; set; }

        [DataMember]
        public int ChannelId { get; set; }

        [DataMember]
        public bool AllowLeaveBehinds { get; set; }

        [DataMember]
        public bool HasXVerify { get; set; }

        [DataMember]
        public int? OpenMailProfileId { get; set; }
    }
}
