using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramProductPriceByState
    {
        public int ProgramProductId { get; set; }
        public int StateId { get; set; }
        public decimal RPL { get; set; }
        public decimal ClickPrice { get; set; }
        public decimal ScrubRate { get; set; }        

        public VW_Matching_ProgramProductPriceByState(IDataReader dr)
        {
            ProgramProductId = System.Convert.ToInt32(dr["ProgramProductId"]);
            StateId = System.Convert.ToInt32(dr["StateId"]);
            RPL = System.Convert.ToDecimal(dr["RPL"]);
            ClickPrice = System.Convert.ToDecimal(dr["ClickPrice"]);
            ScrubRate = System.Convert.ToDecimal(dr["ScrubRate"]);
        }
    }
}
