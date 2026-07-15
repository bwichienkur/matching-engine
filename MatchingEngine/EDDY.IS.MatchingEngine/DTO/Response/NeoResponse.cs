using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
	[DataContract]
	public class NeoResponse : BaseMatchResponse
	{
		[DataMember]
		public List<int> SubjectsFromTextSearch { get; set; }

		[DataMember]
		public List<NeoCampus> LiveTransferList { get; set; }

		[DataMember]
		public List<NeoCampus> FormList { get; set; }
	}
}
