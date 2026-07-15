using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME_Troubleshoot
{
    public class Record
    {
        public string MarketingUnit { get; set; }
        public string CampaignName { get; set;  }
        public string TrackId { get; set; }
        public int ProgramProductId { get; set; }
        public string MatchResponseGuid { get; set; }
        public string MatchRequest { get; set; }
        public int LeadId { get; set; }
        public string FailureReason { get; set; }
    }
}
