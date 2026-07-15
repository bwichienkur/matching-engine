using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.DTO
{
    [Serializable]
    [DataContract]
    public class ThankYouPageResponse
    {
        [DataMember]
        public string RenderedThankYou { get; set; }
        [DataMember]
        public bool MoveToStart { get; set; }
        [DataMember]
        public bool MoveToNoMatch { get; set; }
    }
}
