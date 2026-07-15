using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_LeadScoreReservationTierLevel
    {
        public int LeadScoreReservationId { get; set; }
        public int LeadScoreReservationSupplyUnitId { get; set; }
        public int LeadScoringTierLevel { get; set; }

        public VW_Matching_LeadScoreReservationTierLevel(IDataReader dr)
        {
            LeadScoreReservationId = System.Convert.ToInt32(dr["LeadScoreReservationId"]);
            LeadScoreReservationSupplyUnitId = System.Convert.ToInt32(dr["LeadScoreReservationSupplyUnitId"]);
            LeadScoringTierLevel = System.Convert.ToInt32(dr["LeadScoringTierLevel"]);
        }

    }
}
