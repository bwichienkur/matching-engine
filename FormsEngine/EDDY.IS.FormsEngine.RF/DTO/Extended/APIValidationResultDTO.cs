using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EDDY.IS.FormsEngine.DTO.Extended;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class APIValidationResultDTO
    {
        [DataMember]
        public bool Valid { get; set; }

        [DataMember]
        public bool IsTestLead { get; set; }

        [DataMember]
        public List<KeyValuePair<string, string>> ValidationMessages { get; set; }
    }
}
