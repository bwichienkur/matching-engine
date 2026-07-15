using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    public enum LimboReason
    {
        LeadScoringMinimumTierLevel = 1
    }

    [DataContract]
    public class WizardMatchResponse : BaseMatchResponse
    {
        [DataMember]
        public List<ProgramWithInstitutionCampus> SmartMatchList { get; set; }

        [DataMember]
        public List<CampusWithInstitution> SchoolSelectionList { get; set; }

        [DataMember]
        public List<ProgramWithInstitutionCampus> ThirdPartyMatchList { get; set; }

        [DataMember]
        public int? MaxUserSelections { get; set; }

        [DataMember]
        public LimboReason? WizardLimboReason { get; set; }

        [DataMember]
        public Guid? LimboAlternativeTrackId { get; set; }

        [DataMember]
        public bool LimboAlternativeTrackIdUtilized { get; set; }

        [DataMember]
        public List<Institution> DuplicateForInstitutionList { get; set; }
        [DataMember]
        public bool SkipSchoolSelection { get; set; }
    }
}
