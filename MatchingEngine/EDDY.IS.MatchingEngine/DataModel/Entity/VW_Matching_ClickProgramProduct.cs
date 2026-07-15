using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ClickProgramProduct
    {
        public int ProgramProductId { get; set; }
        public int ProgramId { get; set; }
        public string ClickThroughURL { get; set; }

        public VW_Matching_ClickProgramProduct(IDataReader dr)
        {
            ProgramProductId = System.Convert.ToInt32(dr["ProgramProductId"]);
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            ClickThroughURL = System.Convert.ToString(dr["ClickThroughURL"]);
        }
    }
}