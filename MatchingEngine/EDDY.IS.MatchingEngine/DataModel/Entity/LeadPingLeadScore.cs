using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class LeadPingLeadScore
    {
        public int LeadPingLeadScoreId { get; set; }
        public string LeadScoreValue { get; set; }
        public int? ProductId { get; set; }
        public int? InstitutionId { get; set; }
        public LeadPingLeadScore() { }
        public LeadPingLeadScore(IDataReader dr)
        {
            LeadPingLeadScoreId = System.Convert.ToInt32(dr["LeadPingLeadScoreId"]);

            LeadScoreValue = System.Convert.ToString(dr["LeadScoreValue"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProductId")))
                ProductId = System.Convert.ToInt32(dr["ProductId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("InstitutionId")))
                InstitutionId = System.Convert.ToInt32(dr["InstitutionId"]);

        }
    }
}
