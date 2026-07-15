using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.LeadEngine.DTO
{
    [DataContract]
    public class LeadCreateRequest
    {
        [DataMember]
        public int? TemplateId { get; set; }

        [DataMember]
        public int? ProspectId { get; set; }

        [DataMember]
        public int? ClientRelationContactId { get; set; }

        [DataMember]
        public int ProgramProductId { get; set; }

        [DataMember]
        public Dictionary<string, string> LeadData { get; set; }

        [DataMember]
        public Dictionary<string, string> LeadAdditionalData { get; set; }

        [DataMember]
        public long RawPostDataId { get; set; }

        [DataMember]
        public bool IsBeta { get; set; }

        [DataMember]
        public Guid? TrackId { get; set; }

        [DataMember]
        public Guid? LimboAlternativeCampaignTrackid { get; set; }

        [DataMember]
        public bool LimboAlternativeCampaignTrackidUtilized { get; set; }

        [DataMember]
        public Guid? TrackingSessionGUID { get; set; }

        [DataMember]
        public Guid? MatchResponseGuid { get; set; }

        [DataMember]
        public bool PassedValidation { get; set; }

        [DataMember]
        public int? PaidStatusType { get; set; }

        [DataMember]
        public bool IsExternalDuplicate { get; set; }

        [DataMember]
        public bool IsInternalDuplicate { get; set; }

        [DataMember]
        public int? SubmissionId { get; set; }

        [DataMember]
        public int? LeadCreationTypeId { get; set; }

        [DataMember]
        public string InitialLeadId { get; set; }

        [DataMember]
        public bool AllowedViaLeadScoringUpsell { get; set; }

        [DataMember]
        public int? ProductId { get; set; }

        [DataMember]
        public Guid? ExternalMatchItemGuid { get; set; }

        [DataMember]
        public int? ProspectFlowId { get; set; }

        [DataMember]
        public bool PreValidatedProgram { get; set; }

        [DataMember]
        public int ChannelId { get; set; }

        [DataMember]
        public int SubChannelId { get; set; }

        [DataMember]
        public Guid? ExternalLeadId { get; set; }

        [DataMember]
        public decimal? EstimatedRevShare { get; set; }

        [DataMember]
        public bool ValidateTCPA { get; set; }

        [DataMember]
        public Guid? WidgetRequestGuid { get; set; }

        [DataMember]
        public string WidgetName { get; set; }

        [DataMember]
        public decimal? EstimatedLeadRev { get; set; }

        [DataMember]
        public int? ScoreId { get; set; }
        [DataMember]
        public string InitiatingURL { get; set; }
        [DataMember]
        public string LandingURL { get; set; }
        [DataMember]
        public string CallCenterURL { get; set; }
        [DataMember]
        public bool IsUrlDerived { get; set; }
        [DataMember]
        public bool IsSpam { get; set; }
        [DataMember]
        public bool IsSpamReportingOnly { get; set; }
        [DataMember]
        public int? InstitutionId { get; set; }
        [DataMember]
        public string InstitutionName { get; set; }
        [DataMember]
        public string VideoUrl { get; set; }
    }
}
