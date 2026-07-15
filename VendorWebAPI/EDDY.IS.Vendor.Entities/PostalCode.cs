namespace EDDY.IS.Vendor.Entities
{
    public class PostalCode
    {
        private string postalCode;
        private string stateCode;

        public string PostalCodeString
        {
            get
            {
                return postalCode;
            }

            set
            {
                postalCode = value;
            }
        }

        public string StateCode
        {
            get
            {
                return stateCode;
            }

            set
            {
                stateCode = value;
            }
        }
    }
}
