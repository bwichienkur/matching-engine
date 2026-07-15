using System;
using System.Runtime.Caching;
using System.Configuration;

namespace EDDY.IS.Vendor.Utilities
{
    public static class CacheStore
    {
        public static event EventHandler<CacheItemUpdateEventArgs> CacheUpdated;
     
        public static void AddResponseDataCacheItem(string key, Object cacheItem)
        {

            if (!String.IsNullOrEmpty(key) && cacheItem != null)
            {
                int cacheDuration = 15;
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("ResponseDataCacheMinuteDurationValue")))
                {
                    cacheDuration = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ResponseDataCacheMinuteDurationValue"));
                }
                MemoryCache.Default.Set(key, cacheItem, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheDuration), UpdateCallback = CacheStoreUpdate });
            }
        }
        public static void AddSupportingDataCacheItem(string key, Object cacheItem)
        {

            if (!String.IsNullOrEmpty(key) && cacheItem != null)
            {
                int cacheDuration = 24;
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("SupportingDataCacheHourDurationValue")))
                {
                    cacheDuration = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SupportingDataCacheHourDurationValue"));
                }
                MemoryCache.Default.Set(key, cacheItem, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(cacheDuration), UpdateCallback = CacheStoreUpdate });
            }
        }

        public static Object GetCacheItemByKey(string key)
        {
            Object cacheObject = null;
            if (!String.IsNullOrEmpty(key))
            {
                if (MemoryCache.Default.Contains(key))
                {
                    cacheObject = MemoryCache.Default.Get(key);
                }
            }
            return cacheObject;
        }

            public static void RemoveCacheItemByKey(string key)
            {
                if (!String.IsNullOrEmpty(key))
                {
                    if (MemoryCache.Default.Contains(key))
                    {
                        MemoryCache.Default.Remove(key);
                    }
                }
            }

        private static void CacheStoreUpdate(CacheEntryUpdateArguments args)
        {
            EventHandler<CacheItemUpdateEventArgs> handler = CacheUpdated;
            if (handler != null)
            {
                CacheItemUpdateEventArgs cacheItemUpdateEventArgs = new CacheItemUpdateEventArgs();
                cacheItemUpdateEventArgs.CacheKey = args.Key;
                cacheItemUpdateEventArgs.RemovedReason = args.RemovedReason.ToString();
                handler(null, cacheItemUpdateEventArgs);
            }
        }
    }
    public class CacheItemUpdateEventArgs : EventArgs
    {
        public String CacheKey { get; set; }
        public String RemovedReason { get; set; }
    }
}
