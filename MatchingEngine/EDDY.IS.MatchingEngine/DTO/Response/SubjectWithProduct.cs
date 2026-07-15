using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class SubjectWithProduct : Subject
    {
        [DataMember]
        public List<int> OnlineProducts { get; set; }

        [DataMember]
        public List<int> CampusProducts { get; set; }

        [DataMember]
        public List<ProgramLevel> ProgramLevels { get; set; }

        [DataMember]
        public int WTCount { get; set; }

        [DataMember]
        public int SMPCount { get; set; }
    }
}
