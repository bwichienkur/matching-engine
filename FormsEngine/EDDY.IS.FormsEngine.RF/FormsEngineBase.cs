using EDDY.IS.Base;
using EDDY.IS.Core;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.DataModel;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Mapper;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
namespace EDDY.IS.FormsEngine
{
    public class FormsEngineBase
    {
        //Internal Services
        //internal ValidationService.ValidationServiceClient ValidationEngine = new ValidationService.ValidationServiceClient();
        internal Validation.ValidationEngine ValidationEngine = new Validation.ValidationEngine();
        internal LeadEngine.LeadEngine LeadEngineService = new LeadEngine.LeadEngine();

        //Related services 
        public FormsRelatedServices RelatedServices = new FormsRelatedServices();
        
        //DAO
        internal static TemplateDataService dbTemplateService = new TemplateDataService();
        internal static RenderingStrategyDataService dbRenderingStrategyService = new RenderingStrategyDataService();
        internal static ResourceMetaDataService dbMetaDataService = new ResourceMetaDataService();
        internal static EMSTCPAMessageDataService dbTCPAMessageService = new EMSTCPAMessageDataService();
        internal static CampaignDataService dbCampaignDataService = new CampaignDataService();
        internal static SubmissionDataService dbSubmissionDataService = new SubmissionDataService();
        internal static OpenMailDataService dbOpenMailProfileDataService = new OpenMailDataService();
        internal static LandingPageDataService dbLandingPageDataService = new LandingPageDataService();



        /// <summary>
        /// Returns the name of an alternative template if found.
        /// </summary>
        /// <param name="RequestedTemplateId"></param>
        /// <param name="AlternativeTemplates"></param>
        /// <returns></returns>
        public int FindAlternativeTemplateId(int RequestedTemplateId, string AlternativeTemplates)
        {
            int Result = RequestedTemplateId;
            string[] Alternative = AlternativeTemplates.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string SRequestedTemplateId = RequestedTemplateId.ToString();
            foreach (string s in Alternative)
            {
                string[] pair = s.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (pair[0] == SRequestedTemplateId)
                {
                    int AlternativeTemplateId = 0;
                    if (int.TryParse(pair[1], out AlternativeTemplateId))
                    {
                        Result = AlternativeTemplateId;
                    }
                    break;
                }
            }
            return Result;
        }

        /// <summary>
        /// Returns the list of html rendering strategies
        /// </summary>
        /// <param name="Wizard"></param>
        /// <returns></returns>
        public List<HTMLRenderingStrategyDTO> GetRenderingStrategies(FormTemplateTypes TemplateTypeId)
        {
            return (List<HTMLRenderingStrategyDTO>)dbTemplateService.GetRenderingStrategies(TemplateTypeId).ConvertToDTO();
        }


        /// <summary>
        /// Returns TemplateId if found otherwise default templateid
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="IsWizard"></param>
        /// <returns></returns>
        public int GetTemplateIdOrDefault(int TemplateId, bool IsWizard, int? ProgramId, int? ProgramProductId)
        {
            return dbTemplateService.GetTemplateIdOrDefault(TemplateId, IsWizard, ProgramId, ProgramProductId);
            }


        /// <summary>
        /// Validates if template exists
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public bool TemplateExists(int TemplateId)
        {
            return dbTemplateService.TemplateExists(TemplateId);
        }


        /// <summary>
        /// Gets default template id
        /// </summary>
        /// <param name="IsWizard"></param>
        /// <returns></returns>
        public int GetDefaultTemplateId(bool IsWizard)
        {
            return dbTemplateService.GetDefaultTemplateId(IsWizard);
        }

        /// <summary>
        /// Gets the full template data model (no cached)
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public TemplateDTO GetFullTemplate(int TemplateId)
        {
            return dbTemplateService.GetFullTemplate(TemplateId).ConvertToDTO();
        }

        /// <summary>
        /// Gets Template and Template Type from cache
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public TemplateDTO GetShallowTemplate(int TemplateId)
        {
            return dbTemplateService.GetShallowTemplate(TemplateId).ConvertToDTO();
        }


        /// <summary>
        /// Gets the template from cache if available if not will try to load if from database
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="IsWizard"></param>
        /// <returns></returns>
        public TemplateDTO GetTemplate(int TemplateId, bool IsWizard)
        {
            TemplateDTO Result = null;
            string KeyModel = string.Format(Constants.TEMPLATES_MODELCACHE_KEY, TemplateId).Replace(' ', '_');
            TemplateId = GetTemplateIdOrDefault(TemplateId, IsWizard, null, null);


            if (ConfigurationManager.AppSettings.Get("CacheTemplatesEnabled").ToLower() == "true")
            {
                //Get Item from Cache
                Result = FormsEngineCacheProxy.Cache.Get<TemplateDTO>(KeyModel);
                if (Result == null)
                {
                    Result = GetFullTemplate(TemplateId);
                    //Set Template to cache
                    FormsEngineCacheProxy.Cache.Set(KeyModel, Result, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                }
            }
            else
            {
                //Load directly from DB
                Result = GetFullTemplate(TemplateId);
            }
            return Result;
        }

        /// <summary>
        /// Gets the template from cache if available if not will try to load if from database
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="IsWizard"></param>
        /// <returns></returns>
        public TemplateDTO GetMergedTemplates(int[] templateIds, bool IsWizard)
        {
            TemplateDTO result = null;
            string templateIdsString = string.Join("", templateIds.OrderBy(i => i).ToArray());
            string KeyModel = string.Format(Constants.TEMPLATES_MODELCACHE_KEY, templateIdsString).Replace(' ', '_');
            if (ConfigurationManager.AppSettings.Get("CacheTemplatesEnabled").ToLower() == "true")
            {
                //Get Item from Cache
                result = FormsEngineCacheProxy.Cache.Get<TemplateDTO>(KeyModel);
                if (result == null)
                {
                    result = GetFullMergedTemplates(templateIds);
                    //Set Template to cache
                    FormsEngineCacheProxy.Cache.Set(KeyModel, result, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplatesTimeMinutes")));
                }
            }
            else
            {
                //Load directly from DB
                result = GetFullMergedTemplates(templateIds);
            }
            return result;
        }

        /// <summary>
        /// Gets the full template data model (no cached)
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public TemplateDTO GetFullMergedTemplates(int[] templateIds)
        {
            return dbTemplateService.GetFullMergedTemplates(templateIds).ConvertToDTO();
        }
    }
}
