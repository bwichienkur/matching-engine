namespace EDDY.IS.Vendor.Entities
{
    public class LeadDispositionHistory
    {
        private string createdDate;
        private string disposition;
        private string advisor;
        private string note;
        public string CreatedDate
        {
            get
            {
                return createdDate;
            }

            set
            {
                createdDate = value;
            }
        }
        public string Disposition
        {
            get
            {
                return disposition;
            }

            set
            {
                disposition = value;
            }
        }
        public string Advisor
        {
            get
            {
                return advisor;
            }

            set
            {
                advisor = value;
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
