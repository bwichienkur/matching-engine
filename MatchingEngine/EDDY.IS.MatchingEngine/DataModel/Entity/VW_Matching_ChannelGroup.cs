using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ChannelGroup
    {
        public int ChannelGroupId { get; set; }
        public int ChannelId { get; set; }

        public VW_Matching_ChannelGroup(IDataReader dr)
        {
            ChannelGroupId = System.Convert.ToInt32(dr["ChannelGroupId"]);
            ChannelId = System.Convert.ToInt32(dr["ChannelId"]);
        }
    }
}
