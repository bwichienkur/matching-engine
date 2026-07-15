using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramProductRPL
    {
        public int ProgramProductId { get; set; }
        public decimal RPL { get; set; }
        public decimal ClickPrice { get; set; }
        public System.DateTime ExpirationDate { get; set; }

        public VW_Matching_ProgramProductRPL(IDataReader dr)
        {
            ProgramProductId = System.Convert.ToInt32(dr["ProgramProductId"]);
            RPL = System.Convert.ToDecimal(dr["RPL"]);
            ClickPrice = System.Convert.ToDecimal(dr["ClickPrice"]);
            ExpirationDate = System.Convert.ToDateTime(dr["ExpirationDate"]);
        }
    }
}
