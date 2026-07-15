namespace EDDY.IS.Vendor.Entities
{
    public class LeadDetailsNoteSubmissionResponse 
    {
        private int prospectNoteId;
        private int prospectFlowId;
        public int ProspectNoteId
        {
            get
            {
                return prospectNoteId;
            }

            set
            {
                prospectNoteId = value;
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
