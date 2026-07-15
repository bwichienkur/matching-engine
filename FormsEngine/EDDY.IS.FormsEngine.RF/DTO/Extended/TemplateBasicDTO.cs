using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class TemplateBasicDTO
    {
        [DataMember]
        public int TemplateId { get; set; }
        
        [DataMember]
        public string TemplateName { get; set; }
        
        [DataMember]
        public string Description { get; set; }
    }
}
