using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CampaignDetailedLists
    {
        public long CampaignId { get; set; }
        public long Include { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }

        public VW_Matching_CampaignDetailedLists(IDataReader dr)
        {
            CampaignId = System.Convert.ToInt64(dr["CampaignId"]);
            Include = System.Convert.ToInt64(dr["Include"]);
            EntityType = System.Convert.ToString(dr["EntityType"]);
            EntityId = System.Convert.ToInt32(dr["EntityId"]);
        }
    }
}
