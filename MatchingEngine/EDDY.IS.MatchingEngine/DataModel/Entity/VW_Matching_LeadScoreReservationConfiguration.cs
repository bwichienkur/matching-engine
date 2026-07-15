using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_LeadScoreReservationConfiguration
    {
        public int LeadScoreReservationId { get; set; }
        public string ReservationName { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int DayBegin { get; set; }
        public int DayEnd { get; set; }
        public long CampaignId { get; set; }
        public int VendorId { get; set; }
        public int ChannelId { get; set; }
        public int MarketingUnitId { get; set; }
        public int SubChannelId { get; set; }
        public int ApplicationId { get; set; }

        public VW_Matching_LeadScoreReservationConfiguration(IDataReader dr)
        {
            LeadScoreReservationId = System.Convert.ToInt32(dr["LeadScoreReservationId"]);
            ReservationName = System.Convert.ToString(dr["ReservationName"]);
            StartDate = System.Convert.ToDateTime(dr["StartDate"]);
            EndDate = System.Convert.ToDateTime(dr["EndDate"]);
            DayBegin = System.Convert.ToInt32(dr["DayBegin"]);
            DayEnd = System.Convert.ToInt32(dr["DayEnd"]);
            CampaignId = System.Convert.ToInt64(dr["CampaignId"]);
            VendorId = System.Convert.ToInt32(dr["VendorId"]);
            ChannelId = System.Convert.ToInt32(dr["ChannelId"]);
            MarketingUnitId = System.Convert.ToInt32(dr["MarketingUnitId"]);
            SubChannelId = System.Convert.ToInt32(dr["SubChannelId"]);
            ApplicationId = System.Convert.ToInt32(dr["ApplicationId"]);
        }
    }
}
