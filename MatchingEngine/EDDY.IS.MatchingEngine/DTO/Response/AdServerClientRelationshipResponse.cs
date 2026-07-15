using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class AdServerClientRelationshipResponse : BaseMatchResponse
    {
        [DataMember]
        public List<AdServerClientRelationship> CRList { get; set; }
    }
}
