using EDDY.IS.Common.ExceptionHandler;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.DataAccess
{
    public static class RedisHelper
    {

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConfigurationManager.AppSettings["RedisConnection"];

            return ConnectionMultiplexer.Connect(cacheConnection);
        });


        /// <summary>
        /// for email pings the key will be the email
        /// for phone pings the key will be the phone
        /// for score pings the key will be the necessary prospect info hashed together
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string FormatCacheKey(string key)
        {
            return string.Format("{0}|{1}", ConfigurationManager.AppSettings["RedisPrefix"], key).ToLower();
        }

        /// <summary>
        /// the value we set will be the entire response in whatever format we get it (xml/json) 
        /// </summary>
        /// <param name="key">unique user identifier for this cache key</param>
        /// <param name="value"></param>
        public static void SetInCache(string key, string value)
        {
            IDatabase cache = null;
            try
            {
                if (ConfigurationManager.AppSettings["UseRedisCaching"] == "true")
                {
                    string cachekey = FormatCacheKey(key);
                    cache = lazyConnection.Value.GetDatabase();
                    cache.StringSet(cachekey, value, TimeSpan.FromMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["RedisCacheDurationInMinutes"])));
                }
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">unique user identifier for this cache key</param>
        /// <returns></returns>
        public static string GetFromCache(string key)
        {
            IDatabase cache = null;
            string cachekey = FormatCacheKey(key);
            string res = string.Empty;
            try
            {
                if (ConfigurationManager.AppSettings["UseRedisCaching"] == "true")
                {
                    cache = lazyConnection.Value.GetDatabase();
                    res = cache.StringGet(cachekey);
                }
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }

            return res;
        }
    }
}
