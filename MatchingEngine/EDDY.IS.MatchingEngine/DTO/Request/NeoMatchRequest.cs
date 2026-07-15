using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
	[DataContract]
	public class NeoMatchRequest : BaseMatchRequest
	{
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
        public bool? HasComputer { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string DialerKey { get; set; }
        
        [DataMember]
        public string TSR { get; set; }

    }
}
