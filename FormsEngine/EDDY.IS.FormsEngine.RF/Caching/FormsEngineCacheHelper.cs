using System;
using System.Collections.Generic;
using EDDY.IS.FormsEngine.DataModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EDDY.IS.FormsEngine.Caching
{
    public class FormsEngineCacheHelper
    {
        private static TemplateDataService _TemplateDataService = new TemplateDataService();
        private static RenderingStrategyDataService _RenderingStrategyDataService = new RenderingStrategyDataService();
        private static EMSTCPAMessageDataService _TCPAMessageDataService = new EMSTCPAMessageDataService();
        private static CampaignDataService _CampaignDataService = new CampaignDataService();
        private static OpenMailDataService _OpenMailDataService = new OpenMailDataService();

        public static Dictionary<int, List<TemplateControl>> GeControlTemplateDictionary()
        {
            return _TemplateDataService.GetControlTemplateDictionary();
        }

        public static Dictionary<string, List<ValueList>> GetStandardControlPreDefinedValueList()
        {
            return _TemplateDataService.GetStandardControlPreDefinedValueList();
        }

        public static Dictionary<string, List<string>> GetControlCodeFiltersDictionary()
        {
            return _TemplateDataService.GetControlCodeFiltersDictionary();
        }

        public static Dictionary<int, Template> GetTemplateDictionary()
        {
            return _TemplateDataService.GetTemplateDictionary();
        }

        public static TemplateMappingCache GetTemplateMappingCache()
        {
            return _TemplateDataService.GetTemplateMappingCache();
        }

        public static Dictionary<string, HTMLRenderingStrategy> GetHTMRenderingStrategyDictionary()
        {
            return _RenderingStrategyDataService.GetHTMRenderingStrategyDictionary();
        }

        public static HashSet<string> GetHTMRenderingStrategyAssignmentDictionary()
        {
            return _RenderingStrategyDataService.GetHTMRenderingStrategyAssignmentDictionary();
        }

        public static List<TemplateApplicationOverride> GetTemplateApplicationOverrides()
        {
            return _TemplateDataService.GetTemplateApplicationOverrides();
        }

        public static Dictionary<int, string> GetEMSTCPAMessageDictionary()
        {
            return _TCPAMessageDataService.GetEMSTCPAMessageDictionary();
        }

        public static Dictionary<Guid, int> GetCampaignApplicationIdDictionary()
        {
            return _CampaignDataService.GetCampaignApplicationIdDictionary();
        }

        public static Dictionary<int, KVCodeData> GetEntryTermDictionary()
        {
            return _TemplateDataService.GetKVCodeData(Constants.ENTRYTERM_KVCODEDATA);
        }

        public static Dictionary<int, KVCodeData> GetReferralDictionary()
        {
            return _TemplateDataService.GetKVCodeData(Constants.REFERRAL_KVCODEDATA);
        }

        public static Dictionary<int, ProgramLevel> GetProgramLevelDictionary()
        {
            return _TemplateDataService.GetProgramLevelDictionary();
        }
        public static List<int> GetFeaturedListFromDatabase()
        {
            return _TemplateDataService.GetFeaturedListFromDatabase();
        }

        public static Dictionary<int, OpenMailProfile> GetOpenMailDictionary() {
            return _OpenMailDataService.GetOpenMailProfileDictionary();
        }
    }
}
