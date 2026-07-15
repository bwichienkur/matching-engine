using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramContentDetail
    {
        public int ProgramId { get; set; }
        //public int ApplicationId { get; set; }
        public string ProgramName { get; set; }
        public int ProgramTypeId { get; set; }
        public int ProgramLevelId { get; set; }
        public int InstitutionId { get; set; }
        public string ProgramDescription { get; set; }
        public string Requirements { get; set; }
        public string LearningFormat { get; set; }
        public string CreditsRequired { get; set; }
        public string LanguageName { get; set; }
        public string AccreditationDescription { get; set; }
        public string InternationalRequirements { get; set; }
        public string InternationalTuition { get; set; }
        public string AdditionalTuitionInfo { get; set; }
        public string FinancialAidInfo { get; set; }
        public string AdditionalFinancialAidInfo { get; set; }
        public string OnCampusRequirements { get; set; }
        public string NumberofstartsDates { get; set; }
        public string CourseLength { get; set; }
        public string CostPerCredit { get; set; }
        public string ProgramDisclaimerType { get; set; }
        public string ProgramDisclaimer { get; set; }
        public string EligibilityRequirements { get; set; }
        public string InstitutionIncorporationType { get; set; }
        public int? SchoolId { get; set; }
        public string SchoolName { get; set; }
        //public bool HasSchoolLogo { get; set; }
        public string SchoolFileURL { get; set; }
        public int SchoolCacheBuster { get; set; }
        public string SchoolLogoURL { get; set; }
        //public bool HasProgramLogo { get; set; }
        public string ProgramFileURL { get; set; }
        public int ProgramCacheBuster { get; set; }
        public string ProgramLogoURL { get; set; }
        public string OutStateTuition { get; set; }
        public string InstateTuition { get; set; }
        public string CostPerCreditGrad { get; set; }

        public string Scholarships { get; set; }
        public string ScholarshipsDescription { get; set; }
        public string SettingDescription { get; set; }
        public string ShortDescription { get; set; }
        public string CurrencyCode { get; set; }
        public string CostPerCreditCurrencyCode { get; set; }
        public string InStateTuitionCurrencyCode { get; set; }
        public string OutOfStateTuitionCurrencyCode { get; set; }
        public string DomesticFinancialAidInfo { get; set; }
        public string DomesticFinancialAidDescription { get; set; }
        public string ProgramDisplayName { get; set; }

        //LanguageRequirement, PiorCreditsRequired, Schedule, CourseCost
        public VW_Matching_ProgramContentDetail(IDataReader dr)
        {
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            //ApplicationId = System.Convert.ToInt32(dr["ApplicationId"]);
            ProgramName = System.Convert.ToString(dr["ProgramName"]);
            ProgramTypeId = System.Convert.ToInt32(dr["ProgramTypeId"]);
            ProgramLevelId = System.Convert.ToInt32(dr["ProgramLevelId"]);
            InstitutionId = System.Convert.ToInt32(dr["InstitutionId"]);
            ProgramDescription = System.Convert.ToString(dr["ProgramDescription"]);
            Requirements = System.Convert.ToString(dr["Requirements"]);
            LearningFormat = System.Convert.ToString(dr["LearningFormat"]);
            CreditsRequired = System.Convert.ToString(dr["CreditsRequired"]);
            LanguageName = System.Convert.ToString(dr["LanguageName"]);
            AccreditationDescription = System.Convert.ToString(dr["AccreditationDescription"]);
            InternationalRequirements = System.Convert.ToString(dr["InternationalRequirements"]);
            InternationalTuition = System.Convert.ToString(dr["InternationalTuition"]);
            AdditionalTuitionInfo = System.Convert.ToString(dr["AdditionalTuitionInfo"]);
            FinancialAidInfo = System.Convert.ToString(dr["FinancialAidInfo"]);
            AdditionalFinancialAidInfo = System.Convert.ToString(dr["AdditionalFinancialAidInfo"]);
            OnCampusRequirements = System.Convert.ToString(dr["OnCampusRequirements"]);
            NumberofstartsDates = System.Convert.ToString(dr["NumberofstartsDates"]);
            CourseLength = System.Convert.ToString(dr["CourseLength"]);
            CostPerCredit = System.Convert.ToString(dr["CostPerCredit"]);
            ProgramDisclaimerType = System.Convert.ToString(dr["ProgramDisclaimerType"]);
            ProgramDisclaimer = System.Convert.ToString(dr["ProgramDisclaimer"]);
            EligibilityRequirements = System.Convert.ToString(dr["EligibilityRequirements"]);
            InstitutionIncorporationType = System.Convert.ToString(dr["InstitutionIncorporationType"]);
            SchoolName = System.Convert.ToString(dr["SchoolName"]);

            //HasSchoolLogo = System.Convert.ToBoolean(dr["HasSchoolLogo"]);
            SchoolFileURL = System.Convert.ToString(dr["SchoolFileURL"]);
            if (!dr.IsDBNull(dr.GetOrdinal("SchoolCacheBuster")))
                SchoolCacheBuster = System.Convert.ToInt32(dr["SchoolCacheBuster"]);
            else
                SchoolCacheBuster = 0;
            SchoolLogoURL = FormatLogoURL(SchoolFileURL, SchoolCacheBuster);

            //HasProgramLogo = System.Convert.ToBoolean(dr["HasProgramLogo"]);
            ProgramFileURL = System.Convert.ToString(dr["ProgramFileURL"]);
            if (!dr.IsDBNull(dr.GetOrdinal("ProgramCacheBuster")))
                ProgramCacheBuster = System.Convert.ToInt32(dr["ProgramCacheBuster"]);
            else
                ProgramCacheBuster = 0;
            ProgramLogoURL = FormatLogoURL(ProgramFileURL, ProgramCacheBuster);

            OutStateTuition = System.Convert.ToString(dr["OutStateTuition"]);
            InstateTuition = System.Convert.ToString(dr["InstateTuition"]);
            CostPerCreditGrad = System.Convert.ToString(dr["CostPerCreditGrad"]);

            Scholarships = System.Convert.ToString(dr["Scholarships"]);
            ScholarshipsDescription = System.Convert.ToString(dr["ScholarshipsDescription"]);
            SettingDescription = System.Convert.ToString(dr["SettingDescription"]);
            ShortDescription = System.Convert.ToString(dr["ShortDescription"]);
            CurrencyCode = System.Convert.ToString(dr["CurrencyCode"]);
            OutOfStateTuitionCurrencyCode = System.Convert.ToString(dr["OutOfStateTuitionCurrencyCode"]);
            InStateTuitionCurrencyCode = System.Convert.ToString(dr["InStateTuitionCurrencyCode"]);
            CostPerCreditCurrencyCode = System.Convert.ToString(dr["CostPerCreditCurrencyCode"]);
            DomesticFinancialAidInfo = System.Convert.ToString(dr["DomesticFinancialAidInfo"]);
            DomesticFinancialAidDescription = System.Convert.ToString(dr["DomesticFinancialAidDescription"]);

            if (!dr.IsDBNull(dr.GetOrdinal("SchoolId")))
                SchoolId = System.Convert.ToInt32(dr["SchoolId"]);

            //Pull the program display name if we have it. Otherwise, we pull the program name. Mostly affects EMS institutions.
            if (!String.IsNullOrWhiteSpace(System.Convert.ToString(dr["ProgramDisplayName"])))
                ProgramDisplayName = System.Convert.ToString(dr["ProgramDisplayName"]);
            else
                ProgramDisplayName = System.Convert.ToString(dr["ProgramName"]);
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
