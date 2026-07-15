using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class OpenMailValidationDTO
    {
        [DataMember]
        public int MatchedRuleGroup { get; set; }

        [DataMember]
        public bool SentToNoMatch { get; set; }

    }
}