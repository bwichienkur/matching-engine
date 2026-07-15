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
    public class APITemplateControlResultDTO
    {
        [DataMember]
        public int TemplateId { get; set; }

        [DataMember]
        public List<TemplateControlDTO> TemplateControls { get; set; }
    }
}
