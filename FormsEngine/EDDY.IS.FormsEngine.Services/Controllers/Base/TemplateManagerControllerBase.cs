using EDDY.IS.Core;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.DataModel;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.DTO.Extended;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.FormsEngine.Services.Caching;
using EDDY.IS.FormsEngine.Services.Logging;
using EDDY.IS.FormsEngine.Services.Models;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Util.HTMLExtensions;
using EDDY.IS.Util.NumberExtensions;
using EDDY.IS.Util.StringExtensions;
using EDDY.IS.Validation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EDDY.IS.Util.Linq;

namespace EDDY.IS.FormsEngine.Services.Controllers.Base
{
    public enum PROGRAMTEMPLATE_MESSAGECRITERIA
    {
        CROSSSELL_TOPMESSAGE_SUCCESS = 1,
        CROSSSELL_SUBMESSAGE_SUCCESS = 2,
        CROSSSELL_TOPMESSAGE_FAILURE = 3,
        CROSSSELL_SUBMESSAGE_FAILURE = 4,
        THANKYOU_SUCCESS = 5,
        THANKYOU_FAILURE = 6
    }

    public enum PROGRAMTEMPLATE_MESSAGETYPE
    {
        GENERIC_ONESCHOOL = 1,
        GENERIC_MULTIPLESCHOOL = 2
    }

    //[SessionState(System.Web.SessionState.SessionStateBehavior.Required)]
    public class TemplateManagerControllerBase : ControllerCommon
    {
        protected const int _emsApplicationId = 27;

        const string BASE_THEMES_FOLDER = @"~\Templates\{0}\Themes\";
        const string DYNAMIC_CONTROLS_JS = @"~\Templates\Common\js\DynamicControls.js";

        public static LeadEngine.LeadEngine LeadEngineService = new LeadEngine.LeadEngine();
        public static ValidationEngine Validation = new ValidationEngine();

        public static LeadPingService.ServiceClient LeadPingService = new LeadPingService.ServiceClient();

        /// <summary>
        /// Gets the template from cache if available if not will try to load if from database and set cache item
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="RenderingStrategy"></param>
        /// <param name="IsBeta"></param>
        /// <returns></returns>
        public TemplateCache GetTemplate(int TemplateId, string RenderingStrategy, bool IsBeta, bool? IgnoreTemplateCache, bool IsWizard, string Theme)
        {
            TemplateCache Result = null;
            Theme = string.IsNullOrWhiteSpace(Theme) ? "default" : Theme;
            string Key = string.Format(Constants.TEMPLATES_CACHE_KEY, TemplateId, RenderingStrategy, Theme, Request.Url.Scheme).Replace(' ', '_');

            if (ConfigurationManager.AppSettings.Get("CacheTemplatesEnabled").ToLower() == "true" && IgnoreTemplateCache != true)
            {
                //Get Item from Cache
                Result = FormsEngineCacheProxy.Cache.Get<TemplateCache>(Key);
                if (Result == null)
                {
                    
                    Result = new TemplateCache();
                    Result.TemplateModel = new FormEngineModel();
                    Result.TemplateModel.Initialize(TemplateId, RenderingStrategy, IsBeta, IsWizard, Theme);
                    string Javascript = GetDynamicControlsJavascript(Result.TemplateModel.Template, IsWizard); //From File System
                    if (!string.IsNullOrWhiteSpace(Javascript))
                    {
                        Result.TemplateModel.DynamicControlsJavascript = GenerateDynamicJavascript(Result.TemplateModel.Template.TemplateSteps, Javascript);
                    }
                    Result.TemplateModel.DynamicControlsJavascriptURLArguments = new JavascriptURLArguments() { TemplateId = TemplateId, RenderingStrategy = RenderingStrategy, IgnoreTemplateCache = IgnoreTemplateCache };
                    if(IsWizard)
                    {
                        Result.TemplateModel.FeaturedListSingleProgram =  FormsEngineService.GetFeaturedListSingleProgram();
                    }
                    if(RenderingStrategy.Contains("QDF"))
                    {
                        //set up datafilter collection
                        Result.TemplateModel.TemplateControlFilters = new Dictionary<int, string>();
                        foreach (var Step in Result.TemplateModel.Template.TemplateSteps.OrderBy(t => t.Sequence))
                        {
                            var StepSections = Step.TemplateSections.OrderBy(s => s.Sequence);
                            foreach (var Section in StepSections)
                            {
                                var SectionControls = Section.TemplateControls.OrderBy(c => c.RowSequence);
                                foreach (var Control in SectionControls)
                                {
                                    var StandardControlCode = Control.StandardControl.StandardControlCode.Code;
                                    List<string> Filters = FormsEngineService.GetStandardControlCodeFilters(StandardControlCode);
                                    string FilterList = "";
                                    if (Filters != null)
                                    {
                                        FilterList = string.Join(",", Filters.Select(a => a));
                                    }
                                    Result.TemplateModel.TemplateControlFilters.Add(Control.TemplateControlId, FilterList);
                                }
                            }
                        }
                    }
                    Result.RenderedTemplate = this.RazorViewToString(Result.TemplateModel.RenderingStrategy.FormTemplateView, Result.TemplateModel, true);
                    //Set Template to cache
                    FormsEngineCacheProxy.Cache.Set(Key, Result, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));

                    //Set Template Model to cache
                    string KeyModel = string.Format(Constants.TEMPLATES_MODELCACHE_KEY, Result.TemplateModel.Template.TemplateId, IsBeta).Replace(' ', '_');
                    FormsEngineCacheProxy.Cache.Set(KeyModel, Result.TemplateModel.Template, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                }
            }
            else
            {
                //Load directly from DB
                Result = new TemplateCache();
                Result.TemplateModel = new FormEngineModel();
                Result.TemplateModel.Initialize(TemplateId, RenderingStrategy, IsBeta, IsWizard, Theme);
                string Javascript = GetDynamicControlsJavascript(Result.TemplateModel.Template, IsWizard); //From File System
                if (!string.IsNullOrWhiteSpace(Javascript))
                {
                    Result.TemplateModel.DynamicControlsJavascript = GenerateDynamicJavascript(Result.TemplateModel.Template.TemplateSteps, Javascript);
                }
                Result.TemplateModel.DynamicControlsJavascriptURLArguments = new JavascriptURLArguments() { TemplateId = TemplateId, RenderingStrategy = RenderingStrategy, IgnoreTemplateCache = IgnoreTemplateCache };
                Result.RenderedTemplate = this.RazorViewToString(Result.TemplateModel.RenderingStrategy.FormTemplateView, Result.TemplateModel, true, Result.TemplateModel.Template.InlineDropDowns);
            }

            return Result;
        }

        private string GetDynamicControlsJavascript(TemplateDTO Template, bool IsWizard)
        {
            string Result = "";
            try
            {
                string physicalPath = "";
                if (IsWizard)
                {
                    physicalPath = Server.MapPath(DYNAMIC_CONTROLS_JS);
                }
                if (!string.IsNullOrWhiteSpace(physicalPath))
                {
                    Result = System.IO.File.ReadAllText(physicalPath);
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return Result;
        }


        private void GetAdditionalControls(int WizardTemplateId, List<int> SortedTemplates, string FESessionId, PerformanceLog Log, ref TemplateControlResultDTO Result)
        {
            string WizardQuestionsView = "~/Templates/Common/AdditionalQuestions.cshtml";
            Result.AdditionalControlsTemplates = new List<int>();

            Log.StartLogDetail("GetAdditionalControls.GetAdditionalTemplateQuestions");
            TemplateControlModel TemplateControlModel = new TemplateControlModel();

            TemplateControlModel.ControlList = FormsEngineService.GetAdditionalTemplateQuestions(WizardTemplateId, SortedTemplates);
            Log.EndLogDetail();

            if (TemplateControlModel.ControlList.Count > 0)
            {
                var template = FormsEngineService.GetShallowTemplate(WizardTemplateId);
                if (template != null)
                {
                    TemplateControlModel.InlineDropDownRender = template.InlineDropDowns;
                }

                Result.AdditionalControlsTemplates = TemplateControlModel.ControlList.Select(t => t.TemplateId).Distinct().ToList();
                Log.StartLogDetail("GetAdditionalControls.RazorViewToString");
                Result.RenderedControls = this.RazorViewToString(WizardQuestionsView, TemplateControlModel, true);
                Log.EndLogDetail();
            }
            Result.HasAdditionalControls = !string.IsNullOrWhiteSpace(Result.RenderedControls);
        }

        /// <summary>
        /// Gets Wizard additional controls based on regular matches
        /// To be used on START flow
        /// </summary>
        /// <param name="WizardTemplateId"></param>
        /// <param name="RenderingStrategy"></param>
        /// <param name="TrackId"></param>
        /// <param name="FormFilterValues"></param>
        /// <param name="IsBeta"></param>
        /// <param name="IgnoreTemplateCache"></param>
        /// <param name="Log"></param>
        /// <returns></returns>
        public TemplateControlResultDTO GetWizardTemplateAdditionalControls(int WizardTemplateId, bool IsBeta, string TrackId, string SessionId, string MatchId, string LeadData, string AdditionalData, int PreviousSMLeadCount, int PreviousUSLeadCount, string FESessionId, FormTemplateTypes FormTemplateType, int? ProgramTemplateId, int? ApplicationId, bool? IsWizardAnyMatch, int? ProspectId, int? InstitutionId, int? ProgramProductId, int? ProductId, PerformanceLog Log)
        {
            TemplateControlResultDTO Result = new TemplateControlResultDTO();
            TemplateMatchResponse ProgramTemplates = null;

            if (FormTemplateType == FormTemplateTypes.ProgramWizard && ProgramTemplateId.HasValue)
            {
                ProgramTemplates = new TemplateMatchResponse();
                ProgramTemplates.TemplateIdList = new List<int>() { ProgramTemplateId.Value }.ToArray();
                if(IsWizardAnyMatch == true && !(ProductId == Constants.EMS_PRODUCTID || ProductId == Constants.EMS_PPI_PRODUCTID))
                {
                    LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(WizardTemplateId, null, IsBeta, TrackId, "", false, SessionId, MatchId, LeadData, AdditionalData, null, LeadCreationType.InstitutionFormInitial, ProspectId);
                    ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
                    ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString()); 
                    CrossSellProgramResponse crossSellResponse = FormsEngineService.GetProgramsForCrossSell(Guid.Parse(TrackId), Prospect, null, IsBeta, ProgramProductId.GetValueOrDefault(), InstitutionId.GetValueOrDefault(), 10, WizardTemplateId, null, FESessionId, LeadRequest.LeadData, true, false,ApplicationId);
                    if(crossSellResponse.ProgramList != null)
                    {
                        List<int> templates = new List<int>() { ProgramTemplateId.Value };
                        templates.AddRange(crossSellResponse.ProgramList.Select(p => p.TemplateId.GetValueOrDefault()).Distinct());
                        ProgramTemplates.TemplateIdList = templates.Distinct().ToArray();
                    }
                }
            }
            else
            {
                //1. Call ME to get the matched templates based on Filters
                Log.StartLogDetail("GetWizardTemplateAdditionalControls.BuildMatchRequestDTO");
                string FormFilterValues = LeadData + "&" + AdditionalData;
                Guid session = new Guid();
                Guid.TryParse(SessionId, out session);
                var MatchRequest = BuildDirectoryMatchRequest(IsBeta, TrackId, FormFilterValues, true, null, null, ApplicationId, session);
                Log.EndLogDetail();
                Log.StartLogDetail("GetWizardTemplateAdditionalControls.GetTemplatesForMatches(Matching Engine)");
                MatchRequest.LeadCreationType = MatchingEngine.LeadCreationType.WizardUserSelection; //EV call.
                ProgramTemplates = FormsEngineService.RelatedServices.GetTemplatesForMatches(MatchRequest, IsBeta);
                Log.EndLogDetail();
                Result.MatchResponseGuid = ProgramTemplates.MatchResponseGuid;
            }


            if (ProgramTemplates.TemplateIdList != null && ProgramTemplates.TemplateIdList.Count() > 0)
            {
                List<int> SortedTemplates = ProgramTemplates.TemplateIdList.OrderBy(t => t).ToList<int>();
                string WizardTemplateAdditionalControlsKey = string.Format(Constants.TEMPLATE_ADDITIONALCONTROLS_HTML_KEY, WizardTemplateId, string.Join("-", SortedTemplates));

                if (ConfigurationManager.AppSettings.Get("CacheTemplatesEnabled").ToLower() == "true")
                {
                    //Get Item from Cache
                    Result = FormsEngineCacheProxy.Cache.Get<TemplateControlResultDTO>(WizardTemplateAdditionalControlsKey);
                    if (Result == null)
                    {
                        Result = new TemplateControlResultDTO();
                        Log.StartLogDetail("GetWizardTemplateAdditionalControls.GetAdditionalControls");
                        GetAdditionalControls(WizardTemplateId, SortedTemplates, FESessionId, Log, ref Result);
                        Log.EndLogDetail();

                        //Set Item to Cache
                        FormsEngineCacheProxy.Cache.Set(WizardTemplateAdditionalControlsKey, Result, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                    }
                }
                else
                {
                    //Load directly from DB
                    Log.StartLogDetail("GetWizardTemplateAdditionalControls.GetAdditionalControls");
                    GetAdditionalControls(WizardTemplateId, SortedTemplates, FESessionId, Log, ref Result);
                    Log.EndLogDetail();
                }

                //Add to session the templates asked plus the program templates that the wizard covers
                Log.StartLogDetail("GetAdditionalControls.GetProgramTemplatesCoveredByWizardQuestions");
                List<int> TemplateList = FormsEngineService.GetProgramTemplatesCoveredByWizardQuestions(WizardTemplateId);
                SortedTemplates.AddRange(TemplateList);
                SortedTemplates = SortedTemplates.Distinct().ToList();
                Log.EndLogDetail();
                FESession.Set(FESessionId, Constants.WIZARD_SESSION_PROGRAMTEMPLATESCOVERED, SortedTemplates);
            }

            else // No program templates from match result
            {
                Result.HasAdditionalControls = false;
                Result.AdditionalControlsTemplates = new List<int>();
            }

            return Result;
        }


        /// <summary>
        /// Gets Wizard additional controls based on smart matches
        /// To be used on START flow
        /// </summary>
        /// <param name="WizardTemplateId"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackId"></param>
        /// <param name="SessionId"></param>
        /// <param name="MatchId"></param>
        /// <param name="LeadData"></param>
        /// <param name="AdditionalData"></param>
        /// <param name="PreviousSMLeadCount"></param>
        /// <param name="PreviousUSLeadCount"></param>
        /// <param name="FESessionId"></param>
        /// <param name="IgnoreTemplateCache"></param>
        /// <param name="Log"></param>
        /// <returns></returns>
        public TemplateControlResultDTO GetWizardTemplateAdditionalControlsFromSM(int WizardTemplateId, bool IsBeta, string TrackId, string SessionId, string MatchId, string LeadData, string AdditionalData, int PreviousSMLeadCount, int PreviousUSLeadCount, string DeviceId, string FESessionId, int? ApplicationId, PerformanceLog Log)
        {
            TemplateControlResultDTO Result = new TemplateControlResultDTO();


            //1. Call ME to get the smart matched templates based on Filters
            Log.StartLogDetail("GetWizardTemplateAdditionalControls.GetSmartMatchesForWizard");
            WizardMatchResponse SMPrograms = GetSmartMatchesForWizard(IsBeta, WizardTemplateId, TrackId, SessionId, MatchId, LeadData, AdditionalData, PreviousSMLeadCount, PreviousUSLeadCount, FESessionId, false, DeviceId, false, ApplicationId, false, Log);
            Log.EndLogDetail();

            Result.MatchResponseGuid = SMPrograms.MatchResponseGuid;


            if (SMPrograms.SmartMatchList != null && SMPrograms.SmartMatchList.Count() > 0)
            {
                List<int> SortedTemplates = (from t in SMPrograms.SmartMatchList
                                             where t.TemplateId.HasValue
                                             orderby t.TemplateId
                                             select (int)t.TemplateId).Distinct().ToList();

                string WizardTemplateAdditionalControlsKey = string.Format(Constants.TEMPLATE_ADDITIONALCONTROLS_HTML_KEY, WizardTemplateId, string.Join("-", SortedTemplates));

                if (ConfigurationManager.AppSettings.Get("CacheTemplatesEnabled").ToLower() == "true")
                {
                    //Get Item from Cache
                    Result = FormsEngineCacheProxy.Cache.Get<TemplateControlResultDTO>(WizardTemplateAdditionalControlsKey);
                    if (Result == null)
                    {
                        Result = new TemplateControlResultDTO();
                        Log.StartLogDetail("GetWizardTemplateAdditionalControls.GetAdditionalControls");
                        GetAdditionalControls(WizardTemplateId, SortedTemplates, FESessionId, Log, ref Result);
                        Log.EndLogDetail();

                        //Set Item to Cache
                        FormsEngineCacheProxy.Cache.Set(WizardTemplateAdditionalControlsKey, Result, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                    }
                }
                else
                {
                    //Load directly from DB
                    Log.StartLogDetail("GetWizardTemplateAdditionalControls.GetAdditionalControls");
                    GetAdditionalControls(WizardTemplateId, SortedTemplates, FESessionId, Log, ref Result);
                    Log.EndLogDetail();
                }

                //Add to session the templates asked plus the program templates that the wizard covers
                Log.StartLogDetail("GetAdditionalControls.GetProgramTemplatesCoveredByWizardQuestions");
                List<int> TemplateList = FormsEngineService.GetProgramTemplatesCoveredByWizardQuestions(WizardTemplateId);
                SortedTemplates.AddRange(TemplateList);
                SortedTemplates = SortedTemplates.Distinct().ToList();
                Log.EndLogDetail();
                FESession.Set(FESessionId, Constants.WIZARD_SESSION_PROGRAMTEMPLATESCOVERED, SortedTemplates);
            }
            else // No program templates from match result
            {
                Result.HasAdditionalControls = false;
                Result.AdditionalControlsTemplates = new List<int>();
            }

            return Result;
        }

        public TemplateControlResultDTO GetSchoolPickerWizardTemplateAdditionalControlCollection(int WizardTemplateId, List<int> ProgramTemplateIds, string SessionId, ref PerformanceLog Log)
        {
            var templateControlResultDTO = new TemplateControlResultDTO();

            if (ProgramTemplateIds != null && ProgramTemplateIds.Count() > 0)
            {
                List<int> SortedTemplates = ProgramTemplateIds.OrderBy(t => t).ToList<int>();
                Log.StartLogDetail("GetSchoolPickerWizardTemplateAdditionalControlCollection.RazorViewToString");
                this.GetAdditionalControls(WizardTemplateId, SortedTemplates, SessionId, Log, ref templateControlResultDTO);
                Log.EndLogDetail();
            }

            return templateControlResultDTO;
        }

        public List<TemplateAdditionalQuestionDTO> GetWizardTemplateAdditionalControlCollection(int WizardTemplateId, List<int> ProgramTemplateIds, ref PerformanceLog Log)
        {
            List<TemplateAdditionalQuestionDTO> Result = new List<TemplateAdditionalQuestionDTO>();
            string WizardQuestionsView = "~/Templates/Common/AdditionalQuestions.cshtml";


            if (ProgramTemplateIds != null && ProgramTemplateIds.Count() > 0)
            {
                List<int> SortedTemplates = ProgramTemplateIds.OrderBy(t => t).ToList<int>();
                List<TemplateControlDTO> TemplateControlModel = new List<TemplateControlDTO>();
                Log.StartLogDetail("GetWizardTemplateAdditionalControlCollection.GetAdditionalTemplateQuestions");
                TemplateControlModel = FormsEngineService.GetAdditionalTemplateQuestions(WizardTemplateId, SortedTemplates, true);
                Log.EndLogDetail();

                var template = FormsEngineService.GetShallowTemplate(WizardTemplateId);

                //for each template ID lets create the html in a string and set that in our final list
                foreach (int i in ProgramTemplateIds)
                {
                    TemplateAdditionalQuestionDTO taqDTO = new TemplateAdditionalQuestionDTO();
                    taqDTO.TemplateId = i;
                    Log.StartLogDetail("GetWizardTemplateAdditionalControlCollection.RazorViewToString");
                    TemplateControlModel templateControlModel = new Models.TemplateControlModel();
                    templateControlModel.ControlList = TemplateControlModel.Where(tcm => tcm.TemplateId == i).ToList();
                    if (template != null)
                    {
                        templateControlModel.InlineDropDownRender = template.InlineDropDowns;
                    }

                    //if there are additional questions for this template id render them.
                    if (templateControlModel.ControlList.Count > 0)
                    {
                        taqDTO.RenderedControl = this.RazorViewToString(WizardQuestionsView, templateControlModel, true);
                    }
                    else
                    {
                        taqDTO.RenderedControl = string.Empty;
                    }

                    Result.Add(taqDTO);
                    Log.EndLogDetail();
                }
            }

            return Result;
        }


        public string GetDefaultRenderingStrategy(bool Wizard)
        {
            string DefaultRendering = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings.Get("ProgramTemplateDefaultRenderingStrategy")) ? ConfigurationManager.AppSettings.Get("ProgramTemplateDefaultRenderingStrategy") : "ORIGINAL";
            string URL = Request.UrlReferrer == null ? Request.Url.ToString() : Request.UrlReferrer.ToString();
            new ISException(EDDY.IS.Base.ISApplication.FormsEngine, new Exception(string.Format("NULL or Empty Rendering Strategy requested from URL {0}. Default will be used {1} ", URL, DefaultRendering))).Save();
            return DefaultRendering;
        }

        public TemplateResultDTO GetProgramTemplateDTO(int TemplateId, int? ProgramProductId, int? ProgramId, int InstitutionId, string RenderingStrategy, bool IsBeta, string AlternativeTemplates, bool? IgnoreTemplateCache, string Theme, int? ApplicationId, string TrackId, string DeviceId, int? ProductId, int? PaidStatusTypeId, PerformanceLog Log)
        {
            TemplateResultDTO Result = new TemplateResultDTO();

            //Application Override logic
            var Templates = FormsEngineService.GetTemplateApplicationOverrideForApplication(ApplicationId.GetValueOrDefault());
            if (Templates.Count > 0)
            {
                //set the default
                TemplateId = Templates.Where(t => t.PaidStatusTypeId == null).FirstOrDefault().TemplateId > 0 ? Templates.Where(t => t.PaidStatusTypeId == null).FirstOrDefault().TemplateId : TemplateId;

                if (ProgramId.HasValue)
                {
                    //Call ME to get the product
                    FormProgramResponse ProgramList = GetProgramTemplatePrograms(InstitutionId, ProgramId, IsBeta, TrackId, "", ApplicationId, DeviceId, AlternativeTemplates, false);
                    var Program = (from p in ProgramList.FormProgramList
                                   where p.ProgramId == (int)ProgramId
                                   select p).FirstOrDefault();

                    if (Program != null)
                    {
                        int paidstattype = PaidStatusTypeId == null ? (int)Program.PaidStatusTypeId : PaidStatusTypeId.GetValueOrDefault();
                        if (Program.ProductId == 17 || paidstattype != (int)PaidStatusType.Paid) //only do this swap for grad schools product OR free/fraid
                        {
                            //int product = Program.ProductId == null ? (int)Program.PaidStatusTypeId : ProductId.GetValueOrDefault();
                            int templateid = Templates.Where(t => t.ApplicationId == ApplicationId.GetValueOrDefault() && t.PaidStatusTypeId == paidstattype).FirstOrDefault().TemplateId;
                            if (templateid > 0)
                            {
                                TemplateId = templateid;
                            }
                            else if (templateid == 0 && Program.TemplateId.HasValue)
                            {
                                TemplateId = Program.TemplateId.Value;
                            }
                        }
                    }
                }
            }

            TemplateId = FormsEngineService.GetTemplateIdOrDefault(TemplateId, false, ProgramId, ProgramProductId);

            //Alternative templates support
            if (!string.IsNullOrWhiteSpace(AlternativeTemplates))
            {
                Log.StartLogDetail("GetProgramTemplateDTO.AlternativeTemplatesSupport");
                int AlternativeTemplateId = FormsEngineService.FindAlternativeTemplateId(TemplateId, AlternativeTemplates);
                //Alternative found and was different than requested
                if (AlternativeTemplateId != TemplateId)
                {
                    TemplateId = AlternativeTemplateId;
                }
                Log.EndLogDetail();
            }

            Log.StartLogDetail("GetProgramTemplateDTO.GetTemplate");
            TemplateCache _TemplateCache = GetTemplate(TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, false, Theme);
            Log.EndLogDetail();

            if (_TemplateCache != null)
            {
                Result.Template = _TemplateCache.RenderedTemplate;
                Result.CSS = _TemplateCache.TemplateModel.RenderingStrategy.CSSPath;
                Result.TemplateId = _TemplateCache.TemplateModel.Template.TemplateId;
                Result.DefaultTemplateId = _TemplateCache.TemplateModel.DefaultTemplateId;
            }

            return Result;
        }

        public TemplateResultDTO GetWizardTemplateDTO(int TemplateId, string RenderingStrategy, bool IsBeta, bool? IgnoreTemplateCache, string Theme, string FESessionId, PerformanceLog Log, bool UseInternationalTemplate = false, int InternationalTemplateId = -1, string countryCode = "")
        {
            TemplateResultDTO Result = new TemplateResultDTO();

            TemplateId = FormsEngineService.GetTemplateIdOrDefault(TemplateId, true, null, null);

            // if using international template- back up the old templateId and retrieve the international templateId..
            if (UseInternationalTemplate && InternationalTemplateId > 0)
            {
                Result.UseInternationalTemplate = UseInternationalTemplate;
                Result.OriginalTemplateId = TemplateId;
                TemplateId = InternationalTemplateId;

                if (!string.IsNullOrEmpty(countryCode)) {
                    Result.InternationalCountryCode = countryCode;
                }
            }

            // Clear previous Smart Match & User Select Results and Pixels so this will be a new experience for the user
            FESession.Remove(FESessionId, Constants.WIZARD_ME_THIRDPARTYSMARTMATCHRESULTS_KEY);
            FESession.Remove(FESessionId, Constants.WIZARD_ME_SMARTMATCHRESULTS_KEY);
            FESession.Remove(FESessionId, Constants.WIZARD_ME_USERSELECTRESULTS_KEY);
            FESession.Remove(FESessionId, Constants.WIZARD_SMARTMATCHPIXELS_KEY);
            FESession.Remove(FESessionId, Constants.WIZARD_USERSELECTPIXELS_KEY);
            FESession.Remove(FESessionId, Constants.WIZARD_USERSELECTLEADS_KEY);
            FESession.Remove(FESessionId, Constants.WIZARD_SESSION_PROGRAMTEMPLATESCOVERED);
            FESession.Remove(FESessionId, Constants.PROGRAMWIZARD_INITIALCAMPUS_KEY);
            FESession.Remove(FESessionId, Constants.ME_SKIPSCHOOLSELECTIONFORMATCHONE);

            Log.StartLogDetail("GetWizardTemplateDTO.GetTemplate");
            TemplateCache _TemplateCache = GetTemplate(TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, true, Theme);
            Log.EndLogDetail();

            if (_TemplateCache != null)
            {
                Result.TemplateId = _TemplateCache.TemplateModel.Template.TemplateId;
                Result.Template = _TemplateCache.RenderedTemplate;
                Result.CSS = _TemplateCache.TemplateModel.RenderingStrategy.CSSPath;
                Result.DefaultTemplateId = _TemplateCache.TemplateModel.DefaultTemplateId;
                Result.FormTemplateType = (FormTemplateTypes)Enum.ToObject(typeof(FormTemplateTypes), _TemplateCache.TemplateModel.Template.TemplateType.TemplateTypeId);
                Result.ShowAllQuestionsOnFirstStep = _TemplateCache.TemplateModel.Template.ShowAllQuestionsOnFirstStep;
            }

            return Result;
        }
        
        public TemplateResultDTO GetQDFTemplateDTO(int TemplateId, string RenderingStrategy, bool IsBeta, bool? IgnoreTemplateCache, string Theme, string FESessionId, PerformanceLog Log, bool UseInternationalTemplate = false, int InternationalTemplateId = -1, string countryCode = "")
        {
            TemplateResultDTO Result = new TemplateResultDTO();
            
            Log.StartLogDetail("GetQDFTemplateDTO.GetTemplate");
            TemplateCache _TemplateCache = GetTemplate(TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, true, Theme);
            Log.EndLogDetail();

            if (_TemplateCache != null)
            {
                Result.TemplateId = _TemplateCache.TemplateModel.Template.TemplateId;
                Result.Template = _TemplateCache.RenderedTemplate;
                Result.CSS = _TemplateCache.TemplateModel.RenderingStrategy.CSSPath;
                Result.DefaultTemplateId = _TemplateCache.TemplateModel.DefaultTemplateId;
                Result.FormTemplateType = (FormTemplateTypes)Enum.ToObject(typeof(FormTemplateTypes), _TemplateCache.TemplateModel.Template.TemplateType.TemplateTypeId);
                Result.ShowAllQuestionsOnFirstStep = _TemplateCache.TemplateModel.Template.ShowAllQuestionsOnFirstStep;
                
            }

            return Result;
        }

        public string GetWizardTemplateControlsJS(int TemplateId, string RenderingStrategy, bool IsBeta, bool? IgnoreTemplateCache, ref PerformanceLog Log)
        {
            string Result = "";

            TemplateId = FormsEngineService.GetTemplateIdOrDefault(TemplateId, true, null, null);
            Log.StartLogDetail("GetWizardTemplateDTO.GetWizardTemplateControlsJS");
            TemplateCache _TemplateCache = GetTemplate(TemplateId, RenderingStrategy, IsBeta, IgnoreTemplateCache, true, "");
            Log.EndLogDetail();

            if (_TemplateCache != null)
            {
                Result = _TemplateCache.TemplateModel.DynamicControlsJavascript;
            }
            return Result;
        }


        /// <summary>
        /// Dynamic Javascript for Control Filters
        /// </summary>
        /// <param name="Steps"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        private string GenerateDynamicJavascript(ICollection<TemplateStepDTO> Steps, string Result)
        {
            StringBuilder Script = new StringBuilder();
            const string QStart = "\t //Sorted List of Questions  \n" +
                                  "\t FormsEngine.Questions = []; \n" +
                                  "\t var Question = {}; \n\n";

            const string Question = "\t Question = {{}}; \n " +
                                    "\t Question.Code = '{0}'; \n " +
                                    "\t Question.TemplateControlId = {1}; \n " +
                                    "\t Question.Step = {2}; \n " +
                                    "\t Question.Filters = [{3}]; \n " +
                                    "\t Question.DataBind = {4}; \n " +
                                    "\t Question.LastDataBindFilters = 'none'; \n " +
                                    "\t Question.LastQuestionFromStep = {5}; \n " +
                                    "\t Question.Initialized = false; \n " +
                                    "\t FormsEngine.Questions.push(Question); \n\n ";

            Script.AppendLine(string.Format("//{0}: START Dynamic Controls Code", DateTime.Now.ToString()));
            Script.AppendLine(QStart);

            int StepNumber = 0;
            foreach (var Step in Steps.OrderBy(t => t.Sequence))
            {
                StepNumber++;
                var StepSections = Step.TemplateSections.OrderBy(s => s.Sequence);
                var LastSectionSequence = StepSections.Max(t => t.Sequence);
                foreach (var Section in StepSections)
                {
                    var SectionControls = Section.TemplateControls.OrderBy(c => c.RowSequence);
                    int LastControlRow = Section.Sequence == LastSectionSequence ? SectionControls.Max(t => t.RowSequence) : -1;
                    foreach (var Control in SectionControls)
                    {
                        var StandardControlCode = Control.StandardControl.StandardControlCode.Code;
                        List<string> Filters = FormsEngineService.GetStandardControlCodeFilters(StandardControlCode);
                        string FilterList = "";
                        if (Filters != null)
                        {
                            FilterList = string.Join(",", Filters.Select(a => "'" + a + "'"));
                        }

                        Script.AppendLine(string.Format(Question
                            , StandardControlCode
                            , Control.TemplateControlId
                            , StepNumber
                            , FilterList
                            , string.IsNullOrWhiteSpace(Control.StandardControl.JavaScriptDataBind) ? "undefined" : Control.StandardControl.JavaScriptDataBind
                            , Control.RowSequence == LastControlRow ? "true" : "false"
                            ));
                    }
                }
            }
            Script.AppendLine(string.Format("//{0}: END of Dynamic Controls Code", DateTime.Now.ToString()));
            return Result.Replace("//@@DYNAMIC_FILTERS@@", Script.ToString());
        }


        public NoMatchResultDTO GetNoMatchResultDTO(string FESessionId, string Theme, string RenderingStrategy, bool IsBeta, bool GenericNoMatch, ref PerformanceLog Log)
        {
            NoMatchResultDTO Result = new NoMatchResultDTO();
            var Rendering = FormsEngineService.GetHTMLRenderingStrategy(RenderingStrategy);
            NoMatchModel Model = new NoMatchModel(Theme);

            var UserSession = GetWorkflowSession(FESessionId);
            //Recover FullName from session.
            Model.UserFullName = UserSession != null ? UserSession.UserFullName : "";

            //Generic No Match: Any No match scenario based on user input and not on matching engine results
            Model.GenericNoMatch = GenericNoMatch;

            Log.StartLogDetail("GetNoMatchResultDTO.RazorViewToString");
            Result.RenderedNoMatch = this.RazorViewToString(Rendering.NoMatchTemplateView, Model, true);
            Log.EndLogDetail();

            return Result;
        }

        public ThankYouResultDTO GetThankYouResultDTO(string FESessionId, int? TemplateId, string Theme, string RenderingStrategy, bool IsBeta, ref PerformanceLog Log)
        {
            ThankYouResultDTO Result = new ThankYouResultDTO();
            var Rendering = FormsEngineService.GetHTMLRenderingStrategy(RenderingStrategy);
            ThankYouModel Model = new ThankYouModel(Theme);

            // Session Objects     
            var ProgramWithInstitutionCampusList = FESession.Get<List<ProgramWithInstitutionCampus>>(FESessionId, Constants.WIZARD_ME_SMARTMATCHRESULTS_KEY);
            var CampusWithInstitutionList = FESession.Get<List<CampusWithInstitution>>(FESessionId, Constants.WIZARD_ME_USERSELECTRESULTS_KEY);
            var SmartMatches = FESession.Get<List<LeadSaveData>>(FESessionId, Constants.WIZARD_SMARTMATCHLEADS_KEY);
            var UserSelections = FESession.Get<List<LeadSaveData>>(FESessionId, Constants.WIZARD_USERSELECTLEADS_KEY);
            ProgramWithInstitutionCampus InitialProgramWizard = FESession.Get<ProgramWithInstitutionCampus>(FESessionId, Constants.PROGRAMWIZARD_INITIALCAMPUS_KEY);
            var UserSession = GetWorkflowSession(FESessionId);

            //Recover FullName from session.
            Model.UserFullName = UserSession != null ? UserSession.UserFullName : "";
            Model.UserSelectionProgramIdList = new List<int>();
            Model.SmartMatchProgramIdList = new List<int>();

            /*
                You will need to find the intersecting Institution/Campus /Programs between 
                ME_SmartMatchResults <-X-> SmartMatchLeads
                ME_UserSelectResults <-X-> UserSelectLeads
            */
            //if SM exist --> PIC has to exist
            //IF US exist --> CIC has to exist
            // US and/or SM has to exist
            if ((SmartMatches == null && ProgramWithInstitutionCampusList == null)
                && (UserSelections == null && CampusWithInstitutionList == null)
                && (InitialProgramWizard == null)
               )
            {
                Result.MoveToNoMatch = true;
                return Result;
            }

            ProgramWithInstitutionCampusList = ProgramWithInstitutionCampusList == null ? new List<ProgramWithInstitutionCampus>() : ProgramWithInstitutionCampusList;
            CampusWithInstitutionList = CampusWithInstitutionList == null ? new List<CampusWithInstitution>() : CampusWithInstitutionList;

            List<CampusBasicModel> Online = new List<CampusBasicModel>();
            List<CampusBasicModel> Ground = new List<CampusBasicModel>();
            List<string> LeadIdList = new List<string>();
            EddyLogosDTO EddyLogos = new EddyLogosDTO(); 
            bool hadFailedSelections = false;

            #region User selections based
            if (UserSelections != null)
            {
                List<Guid?> usedExternalMatchGuidList = new List<Guid?>();

                foreach (var us in UserSelections)
                {
                    if (us.IsValid == false)
                    {
                        hadFailedSelections = true;
                        continue;
                    }
                    LeadIdList.Add(us.LeadId == null ? "" : us.LeadId.ToString());
                    
                    var programCampusInstitution = (from c in CampusWithInstitutionList
                                                    from p in c.ProgramList
                                                    where p.ProgramProductId == us.ProgramProductId
                                                        && (!p.ExternalMatchItemGuid.HasValue || !usedExternalMatchGuidList.Contains(p.ExternalMatchItemGuid.Value))
                                                    select new
                                                    {
                                                        CampusId = c.CampusId,
                                                        CampusType = p.ProgramCampusType,
                                                        CampusLogoURL = c.CampusLogoURL,
                                                        InstitutionName = c.InstitutionName,
                                                        InstitutionLogoURL = c.InstitutionLogoURL,
                                                        ProgramName = p.ProgramName,
                                                        ProgramDescription = p.ProgramDescription,
                                                        InstitutionId = c.InstitutionId,
                                                        Description = c.InstitutionDescription,
                                                        ProgramId = p.ProgramId,
                                                        ProductId = p.ProductId,
                                                        ExternalMatchItemGuid = p.ExternalMatchItemGuid
                                                    }).FirstOrDefault(); 
                    
                    if(programCampusInstitution == null && us.OriginalProgramProductId.HasValue)
                    {
                        programCampusInstitution = (from c in CampusWithInstitutionList
                                                    from p in c.ProgramList
                                                    where p.ProgramProductId == us.OriginalProgramProductId
                                                        && (!p.ExternalMatchItemGuid.HasValue || !usedExternalMatchGuidList.Contains(p.ExternalMatchItemGuid.Value))
                                                    select new
                                                    {
                                                        CampusId = c.CampusId,
                                                        CampusType = p.ProgramCampusType,
                                                        CampusLogoURL = c.CampusLogoURL,
                                                        InstitutionName = c.InstitutionName,
                                                        InstitutionLogoURL = c.InstitutionLogoURL,
                                                        ProgramName = p.ProgramName,
                                                        ProgramDescription = p.ProgramDescription,
                                                        InstitutionId = c.InstitutionId,
                                                        Description = c.InstitutionDescription,
                                                        ProgramId = p.ProgramId,
                                                        ProductId = p.ProductId,
                                                        ExternalMatchItemGuid = p.ExternalMatchItemGuid
                                                    }).FirstOrDefault();
                    }

                    if (programCampusInstitution != null)
                    {
                        if (programCampusInstitution.ExternalMatchItemGuid.HasValue)
                            usedExternalMatchGuidList.Add(programCampusInstitution.ExternalMatchItemGuid.Value);

                        CampusBasicModel CampusModel = new CampusBasicModel();
                        CampusModel.CampusName = programCampusInstitution.InstitutionName;
                        CampusModel.ProgramName = programCampusInstitution.ProgramName;
                        CampusModel.CampusDescription = programCampusInstitution.Description;
                        CampusModel.ProgramDescription = programCampusInstitution.ProgramDescription == null ? "" : Regex.Replace(programCampusInstitution.ProgramDescription, @"<(.|\n)*?>", string.Empty);


                        // logo
                        CampusModel.Logo = !string.IsNullOrEmpty(programCampusInstitution.CampusLogoURL) ?
                            EddyLogos.EddyLogoImagePathDomain + programCampusInstitution.CampusLogoURL.Replace("{FILENAME}", string.Format(EddyLogos.EddyLogoImageFileName, EddyLogos.EddyLogoImageSizeSmall)) :
                            EddyLogos.EddyLogoImagePathDomain + programCampusInstitution.InstitutionLogoURL.Replace("{FILENAME}", string.Format(EddyLogos.EddyLogoImageFileName, EddyLogos.EddyLogoImageSizeSmall));

                        //No logo for free / fraid products
                        CampusModel.ShowLogo = programCampusInstitution.ProductId != (int)GradSchoolProduct.FREE;

                        if (programCampusInstitution.CampusType == CampusType.Online)
                        {
                            Online.Add(CampusModel);
                        }
                        else
                        {
                            Ground.Add(CampusModel);
                        }

                        Model.UserSelectionProgramIdList.Add(programCampusInstitution.ProgramId);
                    }
                }
            }
            #endregion User selections based

            #region SmartMatch based
            if (ProgramWithInstitutionCampusList != null)
            {
                foreach (var sm in ProgramWithInstitutionCampusList)
                {
                    if (sm != null)
                    {
                        CampusBasicModel CampusModel = new CampusBasicModel();
                        CampusModel.CampusName = sm.InstitutionName;
                        CampusModel.ProgramName = sm.ProgramName;
                        CampusModel.CampusDescription = sm.InstitutionDescription;
                        CampusModel.ProgramDescription = sm.ProgramDescription == null ? "" : Regex.Replace(sm.ProgramDescription, @"<(.|\n)*?>", string.Empty);

                        // logo
                     

                        //CampusModel.Logo = !string.IsNullOrEmpty(sm.CampusLogoURL) ?
                        //   EddyLogos.EddyLogoImagePathDomain + sm.CampusLogoURL.Replace("{FILENAME}", string.Format(EddyLogos.EddyLogoImageFileName, EddyLogos.EddyLogoImageSizeSmall)) :
                        //   EddyLogos.EddyLogoImagePathDomain + sm.InstitutionLogoURL.Replace("{FILENAME}", string.Format(EddyLogos.EddyLogoImageFileName, EddyLogos.EddyLogoImageSizeSmall));

                        if (!string.IsNullOrEmpty(sm.CampusLogoURL))
                        {
                            if (sm.CampusLogoURL.Contains(EddyLogos.EddyLogoImagePathDomain))
                            {
                                CampusModel.Logo = sm.CampusLogoURL;
                            }
                            else
                            {
                                CampusModel.Logo = EddyLogos.EddyLogoImagePathDomain + sm.CampusLogoURL.Replace("{FILENAME}", string.Format(EddyLogos.EddyLogoImageFileName, EddyLogos.EddyLogoImageSizeSmall));
                            }
                        }
                        else if (!string.IsNullOrEmpty(sm.InstitutionLogoURL))
                        {
                            if (sm.InstitutionLogoURL.Contains(EddyLogos.EddyLogoImagePathDomain))
                            {
                                CampusModel.Logo = sm.InstitutionLogoURL;
                            }
                            else
                            {
                                CampusModel.Logo = EddyLogos.EddyLogoImagePathDomain + sm.InstitutionLogoURL.Replace("{FILENAME}", string.Format(EddyLogos.EddyLogoImageFileName, EddyLogos.EddyLogoImageSizeSmall));
                            }
                        }

                        //No logo for free / fraid products
                        CampusModel.ShowLogo = sm.ProductId != (int)GradSchoolProduct.FREE;

                        if (sm.CampusType == CampusType.Online)
                        {
                            Online.Add(CampusModel);
                        }
                        else
                        {
                            Ground.Add(CampusModel);
                        }

                        Model.SmartMatchProgramIdList.Add(sm.ProgramId);
                    }
                }
            }
            #endregion SmartMatch based

            #region Initial Program Wizard based
            if (InitialProgramWizard != null)
            {
                CampusBasicModel CampusModel = new CampusBasicModel();
                CampusModel.CampusName = InitialProgramWizard.InstitutionName;
                CampusModel.ProgramName = InitialProgramWizard.ProgramName;
                CampusModel.CampusDescription = InitialProgramWizard.InstitutionDescription;
                CampusModel.ProgramDescription = InitialProgramWizard.ProgramDescription == null ? "" : Regex.Replace(InitialProgramWizard.ProgramDescription, @"<(.|\n)*?>", string.Empty);

                // logo
                CampusModel.Logo = !string.IsNullOrEmpty(InitialProgramWizard.CampusLogoURL) ?
                           EddyLogos.EddyLogoImagePathDomain + InitialProgramWizard.CampusLogoURL.Replace("{FILENAME}", string.Format(EddyLogos.EddyLogoImageFileName,  EddyLogos.EddyLogoImageSizeSmall)) :
                           EddyLogos.EddyLogoImagePathDomain + InitialProgramWizard.InstitutionLogoURL.Replace("{FILENAME}", string.Format(EddyLogos.EddyLogoImageFileName, EddyLogos.EddyLogoImageSizeSmall));


                //No logo for free / fraid products
                CampusModel.ShowLogo = InitialProgramWizard.ProductId != (int)GradSchoolProduct.FREE;

                if (InitialProgramWizard.CampusType == CampusType.Online)
                {
                    Online.Add(CampusModel);
                }
                else
                {
                    Ground.Add(CampusModel);
                }
            }

            #endregion Initial Program Wizard based

            //Order alphabetically
            Model.GroundCampuses = Ground.OrderBy(g => g.CampusName).ToList();
            Model.OnlineCampuses = Online.OrderBy(o => o.CampusName).ToList();
            Model.SubmissionsFailed = hadFailedSelections;

            //Copy LeadId's to page
            Model.LeadList = string.Join(",", LeadIdList);

            Log.StartLogDetail("GetThankYouResultDTO.RazorViewToString");
            Result.RenderedThankYou = this.RazorViewToString(Rendering.ThankYouTemplateView, Model, true);
            Log.EndLogDetail();

            return Result;
        }

        public Campus GetCampusFromMatch(string FESessionId, int CampusId)
        {
            Campus Result = new Campus();

            // Session Objects     
            var ProgramWithInstitutionCampus = FESession.Get<List<ProgramWithInstitutionCampus>>(FESessionId, Constants.WIZARD_ME_SMARTMATCHRESULTS_KEY);
            var CampusWithInstitution = FESession.Get<List<CampusWithInstitution>>(FESessionId, Constants.WIZARD_ME_USERSELECTRESULTS_KEY);

            ProgramWithInstitutionCampus = ProgramWithInstitutionCampus == null ? new List<ProgramWithInstitutionCampus>() : ProgramWithInstitutionCampus;
            CampusWithInstitution = CampusWithInstitution == null ? new List<CampusWithInstitution>() : CampusWithInstitution;

            var Campus = (from c in CampusWithInstitution
                          where c.CampusId == CampusId
                          select c).FirstOrDefault();


            if (Campus != null)
            {
                Result = new Campus();
                Result.Address1 = Campus.Address1;
                Result.Address2 = Campus.Address2;
                Result.CampusId = Campus.CampusId;
                Result.CampusName = Campus.CampusName;
                Result.CampusType = Campus.CampusType;
                Result.City = Campus.City;
                Result.CountryCode = Campus.CountryCode;
                Result.CampusLogoURL = Campus.CampusLogoURL;
                //Result.InstitutionLogoURL = Campus.InstitutionLogoURL;
                Result.PostalCode = Campus.PostalCode;
                Result.State = Campus.State;
                //Result.ProgramList //Not needed
                Result.ProgramList = null; //Not needed
            }
            else
            {
                var CampusProgram = (from c in ProgramWithInstitutionCampus
                                     where c.CampusId == CampusId
                                     select c).FirstOrDefault();

                if (CampusProgram != null)
                {
                    Result.Address1 = CampusProgram.CampusAddress1;
                    Result.Address2 = CampusProgram.CampusAddress2;
                    Result.CampusId = CampusProgram.CampusId;
                    Result.CampusName = CampusProgram.CampusName;
                    Result.CampusType = CampusProgram.CampusType;
                    Result.City = CampusProgram.CampusCity;
                    //Result.CountryCode =  //Not available
                    Result.CampusLogoURL = CampusProgram.CampusLogoURL;
                    //Result.InstitutionLogoURL = CampusProgram.InstitutionLogoURL;
                    Result.PostalCode = CampusProgram.CampusPostalCode;
                    Result.State = CampusProgram.CampusState;
                    //Result.ProgramList //Not needed

                }
            }
            return Result;
        }


        /// <summary>
        /// Process Wizard Submission:
        ///     ProgramWizards --> Saves initial lead using program template logic then process the rest of the request as a wizard submission with no SM
        ///     WizardTemplates --> Saves SM and process the rest of the request
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="RenderingStrategy"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackId"></param>
        /// <param name="SessionId"></param>
        /// <param name="MatchGuid"></param>
        /// <param name="ProspectId"></param>
        /// <param name="LeadData"></param>
        /// <param name="AdditionalData"></param>
        /// <param name="SMLeadsCreatedCount"></param>
        /// <param name="USLeadsCreatedCount"></param>
        /// <param name="SplitCampusTypeInResults"></param>
        /// <param name="FESessionId"></param>
        /// <param name="DeviceId"></param>
        /// <param name="RenderingExperience"></param>
        /// <param name="LimboAlternativeCampaignTrackid"></param>
        /// <param name="LimboAlternativeCampaignTrackidUtilized"></param>
        /// <param name="FormTemplateType"></param>
        /// <param name="ProgramTemplateId"></param>
        /// <param name="ProgramProductId"></param>
        /// <param name="InstitutionId"></param>
        /// <returns></returns>
        public ManagedChoiceResultDTO ProcessWizardSubmit(int TemplateId, string RenderingStrategy, bool IsBeta, string TrackId, string SessionId, string MatchGuid, int? ProspectId, string LeadData, string AdditionalData, int SMLeadsCreatedCount, int USLeadsCreatedCount, bool? SplitCampusTypeInResults, string FESessionId, string DeviceId, string RenderingExperience, string LimboAlternativeCampaignTrackid, bool LimboAlternativeCampaignTrackidUtilized, FormTemplateTypes FormTemplateType, int? ProgramTemplateId, int? ProgramProductId, int? ProductId, int? InstitutionId, int? ApplicationId, string InstitutionName, string ProgramName, PerformanceLog Log)
        {
            ManagedChoiceResultDTO Result = new ManagedChoiceResultDTO();

            if (FormTemplateType == FormTemplateTypes.WizardTemplate)
            {
                Result = GetManagedChoiceDTO(TemplateId, RenderingStrategy, IsBeta, TrackId, SessionId, MatchGuid, ProspectId, LeadData, AdditionalData, SMLeadsCreatedCount, USLeadsCreatedCount, SplitCampusTypeInResults, DeviceId, FESessionId, RenderingExperience, LimboAlternativeCampaignTrackid, LimboAlternativeCampaignTrackidUtilized, ApplicationId, Log);
            }
            else if (FormTemplateType == FormTemplateTypes.ProgramWizard)
            {
                Result = SubmitProgramWizard(TemplateId, ProgramProductId.Value, ProductId.HasValue ? ProductId.Value : 0, IsBeta, TrackId, SessionId, ProspectId, RenderingStrategy, InstitutionId.Value, MatchGuid, LeadData, AdditionalData, Log, FESessionId, DeviceId, ApplicationId, InstitutionName, ProgramName);
            }

            return Result;
        }


        /// <summary>
        /// Process Wizard submission, saves smart matches and renders the School Selection view
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="RenderingStrategy"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackId"></param>
        /// <param name="SessionId"></param>
        /// <param name="MatchGuid"></param>
        /// <param name="ProspectId"></param>
        /// <param name="LeadData"></param>
        /// <param name="AdditionalData"></param>
        /// <param name="SMLeadsCreatedCount"></param>
        /// <param name="USLeadsCreatedCount"></param>
        /// <param name="SplitCampusTypeInResults"></param>
        /// <param name="DeviceId"></param>
        /// <param name="FESessionId"></param>
        /// <param name="RenderingExperience"></param>
        /// <param name="LimboAlternativeCampaignTrackid"></param>
        /// <param name="LimboAlternativeCampaignTrackidUtilized"></param>
        /// <param name="Log"></param>
        /// <returns></returns>
        private ManagedChoiceResultDTO GetManagedChoiceDTO(int TemplateId, string RenderingStrategy, bool IsBeta, string TrackId, string SessionId, string MatchGuid, int? ProspectId, string LeadData, string AdditionalData, int SMLeadsCreatedCount, int USLeadsCreatedCount, bool? SplitCampusTypeInResults, string DeviceId, string FESessionId, string RenderingExperience, string LimboAlternativeCampaignTrackid, bool LimboAlternativeCampaignTrackidUtilized, int? ApplicationId, PerformanceLog Log)
        {
            ManagedChoiceResultDTO Result = new ManagedChoiceResultDTO();
            bool IsAdvisingFlow = RenderingExperience == "Prospect";
            Result.SMLeadsCreatedCount = SMLeadsCreatedCount;
            Result.USLeadsCreatedCount = USLeadsCreatedCount;
            Result.Success = false;

            // redirect to Start if LeadData is not properly completed
            if (LeadData == null || LeadData == "null" || LeadData == "undefined" || LeadData.Length < 1)
            {
                Result.MoveToStart = true;
                return Result;
            }

            Log.StartLogDetail("GetManagedChoiceDTO.BuildRawDataObject");
            RawPostDataDTO RawData = BuildRawDataObject(LeadData);
            Log.EndLogDetail();

            //0. create LeadCreateRequest object
            #region Create LeadCreateRequest Object

            Guid? TrackingDeviceGUID = null;

            if (!String.IsNullOrWhiteSpace(DeviceId))
            {
                Guid tempDeviceGuid;

                if (Guid.TryParse(DeviceId, out tempDeviceGuid))
                {
                    TrackingDeviceGUID = tempDeviceGuid;
                }
            }

            Log.StartLogDetail("GetManagedChoiceDTO.BuildLeadCreateRequestObject");
            int ProgramProductId = 0;
            try
            {
                MatchGuid = FESession.Get<Guid>(FESessionId, Constants.WIZARD_ME_TOKEN_SMARTMATCHRESULTS_KEY).ToString();
            }
            catch { } //If is in session, we need to implement tryget on the FESesssion object
            
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(TemplateId, ProgramProductId, IsBeta, TrackId, LimboAlternativeCampaignTrackid, LimboAlternativeCampaignTrackidUtilized, SessionId, MatchGuid, LeadData, AdditionalData, null, null, ProspectId);
            Log.EndLogDetail();
            #endregion

            Log.StartLogDetail("GetManagedChoiceDTO.BuildLeadDTO");
            LeadDTO LeadDTO = LeadEngineService.BuildLeadDTO(LeadRequest, false);
            Log.EndLogDetail();

            Log.StartLogDetail("GetManagedChoiceDTO.BuildProspectInput");
            ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
            ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
            Log.EndLogDetail();

            //1.B Sync/Async Prospect Service call 
            #region Prospect Service
            int ProspectFlowId = FESession.Get(SessionId, Constants.PROSPECT_WORKFLOWID) == null ? 0 : (int)FESession.Get(SessionId, Constants.PROSPECT_WORKFLOWID);
            ProspectFlowTypes pft = IsAdvisingFlow ? ProspectFlowTypes.Advising : ProspectFlowTypes.Prospecting;
            SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.GetValueOrDefault(), pft);
            if (ProspectId == null || ProspectId < 1 || ProspectFlowId == 0)
            {
                Log.StartLogDetail("GetManagedChoiceDTO.SyncProspectServiceCall");
                var ProspectResult = FormsEngineService.RelatedServices.SaveProspect(WebServiceProspect);
                ProspectId = ProspectResult.ProspectId;
                ProspectFlowId = ProspectResult.ProspectFlowId;
                Log.EndLogDetail();
            }
            else
            {
                Log.StartLogDetail("GetManagedChoiceDTO.ASyncProspectServiceCall");
                FormsEngineService.RelatedServices.SaveProspectAsync(WebServiceProspect);
                Log.EndLogDetail();
            }
            LeadRequest.ProspectId = ProspectId;
            Result.ProspectId = ProspectId;
            #endregion

            //if this is a Prospect Only experience return here and redirect to nomatch
            if (IsAdvisingFlow)
            {
                //Save Submission record
                SaveSubmissionAsync(LeadRequest, FESessionId, new APIValidationResultDTO() { IsTestLead = false, Valid = true, ValidationMessages = new List<KeyValuePair<string, string>>() }, (int)SubmissionTypes.Prospect, ProspectFlowId);
                //fire off pixel for advising flow
                List<string> pixelTypes = new List<string>() { "Prospect Conversion" };
                var PixelCheck = GetPixels(IsBeta, WebServiceProspect.Prospect.LastName, WebServiceProspect.Prospect.Email, TrackId, ProspectId, true, new int[0], pixelTypes, IsAdvisingFlow);
                var PreviousPixels = (string)FESession.Get(FESessionId, Constants.WIZARD_USERSELECTPIXELS_KEY);
                if (!string.IsNullOrWhiteSpace(PreviousPixels))
                {
                    PixelCheck.PixelsWithDebugInfo = PreviousPixels + PixelCheck.PixelsWithDebugInfo;
                }
                FESession.Set(FESessionId, Constants.WIZARD_ADVISINGFLOWPIXELS_KEY, PixelCheck.PixelsWithDebugInfo);

                Result.MoveToNoMatch = true;
                return Result;
            }

            //1. Express Consent Mobile Check call
            #region Express Consent
            Log.StartLogDetail("GetManagedChoiceDTO.CheckForExpressConsent");
            ExpressConsentCheckDTO ExpressConsentCheck = CheckForMobile(LeadDTO.Phone1, LeadDTO.Phone2, Log); //phone1 phone2 email first last
            Log.EndLogDetail();
            #endregion

            // GetManagedChoice
            Log.StartLogDetail("GetManagedChoiceDTO.GetManagedChoice");
            Result.UserFullName = LeadDTO.FirstName + " " + LeadDTO.LastName;
            CampusPreference? CampusSoftPreference = null;
            if (LeadRequest.LeadData.ContainsKey("CampusSoftPreference"))
            {
                string cp = LeadRequest.LeadData["CampusSoftPreference"].ToLower();
                if (cp == "online") { CampusSoftPreference = EDDY.IS.FormsEngine.MatchingEngine.CampusPreference.Online; }
                else if (cp == "campus") { CampusSoftPreference = EDDY.IS.FormsEngine.MatchingEngine.CampusPreference.Ground; }
                else { CampusSoftPreference = EDDY.IS.FormsEngine.MatchingEngine.CampusPreference.Both; }
            }
            bool DynamicCampusSoftPreferenceShown = LeadRequest.LeadData.ContainsKey("CampusSoftPreferenceShown") ? Convert.ToBoolean(LeadRequest.LeadData["CampusSoftPreferenceShown"]) : false;
            string Theme = LeadRequest.LeadAdditionalData.ContainsKey("Theme") ? LeadRequest.LeadAdditionalData["Theme"] : "default";
            Result = GetRenderedManagedChoice(LeadRequest, LeadData, AdditionalData, RenderingStrategy, RawData, ExpressConsentCheck, Prospect, CampusSoftPreference, DynamicCampusSoftPreferenceShown, Result.UserFullName, Theme, SMLeadsCreatedCount, USLeadsCreatedCount, FESessionId, SplitCampusTypeInResults, TrackingDeviceGUID, false, null, null, false, false, ApplicationId, Log);
            Log.EndLogDetail();

            Log.StartLogDetail("GetManagedChoiceDTO.AsyncSaveProspectWizardStatus");
            FormsEngineService.RelatedServices.SaveProspectWizardStatus(ProspectFlowId, Result.MoveToNoMatch);
            Log.EndLogDetail();

            return Result;
        }


        /// <summary>
        /// Job Microsites ContactMe
        /// </summary>
        /// <param name="TrackId"></param>
        /// <param name="SessionId"></param>
        /// <param name="MatchGuid"></param>
        /// <param name="ProspectId"></param>
        /// <param name="LeadData"></param>
        /// <param name="AdditionalData"></param>
        /// <param name="DeviceId"></param>
        /// <param name="FESessionId"></param>
        /// <param name="Log"></param>
        /// <param name="TemplateId"></param>
        /// <param name="IsBeta"></param>
        /// <param name="SiteURL"></param>
        /// <param name="Job"></param>
        /// <param name="PostalCode"></param>
        public void SaveProspectForJobContactMe(string TrackId, string SessionId, string MatchGuid, int? ProspectId, string LeadData, string AdditionalData, string DeviceId, string FESessionId, PerformanceLog Log, int? TemplateId, bool IsBeta, string SiteURL, string Job, string PostalCode)
        {
            Log.StartLogDetail("SaveProspectForJobContactMe.BuildLeadCreateRequestObject");
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(TemplateId.GetValueOrDefault(), 0, false, TrackId, "", false, SessionId, MatchGuid, LeadData, AdditionalData, null, null, ProspectId);
            Log.EndLogDetail();

            //Async prospect save and status
            string ProspectAdvisorNotes = "Job Title: {0}\nZipCode: {1}";
            ProspectAdvisorNotes = string.Format(ProspectAdvisorNotes, Job, PostalCode);
            SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.GetValueOrDefault(), ProspectFlowTypes.Jobs, ProspectAdvisorNotes);
            Log.StartLogDetail("SaveProspectForJobContactMe.SyncProspectServiceCall");
            Task.Run(() => SaveJobContactMe(LeadRequest, WebServiceProspect, FESessionId, new APIValidationResultDTO() { IsTestLead = false, Valid = true, ValidationMessages = new List<KeyValuePair<string, string>>() }, SiteURL));
            Log.EndLogDetail();
        }

        private void SaveJobContactMe(LeadCreateRequest LeadRequest, SaveProspectRequest WebServiceProspect, string FESessionId, DTO.APIValidationResultDTO APIValidationResultDTO, string SiteURL)
        {
            int? prospectFlowId;
            int prospectId = FormsEngineService.RelatedServices.SaveJobContactMeProspectAndStatus(WebServiceProspect, SiteURL, out prospectFlowId);
            SaveSubmissionAsync(LeadRequest, FESessionId, APIValidationResultDTO, (int)SubmissionTypes.Prospect, prospectFlowId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Phone1"></param>
        /// <param name="Phone2"></param>
        /// <param name="AllNumbersAsMobile"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public ExpressConsentCheckDTO CheckForMobile(string Phone1, string Phone2, PerformanceLog log)
        {
            ExpressConsentCheckDTO ExpressConsentCheck = new ExpressConsentCheckDTO();
            ExpressConsentCheck.MobilePhones = new List<string>();
            string phone1Cleaned = Phone1 == null ? Phone1 : StripPhone(Phone1);
            string phone2Cleaned = Phone2 == null ? Phone2 : StripPhone(Phone2);

            log.StartLogDetail("DetermineMobilePhones");

            try
            {
                if (!string.IsNullOrWhiteSpace(phone1Cleaned))
                {
                    ExpressConsentCheck.MobilePhones.Add(phone1Cleaned);
                }

                if (!string.IsNullOrWhiteSpace(phone2Cleaned) && phone1Cleaned != phone2Cleaned)
                {
                    ExpressConsentCheck.MobilePhones.Add(phone2Cleaned);
                }

            }
            catch (Exception ex)
            {
                new ISException(ex).Save();
            }

            log.EndLogDetail();
            ExpressConsentCheck.ShowExpressConsent = ExpressConsentCheck.MobilePhones.Count > 0;

            return ExpressConsentCheck;
        }

        private static string StripPhone(string input)
        {
            string phone = "";

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    phone += c;
                }
            }

            return phone;
        }



        /// <summary>
        /// Gets only the rendered managed choice html and css path
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="RenderingStrategy"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackIdGuid"></param>
        /// <param name="TrackingSessionGUID"></param>
        /// <param name="MatchGuid"></param>
        /// <param name="ProspectId"></param>
        /// <param name="RawData"></param>
        /// <param name="LeadData"></param>
        /// <param name="AdditionalData"></param>
        /// <param name="ExpressConsentCheck"></param>
        /// <param name="ProspectInput"></param>
        /// <param name="CampusSoftPreference"></param>
        /// <param name="CampusSoftPreferenceShown"></param>
        /// <param name="UserFullName"></param>
        /// <param name="Theme"></param>
        /// <param name="PreviousSMLeadsCreatedCount"></param>
        /// <param name="PreviousUSLeadsCreatedCount"></param>
        /// <param name="FESessionId"></param>
        /// <param name="LeadDataDictionary"></param>
        /// <param name="SplitCampusTypeInResults"></param>
        /// <param name="TrackingDeviceGUID"></param>
        /// <param name="Log"></param>
        /// <returns></returns>
        public ManagedChoiceResultDTO GetRenderedManagedChoice(
                                                LeadCreateRequest LeadRequest,
                                                string LeadData,
                                                string LeadAdditionalData,
                                                string RenderingStrategy,
                                                RawPostDataDTO RawData,
                                                ExpressConsentCheckDTO ExpressConsentCheck,
                                                ProspectInput ProspectInput,
                                                CampusPreference? CampusSoftPreference,
                                                bool CampusSoftPreferenceShown,
                                                string UserFullName,
                                                string Theme,
                                                int PreviousSMLeadsCreatedCount,
                                                int PreviousUSLeadsCreatedCount,
                                                string FESessionId,
                                                bool? SplitCampusTypeInResults,
                                                Guid? TrackingDeviceGUID,
                                                bool CrossSellSource,
                                                WizardMatchResponse WizardMatchResponse,
                                                string ProgramWizardMessage,
                                                bool ProgramWizardInitialLeadValid,
                                                bool IsPreCheckEnabled,
                                                int? ApplicationId,
                                                PerformanceLog Log
                                        )
        {
            ManagedChoiceModel managedChoiceModel = new ManagedChoiceModel();
            managedChoiceModel.ProgramWizardInitialLeadValid = ProgramWizardInitialLeadValid;
            managedChoiceModel.HasSmartMatchPrograms = false;
            managedChoiceModel.HasUserSelectPrograms = false;
            managedChoiceModel.IsPreCheckEnabled = IsPreCheckEnabled;
            managedChoiceModel.ExpressConsentCheck = ExpressConsentCheck;
            managedChoiceModel.UserFullName = string.IsNullOrEmpty(UserFullName) ? "" : UserFullName;
            managedChoiceModel.CampusSoftPreference = CampusSoftPreference;
            managedChoiceModel.CampusSoftPreferenceShown = CampusSoftPreferenceShown;
            managedChoiceModel.AlreadyAskedTemplateIds = FESession.Get<List<int>>(FESessionId, Constants.WIZARD_SESSION_PROGRAMTEMPLATESCOVERED);
            managedChoiceModel.ApplicationId = ApplicationId;

            ManagedChoiceResultDTO result = new ManagedChoiceResultDTO();
            result.Success = false;
            result.SMLeadsCreatedCount = PreviousSMLeadsCreatedCount;
            result.USLeadsCreatedCount = PreviousUSLeadsCreatedCount;
            result.UserFullName = UserFullName;
            result.ProspectId = LeadRequest.ProspectId;
            managedChoiceModel.ProgramWizardMessage = CrossSellSource ? ProgramWizardMessage : "";

            StringBuilder ManagedChoiceDebugInfo = new StringBuilder();
            ManagedChoiceDebugInfo.Append("<div id=\"dvManagedChoiceDebugInfo\" class=\"hide\">");
            bool matchingEngineCallSuccessful = false;
            Guid schoolSelectionMatchGuid = default(Guid);
            List<string> SchoolsToBeExcludedByAds = new List<string>();
            List<string> LeadCreatedProducts = new List<string>();
            List<int> smInstitutionsToInclude = new List<int>();

            bool? skipSS = FESession.Get<bool?>(FESessionId, Constants.ME_SKIPSCHOOLSELECTIONFORMATCHONE);

            try
            {
                // If there is an initial institution, let's add it to the list of schools to exclude from ads..
                var initialInstitution = FESession.Get<ProgramWithInstitutionCampus>(FESessionId, Constants.PROGRAMWIZARD_INITIALCAMPUS_KEY);
                if (initialInstitution != null)
                {
                    SchoolsToBeExcludedByAds.Add(FESession.Get<ProgramWithInstitutionCampus>(FESessionId, Constants.PROGRAMWIZARD_INITIALCAMPUS_KEY).InstitutionName);
                    FESession.Set(FESessionId, Constants.WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY, SchoolsToBeExcludedByAds);

                    LeadCreatedProducts.Add(initialInstitution.ProductId?.ToString());
                    FESession.Set(FESessionId, Constants.WIZARD_LEADCREATEDPRODUCTS_KEY, LeadCreatedProducts);
                }

                int previousTotalLeadCount = result.SMLeadsCreatedCount + result.USLeadsCreatedCount;
                ManagedChoiceDebugInfo.Append(" | Previous SM Leads Created = " + result.SMLeadsCreatedCount.ToString());
                ManagedChoiceDebugInfo.Append(" | Previous US Leads Created = " + result.USLeadsCreatedCount.ToString());
                ManagedChoiceDebugInfo.Append(" | Mobile Numbers Count = " + ExpressConsentCheck.MobilePhones.Count().ToString());

                Log.StartLogDetail("GetRenderedManagedChoice.GetRenderingStrategies");
                HTMLRenderingStrategyDTO RS = FormsEngineService.GetHTMLRenderingStrategy(RenderingStrategy);
                Log.EndLogDetail();

                managedChoiceModel.Theme = string.IsNullOrWhiteSpace(Theme) ? "default" : Theme;

                if (RS != null)
                {
                    bool hasMobilePhones = (ExpressConsentCheck.MobilePhones.Count() > 0);

                    // get SMs from session from Wizard or previous call
                    List<ProgramWithInstitutionCampus> PreviousSmartMatchList = FESession.Get<List<ProgramWithInstitutionCampus>>(FESessionId, Constants.WIZARD_ME_SMARTMATCHRESULTS_KEY);
                    List<ProgramWithInstitutionCampus> ThirdPartySmartMatchList1 = FESession.Get<List<ProgramWithInstitutionCampus>>(FESessionId, Constants.WIZARD_ME_THIRDPARTYSMARTMATCHRESULTS_KEY);

                    //TCPA testing - Remove smart matches that weren't selected as part of the TCPA
                    if (LeadRequest.LeadData.ContainsKey("IsNewSmartMatchTCPA") && LeadRequest.LeadData["IsNewSmartMatchTCPA"] == "Yes")
                    {
                        if (LeadRequest.LeadData.ContainsKey("TCPASelectedSchoolList"))
                        {
                            smInstitutionsToInclude = ConvertToIntList(LeadRequest.LeadData["TCPASelectedSchoolList"]);
                        }
                        PreviousSmartMatchList = PreviousSmartMatchList.Where(a => smInstitutionsToInclude.Contains(a.InstitutionId)).ToList();
                        ThirdPartySmartMatchList1 = ThirdPartySmartMatchList1.Where(a => smInstitutionsToInclude.Contains(a.InstitutionId)).ToList();
                    }

                    bool hasSMsFromSession = (PreviousSmartMatchList != null && PreviousSmartMatchList.Count() > 0) || (ThirdPartySmartMatchList1 != null && ThirdPartySmartMatchList1.Count() > 0);

                    bool saveSmartMatches = (PreviousSMLeadsCreatedCount < 1) ? true : false;
                    bool? saveThirdPartySM = FESession.Get<bool?>(FESessionId, Constants.WIZARD_ME_SAVETHIRDPARTYSMARTMATCHRESULTS_KEY);

                    // if No SM Leads created so far, then get SM's from ME 
                    bool includeSMs = false;
                    bool includeUSs = true;
                    List<int> institutionsSmartMatched = null;

                    if (hasSMsFromSession)
                    {
                        institutionsSmartMatched = new List<int>();

                        if(PreviousSmartMatchList != null && PreviousSmartMatchList.Any())
                            institutionsSmartMatched.AddRange(PreviousSmartMatchList.Select(sm => sm.InstitutionId));

                        if (ThirdPartySmartMatchList1 != null && ThirdPartySmartMatchList1.Any())
                            institutionsSmartMatched.AddRange(ThirdPartySmartMatchList1.Select(sm => sm.InstitutionId));

                        SchoolsToBeExcludedByAds.AddRange(PreviousSmartMatchList.Select(sm => sm.InstitutionName).ToList());
                        FESession.Set(FESessionId, Constants.WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY, SchoolsToBeExcludedByAds);

                        LeadCreatedProducts.AddUnique(PreviousSmartMatchList.Select(sm => sm.ProductId?.ToString()));
                        FESession.Set(FESessionId, Constants.WIZARD_LEADCREATEDPRODUCTS_KEY, LeadCreatedProducts);

                    }

                    int ProspectFlowId = FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID) == null ? 0 : (int)FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID);
                    
                    if (!CrossSellSource)
                    {
                        Log.StartLogDetail("GetRenderedManagedChoice.GetProgramsForManagedChoice");
                        WizardMatchResponse = FormsEngineService.GetProgramsForManagedChoice(LeadRequest.TrackId.GetValueOrDefault(), LeadRequest.LimboAlternativeCampaignTrackid.GetValueOrDefault(), LeadRequest.LimboAlternativeCampaignTrackidUtilized, ProspectInput, CampusSoftPreference, LeadRequest.IsBeta, result.SMLeadsCreatedCount, result.USLeadsCreatedCount, includeSMs, includeUSs, LeadRequest.LeadData, institutionsSmartMatched, SplitCampusTypeInResults, TrackingDeviceGUID, FESessionId, ApplicationId, ThirdPartySmartMatchList1);

                        if (WizardMatchResponse != null)
                        {
                            FESession.Set(FESessionId, Constants.SUBMISSION_SS_MATCHRESPONSEGUID, WizardMatchResponse.MatchResponseGuid);
                        }

                        Log.EndLogDetail();
                    }

                    matchingEngineCallSuccessful = (WizardMatchResponse != null) ? true : false;
                    int newSMLeadsCreatedCount = 0;

                    if (matchingEngineCallSuccessful)
                    {
                        schoolSelectionMatchGuid = WizardMatchResponse.MatchResponseGuid;

                        //if matching engine forces a limbo wipe out the school selection list
                        if (WizardMatchResponse.WizardLimboReason == LimboReason.LeadScoringMinimumTierLevel)
                        {
                            FESession.Set(FESessionId, Constants.WIZARD_ME_LIMBOREASON_KEY, WizardMatchResponse.WizardLimboReason);
                            WizardMatchResponse.SchoolSelectionList = null;
                        }

                        if (LeadRequest.MatchResponseGuid == default(Guid))
                        {
                            LeadRequest.MatchResponseGuid = schoolSelectionMatchGuid;
                        }

                        // set the US List
                        WizardMatchResponse.SchoolSelectionList = WizardMatchResponse.SchoolSelectionList == null ? new CampusWithInstitution[] { } : WizardMatchResponse.SchoolSelectionList;
                        managedChoiceModel.SchoolSelectionList = new List<CampusWithInstitution>(WizardMatchResponse.SchoolSelectionList);
                        managedChoiceModel.MaxUserSelections = WizardMatchResponse.MaxUserSelections;

                        SchoolsToBeExcludedByAds.AddRange(managedChoiceModel.SchoolSelectionList.Select(us => us.InstitutionName).ToList());
                        FESession.Set(FESessionId, Constants.WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY, SchoolsToBeExcludedByAds);

                        // set the SM List
                        if (hasSMsFromSession)
                        {
                            managedChoiceModel.SmartMatchList = PreviousSmartMatchList;
                        }

                        //enable server side validation for smart matches
                        LeadSaveManger leadSaveManager = new LeadSaveManger(Log);
                        LeadRequest.LeadCreationTypeId = (int)LeadCreationType.WizardSmartMatch;
                        APIValidationResultDTO QuickCheckValidationResult = FormsEngineService.QuickCheckValidateForm(LeadRequest.TemplateId.GetValueOrDefault(), LeadRequest.IsBeta, LeadRequest.TrackId.GetValueOrDefault(), LeadRequest.LeadData, ref Log);

                        List<LeadSaveData> leadSaveList = new List<LeadSaveData>();
                        List<LeadCreateResponse> leadCreateResponseList = new List<LeadCreateResponse>();

                        // create SM Leads and get Pixels
                        if (managedChoiceModel.SmartMatchList != null && managedChoiceModel.SmartMatchList.Count > 0 && QuickCheckValidationResult.Valid)
                        {
                            managedChoiceModel.HasSmartMatchPrograms = true;
                            if (saveSmartMatches)
                            {
                                foreach (ProgramWithInstitutionCampus program in managedChoiceModel.SmartMatchList)
                                {
                                    if (program.PaidStatusTypeId == null)
                                    {
                                        leadSaveList.Add(new LeadSaveData() { ProgramProductId = Convert.ToInt32(program.ProgramProductId), TemplateId = Convert.ToInt32(program.TemplateId), PaidStatusTypeId = (int)PaidStatusType.Paid, AllowedViaLeadScoringUpsell = program.AllowedViaLeadScoringUpsell, ProductId = program.ProductId, InstitutionId = program.InstitutionId, InstitutionName = program.InstitutionName });
                                    }
                                    else
                                    {
                                        leadSaveList.Add(new LeadSaveData() { ProgramProductId = Convert.ToInt32(program.ProgramProductId), TemplateId = Convert.ToInt32(program.TemplateId), PaidStatusTypeId = (int)program.PaidStatusTypeId.GetValueOrDefault(), AllowedViaLeadScoringUpsell = program.AllowedViaLeadScoringUpsell, ProductId = program.ProductId, InstitutionId = program.InstitutionId,  InstitutionName = program.InstitutionName });
                                    }

                                }
                            }
                        }

                        //save third party smart matches
                        if (saveThirdPartySM != false)
                        {
                            List<ProgramWithInstitutionCampus> ThirdPartySmartMatchList = FESession.Get<List<ProgramWithInstitutionCampus>>(FESessionId, Constants.WIZARD_ME_THIRDPARTYSMARTMATCHRESULTS_KEY);
                            if (LeadRequest.LeadData.ContainsKey("IsNewSmartMatchTCPA") && LeadRequest.LeadData["IsNewSmartMatchTCPA"] == "Yes")
                            {
                                if (LeadRequest.LeadData.ContainsKey("TCPASelectedSchoolList"))
                                {
                                    smInstitutionsToInclude = ConvertToIntList(LeadRequest.LeadData["TCPASelectedSchoolList"]);
                                }
                                ThirdPartySmartMatchList = ThirdPartySmartMatchList.Where(a => smInstitutionsToInclude.Contains(a.InstitutionId)).ToList();
                            }

                            if (ThirdPartySmartMatchList != null && ThirdPartySmartMatchList.Count > 0)
                            {
                                managedChoiceModel.HasSmartMatchPrograms = true;
                                managedChoiceModel.SmartMatchList = managedChoiceModel.SmartMatchList ?? new List<ProgramWithInstitutionCampus>();

                                SchoolsToBeExcludedByAds.AddRange(ThirdPartySmartMatchList.Select(tp => tp.InstitutionName).ToList());
                                FESession.Set(FESessionId, Constants.WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY, SchoolsToBeExcludedByAds);

                                LeadCreatedProducts.AddUnique(ThirdPartySmartMatchList.Select(tp => tp.ProductId?.ToString()));
                                FESession.Set(FESessionId, Constants.WIZARD_LEADCREATEDPRODUCTS_KEY, LeadCreatedProducts);

                                foreach (ProgramWithInstitutionCampus party in ThirdPartySmartMatchList)
                                {
                                    leadSaveList.Add(new LeadSaveData() { ProgramProductId = Convert.ToInt32(party.ProgramProductId)
                                        , LeadCreationType = LeadCreationType.ThirdPartySmartMatch
                                        , ExternalMatchItemGuid = party.ExternalMatchItemGuid
                                        , InstitutionName = party.InstitutionName
                                        , InstitutionId = party.InstitutionId
                                    });

                                    if (party.InstitutionType == MatchingEngine.InstitutionLeadTypes.ThirdPartyApiMatch)
                                    {
                                        managedChoiceModel.SmartMatchList.Add(new ProgramWithInstitutionCampus()
                                        {
                                            CampusName = party.InstitutionName,
                                            InstitutionName = party.InstitutionName,
                                            InstitutionDescription = party.InstitutionDescription,
                                            InstitutionDisclaimer = party.InstitutionDisclaimer,
                                            CampusAddress1 = party.CampusAddress1,
                                            CampusAddress2 = party.CampusAddress2,
                                            CampusCity = party.CampusCity,
                                            CampusState = party.CampusState,
                                            ProgramDescription = party.ProgramDescription,
                                            CampusLogoURL = "",
                                            InstitutionLogoURL = "",
                                            ProgramDisclaimer = party.ProgramDisclaimer,
                                            ProgramDisclaimerType = party.ProgramDisclaimerType,
                                            ProgramName = party.ProgramName,
                                            ProgramId = party.ProgramId,
                                            ProgramProductId = party.ProgramProductId,
                                            CampusType = CampusType.Ground
                                        });
                                    }
                                }

                                FESession.Set(FESessionId, Constants.WIZARD_ME_SAVETHIRDPARTYSMARTMATCHRESULTS_KEY, false);
                            }
                        }


                        //#67819  - Lead not getting saved with Alternative TrackID when used
                        if (WizardMatchResponse.LimboAlternativeTrackIdUtilized)
                        {
                            LeadRequest.LimboAlternativeCampaignTrackidUtilized = true;
                            LeadRequest.LimboAlternativeCampaignTrackid = WizardMatchResponse.LimboAlternativeTrackId;
                        }

                        if (leadSaveList.Count > 0)
                        {
                            Five9RouteStatus five9Status = FormsEngineService.RelatedServices.ProcessDialerRoute(LeadRequest, managedChoiceModel.SmartMatchList, ProspectFlowId);
                            Log.StartLogDetail("GetRenderedManagedChoice.LeadSubmissions");
                            leadCreateResponseList = leadSaveManager.Execute(LeadRequest.TemplateId.GetValueOrDefault(), FESessionId, LeadCreationType.WizardSmartMatch, null, leadSaveList, LeadRequest.IsBeta, LeadRequest.TrackId.GetValueOrDefault().ToString(), LeadRequest.LimboAlternativeCampaignTrackid.GetValueOrDefault().ToString(), LeadRequest.LimboAlternativeCampaignTrackidUtilized, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString(), LeadRequest.MatchResponseGuid.GetValueOrDefault().ToString(), RawData, LeadData, LeadAdditionalData, LeadRequest.ProspectId, null, null, MatchResponseType.SmartMatch, null, false, QuickCheckValidationResult, null, ProspectFlowId);

                            if(five9Status == Five9RouteStatus.WTTToThirdParty)
                            {
                                FormsEngineService.RelatedServices.RouteWTTToThirdParty(LeadRequest, leadCreateResponseList, managedChoiceModel.SmartMatchList);
                            }
                            Log.EndLogDetail();
                        }
                        else if (!CrossSellSource) //no smart matches or server side validation failed, but still save Submission record if not saved before
                        {
                            SaveSubmissionAsync(LeadRequest, FESessionId, QuickCheckValidationResult, (int)SubmissionTypes.Full, ProspectFlowId);
                        }

                        result.SmartMatchProgramCount = leadCreateResponseList.Where(lc => lc.Success && lc.Lead.LeadCreationTypeId == (int)LeadCreationType.WizardSmartMatch).Count();
                        newSMLeadsCreatedCount = result.SmartMatchProgramCount;
                        if (newSMLeadsCreatedCount > 0)
                        {
                            // set ME SM Results into Session for ThankYou to retrieve
                            FESession.Set(FESessionId, Constants.WIZARD_ME_SMARTMATCHRESULTS_KEY, managedChoiceModel.SmartMatchList);
                            // set SM Leads into Session for ThankYou to retrieve
                            FESession.Set(FESessionId, Constants.WIZARD_SMARTMATCHLEADS_KEY, leadSaveList);
                            ManagedChoiceDebugInfo.Append(" | New SM Leads Created = " + newSMLeadsCreatedCount.ToString());
                            result.SMLeadsCreatedCount = result.SMLeadsCreatedCount + newSMLeadsCreatedCount;
                            previousTotalLeadCount = result.SMLeadsCreatedCount + result.USLeadsCreatedCount;

                            #region Pixel Service
                            Log.StartLogDetail("GetRenderedManagedChoice.GetPixels");

                            int[] leadIds = leadCreateResponseList.Where(l => l.Success && (l.Lead.LeadCreationTypeId == (int)LeadCreationType.WizardSmartMatch || l.Lead.LeadCreationTypeId == (int)LeadCreationType.ThirdPartySmartMatch)).Select(id => Convert.ToInt32(id.Lead.LeadId)).ToArray();

                            List<string> pixelTypes = new List<string>();
                            if (PreviousSMLeadsCreatedCount < 1 && PreviousUSLeadsCreatedCount < 1)
                            {
                                pixelTypes.Add("Initial Conversion Only");
                            }
                            pixelTypes.Add("Conversion");
                            pixelTypes.Add("Cumulative Conversion");
                            pixelTypes.Add("Client Relation Pixel Conversion");

                            LeadCreateResponse firstValidLeadResonse = leadCreateResponseList.First(f => f.Success && f.Lead.LeadCreationTypeId == (int)LeadCreationType.WizardSmartMatch);

                            result.IsTestLead = firstValidLeadResonse.IsTestLead;
                            string lastName = firstValidLeadResonse.Lead.LastName;
                            string email = firstValidLeadResonse.Lead.EmailAddress;

                            var PixelCheck = GetPixels(LeadRequest.IsBeta, lastName, email, LeadRequest.TrackId.GetValueOrDefault().ToString(), LeadRequest.ProspectId, true, leadIds, pixelTypes, false);
                            FESession.Set(FESessionId, Constants.WIZARD_SMARTMATCHPIXELS_KEY, PixelCheck.PixelsWithDebugInfo);

                            Log.EndLogDetail();
                            #endregion
                        }

                        //after saving appropriate records if server side validation failed return
                        if (!QuickCheckValidationResult.Valid)
                        {
                            //server side validation for wizard falied. return to start of wizard.
                            result.Success = false;
                            result.MoveToStart = true;
                            return result;
                        }

                        // determine the max User Select User Selections
                        //          user selection count must be reduced by the previous lead submission count and be the lesser of it and the available program count
                        managedChoiceModel.MaxManagedChoiceUserSelections = DetermineMaxMCUserSelections(previousTotalLeadCount, managedChoiceModel.MaxUserSelections, managedChoiceModel.SchoolSelectionList.Count());
                        managedChoiceModel.MaxManagedChoiceUserSelectionsAlpha = Convert.ToDecimal(managedChoiceModel.MaxManagedChoiceUserSelections).ConverDecimalToLetter(false, false);
                        ManagedChoiceDebugInfo.Append(" | Max User Selections Allowed = " + managedChoiceModel.MaxManagedChoiceUserSelections.ToString() + "[(ME Response=" + managedChoiceModel.MaxUserSelections.ToString() + " -Previous Total Lead Count=" + previousTotalLeadCount.ToString() + ") OR User Select School Count]");


                        if (skipSS.GetValueOrDefault() && newSMLeadsCreatedCount > 0)
                        {
                            result.Success = true;
                            result.MoveToThankYou = true;
                            return result;
                        }
                        else if (managedChoiceModel.SchoolSelectionList.Count() > 0 && managedChoiceModel.MaxManagedChoiceUserSelections > 0)
                        {
                            string desiredDegree = StringExtensions.GetFieldValue("Desired_Degree_Level", LeadRequest.LeadData, false);
                            if (!String.IsNullOrWhiteSpace(desiredDegree))
                            {
                                managedChoiceModel.UserDesiredProgramLevelId = desiredDegree.Split(',').Select(f => Convert.ToInt32(f)).ToArray().First();
                            }

                            // set ME US Results into Session for ThankYou to retrieve
                            FESession.Set(FESessionId, Constants.WIZARD_ME_USERSELECTRESULTS_KEY, managedChoiceModel.SchoolSelectionList);
                            managedChoiceModel.HasUserSelectPrograms = true;
                            result.UserSelectSchoolCount = managedChoiceModel.SchoolSelectionList.Count();
                            ManagedChoiceDebugInfo.Append(" | US School Count = " + managedChoiceModel.SchoolSelectionList.Count().ToString());
                        }
                        else
                        {
                            if (newSMLeadsCreatedCount > 0)
                            {
                                RemoveSchoolSelectionSchoolsFromAdExclusion(managedChoiceModel.SchoolSelectionList, FESessionId);

                                result.Success = true;
                                result.MoveToThankYou = true;
                                return result;
                            }
                            else
                            {
                                result.Success = true;
                                result.MoveToNoMatch = true;
                                return result;
                            }
                        }
                    }
                }

                // only go through process of getting rendered managed choice if there are User Select programs to display and allowed selection count > 0
                Log.StartLogDetail("GetRenderedManagedChoice.RenderManagedChoice");
                if (managedChoiceModel.HasUserSelectPrograms && managedChoiceModel.MaxManagedChoiceUserSelections > 0)
                {
                    managedChoiceModel.MatchResponseGuid = schoolSelectionMatchGuid;

                    //get program descriptions for each of the first programs for the school
                    foreach (var ssitem in managedChoiceModel.SchoolSelectionList)
                    {
                        int chosenProgramId = 0;
                        string chosenProgramName = "";
                        var programByDegreeLevel = from program in ssitem.ProgramList
                                                   group program by program.ProgramLevelName
                                                   into programGroup
                                                   select new { ProgramLevelName = programGroup.Key, Programs = programGroup.ToList() };

                        decimal maxRankScore;
                        MatchingEngine.Program maxScoreProgram;

                        //Determine which program to get information for
                        if (managedChoiceModel.UserDesiredProgramLevelId > 0)
                        {
                            //user has a program level preference so get the program for that program level if it exists
                            if (ssitem.ProgramList.Any(a => a.ProgramLevelId == managedChoiceModel.UserDesiredProgramLevelId))
                            {
                                maxRankScore = (from p in programByDegreeLevel.First(a => a.Programs.First().ProgramLevelId == managedChoiceModel.UserDesiredProgramLevelId).Programs
                                                select p.ProgramRankScore).Max();

                                foreach (var programLevelName in programByDegreeLevel.Where(a => a.Programs.First().ProgramLevelId == managedChoiceModel.UserDesiredProgramLevelId))
                                {
                                    maxScoreProgram = programLevelName.Programs.OrderBy(p => p.ProgramName).FirstOrDefault(a => a.ProgramRankScore == maxRankScore);

                                    if (maxScoreProgram != null)
                                    {
                                        chosenProgramId = maxScoreProgram.ProgramId;
                                        chosenProgramName = maxScoreProgram.ProgramName;
                                        continue;
                                    }

                                }

                            }
                            else //otherwise 
                            {
                                maxRankScore = (from p in ssitem.ProgramList
                                                select p.ProgramRankScore).Max();

                                foreach (var programLevelName in programByDegreeLevel.OrderBy(a => a.ProgramLevelName))
                                {
                                    maxScoreProgram = programLevelName.Programs.OrderBy(p => p.ProgramName).FirstOrDefault(a => a.ProgramRankScore == maxRankScore);

                                    if (maxScoreProgram != null)
                                    {
                                        chosenProgramId = maxScoreProgram.ProgramId;
                                        chosenProgramName = maxScoreProgram.ProgramName;
                                        continue;
                                    }

                                }
                                
                            }
                        }
                        else
                        {
                            maxRankScore = (from p in ssitem.ProgramList
                                            select p.ProgramRankScore).Max();

                            foreach (var programLevelName in programByDegreeLevel.OrderBy(a => a.ProgramLevelName))
                            {
                                maxScoreProgram = programLevelName.Programs.OrderBy(p => p.ProgramName).FirstOrDefault(a => a.ProgramRankScore == maxRankScore);

                                if (maxScoreProgram != null)
                                {
                                    chosenProgramId = maxScoreProgram.ProgramId;
                                    chosenProgramName = maxScoreProgram.ProgramName;
                                    continue;
                                }

                            }
                        }
                        
                        ProgramResponse programDescription =  FormsEngineService.RelatedServices.GetPrograms(chosenProgramId, false, LeadRequest.TrackId.GetValueOrDefault());
                        if (programDescription != null && programDescription.ProgramList.Any())
                        {
                            managedChoiceModel.ProgramRankInstitutions.Add(new ProgramRankInstitution() { ProgramDescription = !string.IsNullOrEmpty(programDescription.ProgramList.First().ProgramDescription) ? Regex.Replace(programDescription.ProgramList.First().ProgramDescription, @"<(.|\n)*?>", string.Empty) : string.Empty, InstitutionId = ssitem.InstitutionId ?? 0, ProgramId = chosenProgramId, ProgramName = chosenProgramName });
                        }
                    }

                    if (RS != null)
                    {
                        if ((RS.Name.ToLower() == "wizardprofessionalbootstrap" || RS.Name.ToLower() == "wizardprofessional")
                            && managedChoiceModel.Theme.ToLower() == "alternative")
                        {
                            RS.ManagedChoiceTemplateView = RS.ManagedChoiceTemplateView.Replace("ManagedChoice.cshtml", "ManagedChoice_Alternative.cshtml");
                        }
                        else if((RS.Name.ToLower() == "wizardprofessionalbootstrap" || RS.Name.ToLower() == "programwizard")
                            && managedChoiceModel.Theme.ToLower() == "daisychain")
                        {
                            RS.ManagedChoiceTemplateView = RS.ManagedChoiceTemplateView.Replace("ManagedChoice.cshtml", "ManagedChoice_Daisychain.cshtml");
                        }
                        else if ((RS.Name.ToLower() == "wizardprofessionalbootstrap" || RS.Name.ToLower() == "programwizard")
                            && managedChoiceModel.Theme.ToLower() == "daisychainplain")
                        {
                            RS.ManagedChoiceTemplateView = RS.ManagedChoiceTemplateView.Replace("ManagedChoice.cshtml", "ManagedChoice_DaisychainPlain.cshtml");
                        }

                        result.RenderedManagedChoice = this.RazorViewToString(RS.ManagedChoiceTemplateView, managedChoiceModel, true);
                    }
                    result.MaxManagedChoiceUserSelections = managedChoiceModel.MaxManagedChoiceUserSelections;

                    if (result.RenderedManagedChoice != null && result.RenderedManagedChoice.Length > 0)
                    {
                        result.Success = true;
                    }

                    ManagedChoiceDebugInfo.Append("</div>");
                    result.RenderedManagedChoice = (result.RenderedManagedChoice != null) ? result.RenderedManagedChoice + Url.Encode(ManagedChoiceDebugInfo.ToString()) : Url.Encode(ManagedChoiceDebugInfo.ToString());
                }
                else
                {
                    ManagedChoiceDebugInfo.Append("Managed Choice will only be rendered when the following conditions are true::: ");
                    ManagedChoiceDebugInfo.Append(" | ManagedChoiceProgramResponse returned sucessfully = " + matchingEngineCallSuccessful.ToString());
                    int uSCount = (matchingEngineCallSuccessful) ? managedChoiceModel.SchoolSelectionList.Count() : 0;
                    ManagedChoiceDebugInfo.Append(" | ManagedChoiceProgramResponse.SchoolSelectionList.Count is greater than zero = " + uSCount.ToString());
                    ManagedChoiceDebugInfo.Append(" | ManagedChoiceProgramResponse.MaxManagedChoiceUserSelections.Count is greater than zero = " + managedChoiceModel.MaxManagedChoiceUserSelections.ToString());
                    ManagedChoiceDebugInfo.Append("</div>");

                    result.RenderedManagedChoice = Url.Encode(ManagedChoiceDebugInfo.ToString());

                    if (!managedChoiceModel.HasSmartMatchPrograms && managedChoiceModel.MaxManagedChoiceUserSelections < 1)
                    {
                        result.MoveToNoMatch = true;
                    }
                }
                Log.EndLogDetail();
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
                result.Success = false;
                result.MoveToNoMatch = true;
            }

            if (schoolSelectionMatchGuid != default(Guid))
            {
                result.MatchGuid = schoolSelectionMatchGuid;
            }
            else
            {
                result.MatchGuid = null;
            }

            return result;
        }

        private List<int> ConvertToIntList(string commaDelimitedString)
        {
            if (string.IsNullOrEmpty(commaDelimitedString))
                return new List<int>();

            return commaDelimitedString
                .Split(',')
                .Select(s => s.Trim())
                .Select(int.Parse)
                .ToList();
        }

        private static void SaveSubmissionAsync(LeadCreateRequest LeadRequest, string FESessionId, APIValidationResultDTO QuickCheckValidationResult, int submissionType, int? prospectFlowId)
        {
            Task.Run(() =>
            {
                LeadSaveManger leadSave = new LeadSaveManger(new PerformanceLog());
                LeadRequest.PassedValidation = QuickCheckValidationResult.Valid;
                leadSave.SaveSubmission(LeadRequest.TemplateId.GetValueOrDefault(), LeadRequest, LeadRequest.ProspectId.GetValueOrDefault(), null, FESessionId, QuickCheckValidationResult, MatchResponseType.SchoolSelection, submissionType, null, prospectFlowId);
            });
        }

        private static int DetermineMaxMCUserSelections(int previousTotalLeadCount, int? MEMaxUserSelections, int SchoolSelectionListCount)
        {
            int MaxManagedChoiceUserSelections;

            MaxManagedChoiceUserSelections = Convert.ToInt32(MEMaxUserSelections);
            MaxManagedChoiceUserSelections -= previousTotalLeadCount;
            MaxManagedChoiceUserSelections = Math.Min(MaxManagedChoiceUserSelections, SchoolSelectionListCount);
            return MaxManagedChoiceUserSelections;
        }


        public static PixelCheckDTO GetPixels(bool IsBeta, string LastName, string Email, string TrackId, int? ProspectId, bool passedInitialLeadValidation, int[] leadIds, List<string> pixelTypes, bool IsAdvisingFlow)
        {


            bool leadsCreated = (leadIds.Count() > 0);
            PixelCheckDTO PixelCheck = new PixelCheckDTO();
            if ((IsAdvisingFlow && (ProspectId.HasValue && ProspectId > 0)) || (leadsCreated && passedInitialLeadValidation && !IsBeta && (ProspectId.HasValue && ProspectId > 0) &&
                    // rules per Kim 6/12/13 // updated by Erick, Roberto & Pete, disliked by Zack & Alexis :)
                    (!LeadEngineService.IsTestLead("", LastName, Email) || IsPixelTest(LastName, Email)))
                )
            {
                PixelCheck.Pixels = FormsEngineService.RelatedServices.GetPixelsData(TrackId, pixelTypes.ToArray(), ProspectId.Value, leadIds);
                PixelCheck.ServiceCalled = true;
                StringBuilder pixelDebugInfo = new StringBuilder();
                pixelDebugInfo.Append("<div id=\"pixelDebugInfo\" style=\"display: none;\"> ");
                pixelDebugInfo.Append("Pixel Service Called: ");
                pixelDebugInfo.Append(" | TrackId=" + TrackId);
                pixelDebugInfo.Append(" | PixelsTypes=" + string.Join(",", pixelTypes));
                pixelDebugInfo.Append(" | ProspectId=" + ProspectId.ToString());
                pixelDebugInfo.Append(" | LeadIds=" + string.Join(",", leadIds));
                pixelDebugInfo.Append(" | IsAdvisingFlow=" + string.Join(",", IsAdvisingFlow));
                if (PixelCheck.Pixels != null & PixelCheck.Pixels != "")
                {
                    pixelDebugInfo.Append(" | </div> ");
                    PixelCheck.PixelsWithDebugInfo = pixelDebugInfo.ToString() + PixelCheck.Pixels;    //Url.Encode();
                }
                else
                {
                    pixelDebugInfo.Append(" | NO PIXELS RETURNED </div> ");
                    PixelCheck.PixelsWithDebugInfo = pixelDebugInfo.ToString();    //Url.Encode();
                }
            }
            else
            {
                StringBuilder pixelDebugInfo = new StringBuilder();
                pixelDebugInfo.Append("<div id=\"pixelDebugInfo\" style=\"display: none;\"> ");
                pixelDebugInfo.Append("Pixel Service Not Called: ");
                pixelDebugInfo.Append("All of the following must be true:: ");
                pixelDebugInfo.Append(" | ProgramsPassedMatchingEngineValidation=" + passedInitialLeadValidation.ToString());
                pixelDebugInfo.Append(" | LeadsCreated=" + leadsCreated.ToString());
                pixelDebugInfo.Append(" | IsNotBeta=" + (!IsBeta).ToString());
                pixelDebugInfo.Append(" | ProspectId != null OR 0");
                pixelDebugInfo.Append(" | Pixels will NOT be called if Email contains @test.com OR LastName == test, EXCEPT you can override by using Email contains @test.com && LastName == pixeltest,pixelstest,testpixel,or testpixels");
                pixelDebugInfo.Append(" | </div> ");
                PixelCheck.PixelsWithDebugInfo = pixelDebugInfo.ToString(); //Url.Encode();
            }
            return PixelCheck;
        }

        private static bool IsPixelTest(string LastName, string Email)
        {
            // way to test pixels, do this:
            bool isPixelTest = false;
            isPixelTest = (
                            Email.ToLower().Trim().Contains("@test.com") &&
                                (
                                       LastName.ToLower().Trim() == "pixeltest"
                                    || LastName.ToLower().Trim() == "pixelstest"
                                    || LastName.ToLower().Trim() == "testpixel"
                                    || LastName.ToLower().Trim() == "testpixels"
                                )
                        );
            return isPixelTest;
        }




        private void bindProgramTemplateMessage(ref CrossSellModel TheCrossSellModel, ref string CrossSellThankYouMessage
            , string Type, bool IsSuccess, string UserFirstName, string InstitutionName)
        {
            List<ProgramTemplateMessageDTO> TheProgramTemplateMessageDTOList = null;

            try
            {
                TheProgramTemplateMessageDTOList = FormsEngineService.GetProgramTemplateMessageForType(Type);

                if (TheProgramTemplateMessageDTOList != null)
                {
                    ProgramTemplateMessageDTO TheProgramTemplateMessageDTO = null;

                    TheProgramTemplateMessageDTO = TheProgramTemplateMessageDTOList
                        .SingleOrDefault(t => t.ProgramTemplateMessageCriteriaName.ToUpper() == (IsSuccess ? PROGRAMTEMPLATE_MESSAGECRITERIA.CROSSSELL_TOPMESSAGE_SUCCESS : PROGRAMTEMPLATE_MESSAGECRITERIA.CROSSSELL_TOPMESSAGE_FAILURE).ToString());
                    if (TheProgramTemplateMessageDTO != null) TheCrossSellModel.CrossSellHeaderMessage = (IsSuccess ? string.Format(TheProgramTemplateMessageDTO.ResourceMetaDataText, InstitutionName) : string.Format(TheProgramTemplateMessageDTO.ResourceMetaDataText, UserFirstName, InstitutionName));

                    if (TheCrossSellModel.CrossSellProgramResponse != null && TheCrossSellModel.CrossSellProgramResponse.IsPreCheckEnabled.HasValue && TheCrossSellModel.CrossSellProgramResponse.IsPreCheckEnabled.Value)
                    {
                        TheCrossSellModel.CrossSellSubHeaderMessage = FormsEngine.GetResourceMetaDataTextForKey("CROSSSELLSUBHEADERMESSAGE_PRECHECK");
                    }
                    else
                    {
                        TheProgramTemplateMessageDTO = TheProgramTemplateMessageDTOList
                            .SingleOrDefault(p => p.ProgramTemplateMessageCriteriaName.ToUpper() == (IsSuccess ? PROGRAMTEMPLATE_MESSAGECRITERIA.CROSSSELL_SUBMESSAGE_SUCCESS : PROGRAMTEMPLATE_MESSAGECRITERIA.CROSSSELL_SUBMESSAGE_FAILURE).ToString());
                        if (TheProgramTemplateMessageDTO != null)
                        {
                            TheCrossSellModel.CrossSellSubHeaderMessage = string.Format(TheProgramTemplateMessageDTO.ResourceMetaDataText, (TheCrossSellModel.CrossSellProgramResponse != null && TheCrossSellModel.CrossSellProgramResponse.IsPreCheckEnabled.HasValue && TheCrossSellModel.CrossSellProgramResponse.IsPreCheckEnabled.Value ? Math.Min(3, TheCrossSellModel.MaxCrossSellUserSelections) : TheCrossSellModel.MaxCrossSellUserSelections));
                        }
                    }

                    TheProgramTemplateMessageDTO = TheProgramTemplateMessageDTOList
                        .SingleOrDefault(p => p.ProgramTemplateMessageCriteriaName.ToUpper() == (IsSuccess ? PROGRAMTEMPLATE_MESSAGECRITERIA.THANKYOU_SUCCESS : PROGRAMTEMPLATE_MESSAGECRITERIA.THANKYOU_FAILURE).ToString());
                    if (TheProgramTemplateMessageDTO != null) CrossSellThankYouMessage = string.Format(TheProgramTemplateMessageDTO.ResourceMetaDataText, UserFirstName, InstitutionName);
                }
            }
            catch (Exception ex) { new ISException(EDDY.IS.Base.ISApplication.FormsEngine, new Exception(string.Format("TemplateManagerControllerBase.bindProgramTemplateMessage failed with the message - {0}", ex.Message)), Type, IsSuccess).Save(); }
        }

        private string getProgramTemplateMessages(ref CrossSellModel TheCrossSellModel
            , bool IsSuccess, RuleFailure LastRuleFailure, string UserFullName, string InstitutionName)
        {
            string CrossSellThankYouMessage = string.Empty;
            string UserFirstName = UserFullName.Split(' ')[0];//Displaying only the First Name

            //Default Success or Failure Messages
            string TypeName = TheCrossSellModel.MaxCrossSellUserSelections > 1 ? PROGRAMTEMPLATE_MESSAGETYPE.GENERIC_MULTIPLESCHOOL.ToString() : PROGRAMTEMPLATE_MESSAGETYPE.GENERIC_ONESCHOOL.ToString();
            this.bindProgramTemplateMessage(ref TheCrossSellModel, ref CrossSellThankYouMessage, TypeName, IsSuccess, UserFirstName, InstitutionName);

            //Failure Rule Based Messages
            if (LastRuleFailure != null)
            {
                try
                {
                    if (LastRuleFailure.RuleFailureType == BaseRuleType.Custom_KVLookup)
                    {
                        this.bindProgramTemplateMessage(ref TheCrossSellModel, ref CrossSellThankYouMessage, LastRuleFailure.RuleFailureName, IsSuccess, UserFirstName, InstitutionName);
                    }
                    else if (LastRuleFailure.RuleFailureType == BaseRuleType.Minimum_Education_Level)//Education Level based customized failure message  from ME
                    {
                        TheCrossSellModel.CrossSellSubHeaderMessage = TheCrossSellModel.CrossSellProgramResponse.InitialLeadInvalidMessage;
                    }
                    else
                    {
                        this.bindProgramTemplateMessage(ref TheCrossSellModel, ref CrossSellThankYouMessage, LastRuleFailure.RuleFailureType.ToString(), IsSuccess, UserFirstName, InstitutionName);
                    }
                }
                catch (Exception ex)
                {
                    new ISException(EDDY.IS.Base.ISApplication.FormsEngine, new Exception(string.Format("TemplateManagerControllerBase.getProgramTemplateMessages failed with the message - {0}", ex.Message)),"", LastRuleFailure).Save();
                }
            }

            return CrossSellThankYouMessage;
        }



        private CrossSellModel GetCrossSellModel(int TemplateId,
                                                bool IsBeta,
                                                Guid TrackId,
                                                ProspectInput ProspectInput,
                                                CampusType? CampusType,
                                                int InitialMatchProgramProductId,
                                                int InstitutionId,
                                                ProgramValidateResponse ProgramValidationResponse,
                                                bool WasLeadCreated,
                                                RuleFailure LastRuleFailure,
                                                string UserFullName,
                                                string InstitutionName,
                                                string ProgramName,
                                                List<string> MobilePhones,
                                                bool IsForOptimizelyCrossSell,
                                                string Theme,
                                                string FESessionId,
                                                Guid? TrackingDeviceGUID,
                                                Dictionary<string, string> LeadDataDictionary,
                                                int? ApplicationId,
                                                string AlternativeTemplates,
                                                bool ProgramWizard
                                        )
        {
            CrossSellModel crossSellModel = new CrossSellModel();
            crossSellModel.TemplateId = TemplateId;
            bool DoCrossSell = false;

            
            if (ProgramValidationResponse != null && ProgramValidationResponse.ProgramType.HasValue)
            {
                DoCrossSell = true;
            }

            if (IsForOptimizelyCrossSell)
            {
                crossSellModel.CrossSellProgramResponse = FormsEngineService.GetProgramsForOptimizelyCrossSell(4, 3);
            }
            else if (DoCrossSell)
            {
                crossSellModel.CrossSellProgramResponse = FormsEngineService.GetProgramsForCrossSell(TrackId, ProspectInput, CampusType, IsBeta, InitialMatchProgramProductId, InstitutionId, 10, TemplateId, TrackingDeviceGUID, FESessionId, LeadDataDictionary, ProgramWizard, ProgramValidationResponse.PassedValidation,ApplicationId);

                if (crossSellModel.CrossSellProgramResponse != null)
                {
                    crossSellModel.CrossSellProgramResponse.IsPreCheckEnabled = crossSellModel.CrossSellProgramResponse.IsPreCheckEnabled.HasValue ? crossSellModel.CrossSellProgramResponse.IsPreCheckEnabled : false;
                    crossSellModel.CrossSellProgramResponse.IsCrossSellALternateList = crossSellModel.CrossSellProgramResponse.IsCrossSellALternateList.HasValue ? crossSellModel.CrossSellProgramResponse.IsCrossSellALternateList : false;

                    var Templates = FormsEngineService.GetTemplateApplicationOverrideForApplication(ApplicationId.GetValueOrDefault());
                    if (Templates.Count > 0)
                    {
                        AssignApplicationOverrideTemplatesToPrograms(crossSellModel.CrossSellProgramResponse, AlternativeTemplates, ApplicationId.GetValueOrDefault());
                    }

                    //added to store the match response guid for the cross sell in session
                    Guid crossSellSelectionMatchGuid = default(Guid);
                    crossSellSelectionMatchGuid = crossSellModel.CrossSellProgramResponse.MatchResponseGuid;
                    FESession.Set(FESessionId, Constants.SUBMISSION_CROSSELL_MATCHRESPONSEGUID, crossSellSelectionMatchGuid);
                    crossSellModel.MaxProgramsToDisplayTotal = DetermineMaxCrossSellProgramsToDisplay(10, crossSellModel.CrossSellProgramResponse.MaxProgramsToDisplay);

                    // catch in case the MaxUserSelections did not come back from ME for the campaign
                    if (crossSellModel.CrossSellProgramResponse.MaxUserSelections == null)
                    {
                        crossSellModel.MaxCrossSellUserSelections = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MaxCrossSellUserSelections"));
                    }
                    else
                    {
                        crossSellModel.MaxCrossSellUserSelections = Convert.ToInt32(crossSellModel.CrossSellProgramResponse.MaxUserSelections);
                    }

                    // if the first match was valid, decrease the allowed selection limit by one (changed to decriment as requested by Kim & Erick 5/29/13)
                    if (ProgramValidationResponse.PassedValidation && WasLeadCreated)
                    {
                        crossSellModel.MaxCrossSellUserSelections--;
                    }

                    // user selection count must be the lesser of it and the available program count
                    crossSellModel.MaxCrossSellUserSelections = Math.Min(crossSellModel.MaxCrossSellUserSelections, crossSellModel.CrossSellProgramResponse.ProgramList.Count());
                    crossSellModel.MaxCrossSellUserSelectionsAlpha = Convert.ToDecimal(crossSellModel.MaxCrossSellUserSelections).ConverDecimalToLetter(false, false);
                }
            }
            else
            {
                crossSellModel.CrossSellProgramResponse = new CrossSellProgramResponse() { ProgramList = new ProgramWithInstitutionCampus[] { } };
            }

            crossSellModel.IsForOptimizelyCrossSell = IsForOptimizelyCrossSell;
            crossSellModel.MobilePhones = MobilePhones;
            crossSellModel.Theme = string.IsNullOrWhiteSpace(Theme) ? "default" : Theme;
            crossSellModel.InitialMatchWasValid = ProgramValidationResponse.PassedValidation;
            crossSellModel.WasLeadCreated = WasLeadCreated;
            crossSellModel.UserFullName = UserFullName;
            crossSellModel.UserSelectedInstitutionName = InstitutionName;
            crossSellModel.UserSelectedProgramName = ProgramName;

            return crossSellModel;
        }


        /// <summary>
        /// Gets only the rendered cross sell html and css path
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="RenderingStrategy"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackId"></param>
        /// <param name="ProspectInput"></param>
        /// <param name="CampusType"></param>
        /// <param name="InitialMatchProgramProductId"></param>
        /// <param name="InstitutionId"></param>
        /// <param name="InitialMatchWasValid"></param>
        /// <param name="WasLeadCreated"></param>
        /// <param name="LastRuleFailure"></param>
        /// <param name="UserFullName"></param>
        /// <param name="InstitutionName"></param>
        /// <param name="ProgramName"></param>
        /// <param name="MobilePhones"></param>
        /// <param name="IsForOptimizelyCrossSell"></param>
        /// <param name="Theme"></param>
        /// <param name="FESessionId"></param>
        /// <param name="TrackingDeviceGUID"></param>
        /// <param name="LeadDataDictionary"></param>
        /// <param name="ApplicationId"></param>
        /// <returns></returns>
        public CrossSellResultDTO GetRenderedCrossSell(
                                                int TemplateId,
                                                string RenderingStrategy,
                                                bool IsBeta,
                                                Guid TrackId,
                                                ProspectInput ProspectInput,
                                                CampusType? CampusType,
                                                int InitialMatchProgramProductId,
                                                int InstitutionId,
                                                ProgramValidateResponse ProgramValidationResponse,
                                                bool WasLeadCreated,
                                                RuleFailure LastRuleFailure,
                                                string UserFullName,
                                                string InstitutionName,
                                                string ProgramName,
                                                List<string> MobilePhones,
                                                bool IsForOptimizelyCrossSell,
                                                string Theme,
                                                string FESessionId,
                                                Guid? TrackingDeviceGUID,
                                                Dictionary<string, string> LeadDataDictionary,
                                                int? ApplicationId,
                                                string AlternativeTemplates
                                        )
        {

            CrossSellResultDTO result = new CrossSellResultDTO();
            result.Success = false;

            try
            {
                CrossSellModel crossSellModel = GetCrossSellModel(TemplateId, IsBeta, TrackId, ProspectInput, CampusType, InitialMatchProgramProductId, InstitutionId, ProgramValidationResponse, WasLeadCreated, LastRuleFailure, UserFullName, InstitutionName, ProgramName, MobilePhones, IsForOptimizelyCrossSell, Theme, FESessionId, TrackingDeviceGUID, LeadDataDictionary, ApplicationId, AlternativeTemplates, false);
                HTMLRenderingStrategyDTO RS = FormsEngineService.GetHTMLRenderingStrategy(RenderingStrategy);

                //Get the appropriate message based on the validation result
                result.CrossSellThankYouMessage = this.getProgramTemplateMessages(ref crossSellModel, (ProgramValidationResponse.PassedValidation && WasLeadCreated), LastRuleFailure, UserFullName, InstitutionName);

                bool HasPrograms = crossSellModel.CrossSellProgramResponse != null && crossSellModel.CrossSellProgramResponse.ProgramList != null && crossSellModel.CrossSellProgramResponse.ProgramList.Count() > 0;

                // only go through process of getting rendered cross sell if there are programs to display and allowed selection count > 0
                if (HasPrograms && crossSellModel.MaxProgramsToDisplayTotal > 0 && crossSellModel.MaxCrossSellUserSelections > 0)
                {
                    if (RS != null)
                    {
                        result.RenderedCrossSell = this.RazorViewToString(RS.CrossSellTemplateView, crossSellModel, true);
                    }
                    result.MaxCrossSellUserSelections = crossSellModel.MaxCrossSellUserSelections;

                    if (result.RenderedCrossSell != null && result.RenderedCrossSell.Length > 0)
                    {
                        result.Success = true;
                        FESession.Set(FESessionId, Constants.PROGRAMTEMPLATE_CROSSSELLPROGRAMS_KEY, crossSellModel.CrossSellProgramResponse.ProgramList.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return result;
        }

        private static int DetermineMaxCrossSellProgramsToDisplay(int? CrossSellMaxProgramsToDisplay, int? MEMaxProgramsToDisplay)
        {
            int r;
            // determine the max programs to display
            if (CrossSellMaxProgramsToDisplay != null && MEMaxProgramsToDisplay != null)
            {
                r = Math.Min(Convert.ToInt32(CrossSellMaxProgramsToDisplay), Convert.ToInt32(MEMaxProgramsToDisplay));
            }
            else if (CrossSellMaxProgramsToDisplay != null && MEMaxProgramsToDisplay == null)
            {
                r = Convert.ToInt32(CrossSellMaxProgramsToDisplay);
            }
            else if (CrossSellMaxProgramsToDisplay == null && MEMaxProgramsToDisplay != null)
            {
                r = Convert.ToInt32(MEMaxProgramsToDisplay);
            }
            else // both RS and ME have null values for MaxProgramsToDisplay
            {
                r = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MaxCrossSellProgramsToDisplay"));
            }
            return r;
        }

        /// <summary>
        /// Program templates submit
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="ProgramProductId"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackId"></param>
        /// <param name="SessionId"></param>
        /// <param name="ProspectId"></param>
        /// <param name="RenderingStrategy"></param>
        /// <param name="InstitutionId"></param>
        /// <param name="InstitutionName"></param>
        /// <param name="ProgramName"></param>
        /// <param name="MatchGuid"></param>
        /// <param name="LeadData"></param>
        /// <param name="AdditionalData"></param>
        /// <param name="RawData"></param>
        /// <param name="Log"></param>
        /// <param name="FESessionId"></param>
        /// <param name="DeviceId"></param>
        /// <returns></returns>
        public SubmissionResultDTO ProcessSubmit(int TemplateId, int ProgramProductId, int ProductId, bool IsBeta, string TrackId, string SessionId, int? ProspectId, string RenderingStrategy, int InstitutionId, string InstitutionName, string ProgramName, string MatchGuid, string LeadData, string AdditionalData, ref RawPostDataDTO RawData, ref PerformanceLog Log, string FESessionId, string DeviceId, int? ApplicationId, string AlternativeTemplates)
        {
            SubmissionResultDTO submissionResultDTO = new SubmissionResultDTO();
            LeadSaveManger leadSaveManager = new LeadSaveManger(Log);

            //0. create LeadCreateRequest object
            #region Create LeadCreateRequest Object
            Guid? TrackingDeviceGUID = null;

            if (!String.IsNullOrWhiteSpace(DeviceId))
            {
                Guid tempDeviceGuid;
                if (Guid.TryParse(DeviceId, out tempDeviceGuid))
                {
                    TrackingDeviceGUID = tempDeviceGuid;
                }
            }

            Log.StartLogDetail("ProcessSubmit.BuildLeadCreateRequestObject");
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(TemplateId, ProgramProductId, IsBeta, TrackId, "", false, SessionId, MatchGuid, LeadData, AdditionalData, null, LeadCreationType.InstitutionFormInitial, ProspectId);
            Log.EndLogDetail();
            #endregion

            //1. Matching Engine validate Business rules
            #region Form Engine validate Business rules
            ProgramValidateRequest ProgramValidationRequest = new ProgramValidateRequest();
            ProgramValidationRequest.Application = MatchingEngine.ISApplication.FormsEngine;
            ProgramValidationRequest.TrackGuid = LeadRequest.TrackId.GetValueOrDefault();
            ProgramValidationRequest.ProgramProductId = ProgramProductId;
            ProgramValidationRequest.BreakOnFirstValidationFailure = true;
            ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
            ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
            ProgramValidationRequest.ProspectInput = Prospect;

            Log.StartLogDetail("ProcessSubmit.LeadScore");
            //if (LeadRequest.LeadData != null && LeadRequest.LeadData.Count > 0)
            //{
            //    LeadScoringService.ScoringRequest scoreRequest = FormsEngineService.RelatedServices.BuildLeadScoreRequest(LeadRequest.TrackId.GetValueOrDefault(), LeadRequest.LeadData, new List<int>(), new List<int>(), null, Prospect);
            //    var LeadScore = FormsEngineService.RelatedServices.GetLeadScoringInput(scoreRequest, FESessionId);
            //    if (LeadScore != null)
            //    {
            //        ProgramValidationRequest.LeadScoringTierLevel = LeadScore.LeadScoringTierLevel;
            //    }
            //}
            Log.EndLogDetail();

            Log.StartLogDetail("ProcessSubmit.MatchingEngineValidation");
            ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.InstitutionFormInitial;
            ProgramValidateResponse ProgramValidationResponse = FormsEngineService.RelatedServices.ValidateProgram(ProgramValidationRequest, IsBeta);
            bool skipSS = ProgramValidationResponse.SkipSchoolSelection;
            Log.EndLogDetail();
            submissionResultDTO.InitialMatchWasValid = ProgramValidationResponse.PassedValidation;
            if (ProgramValidationResponse.AlternateProgramProductId.HasValue)
            {
                LeadRequest.ProgramProductId = ProgramValidationResponse.AlternateProgramProductId.GetValueOrDefault();
            }

            if (ProgramValidationResponse.PaidStatusTypeId.HasValue)
            {
                LeadRequest.PaidStatusType = Convert.ToInt32(ProgramValidationResponse.PaidStatusTypeId.GetValueOrDefault());
            }

            RuleFailure LastRuleFailure = null;
            if (!ProgramValidationResponse.PassedValidation)
            {
                foreach (var Failure in ProgramValidationResponse.RuleFailures)
                {
                    LastRuleFailure = Failure;
                    if (Failure.RuleFailureType == BaseRuleType.ExternalDuplicate)
                    {
                        LeadRequest.IsExternalDuplicate = true;
                    }
                    else if (Failure.RuleFailureType == BaseRuleType.InternalDuplicate)
                    {
                        LeadRequest.IsInternalDuplicate = true;
                    }
                }
            }

            //1.5 Quick Check server side validation
            APIValidationResultDTO QuickCheckValidationResult = FormsEngineService.QuickCheckValidateForm(TemplateId, IsBeta, LeadRequest.TrackId.GetValueOrDefault(), LeadRequest.LeadData, ref Log);
            LeadRequest.PassedValidation = ProgramValidationResponse.PassedValidation && QuickCheckValidationResult.Valid;

            //Add Additional fields based on ME response
            if (!LeadRequest.PassedValidation)
            {
                if (!LeadRequest.LeadData.ContainsKey("InitialLeadValidationFailedReason"))
                {
                    string ruleFailureType = (LastRuleFailure != null) ? LastRuleFailure.RuleFailureType.ToString() : "ME unknown";
                    string ruleFailureName = (LastRuleFailure != null) ? LastRuleFailure.RuleFailureName : "ME unknown rule";
                    LeadRequest.LeadData.Add("InitialLeadValidationFailedReason", ruleFailureType + " - " + ruleFailureName);
                }
            }
            if (!LeadRequest.LeadData.ContainsKey("InitialLeadValidationFailed"))
            {
                LeadRequest.LeadData.Add("InitialLeadValidationFailed", (LeadRequest.PassedValidation) ? "0" : "1");
            }
            #endregion

            //1.B Sync/Async Prospect Service call 
            #region Prospect Service
            Log.StartLogDetail("ProcessSubmit.ProspectServiceCall - START");
            Log.StartLogDetail("ProcessSubmit.ProspectServiceCall - ProspectId before call = " + ProspectId);

            var prospectFlowType = ApplicationId == _emsApplicationId ? ProspectFlowTypes.EMS : ProspectFlowTypes.Prospecting;

            Log.StartLogDetail("ProcessSubmit.ProspectServiceCall - Site Setting to save Prospect=True");
            if (ProspectId == null || ProspectId < 1)
            {
                SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.GetValueOrDefault(), prospectFlowType);
                Log.StartLogDetail("ProcessSubmit.SyncProspectServiceCall");
                var ProspectResult = FormsEngineService.RelatedServices.SaveProspect(WebServiceProspect);
                ProspectId = ProspectResult.ProspectId;
                Log.StartLogDetail("ProcessSubmit.SyncProspectServiceCall - ProspectId after call (overwritten) = " + ProspectId);
                LeadRequest.ProspectId = ProspectId;
            }
            else
            {
                SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.GetValueOrDefault(), prospectFlowType);
                Log.StartLogDetail("ProcessSubmit.ASyncProspectServiceCall");
                FormsEngineService.RelatedServices.SaveProspectAsync(WebServiceProspect);
                Log.StartLogDetail("ProcessSubmit.ASyncProspectServiceCall - ProspectId after call (not overwritten) = " + ProspectId);
            }

            Log.StartLogDetail("ProcessSubmit.ProspectServiceCall - END");
            Log.EndLogDetail();
            submissionResultDTO.ProspectId = ProspectId;
            #endregion

            //3. Express Consent Mobile Check call
            #region Express Consent
            string Phone1 = "", Phone2 = "";
            if (LeadRequest.LeadData.ContainsKey("phone"))
            {
                Phone1 = LeadRequest.LeadData["phone"];
            }
            if (LeadRequest.LeadData.ContainsKey("alternate_phone"))
            {
                Phone2 = LeadRequest.LeadData["alternate_phone"];
            }

            ExpressConsentCheckDTO ExpressConsentCheck = CheckForMobile(Phone1, Phone2, Log);
            FESession.Set(FESessionId, Constants.PROGRAMTEMPLATE_MOBILENUMBERS, ExpressConsentCheck.MobilePhones);

            if (ExpressConsentCheck.MobilePhones.Count > 0)
            {
                LeadRequest.LeadData.Add("HasMobilePhone", "1");
            }
            else
            {
                LeadRequest.LeadData.Add("HasMobilePhone", "0");
            }

            #endregion

            //4 Save Lead
            #region Save Submission and Lead
            int ProspectFlowId = FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID) == null ? 0 : (int)FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID);
            Log.StartLogDetail("ProcessSubmit.CreateLead");
            LeadCreateResponse leadCreateResponse = leadSaveManager.Execute(TemplateId, LeadRequest, FESessionId, null, RawData, ProspectId, null, MatchResponseType.Directory, false, QuickCheckValidationResult, ProspectFlowId);
            Log.EndLogDetail();

            //4.1 set PT Lead into Session for ThankYou to retrieve
            List<LeadSaveData> leadSaveList = new List<LeadSaveData>();
            if (leadCreateResponse.Success && leadCreateResponse.Lead.LeadId > 0)
            {
                leadSaveList.Add(new LeadSaveData() { ProgramProductId = Convert.ToInt32(leadCreateResponse.Lead.ProgramProductId), TemplateId = TemplateId });
            }
            FESession.Set(FESessionId, Constants.PROGRAMTEMPLATE_INITIALLEAD_KEY, leadSaveList);

            leadCreateResponse = leadCreateResponse == null ? new LeadCreateResponse() { Lead = new LeadDTO() } : leadCreateResponse;
            submissionResultDTO.WasLeadCreated = leadCreateResponse.Success;
            submissionResultDTO.InitialLeadId = leadCreateResponse.Lead.LeadId;
            submissionResultDTO.UID = leadCreateResponse.Lead.UID;
            submissionResultDTO.RawPostDataId = leadCreateResponse.Lead.RawPostDataId;
            submissionResultDTO.IsTestLead = leadCreateResponse.IsTestLead;
            #endregion

            //5. Sync Pixel Service call
            #region Pixel Service
            Log.StartLogDetail("ProcessSubmit.GetPixels");
            int[] leadIds = new int[1];
            leadIds[0] = Convert.ToInt32(leadCreateResponse.Lead.LeadId);
            List<string> pixelTypes = new List<string>();
            pixelTypes.Add("Initial Conversion Only"); // is mispelled in db but Pixel Service wants correct spelling
            pixelTypes.Add("Conversion");
            pixelTypes.Add("Cumulative Conversion");
            pixelTypes.Add("Client Relation Pixel Conversion");
            var Pixels = GetPixels(IsBeta, leadCreateResponse.Lead.LastName, leadCreateResponse.Lead.EmailAddress, TrackId, ProspectId, ProgramValidationResponse.PassedValidation, leadIds, pixelTypes, false);    //IsBeta, leadCreateResponse.Lead, leadCreateResponse.Success, ProgramValidationResponse.PassedValidation);
            // set PT Pixels into Session
            FESession.Set(FESessionId, Constants.PROGRAMTEMPLATE_INITIALEADPIXELS_KEY, Pixels.PixelsWithDebugInfo);
            Log.EndLogDetail();
            #endregion

            //6. Cross Sell
            #region Cross Sell
            if (QuickCheckValidationResult.Valid) //if we failed server side validation no cross sell
            {
                CampusType? CampusPreference = null;
                string userFullName = leadCreateResponse.Lead.FirstName + " " + leadCreateResponse.Lead.LastName;
                string Theme = LeadRequest.LeadAdditionalData.ContainsKey("Theme") ? LeadRequest.LeadAdditionalData["Theme"] : "default";


                if (!skipSS)
                {
                    Log.StartLogDetail("ProcessSubmit.GetRenderedCrossSell");
                    CrossSellResultDTO crossSellResultDTO = GetRenderedCrossSell(TemplateId, RenderingStrategy, IsBeta, LeadRequest.TrackId.GetValueOrDefault(), Prospect, CampusPreference,
                        ProgramProductId, InstitutionId, ProgramValidationResponse, leadCreateResponse.Success
                        , LastRuleFailure//#54902 - Directory Cross-Sell Layer - Need code for Optimizely to test copy
                        , userFullName, InstitutionName, ProgramName, ExpressConsentCheck.MobilePhones, false, Theme, FESessionId, TrackingDeviceGUID, LeadRequest.LeadData, ApplicationId, AlternativeTemplates);
                    Log.EndLogDetail();
                    submissionResultDTO.CrossSellResult = crossSellResultDTO;
                }

                //7. User selection for ThankYou page
                if (LeadRequest.PassedValidation)
                {
                    AddUserSelectionToSession(InstitutionId, InstitutionName, ProgramName, ProgramProductId, false, FESessionId);
                }
            }

            #endregion

            //adding code to insert submissionmatchresponse record for cross sell
            Guid? crossSellMatchResponseGuid = FESession.Get<Guid?>(FESessionId, Constants.SUBMISSION_CROSSELL_MATCHRESPONSEGUID);

            int submissionId = 0;
            if (FESession.Get(FESessionId, Constants.SUBMISSION_ID_SESSION_KEY) != null)
            {
                submissionId = FESession.Get<int>(FESessionId, Constants.SUBMISSION_ID_SESSION_KEY);
            }

            //save the submission match response record if we have a submissionid in session and a cross sell match guid
            if (crossSellMatchResponseGuid.HasValue && submissionId != 0)
            {
                leadSaveManager.SaveCrossSellSubmissionMatchResponse(crossSellMatchResponseGuid.GetValueOrDefault(), FESessionId, submissionId);
            }


            return submissionResultDTO;
        }



        /// <summary>
        /// Program templates submit
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="ProgramProductId"></param>
        /// <param name="IsBeta"></param>
        /// <param name="TrackId"></param>
        /// <param name="SessionId"></param>
        /// <param name="ProspectId"></param>
        /// <param name="RenderingStrategy"></param>
        /// <param name="InstitutionId"></param>
        /// <param name="InstitutionName"></param>
        /// <param name="ProgramName"></param>
        /// <param name="MatchGuid"></param>
        /// <param name="LeadData"></param>
        /// <param name="AdditionalData"></param>
        /// <param name="RawData"></param>
        /// <param name="Log"></param>
        /// <param name="FESessionId"></param>
        /// <param name="DeviceId"></param>
        /// <returns></returns>
        public ManagedChoiceResultDTO SubmitProgramWizard(int TemplateId,
            int ProgramProductId,
            int ProductId,
            bool IsBeta,
            string TrackId,
            string SessionId,
            int? ProspectId,
            string RenderingStrategy,
            int InstitutionId,
            string MatchGuid,
            string LeadData,
            string AdditionalData,
            PerformanceLog Log,
            string FESessionId,
            string DeviceId,
            int? ApplicationId,
            string InstitutionName,
            string ProgramName)
        {
            ManagedChoiceResultDTO Result = new ManagedChoiceResultDTO();
            SubmissionResultDTO submissionResultDTO = new SubmissionResultDTO();
            LeadSaveManger leadSaveManager = new LeadSaveManger(Log);
            RawPostDataDTO RawData = BuildRawDataObject(LeadData);
            bool hasEMSProductId = ProductId == Constants.EMS_PRODUCTID || ProductId == Constants.EMS_PPI_PRODUCTID;


            //0. create LeadCreateRequest object
            #region Create LeadCreateRequest Object
            Guid? TrackingDeviceGUID = null;

            if (!String.IsNullOrWhiteSpace(DeviceId))
            {
                Guid tempDeviceGuid;
                if (Guid.TryParse(DeviceId, out tempDeviceGuid))
                {
                    TrackingDeviceGUID = tempDeviceGuid;
                }
            }

            Log.StartLogDetail("SubmitProgramWizard.BuildLeadCreateRequestObject");
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(TemplateId, ProgramProductId, IsBeta, TrackId, "", false, SessionId, MatchGuid, LeadData, AdditionalData, null, LeadCreationType.ProgramWizardInitial, ProspectId);
            //EMS Applciation will append Form name to additionalData
            if (ApplicationId.HasValue && ApplicationId == Constants.EMS_APPLICATIONID)
            {
                LeadRequest.LeadData.Add("FormName", FormsEngineService.GetShallowTemplate(TemplateId).TemplateName);
            }
            Log.EndLogDetail();
            #endregion

            //1. Matching Engine validate Business rules
            #region Form Engine validate Business rules
            ProgramValidateRequest ProgramValidationRequest = new ProgramValidateRequest();
            ProgramValidationRequest.Application = MatchingEngine.ISApplication.FormsEngine;
            ProgramValidationRequest.TrackGuid = LeadRequest.TrackId.GetValueOrDefault();
            ProgramValidationRequest.ProgramProductId = ProgramProductId;
            ProgramValidationRequest.BreakOnFirstValidationFailure = true;
            ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
            ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
            ProgramValidationRequest.ProspectInput = Prospect;


            Log.StartLogDetail("SubmitProgramWizard.MatchingEngineValidation");
            ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.ProgramWizardInitial;
            ProgramValidateResponse ProgramValidationResponse = FormsEngineService.RelatedServices.ValidateProgram(ProgramValidationRequest, IsBeta);

            bool skipSS = ProgramValidationResponse.SkipSchoolSelection;
            FESession.Set(FESessionId, Constants.ME_SKIPSCHOOLSELECTIONFORMATCHONE, ProgramValidationResponse.SkipSchoolSelection);

            Log.EndLogDetail();
            submissionResultDTO.InitialMatchWasValid = ProgramValidationResponse.PassedValidation;
            if (ProgramValidationResponse.AlternateProgramProductId.HasValue)
            {
                LeadRequest.ProgramProductId = ProgramValidationResponse.AlternateProgramProductId.GetValueOrDefault();
            }

            if (ProgramValidationResponse.PaidStatusTypeId.HasValue)
            {
                LeadRequest.PaidStatusType = Convert.ToInt32(ProgramValidationResponse.PaidStatusTypeId.GetValueOrDefault());
            }

            RuleFailure LastRuleFailure = null;
            if (!ProgramValidationResponse.PassedValidation)
            {
                foreach (var Failure in ProgramValidationResponse.RuleFailures)
                {
                    LastRuleFailure = Failure;
                    if (Failure.RuleFailureType == BaseRuleType.ExternalDuplicate)
                    {
                        LeadRequest.IsExternalDuplicate = true;
                    }
                    else if (Failure.RuleFailureType == BaseRuleType.InternalDuplicate)
                    {
                        LeadRequest.IsInternalDuplicate = true;
                    }
                }
            }

            //1.5 Quick Check server side validation
            APIValidationResultDTO QuickCheckValidationResult = FormsEngineService.QuickCheckValidateForm(TemplateId, IsBeta, LeadRequest.TrackId.GetValueOrDefault(), LeadRequest.LeadData, ref Log);
            LeadRequest.PassedValidation = ProgramValidationResponse.PassedValidation && QuickCheckValidationResult.Valid;

            //Add Additional fields based on ME response
            if (!LeadRequest.PassedValidation)
            {
                if (!LeadRequest.LeadData.ContainsKey("InitialLeadValidationFailedReason"))
                {
                    string ruleFailureType = (LastRuleFailure != null) ? LastRuleFailure.RuleFailureType.ToString() : "ME unknown";
                    string ruleFailureName = (LastRuleFailure != null) ? LastRuleFailure.RuleFailureName : "ME unknown rule";
                    LeadRequest.LeadData.Add("InitialLeadValidationFailedReason", ruleFailureType + " - " + ruleFailureName);
                }
            }
            if (!LeadRequest.LeadData.ContainsKey("InitialLeadValidationFailed"))
            {
                LeadRequest.LeadData.Add("InitialLeadValidationFailed", (LeadRequest.PassedValidation) ? "0" : "1");
            }
            #endregion

            //1.B Sync/Async Prospect Service call 
            #region Prospect Service
            Log.StartLogDetail("SubmitProgramWizard.ProspectServiceCall - START");
            Log.StartLogDetail("SubmitProgramWizard.ProspectServiceCall - ProspectId before call = " + ProspectId);

            Log.StartLogDetail("SubmitProgramWizard.ProspectServiceCall - Site Setting to save Prospect=True");

            var prospectFlowType = ApplicationId == _emsApplicationId ? ProspectFlowTypes.EMS : ProspectFlowTypes.Prospecting;

            if (ProspectId == null || ProspectId < 1)
            {
                SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.GetValueOrDefault(), prospectFlowType);
                Log.StartLogDetail("SubmitProgramWizard.SyncProspectServiceCall");
                var ProspectResult = FormsEngineService.RelatedServices.SaveProspect(WebServiceProspect);
                ProspectId = ProspectResult.ProspectId;
                Log.StartLogDetail("SubmitProgramWizard.SyncProspectServiceCall - ProspectId after call (overwritten) = " + ProspectId);
                LeadRequest.ProspectId = ProspectId;
            }
            else
            {
                SaveProspectRequest WebServiceProspect = EntityBuildHelper.BuildSaveProspectRequest(LeadRequest, LeadRequest.TrackId.GetValueOrDefault(), prospectFlowType);
                Log.StartLogDetail("SubmitProgramWizard.ASyncProspectServiceCall");
                FormsEngineService.RelatedServices.SaveProspectAsync(WebServiceProspect);
                Log.StartLogDetail("SubmitProgramWizard.ASyncProspectServiceCall - ProspectId after call (not overwritten) = " + ProspectId);
            }

            Log.StartLogDetail("SubmitProgramWizard.ProspectServiceCall - END");
            Log.EndLogDetail();
            submissionResultDTO.ProspectId = ProspectId;
            #endregion

            //3. Express Consent Mobile Check call
            #region Express Consent
            string Phone1 = "", Phone2 = "";
            if (LeadRequest.LeadData.ContainsKey("phone"))
            {
                Phone1 = LeadRequest.LeadData["phone"];
            }
            if (LeadRequest.LeadData.ContainsKey("alternate_phone"))
            {
                Phone2 = LeadRequest.LeadData["alternate_phone"];
            }

            ExpressConsentCheckDTO ExpressConsentCheck = CheckForMobile(Phone1, Phone2, Log);
            FESession.Set(FESessionId, Constants.PROGRAMTEMPLATE_MOBILENUMBERS, ExpressConsentCheck.MobilePhones);

            if (ExpressConsentCheck.MobilePhones.Count > 0)
            {
                LeadRequest.LeadData.Add("HasMobilePhone", "1");
            }
            else
            {
                LeadRequest.LeadData.Add("HasMobilePhone", "0");
            }


            #endregion

            //4 Save Lead
            #region Save Submission and Lead
            int ProspectFlowId = FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID) == null ? 0 : (int)FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID);
            Log.StartLogDetail("SubmitProgramWizard.CreateLead");
            LeadCreateResponse leadCreateResponse = leadSaveManager.Execute(TemplateId, LeadRequest, FESessionId, null, RawData, ProspectId, null, MatchResponseType.Directory, false, QuickCheckValidationResult, ProspectFlowId);
            Log.EndLogDetail();

            string userFullName = leadCreateResponse.Lead.FirstName + " " + leadCreateResponse.Lead.LastName;

            bool ProgramWizardInitialLeadValid = leadCreateResponse.Success && leadCreateResponse.Lead.LeadId > 0 && LeadRequest.PassedValidation;

            //4.1 set PT Lead into Session for ThankYou to retrieve
            List<LeadSaveData> leadSaveList = new List<LeadSaveData>();
            if (ProgramWizardInitialLeadValid)
            {
                LeadSaveData lsd = new LeadSaveData();
                lsd.IsValid = true;
                lsd.LeadId = lsd.LeadId;
                lsd.ProgramProductId = leadCreateResponse.Lead.ProgramProductId.Value;
                lsd.TemplateId = TemplateId;
                leadSaveList.Add(lsd);
                FESession.Set(FESessionId, Constants.PROGRAMTEMPLATE_INITIALLEAD_KEY, leadSaveList);
                Result.ProgramWizardInitialValidLeadId = Convert.ToInt32(leadCreateResponse.Lead.LeadId);

                //Program With Institution for Thank you page
                ProgramWithInstitutionCampus InitialCampus = new ProgramWithInstitutionCampus();
                InitialCampus.CampusId = ProgramValidationResponse.CampusId.Value;
                InitialCampus.CampusName = ProgramValidationResponse.InstitutionName; //TBD
                InitialCampus.CampusLogoURL = ProgramValidationResponse.CampusLogoURL;
                InitialCampus.InstitutionDescription = ProgramValidationResponse.InstitutionDescription;
                InitialCampus.InstitutionId = ProgramValidationResponse.InstitutionId.Value;
                InitialCampus.InstitutionName = ProgramValidationResponse.InstitutionName;
                InitialCampus.CampusType = ProgramValidationResponse.ProgramCampusType.Value;
                InitialCampus.ProgramDescription = ProgramValidationResponse.ProgramDescription;
                InitialCampus.ProductId = ProductId;
                InitialCampus.ProgramId = ProgramValidationResponse.ProgramId.Value;
                InitialCampus.ProgramName = ProgramValidationResponse.ProgramName;
                InitialCampus.ProgramProductId = ProgramProductId;
                InitialCampus.TemplateId = TemplateId;
                InitialCampus.CampusLogoURL = ProgramValidationResponse.CampusLogoURL;
                InitialCampus.InstitutionLogoURL = ProgramValidationResponse.InstitutionLogoURL;

                FESession.Set(FESessionId, Constants.PROGRAMWIZARD_INITIALCAMPUS_KEY, InitialCampus);
            }

            leadCreateResponse = leadCreateResponse == null ? new LeadCreateResponse() { Lead = new LeadDTO() } : leadCreateResponse;
            submissionResultDTO.WasLeadCreated = leadCreateResponse.Success;
            submissionResultDTO.InitialLeadId = leadCreateResponse.Lead.LeadId;
            submissionResultDTO.UID = leadCreateResponse.Lead.UID;
            submissionResultDTO.RawPostDataId = leadCreateResponse.Lead.RawPostDataId;
            submissionResultDTO.IsTestLead = leadCreateResponse.IsTestLead;
            #endregion

            //5. Sync Pixel Service call
            #region Pixel Service
            Log.StartLogDetail("SubmitProgramWizard.GetPixels");
            int[] leadIds = new int[1];
            leadIds[0] = Convert.ToInt32(leadCreateResponse.Lead.LeadId);
            List<string> pixelTypes = new List<string>();
            pixelTypes.Add("Initial Conversion Only"); // is mispelled in db but Pixel Service wants correct spelling
            pixelTypes.Add("Conversion");
            pixelTypes.Add("Cumulative Conversion");
            pixelTypes.Add("Client Relation Pixel Conversion");
            var Pixels = GetPixels(IsBeta, leadCreateResponse.Lead.LastName, leadCreateResponse.Lead.EmailAddress, TrackId, ProspectId, ProgramValidationResponse.PassedValidation, leadIds, pixelTypes, false);    //IsBeta, leadCreateResponse.Lead, leadCreateResponse.Success, ProgramValidationResponse.PassedValidation);
            // set PT Pixels into Session Use Smart matches pixel section.
            FESession.Set(FESessionId, Constants.WIZARD_SMARTMATCHPIXELS_KEY, Pixels.PixelsWithDebugInfo);
            Log.EndLogDetail();
            #endregion

            //6. Cross Sell
            #region Cross Sell
            if (!QuickCheckValidationResult.Valid)
            {
                //Failed server side validation no cross sell
                Result.MoveToStart = true;
            }
            else if (skipSS)
            {
                Result.SMLeadsCreatedCount = 1;
                Result.SmartMatchProgramCount = 1;
                Result.MoveToNoMatch = false;
                Result.MoveToThankYou = true;
            }
            else if (hasEMSProductId)
            {
                Result.SMLeadsCreatedCount = ProgramWizardInitialLeadValid ? 1 : 0;
                Result.SmartMatchProgramCount = ProgramWizardInitialLeadValid ? 1 : 0;
                Result.MoveToNoMatch = ProgramWizardInitialLeadValid ? false : true;
                Result.MoveToThankYou = ProgramWizardInitialLeadValid ? true : false;
                
                Result.UserFullName = !string.IsNullOrWhiteSpace(userFullName) ? userFullName : Result.UserFullName;
            }
            else
            {
                CampusType? CampusPreference = null;
                string Theme = LeadRequest.LeadAdditionalData.ContainsKey("Theme") ? LeadRequest.LeadAdditionalData["Theme"] : "default";
                Log.StartLogDetail("SubmitProgramWizard.GetCrossSellModel");

                CrossSellModel CrossSell = GetCrossSellModel(TemplateId, IsBeta, LeadRequest.TrackId.GetValueOrDefault(), Prospect, CampusPreference,
                    ProgramProductId, InstitutionId, ProgramValidationResponse, leadCreateResponse.Success
                    , LastRuleFailure
                    , userFullName, InstitutionName, ProgramName, ExpressConsentCheck.MobilePhones, false, Theme, FESessionId, TrackingDeviceGUID, LeadRequest.LeadData, ApplicationId, string.Empty, true);

                Log.EndLogDetail();

                //Get the appropriate message based on the validation result
                int InternalMessageMappingIssue = CrossSell.MaxCrossSellUserSelections;
                CrossSell.MaxCrossSellUserSelections = 1;
                string ProgramWizardMessage = this.getProgramTemplateMessages(ref CrossSell, (leadCreateResponse.Success && LeadRequest.PassedValidation), LastRuleFailure, userFullName, InstitutionName);
                CrossSell.MaxCrossSellUserSelections = InternalMessageMappingIssue;


                //adding code to insert submissionmatchresponse record for cross sell
                Guid? crossSellMatchResponseGuid = FESession.Get<Guid?>(FESessionId, Constants.SUBMISSION_CROSSELL_MATCHRESPONSEGUID);

                int submissionId = 0;
                if (FESession.Get(FESessionId, Constants.SUBMISSION_ID_SESSION_KEY) != null)
                {
                    submissionId = FESession.Get<int>(FESessionId, Constants.SUBMISSION_ID_SESSION_KEY);
                }

                //save the submission match response record if we have a submissionid in session and a cross sell match guid
                if (crossSellMatchResponseGuid.HasValue && submissionId != 0)
                {
                    leadSaveManager.SaveCrossSellSubmissionMatchResponse(crossSellMatchResponseGuid.GetValueOrDefault(), FESessionId, submissionId);
                }

                WizardMatchResponse CrossSellMatches = Transform(CrossSell.CrossSellProgramResponse);
                CrossSell.CrossSellProgramResponse.IsPreCheckEnabled = CrossSell.CrossSellProgramResponse.IsPreCheckEnabled.HasValue ? CrossSell.CrossSellProgramResponse.IsPreCheckEnabled.Value : false;
                Result = GetRenderedManagedChoice(LeadRequest, LeadData, AdditionalData, RenderingStrategy, RawData, ExpressConsentCheck, Prospect, null, false, userFullName, Theme, 0, 0, FESessionId, null, TrackingDeviceGUID, true, CrossSellMatches, ProgramWizardMessage, ProgramWizardInitialLeadValid, CrossSell.CrossSellProgramResponse.IsPreCheckEnabled.Value, ApplicationId, Log);
                Result.ProgramWizardInitialProgramValid = ProgramWizardInitialLeadValid;

                if (ProgramWizardInitialLeadValid)
                {
                    Result.SMLeadsCreatedCount = 1;
                    Result.SmartMatchProgramCount = 1;
                }

                //No-match on program templates doesn't make sense
                if (Result.MoveToNoMatch)
                {
                    Result.MoveToNoMatch = false;
                    Result.MoveToThankYou = true;
                }
            }
            #endregion

            Result.IsProgramWizardResult = true;
            if (LeadRequest.PaidStatusType.HasValue)
            {
                Result.ProgramWizardInitialProgramPaidStatusTypeId = LeadRequest.PaidStatusType.Value;
            }

            return Result;
        }


        private WizardMatchResponse Transform(CrossSellProgramResponse Request)
        {
            WizardMatchResponse Response = new WizardMatchResponse();

            Response.MatchResponseGuid = Request.MatchResponseGuid;
            Response.MaxUserSelections = Request.MaxUserSelections;
            Response.ResultCount = Request.ResultCount;
            List<CampusWithInstitution> WizardMatchResponse = new List<CampusWithInstitution>();

            foreach (var ProgramCampusMatch in Request.ProgramList)
            {
                WizardMatchResponse.Add(Transform(ProgramCampusMatch));
            }
            Response.SchoolSelectionList = WizardMatchResponse.ToArray();
            Response.SmartMatchList = new ProgramWithInstitutionCampus[] { };
            Response.ThirdPartyMatchList = new ProgramWithInstitutionCampus[] { };

            return Response;
        }


        private CampusWithInstitution Transform(ProgramWithInstitutionCampus Request)
        {
            CampusWithInstitution Result = new CampusWithInstitution();

            Result.Address1 = Request.CampusAddress1;
            Result.Address2 = Request.CampusAddress2;
            Result.CampusId = Request.CampusId;
            Result.CampusName = Request.CampusName;
            Result.CampusType = Request.CampusType;
            Result.City = Request.CampusCity;
            Result.CountryCode = Request.CampusCountryCode;
            Result.CountryName = Request.CampusCountryName;
            //Result.DistanceFromProspect = Request.;
            Result.Fax = Request.CampusFax;
            Result.CampusLogoURL = Request.CampusLogoURL;
            Result.InstitutionLogoURL = Request.InstitutionLogoURL;
            Result.InstitutionDescription = Request.InstitutionDescription;
            Result.InstitutionDisclaimer = Request.InstitutionDisclaimer;
            Result.InstitutionDisclaimerType = Request.InstitutionDisclaimerType;
            Result.InstitutionId = Request.InstitutionId;
            Result.InstitutionName = Request.InstitutionName;
            Result.Phone = Request.CampusPhone;
            Result.PostalCode = Request.CampusPostalCode;
            Result.CustomTCPA = Request.CustomTCPA;

            //Program
            Result.ProgramCount = 1;
            List<MatchingEngine.Program> ProgramList = new List<MatchingEngine.Program>();
            MatchingEngine.Program Program = new MatchingEngine.Program();
            Program.CategoryId = Request.CategoryId;
            Program.ClickThroughUrl = Request.ClickThroughUrl;
            Program.FailedValidation = Request.FailedValidation;
            Program.CampusLogoURL = Request.CampusLogoURL;
            Program.InstitutionLogoURL = Request.InstitutionLogoURL;
            Program.InquiryDisabled = Request.InquiryDisabled;
            Program.IsHybrid = Request.IsHybrid;
            Program.PaidStatusTypeId = Request.PaidStatusTypeId;
            Program.ProgramCampusType = Request.ProgramCampusType;
            Program.ProgramDescription = Request.ProgramDescription;
            Program.ProgramDisclaimer = Request.ProgramDisclaimer;
            Program.ProgramDisclaimerType = Request.ProgramDisclaimerType;
            Program.ProgramDisplayGroupProgramLevelNameList = Request.ProgramDisplayGroupProgramLevelNameList;
            Program.ProgramId = Request.ProgramId;
            Program.ProgramLevelId = Request.ProgramLevelId;
            Program.ProgramLevelName = Request.ProgramLevelName;
            Program.ProgramName = Request.ProgramName;
            Program.ProductId = Request.ProductId;
            Program.ProgramProductId = Request.ProgramProductId;
            Program.ProgramProductIdClick = Request.ProgramProductIdClick;
            Program.ProgramRankScore = Request.ProgramRankScore;
            Program.ProgramType = Request.ProgramType;
            Program.SchoolId = Request.SchoolId;
            Program.SubjectId = Request.SubjectId;
            Program.TemplateId = Request.TemplateId;
            ProgramList.Add(Program);
            Result.ProgramList = ProgramList.ToArray();

            Result.ProgramRankScore = Request.ProgramRankScore;
            Result.State = Request.CampusState;


            return Result;
        }

        /// <summary>
        /// Method will check if the last internal validation ran was against same LeadData. If it is, it returns true, otherwise it returns the result of the internal
        /// validation
        /// </summary>
        /// <param name="LeadData"></param>
        /// <returns></returns>
        private bool IsValidRequest(string LeadData, string FESessionId)
        {
            SubmissionValidationResultDTO lastRequest = FESession.Get<SubmissionValidationResultDTO>(FESessionId, Constants.SUBMISSION_ID_SESSION_KEY);
            if (lastRequest.LeadData == LeadData && lastRequest.Valid)
            {
                return lastRequest.Valid;
            }
            else
            {
                //we need to revalidate so return the result of the new validation.
                return false;
            }
        }

        /// <summary>
        /// Adds user selection to the session
        /// </summary>
        /// <param name="InstitutionId"></param>
        /// <param name="InstitutionName"></param>
        /// <param name="ProgramName"></param>
        /// <param name="ProgramProductId"></param>
        /// <param name="FESessionId"></param>
        public static void AddUserSelectionToSession(int InstitutionId, string InstitutionName, string ProgramName, int ProgramProductId, bool Append, string FESessionId)
        {
            UserSelectionDTO UserSelection = new UserSelectionDTO();
            EddyLogosDTO EddyLogos = new EddyLogosDTO();

            UserSelection.InstitutionId = InstitutionId;
            UserSelection.InstitutionName = InstitutionName;
            UserSelection.ProgramName = ProgramName;
            UserSelection.ProgramProductId = ProgramProductId;
            UserSelection.Logo = EddyLogos.EddyLogoImagePathDomain + string.Format(EddyLogos.EddyLogoImagePathInstitution, InstitutionId, EddyLogos.EddyLogoImageSizeSmall);
            List<UserSelectionDTO> UserSelectionList = new List<UserSelectionDTO>();

            if (!Append)
            {
                FESession.Remove(FESessionId, Constants.PROGRAMTEMPLATE_USERSELECTIONS_KEY);
            }
            else
            {
                UserSelectionList = FESession.Get<List<UserSelectionDTO>>(FESessionId, Constants.PROGRAMTEMPLATE_USERSELECTIONS_KEY);
                UserSelectionList = UserSelectionList == null ? new List<UserSelectionDTO>() : UserSelectionList;
            }
            UserSelectionList.Add(UserSelection);
            FESession.Set(FESessionId, Constants.PROGRAMTEMPLATE_USERSELECTIONS_KEY, UserSelectionList);
        }


        public SubmissionResultDTO GetOptimizelyCrossSellDTO(int TemplateId, bool IsBeta, string RenderingStrategy)
        {
            SubmissionResultDTO submissionResultDTO = new SubmissionResultDTO();
            // sample data for fake optimizely cross sell
            //int applicationId = 1;
            System.Guid trackGuid = new System.Guid();
            ProspectInput prospectInput = new ProspectInput();
            string userFullName = "John Doe";
            string institutionName = "National American Community College";
            string programName = "B.S. in Basket Weaving";
            int programProductId = 1;
            int institutionId = 1;

            CrossSellResultDTO crossSellResultDTO = GetRenderedCrossSell(TemplateId, RenderingStrategy, IsBeta, trackGuid, prospectInput, null, programProductId, institutionId, new ProgramValidateResponse() { PassedValidation = true, ProgramType = ProgramType.FullDegree }, true
                , null//#54902 - Directory Cross-Sell Layer - Need code for Optimizely to test copy
                , userFullName, institutionName, programName, null, true, "default", "", null, null, null, string.Empty);
            submissionResultDTO.CrossSellResult = crossSellResultDTO;

            return submissionResultDTO;
        }


        public List<ThemeDTO> GetRenderingStrategyThemes(string Name)
        {
            List<ThemeDTO> Result = new List<ThemeDTO>();

            try
            {
                var RenderingStrategy = FormsEngineService.GetHTMLRenderingStrategy(Name);
                if (!string.IsNullOrWhiteSpace(RenderingStrategy.Name))
                {
                    string PhysicalPath = Server.MapPath(string.Format(BASE_THEMES_FOLDER, RenderingStrategy.Name));
                    DirectoryInfo[] SubDirectories = new DirectoryInfo(PhysicalPath).GetDirectories();
                    foreach (var Directory in SubDirectories)
                    {
                        ThemeDTO Theme = new ThemeDTO();
                        Theme.Name = Directory.Name;
                        Result.Add(Theme);
                    }
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return Result;
        }


        public ManagedChoiceSubmissionResultDTO ManagedChoiceLeadSubmissionDTO(int TemplateId, string ProgramArrayString, bool IsBeta, string TrackId, string LimboAlternativeCampaignTrackid, bool LimboAlternativeCampaignTrackidUtilized, string SessionId, string MatchId, int? ProspectId, string LeadData, string AdditionalData, ref RawPostDataDTO RawData, int PreviousSMLeadsCreatedCount, int PreviousUSLeadsCreatedCount, string FESessionId, FormTemplateTypes FormTemplateType, PerformanceLog Log, Dictionary<string, string> tcpaArrayString)
        {
            ManagedChoiceSubmissionResultDTO Result = new ManagedChoiceSubmissionResultDTO();
            Result.SMLeadsCreatedCount = PreviousSMLeadsCreatedCount;
            Result.USLeadsCreatedCount = PreviousUSLeadsCreatedCount;
            Guid.TryParse(SessionId, out Guid sessionGuid);
            LeadCreationType LeadType = LeadCreationType.WizardUserSelection;

            if (FormTemplateType == FormTemplateTypes.ProgramWizard)
            {
                LeadType = LeadCreationType.ProgramWizardUserSelection;
            }

            Result.Success = false;
            List<LeadSaveData> leadSaveList = GetValidatedLeadSaveDataCustomTCPA(FESessionId, Result.SMLeadsCreatedCount == 0, ProgramArrayString, TrackId, LeadData, AdditionalData, IsBeta, FormTemplateType == FormTemplateTypes.ProgramWizard, sessionGuid, ref Log, tcpaArrayString);
            int? submissionId = FESession.Get<int?>(FESessionId, Constants.SUBMISSION_ID_SESSION_KEY);
            var managedChoiceCampuses = FESession.Get<List<CampusWithInstitution>>(FESessionId, Constants.WIZARD_ME_USERSELECTRESULTS_KEY);

            foreach (LeadSaveData lsd in leadSaveList)
            {
                if (lsd.PaidStatusTypeId == null)
                {
                    lsd.PaidStatusTypeId = (int)PaidStatusType.Paid;
                }
            }

            var productIdList = new List<string>();
            var sessionProduct =  FESession.Get(FESessionId, Constants.WIZARD_LEADCREATEDPRODUCTS_KEY);
            if (sessionProduct != null)
                productIdList.AddRange(sessionProduct as List<string>);

            if (managedChoiceCampuses != null && managedChoiceCampuses.Any())
            {
                foreach (LeadSaveData lsd in leadSaveList)
                {
                    var campus = managedChoiceCampuses.Where(mc => mc.ProgramList.Any(p => p.ProgramProductId == lsd.ProgramProductId)).FirstOrDefault();

                    if (campus != default(CampusWithInstitution))
                    {
                        var program = campus.ProgramList.FirstOrDefault(p => p.ProgramProductId == lsd.ProgramProductId);

                        if (program != default(MatchingEngine.Program))
                            lsd.AllowedViaLeadScoringUpsell = program.AllowedViaLeadScoringUpsell;

                        if(program.ProductId.HasValue)
                            productIdList.Add(program.ProductId.Value.ToString());
                    }
                }
            }

            if(productIdList.Count > 0)
                FESession.Set(FESessionId, Constants.WIZARD_LEADCREATEDPRODUCTS_KEY, productIdList);

            LeadSaveManger leadSaveManager = new LeadSaveManger(Log);
            int ProspectFlowId = FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID) == null ? 0 : (int)FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID);
            List<LeadCreateResponse> leadCreateResponseList = leadSaveManager.Execute(TemplateId, FESessionId, LeadType, null, leadSaveList, IsBeta, TrackId, LimboAlternativeCampaignTrackid, LimboAlternativeCampaignTrackidUtilized, SessionId, MatchId, RawData, LeadData, AdditionalData, ProspectId, submissionId, null, MatchResponseType.SchoolSelection, null, false, null, null, ProspectFlowId);

            int newUSLeadsCreatedCount = leadCreateResponseList.Where(lc => lc.Success).Count();

            Result.USLeadsCreatedCount = Result.USLeadsCreatedCount + newUSLeadsCreatedCount;

            if (newUSLeadsCreatedCount > 0)
            {
                // set-append US Leads into Session for ThankYou to retrieve
                var UserSelections = FESession.Get<List<LeadSaveData>>(FESessionId, Constants.WIZARD_USERSELECTLEADS_KEY);
                if (UserSelections != null && UserSelections.Count > 0)
                {
                    leadSaveList.AddRange(UserSelections);
                }
                FESession.Set(FESessionId, Constants.WIZARD_USERSELECTLEADS_KEY, leadSaveList);

                #region Pixel Service
                Log.StartLogDetail("ManagedChoiceLeadSubmissionDTO.GetPixels");

                int[] leadIds = leadCreateResponseList.Where(l => l.Success).Select(id => Convert.ToInt32(id.Lead.LeadId)).ToArray();

                List<string> pixelTypes = new List<string>();
                if (PreviousSMLeadsCreatedCount < 1 && PreviousUSLeadsCreatedCount < 1)
                {
                    pixelTypes.Add("Initial Conversion Only");
                }
                pixelTypes.Add("Conversion");
                pixelTypes.Add("Cumulative Conversion");
                pixelTypes.Add("Client Relation Pixel Conversion");
                LeadCreateResponse firstValidLeadResonse = leadCreateResponseList.First(f => f.Success);

                string lastName = firstValidLeadResonse.Lead.LastName;
                string email = firstValidLeadResonse.Lead.EmailAddress;

                // set-append US Pixels into Session for ThankYou to retrieve
                var PixelCheck = GetPixels(IsBeta, lastName, email, TrackId, ProspectId, true, leadIds, pixelTypes, false);
                var PreviousPixels = (string)FESession.Get(FESessionId, Constants.WIZARD_USERSELECTPIXELS_KEY);
                if (!string.IsNullOrWhiteSpace(PreviousPixels))
                {
                    PixelCheck.PixelsWithDebugInfo = PreviousPixels + PixelCheck.PixelsWithDebugInfo;
                }
                FESession.Set(FESessionId, Constants.WIZARD_USERSELECTPIXELS_KEY, PixelCheck.PixelsWithDebugInfo);

                Log.EndLogDetail();
                #endregion

                Result.Success = true;
                Result.MoveToThankYou = true;
            }
            Log.EndLogDetail();
            return Result;
        }

        /// <summary>
        /// this method will build a list of LeadSaveData which includes a boolean for whether the program is valid or not based on a matching engine call
        /// to validate if it is a program attached to a template with questions the user answered which had not yet been validated. 
        /// </summary>
        /// <param name="FESessionId"></param>
        /// <param name="ProgramArrayString"></param>
        /// <param name="TrackId"></param>
        /// <param name="LeadData"></param>
        /// <param name="AdditionalData"></param>
        /// <param name="IsBeta"></param>
        /// <returns></returns>
        private static List<LeadSaveData> GetValidatedLeadSaveData(string FESessionId, bool lookForEPLiteUpsell, string ProgramArrayString, string TrackId, string LeadData, string AdditionalData, bool IsBeta, bool IsProgramWizard, Guid? TrackingSessionGUID, ref PerformanceLog Log)
        {
            LeadSaveManger leadSaveManager = new LeadSaveManger(Log);
            int? submissionId = FESession.Get<int?>(FESessionId, Constants.SUBMISSION_ID_SESSION_KEY);
            APIValidationResultDTO QuickCheckValidationResult = new APIValidationResultDTO();
            QuickCheckValidationResult.IsTestLead = false;
            QuickCheckValidationResult.Valid = true;
            QuickCheckValidationResult.ValidationMessages = new List<KeyValuePair<string, string>>();
            ProgramValidateRequest ProgramValidationRequest = new ProgramValidateRequest();
            ProgramValidationRequest.Application = MatchingEngine.ISApplication.FormsEngine;
            ProgramValidationRequest.TrackGuid = new Guid(TrackId);
            ProgramValidationRequest.BreakOnFirstValidationFailure = true;

            int ProgramProductId = 0;
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(0, ProgramProductId, IsBeta, TrackId, "", false, null, null, LeadData, AdditionalData, null, null, null);
            ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
            ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, TrackingSessionGUID.GetValueOrDefault().ToString());
            ProgramValidationRequest.ProspectInput = Prospect;
            var managedChoiceCampuses = FESession.Get<List<CampusWithInstitution>>(FESessionId, Constants.WIZARD_ME_USERSELECTRESULTS_KEY);
            var SmartMatches = FESession.Get<List<LeadSaveData>>(FESessionId, Constants.WIZARD_SMARTMATCHLEADS_KEY);
            lookForEPLiteUpsell = SmartMatches != null && SmartMatches.Count > 0 ? false : lookForEPLiteUpsell;

            //get list of programids that need to be validated because of additional questions
            List<string> programStringsList = new List<string>(ProgramArrayString.Split(','));

            //Set LookForEPUpsell if there is only one user selection and the calling method indicated to.
            ProgramValidationRequest.LookForEPUpsell = lookForEPLiteUpsell && programStringsList.Count == 1;

            List<LeadSaveData> leadSaveList = new List<LeadSaveData>();

            List<Guid?> usedExternalMatchGuidList = new List<Guid?>();

            foreach (string str in programStringsList)
            {
                try
                {
                    Guid? externalMatchItemGuid = null;
                    List<int> program = new List<int>(str.Split('_').Select(int.Parse));
                    ProgramValidateResponse ProgramValidationResponse = null;
                    //0 is program 1 is template
                    bool programValid = true;
                    ProgramValidationRequest.ProgramProductId = Convert.ToInt32(program[0]);
                    bool isThirdPartyProgram = false;

                    //Determine third party
                    if (managedChoiceCampuses != null)
                    {
                        var campus = managedChoiceCampuses.Where(mc => mc.ProgramList.Any(p => p.ProgramProductId == ProgramValidationRequest.ProgramProductId && !usedExternalMatchGuidList.Contains(p.ExternalMatchItemGuid))).FirstOrDefault();

                        if (campus != default(CampusWithInstitution))
                        {
                            var campusProgram = campus.ProgramList.FirstOrDefault(p => p.ProgramProductId == ProgramValidationRequest.ProgramProductId);

                            if (campusProgram != default(MatchingEngine.Program) && campusProgram.ExternalMatchItemGuid.HasValue)
                            {
                                usedExternalMatchGuidList.Add(campusProgram.ExternalMatchItemGuid.Value);

                                externalMatchItemGuid = campusProgram.ExternalMatchItemGuid;
                                isThirdPartyProgram = campusProgram.InstitutionType == MatchingEngine.InstitutionLeadTypes.ThirdPartyApiMatch;
                            }
                        }
                    }

                    if (IsProgramWizard)
                    {
                        ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.ProgramWizardUserSelection;
                    }
                    else if(!isThirdPartyProgram)
                    {
                        ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.WizardUserSelection;
                    }
                    else //Only wizards have third party programs: WizardUserSelectionThirdParty
                    {
                        ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.WizardUserSelectionThirdParty;
                    }

                    ProgramValidationResponse = FormsEngineService.RelatedServices.ValidateProgram(ProgramValidationRequest, IsBeta);
                    FESession.Set(FESessionId, Constants.ME_SKIPSCHOOLSELECTIONFORMATCHONE, ProgramValidationResponse.SkipSchoolSelection);
                    programValid = ProgramValidationResponse.PassedValidation;
                    if (!programValid)
                    {
                        //program was invalid so add on to our collection
                        QuickCheckValidationResult.Valid = false;
                        QuickCheckValidationResult.ValidationMessages
                            .Add(new KeyValuePair<string, string>(ProgramValidationResponse.RuleFailures[0].RuleFailureType.GetValueOrDefault().ToString(), ProgramValidationResponse.RuleFailures[0].RuleFailureName));
                    }
                    int programProductId = Convert.ToInt32(program[0]);

                    //If ME returns an alternate program product id then use that for the lead.
                    if (ProgramValidationResponse.AlternateProgramProductId.HasValue && ProgramValidationResponse.AlternateProgramProductId.Value > 0)
                        programProductId = ProgramValidationResponse.AlternateProgramProductId.Value;

                    if (ProgramValidationResponse != null)
                    {
                        leadSaveList.Add(new LeadSaveData() { ProgramProductId = programProductId, TemplateId = Convert.ToInt32(program[1]), IsValid = programValid, PaidStatusTypeId = (int)ProgramValidationResponse.PaidStatusTypeId.GetValueOrDefault(), ExternalMatchItemGuid = externalMatchItemGuid, OriginalProgramProductId = program[0] });
                    }
                    else
                    {
                        leadSaveList.Add(new LeadSaveData() { ProgramProductId = programProductId, TemplateId = Convert.ToInt32(program[1]), IsValid = programValid, ExternalMatchItemGuid = externalMatchItemGuid, OriginalProgramProductId = program[0] });
                    }
                }
                catch { }
            }
            if (!QuickCheckValidationResult.Valid)
            {
                //we had ME submission errors so lets save them. 
                leadSaveManager.SaveSubmissionValidationErrors(submissionId, QuickCheckValidationResult);
            }
            return leadSaveList;
        }

        private static List<LeadSaveData> GetValidatedLeadSaveDataCustomTCPA(string FESessionId, bool lookForEPLiteUpsell, string ProgramArrayString, string TrackId, string LeadData, string AdditionalData, bool IsBeta, bool IsProgramWizard, Guid? TrackingSessionGUID, ref PerformanceLog Log, Dictionary<string, string> tcpaArrayString)
        {
            LeadSaveManger leadSaveManager = new LeadSaveManger(Log);
            int? submissionId = FESession.Get<int?>(FESessionId, Constants.SUBMISSION_ID_SESSION_KEY);
            APIValidationResultDTO QuickCheckValidationResult = new APIValidationResultDTO();
            QuickCheckValidationResult.IsTestLead = false;
            QuickCheckValidationResult.Valid = true;
            QuickCheckValidationResult.ValidationMessages = new List<KeyValuePair<string, string>>();
            ProgramValidateRequest ProgramValidationRequest = new ProgramValidateRequest();
            ProgramValidationRequest.Application = MatchingEngine.ISApplication.FormsEngine;
            ProgramValidationRequest.TrackGuid = new Guid(TrackId);
            ProgramValidationRequest.BreakOnFirstValidationFailure = true;

            int ProgramProductId = 0;
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(0, ProgramProductId, IsBeta, TrackId, "", false, null, null, LeadData, AdditionalData, null, null, null);
            ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
            ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, null, TrackingSessionGUID.GetValueOrDefault().ToString());
            ProgramValidationRequest.ProspectInput = Prospect;
            var managedChoiceCampuses = FESession.Get<List<CampusWithInstitution>>(FESessionId, Constants.WIZARD_ME_USERSELECTRESULTS_KEY);
            var SmartMatches = FESession.Get<List<LeadSaveData>>(FESessionId, Constants.WIZARD_SMARTMATCHLEADS_KEY);
            lookForEPLiteUpsell = SmartMatches != null && SmartMatches.Count > 0 ? false : lookForEPLiteUpsell;

            //get list of programids that need to be validated because of additional questions
            List<string> programStringsList = new List<string>(ProgramArrayString.Split(','));

            //Set LookForEPUpsell if there is only one user selection and the calling method indicated to.
            ProgramValidationRequest.LookForEPUpsell = lookForEPLiteUpsell && programStringsList.Count == 1;

            List<LeadSaveData> leadSaveList = new List<LeadSaveData>();

            List<Guid?> usedExternalMatchGuidList = new List<Guid?>();

            foreach (string str in programStringsList)
            {
                try
                {
                    Guid? externalMatchItemGuid = null;
                    List<int> program = new List<int>(str.Split('_').Select(int.Parse));
                    ProgramValidateResponse ProgramValidationResponse = null;
                    //0 is program 1 is template
                    bool programValid = true;
                    ProgramValidationRequest.ProgramProductId = Convert.ToInt32(program[0]);
                    bool isThirdPartyProgram = false;

                    //Determine third party
                    if (managedChoiceCampuses != null)
                    {
                        var campus = managedChoiceCampuses.Where(mc => mc.ProgramList.Any(p => p.ProgramProductId == ProgramValidationRequest.ProgramProductId && !usedExternalMatchGuidList.Contains(p.ExternalMatchItemGuid))).FirstOrDefault();

                        if (campus != default(CampusWithInstitution))
                        {
                            var campusProgram = campus.ProgramList.FirstOrDefault(p => p.ProgramProductId == ProgramValidationRequest.ProgramProductId);

                            if (campusProgram != default(MatchingEngine.Program) && campusProgram.ExternalMatchItemGuid.HasValue)
                            {
                                usedExternalMatchGuidList.Add(campusProgram.ExternalMatchItemGuid.Value);

                                externalMatchItemGuid = campusProgram.ExternalMatchItemGuid;
                                isThirdPartyProgram = campusProgram.InstitutionType == MatchingEngine.InstitutionLeadTypes.ThirdPartyApiMatch;
                            }
                        }
                    }

                    if (IsProgramWizard)
                    {
                        ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.ProgramWizardUserSelection;
                    }
                    else if (!isThirdPartyProgram)
                    {
                        ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.WizardUserSelection;
                    }
                    else //Only wizards have third party programs: WizardUserSelectionThirdParty
                    {
                        ProgramValidationRequest.LeadCreationType = MatchingEngine.LeadCreationType.WizardUserSelectionThirdParty;
                    }

                    ProgramValidationResponse = FormsEngineService.RelatedServices.ValidateProgram(ProgramValidationRequest, IsBeta);
                    FESession.Set(FESessionId, Constants.ME_SKIPSCHOOLSELECTIONFORMATCHONE, ProgramValidationResponse.SkipSchoolSelection);
                    programValid = ProgramValidationResponse.PassedValidation;
                    if (!programValid)
                    {
                        //program was invalid so add on to our collection
                        QuickCheckValidationResult.Valid = false;
                        QuickCheckValidationResult.ValidationMessages
                            .Add(new KeyValuePair<string, string>(ProgramValidationResponse.RuleFailures[0].RuleFailureType.GetValueOrDefault().ToString(), ProgramValidationResponse.RuleFailures[0].RuleFailureName));
                    }
                    int programProductId = Convert.ToInt32(program[0]);

                    string programProductIdString = program[0].ToString();
                    string customTCPA = tcpaArrayString.ContainsKey(programProductIdString) ? HttpUtility.UrlDecode(tcpaArrayString[programProductIdString]) : null;

                    //If ME returns an alternate program product id then use that for the lead.
                    if (ProgramValidationResponse.AlternateProgramProductId.HasValue && ProgramValidationResponse.AlternateProgramProductId.Value > 0)
                        programProductId = ProgramValidationResponse.AlternateProgramProductId.Value;

                    if (ProgramValidationResponse != null)
                    {
                        leadSaveList.Add(new LeadSaveData() { ProgramProductId = programProductId, TemplateId = Convert.ToInt32(program[1]), IsValid = programValid, PaidStatusTypeId = (int)ProgramValidationResponse.PaidStatusTypeId.GetValueOrDefault(), ExternalMatchItemGuid = externalMatchItemGuid, OriginalProgramProductId = program[0], CustomTCPA = customTCPA });
                    }
                    else
                    {
                        leadSaveList.Add(new LeadSaveData() { ProgramProductId = programProductId, TemplateId = Convert.ToInt32(program[1]), IsValid = programValid, ExternalMatchItemGuid = externalMatchItemGuid, OriginalProgramProductId = program[0], CustomTCPA = customTCPA });
                    }
                }
                catch { }
            }
            if (!QuickCheckValidationResult.Valid)
            {
                //we had ME submission errors so lets save them. 
                leadSaveManager.SaveSubmissionValidationErrors(submissionId, QuickCheckValidationResult);
            }
            return leadSaveList;
        }

        public static Tuple<Boolean, string> CrossSellLeadSubmissionDTO(int TemplateId, string ProgramArrayString, bool IsBeta, string TrackId, string SessionId, string MatchGuid, int? ProspectId, string LeadData, string AdditionalData, bool InitialMatchWasValid, string InitialLeadId, string FESessionId, PerformanceLog Log, RawPostDataDTO RawData)
        {
            string ThankYouMessage = string.Empty;
            List<LeadSaveData> leadSaveList = GetValidatedLeadSaveData(FESessionId, !InitialMatchWasValid, ProgramArrayString, TrackId, LeadData, AdditionalData, IsBeta, true, new Guid(SessionId), ref Log);

            int? submissionId = FESession.Get<int?>(FESessionId, Constants.SUBMISSION_ID_SESSION_KEY);
            var crossSellPrograms = FESession.Get<List<ProgramWithInstitutionCampus>>(FESessionId, Constants.PROGRAMTEMPLATE_CROSSSELLPROGRAMS_KEY);
            int ProspectFlowId = FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID) == null ? 0 : (int)FESession.Get(FESessionId, Constants.PROSPECT_WORKFLOWID);

            if (crossSellPrograms != null)
            {
                foreach (LeadSaveData lsd in leadSaveList)
                {
                    if (lsd.PaidStatusTypeId == null)
                    {
                        lsd.PaidStatusTypeId = (int)PaidStatusType.Paid;
                    }

                    lsd.AllowedViaLeadScoringUpsell = crossSellPrograms.First(p => p.ProgramProductId == lsd.ProgramProductId).AllowedViaLeadScoringUpsell;
                }
            }

            LeadSaveManger leadSaveManager = new LeadSaveManger(Log);
            Guid NoGuid = Guid.Empty;

            List<LeadCreateResponse> leadCreateResponseList = leadSaveManager.Execute(TemplateId, FESessionId, LeadCreationType.InstitutionFormCrossSell, null, leadSaveList, IsBeta, TrackId, NoGuid.ToString(), false, SessionId, MatchGuid, RawData, LeadData, AdditionalData, ProspectId, submissionId, InitialLeadId, MatchResponseType.CrossSell, null, false, null, null, ProspectFlowId);

            int newCSLeadsCreatedCount = leadCreateResponseList.Where(lc => lc.Success).Count();

            if (newCSLeadsCreatedCount > 0)
            {
                // set CS Leads into Session for ThankYou to retrieve
                FESession.Set(FESessionId, Constants.PROGRAMTEMPLATE_CROSSSELLLEADS_KEY, leadSaveList);

                #region Pixel Service
                Log.StartLogDetail("CrossSellLeadSubmission.GetPixels");

                int[] leadIds = leadCreateResponseList.Where(l => l.Success).Select(id => Convert.ToInt32(id.Lead.LeadId)).ToArray();

                List<string> pixelTypes = new List<string>();
                if (!InitialMatchWasValid || InitialLeadId == null || InitialLeadId == "")
                {
                    pixelTypes.Add("Initial Conversion Only");
                }
                pixelTypes.Add("Conversion");
                pixelTypes.Add("Cumulative Conversion");
                pixelTypes.Add("Client Relation Pixel Conversion");
                LeadCreateResponse firstValidLeadResonse = leadCreateResponseList.First(f => f.Success);

                string firstname = firstValidLeadResonse.Lead.FirstName;
                string lastName = firstValidLeadResonse.Lead.LastName;
                string email = firstValidLeadResonse.Lead.EmailAddress;
                string institutionname = GetUserSelectionFromCache(firstValidLeadResonse.Lead.ProgramProductId, FESessionId).InstitutionName;

                PixelCheckDTO CSPixels = GetPixels(IsBeta, lastName, email, TrackId, ProspectId, true, leadIds, pixelTypes, false);

                // set CS Pixels into Session for ThankYou to retrieve
                FESession.Set(FESessionId, Constants.PROGRAMTEMPLATE_CROSSSELLPIXELS_KEY, CSPixels.PixelsWithDebugInfo);

                //Set user selections for thank you page
                var selected = leadCreateResponseList.Where(lc => lc.Success);
                foreach (var success in selected)
                {
                    var UserSelection = GetUserSelectionFromCache(success.Lead.ProgramProductId, FESessionId);
                    if (UserSelection != null)
                    {
                        AddUserSelectionToSession(UserSelection.InstitutionId, UserSelection.InstitutionName, UserSelection.ProgramName, UserSelection.ProgramProductId, true, FESessionId);
                    }

                }

                ThankYouMessage = (!InitialMatchWasValid || InitialLeadId == null || InitialLeadId == "") ? (newCSLeadsCreatedCount == 1) ? FormsEngine.GetResourceMetaDataTextForKey("Generic_OneSchool_ThankYou_Success") : FormsEngine.GetResourceMetaDataTextForKey("Generic_MultipleSchool_ThankYou_Success") : "";
                ThankYouMessage = string.Format(ThankYouMessage, firstname, institutionname);

                Log.EndLogDetail();
                #endregion
            }

            return new Tuple<Boolean, string>(true, ThankYouMessage);
        }


        public static ProgramWithInstitutionCampus GetUserSelectionFromCache(int? ProgramProductId, string FESessionId)
        {
            ProgramWithInstitutionCampus Result = null;
            var CrossSellPrograms = FESession.Get<List<ProgramWithInstitutionCampus>>(FESessionId, Constants.PROGRAMTEMPLATE_CROSSSELLPROGRAMS_KEY);

            if (ProgramProductId != null && CrossSellPrograms != null && CrossSellPrograms.Count() > 0)
            {
                foreach (var CrossSellItem in CrossSellPrograms)
                {
                    if (CrossSellItem.ProgramProductId == ProgramProductId)
                    {
                        Result = CrossSellItem;
                        break; 
                    }
                }
            }

            return Result;
        }


        public WizardMatchResponse GetSmartMatchesForWizard(bool IsBeta, int WizardTemplateId, string TrackId, string SessionId, string MatchId, string LeadData, string AdditionalData, int PreviousSMLeadCount, int PreviousUSLeadCount, string FESessionId, bool IncludeProgramTemplatesFromSession, string DeviceId, bool SmartMatchZero, int? ApplicationId, bool includeThirdParty, PerformanceLog Log)
        {
            //0. create LeadCreateRequest object
            #region Create LeadCreateRequest Object
            Guid? TrackingDeviceGUID = null;

            if (!String.IsNullOrWhiteSpace(DeviceId))
            {
                Guid tempDeviceGuid;

                if (Guid.TryParse(DeviceId, out tempDeviceGuid))
                {
                    TrackingDeviceGUID = tempDeviceGuid;
                }
            }

            Log.StartLogDetail("GetSmartMatchesForWizard.BuildLeadCreateRequestObject");
            int ProgramProductId = 0;
            LeadCreateRequest LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(0, ProgramProductId, IsBeta, TrackId, "", false, SessionId, MatchId, LeadData, AdditionalData, null, null, null);
            Log.EndLogDetail();
            #endregion

            Log.StartLogDetail("GetSmartMatchesForWizard.BuildLeadDTO");
            LeadDTO LeadDTO = LeadEngineService.BuildLeadDTO(LeadRequest, false);
            Log.EndLogDetail();
            
            //1. Express Consent Mobile Check call
            #region Express Consent
            Log.StartLogDetail("GetSmartMatchesForWizard.CheckForExpressConsent");
            ExpressConsentCheckDTO ExpressConsentCheck = CheckForMobile(LeadDTO.Phone1, LeadDTO.Phone2, Log); //phone1 phone2 email first last
            Log.EndLogDetail();
            #endregion


            Log.StartLogDetail("GetManagedChoiceDTO.BuildProspectInput");
            ProspectInputBuilder.AggregateCreativeURLs(LeadRequest);
            ProspectInput Prospect = ProspectInputBuilder.BuildProspectInput(LeadRequest.LeadData, LeadRequest.LeadAdditionalData, ExpressConsentCheck.MobilePhones.Count > 0, LeadRequest.TrackingSessionGUID.GetValueOrDefault().ToString());
            Log.EndLogDetail();

            CampusPreference? CampusPreference = null;
            if (LeadRequest.LeadData.ContainsKey("CampusSoftPreference"))
            {
                string cp = LeadRequest.LeadData["CampusSoftPreference"].ToLower();
                if (cp == "online") { CampusPreference = EDDY.IS.FormsEngine.MatchingEngine.CampusPreference.Online; }
                else if (cp == "campus") { CampusPreference = EDDY.IS.FormsEngine.MatchingEngine.CampusPreference.Ground; }
                else { CampusPreference = EDDY.IS.FormsEngine.MatchingEngine.CampusPreference.Both; }
            }

            List<int> Templates = null;
            if (!SmartMatchZero)
            {
                //Wizard match request (smart match)
                if (IncludeProgramTemplatesFromSession)
                {
                    Templates = FESession.Get<List<int>>(FESessionId, Constants.WIZARD_SESSION_PROGRAMTEMPLATESCOVERED);
                    if (Templates == null)
                    {
                        Templates = FormsEngineService.GetProgramTemplatesCoveredByWizardQuestions(WizardTemplateId);
                        FESession.Set(FESessionId, Constants.WIZARD_SESSION_PROGRAMTEMPLATESCOVERED, Templates);
                    }
                }
            }
            WizardMatchRequest Request = FormsEngineService.BuildWizardMatchRequest(IsBeta, PreviousSMLeadCount, PreviousUSLeadCount, true, false, LeadRequest.TrackId.GetValueOrDefault(), LeadRequest.LeadData, Prospect, CampusPreference, null, null, Templates, TrackingDeviceGUID, ApplicationId, FESessionId, null);
            Request.LeadCreationType = MatchingEngine.LeadCreationType.WizardSmartMatch;
            Request.IncludeThirdPartyMatchList = includeThirdParty;

            if (SmartMatchZero)
            {
                Request.IncludeSchoolSelectionList = true;
                Request.IncludeSmartMatchList = false;
            }
            WizardMatchResponse Response = FormsEngineService.RelatedServices.GetWizardMatches(Request, IsBeta);
            FESession.Set(FESessionId, Constants.ME_SKIPSCHOOLSELECTIONFORMATCHONE, Response.SkipSchoolSelection);


            if (SmartMatchZero)
            {
                Response.SmartMatchList = null;
                Response.ThirdPartyMatchList = null;
                Response.ResultCount = Response.SchoolSelectionList != null ? Response.SchoolSelectionList.Count() : 0;
                
            }
      
            
        
            //matching engine is saying this is a forced limbo, remove all smart matches
            if (Response.WizardLimboReason == LimboReason.LeadScoringMinimumTierLevel)
            {
                FESession.Set(FESessionId, Constants.WIZARD_ME_LIMBOREASON_KEY, Response.WizardLimboReason);
                Response.SmartMatchList = null;
            }

        
            Response.SmartMatchList = Response.SmartMatchList == null ? new ProgramWithInstitutionCampus[] { } : Response.SmartMatchList;
            Response.ThirdPartyMatchList = Response.ThirdPartyMatchList == null ? new ProgramWithInstitutionCampus[] { } : Response.ThirdPartyMatchList;

            FESession.Set(FESessionId, Constants.WIZARD_ME_TOKEN_SMARTMATCHRESULTS_KEY, Response.MatchResponseGuid);
            FESession.Set(FESessionId, Constants.WIZARD_ME_SMARTMATCHRESULTS_KEY, Response.SmartMatchList.ToList());
            FESession.Set(FESessionId, Constants.WIZARD_ME_THIRDPARTYSMARTMATCHRESULTS_KEY, Response.ThirdPartyMatchList.ToList());

            return Response;
        }

        public IDictionary<int, bool> ResubmitProspects(List<int> prospectResubmissionId, PerformanceLog Log)
        {
            List<VW_ProspectResubmissionsDTO> prospectResubmissions = FormsEngineService.GetProspectResubmissions(prospectResubmissionId);

            IDictionary<int, bool> prospectResubmitResults = new Dictionary<int, bool>();

            Parallel.ForEach(prospectResubmissions, (prospectResubmission) => {

                //Convert the resubmission data to the request object in the DTO for submission
                ProspectResubmitRequestDTO resubmissionRequest = new ProspectResubmitRequestDTO(prospectResubmission);

                //phone1 phone2 email first last;
                ExpressConsentCheckDTO expressConsentCheck = CheckForMobile(resubmissionRequest.ProspectInput.Phone1, resubmissionRequest.ProspectInput.Phone2, Log);

                //Get the wizard matches to be put in the FE session cache
                WizardMatchResponse wizardMatchResponse = GetSmartMatchesForWizard(false, 0, resubmissionRequest.TrackId, resubmissionRequest.FESessionId, resubmissionRequest.MatchRecordId, resubmissionRequest.LeadData, null, 0, 0,
                    resubmissionRequest.FESessionId, false, string.Empty, false, resubmissionRequest.ApplicationId, true, Log);

                //This call uses the actual cached wizard matches to create the leads for the prospect
                ManagedChoiceResultDTO result = GetRenderedManagedChoice(resubmissionRequest.LeadRequest, resubmissionRequest.LeadData, null, resubmissionRequest.RenderingStrategy, new RawPostDataDTO(), expressConsentCheck,
                    resubmissionRequest.ProspectInput, resubmissionRequest.CampusSoftPreference, false, resubmissionRequest.UserFullName, string.Empty, 0, 0, resubmissionRequest.FESessionId, false, null, false, wizardMatchResponse, string.Empty, true,
                false, resubmissionRequest.ApplicationId, Log);

                //Removing from session
                FESession.Remove(resubmissionRequest.FESessionId, Constants.WIZARD_ME_TOKEN_SMARTMATCHRESULTS_KEY);
                FESession.Remove(resubmissionRequest.FESessionId, Constants.WIZARD_ME_SMARTMATCHRESULTS_KEY);
                FESession.Remove(resubmissionRequest.FESessionId, Constants.WIZARD_ME_THIRDPARTYSMARTMATCHRESULTS_KEY);

                prospectResubmitResults.Add((int)result.ProspectId, result.Success);
            });

            return prospectResubmitResults;
        }

        private void RemoveSchoolSelectionSchoolsFromAdExclusion(List<CampusWithInstitution> schoolSelections, string feSessionId)
        {
            if (!string.IsNullOrWhiteSpace(feSessionId) && schoolSelections?.Count() > 0)
            {
                List<string> schoolNamesInSchoolSelection = schoolSelections.Select(s => s.InstitutionName).ToList();

                var schoolsToBeExcludedFromAds = FESession.Get<List<string>>(feSessionId, Constants.WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY);
                schoolsToBeExcludedFromAds = schoolsToBeExcludedFromAds.Where(s => !schoolNamesInSchoolSelection.Contains(s)).ToList();

                FESession.Set(feSessionId, Constants.WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY, schoolsToBeExcludedFromAds);
            }
        }

        private static bool ShouldGetCreativeUrls(LeadCreateRequest leadRequest, string feSessionId) {
            return true;
        }
    }
}