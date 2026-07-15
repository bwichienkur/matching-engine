namespace EDDY.IS.Vendor.Entities
{
    public class EduMaxLeadSubmissionResponse : LeadSubmissionResponse
    {
        private string matchId;
        public string MatchId
        {
            get
            {
                return matchId;
            }

            set
            {
                matchId = value;
            }
        }

        private int leadId;
        public int LeadId
        {
            get
            {
                return leadId;
            }

            set
            {
                leadId = value;
            }
        }
    }
}
