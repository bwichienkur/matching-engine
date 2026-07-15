using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class City
    {
        [DataMember]
        public int CityId { get; set; }

        [DataMember]
        public string CityName { get; set; }

        [DataMember]
        public int StateId { get; set; }

        public override bool Equals(object obj)
        {
            City q = obj as City;
            return q != null && q.CityId == this.CityId && q.CityName == this.CityName;
        }

        public override int GetHashCode()
        {
            return this.CityId.GetHashCode() ^ this.CityName.GetHashCode();
        }
    }
}
