using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class ThankYouResultDTO
    {
        [DataMember]
        public string RenderedThankYou { get; set; }

        [DataMember]
        public bool MoveToStart { get; set; }

        [DataMember]
        public bool MoveToNoMatch { get; set; }
    }
}