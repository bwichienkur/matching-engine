using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EDDY.IS.FormsEngine.MatchingEngine;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class CampaignDetailDTO
    {
        [DataMember]
        public int MaxSmartMatchCount { get; set; }

        [DataMember]
        public bool AdditionalQuestionsOnlyInSchoolSelection { get; set; }

        [DataMember]
        public bool AdditionalQuestionsFromSmartMatch { get; set; }
        
        [DataMember]
        public bool IsCallCenter { get; set; }
        
        [DataMember]
        public string CampaignTCPAMessageName { get; set; }

        [DataMember]
        public int ProgramWizardAdditionalQuestionsFlowType { get; set; }

        [DataMember]
        public bool UseInternationalTemplate { get; set; }


        [DataMember]
        public bool AllowRemonetization { get; set; }

        [DataMember]
        public int ChannelId { get; set; }

        [DataMember]
        public bool AllowExitPops { get; set; }

        [DataMember]
        public bool HasXVerify { get; set; }

        [DataMember]
        public int MaxSubmissionCount { get; set; }
        [DataMember]
        public int? OpenMailProfileId { get; set; }

    }
}