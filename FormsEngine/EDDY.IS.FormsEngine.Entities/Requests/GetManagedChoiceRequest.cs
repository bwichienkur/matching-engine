using EDDY.IS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Entities.Requests
{
    public class GetManagedChoiceRequest
    {
        public int TemplateId { get; set; }
        public string RenderingStrategy { get; set; }
        public bool IsBeta { get; set; }
        public string TrackId { get; set; }
        public string SessionId { get; set; }
        public string MatchGuid { get; set; }
        public string LeadData { get; set; }
        public string AdditionalData { get; set; }
        public int SMLeadsCreatedCount { get; set; }
        public int USLeadsCreatedCount { get; set; }
        public string FESessionId { get; set; }
        public string DeviceId { get; set; }
        public string RenderingExperience { get; set; }
        public string LimboAlternativeCampaignTrackid { get; set; }
        public bool LimboAlternativeCampaignTrackidUtilized { get; set; }
        public FormTemplateTypes FormTemplateType { get; set; }
        public string InstitutionName { get; set; }
        public string ProgramName { get; set; }
        public bool? SplitCampusTypeInResults { get; set; }
        public int? ProspectId { get; set; }
        public int? ProgramTemplateId { get; set; }
        public int? ProgramProductId { get; set; }
        public int? ProductId { get; set; }
        public int? InstitutionId { get; set; }
        public int? ApplicationId { get; set; }
    }
}
