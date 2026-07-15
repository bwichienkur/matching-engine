using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class MetaDataRepository : IMetaDataRepository
    {
        private readonly ResourceMetaDataService _resourceMetaDataService = new ResourceMetaDataService();
        private readonly int _cacheExpirationTime = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheResourceMetaDatasTimeMinutes"));

        public string GetMetaDataMessageByKey(string key)
        {
            return  _resourceMetaDataService.GetResourceMetaDataTextForKey(key);
        }

        public Dictionary<string, string> GetMetaDataMessagesByPrefix(string prefix)
        {
            string metaDataCacheKey = $"{prefix}_METADATA";

            Dictionary<string, string> renderingStrategyMetaData = GetMetaDataMessagesFromCache(metaDataCacheKey);

            if (renderingStrategyMetaData == null)
            {
                renderingStrategyMetaData = GetMetaDataMessagesByPrefixAndAddToCache(prefix, metaDataCacheKey);
            }

            return renderingStrategyMetaData;
        }

        private Dictionary<string, string> GetMetaDataMessagesByPrefixAndAddToCache(string prefix, string metaDataCacheKey)
        {
            AddMetaDataMessagesToCache(prefix, metaDataCacheKey);
            return GetMetaDataMessagesFromCache(metaDataCacheKey);
        }

        private void AddMetaDataMessagesToCache(string prefix, string metaDataCacheKey)
        {
            FormsEngineCacheProxy.Cache.Set(metaDataCacheKey, RetrieveMetaDataByPrefix(prefix), _cacheExpirationTime);
        }

        private Dictionary<string, string> GetMetaDataMessagesFromCache(string metaDataCacheKey)
        {
            return FormsEngineCacheProxy.Cache.Get<Dictionary<string, string>>(metaDataCacheKey);
        }

        public Dictionary<string, string> RetrieveMetaDataByPrefix(string prefix)
        {
            Dictionary<string, string> metaDataForRenderingStrategy = new Dictionary<string, string>();
            Dictionary<string, string> resourceMetaData = _resourceMetaDataService.GetResourceMetaData();

            if (resourceMetaData?.Count() > 0)
            {
                metaDataForRenderingStrategy = resourceMetaData.Where(m => m.Key.StartsWith(prefix)).ToDictionary(m => m.Key, m => m.Value);
            }

            return metaDataForRenderingStrategy;
        }
    }
}
