using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class LeadSubmissionsResultDTO
    {
        [DataMember]
        public List<decimal> LeadIds { get; set; }

        public LeadSubmissionsResultDTO()
        {
            LeadIds = new List<decimal>();
        }
    }
}
