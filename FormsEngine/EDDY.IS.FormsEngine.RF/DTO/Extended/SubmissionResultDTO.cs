using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class SubmissionResultDTO
    {
        [DataMember]
        public bool InitialMatchWasValid { get; set; }

        [DataMember]
        public bool WasLeadCreated { get; set; }

        [DataMember]
        public decimal InitialLeadId { get; set; }

        [DataMember]
        public string UID { get; set; }

        [DataMember]
        public long RawPostDataId { get; set; }

        [DataMember]
        public bool IsTestLead { get; set; }

        [DataMember]
        public CrossSellResultDTO CrossSellResult { get; set; }

        [DataMember]
        public int? ProspectId { get; set; }

        public SubmissionResultDTO()
        {
            this.CrossSellResult = new CrossSellResultDTO();
        }
    }
}
