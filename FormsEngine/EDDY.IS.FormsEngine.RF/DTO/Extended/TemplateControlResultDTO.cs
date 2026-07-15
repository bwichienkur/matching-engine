using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class TemplateControlResultDTO
    {
        [DataMember]
        public string RenderedControls { get; set; }
        
        [DataMember]
        public bool HasAdditionalControls { get; set; }

        [DataMember]
        public Guid MatchResponseGuid { get; set; }

        [DataMember]
        public List<int> AdditionalControlsTemplates { get; set; }
    }
}
