using EDDY.IS.LeadEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Util.StringExtensions;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.Core.Logging;
using System.Configuration;
using EDDY.IS.FormsEngine.DataModel;
using System.Web;
using EDDY.IS.Base.Util;

namespace EDDY.IS.FormsEngine
{
    public class LeadSaveData
    {
        public int ProgramProductId { get; set; }
        public int TemplateId { get; set; }
        public decimal? LeadId { get; set; }
        public bool? IsValid { get; set; }
        public LeadCreationType? LeadCreationType { get; set; }
        public int? PaidStatusTypeId { get; set; }
        public bool AllowedViaLeadScoringUpsell {get; set;}

        public int? ProductId { get; set; }
        public Guid? ExternalMatchItemGuid { get; set; }

        public int? OriginalProgramProductId { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string CustomTCPA { get; set; }
    }

    public enum LeadCreationType
    {
        InstitutionFormInitial = 1,
        InstitutionFormCrossSell = 2,
        WizardSmartMatch = 3,
        WizardUserSelection = 4,
        HostAndPost = 5,
        Apollo = 6,
        ThirdPartySmartMatch = 7,
        HomeSecurity = 8,
        ProgramWizardInitial = 9,
        ProgramWizardUserSelection = 10,
        SchoolPickerUserSelection = 12
    }

    public enum ISApplication
    {
        eLDrupal = 1,
        EMDDrupal = 2,
        ExpressDirectories = 3,
        FormsEngine = 4,
        MatchingEngine = 5,
        VendorAPI = 6,
        HostAndPost = 7,
        PixelService = 8,
        Apollo = 9,
        TDC = 10,
        CE = 11,
        SC = 12,
        LandingPages = 13,
        ProspectService = 14,
        Tracking = 15,
        LeadService = 16
    }

    public enum MatchResponseType
    {
        SmartMatch = 1,
        SchoolSelection = 2,
        CrossSell = 3,
        Directory = 4
    }

    public class LeadSaveManger
    {
        private static TemplateDataService dbTemplateService = new TemplateDataService();
        private static SubmissionDataService dbSubmissionService = new SubmissionDataService();
        public static FormsEngine FormsEngineService = new FormsEngine();
        private static LeadEngine.LeadEngine leadEngine = new LeadEngine.LeadEngine();
        private PerformanceLog _peformanceLog;

        private readonly HashSet<string> keysToIgnoreInAdditionalDetails = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { 
            "AffiliateId", 
            "templateId", 
            "BirthDate", 
            "WidgetRequestGuid", 
            "WidgetName",
            "InitiatingURL",
            "LandingURL",
            "CallCenterURL"
        };

        public LeadSaveManger(PerformanceLog log)
        {
            _peformanceLog = log;
        }

        public LeadCreateResponse UpdateLead(LeadSaveData lead, string LeadData, int? ProspectId, int? ClientRelationContactId, int? prospectFlowId, int leadId)
        {
            LeadCreateRequest leadCreate = EntityBuildHelper.BuildLeadCreateRequestObject(lead.TemplateId, lead.ProgramProductId, false, "", "", false, "", "", LeadData, "", "", LeadCreationType.Apollo, ProspectId);

            if (ProspectId.HasValue)
                leadCreate.ProspectId = ProspectId;

            leadCreate.ClientRelationContactId = ClientRelationContactId;
            LeadCreateResponse leadCreateResponse = leadEngine.UpdateLead(leadCreate, leadId, _peformanceLog);

            return leadCreateResponse;
        }

        public LeadCreateResponse Execute(int? TemplateId, LeadCreateRequest LeadCreate, string FeSessionId, ISApplication? ConsumingApplication, RawPostDataDTO RawPostData, int? ProspectId, int? SubmissionId, MatchResponseType? matchResponseType, bool realtimeDelivery, APIValidationResultDTO QuickCheckValidationResult, int? prospectFlowId)
        {
            List<LeadCreateResponse> leadResponseList = GetSessionSave(FeSessionId, new List<LeadCreateRequest> { LeadCreate });

            if (leadResponseList == null)
            {
                leadResponseList = Execute(TemplateId, new List<LeadCreateRequest>() { LeadCreate }, FeSessionId, ConsumingApplication, RawPostData, ProspectId, SubmissionId, matchResponseType, LeadCreate.ClientRelationContactId, realtimeDelivery, QuickCheckValidationResult, null, prospectFlowId);
            }

            if (leadResponseList != null && leadResponseList.Count > 0)
            {
                SetSessionSave(FeSessionId, leadResponseList);
                return leadResponseList[0];
            }
            else
            {
                return null;
            }
        }

        public List<LeadCreateResponse> Execute(int? TemplateId, List<LeadCreateRequest> LeadCreateList, string FeSessionId, ISApplication? ConsumingApplication, RawPostDataDTO RawPostData, int? ProspectId, int? SubmissionId, MatchResponseType? matchResponseType, int? ClientRelationContactId, bool realtimeDelivery, APIValidationResultDTO QuickCheckValidationResult, Guid? leadScoringGuid, int? prospectFlowId)
        {
            _peformanceLog.StartLogDetail("LeadSaveManger.Execute2");
            List<LeadCreateResponse> leadResponseList = GetSessionSave(FeSessionId, LeadCreateList);

            if (leadResponseList == null)
            {
                leadResponseList = new List<LeadCreateResponse>();
                //Save each lead
                _peformanceLog.StartLogDetail("LeadSaveManger.CreateLead.Loop");
                foreach (var createRequest in LeadCreateList)
                {
                    if (SubmissionId.HasValue)
                    {
                        createRequest.SubmissionId = SubmissionId;
                    }

                    createRequest.ProspectId = ProspectId;
                    createRequest.ClientRelationContactId = ClientRelationContactId;
                    bool passedSSV = QuickCheckValidationResult == null ? true : QuickCheckValidationResult.Valid;
                    bool profanity = QuickCheckValidationResult == null || QuickCheckValidationResult.ValidationMessages == null ? false : QuickCheckValidationResult.ValidationMessages.Any(t => t.Key == "ProfanityCheck");
                    LeadCreateResponse leadCreateResponse = leadEngine.CreateLead(createRequest, ProspectId, _peformanceLog, realtimeDelivery, passedSSV, profanity);
                    leadCreateResponse = leadCreateResponse ?? new LeadCreateResponse() { Lead = new LeadDTO() { ProgramProductId = createRequest.ProgramProductId } };
                    leadResponseList.Add(leadCreateResponse);
                }
                _peformanceLog.EndLogDetail();

                SetSessionSave(FeSessionId, leadResponseList);

                _peformanceLog.StartLogDetail("LeadSaveManger.SubmissionSave.Async");
                
                List<Tuple<int, int,DateTime?>> leadIdList = leadResponseList.Where(lc => lc.Success).Select(lc => new Tuple<int, int, DateTime?>(Convert.ToInt32(lc.Lead.LeadId), lc.Lead.ProgramProductId.Value, lc.Lead.DateEntered)).ToList();

                if (leadIdList != null && leadIdList.Count() > 0)
                {
                    if (!realtimeDelivery)
                    {
                        Task.Run(() => SaveSubmissionAndUpdateLeadsAsync(TemplateId, SubmissionId, RawPostData, LeadCreateList[0], ProspectId, ConsumingApplication, FeSessionId, leadIdList, matchResponseType, QuickCheckValidationResult, leadScoringGuid, prospectFlowId));
                    }
                    else
                    {
                        SaveSubmissionAndUpdateLeadsAsync(TemplateId, SubmissionId, RawPostData, LeadCreateList[0], ProspectId, ConsumingApplication, FeSessionId, leadIdList, matchResponseType, QuickCheckValidationResult, leadScoringGuid, prospectFlowId);
                    }
                }
            }

            _peformanceLog.EndLogDetail();


            return leadResponseList;
        }

        public void SaveCrossSellSubmissionMatchResponse(Guid crossSellMatchGuid, String FESessionId, int SubmissionId)
        {
            try
            {
                SubmissionMatchResponse subDto = new SubmissionMatchResponse();
                subDto.MatchResponseTypeId = (int)MatchResponseType.CrossSell;
                subDto.MatchResponseGuid = (System.Guid)crossSellMatchGuid;
                subDto.CreatedDate = DateTime.UtcNow;
                subDto.SubmissionId = SubmissionId;
                dbSubmissionService.SaveSubmissionMatchResponse(subDto);
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex);
                isEx.Save();
            }

        }

        public List<LeadCreateResponse> Execute(int? TemplateId, string FeSessionId, LeadCreationType LeadCreationType, ISApplication? ConsumingApplication, List<LeadSaveData> LeadSaveData, bool IsBeta, string TrackId, string LimboAlternativeCampaignTrackid, bool LimboAlternativeCampaignTrackidUtilized, string TrackingSessionGuid, string MatchResponseGuid, RawPostDataDTO RawPostData, string LeadData, string LeadAdditionalData, int? ProspectId, int? SubmissionId, string InitialLeadId, MatchResponseType? matchResponseType, int? ClientRelationContactId, bool realtimeDelivery, APIValidationResultDTO QuickCheckValidationResult, Guid? leadScoringGuid, int? prospectFlowId)
        {
            List<LeadCreateRequest> leadCreateList = new List<LeadCreateRequest>();
            List<LeadCreateResponse> leadResponseList = new List<LeadCreateResponse>();
            _peformanceLog.StartLogDetail("LeadSaveManger.Execute1");
            bool passedServerSideValidation = QuickCheckValidationResult == null ? true : QuickCheckValidationResult.Valid;
            _peformanceLog.StartLogDetail("LeadSaveManger.BuildLeadCreateRequest.Loop");

            //Build unique LeadCreateRequest (most of the data is the same, just different programProducts)
            foreach (var saveData in LeadSaveData)
            {
                LeadCreationType lct = saveData.LeadCreationType == null ? LeadCreationType : saveData.LeadCreationType.Value;
                LeadCreateRequest leadCreate = EntityBuildHelper.BuildLeadCreateRequestObjectCustomTCPA(saveData.TemplateId, saveData.ProgramProductId, IsBeta, TrackId, LimboAlternativeCampaignTrackid, LimboAlternativeCampaignTrackidUtilized, TrackingSessionGuid, MatchResponseGuid, LeadData, LeadAdditionalData, InitialLeadId, lct, ProspectId, saveData.CustomTCPA);
                leadCreate.AllowedViaLeadScoringUpsell = saveData.AllowedViaLeadScoringUpsell;
                leadCreate.PaidStatusType = saveData.PaidStatusTypeId;
                leadCreate.ProductId = saveData.ProductId;
                leadCreate.PassedValidation = true;
                leadCreate.ExternalMatchItemGuid = saveData.ExternalMatchItemGuid;
                leadCreate.InitiatingURL = FESession.Get<string>(FeSessionId, Constants.INITIATING_URL).Truncate(3000);
                leadCreate.LandingURL = FESession.Get<string>(FeSessionId, Constants.LANDING_URL).Truncate(3000);
                leadCreate.CallCenterURL = FESession.Get<string>(FeSessionId, Constants.CALL_CENTER_URL).Truncate(3000);
                leadCreate.InstitutionId = saveData.InstitutionId;
                leadCreate.InstitutionName = saveData.InstitutionName;

                if (!passedServerSideValidation)
                {
                    leadCreate.PassedValidation = false;
                }

                if (saveData.IsValid == false)
                {
                    leadCreate.PassedValidation = false;
                }

                leadCreateList.Add(leadCreate);
            }

            _peformanceLog.EndLogDetail();

            leadResponseList = GetSessionSave(FeSessionId, leadCreateList);
            if (leadResponseList == null)
            {

                leadResponseList = Execute(TemplateId, leadCreateList, FeSessionId, ConsumingApplication, RawPostData, ProspectId, SubmissionId, matchResponseType, ClientRelationContactId, realtimeDelivery, QuickCheckValidationResult, leadScoringGuid, prospectFlowId);

                //return the leadid in the LeadSaveData list
                foreach (var leadResponse in leadResponseList)
                {
                    if (leadResponse.Success)
                    {
                        var dataItem = LeadSaveData.Where(lsd => lsd.ProgramProductId == leadResponse.Lead.ProgramProductId).FirstOrDefault();

                        if (dataItem != default(LeadSaveData))
                        {
                            dataItem.LeadId = leadResponse.Lead.LeadId;
                        }
                    }
                }

                if (leadResponseList != null)
                {
                    SetSessionSave(FeSessionId, leadResponseList);
                }
            }

            _peformanceLog.EndLogDetail();

            return leadResponseList;
        }

        private void SaveSubmissionAndUpdateLeadsAsync(int? TemplateId, int? submissionId, RawPostDataDTO RawPostData, LeadCreateRequest leadData, int? prospectId, ISApplication? consumingApplication, string feSessionId, List<Tuple<int, int,DateTime?>> leadIdList, MatchResponseType? matchResponseType, APIValidationResultDTO QuickCheckValidationResult, Guid? leadScoringGuid, int? prospectFlowId)
        {
            try
            {
                if (!submissionId.HasValue)
                {
                    submissionId = SaveSubmission(TemplateId, leadData, prospectId, consumingApplication, feSessionId, QuickCheckValidationResult, matchResponseType, 1, leadScoringGuid, prospectFlowId);
                }
                else //if submission has already been saved, and this is a user selection or cross sell, we need to save the optin text for the userselection
                {
                    AddConsentToSubmissionDetail(submissionId.Value, leadData);
                }

                //leadEngine.SaveLeadCreative(leadIdList, leadData.IsBeta, submissionId.Value);
                //todo: create APIValidationResultDTO 
                //SaveSubmissionValidationErrors(submissionId, QuickCheckValidationResult);
                var leadProductList = leadEngine.UpdateLeads(submissionId, RawPostData, leadData.TrackId.Value, leadIdList, leadData.IsBeta);
                ProcessLeadsForEmsLeadService(leadProductList, consumingApplication);
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex);
                isEx.Save();
            }
        }

        //only saves if errors exist
        public void SaveSubmissionValidationErrors(int? submissionId, APIValidationResultDTO QuickCheckValidationResult)
        {
            if (QuickCheckValidationResult != null && !QuickCheckValidationResult.Valid)
            {
                //we need to save the error for this submission
                foreach (KeyValuePair<string, string> s in QuickCheckValidationResult.ValidationMessages)
                {
                    try
                    {
                        SubmissionValidationError subError = new SubmissionValidationError();
                        subError.SubmissionId = submissionId;
                        subError.ValidationRule = s.Key;
                        subError.ErrorMessage = s.Value;
                        subError.CreatedDate = System.DateTime.Now;
                        dbSubmissionService.SaveSubmissionValidationError(subError);
                    }
                    catch (Exception exX)
                    {
                        ISException isEx = new ISException(exX);
                        isEx.Save();
                    }
                }
            }
        }

        public int? SaveSubmission(int? TemplateId, LeadCreateRequest leadData, int? prospectId, ISApplication? consumingApplication, string feSessionId, APIValidationResultDTO QuickCheckValidationResult, MatchResponseType? matchResponseType, int submissionType, Guid? leadScoringGuid, int? prospectFlowId)
        {
            int? submissionId = null;

            try
            {
                LeadScoreDTO lastDTO = FESession.Get<LeadScoreDTO>(feSessionId, Constants.LEADSCORE_SESSION_KEY);
                Submission submissionRecord = BuildSubmissionRecord(TemplateId, leadData, prospectId, consumingApplication, matchResponseType, feSessionId, submissionType, lastDTO, leadScoringGuid, prospectFlowId);

                submissionRecord = dbSubmissionService.SaveSubmission(submissionRecord);
                submissionId = submissionRecord.SubmissionId;
                SaveSubmissionValidationErrors(submissionId, QuickCheckValidationResult);
                if (submissionId > 0 && !String.IsNullOrWhiteSpace(feSessionId))
                {
                    FESession.Set(feSessionId, Constants.SUBMISSION_ID_SESSION_KEY, submissionId);
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex);
                isEx.Save();
            }

            return submissionId;
        }

        private void AddConsentToSubmissionDetail(int submissionId, LeadCreateRequest leadData)
        {
            string[] consentKeys = { "schoolselectionexpressconsent", "crosssellexpressconsent" }; //these are hardcoded in the javascript, no way around this "hack" until they are made proper controls

            try
            {
                var consent = leadData.LeadData.Where(ld => consentKeys.Contains(ld.Key.ToLowerInvariant()));

                if (consent != null && consent.Count() > 0)
                {
                    SubmissionDetailAdditional subDto = new SubmissionDetailAdditional();

                    subDto.SubmissionId = submissionId;
                    subDto.SubmissionDetailAdditionalKey = consent.First().Key;
                    subDto.SubmissionDetailAdditionalValue = consent.First().Value;

                    dbSubmissionService.SaveSubmissionDetailAdditional(subDto);
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex);
                isEx.Save();
            }
        }

        private  List<SubmissionMatchResponse> GetSubmissionMatchResponses(string feSessionid)
        {
            var Result = new List<SubmissionMatchResponse>();

            Guid? ssMatchResponseGuid = FESession.Get<Guid?>(feSessionid, Constants.SUBMISSION_SS_MATCHRESPONSEGUID);

            if(ssMatchResponseGuid.HasValue)
            {
                Result.Add(new SubmissionMatchResponse() {

                    MatchResponseTypeId = (int)MatchResponseType.SchoolSelection,
                     CreatedDate = DateTime.UtcNow,
                      MatchResponseGuid = ssMatchResponseGuid.Value
                });
                FESession.Remove(feSessionid, Constants.SUBMISSION_SS_MATCHRESPONSEGUID);
            }

            Guid? smMatchResponseGuid = FESession.Get<Guid?>(feSessionid, Constants.WIZARD_ME_TOKEN_SMARTMATCHRESULTS_KEY);

            if (smMatchResponseGuid.HasValue)
            {
                Result.Add(new SubmissionMatchResponse()
                {

                    MatchResponseTypeId = (int)MatchResponseType.SmartMatch,
                    CreatedDate = DateTime.UtcNow,
                    MatchResponseGuid = smMatchResponseGuid.Value
                });

                FESession.Remove(feSessionid, Constants.WIZARD_ME_TOKEN_SMARTMATCHRESULTS_KEY);
            }

            return Result;

        }

        private Submission BuildSubmissionRecord(int? TemplateId, LeadCreateRequest leadData, int? prospectId, ISApplication? consumingApplication, MatchResponseType? matchResponseType, string feSessionid, int submissionType, LeadScoreDTO lastDTO, Guid? leadScoringGuid, int? prospectFlowId)
        {
            Submission submission = new Submission();

            Dictionary<string, int> templateControlCodes = dbTemplateService.GetStandardControlCodeDictionary();
            Dictionary<int, Tuple<string, string>> submissionDetailValues = new Dictionary<int, Tuple<string, string>>();
            Dictionary<string, string> submissionDetailAdditionalValues = new Dictionary<string, string>();

            var foundLeadDataControls = (from ld in leadData.LeadData
                                         join cc in templateControlCodes on ld.Key.ToLowerInvariant() equals cc.Key.ToLowerInvariant()
                                         select new { StandardControlCodeId = cc.Value, Value = ld.Value, Code = cc.Key });

            submissionDetailValues = foundLeadDataControls.ToDictionary(key => key.StandardControlCodeId, value => new Tuple<string, string>(value.Value, value.Code));

            var nonTemplateControlData = from ld in leadData.LeadData
                                         join cc in templateControlCodes on ld.Key.ToLowerInvariant() equals cc.Key.ToLowerInvariant() into nontemplate
                                         from cc in nontemplate.DefaultIfEmpty()
                                         where default(KeyValuePair<string, int>).Equals(cc)
                                         select ld;

            submissionDetailAdditionalValues = nonTemplateControlData.ToDictionary(key => key.Key, value => value.Value);

            int wizardTemplateId = 0;
            if (TemplateId == null) //apollo and host and post will pass in null so use the program template id as has been used
                wizardTemplateId = leadData.TemplateId.Value;
            else //others should pass in a template id. record that as its the form engine template id. 
                wizardTemplateId = Convert.ToInt32(TemplateId);

            submission.TemplateId = wizardTemplateId;
            submission.IsBeta = leadData.IsBeta;
            submission.CampaignTrackId = leadData.TrackId;
            submission.TrackingSessionGuid = leadData.TrackingSessionGUID;
            submission.LimboAlternativeCampaignTrackid = leadData.LimboAlternativeCampaignTrackid;
            submission.LimboAlternativeCampaignTrackidUtilized = leadData.LimboAlternativeCampaignTrackidUtilized;
            submission.ProspectId = prospectId;
            submission.ConsumingApplicationId = (int?)consumingApplication;
            submission.CreatedDate = DateTime.UtcNow;
            submission.ServerName = System.Environment.MachineName;
            submission.ProspectFlowId = prospectFlowId;
            submission.WidgetName = leadData.WidgetName;
            submission.WidgetRequestGuid = leadData.WidgetRequestGuid;

            if (!leadData.IsUrlDerived)
                ProspectInputBuilder.AggregateCreativeURLs(leadData);
            submission.InitiatingURL = leadData.InitiatingURL.Truncate(3000);
            submission.LandingURL = leadData.LandingURL.Truncate(3000);
            submission.CallCenterURL = leadData.CallCenterURL.Truncate(3000);

            if(!string.IsNullOrEmpty(leadData.VideoUrl))
                submission.VideoUrl = leadData.VideoUrl.Truncate(3000);

            var limboReason = FESession.Get(feSessionid, Constants.WIZARD_ME_LIMBOREASON_KEY);

            if (limboReason != null)
            {
                submission.LimboReasonId = (int)limboReason;
            }

            if (lastDTO != null && lastDTO.Response != null)
            {
                submission.SubmissionLeadScoringResponses = new List<SubmissionLeadScoringResponse>();

                submission.SubmissionLeadScoringResponses.Add(new SubmissionLeadScoringResponse() { LeadScoringGuid = lastDTO.Response.LeadScoringGuid });
            }
            else if(leadScoringGuid.HasValue)
            {
                submission.SubmissionLeadScoringResponses = new List<SubmissionLeadScoringResponse>();

                submission.SubmissionLeadScoringResponses.Add(new SubmissionLeadScoringResponse() { LeadScoringGuid = leadScoringGuid.Value });
            }

            submission.SubmissionDetails = new List<SubmissionDetail>();
            submission.SubmissionDetailAdditionals = new List<SubmissionDetailAdditional>();
            submission.SubmissionMatchResponses = new List<SubmissionMatchResponse>();
            submission.SubmissionTypeId = submissionType;
            submission.SubmissionMatchResponses = GetSubmissionMatchResponses(feSessionid);
            
            foreach (var item in submissionDetailValues)
            {
                SubmissionDetail detail = new SubmissionDetail();

                switch (item.Value.Item2.ToLower()) //StandardControlCode.Code
                {
                    case "categories":
                    case "subcategories":
                    case "specialties":
                        DeserializeControlCodeValue(item.Key, item.Value.Item1, submission.SubmissionDetails);
                        break;
                    case "birthdate": //do not add birth date
                        detail.StandardControlCodeId = item.Key;
                        detail.SubmissionDetailValue = "01/01/0001"; //do not store birth date information in this table ever - "Compliance"

                        submission.SubmissionDetails.Add(detail);
                        break;
                    default:
                        detail.StandardControlCodeId = item.Key;
                        detail.SubmissionDetailValue = item.Value.Item1.Substring(0, Math.Min(item.Value.Item1.Length, 4000));

                        submission.SubmissionDetails.Add(detail);
                        break;
                }
            }

            foreach (var item in submissionDetailAdditionalValues)
            {
                if (!keysToIgnoreInAdditionalDetails.Contains(item.Key))
                {
                    SubmissionDetailAdditional additionalDetail = new SubmissionDetailAdditional();
                    additionalDetail.SubmissionDetailAdditionalKey = item.Key.Substring(0, Math.Min(item.Key.Length, 200));
                    additionalDetail.SubmissionDetailAdditionalValue = item.Value.Substring(0, Math.Min(item.Value.Length, 4000));

                    submission.SubmissionDetailAdditionals.Add(additionalDetail);
                }
            }

            if (submissionDetailAdditionalValues.TryGetValue("LeadSourceUrl", out string leadSourceUrl))
            {
                submission.LeadSourceUrl = HttpUtility.UrlDecode(leadSourceUrl).Truncate(3000);
            }

            if (submissionDetailAdditionalValues.TryGetValue("LeadInitiatingUrl", out string leadInitiatingUrl))
            {
                submission.LeadInitiatingUrl = HttpUtility.UrlDecode(leadInitiatingUrl).Truncate(3000);
            }

            if (submissionDetailAdditionalValues.TryGetValue("FormLeadUrl", out string formLeadUrl))
            {
                submission.FormLeadUrl = HttpUtility.UrlDecode(formLeadUrl).Truncate(3000); 
            }

            if (submissionDetailAdditionalValues.TryGetValue("VideoUrl", out string videoUrl))
            {
                submission.VideoUrl = HttpUtility.UrlDecode(videoUrl).Truncate(3000);
            }

            return submission;
        }

        private void DeserializeControlCodeValue(int standardControlCodeId, string commaString, ICollection<SubmissionDetail> submissionDetailList)
        {
            List<string> valueList = commaString.Split(',').Select(f => f).ToList();

            foreach (var val in valueList)
            {
                submissionDetailList.Add(new SubmissionDetail() { StandardControlCodeId = standardControlCodeId, SubmissionDetailValue = val });
            }

            return;
        }

        private List<LeadCreateResponse> GetSessionSave(string FESessionId, List<LeadCreateRequest> LeadCreateList)
        {
            List<LeadCreateResponse> Result = null;
            bool MultipleSaveProtection = false;
            if (!string.IsNullOrEmpty(FESessionId) && bool.TryParse(ConfigurationManager.AppSettings.Get("MultipleSaveProtection"), out MultipleSaveProtection) && MultipleSaveProtection == true)
            {
                string ProgramProducts = String.Join("-", LeadCreateList.Select(l => l.ProgramProductId.ToString()).ToArray());
                string Key = string.Format(Constants.LEAD_SAVE_CACHE_KEY, FESessionId, ProgramProducts);

                if (FormsEngineCacheProxy.Cache.Get(Key) != null)
                {
                    Result = FormsEngineCacheProxy.Cache.Get<List<LeadCreateResponse>>(Key);
                }
            }

            return Result;
        }

        private void SetSessionSave(string FESessionId, List<LeadCreateResponse> LeadCreateResponseList)
        {
            bool MultipleSaveProtection = false;
            if (!string.IsNullOrEmpty(FESessionId) && bool.TryParse(ConfigurationManager.AppSettings.Get("MultipleSaveProtection"), out MultipleSaveProtection) && MultipleSaveProtection == true)
            {

                string ProgramProducts = String.Join("-", LeadCreateResponseList.Select(l => l.Lead.ProgramProductId.ToString()).ToArray());
                string Key = string.Format(Constants.LEAD_SAVE_CACHE_KEY, FESessionId, ProgramProducts);
                int Expiration;

                if (!Int32.TryParse(ConfigurationManager.AppSettings.Get("MultipleSaveProtectionMinutes"), out Expiration))
                {
                    Expiration = 10;
                }

                if (LeadCreateResponseList != null)
                {
                    FormsEngineCacheProxy.Cache.Set(Key, LeadCreateResponseList, Expiration);
                }
            }
        }


        private void ProcessLeadsForEmsLeadService(List<Tuple<int, int>> leadProductList, ISApplication? consumingApplication)
        {
            try
            {
                _peformanceLog.StartLogDetail("LeadSaveManger.ProcessLeadsForEmsLeadService");

                HashSet<int> emsProductIds = new HashSet<int>() { Constants.EMS_PRODUCTID, Constants.EMS_PPI_PRODUCTID };
                var emsLeadIdList = leadProductList.Where(l => emsProductIds.Contains(l.Item2)).Select(l => l.Item1).ToList();

                if (emsLeadIdList.Count() > 0 && consumingApplication != ISApplication.Apollo)
                {
                    Task.Run(() => FormsEngineService.RelatedServices.SendLeadsToEmsLeadService(emsLeadIdList));
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex, "LeadIds To Be Sent To EMS Lead Service", leadProductList);
                isEx.Save();
            }
        }

    }
}
