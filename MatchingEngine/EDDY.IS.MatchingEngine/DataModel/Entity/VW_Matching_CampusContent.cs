using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CampusContent
    {
        public int CampusId { get; set; }
        public string CampusName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        //public Nullable<bool> HasLogo { get; set; }
        public string FileURL { get; set; }
        public int CacheBuster { get; set; }
        public string LogoURL { get; set; }
        public Nullable<int> CountryId { get; set; }
        public Nullable<int> StateId { get; set; }
        public Nullable<int> CityId { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public VW_Matching_CampusContent(IDataReader dr)
        {
            CampusId = System.Convert.ToInt32(dr["CampusId"]);
            CampusName = System.Convert.ToString(dr["CampusName"]);
            Address1 = System.Convert.ToString(dr["Address1"]);
            Address2 = System.Convert.ToString(dr["Address2"]);
            City = System.Convert.ToString(dr["City"]);
            StateProvinceCode = System.Convert.ToString(dr["StateProvinceCode"]);
            PostalCode = System.Convert.ToString(dr["PostalCode"]);
            Phone = System.Convert.ToString(dr["Phone"]);
            Fax = System.Convert.ToString(dr["Fax"]);
            CountryCode = System.Convert.ToString(dr["CountryCode"]);
            CountryName = System.Convert.ToString(dr["CountryName"]);

            FileURL = System.Convert.ToString(dr["FileURL"]);
            if (!dr.IsDBNull(dr.GetOrdinal("CacheBuster")))
                CacheBuster = System.Convert.ToInt32(dr["CacheBuster"]);
            else
                CacheBuster = 0;
            LogoURL = FormatLogoURL(FileURL, CacheBuster);


            //if (!dr.IsDBNull(dr.GetOrdinal("HasLogo")))
            //    HasLogo = System.Convert.ToBoolean(dr["HasLogo"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CountryId")))
                CountryId = System.Convert.ToInt32(dr["CountryId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("StateId")))
                StateId = System.Convert.ToInt32(dr["StateId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CityId")))
                CityId = System.Convert.ToInt32(dr["CityId"]);
        }

        private string FormatLogoURL(string fileUrl, int cacheBuster)
        {
            string fileName;
            string replaceString;
            string logoURL;

            // If the file url is empty then just return empty string
            if (fileUrl == "")
            {
                return "";
            }
            // Get the file name 
            fileName = Path.GetFileName(fileUrl);
            // Build replacement string with the cache buster
            replaceString = "{FILENAME}?" + cacheBuster;
            // Generate the cache buster string
            logoURL = fileUrl.Replace(fileName, replaceString);

            return logoURL;
        }
    }
}
