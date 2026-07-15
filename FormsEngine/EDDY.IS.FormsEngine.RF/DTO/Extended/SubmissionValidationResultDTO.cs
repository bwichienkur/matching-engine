using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class SubmissionValidationResultDTO
    {
        [DataMember]
        public bool Valid { get; set; }

        [DataMember]
        public string LeadData { get; set; }
    }
}
