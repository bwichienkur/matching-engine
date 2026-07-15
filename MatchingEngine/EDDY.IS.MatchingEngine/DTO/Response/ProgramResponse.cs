using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    [KnownType(typeof(ProgramWithInstitutionCampus))]
    public class ProgramResponse : BaseMatchResponse
    {
        [DataMember]
        public List<Program> ProgramList { get; set; }

        [DataMember]
        public int PaidInAreaProgramCount { get; set; }
    }
}