using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ClientRelationshipEntitySchedule
    {
        public int ClientRelationProductAutomationId { get; set; }
        public int EntityMetaId { get; set; }
        public string EntityValue { get; set; }
        public int Priority { get; set; }
        public int WeekDay { get; set; }
        public TimeSpan TurnOnTime { get; set; }
        public TimeSpan TurnOffTime { get; set; }
        public bool IsOvernightSchedule { get; set; }

        public VW_Matching_ClientRelationshipEntitySchedule(IDataReader dr)
        {
            ClientRelationProductAutomationId = System.Convert.ToInt32(dr["ClientRelationProductAutomationId"]);
            EntityMetaId = System.Convert.ToInt32(dr["EntityMetaId"]);
            EntityValue = System.Convert.ToString(dr["EntityValue"]);
            Priority = System.Convert.ToInt32(dr["Priority"]);
            WeekDay = System.Convert.ToInt32(dr["WeekDay"]);
            TurnOnTime = TimeSpan.Parse((dr["TurnOnTime"].ToString()));
            TurnOffTime = TimeSpan.Parse((dr["TurnOffTime"].ToString()));
            IsOvernightSchedule = System.Convert.ToBoolean(dr["IsOvernightSchedule"]);
        }
    }
}
