using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ProgramDetailResponse : BaseMatchResponse
    {
        [DataMember]
        public ProgramDetail ProgramDetails { get; set; }
    }
}
