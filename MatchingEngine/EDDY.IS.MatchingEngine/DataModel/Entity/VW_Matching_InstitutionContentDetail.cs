using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_InstitutionContentDetail
    {
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string Description { get; set; }
        public string InternationalDescription { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string CarnegieClassificationName { get; set; }
        public string CalendarTypeOnline { get; set; }
        public string LearningFormatOnline { get; set; }
        public string GeographicComments { get; set; }
        //public string InstitutionType { get; set; }
        public string InstitutionalAccreditationType { get; set; }
        public string LoansOffered { get; set; }
        public string OnCampusRequirements { get; set; }
        public string ScholarshipOffered { get; set; }
        public string RegistrationDetails { get; set; }
        public string ComputerRequirement { get; set; }
        public string TotalEnrollment { get; set; }
        public string InstitutionDisclaimerType { get; set; }
        public string InstitutionDisclaimer { get; set; }
        public bool ThirdPartyContactAllowed { get; set; }
        public string Locale { get; set; }
        public string AcademicYearBeginMonth { get; set; }
        public string AcademicYearBeginDay { get; set; }
        public string AccreditationOrganization { get; set; }
        public bool IsFaithBased { get; set; }
        public bool HasAdditionalScript { get; set; }
        public string FileURL { get; set; }
        public int CacheBuster { get; set; }
        public string LogoURL { get; set; }

        public VW_Matching_InstitutionContentDetail(IDataReader dr)
        {
            InstitutionId = System.Convert.ToInt32(dr["InstitutionId"]);
            //ApplicationId = System.Convert.ToInt32(dr["ApplicationId"]);
            InstitutionName = System.Convert.ToString(dr["InstitutionName"]);
            Description = System.Convert.ToString(dr["Description"]);
            InternationalDescription = System.Convert.ToString(dr["InternationalDescription"]);
            Address1 = System.Convert.ToString(dr["Address1"]);
            Address2 = System.Convert.ToString(dr["Address2"]);
            City = System.Convert.ToString(dr["City"]);
            StateProvinceCode = System.Convert.ToString(dr["StateProvinceCode"]);
            Country = System.Convert.ToString(dr["Country"]);
            PostalCode = System.Convert.ToString(dr["PostalCode"]);
            CarnegieClassificationName = System.Convert.ToString(dr["CarnegieClassificationName"]);
            CalendarTypeOnline = System.Convert.ToString(dr["CalendarTypeOnline"]);
            LearningFormatOnline = System.Convert.ToString(dr["LearningFormatOnline"]);
            GeographicComments = System.Convert.ToString(dr["GeographicComments"]);
            //InstitutionType = System.Convert.ToString(dr["InstitutionType"]);
            InstitutionalAccreditationType = System.Convert.ToString(dr["InstitutionalAccreditationType"]);
            LoansOffered = System.Convert.ToString(dr["LoansOffered"]);
            OnCampusRequirements = System.Convert.ToString(dr["OnCampusRequirements"]);
            ScholarshipOffered = System.Convert.ToString(dr["ScholarshipOffered"]);
            RegistrationDetails = System.Convert.ToString(dr["RegistrationDetails"]);
            ComputerRequirement = System.Convert.ToString(dr["ComputerRequirement"]);
            TotalEnrollment = System.Convert.ToString(dr["TotalEnrollment"]);
            InstitutionDisclaimerType = System.Convert.ToString(dr["InstitutionDisclaimerType"]);
            InstitutionDisclaimer = System.Convert.ToString(dr["InstitutionDisclaimer"]);
            ThirdPartyContactAllowed = System.Convert.ToBoolean(dr["ThirdPartyContactAllowed"]);
            Locale = System.Convert.ToString(dr["Locale"]);
            AcademicYearBeginMonth = System.Convert.ToString(dr["AcademicYearBeginMonth"]);
            AcademicYearBeginDay = System.Convert.ToString(dr["AcademicYearBeginDay"]);
            AccreditationOrganization = System.Convert.ToString(dr["AccreditationOrganization"]);

            FileURL = System.Convert.ToString(dr["FileURL"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CacheBuster")))
                CacheBuster = System.Convert.ToInt32(dr["CacheBuster"]);
            else
                CacheBuster = 0;
            LogoURL = FormatLogoURL(FileURL, CacheBuster);

            if (!dr.IsDBNull(dr.GetOrdinal("IsFaithBased")))
                IsFaithBased = System.Convert.ToBoolean(dr["IsFaithBased"]);
            else
                IsFaithBased = false;

            if (!dr.IsDBNull(dr.GetOrdinal("HasAdditionalScript")))
                HasAdditionalScript = System.Convert.ToBoolean(dr["HasAdditionalScript"]);
            else
                HasAdditionalScript = false;
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
