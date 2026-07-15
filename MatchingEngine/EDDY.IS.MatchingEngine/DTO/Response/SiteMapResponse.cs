using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class SiteMapResponse : BaseMatchResponse
    {
        [DataMember]
        public List<SiteMapCountry> SiteMapCountryList { get; set; }

        [DataMember]
        public List<SiteMapState> SiteMapStateList { get; set; }
    }

    [DataContract]
    public class SiteMapCountry
    {
        [DataMember]
        public int CountryId { get; set; }

        [DataMember]
        public int PaidProgramCount { get; set; }

        [DataMember]
        public int FraidProgramCount { get; set; }

        [DataMember]
        public int FreeProgramCount { get; set; }
    }

    [DataContract]
    public class SiteMapState
    {
        [DataMember]
        public int StateId { get; set; }

        [DataMember]
        public int PaidInAreaProgramCount { get; set; }
    }
}
