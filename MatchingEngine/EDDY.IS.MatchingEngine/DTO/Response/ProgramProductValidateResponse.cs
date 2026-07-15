using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ProgramProductValidateResponse
    {
        [DataMember]
        public List<ProgramValidateResponse> ProgramProductValidateResults { get; set; }
    }
}