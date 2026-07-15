using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CategoryContent
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public VW_Matching_CategoryContent(IDataReader dr)
        {
            CategoryId = System.Convert.ToInt32(dr["CategoryId"]);
            CategoryName = System.Convert.ToString(dr["CategoryName"]);
        }
    }
}
