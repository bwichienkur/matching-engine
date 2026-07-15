using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class Address
    {
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string Address2 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string StateProvinceCode { get; set; }
        [DataMember]
        public string PostalCode { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string CountryName { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Fax { get; set; }
        [DataMember]
        public int? CountryId { get; set; }
        [DataMember]
        public int? CityId { get; set; }
        [DataMember]
        public int? StateId { get; set; }
    }
}
