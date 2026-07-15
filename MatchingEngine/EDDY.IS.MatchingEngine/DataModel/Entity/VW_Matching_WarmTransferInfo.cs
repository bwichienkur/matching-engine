using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_WarmTransferInfo
    {
        public int ClientRelationshipId { get; set; }
        public Nullable<int> WarmTransferTimeDelay { get; set; }
        public Nullable<int> WarmTransferDailyGoal { get; set; }
        public Nullable<int> FixedWarmTransferRank { get; set; }
        public Nullable<System.DateTime> LastWarmTransferTime { get; set; }
        public Nullable<int> WarmTransferCount { get; set; }

        public VW_Matching_WarmTransferInfo(IDataReader dr)
        {
            ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("WarmTransferTimeDelay")))
                WarmTransferTimeDelay = System.Convert.ToInt32(dr["WarmTransferTimeDelay"]);

            if (!dr.IsDBNull(dr.GetOrdinal("WarmTransferDailyGoal")))
                WarmTransferDailyGoal = System.Convert.ToInt32(dr["WarmTransferDailyGoal"]);

            if (!dr.IsDBNull(dr.GetOrdinal("FixedWarmTransferRank")))
                FixedWarmTransferRank = System.Convert.ToInt32(dr["FixedWarmTransferRank"]);

            if (!dr.IsDBNull(dr.GetOrdinal("LastWarmTransferTime")))
                LastWarmTransferTime = System.Convert.ToDateTime(dr["LastWarmTransferTime"]);

            if (!dr.IsDBNull(dr.GetOrdinal("WarmTransferCount")))
                WarmTransferCount = System.Convert.ToInt32(dr["WarmTransferCount"]);
        }
    }
}
