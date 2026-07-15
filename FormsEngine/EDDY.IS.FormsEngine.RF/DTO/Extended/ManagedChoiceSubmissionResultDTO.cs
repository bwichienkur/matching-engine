using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class ManagedChoiceSubmissionResultDTO
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public bool MoveToStart { get; set; }

        [DataMember]
        public bool MoveToThankYou { get; set; }

        [DataMember]
        public bool MoveToNoMatch { get; set; }

        [DataMember]
        public int SMLeadsCreatedCount { get; set; }

        [DataMember]
        public int USLeadsCreatedCount { get; set; }
    }
}