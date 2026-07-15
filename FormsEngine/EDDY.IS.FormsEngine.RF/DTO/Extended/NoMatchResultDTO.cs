using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class NoMatchResultDTO
    {
        [DataMember]
        public string RenderedNoMatch { get; set; }
    }
}