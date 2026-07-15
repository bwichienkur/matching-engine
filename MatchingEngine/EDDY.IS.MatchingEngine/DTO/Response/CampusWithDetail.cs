using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class CampusWithInstitution : Campus
    {
        [DataMember]
        public bool Is2USchool { get; set; }

        [DataMember]
        public int? InstitutionId { get; set; }

        [DataMember]
        public string InstitutionName { get; set; }

        [DataMember]
        public DisclaimerType? InstitutionDisclaimerType { get; set; }

        [DataMember]
        public string InstitutionDisclaimer { get; set; }

        [DataMember]
        public string InstitutionDescription { get; set; }

        [DataMember]
        public string InstitutionDescriptionInternational { get; set; }

        [DataMember]
        public List<string> StartDateList { get; set; }

        [DataMember]
        public bool IsLiveTransfer { get; set; }

        [DataMember]
        public List<SchoolAgent> SchoolAgents { get; set; }
        [DataMember]
        public string CustomTCPA { get; set; }
        [DataMember]
        public string CustomContactCenterTCPA { get; set; }
    }
}
