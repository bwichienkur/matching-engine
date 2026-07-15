using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDDY.IS.FormsEngine.Services.Caching;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.LeadEngine.DTO.Request;
using EDDY.IS.Util.Network;
using EDDY.IS.Util.StringExtensions;

namespace EDDY.IS.FormsEngine.Services.Classic.Base
{
    public abstract class FormsEngineAPIBase
    {
        public static FormsEngine FormsEngineService = new FormsEngine();

        public RawPostDataDTO BuildRawDataObject(string LeadData)
        {
            RawPostDataDTO RawData = new RawPostDataDTO();

            RawData.RemoteIp = IPHelper.GetClientIPAddress(new System.Web.HttpContextWrapper(System.Web.HttpContext.Current));
            RawData.BrowserInfo = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            RawData.Referer = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
            RawData.PostData = LeadData;
            return RawData;
        }

        public string MapAPILeadToFormStandardControls(APILead Lead)
        {
            List<string> FieldList = new List<string>();


            //code added to clean phone number of extra characters

            bool SkipCountryIdCheck = false;
            bool IsUs = false;
            bool IsCanada = false;

            if (!string.IsNullOrEmpty(Lead.CountryCode) && (Lead.CountryCode.ToLower().Equals("us") || Lead.CountryCode.ToLower().Equals("usa")))
            {
                IsUs = true;
                SkipCountryIdCheck = true;
            }

            if (!string.IsNullOrEmpty(Lead.CountryCode) && (Lead.CountryCode.ToLower().Equals("ca") || Lead.CountryCode.ToLower().Equals("can")))
            {
                IsCanada = true;
                SkipCountryIdCheck = true;
            }

            if (SkipCountryIdCheck == false)
            {
                int intCountry = 0;
                if (Int32.TryParse(Lead.CountryCode, out intCountry))
                {
                    IsUs = string.IsNullOrEmpty(Lead.CountryCode) ? false : intCountry == 4;
                    IsCanada = string.IsNullOrEmpty(Lead.CountryCode) ? false : intCountry == 5;
                }
            }

            bool IsUSorCanada = IsUs || IsCanada;

            String Phone1 = string.IsNullOrEmpty(Lead.Phone1) ? "" : Lead.Phone1.CleanPhoneNumber(IsUSorCanada);
            String Phone2 = string.IsNullOrEmpty(Lead.Phone2) ? "" : Lead.Phone2.CleanPhoneNumber(IsUSorCanada);

            
            FieldList.Add("Prefix=" + Lead.Prefix);
            FieldList.Add("First_Name=" + Lead.FirstName);
            FieldList.Add("Last_Name=" + Lead.LastName);
            FieldList.Add("Address=" + Lead.Address1);
            FieldList.Add("Address_2=" + Lead.Address2);
            FieldList.Add("City=" + Lead.City);
            FieldList.Add("Postal_Code=" + Lead.ZipCode);
            FieldList.Add("State=" + Lead.StateProvince);
            FieldList.Add("Country=" + Lead.CountryCode);
            FieldList.Add("us_citizen=" + Lead.USCitizen);
            FieldList.Add("Email=" + Lead.EmailAddress);
            FieldList.Add("Phone=" + Phone1);
            FieldList.Add("Alternate_Phone=" + Phone2);
            FieldList.Add("Age=" + Lead.Age);
            FieldList.Add("Year_of_Highest_Education_Completed=" + Lead.YearHighestEduCompleted);
            FieldList.Add("Highest_Level_of_Education_Completed=" + Lead.HighestLevelOfEdu);
            FieldList.Add("Military_Affiliation=" + Lead.Military);
            FieldList.Add("Desired_Start_Date=" + Lead.StartDate);
            FieldList.Add("Prospect_Flow_Status_History_Id=" + Lead.ProspectFlowStatusHistoryId);
            FieldList.Add("Chat_Integration_Session_Id=" + Lead.ChatIntegrationSessionId);
            FieldList.Add("Desired_Degree_Level=" + Lead.DesiredDegreeLevel);
            FieldList.Add("ChannelId=" + Lead.ChannelId);
            FieldList.Add("SubChannelId=" + Lead.SubChannelId);
            FieldList.Add("EstimatedRevShare=" + Lead.EstimatedRevShare);
            FieldList.Add("ValidateTCPA=" + Lead.ValidateTCPA);
            FieldList.Add("InstitutionName=" + Lead.InstitutionName);
            FieldList.Add("DialerKey=" + Lead.DialerKey);
            FieldList.Add("Tsr=" + Lead.TSR);

            if (Lead.LeadId_Token != default(Guid))
                FieldList.Add("LeadId_Token=" + Lead.LeadId_Token.ToString());

            if (Lead.Categories != null && Lead.Categories.Count > 0)
            {
                FieldList.Add("Categories=" + string.Join(",", Lead.Categories));
            }

            if (Lead.SubCategories != null && Lead.SubCategories.Count > 0)
            {
                FieldList.Add("SubCategories=" + string.Join(",", Lead.SubCategories));
            }

            if (Lead.Specialties != null && Lead.Specialties.Count > 0)
            {
                FieldList.Add("Specialties=" + string.Join(",", Lead.Specialties));
            }

            if(Lead.UserId != null)
            {
                FieldList.Add("UserId=" + Lead.UserId.ToString());
            }

            FieldList.Add("ExternalMatchItemGuid=" + Lead.ExternalMatchItemGuid);
            FieldList.Add("PreValidatedProgram=" + Lead.PreValidatedProgram);
            FieldList.Add("ProspectFlowId=" + Lead.ProspectFlowId);
            Lead.AdditionalFields = Lead.AdditionalFields==null? new List<AdditionalField>() : Lead.AdditionalFields;

            var AdditionalFields = from f in Lead.AdditionalFields
                                   select f.Key + "=" + f.Value;

            if (AdditionalFields!=null && AdditionalFields.Count() > 0)
            {
                FieldList.AddRange(AdditionalFields);
            }

            return string.Join("&", FieldList);
        }


    }
}