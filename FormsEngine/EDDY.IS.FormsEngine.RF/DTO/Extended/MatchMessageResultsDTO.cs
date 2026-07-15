using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class MatchMessageResultsDTO
    {
        // top section
        [DataMember]
        public string NameText { get; set; }
        [DataMember]
        public string NoSmartMatchMessage1 { get; set; }
        [DataMember]
        public string SmartMatchMessage1 { get; set; }

        // bottom section
        [DataMember]
        public string UserSelectMessage1 { get; set; }
        [DataMember]
        public string UserSelectMessage2 { get; set; }

        public MatchMessageResultsDTO()
        {
            this.NameText = "";
            this.NoSmartMatchMessage1 = "";
            this.SmartMatchMessage1 = "";
            this.UserSelectMessage1 = "";
            this.UserSelectMessage2 = "";
        }
    }
}
