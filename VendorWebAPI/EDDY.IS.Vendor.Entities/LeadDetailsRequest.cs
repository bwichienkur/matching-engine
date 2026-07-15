namespace EDDY.IS.Vendor.Entities
{
    public class LeadDetailsRequest : VendorRequestBase
    {
        private int prospectFlowId;
        private string advisorEmail;
        public int ProspectFlowId
        {
            get
            {
                return prospectFlowId;
            }

            set
            {
                prospectFlowId = value;
            }
        }
        public string AdvisorEmail
        {
            get
            {
                return advisorEmail;
            }

            set
            {
                advisorEmail = value;
            }
        }
    }
}
