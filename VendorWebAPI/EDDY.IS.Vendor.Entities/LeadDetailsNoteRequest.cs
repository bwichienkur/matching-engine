namespace EDDY.IS.Vendor.Entities
{
    public class LeadDetailsNoteRequest : LeadDetailsRequest
    {
        private int advisorId;
        private string note;
        public int AdvisorId
        {
            get
            {
                return advisorId;
            }

            set
            {
                advisorId = value;
            }
        }
        public string Note
        {
            get
            {
                return note;
            }

            set
            {
                note = value;
            }
        }
    }
}
