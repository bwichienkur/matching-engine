using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Base;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class BaseMatchRequest : BaseEntity
    {
        [DataMember]
        public Guid TrackGuid { get; set; } //campaign trackid

        [DataMember]
        public Guid? TrackingDeviceGuid { get; set; }

        [DataMember]
        public IS.Base.ISApplication? Application { get; set; }

        [DataMember]
        public int ApplicationId { get; set; }

        [DataMember]
        public bool IsBeta { get; set; }

        [DataMember]
        public int? MaxResultsCount { get; set; }

        [DataMember]
        public List<ProgramType> ProgramTypeList { get; set; } //1 = Degree, 2 = Course, 3 = Certificate

        [DataMember]
        public List<int> ProgramLevelList { get; set; } // 2 = Associate, 3 = Bachelor etc

        [DataMember]
        public CampusType? CampusType { get; set; } // 1 = Online, 2 = Ground

        [DataMember]
        public List<int> CategoryList { get; set; }

        [DataMember]
        public List<int> SubjectList { get; set; }

        [DataMember]
        public List<int> SpecialtyList { get; set; }

        [DataMember]
        public ProspectInput ProspectInput { get; set; }

        [DataMember]
        public bool? IsHybrid { get; set; }

        [DataMember]
        public List<int> CampusList { get; set; }

        [DataMember]
        public bool? IsNonProfit { get; set; }

        [DataMember]
        public List<SFProductCode> SFProductCodes { get; set; }

        public bool PaidOnly
        {
            get
            {
                //TODO: don't hardcode applications which allow free programs
                return ApplicationId == 7 || ApplicationId == 1 || ApplicationId == 20 ? false : true;
            }
        }
    }
}
