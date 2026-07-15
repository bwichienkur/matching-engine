using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace EDDY.IS.Vendor.Entities
{
    [DataContract]
    public class CampusBase
    {
        private string address;
        private string city;
        private Country country;
        private int campusId;
        private string campusName;
        private State state;
        private string campusType;
        private List<string> excludedZips;
        private List<string> includedZips;
        private bool isEnabled;
     
      
        //private string logoURL;
        private string postalCode;
        [DataMember(Order = 3)]
        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
            }
        }
        [DataMember(Order = 4)]
        public string City
        {
            get
            {
                return city;
            }

            set
            {
                city = value;
            }
        }
        [DataMember(Order = 5)]
        public Country Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
            }
        }
        [DataMember(Order = 0)]
        public int CampusId
        {
            get
            {
                return campusId;
            }

            set
            {
                campusId = value;
            }
        }
        [DataMember(Order = 1)]
        public string CampusName
        {
            get
            {
                return campusName;
            }

            set
            {
                campusName = value;
            }
        }
        [DataMember(Order = 6)]
        public State State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
            }
        }
        [DataMember(Order = 2)]
        public string CampusType
        {
            get
            {
                return campusType;
            }

            set
            {
                campusType = value;
            }
        }
        [DataMember(Order = 7)]
        public string PostalCode
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
        [JsonIgnore]
        [DataMember(Order = 8)]
        public List<string> ExcludedZips
        {
            get
            {
                return excludedZips;
            }

            set
            {
                excludedZips = value;
            }
        }
        [JsonIgnore]
        [DataMember(Order = 9)]
        public List<string> IncludedZips
        {
            get
            {
                return includedZips;
            }
            set
            {
                includedZips = value;
            }
        }

        [JsonIgnore]
        [DataMember(Order = 10)]
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

      

        //public string LogoURL
        //{
        //    get
        //    {
        //        return logoURL;
        //    }

        //    set
        //    {
        //        logoURL = value;
        //    }
        //}
    }
}
