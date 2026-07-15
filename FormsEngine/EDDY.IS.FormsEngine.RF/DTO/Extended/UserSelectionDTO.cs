using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class UserSelectionDTO
    {
        [DataMember]
        public int  InstitutionId { get; set; }

        [DataMember]
        public string InstitutionName { get; set; }

        [DataMember]
        public string ProgramName { get; set; }

        [DataMember]
        public int ProgramProductId { get; set; }

        [DataMember]
        public string Logo { get; set; }
    }
}
