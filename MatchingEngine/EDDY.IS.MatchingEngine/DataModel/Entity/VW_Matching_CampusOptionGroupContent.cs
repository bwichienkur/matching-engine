using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
	public class VW_Matching_CampusOptionGroupContent
	{
		public int CampusOptionGroupId { get; set; }
		public string CampusOptionGroupName { get; set; }
		public int Position { get; set; }

		public VW_Matching_CampusOptionGroupContent(IDataReader dr)
		{
			CampusOptionGroupId = System.Convert.ToInt32(dr["CampusOptionGroupId"]);
			CampusOptionGroupName = System.Convert.ToString(dr["Name"]);
			Position = System.Convert.ToInt32(dr["Position"]);
		}
	}
}
