using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.LeadEngine.DataModel;

namespace EDDY.IS.LeadEngine.DTO
{
    [DataContract]
    public class LeadCreateResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public LeadDTO Lead { get; set; }

        [DataMember]
        public string ResultMessage { get; set; }

        [DataMember]
        public bool IsTestLead { get; set; }
    }
}
