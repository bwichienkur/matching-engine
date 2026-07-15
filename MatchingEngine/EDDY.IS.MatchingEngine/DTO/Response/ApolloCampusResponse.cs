using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ApolloCampusResponse : BaseMatchResponse
    {
        [DataMember]
        public List<ApolloProductGroupResponse> ProductGroupResponseList { get; set; }

        [DataMember]
        public List<int> SubjectsFromTextSearch { get; set; }

        public ApolloCampusResponse()
        {
            ProductGroupResponseList = new List<ApolloProductGroupResponse>();
            SubjectsFromTextSearch = new List<int>();
        }
    }

    [DataContract]
    public class ApolloProductGroupResponse
    {
        [DataMember]
        public string groupIdentifier { get; set; }

        [DataMember]
        public List<ApolloCampus> OnlineCampusList { get; set; }

        [DataMember]
        public List<ApolloCampus> GroundCampusList { get; set; }
    }
}
