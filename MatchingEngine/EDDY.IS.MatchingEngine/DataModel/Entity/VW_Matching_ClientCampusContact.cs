using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
	public class VW_Matching_ClientCampusContact
	{
		public int ClientCampusContactId { get; set; }
		public int ClientCampusRelationshipId { get; set; }
		public int ContactId { get; set; }
		public string ContactName { get; set; }
		public string Email { get; set; }

		public VW_Matching_ClientCampusContact(IDataReader dr)
		{
			ClientCampusContactId = System.Convert.ToInt32(dr["ClientCampusContactId"]);
			ClientCampusRelationshipId = System.Convert.ToInt32(dr["ClientCampusRelationshipId"]);
			ContactId = System.Convert.ToInt32(dr["ContactId"]);
			ContactName = System.Convert.ToString(dr["ContactName"]);
			Email = System.Convert.ToString(dr["Email"]);
		}
	}
}
