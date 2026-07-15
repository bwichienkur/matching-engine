using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine;
using EDDY.IS.FormsEngine.Services.Logging;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Services.Controllers.Base;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Util.HTMLExtensions;
using System.Linq;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.Core;
using System.Web;
using EDDY.IS.Validation;
using EDDY.IS.FormsEngine.DTO.Extended;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.Base;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
   
    /// <summary>
    /// Razor TemplateManager
    /// </summary>
    public class TemplateManagerController : TemplateManagerControllerBase
    {
        public static FormsEngine FormsEngine = new IS.FormsEngine.FormsEngine(); 
        
        [HttpGet]
        public ActionResult GetProgramTemplate(int? TemplateId, int? ProgramProductId, int? ProgramId, int InstitutionId, string RenderingStrategy, bool IsBeta, string AlternativeTemplates, bool? IgnoreTemplateCache, string Theme, int? ApplicationId, string TrackId, string DeviceId, int? ProductId, int? PaidStatusTypeId)
        {
            TemplateResultDTO Result = null;
            try
            {
                TemplateId = TemplateId.HasValue ? TemplateId : FormsEngineService.GetTemplateIdOrDefault(-1, false, ProgramId, ProgramProductId);
                RenderingStrategy = string.IsNullOrWhiteSpace(RenderingStrategy) || RenderingStrategy.ToLower() == "null" || RenderingStrategy.ToLower() == "undefined" ? GetDefaultRenderingStrategy(false) : RenderingStrategy;
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetProgramTemplate", null, TemplateId, ProgramProductId, ProgramId, InstitutionId, RenderingStrategy, IsBeta, AlternativeTemplates, IgnoreTemplateCache, Theme, ApplicationId, TrackId, DeviceId);
                Log.StartLogDetail("TemplateManagerController.GetProgramTemplate");
                Result = GetProgramTemplateDTO(TemplateId.Value, ProgramProductId, ProgramId, InstitutionId, RenderingStrategy, IsBeta, AlternativeTemplates, IgnoreTemplateCache, Theme, ApplicationId, TrackId, DeviceId, ProductId, PaidStatusTypeId, Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TemplateId, ProgramProductId, ProgramId, InstitutionId, RenderingStrategy, IsBeta, AlternativeTemplates, IgnoreTemplateCache, Theme, ApplicationId, TrackId, DeviceId);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetQDFTemplate(int TemplateId, string RenderingStrategy, bool IsBeta, bool? IgnoreTemplateCache, string Theme, string FESessionId, string TrackId, string IPOverride = "")
        {
            TemplateResultDTO Result = new TemplateResultDTO();

            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetQDFTemplate", null, TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, Theme, FESessionId);
                Log.StartLogDetail("TemplateManagerController.GetQDFTemplate");

                //Find template
                Result = GetQDFTemplateDTO(TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, Theme, FESessionId, Log, false);

                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                Result.IsLocalIP = true;
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, Theme, FESessionId, TrackId, IPOverride);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetWizardTemplate(int TemplateId, string RenderingStrategy, bool IsBeta, bool? IgnoreTemplateCache, string Theme, string FESessionId, string TrackId, string IPOverride = "")
        {
            TemplateResultDTO Result = new TemplateResultDTO();            

            try
            {                  
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetWizardTemplate", null, TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, Theme, FESessionId);
                Log.StartLogDetail("TemplateManagerController.GetWizardTemplate");

                //Find template
                var Template = FormsEngineService.GetShallowTemplate(TemplateId);
                // Determine the international template id..
                String InternationalTemplateIdSettingValue = ConfigurationManager.AppSettings.Get("InternationalWizardTemplatePaidId");
                int internationalTemplateId = -1;
                var UseInternationalOverride = false;
                var IsLocalIP = true;
                string countryCode = "";
                var trackIdGuid = new Guid();
                if (Guid.TryParse(TrackId, out trackIdGuid) && (FormTemplateTypes)Template.TemplateTypeId == FormTemplateTypes.WizardTemplate)
                {
                    try
                    {
                        // Determine if this is an international IP address..
                        var ipString = !String.IsNullOrEmpty(IPOverride) ? IPOverride : GetClientIPAddress(Request);
                        countryCode = FormsEngineService.RelatedServices.GetCountryCodeByIP(ipString);
                        IsLocalIP = string.IsNullOrEmpty(countryCode) || (new string[] { "us", "ca" }).Contains(countryCode.ToLower());
                        var CampaignDetail = FormsEngineService.RelatedServices.GetCampaignDetailByTrackId(trackIdGuid);

                        // if SEO then use that International Template..
                        if (CampaignDetail.ChannelId == (int) MarketingChannels.SEO)
                        {
                            InternationalTemplateIdSettingValue = ConfigurationManager.AppSettings.Get("InternationalWizardTemplateSEOId") ?? InternationalTemplateIdSettingValue;
                        }

                        UseInternationalOverride = CampaignDetail.UseInternationalTemplate && !IsLocalIP && int.TryParse(InternationalTemplateIdSettingValue, out internationalTemplateId) && internationalTemplateId > 0;
                    }
                    catch (Exception ex)
                    {
                        UseInternationalOverride = false;
                        WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, Theme, FESessionId, TrackId, IPOverride, countryCode);
                    }

                }

                // If international and there is an international template id override the current template..
                // The old templateId will be assigned to Result.IgnoredTemplateId..
                // The internationalTemplateId will be assigned to Result.TemplateId..
                if (UseInternationalOverride)
                {
                    Result = GetWizardTemplateDTO(TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, Theme, FESessionId, Log, true, internationalTemplateId, countryCode);
                }
                else
                {
                    Result = GetWizardTemplateDTO(TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, Theme, FESessionId, Log, false);
                    if (IsLocalIP)
                    {
                        Result.IsLocalIP = IsLocalIP;
                    }
                }
                
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                Result.IsLocalIP = true;
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, Theme, FESessionId, TrackId, IPOverride);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public string GetWizardTemplateJS(int TemplateId, string RenderingStrategy, bool IsBeta, bool? IgnoreTemplateCache)
        {
            string Result = "";
            bool FEDebugMode = true;
            try
            {
                FEDebugMode = Request.Cookies["FE_DebugMode"] == null;
                Response.ContentType = "application/javascript";
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetWizardTemplateJS", null, TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache);
                Log.StartLogDetail("TemplateManagerController.GetWizardTemplateJS");
                Result = GetWizardTemplateControlsJS(TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, ref Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
                if (FEDebugMode)
                {
                    var minifier = new Microsoft.Ajax.Utilities.Minifier();
                    Result = minifier.MinifyJavaScript(Result);
                }
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache);
            }
            return Result;
        }


        [HttpGet]
        public ActionResult GetAdditionalTemplateQuestions(int WizardTemplateId, bool IsBeta, string TrackId, string SessionId, string MatchId, string LeadData, string AdditionalData, int PreviousSMLeadCount, int PreviousUSLeadCount, string DeviceId, string FESessionId, bool UseSmartMatch, FormTemplateTypes FormTemplateType, int? ProgramTemplateId, int? ApplicationId, bool? IsWizardAnyMatch, int? ProspectId, int? InstitutionId, int? ProgramProductId, int? ProductId)
        {
            TemplateControlResultDTO Result = new TemplateControlResultDTO(); 
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetAdditionalTemplateQuestions", null, WizardTemplateId, IsBeta, TrackId, SessionId, MatchId, LeadData, AdditionalData, PreviousSMLeadCount, PreviousUSLeadCount, DeviceId, FESessionId, UseSmartMatch, FormTemplateType, ProgramTemplateId);

                if (!UseSmartMatch || FormTemplateType == FormTemplateTypes.ProgramWizard)
                {
                    Log.StartLogDetail("TemplateManagerController.GetWizardTemplateAdditionalControls");
                    Result = GetWizardTemplateAdditionalControls(WizardTemplateId, IsBeta, TrackId, SessionId, MatchId, LeadData, AdditionalData, PreviousSMLeadCount, PreviousUSLeadCount, FESessionId, FormTemplateType, ProgramTemplateId, ApplicationId, IsWizardAnyMatch, ProspectId, InstitutionId, ProgramProductId, ProductId, Log);
                    Log.EndLogDetail();
                }
                else 
                {
                    Log.StartLogDetail("TemplateManagerController.GetWizardTemplateAdditionalControlsFromSM");
                    Result = GetWizardTemplateAdditionalControlsFromSM(WizardTemplateId, IsBeta, TrackId, SessionId, MatchId, LeadData, AdditionalData, PreviousSMLeadCount, PreviousUSLeadCount, DeviceId, FESessionId, ApplicationId, Log);
                    Log.EndLogDetail();
                }
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, ex, WizardTemplateId, IsBeta, TrackId, SessionId, MatchId, LeadData, AdditionalData, PreviousSMLeadCount, PreviousUSLeadCount, DeviceId, FESessionId, UseSmartMatch, FormTemplateType, ProgramTemplateId);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetAdditionalTemplateQuestionCollection(string FESessionId, int WizardTemplateId, string ProgramTemplateIds, string RenderingStrategy, string TrackId, string FormFilterValues, bool IsBeta, bool? IgnoreTemplateCache)
        {
            JsonpResult jsonResult = new JsonpResult();

            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetAdditionalTemplateQuestionCollection", null, WizardTemplateId, RenderingStrategy, TrackId, FormFilterValues, IsBeta, IgnoreTemplateCache);
                List<int> programTemplateIds = ProgramTemplateIds.Split(',').Select(int.Parse).ToList();

                bool renderingStrategyIsSchoolPickerWizard = RenderingStrategy.ToUpper() == Constants.SCHOOLPICKERWIZARD_RENDERINGSTRATEGY;

                if (renderingStrategyIsSchoolPickerWizard)
                {
                    Log.StartLogDetail("TemplateManagerController.GetInstitutionWizardTemplateAdditionalControlCollection");
                    TemplateControlResultDTO templateControlResult = GetSchoolPickerWizardTemplateAdditionalControlCollection(WizardTemplateId, programTemplateIds, FESessionId, ref Log);
                    Log.EndLogDetail();

                    jsonResult = new JsonpResult(templateControlResult);
                }
                else
                {

                    Log.StartLogDetail("TemplateManagerController.GetWizardTemplateAdditionalControlCollection");
                    List<TemplateAdditionalQuestionDTO> additionalQuestionDTOs = GetWizardTemplateAdditionalControlCollection(WizardTemplateId, programTemplateIds, ref Log);
                    if (additionalQuestionDTOs.Count > 0)
                    {
                        string resToStrForSession = string.Empty;
                        foreach (TemplateAdditionalQuestionDTO taq in additionalQuestionDTOs)
                        {
                            resToStrForSession += taq.TemplateId + "," + taq.RenderedControl + ";";
                        }
                        FESession.Set(FESessionId, Constants.WIZARD_ADDITIONALQPERTEMPLATE_KEY, resToStrForSession.Substring(0, resToStrForSession.Length - 1));
                    }
                    Log.EndLogDetail();

                    jsonResult = new JsonpResult(additionalQuestionDTOs);
                }

                Log.EndLog(jsonResult);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, WizardTemplateId, RenderingStrategy, TrackId, FormFilterValues, IsBeta, IgnoreTemplateCache);
            }

            return jsonResult;
        }


        [HttpGet]
        public ActionResult GetMatchingEngineWizardResponse(bool IsBeta, int WizardTemplateId, string TrackId, string SessionId, string MatchId, string LeadData, string AdditionalData, int PreviousSMLeadCount, int PreviousUSLeadCount, string FESessionId, string DeviceId, FormTemplateTypes FormTemplateType, int? ProgramTemplateId, int? ApplicationId)
        {
            MatchingEngine.WizardMatchResponse Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetMatchingEngineWizardResponse", null, IsBeta, WizardTemplateId, TrackId, SessionId, MatchId, LeadData, AdditionalData, PreviousSMLeadCount, PreviousUSLeadCount, FESessionId, DeviceId, FormTemplateType, ProgramTemplateId, ApplicationId);
                Log.StartLogDetail("TemplateManagerController.GetMatchingEngineWizardResponse");
                Result = GetSmartMatchesForWizard(IsBeta, WizardTemplateId, TrackId, SessionId, MatchId, LeadData, AdditionalData, PreviousSMLeadCount, PreviousUSLeadCount, FESessionId, true, DeviceId, false, ApplicationId, true, Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, IsBeta, WizardTemplateId, TrackId, SessionId, MatchId, LeadData, AdditionalData, PreviousSMLeadCount, PreviousUSLeadCount, FESessionId, DeviceId, FormTemplateType, ProgramTemplateId);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult AnySchoolMatches(bool IsBeta, int WizardTemplateId, string TrackId, string SessionId, string MatchId, string LeadData, string AdditionalData, string FESessionId, string DeviceId, FormTemplateTypes FormTemplateType, int? ProgramTemplateId, int? ApplicationId)
        {
            bool Result = true;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.AnySchoolAvailable", null, IsBeta, WizardTemplateId, TrackId, SessionId, MatchId, LeadData, AdditionalData, FESessionId, DeviceId, FormTemplateType, ProgramTemplateId);
                Log.StartLogDetail("TemplateManagerController.GetSmartMatchesForWizard");
                MatchingEngine.WizardMatchResponse wmr = GetSmartMatchesForWizard(IsBeta, WizardTemplateId, TrackId, SessionId, MatchId, LeadData, AdditionalData, 0, 0, FESessionId, true, DeviceId, true, ApplicationId, true, Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
                Result = wmr.ResultCount > 0;
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, IsBeta, WizardTemplateId, TrackId, SessionId, MatchId, LeadData, AdditionalData, FESessionId, DeviceId, FormTemplateType, ProgramTemplateId);
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult GetManagedChoice(int TemplateId, string RenderingStrategy, bool IsBeta, string TrackId, string SessionId, string MatchGuid, int? ProspectId, string LeadData, string AdditionalData, int SMLeadsCreatedCount, int USLeadsCreatedCount, bool? SplitCampusTypeInResults, string FESessionId, string DeviceId, string RenderingExperience, string LimboAlternativeCampaignTrackid, bool LimboAlternativeCampaignTrackidUtilized, FormTemplateTypes FormTemplateType, int? ProgramTemplateId, int? ProgramProductId, int? ProductId, int? InstitutionId, int? ApplicationId, string InstitutionName, string ProgramName)
        {
            ManagedChoiceResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetManagedChoice", null, TemplateId, RenderingStrategy, IsBeta, TrackId, SessionId, MatchGuid, ProspectId, LeadData, AdditionalData, SMLeadsCreatedCount, USLeadsCreatedCount, SplitCampusTypeInResults, FESessionId, DeviceId, RenderingExperience, LimboAlternativeCampaignTrackid, LimboAlternativeCampaignTrackidUtilized, FormTemplateType, ProgramTemplateId, ProgramProductId, InstitutionId, ApplicationId, InstitutionName, ProgramName);
                Log.StartLogDetail("TemplateManagerController.GetManagedChoice");
                Result = ProcessWizardSubmit(TemplateId, RenderingStrategy, IsBeta, TrackId, SessionId, MatchGuid, ProspectId, LeadData, AdditionalData, SMLeadsCreatedCount, USLeadsCreatedCount, SplitCampusTypeInResults, FESessionId, DeviceId, RenderingExperience, LimboAlternativeCampaignTrackid, LimboAlternativeCampaignTrackidUtilized, FormTemplateType, ProgramTemplateId, ProgramProductId, ProductId, InstitutionId, ApplicationId, InstitutionName, ProgramName, Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TemplateId, RenderingStrategy, IsBeta, TrackId, SessionId, MatchGuid, ProspectId, LeadData, AdditionalData, SMLeadsCreatedCount, USLeadsCreatedCount, SplitCampusTypeInResults, FESessionId, DeviceId, FormTemplateType, ProgramTemplateId, ProgramProductId, ProductId, InstitutionId, ApplicationId);
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult SaveProspectJobContactMe(string TrackId, string SessionId, string MatchGuid, int? ProspectId, string LeadData, string AdditionalData, string DeviceId, string FESessionId, int? TemplateId, bool IsBeta, string SiteURL,string Job, string PostalCode)
        {
            bool Result = true;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.SaveProspectJobContactMe", null, TrackId, SessionId, MatchGuid, ProspectId, LeadData, AdditionalData, DeviceId, FESessionId, TemplateId, IsBeta, SiteURL, Job, PostalCode);
                Log.StartLogDetail("TemplateManagerController.SaveProspectForJobContactMe");
                SaveProspectForJobContactMe(TrackId, SessionId, MatchGuid, ProspectId, LeadData, AdditionalData, DeviceId, FESessionId, Log, TemplateId, IsBeta, SiteURL,Job, PostalCode);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TrackId, SessionId, MatchGuid, ProspectId, LeadData, AdditionalData, DeviceId, FESessionId);
            }
            return new JsonpResult(Result);
        }



        [HttpGet]
        public ActionResult GetThankYou(string FESessionId, int? TemplateId, string Theme, string RenderingStrategy, bool IsBeta, string TrackId, int? ApplicationId, bool useNewEndpoint, string callback)
        {
            ThankYouResultDTO Result = null;
            try
            {
                if (useNewEndpoint)
                {
                    //return RedirectToAction(actionName: nameof(ThankYouController.GetThankYouPage), controllerName: "ThankYou", new { FESessionId, TemplateId, Theme, RenderingStrategy, IsBeta, TrackId, ApplicationId, callback });
                }

                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetThankYou", null, FESessionId, TemplateId, Theme, RenderingStrategy, IsBeta);
                Log.StartLogDetail("TemplateManagerController.GetThankYouResultDTO");
                Result = GetThankYouResultDTO(FESessionId, TemplateId, Theme, RenderingStrategy, IsBeta, ref Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, FESessionId, TemplateId, Theme, RenderingStrategy, IsBeta);
            }

            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetNoMatch(string FESessionId, string Theme, string RenderingStrategy, bool IsBeta, bool GenericNoMatch)
        {
            NoMatchResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetNoMatch", null, Theme, RenderingStrategy, IsBeta, GenericNoMatch);
                Log.StartLogDetail("TemplateManagerController.GetNoMatch");
                Result = GetNoMatchResultDTO(FESessionId, Theme, RenderingStrategy, IsBeta, GenericNoMatch, ref Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, Theme, RenderingStrategy, IsBeta, GenericNoMatch);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult CheckMobileNumbers(string phone1, string phone2)
        {
            ExpressConsentCheckDTO result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.CheckMobileNumbers", null, phone1, phone2);
                Log.StartLogDetail("TemplateManagerController.CheckForMobile");
                result = CheckForMobile(phone1, phone2, Log);
                Log.EndLogDetail();
                Log.EndLog(result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, phone1, phone2);
            }
            return new JsonpResult(result);
        }

        [HttpGet]
        public ActionResult GetRenderingStrategies(bool Wizard)
        {
            List<DTO.HTMLRenderingStrategyDTO> Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetRenderingStrategies", null, Wizard);
                Log.StartLogDetail("TemplateManagerController.GetRenderingStrategies");
                FormTemplateTypes FormTemplateType = Wizard ? FormTemplateTypes.ProgramWizard : FormTemplateTypes.ProgramTemplate;
                Result = FormsEngineService.GetRenderingStrategies(FormTemplateType);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, Wizard);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetRenderingStrategiesByType(FormTemplateTypes FormTemplateType)
        {
            List<DTO.HTMLRenderingStrategyDTO> Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetRenderingStrategiesByType", null, FormTemplateType);
                Log.StartLogDetail("TemplateManagerController.GetRenderingStrategiesByType");
                Result = FormsEngineService.GetRenderingStrategies(FormTemplateType);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, FormTemplateType);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetThemes(string RenderingStrategyName)
        {
            List<DTO.ThemeDTO> Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetThemes", null, RenderingStrategyName);
                Log.StartLogDetail("TemplateManagerController.GetThemes");
                Result = GetRenderingStrategyThemes(RenderingStrategyName);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, RenderingStrategyName);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetTemplateListByType(FormTemplateTypes FormTemplateType, BusinessDivisionType BusinessDivisionType = BusinessDivisionType.ProspectDelivery, int? institutionId = null)
        {
            List<DTO.TemplateBasicDTO> Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetTemplateListByType", null, FormTemplateType, BusinessDivisionType);
                Log.StartLogDetail("TemplateManagerController.GetTemplateListByType");
                Result = FormsEngineService.GetShallowTemplateList(FormTemplateType, BusinessDivisionType, institutionId);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, FormTemplateType);
            }

            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetTemplateList(bool Wizard)
        {
            List<DTO.TemplateBasicDTO> Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetTemplateList", null, Wizard);
                FormTemplateTypes FormTemplateType = Wizard ? FormTemplateTypes.WizardTemplate : FormTemplateTypes.ProgramTemplate;
                Log.StartLogDetail("TemplateManagerController.GetTemplateList");
                Result = FormsEngineService.GetShallowTemplateList(FormTemplateType);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, Wizard);
            }
            return new JsonpResult(Result);
        }

    
        [HttpGet]
        public ActionResult ValidateForm(int ProgramProductId, bool IsBeta, string TrackId, string LeadData)
        {
            APIValidationResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.ValidateForm", null, ProgramProductId, IsBeta, TrackId, LeadData);
                Log.StartLogDetail("TemplateManagerController.ValidateForm");
                Result = FormsEngineService.ValidateForm(ProgramProductId, IsBeta, TrackId, LeadData, true, ref Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, ProgramProductId, IsBeta, TrackId, LeadData);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult ProcessHostAndPostLead(int ProgramProductId, bool IsBeta, string TrackId, string LeadData)
        {
            APISubmissionResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.ProcessHostAndPostLead", null, ProgramProductId, IsBeta, TrackId, LeadData);
                Log.StartLogDetail("TemplateManagerController.BuildRawDataObject");
                RawPostDataDTO RawData = BuildRawDataObject(LeadData);
                Log.EndLogDetail();
                Log.StartLogDetail("TemplateManagerController.ProcessHostAndPostLead");
                Result = FormsEngineService.ProcessHostAndPostLead(ProgramProductId, IsBeta, TrackId, LeadData, ref RawData, ref Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, ProgramProductId, IsBeta, TrackId, LeadData);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetProgramMatches(bool IsBeta, string TrackId, string LeadData)
        {
            APIProgramMatchesDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetProgramMatches", null, IsBeta, TrackId, LeadData);
                Log.StartLogDetail("TemplateManagerController.GetProgramMatches");
                Result = FormsEngineService.GetProgramMatches(IsBeta, TrackId, LeadData, Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, IsBeta, TrackId, LeadData);
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult SaveProspect(bool IsBeta, string SessionId, string LeadData, string AdditionalData, string TrackId, string FESessionId, int ApplicationId)
        {
            int Result = 0;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.SaveProspect", null, IsBeta, SessionId, LeadData, AdditionalData);
                Log.StartLogDetail("TemplateManagerController.SaveProspect");
                var ProspectResult = FormsEngineService.RelatedServices.SaveProspect(IsBeta, SessionId, LeadData, AdditionalData,TrackId, ApplicationId);
                FESession.Set(FESessionId, Constants.PROSPECT_WORKFLOWID, ProspectResult.ProspectFlowId);
                Result = ProspectResult.ProspectId;
                Log.EndLogDetail();
                Log.EndLog(ProspectResult);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, IsBeta, SessionId, LeadData, AdditionalData);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult SaveProspectAdditionalInfo(int? ProspectId, string Email, string ProspectAdditionalData)
        {
            int Result = 0;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.SaveProspectAdditionalInfo", null, ProspectId, Email, ProspectAdditionalData);
                Log.StartLogDetail("TemplateManagerController.SaveProspectAdditionalInfo");
                Result = FormsEngineService.RelatedServices.SaveProspectAdditionalInfo(ProspectId, Email, ProspectAdditionalData);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, ProspectId, ProspectAdditionalData);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult ProcessSubmit(int TemplateId, int ProgramProductId, int? ProductId, bool IsBeta, string TrackId, 
            string SessionId, int? ProspectId, string RenderingStrategy, int InstitutionId, string InstitutionName,
            string ProgramName, string MatchGuid, string LeadData, string AdditionalData, string FESessionId, string DeviceId
            , int? ApplicationId, string AlternativeTemplates)
        {
            SubmissionResultDTO Result = null;
            try
            {
                RenderingStrategy = string.IsNullOrWhiteSpace(RenderingStrategy) || RenderingStrategy.ToLower() == "null" || RenderingStrategy.ToLower() == "undefined" ? GetDefaultRenderingStrategy(false) : RenderingStrategy;
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.ProcessSubmit", null, TemplateId, ProgramProductId, IsBeta, TrackId, SessionId, ProspectId, RenderingStrategy, InstitutionId, InstitutionName, ProgramName, MatchGuid, LeadData, AdditionalData, FESessionId, DeviceId, ApplicationId);
                RawPostDataDTO RawData = BuildRawDataObject(LeadData);
                Log.StartLogDetail("TemplateManagerController.ProcessSubmit");
                Result = ProcessSubmit(TemplateId, ProgramProductId, ProductId.HasValue?ProductId.Value:0, IsBeta, TrackId, SessionId, ProspectId, RenderingStrategy, InstitutionId, InstitutionName, ProgramName, MatchGuid, LeadData, AdditionalData, ref RawData, ref Log, FESessionId, DeviceId, ApplicationId, AlternativeTemplates);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TemplateId, ProgramProductId, IsBeta, TrackId, SessionId, ProspectId, RenderingStrategy, InstitutionId, InstitutionName, ProgramName, MatchGuid, LeadData, AdditionalData, FESessionId, DeviceId, ApplicationId);
                
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetOptimizelyCrossSell(int TemplateId, bool IsBeta, string RenderingStrategy)
        {
            SubmissionResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetOptimizelyCrossSell", null, TemplateId, IsBeta, RenderingStrategy);
                Log.StartLogDetail("TemplateManagerController.GetOptimizelyCrossSellDTO");
                Result = GetOptimizelyCrossSellDTO(TemplateId, IsBeta, RenderingStrategy);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TemplateId, IsBeta, RenderingStrategy);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult CrossSellLeadSubmission(int TemplateId, string ProgramArrayString, bool IsBeta, string TrackId, string SessionId, string MatchGuid, int? ProspectId, string LeadData, string AdditionalData, long InitialLeadRawPostDataId, bool InitialMatchWasValid, string InitialLeadId, string FESessionId)
        {
            Tuple<Boolean, string> Result = new Tuple<bool,string>(false, "");
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.CrossSellLeadSubmission", null, TemplateId, ProgramArrayString, IsBeta, TrackId, SessionId, MatchGuid, ProspectId, LeadData, AdditionalData, InitialLeadRawPostDataId, InitialMatchWasValid, InitialLeadId);
                RawPostDataDTO RawData = BuildRawDataObject(LeadData);
                Log.StartLogDetail("TemplateManagerController.CrossSellLeadSubmission");
                Result = CrossSellLeadSubmissionDTO(TemplateId, ProgramArrayString, IsBeta, TrackId, SessionId, MatchGuid, ProspectId, LeadData, AdditionalData, InitialMatchWasValid, InitialLeadId, FESessionId, Log, RawData);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, TemplateId, ProgramArrayString, IsBeta, TrackId, SessionId, MatchGuid, LeadData, AdditionalData, InitialLeadRawPostDataId, InitialMatchWasValid, InitialLeadId);
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult ManagedChoiceLeadSubmission(int TemplateId, string ProgramArrayString, bool IsBeta, string TrackId, string LimboAlternativeCampaignTrackid, bool LimboAlternativeCampaignTrackidUtilized, string SessionId, string MatchId, int? ProspectId, string LeadData, string AdditionalData, int PreviousSMLeadsCreatedCount, int PreviousUSLeadsCreatedCount, FormTemplateTypes FormTemplateType, string FESessionId, Dictionary<string, string> tcpaArrayString)
        {
            ManagedChoiceSubmissionResultDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.ManagedChoiceLeadSubmission", null, ProgramArrayString, IsBeta, TrackId, LimboAlternativeCampaignTrackid, LimboAlternativeCampaignTrackidUtilized, SessionId, MatchId, ProspectId, LeadData, AdditionalData, PreviousSMLeadsCreatedCount, PreviousUSLeadsCreatedCount, FormTemplateType, FESessionId);
                RawPostDataDTO RawData = BuildRawDataObject(LeadData);
                Log.StartLogDetail("TemplateManagerController.ManagedChoiceLeadSubmission");
                Result = ManagedChoiceLeadSubmissionDTO(TemplateId, ProgramArrayString, IsBeta, TrackId, LimboAlternativeCampaignTrackid, LimboAlternativeCampaignTrackidUtilized, SessionId, MatchId, ProspectId, LeadData, AdditionalData, ref RawData, PreviousSMLeadsCreatedCount, PreviousUSLeadsCreatedCount, FESessionId, FormTemplateType, Log, tcpaArrayString);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, ProgramArrayString, IsBeta, TrackId, SessionId, MatchId, ProspectId, LeadData, AdditionalData, PreviousSMLeadsCreatedCount, PreviousUSLeadsCreatedCount, FormTemplateType, FESessionId);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetProgramTemplateModel(int ProgramProductId, bool IsBeta)
        {
            TemplateDTO Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetProgramTemplateModel", null, ProgramProductId, IsBeta);
                Log.StartLogDetail("TemplateManagerController.GetProgramTemplateModel");
                Result = FormsEngineService.GetProgramTemplateModel(ProgramProductId);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, ProgramProductId, IsBeta);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult ResubmitProspects(List<int> prospectResubmissionId)
        {
            IDictionary<int, bool> Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.ResubmitProspect", null, null);
                Log.StartLogDetail("TemplateManagerController.ResubmitProspect");
                Result = ResubmitProspects(prospectResubmissionId, Log);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex);
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetMatchedCampusDetail(string FESessionId, int CampusId)
        {
            MatchingEngine.Campus Result = null;
            try
            {
                PerformanceLog Log = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "TemplateManagerController.GetMatchedCampusDetail", null, FESessionId, CampusId);
                Log.StartLogDetail("TemplateManagerController.GetMatchedCampusDetail");
                Result = GetCampusFromMatch(FESessionId, CampusId);
                Log.EndLogDetail();
                Log.EndLog(Result);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, FESessionId, CampusId);
            }
            return new JsonpResult(Result);
        }

       

        [HttpGet]
        public ActionResult LogClientException(string Title, string URL, string DebugInfo, string Exception, string ExceptionDetail)
        {
            bool Result = true;
            try
            {
                WebISException.LogClientException(Request, Title, URL, DebugInfo, Exception, ExceptionDetail);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex, Title, Exception, ExceptionDetail);
            }
            return new JsonpResult(Result);
        }

    }
}
