using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class SchoolAgent
    {
        [DataMember]
        public int AgentId { get; set; }

        [DataMember]
        public string AgentName { get; set; }
    }
}
