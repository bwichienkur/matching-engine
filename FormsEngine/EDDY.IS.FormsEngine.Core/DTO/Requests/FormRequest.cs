using EDDY.IS.Core;
using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.DTO
{
    public class FormRequest
    {
        public string TrackId { get; set; }
        public string SessionId { get; set; }
        public string FESessionId { get; set; }
        public string MatchGuid { get; set; }
        public int ApplicationId { get; set; }
        public int? InstitutionId { get; set; }
        public int? FeatureId { get; set; }
        public bool IsBeta { get; set; }
        public int TemplateId { get; set; }
        public int? ProspectId { get; set; }
        public string RenderingStrategy { get; set; }
        public string LeadData { get; set; }
        public string AdditionalData { get; set; }
        public string LeadIdToken { get; set; }
        public string InitialLeadId { get; set; }
        public string LimboAlternativeCampaignTrackid { get; set; }
        public bool LimboAlternativeCampaignTrackidUtilized { get; set; }
        public string Theme { get; set; }
    }
}
