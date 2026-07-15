using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace EDDY.IS.Vendor.Entities
{
    [DataContract]
    public class Institution : ProgramInstitution
    {
        
       // private string address;
        //private List<Advisor> advisors;
        
        //private string city;
        //private Country country;
      
       // private string institutionCode;

        private string institutionFormURL;
        private bool isEnabled;


        private State state;
        private string postalCode;



        //public string Address
        //{
        //    get
        //    {
        //        return address;
        //    }

        //    set
        //    {
        //        address = value;
        //    }
        //}

        //public List<VendorAPIPOC.Advisor> Advisors
        //{
        //    get
        //    {
        //        return advisors;
        //    }

        //    set
        //    {
        //        advisors = value;
        //    }
        //}



        //public string City
        //{
        //    get
        //    {
        //        return city;
        //    }

        //    set
        //    {
        //        city = value;
        //    }
        //}

        //public Country Country
        //{
        //    get
        //    {
        //        return country;
        //    }

        //    set
        //    {
        //        country = value;
        //    }
        //}



        //public string InstitutionCode
        //{
        //    get
        //    {
        //        return institutionCode;
        //    }

        //    set
        //    {
        //        institutionCode = value;
        //    }
        //}


        [DataMember(Order = 4)]
        public string InstitutionFormURL
        {
            get
            {
                return institutionFormURL;
            }

            set
            {
                institutionFormURL = value;
            }
        }

        [JsonIgnore]
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
        


        //public State State
        //{
        //    get
        //    {
        //        return state;
        //    }

        //    set
        //    {
        //        state = value;
        //    }
        //}

        //public string PostalCode
        //{
        //    get
        //    {
        //        return postalCode;
        //    }

        //    set
        //    {
        //        postalCode = value;
        //    }
        //}
    }
}
