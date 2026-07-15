using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.Util.StringExtensions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers.Base
{
    public class SessionControllerBase : Controller
    {

        public const int MAX_SIZE = 32768;

        private const string APP_PREFIX = "FE";
        private static ConnectionMultiplexer RedisConnection = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings.Get("RedisServer"));
        private string ENV_PREFIX = ConfigurationManager.AppSettings.Get("RedisCachePrefix");

        public bool SetRedisSessionCache(string FESessionId, string value)
        {
            bool Result = true;

            try
            {
                IDatabase db = RedisConnection.GetDatabase();
                db.StringSet($"{ENV_PREFIX}.{APP_PREFIX}.Session[{FESessionId}]", value, TimeSpan.FromMinutes(double.Parse(ConfigurationManager.AppSettings.Get("RedisCacheSessionMinutes"))));
            }
            catch (Exception ex)
            {
                Result = false;
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return Result;

        }

        public bool ExtendRedisSessionCache(string FESessionId)
        {
            bool Result = true;

            try
            {
                IDatabase db = RedisConnection.GetDatabase();
                db.KeyExpire($"{ENV_PREFIX}.{APP_PREFIX}.Session[{FESessionId}]", TimeSpan.FromMinutes(double.Parse(ConfigurationManager.AppSettings.Get("RedisCacheSessionMinutes"))));
            }
            catch (Exception ex)
            {
                Result = false;
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return Result;

        }

        public List<KeyValuePair<string, string>> GetWizardFormValues(string FESessionId)
        {
            List<KeyValuePair<string, string>> Result = new List<KeyValuePair<string, string>>();
            string FormValues = string.Empty, FormAdditionalValues = string.Empty;
            string FormValuesAlternative = string.Empty;
            List<string> SchoolsToBeExcludedByAds = new List<string>();
            List<string> LeadCreatedProducts = new List<string>();
           
            try
            {
                if (FESession.ContainsKey(FESessionId, Constants.FORMFIELDS_SESSION))
                {
                    FormValues = ((string[])FESession.Get(FESessionId, Constants.FORMFIELDS_SESSION))[0];
                }
            }
            catch { }

            try
            {
                if (FESession.ContainsKey(FESessionId, Constants.WORKFLOW_SESSIONKEY))
                {
                    FormValuesAlternative = Server.UrlDecode((((DTO.Extended.FormsEngineWorkflowStatus)FESession.Get(FESessionId, Constants.WORKFLOW_SESSIONKEY)).LeadDataEncoded));
                }
            }
            catch { }

            

            try
            {
                if (FESession.ContainsKey(FESessionId, Constants.FORMFIELDS_SESSION_ADDITIONAL))
                {
                    FormAdditionalValues = ((string[])FESession.Get(FESessionId, Constants.FORMFIELDS_SESSION_ADDITIONAL))[0];
                }
            }
            catch { }

            try
            {
                if (FESession.ContainsKey(FESessionId, Constants.WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY))
                {
                    SchoolsToBeExcludedByAds = FESession.Get<List<string>>(FESessionId, Constants.WIZARD_SCHOOLSTOBEEXCLUDEDBYADS_KEY);
                }
                SchoolsToBeExcludedByAds = SchoolsToBeExcludedByAds ?? new List<string>();
            }
            catch { }

            try
            {
                if (FESession.ContainsKey(FESessionId, Constants.WIZARD_LEADCREATEDPRODUCTS_KEY))
                {
                    LeadCreatedProducts = FESession.Get<List<string>>(FESessionId, Constants.WIZARD_LEADCREATEDPRODUCTS_KEY);
                }
                LeadCreatedProducts = LeadCreatedProducts ?? new List<string>();
            }
            catch { }
            

            FormValues = FormValues ?? string.Empty;
            FormAdditionalValues = FormAdditionalValues ?? string.Empty;
            FormValuesAlternative = FormValuesAlternative ?? string.Empty;
            Dictionary<string, string> dict = FormValues.BuildCaseInsensitiveDictionary();
            Dictionary<string, string> dictAlternative = FormValuesAlternative.BuildCaseInsensitiveDictionary();
            Dictionary<string, string> dictAdditional = FormAdditionalValues.BuildCaseInsensitiveDictionary();
            
            //One merged dictionary
            dict = GetMergedDictionary(dict, dictAlternative);
            dict = GetMergedDictionary(dict, dictAdditional);



            Result.Add(new KeyValuePair<string, string>("schoolsselected", string.Join(", ", SchoolsToBeExcludedByAds)));
            Result.Add(new KeyValuePair<string, string>("leadcreatedproduct", string.Join(",", LeadCreatedProducts)));

            foreach (var field in dict.Keys)
            {
                Result.Add(new KeyValuePair<string, string>(field, Server.UrlDecode(dict[field])));
            }

            if (FESession.Get(FESessionId, "FormLeadUrl") != null)
            {
                string[] formLeadUrl = (string[])FESession.Get(FESessionId, "FormLeadUrl");
                if (formLeadUrl != null && formLeadUrl.Length > 0)
                {
                    Result.Add(new KeyValuePair<string, string>("FormLeadUrl", formLeadUrl[0]));
                }
            }
                
            if (FESession.Get(FESessionId, "LeadSourceUrl") != null)
            {
                string[] leadSourceUrl = (string[])FESession.Get(FESessionId, "LeadSourceUrl");
                if (leadSourceUrl != null && leadSourceUrl.Length > 0)
                {
                    Result.Add(new KeyValuePair<string, string>("LeadSourceUrl", leadSourceUrl[0]));
                }
            }
            
            return Result;
        }

        public  Dictionary<string, string> GetMergedDictionary(Dictionary<string, string> firstDict, Dictionary<string, string> secondDict)
        {
            if (firstDict == null && secondDict == null)
            {
                throw new Exception();
            }

            // FirstDict is prioritized..
            Dictionary<string, string> mergedDictionary = firstDict != null
                ? new Dictionary<string, string>(firstDict, StringComparer.InvariantCultureIgnoreCase)
                : new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            // Iterate through SecondDict and add KVP's if they don't already exist in the FirstDict..
            foreach (string key in secondDict.Keys)
            {
                if (!mergedDictionary.ContainsKey(key))
                {
                    mergedDictionary.Add(key, secondDict[key]);
                }
                else if (mergedDictionary.ContainsKey(key) && string.IsNullOrEmpty(mergedDictionary[key]))
                {
                    mergedDictionary[key] = secondDict[key];
                }
            }

            return mergedDictionary;
        }


    }
}