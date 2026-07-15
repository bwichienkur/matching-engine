using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    // NOTE: No database table for this entity yet, will come from the RS directories
    public partial class ThemeDTO
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Path { get; set; }
    }

}
