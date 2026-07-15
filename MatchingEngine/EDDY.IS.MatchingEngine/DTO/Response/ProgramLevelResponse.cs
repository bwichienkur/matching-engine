using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ProgramLevelResponse : BaseMatchResponse
    {
        [DataMember]
        public List<ProgramLevel> ProgramLevelList { get; set; }
    }
}