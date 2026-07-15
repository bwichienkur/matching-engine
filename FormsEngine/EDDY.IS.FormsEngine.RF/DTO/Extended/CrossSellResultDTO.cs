using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class CrossSellResultDTO
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string RenderedCrossSell { get; set; }

        [DataMember]
        public int CrossSellProgramCount { get; set; }

        [DataMember]
        public int MaxCrossSellUserSelections { get; set; }

        [DataMember]
        public string CrossSellThankYouMessage { get; set; }
    }
}
