using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.DTO
{
    public class FormInput
    {
        public int TemplateId { get; set; }
        public Guid TrackId { get; set; }
        public string SessionId { get; set; }
        public string FESessionId { get; set; }
        public Guid MatchResponseGuid { get; set; }
        public int ApplicationId { get; set; }
        public int? FeatureId { get; set; }
        public bool IsBeta { get; set; }
        public int? ProgramId { get; set; }
        public List<int> ProgramIds { get; set; }
        public int? CampusId { get; set; }
        public string CampusPreference { get; set; }
        public int? InstitutionId { get; set; }
        public List<int> InstitutionIds { get; set; }
        public List<int> Categories { get; set; }
        public List<int> Subcategories { get; set; }
        public List<int> Specialties { get; set; }
        public Prospect Prospect { get; set; }
        public int? ProgramLevelId { get; set; }
        public List<int> ProgramLevelIds { get; set; }
        public List<Match> Matches { get; set; }
        public int MaxSchoolPickerMatches { get; set; }
        public string HttpReferer { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
        public string LeadData { get; set; }
        public string LeadAdditionalData { get; set; }
        public LeadCreationType? LeadCreationType { get; set; }
        public string InitialLeadId { get; set; }
        public Guid LimboAlternativeCampaignTrackId { get; set; }
        public bool LimboAlternativeCampaignTrackIdUtilized { get; set; }
        public int? AdvisorId { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public FormValidationResult FormValidationResult { get; set; }
        public int ProspectFlowTypeId { get; set; }
        public int? ProspectFlowId { get; set; }
    }
}
