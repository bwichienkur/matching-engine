using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.Vendor.Entities
{
    public class Category
    {
   
        private int categoryId;
        private string categoryName;
        private string directoryURL;
        private string formURL;
        private bool isEnabled;


        [DataMember(Order = 0)]
        public int CategoryId
        {
            get
            {
                return categoryId;
            }

            set
            {
                categoryId = value;
            }
        }
        [DataMember(Order = 1)]
        public string CategoryName
        {
            get
            {
                return categoryName;
            }

            set
            {
                categoryName = value;
            }
        }

        [DataMember(Order = 2)]
        public string DirectoryURL
        {
            get
            {
                return directoryURL;
            }

            set
            {
                directoryURL = value;
            }
        }

        [DataMember(Order = 3)]
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }


    }
}
