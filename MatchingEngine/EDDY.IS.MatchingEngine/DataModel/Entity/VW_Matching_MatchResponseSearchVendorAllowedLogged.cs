using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
	public class VW_Matching_MatchResponseSearchVendorAllowedLogged
	{
		public int VendorId;

		public VW_Matching_MatchResponseSearchVendorAllowedLogged(IDataReader dr)
		{
			VendorId = Convert.ToInt32(dr["VendorId"]);
		}
	}
}
