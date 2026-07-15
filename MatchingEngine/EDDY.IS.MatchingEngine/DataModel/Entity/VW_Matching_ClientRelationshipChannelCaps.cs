using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ClientRelationshipChannelCaps
    {
        public int ClientRelationshipId { get; set; }
        public int ChannelId { get; set; }
        public int CapAmount { get; set; }
        public DateTime Date { get; set; }
        public int LeadCount { get; set; }

        public VW_Matching_ClientRelationshipChannelCaps(IDataReader dr)
        {
            ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);
            ChannelId = System.Convert.ToInt32(dr["ChannelId"]);
            CapAmount = System.Convert.ToInt32(dr["CapAmount"]);
            Date = System.Convert.ToDateTime(dr["Date"]);
            LeadCount = System.Convert.ToInt32(dr["LeadCount"]);
        }
    }
}
