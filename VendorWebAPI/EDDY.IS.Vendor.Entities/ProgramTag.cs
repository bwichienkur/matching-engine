using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    [DataContract]
    public class ProgramTag
    {
        [DataMember]
        public string TagType { get; set; }

        [DataMember]
        public string Tag { get; set; }
    }
}
