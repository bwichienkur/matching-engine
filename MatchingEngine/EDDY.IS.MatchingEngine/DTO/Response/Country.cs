using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class CampusCountry
    {
        [DataMember]
        public int CountryId { get; set; }

        [DataMember]
        public string CountryName { get; set; }

        public override bool Equals(object obj)
        {
            CampusCountry q = obj as CampusCountry;
            return q != null && q.CountryId == this.CountryId && q.CountryName == this.CountryName;
        }

        public override int GetHashCode()
        {
            return this.CountryId.GetHashCode() ^ this.CountryName.GetHashCode();
        }
    }
}
