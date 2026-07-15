namespace EDDY.IS.Vendor.Entities
{
    public class ProspectSubmissionResponse
    {
        private int prospectId;
        private int prospectFlowId;
        public int ProspectId
        {
            get
            {
                return prospectId;
            }

            set
            {
                prospectId = value;
            }
        }

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
    }
}
