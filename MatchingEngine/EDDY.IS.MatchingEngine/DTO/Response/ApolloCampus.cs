using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ApolloCampus : CampusWithInstitution
    {
        [DataMember]
        public int? DailyGoal { get; set; }
        [DataMember]
        public int? TransferAmount { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public double? Distance { get; set; }
        [DataMember]
        public bool IsFaithBased { get; set; }
        [DataMember]
        public bool HasAdditionalScript { get; set; }

        [DataMember]
        public Guid? ExternalMatchItemGuid { get; set; }

        [DataMember]
        public int? InstitutionGroupId { get; set; }

        [DataMember]
        public bool AllowMultiSelectForInstitutionGroup { get; set; }
    }
}
