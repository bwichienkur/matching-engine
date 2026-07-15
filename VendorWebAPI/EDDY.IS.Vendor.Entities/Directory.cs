namespace EDDY.IS.Vendor.Entities
{
    public class Directory
    {
        private int applicationId;

        private string baseURL;
        private int directoryId;
        private string directoryName;
        public int ApplicationId
        {
            get
            {
                return applicationId;
            }

            set
            {
                applicationId = value;
            }
        }
        public string BaseURL
        {
            get
            {
                return baseURL;
            }

            set
            {
                baseURL = value;
            }
        }
        public int DirectoryId
        {
            get
            {
                return directoryId;
            }

            set
            {
                directoryId = value;
            }
        }

        public string DirectoryName
        {
            get
            {
                return directoryName;
            }

            set
            {
                directoryName = value;
            }
        }


    }
}
