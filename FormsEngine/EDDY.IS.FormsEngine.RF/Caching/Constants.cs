
namespace EDDY.IS.FormsEngine.Caching
{
    public class Constants
    {
        public const int CACHE_EXCEPTION_DEFAULT_EXPIRATION = 10;
        public const string CACHE_ITEM_PREFIX = "FE_";
        public const string TEMPLATEMAPPING_CACHE_KEY = "TemplateMapping";
        public const string HTML_RENDERINGSTRATEGY_KEY = "HtmlRenderingStrategy";
        public const string HTML_RENDERINGSTRATEGY_ASSIGNMENT_KEY = "HtmlRenderingStrategyAssignment";
        public const string BASIC_TEMPLATE_CACHE_KEY = "BasicTemplateEntities";
        public const string TEMPLATECONTROL_CACHE_KEY = "TemplateControlEntities";
        public const string STANDARDCONTROLLIST_CACHE_KEY = "StandardControlListEntities";
        public const string TEMPLATECONTROLCODEFILTER_CACHE_KEY = "TemplateControlCodeFilterEntities";
        public const string TEMPLATE_PROGRAM_BOUND_CACHE_KEY = "TemplateProgramBoundKey";
        public const string TEMPLATE_CONTROLSMISSING_KEY = "TemplateControlsMissing.{0}.{1}"; //wizardid, templateid
        public const string TEMPLATE_CONTROLS_ENHANCEDPROGMATCH_KEY = "TemplateControlsEnhancedProgMatch.{0}.{1}"; //MatchedTemplatedId, templateid
        public const string HTML_RENDERINGSTRATEGY_HASHKEY = "HTMLRS.{0}.{1}"; //HTMLRenderingStrategyId TemplateTypeId
        public const string WIZARDTEMPLATE_TEMPLATESCOVERED_KEY = "WizardTemplateTemplatesCovered.{0}"; //wizardid
        public const string TEMPLATE_ADDITIONALCONTROLS_HTML_KEY = "TemplateAdditionalControls_html.{0}.{1}"; //wizardid, templateid
        public const string TEMPLATES_CACHE_KEY = "TemplateManagerController.Templates.[{0}]|[{1}]|[{2}]|[{3}]"; //TemplateId, RenderingStrategy, Theme, protocol (http/https)
        public const string TEMPLATES_MODELCACHE_KEY = "TemplateManagerController.Templates.Model[{0}]"; //TemplateId
        public const string TEMPLATE_APPLICATION_OVERRIDES = "TemplateApplicationOverrides"; //TemplateApplicationOverride Collection
        public const string STATIC_FILE_CACHE_KEY = "STATIC_FILE.{0}";
        public const string CLIENTJS_CACHE_KEY = "CLIENT_JS";
        public const string CLIENTWIZARDJS_CACHE_KEY = "CLIENTWIZARD_JS";
        public const string PROGRAMCOUNTER_CACHE_KEY = "MatchingEngineController.ProgramCounter.[{0}]"; //Initial Program Counter by Trackid
        public const string LEAD_SAVE_CACHE_KEY = "LeadSave.Session{0}.PP{1}";
        public const string WIZARD_BUNDLE_JAVASCRIPT = "~/Bundles/Dynamic/js/Wizard";
        public const string QDFPlugin_BUNDLE_JAVASCRIPT = "~/Bundles/Dynamic/js/QDFPlugin";
        public const string QDF_BUNDLE_JAVASCRIPT = "~/Bundles/Dynamic/js/QDF";
        public const string QDF_BUNDLE_CSS_PATH = "~/Bundles/Dynamic/CSS/QDF/{0}";
        public const string QDFPlugin_BUNDLE_CSS_CACHE_KEY = "QDFPlugin_Bundle_CSS_{0}_{1}";
        public const string QDF_BUNDLE_CSS_CACHE_KEY = "QDF_Bundle_CSS_{0}_{1}";
        public const string WIZARD_BUNDLE_GLOBAL = "~/Bundles/Dynamic/js/Global";
        public const string WIZARD_BUNDLE_CSS_PATH = "~/Bundles/Dynamic/CSS/Wizard/{0}";
        public const string WIZARD_BUNDLE_CSS_CACHE_KEY = "Wizard_Bundle_CSS_{0}_{1}";
        public const string STANARDARD_CONTROL_CODE_CACHE_KEY = "StandardControlCode";
        public const string ENTRYTERM_CACHE_KEY = "EntryTermEntities";
        public const string PROGRAMLEVEL_CACHE_KEY = "ProgramLevelEntities";
        public const string FEATUREDLIST_CACHE_KEY = "FeaturedListEntities";
        public const string REFERRAL_CACHE_KEY = "ReferralEntities";
        public const string OPENMAILPROFILE_CACHE_KEY = "OpenMailProfile";

        // Session Objects
        public const string P3P_HEADER = "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"";
        public const string WORKFLOW_SESSIONKEY = "WZ-WF";
        public const string FORMFIELDS_SESSION = "WFORM";
        public const string PROGRAMTEMPLATEFORMFIELDS_SESSION = "TFORM";
        public const string FORMFIELDS_SESSION_ADDITIONAL = "WFORM_DynamicQuestions";
        public const string WIZARD_ADDITIONALQACOLLECTION_KEY = "ADDITIONALQUESTIONSCODEANSWERSARRAY";
        public const string WIZARD_ADDITIONALQPERTEMPLATE_KEY = "ADDITIONALQUESTIONSPERTEMPLATEARRAY";
        public const string WIZARD_ME_SMARTMATCHRESULTS_KEY = "ME_SmartMatchResults";
        public const string WIZARD_ME_TOKEN_SMARTMATCHRESULTS_KEY = "ME_SmartMatchResults_METoken";
        public const string WIZARD_ME_THIRDPARTYSMARTMATCHRESULTS_KEY = "ME_ThirdPartySmartMatchResults";
        public const string WIZARD_ME_SAVETHIRDPARTYSMARTMATCHRESULTS_KEY = "ME_SaveThirdPartySmartMatchResults";
        public const string WIZARD_ME_USERSELECTRESULTS_KEY = "ME_UserSelectResults";
        public const string WIZARD_ME_LIMBOREASON_KEY = "ME_LimboReason";
        public const string WIZARD_SMARTMATCHLEADS_KEY = "SmartMatchLeads";
        public const string WIZARD_USERSELECTLEADS_KEY = "UserSelectLeads";
        public const string WIZARD_USERSELECTLEADIDS_KEY = "UserSelectLeadIds";
        public const string WIZARD_USERSELECTPROGRAMIDS_KEY = "UserSelectProgramIds";
        public const string WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY = "SchoolsToBeExcludedByAds";
        public const string WIZARD_LEADCREATEDPRODUCTS_KEY = "LeadCreatedProducts";
        public const string WIZARD_SMARTMATCHPIXELS_KEY = "SmartMatchPixels";
        public const string WIZARD_USERSELECTPIXELS_KEY = "UserSelectPixels";
        public const string WIZARD_ADVISINGFLOWPIXELS_KEY = "AdvisingFlowPixels";
        public const string PROGRAMTEMPLATE_INITIALLEAD_KEY = "InitialLead";
        public const string PROGRAMWIZARD_INITIALCAMPUS_KEY = "PW.InitialCampus";
        public const string PROGRAMTEMPLATE_INITIALEADPIXELS_KEY = "InitialLeadPixels";
        public const string PROGRAMTEMPLATE_CROSSSELLLEADS_KEY = "CrossSellLeads";
        public const string PROGRAMTEMPLATE_CROSSSELLPIXELS_KEY = "CrossSellPixels";
        public const string PROGRAMTEMPLATE_MOBILENUMBERS = "ProgramTemplateMobileNumbers";
        public const string PROSPECT_WORKFLOWID = "Prospect.WorkflowId";
        public const string PROGRAMTEMPLATE_USERSELECTIONS_KEY = "ProgramTemplate.UserSelections";
        public const string PROGRAMTEMPLATE_CROSSSELLPROGRAMS_KEY = "ProgramTemplate.CrossSellPrograms";

        public const string SUBMISSION_ID_SESSION_KEY = "SubmissionId";
        public const string SUBMISSION_SS_MATCHRESPONSEGUID = "SubmissionSSMatchResponseGuid";
        public const string SUBMISSION_CROSSELL_MATCHRESPONSEGUID = "SubmissionCrossSellMatchResponseGuid";
        public const string WIZARD_SESSION_PROGRAMTEMPLATESCOVERED = "Wizard.Session.ProgramTemplatesCovered";

        public const string RESOURCEMETADATA_CACHE_KEY = "ResourceMetaData";
        public const string RESOURCEMETADATATCPAKEYS_CACHE_KEY = "ResourceMetaDataTCPAKeys";
        public const string PROGRAMTEMPLATEMESSAGE_CACHE_KEY = "ProgramTemplateMessageList";
        public const string EMS_INSTITUTION_CACHE_KEY = "EMSInstitutionTCPAMessages";
        public const string CAMPAIGN_APPLICATION_ID_CACHE_KEY = "CampaignApplicationIdList";
        public const string LANDING_PAGE_SETTING_CACHE_KEY = "LandingPageSettingList";

        public const string LEADSCORE_SESSION_KEY = "LeadScoreObject";
        public const int MATCHONE_PRODUCTID = 48;
        public const int WTT_PRODUCTID = 52;
        public const int EMS_PRODUCTID = 101;
        public const int EMS_PPI_PRODUCTID = 121;
        public const int EMS_APPLICATIONID = 27;

        public const string ME_SKIPSCHOOLSELECTIONFORMATCHONE = "SkipSchoolSelection";
        public const string SCHOOLPICKERWIZARD_RENDERINGSTRATEGY = "SCHOOLPICKERWIZARD";

        public const string ENTRYTERM_KVCODEDATA = "EntryTerm";
        public const string REFERRAL_KVCODEDATA = "Referral";

        public const string INITIATING_URL = "InitiatingURL";
        public const string LANDING_URL = "LandingURL";
        public const string CALL_CENTER_URL = "CallCenterURL";
    }
}
