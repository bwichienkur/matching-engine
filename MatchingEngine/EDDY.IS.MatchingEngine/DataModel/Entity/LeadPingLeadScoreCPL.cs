using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class LeadPingLeadScoreCPL
    {
        public int LeadPingLeadScoreCPLId { get; set; }
        public int LeadPingLeadScoreId { get; set; }

        public int ProductId { get; set; }
        public int InstitutionId { get; set; }
        public int CampusTypeId { get; set; }
        public decimal CPL { get; set; }
        public LeadPingLeadScoreCPL() { }
        public LeadPingLeadScoreCPL(IDataReader dr)
        {
            LeadPingLeadScoreCPLId = System.Convert.ToInt32(dr["LeadPingLeadScoreCPLId"]);
            LeadPingLeadScoreId = System.Convert.ToInt32(dr["LeadPingLeadScoreId"]);
            ProductId = System.Convert.ToInt32(dr["ProductId"]);
            InstitutionId = System.Convert.ToInt32(dr["InstitutionId"]);
            CampusTypeId = System.Convert.ToInt32(dr["CampusTypeId"]);
            CPL = System.Convert.ToDecimal(dr["CPL"]);
        }
    }
}
