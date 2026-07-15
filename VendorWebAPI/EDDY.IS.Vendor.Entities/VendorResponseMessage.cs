namespace EDDY.IS.Vendor.Entities
{
    public class VendorResponseMessage
    {
        private string messageCode;

        private string message;

        public string MessageCode
        {
            get
            {
                return messageCode;
            }

            set
            {
                messageCode = value;
            }
        }
        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }
    }
}
