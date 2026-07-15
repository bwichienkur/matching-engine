using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramLogo
    {
        public int ProgramId { get; set; }

        //public bool HasLogo { get; set; }

        public string FileURL { get; set; }

        public int CacheBuster { get; set; }

        public string LogoURL { get; set; }

        public VW_Matching_ProgramLogo(IDataReader dr)
        {
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);

            FileURL = System.Convert.ToString(dr["FileURL"]);
            if (!dr.IsDBNull(dr.GetOrdinal("CacheBuster")))
                CacheBuster = System.Convert.ToInt32(dr["CacheBuster"]);
            else
                CacheBuster = 0;
            LogoURL = FormatLogoURL(FileURL, CacheBuster);

            //if (!dr.IsDBNull(dr.GetOrdinal("HasProgramLogo")))
            //    HasLogo = (bool)dr.GetValue(dr.GetOrdinal("HasProgramLogo"));
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
