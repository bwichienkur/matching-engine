using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
	public class VW_Matching_ClientRelationContact
	{
		public int ClientRelationContactId { get; set; }
		public int ClientRelationshipId { get; set; }
		public int ContactId { get; set; }
		public string ContactName { get; set; }
		public string Email { get; set; }

		public VW_Matching_ClientRelationContact(IDataReader dr)
		{
			ClientRelationContactId = System.Convert.ToInt32(dr["ClientRelationContactId"]);
			ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);
			ContactId = System.Convert.ToInt32(dr["ContactId"]);
			ContactName = System.Convert.ToString(dr["ContactName"]);
			Email = System.Convert.ToString(dr["Email"]);
		}
	}
}
