using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace EDDY.IS.Vendor.Entities
{
    public class Subject
    {

        private int categoryId;
        private int subjectId;
        private string subjectName;
        private string directoryURL;
        private string formURL;
        private bool isEnabled;

        [JsonIgnore]
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
        public int SubjectId
        {
            get
            {
                return subjectId;
            }

            set
            {
                subjectId = value;
            }
        }

        public string SubjectName
        {
            get
            {
                return subjectName;
            }

            set
            {
                subjectName = value;
            }
        }

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

        //public string FormURL
        //{
        //    get
        //    {
        //        return formURL;
        //    }

        //    set
        //    {
        //        formURL = value;
        //    }
        //}
    }
}
