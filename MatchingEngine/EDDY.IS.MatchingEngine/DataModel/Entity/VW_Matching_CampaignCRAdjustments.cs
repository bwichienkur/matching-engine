using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CampaignCRAdjustments
    {
        public long CampaignId { get; set; }
        public int ProductId { get; set; }
        public int ClientRelationshipId { get; set; }
        public bool Include { get; set; }

        public VW_Matching_CampaignCRAdjustments(IDataReader dr)
        {
            CampaignId = System.Convert.ToInt64(dr["CampaignId"]);
            ProductId = System.Convert.ToInt32(dr["ProductId"]);
            ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);
            Include = System.Convert.ToBoolean(dr["Include"]);
        }
    }

    public class VW_Matching_CampaignCRPSIAdjustments
    {
        public long CampaignId { get; set; }
        public int PSIId { get; set; }
        public int ClientRelationshipId { get; set; }

        public VW_Matching_CampaignCRPSIAdjustments(IDataReader dr)
        {
            CampaignId = System.Convert.ToInt64(dr["CampaignId"]);
            PSIId = System.Convert.ToInt32(dr["PSIId"]);
            ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);
        }
    }
}
