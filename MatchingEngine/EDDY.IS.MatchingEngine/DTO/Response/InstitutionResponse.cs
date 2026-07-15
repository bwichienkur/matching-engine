using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    [KnownType(typeof(InstitutionWithCampus))]
    [KnownType(typeof(InstitutionWithProgram))]
    public class InstitutionResponse : BaseMatchResponse
    {
        [DataMember]
        public List<Institution> InstitutionList { get; set; }

        [DataMember]
        public int ProgramResultCount { get; set; }
    }
}