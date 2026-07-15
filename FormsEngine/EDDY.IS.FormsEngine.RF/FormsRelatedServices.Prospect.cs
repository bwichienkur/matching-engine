using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.FormsEngine.PixelsService;
using System.Threading;
using EDDY.IS.Core.Logging;
using System.Configuration;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.Util.StringExtensions;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Core;
using EDDY.IS.Base;

namespace EDDY.IS.FormsEngine
{
    public partial class FormsRelatedServices
    {
        private static ProspectServiceClient ProspectService = new ProspectServiceClient();
        private const int _emsApplicationId = 27;

        #region Prospect Service


        public SaveProspectResponse SaveProspect(bool IsBeta, string SessionId, string LeadData, string AdditionalData, string TrackId, int ApplicationId)
        {
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.SaveProspect", null, IsBeta, SessionId, LeadData, AdditionalData, TrackId);
            Log.StartLogDetail("FormsEngine.SaveProspect");
            SaveProspectResponse Result = new SaveProspectResponse();
            SubmissionResultDTO submissionResultDTO = new SubmissionResultDTO();
            int NoId = -1;

            var prospectFlowType = ApplicationId == _emsApplicationId ? ProspectFlowTypes.EMS : ProspectFlowTypes.Prospecting;

            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(NoId, NoId, IsBeta, TrackId, "", false, SessionId, "", LeadData, AdditionalData, null, null,null);
            SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.HasValue ? LeadRequest.TrackId.Value : Guid.Empty, prospectFlowType);
            Result = SaveProspect(WebServiceProspect);
            Log.EndLogDetail();
            Log.EndLog(Result);

            return Result;
        }

        public int SaveProspectAdditionalInfo(int? ProspectId, string Email, string ProspectAdditionalData)
        {
            int Result = 0;
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.SaveProspectAdditionalInfo", null, ProspectId, Email, ProspectAdditionalData);
            Log.StartLogDetail("FormsEngine.SaveProspectAdditionalInfo");
            Dictionary<string, string> ProspectAdditionalDataDict = ProspectAdditionalData.BuildCaseInsensitiveDictionary();
            Dictionary<string, string> ProspectAdditionalDecoded = new Dictionary<string, string>();
            foreach (string key in ProspectAdditionalDataDict.Keys)
            {
                if(Uri.UnescapeDataString(ProspectAdditionalDataDict[key]).Length > 2000) //TODO: move this to prospect once prospect engine becomes editable
                    ProspectAdditionalDecoded.Add(key, Uri.UnescapeDataString(ProspectAdditionalDataDict[key]).Substring(0,1999)); //this field has 2000 character max so 
                else
                    ProspectAdditionalDecoded.Add(key, Uri.UnescapeDataString(ProspectAdditionalDataDict[key]));
            }
            Result = SaveProspectAdditionalInfo(ProspectId, Email, ProspectAdditionalDecoded);
            Log.EndLogDetail();
            Log.EndLog(Result);

            return Result;
        }

        /// <summary>
        /// Saves prospect
        /// </summary>
        /// <param name="prospect"></param>
        public SaveProspectResponse SaveProspect(SaveProspectRequest Prospect)
        {
            SaveProspectResponse Result = new SaveProspectResponse();
            try
            {
                Result = ProspectService.SaveProspect(Prospect);
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, new Exception("Exception Thrown from Prospect Service", ex)).Save();
            }

            return Result;
        }

        /// <summary>
        /// Saves prospect additional fields
        /// Any Key can be passed in and if doesn't already exist will be created
        /// If Record already exists for the Prospect/Key combo, new value will replace original
        /// </summary>
        /// <param name="ProspectId"></param>
        /// <param name="ProspectAdditionalData"></param>
        /// <returns></returns>
        public int SaveProspectAdditionalInfo(int? ProspectId, string Email, Dictionary<string, string> ProspectAdditionalData)
        {
            int Result = 0;
            try
            {
                Result = ProspectService.SaveProspectAdditionalData(Convert.ToInt32(ProspectId), Email, ProspectAdditionalData);
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, ex).Save();
            }

            return Result;
        }

        /// <summary>
        /// Prospect status Save
        /// </summary>
        /// <param name="ProspectFlowId"></param>
        /// <param name="IsLimbo"></param>
        public void SaveProspectWizardStatus(int ProspectFlowId, bool IsLimbo)
        {
            SaveProspectStatusRequest Request = new SaveProspectStatusRequest();
            Request.ProspectFlowId = ProspectFlowId;
            int WizardMatched = int.Parse(ConfigurationManager.AppSettings.Get("ProspectStatusIdWizardMatched"));
            int WizardLimbo = int.Parse(ConfigurationManager.AppSettings.Get("ProspectStatusIdWizardLimbo"));
            Request.ProspectStatusId = IsLimbo ? WizardLimbo : WizardMatched;
            Task.Run(() => SaveProspectWizardStatusAsync(Request));
        }


        public int SaveJobContactMeProspectAndStatus(SaveProspectRequest WebServiceProspect, string SiteURL, out int? prospectFlowId)
        {
            var Response = SaveProspect(WebServiceProspect);
            SaveProspectStatusRequest Request = new SaveProspectStatusRequest();
            Request.ProspectFlowId = Response.ProspectFlowId;
            string DomainKey = "JobContactMeProspectStatusID_{0}";
            string DomainString = string.Format(DomainKey, "hiringhonchos.com");
            int ProspectStatusId = 0;

            try
            {
                SiteURL = SiteURL.ToLower();
                string[] DomainParts = SiteURL.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                DomainString = string.Format(DomainKey, String.Join(".", DomainParts.Skip(Math.Max(0, DomainParts.Length - 2)).Take(2)));
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, new Exception(string.Format("Domain '{0}' specified is not a valid domain.", SiteURL), ex)).Save();
            }

            if (!int.TryParse(FormsEngine.GetResourceMetaDataTextForKey(DomainString), out ProspectStatusId))
            {
                ProspectStatusId = int.Parse(ConfigurationManager.AppSettings.Get("ProspectStatusIdJobsContactMe"));
            }
            Request.ProspectStatusId = ProspectStatusId;

            SaveProspectWizardStatusAsync(Request);
            prospectFlowId = Response.ProspectFlowId;
            return Response.ProspectId;
        }

        public void SaveProspectWizardStatusAsync(SaveProspectStatusRequest Request)
        {
            try
            {
                ProspectService.SaveProspectStatus(Request);
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, ex).Save();
            }

        }


        /// <summary>
        /// Saves prospect asynchronously
        /// </summary>
        /// <param name="prospect"></param>
        public void SaveProspectAsync(SaveProspectRequest Prospect)
        {
            Task.Run(() => SaveProspectAsyncTask(Prospect));
        }

        /// <summary>
        /// Sape prospect async helper
        /// </summary>
        /// <param name="prospect"></param>
        /// <param name="TrackingSessionGUID"></param>
        private void SaveProspectAsyncTask(SaveProspectRequest Prospect)
        {
            try
            {
                ProspectService.SaveProspect(Prospect);
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, ex).Save();
            }
        }

        public GetProspectFlowDetailsResponse GetProspectFlowDetails(int prospectFlowId)
        {
            GetProspectFlowDetailsResponse Result = new GetProspectFlowDetailsResponse();
            try
            {
                GetProspectFlowDetailsRequest getProspectFlowDetailsRequest = new GetProspectFlowDetailsRequest();
                getProspectFlowDetailsRequest.ProspectFlowID = prospectFlowId;
                Result = ProspectService.GetProspectFlowDetailsById(getProspectFlowDetailsRequest);
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, new Exception("Exception Thrown from Prospect Service", ex)).Save();
            }

            return Result;
        }

        #endregion Prospect Service
    }
}
