using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;

namespace EDDY.IS.FormsEngine
{
    /// <summary>
    /// Session emulation using cache
    /// Can be replaced with a different store provider
    /// </summary>
    public class FESession
    {
        private static string CACHEDSESSIONDICTIONARY_KEY = "FE.CACHEDDICTIONARY.{0}";

        public static void Set(string FESessionId, string Key, object value)
        {
            int SessionMinutes = 20;
            if (!int.TryParse(ConfigurationManager.AppSettings.Get("FormsEngineSessionMinutes"), out SessionMinutes))
            {
                SessionMinutes = 20;
            }
            ConcurrentDictionary<string, object> cacheDictionary = GetDictionary(FESessionId);
            cacheDictionary.AddOrUpdate(Key, value, (k, v) => value);
            HttpRuntime.Cache.Insert(string.Format(CACHEDSESSIONDICTIONARY_KEY, FESessionId), cacheDictionary, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(SessionMinutes));
        }

        public static object Get(string FESessionId, string Key)
        {
            ConcurrentDictionary<string, object> cacheDictionary = GetDictionary(FESessionId);
            object val;
            cacheDictionary.TryGetValue(Key, out val);
            return val;
        }

        public static bool ContainsKey(string FESessionId, string Key)
        {
            ConcurrentDictionary<string, object> cacheDictionary = GetDictionary(FESessionId);
            return cacheDictionary.ContainsKey(Key);
        }

        public static T Get<T>(string FESessionId, string Key)
        {
            ConcurrentDictionary<string, object> cacheDictionary = GetDictionary(FESessionId);
            object val;
            if (cacheDictionary.TryGetValue(Key, out val))
            {
                return (T)val;
            }
            return default;
        }

        public static ConcurrentDictionary<string, object> GetDictionary(string FESessionId)
        {
            ConcurrentDictionary<string, object> cacheDictionary = (ConcurrentDictionary<string, object>)HttpRuntime.Cache[string.Format(CACHEDSESSIONDICTIONARY_KEY, FESessionId)];
            if (cacheDictionary == null)
            {
                cacheDictionary = new ConcurrentDictionary<string, object>();
            }
            return cacheDictionary;
        }

        public static void Remove(string FESessionId, string Key)
        {
            ConcurrentDictionary<string, object> cacheDictionary = GetDictionary(FESessionId);
            object val;
            cacheDictionary.TryRemove(Key, out val);
            int SessionMinutes = 20;
            if (!int.TryParse(ConfigurationManager.AppSettings.Get("FormsEngineSessionMinutes"), out SessionMinutes))
            {
                SessionMinutes = 20;
            }
            HttpRuntime.Cache.Insert(string.Format(CACHEDSESSIONDICTIONARY_KEY, FESessionId), cacheDictionary, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(SessionMinutes));
        }
    }
}