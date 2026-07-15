using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CampusCallCenterHours
    {
        public int ClientCampusRelationshipId { get; set; }
        public int Day { get; set; }
        public string TimeZone { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }

        public VW_Matching_CampusCallCenterHours(IDataReader dr)
        {
            ClientCampusRelationshipId = System.Convert.ToInt32(dr["ClientCampusRelationshipId"]);
            Day = System.Convert.ToInt32(dr["Day"]);
            TimeZone = System.Convert.ToString(dr["TimeZone"]);
            StartHour = System.Convert.ToInt32(dr["StartHour"]);
            StartMinute = System.Convert.ToInt32(dr["StartMinute"]);
            EndHour = System.Convert.ToInt32(dr["EndHour"]);
            EndMinute = System.Convert.ToInt32(dr["EndMinute"]);
        }
    }
}
