using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public abstract class BaseMatchEntity
    {
        [DataMember]
        public bool FailedValidation { get; set; }

        [DataMember]
        public decimal ProgramRankScore { get; set; }
    }
}
