using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public enum DisclaimerTypeDTO
    {
        Text = 1,
        Link = 2
    }
}
