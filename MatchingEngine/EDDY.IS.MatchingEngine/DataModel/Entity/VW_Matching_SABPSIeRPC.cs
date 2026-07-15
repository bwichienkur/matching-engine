using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_SABPSIeRPC
    {
        public int PsiId { get; set; }
        public decimal eRPC { get; set; }

        public VW_Matching_SABPSIeRPC(IDataReader dr)
        {
            PsiId = System.Convert.ToInt32(dr["PsiId"]);

            eRPC = System.Convert.ToDecimal(dr["eRPC"]);
        }
    }
}
