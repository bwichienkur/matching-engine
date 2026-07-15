using EDDY.IS.Base;
using EDDY.IS.Core;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.LeadEngine.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EDDY.IS.FormsEngine
{
    public enum Five9RouteStatus
    {
        None,
        WTTToContactCenter,
        WTTToThirdParty,
        SMPUpsell
    }

    public partial class FormsRelatedServices
    {
        private static Five9Service.ServiceClient Five9Client = new Five9Service.ServiceClient("BasicHttpBinding_Five9Service");

        private Five9RouteStatus SendToTitanium(LeadCreateRequest LeadRequest, ProgramWithInstitutionCampus program)
        {
            ProspectFlowTypes pft = ProspectFlowTypes.WTTitanium;
            SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.GetValueOrDefault(), pft);
            var ProspectResult = SaveProspect(WebServiceProspect);
            var ProspectId = ProspectResult.ProspectId;
            var ProspectFlowId = ProspectResult.ProspectFlowId;

            if (ProspectFlowId > 0)
            {
                //if (ShouldRouteToThirdParty(WebServiceProspect, program))
                //{
                //    return Five9RouteStatus.WTTToThirdParty;
                //}
                //else
                //{
                    Five9Service.Five9Request request = CreateFive9Request(WebServiceProspect, LeadRequest, ProspectFlowId);
                    request.DegreeInterest = program.InstitutionName;
                    request.ProgramInterest = program.ProgramName;
                    //request.ListName = ConfigurationManager.AppSettings["Five9WTTList"];
                    request.ListName = "WTT - " + program.InstitutionName;
                    Five9Client.SendToFive9Async(request);

                    return Five9RouteStatus.WTTToContactCenter;
                //}
            }

            return Five9RouteStatus.None;
        }

        private bool ShouldRouteToThirdParty(SaveProspectRequest prospectRequest, ProgramWithInstitutionCampus program)
        {
            bool sendToThirdParty = false;
            string testInstitutions = ConfigurationManager.AppSettings.Get("WTTTestInstitutions");

            if(!String.IsNullOrEmpty(testInstitutions))
            {
                foreach (string s in testInstitutions.Split(','))
                {
                    if (Convert.ToInt32(s) == program.InstitutionId)
                    {
                        if (!String.IsNullOrEmpty(prospectRequest.Prospect.Address2) && prospectRequest.Prospect.Address2 == "WTT")
                        {
                            sendToThirdParty = true;
                        }
                        else
                        {
                            string strPercent = ConfigurationManager.AppSettings.Get("WTTPercentRoute");

                            if (!String.IsNullOrEmpty(strPercent))
                            {
                                int percentToRoute = Convert.ToInt32(strPercent);
                                Random rand = new Random();
                                int diceRoll = rand.Next(1, 100);

                                if (diceRoll <= percentToRoute)
                                    sendToThirdParty = true;

                            }
                        }
                           
                    }
                }
            }

            return sendToThirdParty;
        }

        private Five9Service.Five9Request CreateFive9Request(SaveProspectRequest WebServiceProspect, LeadCreateRequest LeadCreateRequest, int ProspectFlowId)
        {
            Dictionary<string, string> leadData = LeadCreateRequest.LeadData;
            Five9Service.Five9Request request = new Five9Service.Five9Request();


            request.FirstName = WebServiceProspect.Prospect.FirstName;
            request.LastName = WebServiceProspect.Prospect.LastName;
            request.ProspectFlowId = ProspectFlowId;
            request.ZipCode = WebServiceProspect.Prospect.PostalCode;
            request.State = leadData.ContainsKey("state") ? leadData["state"].ToUpper() : null;
            request.City = WebServiceProspect.Prospect.City;

            string eduLevel = GetFieldValue("highest_level_of_education_completed", leadData);
            if (eduLevel != "")
                request.EducationLevel = ((EDDY.IS.Base.EducationLevel)Convert.ToInt32(eduLevel)).ToString();

            request.Email = WebServiceProspect.Prospect.Email;
            request.GradYear = GetFieldValue("year_of_highest_education_completed", leadData);
            request.Number1 = WebServiceProspect.Prospect.Phone;
            request.Number2 = WebServiceProspect.Prospect.OtherPhone;          
            request.Street = WebServiceProspect.Prospect.Address1;
            request.TrackId = LeadCreateRequest.TrackId.ToString();
            request.ExternalLeadId = LeadCreateRequest.ExternalLeadId?.ToString();

            return request;
        }

        private string GetFieldValue(string key, Dictionary<string, string> values, bool RemoveFromDictionary = true)
        {
            string result = "";
            if (values.ContainsKey(key))
            {
                result = values[key];
                if (RemoveFromDictionary)
                {
                    values.Remove(key);
                }
            }
            return result;
        }

        public void RouteWTTToThirdParty(LeadCreateRequest LeadRequest, List<LeadCreateResponse> leadRecords, List<ProgramWithInstitutionCampus> programs)
        {
            ProspectFlowTypes pft = ProspectFlowTypes.WTTitanium;
            SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.GetValueOrDefault(), pft);
            ProgramWithInstitutionCampus titaniumProgram = null;

            ProcessPrograms(programs, out titaniumProgram);

            if(titaniumProgram != null)
            {
                decimal leadId = 0;

                foreach(var l in leadRecords)
                {
                    if(l.Lead.ProgramProductId == titaniumProgram.ProgramProductId)
                    {
                        leadId = l.Lead.LeadId;
                        break;
                    }
                }

                if(leadId > 0)
                {
                    string state = LeadRequest.LeadData.ContainsKey("state") ? LeadRequest.LeadData["state"].ToUpper() : null;

                    if (state != null)
                        SendToBrightfire(WebServiceProspect, leadId, titaniumProgram, state);
                }
            }
        }

        private void SendToBrightfire(SaveProspectRequest prospect, decimal leadId, ProgramWithInstitutionCampus titaniumProgram, string state)
        {
            string url = "http://shadow.brightfire.net/offersys/public_html/edu/titan/post?";

            

            url += "Program_Of_Interest=" + titaniumProgram.ProgramProductId.ToString();
            url += "&First_Name=" + HttpUtility.UrlEncode(prospect.Prospect.FirstName);
            url += "&Last_Name=" + HttpUtility.UrlEncode(prospect.Prospect.LastName);
            url += "&Phone=" + HttpUtility.UrlEncode(prospect.Prospect.Phone);
            url += "&City=" + HttpUtility.UrlEncode(prospect.Prospect.City);
            url += "&State=" + HttpUtility.UrlEncode(state);
            url += "&Postal_Code=" + HttpUtility.UrlEncode(prospect.Prospect.PostalCode);
            url += "&Country=USA";
            url += "&Address=" + HttpUtility.UrlEncode(prospect.Prospect.Address1);
            url += "&Address_2=";
            url += "&Highest_Level_of_Education_Completed=" + prospect.Prospect.EducationLevelID.ToString();

            url += "&Year_of_Highest_Education_Completed=";
            if (prospect.Prospect.GraduationYear.HasValue)
            {
                url += prospect.Prospect.GraduationYear.Value.ToString();
            }

            url += "&Age=";
            if (prospect.Prospect.Age.HasValue)
            {
                url += prospect.Prospect.Age.Value.ToString();
            }

            url += "&Email=" + HttpUtility.UrlEncode(prospect.Prospect.Email);
            url += "&Us_citizen=" + prospect.Prospect.IsUsCitizen.ToString();

            url += "&Military_Affiliation=";
            if (prospect.Prospect.MilitaryStatusID.HasValue)
            {
                url += prospect.Prospect.MilitaryStatusID.Value.ToString();

                bool isMilitary = prospect.Prospect.MilitaryStatusID.Value == 126;
                url += "&IsMilitary=" + isMilitary.ToString();
            }
            else
            {
                url += "&IsMilitary=false";
            }

            url += "&Leadid=" + leadId.ToString();
            url += "&Schoolid=" + titaniumProgram.InstitutionId.ToString();

            try
            {
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(url);

                var response = wrGETURL.GetResponse();
                
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine
                                , new Exception(string.Format("SendToBrightfire - {0}", ex.Message))
                                , "SendToBrightfire failed").Save();
            }
        }

        private bool ProcessPrograms(List<ProgramWithInstitutionCampus> programs, out ProgramWithInstitutionCampus titaniumProgram)
        {
            bool upsellEligible = true;
            titaniumProgram = null;

            foreach (var p in programs)
            {
                if (p.ProductId == 52)
                {
                    titaniumProgram = p;
                    break;
                }

                if (p.RemonetizationRestriction)
                {
                    upsellEligible = false;
                }
            }

            return upsellEligible;
        }

        public Five9RouteStatus ProcessDialerRoute(LeadCreateRequest LeadRequest, List<ProgramWithInstitutionCampus> programs, int prospectFlowId)
        {
            Five9RouteStatus status = Five9RouteStatus.None;

            string emailAddress = Util.StringExtensions.StringExtensions.GetFieldValue("email", LeadRequest.LeadData);
            if (!String.IsNullOrEmpty(emailAddress) && !emailAddress.EndsWith("@test.com", StringComparison.OrdinalIgnoreCase))
            {
                if (programs != null && programs.Count == 0)
                {
                    //Limbo path
                }
                else if (programs != null)
                {
                    ProgramWithInstitutionCampus titaniumProgram = null;
                    bool upsellEligible = ProcessPrograms(programs, out titaniumProgram);
                    DTO.CampaignDetailDTO cd = null;

                    if (LeadRequest.TrackId.HasValue)
                    {
                        cd = GetCampaignDetailByTrackId(LeadRequest.TrackId.Value);
                    }

                    if (titaniumProgram != null)
                    {
                        status = SendToTitanium(LeadRequest, titaniumProgram);
                    }
                }
            }

            return status;
        }
    }
}
