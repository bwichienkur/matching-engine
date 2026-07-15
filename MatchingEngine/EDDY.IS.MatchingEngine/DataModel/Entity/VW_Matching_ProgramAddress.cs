using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramAddress
    {

        public int ProgramId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }

        public VW_Matching_ProgramAddress(IDataReader dr)
        {
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            Address1 = System.Convert.ToString(dr["address1"]);
            Address2 = System.Convert.ToString(dr["address2"]);
            City = System.Convert.ToString(dr["city"]);
            StateProvinceCode = System.Convert.ToString(dr["StateProvinceCode"]);
            PostalCode = System.Convert.ToString(dr["PostalCode"]);
            CountryCode = System.Convert.ToString(dr["CountryCode"]);
            CountryName = System.Convert.ToString(dr["CountryName"]);
            Phone = System.Convert.ToString(dr["Phone"]);
            Fax = System.Convert.ToString(dr["Fax"]);
            CountryId = System.Convert.ToInt32(dr["CountryId"]);
            CityId = System.Convert.ToInt32(dr["CityId"]);
            StateId = System.Convert.ToInt32(dr["StateId"]);
        }
    }
}
