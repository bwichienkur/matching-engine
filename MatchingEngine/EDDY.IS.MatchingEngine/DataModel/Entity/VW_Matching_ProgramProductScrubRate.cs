using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramProductScrubRate
    {
        public int ProgramProductId { get; set; }
        public decimal ScrubRate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }

        public VW_Matching_ProgramProductScrubRate(IDataReader dr)
        {
            ProgramProductId = System.Convert.ToInt32(dr["ProgramProductId"]);
            ScrubRate = System.Convert.ToDecimal(dr["ScrubRate"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ExpirationDate")))
                ExpirationDate = System.Convert.ToDateTime(dr["ExpirationDate"]);
        }
    }
}
