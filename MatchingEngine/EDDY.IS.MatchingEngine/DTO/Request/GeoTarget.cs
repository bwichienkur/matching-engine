using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{ 
    [DataContract]
    public class GeoTarget
    {
        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public int? RadiusFromPostalCode { get; set; }

        [DataMember]
        public List<int> StateList { get; set; }

        [DataMember]
        public List<int> CountryList { get; set; }

        [DataMember]
        public List<int> CityList { get; set; }

        [DataMember]
        public int? FreeInstitutionCount { get; set; }

        [DataMember]
        public int? OnlineInstitutionCount { get; set; }
    }
}