using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_Feature
    {
        public int FeatureId { get; set; }
        public string FeatureCode { get; set; }
        public int FeatureTypeId { get; set; }
        public int EntityId { get; set; }

        public VW_Matching_Feature(IDataReader dr)
        {
            FeatureId = System.Convert.ToInt32(dr["FeatureId"]);
            FeatureCode = System.Convert.ToString(dr["FeatureCode"]);
            FeatureTypeId = System.Convert.ToInt32(dr["FeatureTypeId"]);
            EntityId = System.Convert.ToInt32(dr["EntityId"]);
        }
    }
}
