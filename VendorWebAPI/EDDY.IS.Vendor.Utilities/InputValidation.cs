using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.IO;
using System.Xml;
namespace EDDY.IS.Vendor.Utilities
{
    public static class InputValidation
    {
        public enum LeadSources { web, callcenter };
        //Start DNC Call Settings
        private const String SanId = "10286448-386448-15";
        private const int Uid = 378248;
        private const String States = "ALL";
        private const String Filters = "ALL";
        private const String Project = "";
        private const String PostingUrl = "https://www3.dncsolution.com/dncwebservices/DNCQuickCheck.asmx/Execute";
        //End DNC Call Settings

        public static bool IsCsvStringNumeric(string csv)
        {
            return (Regex.IsMatch(csv, @"^[0-9]+(\s*,\s*[0-9]+)*$"));
        }

        public static bool IsValidEMSEmail(string email)
        {
            return (Regex.IsMatch(email, @"^([0-9a-zA-Z]([-.+\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*\.)+[a-zA-Z]{2,9})$"));
        }

        public static bool IsValidEmail(string email)
        {
            return (Regex.IsMatch(email, @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"));
        }

        public static bool IsValidUSPostalCode(string postalCode)
        {
            postalCode = postalCode.Trim();
            return (Regex.IsMatch(postalCode, @"^\d{5}(?:[-\s]\d{4})?$"));
        }

        public static bool IsValidCanadaPostalCode(string postalCode)
        {
            postalCode = postalCode.Trim();
            return (Regex.IsMatch(postalCode, @"^[ABCEGHJKLMNPRSTVXY]{1}\d{1}[A-Z]{1} *\d{1}[A-Z]{1}\d{1}$"));
        }

        public static bool IsValidNorthAmericanPostalCode(string postalCode)
        {
            return IsValidUSPostalCode(postalCode) || IsValidCanadaPostalCode(postalCode);
        }

        public static List<int> CsvToIntList(string csv)
        {
            List<int> intList = new List<int>();

            string[] csvArray = csv.ToString().Split(',');
            if (csvArray.Length > 0) { }
            foreach (string str in csvArray)
            {
                int convertedInt = 0;
                if (int.TryParse(str, out convertedInt))
                {
                    intList.Add(convertedInt);
                }
            }
            return intList;
        }

        public static bool IsPhoneNumberDoNotCall(string phoneNumbersToCheck)
        {

            string currentPostingString = String.Empty;
            var result = false;

            WebRequest request;
            StringBuilder postingStringWithoutNumbersBuilder = new StringBuilder();
            string responseFromServer = string.Empty;

            //build out our posting string
            postingStringWithoutNumbersBuilder.Append("UID=").Append(Uid.ToString());
            postingStringWithoutNumbersBuilder.Append("&SAN=").Append(SanId);
            postingStringWithoutNumbersBuilder.Append("&States=").Append(States);
            postingStringWithoutNumbersBuilder.Append("&Filters=").Append(Filters);
            postingStringWithoutNumbersBuilder.Append("&Project=").Append(Project);
            request = WebRequest.Create(PostingUrl);
            //set the content type
            request.ContentType = "application/x-www-form-urlencoded";

            //set verb and timeout
            request.Timeout = 5000;
            request.Method = "POST";

            String postingStringWithoutNumbers = String.Copy(postingStringWithoutNumbersBuilder.ToString());

            String FullPostingString = postingStringWithoutNumbers + "&PhnNbrs=" + phoneNumbersToCheck;

            currentPostingString = FullPostingString;

            byte[] byteArray = Encoding.UTF8.GetBytes(FullPostingString);
            request.ContentLength = byteArray.Length;

            //perform post call
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();
            reader.Close();

            //PROCESS RESPONSE
            //loop through each phone number node and if NDC = 0 then add to the AllowedNumbers List
            XmlDocument xDocument = new XmlDocument();

            String DecodedString1 = responseFromServer.Replace("&lt;", "<");
            String DecodedString2 = DecodedString1.Replace("&gt;", ">");

            xDocument.LoadXml(DecodedString2);

            XmlNodeList phoneNumbers = xDocument.GetElementsByTagName("PhnNbr");

            foreach (XmlNode phoneNumber in phoneNumbers)
            {
                if (!phoneNumber["DNC"].InnerText.ToString().Equals("0"))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public sealed class MessageCodes
        {
            public static readonly string InvalidAPIKey = "msg0001";

            public static readonly string APICallRateExceeded = "msg0002";
            public static readonly string MissingFields = "msg0003";
            public static readonly string EmptyLeadIdValue = "msg0004";

            public static readonly string MissingAPIKey = "msg0005";
            public static readonly string APIKeyNotGuid = "msg0006";
            public static readonly string MissingLeadId = "msg0007";
            public static readonly string HostPostNotAvailable = "msg0008";

            public static readonly string GeneralException = "msg0009";
            public static readonly string DialerListNameRequired = "msg0010";
            public static readonly string DuplicateRecord = "msg0011";
            public static readonly string Address1Required = "msg0012";

            public static readonly string Address2MaxLength = "msg0013";
            public static readonly string CityRequired = "msg0014";
            public static readonly string CityMaxLength = "msg0015";
            public static readonly string CountryRequired = "msg0016";

            public static readonly string CountryMaxLength = "msg0017";
            public static readonly string CountryIsInvalid = "msg0018";
            public static readonly string PhoneRequired = "msg0019";
            public static readonly string PhoneMaxLength = "msg0020";

            public static readonly string PhoneNotNumeric = "msg0021";
            public static readonly string AlternatePhoneNotNumeric = "msg0022";
            public static readonly string AlternatePhoneMaxLength = "msg0023";
            public static readonly string PhoneNumberMarkedDoNotCall = "msg0024";

            public static readonly string EmailRequired = "msg0025";
            public static readonly string EmailMaxLength = "msg0026";
            public static readonly string EmailIsInvalid = "msg0027";
            public static readonly string FirstNameRequired = "msg0028";

            public static readonly string FirstNameMaxLength = "msg0029";
            public static readonly string LastNameRequired = "msg0030";
            public static readonly string LastNameMaxLength = "msg0031";
            public static readonly string StateRequired = "msg0032";

            public static readonly string StateMaxLength = "msg0033";
            public static readonly string PostalCodeRequired = "msg0034";
            public static readonly string PostalStateMisMatch = "msg0035";
            public static readonly string AgeNotNumeric = "msg0036";

            public static readonly string MilitaryNotNumeric = "msg0037";
            public static readonly string ValidationPassed = "msg0038";
            public static readonly string CampaignDisabled = "msg0039";

            public static readonly string CategoryIdsNotNumeric = "msg0040";
            public static readonly string LevelIdsNotNumeric = "msg0041";
            public static readonly string SubCategoryIdsNotNumeric = "msg0042";

            public static readonly string CampusTypeRequired = "msg0043";
            public static readonly string CampusIdRequired = "msg0044";

            public static readonly string PageSizeRequired = "msg0045";
            public static readonly string PageSizeNotNumeric = "msg0046";

            public static readonly string StartIndexRequired = "msg0047";
            public static readonly string StartIndexNotNumeric = "msg0048";
            public static readonly string RoutedSuccessfully = "msg0049";
            public static readonly string LeadIdTokenRequired = "msg0050";
            public static readonly string StateInvalid = "msg0051";

            public static readonly string ProgramIdRequired = "msg0052";
       
            public static readonly string TemplateIdRequired = "msg0053";

            public static readonly string NoResults = "msg0054";

            public static readonly string FailedProgramRule = "msg0055";

            public static readonly string UserAgreementRequired = "msg0056";

            public static readonly string StartIndexLessThanZero = "msg0057";

            public static readonly string StartIndexGreaterThanPageSize = "msg0058";

            public static readonly string CampaignInactive = "msg0059";

            public static readonly string CampaignExpired = "msg0060";

            public static readonly string CampaignPending = "msg0061";

            public static readonly string CampaignTerminated = "msg0062";

            public static readonly string PageSizeLessThanZero = "msg0063";

            public static readonly string FeatureNotConfigured = "msg0064";

            public static readonly string FeatureNotConfiguredAPIDirectory = "msg0065";

            public static readonly string FeatureNotConfiguredHP = "msg0066";

            public static readonly string FeatureNotConfiguredProspecting = "msg0067";

            public static readonly string Address1MaxLength = "msg0068";

            public static readonly string ProspectIsValid = "msg0069";

            public static readonly string ProgramLevelIdIsNotNumeric = "msg0070";

            public static readonly string InstitutionIdIsNotNumeric = "msg0071";

            public static readonly string CampusIdIsNotNumeric = "msg0072";

            public static readonly string PostalCodeMaxLength = "msg0073";

            public static readonly string ProgramIdIsNotNumeric = "msg0074";

            public static readonly string IncludeAdditionalProgramQuestionsIsNotBool = "msg0075";

            public static readonly string NotAValidUSPostalCode = "msg0076";

            public static readonly string YearHighestEducationCompletedIsNotNumeric = "msg0077";

            public static readonly string LeadIdTokenNotGuid = "msg0078";

            public static readonly string CategorysIdsIsRequired = "msg0079";

            public static readonly string InvalidRequestBody = "msg0080";

            public static readonly string ProgramLevelIdsIsRequired = "msg0081";

            public static readonly string SubCategoryIdsIsRequired = "msg0082";

            public static readonly string PageSizeExceedsIntegerSize = "msg0083";

            public static readonly string StartIndexExceedsIntegerSize = "msg0084";

            public static readonly string ProgramUnavailable = "msg0085";

            public static readonly string CategoryUnavailable = "msg0086";

            public static readonly string SubjectUnavailable = "msg0087";

            public static readonly string InstitutionUnavailable = "msg0088";

            public static readonly string ProgramLevelUnavailable = "msg0089";

            public static readonly string CampusUnavailable = "msg0090";
            
            public static readonly string NotAValidCanadianPostalCode = "msg0091";

            public static readonly string AgeLimitExceeded = "msg0092";

            public static readonly string YearHighestEducationCompletedInvalid = "msg0093";

            public static readonly string MilitaryAffiliationUnavailable = "msg0094";

            public static readonly string LeadSourceIsRequired = "msg0095";

            public static readonly string LeadSourceUrlIsRequired = "msg0096";

            public static readonly string LeadSourceUrlIsInvalid = "msg0097";

            public static readonly string LeadSourceIsInvalid = "msg0098";

            public static readonly string LeadInitiatingUrlIsInvalid = "msg0099";

            public static readonly string LeadInitiatingUrlIsRequired = "msg0100";

            public static readonly string ProspectSourceIsRequired = "msg0101";

            public static readonly string ProspectSourceUrlIsRequired = "msg0104";

            public static readonly string ProspectSourceUrlIsInvalid = "msg0103";

            public static readonly string ProspectSourceIsInvalid = "msg0102";

            public static readonly string ProspectInitiatingUrlIsInvalid = "msg0105";

            public static readonly string ProspectInitiatingUrlIsRequired = "msg0106";

            public static readonly string AgentNameIsRequired = "msg0107";


            public static readonly string InstitutionIdIsRequired = "msg0108";
            public static readonly string LeadStateIdIsInvalid = "msg0109";
            public static readonly string LookUpKeyListIsRequired = "msg0110";
            public static readonly string LookUpKeyIsInvalid = "msg0111";
            public static readonly string LookUpValueIsRequired = "msg0112";
            public static readonly string NoLeadFound = "msg0113";
            public static readonly string NoUpdateNeeded = "msg0114";
            public static readonly string UpdateFailed = "msg0115";
            public static readonly string ExceptionOccurredWhileUpdating = "msg0116"; //msg0009 use exissting exception code instead?
            public static readonly string SuccessfulLeadUpdate = "msg0117";


        }
    }
}
