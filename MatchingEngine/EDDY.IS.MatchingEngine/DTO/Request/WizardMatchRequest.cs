using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class WizardMatchRequest : BaseMatchRequest
    {
        [DataMember]
        public bool IncludeSmartMatchList { get; set; }

        [DataMember]
        public bool IncludeSchoolSelectionList { get; set; }

        [DataMember]
        public bool IncludeThirdPartyMatchList { get; set; }

        [DataMember]
        public int TotalLeadsSaved { get; set; }

        [DataMember]
        public int TotalSmartMatchesSaved { get; set; }

        [DataMember]
        public CampusPreference? CampusPreference { get; set; }

        [DataMember]
        public List<int> DesiredProgramLevelList { get; set; }

        [DataMember]
        public List<int> SmartMatchedInstituionIdList { get; set; }

        [DataMember]
        public bool? SplitCampusTypeInResults { get; set; }

        [DataMember]
        public List<int> TemplateList { get; set; }

        [DataMember]
        public LeadScoringInput LeadScoringInput { get; set; }

        [DataMember]
        public LeadCreationType LeadCreationType { get; set; }

        [DataMember]
        public string DialerKey { get; set; }

        [DataMember]
        public string TSR { get; set; }

        [DataMember]
        public int? UserId { get; set; }

        [DataMember]
        public GeoTarget GeoTarget { get; set; }

        [DataMember]
        public List<ProgramWithInstitutionCampus> ThirdPartySmartMatchList { get; set; }

        [DataMember]
        public List<int> BoostSpecialtyList { get; set; }

        public WizardMatchRequest(DirectoryMatchRequest request)
        {
            this.TrackGuid = request.TrackGuid;
            this.ProspectInput = request.ProspectInput;
            this.ProgramLevelList = request.ProgramLevelList;
            this.CategoryList = request.CategoryList;
            this.SubjectList = request.SubjectList;
            this.SpecialtyList = request.SpecialtyList;
            this.ApplicationId = request.ApplicationId;
            this.Application = request.Application;
            this.LeadCreationType = request.LeadCreationType.HasValue ? request.LeadCreationType.Value : LeadCreationType.WizardSmartMatch;
            this.IncludeThirdPartyMatchList = true;
        }
    }
}
