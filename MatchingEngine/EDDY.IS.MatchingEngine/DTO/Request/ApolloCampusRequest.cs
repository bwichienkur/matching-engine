using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ApolloCampusRequest : BaseMatchRequest
    {
        [DataMember]
        public List<ProductGroupRequest> ProductGroupList { get; set; }

        [DataMember]
        public string SearchTerm { get; set; }

        [DataMember]
        public int? MaxNestedProgramCount { get; set; }

        [DataMember]
        public int? MaxCampusCountPerGrouping { get; set; }

        [DataMember]
        public string ExpectedStartDateCode { get; set; }

        [DataMember]
        public int? ExpectedStartDateValue { get; set; }

        [DataMember]
        public int UserId { get; set; }
    }

    [DataContract]
    public class ProductGroupRequest
    {
        [DataMember]
        public string ProductGroupIdentifier { get; set; }

        [DataMember]
        public List<int> Products { get; set; }
    }
}
