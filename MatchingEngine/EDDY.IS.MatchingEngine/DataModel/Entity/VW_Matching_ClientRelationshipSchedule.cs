using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ClientRelationshipSchedule
    {
        public int ClientRelationshipId { get; set; }
        public int ClientRelationProductAutomationId { get; set; }

        public int? ProductId { get; set; }
        public int WeekDay { get; set; }
        public TimeSpan TurnOnTime { get; set; }
        public TimeSpan TurnOffTime { get; set; }
        public bool IsOvernightSchedule { get; set; }

        public VW_Matching_ClientRelationshipSchedule()
        {

        }

        public VW_Matching_ClientRelationshipSchedule(IDataReader dr)
        {
            ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);
            ClientRelationProductAutomationId = System.Convert.ToInt32(dr["ClientRelationProductAutomationId"]);
            WeekDay = System.Convert.ToInt32(dr["WeekDay"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProductId")))
                ProductId = System.Convert.ToInt32(dr["ProductId"]);

            TurnOnTime = TimeSpan.Parse((dr["TurnOnTime"].ToString()));
            TurnOffTime = TimeSpan.Parse((dr["TurnOffTime"].ToString()));
            IsOvernightSchedule = System.Convert.ToBoolean(dr["IsOvernightSchedule"]);
        }
    }
}
