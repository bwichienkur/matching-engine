using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_KVCodeDataCache
    {
        public int KVCodeDataId { get; set; }
        public int KVCodeId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Nullable<decimal> StartRange { get; set; }
        public Nullable<decimal> EndRange { get; set; }

        public VW_Matching_KVCodeDataCache(IDataReader dr)
        {
            KVCodeDataId = System.Convert.ToInt32(dr["KVCodeDataId"]);
            KVCodeId = System.Convert.ToInt32(dr["KVCodeId"]);
            Key = System.Convert.ToString(dr["Key"]);
            Value = System.Convert.ToString(dr["Value"]);

            if (!dr.IsDBNull(dr.GetOrdinal("StartRange")))
                StartRange = System.Convert.ToDecimal(dr["StartRange"]);

            if (!dr.IsDBNull(dr.GetOrdinal("EndRange")))
                EndRange = System.Convert.ToDecimal(dr["EndRange"]);
        }
    }
}
