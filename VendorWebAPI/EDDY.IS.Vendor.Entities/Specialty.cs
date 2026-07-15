using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EDDY.IS.Vendor.Entities
{
    public class Specialty
    {
        
        private int subjectId;
        private int specialtyId;
        private string specialtyName;
        private string directoryURL;
        private string formURL;
        private bool isEnabled;

        [JsonIgnore]
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
        public int SpecialtyId
        {
            get
            {
                return specialtyId;
            }

            set
            {
                specialtyId = value;
            }
        }

        public string SpecialtyName
        {
            get
            {
                return specialtyName;
            }

            set
            {
                specialtyName = value;
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
    }
}
