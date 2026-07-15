
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace EDDY.IS.Vendor.Entities
{
    public class State
    {
        private int stateId;
        private string name;
        private string stateCode;
        [JsonIgnore]
        public int StateId
        {
            get
            {
                return stateId;
            }

            set
            {
                stateId = value;
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

        private int countryId;

        [IgnoreDataMember]
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
    }
}
