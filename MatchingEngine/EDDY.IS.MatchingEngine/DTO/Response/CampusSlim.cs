using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class CampusSlim
    {
        [DataMember]
        public int CampusId { get; set; }

        [DataMember]
        public CampusType? CampusType { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public string CountryName { get; set; }
    }
}
