using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ZipCodeCache
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public int ZipCodeId { get; set; }
        public string ListType { get; set; }

        public VW_Matching_ZipCodeCache(IDataReader dr)
        {
            EntityId = System.Convert.ToInt32(dr["EntityId"]);
            EntityType = System.Convert.ToString(dr["EntityType"]);
            ZipCodeId = System.Convert.ToInt32(dr["ZipCodeId"]);
            ListType = System.Convert.ToString(dr["ListType"]);
        }
    }
}
