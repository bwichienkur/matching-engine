using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO        
{
    [DataContract]
    public class CountryValidateRequest
    {
        [DataMember]
        public int ProgramProductId { get; set; }

        [DataMember]
        public int CountryId { get; set; }

        [DataMember]
        public IS.Base.ISApplication Application { get; set; }
    }
}
