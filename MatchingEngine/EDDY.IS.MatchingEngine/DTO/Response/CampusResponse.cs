using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class CampusResponse : BaseMatchResponse
    {
        [DataMember]
        public List<CampusWithInstitution> CampusList { get; set; }

        [DataMember]
        public int ProgramResultCount { get; set; }
    }
}