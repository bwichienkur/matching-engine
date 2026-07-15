using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CountryCache
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public int CountryId { get; set; }

        public VW_Matching_CountryCache(IDataReader dr)
        {
            EntityId = System.Convert.ToInt32(dr["EntityId"]);
            EntityType = System.Convert.ToString(dr["EntityType"]);
            CountryId = System.Convert.ToInt32(dr["CountryId"]);
        }
    }
}
