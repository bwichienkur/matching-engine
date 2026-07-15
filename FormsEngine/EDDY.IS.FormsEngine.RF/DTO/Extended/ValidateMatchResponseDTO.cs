using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class ValidateMatchResponseDTO
    {
        [DataMember]
        public bool Valid { get; set; }
    }
}
