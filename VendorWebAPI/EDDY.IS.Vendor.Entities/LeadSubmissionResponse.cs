namespace EDDY.IS.Vendor.Entities
{
    public class LeadSubmissionResponse 
    {
        private string uid;
        private int leadTier;
        private long leadId;
        private decimal estimatedRevShare;
        public string UID
        {
            get
            {
                return uid;
            }

            set
            {
                uid = value;
            }
        }

        public int LeadTier
        {
            get
            {
                return leadTier;
            }

            set
            {
                leadTier = value;
            }
        }

        public long LeadId
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
        public decimal EstimatedRevShare
        {
            get
            {
                return estimatedRevShare;
            }

            set
            {
                estimatedRevShare = value;
            }
        }
    }
}
