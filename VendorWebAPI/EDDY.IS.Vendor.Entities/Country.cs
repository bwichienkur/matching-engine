using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace EDDY.IS.Vendor.Entities
{
    public class Country
    {

        
        private int countryId;
        private string name;
        private string countryCode;
        [JsonIgnore]
        public int CountryId
        {
            get
            {
                return countryId;
            }

            set
            {
                countryId = value;
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

        public string CountryCode
        {
            get
            {
                return countryCode;
            }

            set
            {
                countryCode = value;
            }
        }

    }
}
