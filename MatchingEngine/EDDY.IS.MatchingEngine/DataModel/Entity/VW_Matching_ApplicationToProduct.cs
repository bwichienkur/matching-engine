using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ApplicationToProduct
    {
        public int ApplicationId { get; set; }
        public int ProductId { get; set; }

        public VW_Matching_ApplicationToProduct(IDataReader dr)
        {
            ProductId = System.Convert.ToInt32(dr["ProductId"]);
            ApplicationId = System.Convert.ToInt32(dr["ApplicationId"]);
        }
    }
}
