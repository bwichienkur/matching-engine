namespace EDDY.IS.Vendor.Entities
{
    public class ProspectStatus
    {
        private int prospectStatusId;
        private string code;
        private string name;
        private string description;
        public int ProspectStatusId
        {
            get
            {
                return prospectStatusId;
            }

            set
            {
                prospectStatusId = value;
            }
        }
       

        public string Code {
            get
            {
                return code;
            }

            set
            {
                code = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }
    }
}
