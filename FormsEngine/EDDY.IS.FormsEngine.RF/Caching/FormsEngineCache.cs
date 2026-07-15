using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using EDDY.IS.LocalCache;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.FormsEngine.Caching
{
    public class FormsEngineCache : LocalCacheBase
    {
        private Dictionary<string, Tuple<int?, Func<object>>> PopulateMethods = new Dictionary<string, Tuple<int?, Func<object>>>(StringComparer.OrdinalIgnoreCase);

        public override void PreloadEntireCache()
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            return (T)Get(key);
        }

        public void Set(string key, Func<object> populateFunc)
        {
            int? expirationMinutes = GetCacheItemExpiration(key);
            key = Constants.CACHE_ITEM_PREFIX + key;

            if (!PopulateMethods.ContainsKey(key))
            {
                PopulateMethods.Add(key, new Tuple<int?, Func<object>>(expirationMinutes, populateFunc));
            }

            if (expirationMinutes.HasValue)
            {
                appCache.Insert(key, populateFunc.Invoke(), null, DateTime.Now.AddMinutes((int)expirationMinutes), Cache.NoSlidingExpiration, new CacheItemUpdateCallback(CacheItemUpdateCallback));
            }
            else
            {
                appCache.Insert(key, populateFunc.Invoke());
            }
        }

        public void Set(string key, Func<object> populateFunc, int ExpirationMinutes)
        {
            key = Constants.CACHE_ITEM_PREFIX + key;
            if (!PopulateMethods.ContainsKey(key))
            {
                PopulateMethods.Add(key, new Tuple<int?, Func<object>>(ExpirationMinutes, populateFunc));
            }
            appCache.Insert(key, populateFunc.Invoke(), null, DateTime.Now.AddMinutes(ExpirationMinutes), Cache.NoSlidingExpiration, new CacheItemUpdateCallback(CacheItemUpdateCallback));

        }

        public void Set(string key, object value, int expirationMinutes, CacheItemUpdateCallback callbackFunc)
        {
            key = Constants.CACHE_ITEM_PREFIX + key;
            appCache.Insert(key, value, null, DateTime.Now.AddMinutes(expirationMinutes), Cache.NoSlidingExpiration, new CacheItemUpdateCallback(CacheItemUpdateCallback));
        }

        /// <summary>
        /// Quick expiring entry no refresh
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationMinutes"></param>
        new public void Set(string key, object value, int expirationMinutes)
        {
            key = Constants.CACHE_ITEM_PREFIX + key;
            appCache.Insert(key, value, null, DateTime.Now.AddMinutes(expirationMinutes), Cache.NoSlidingExpiration);
        }

        new public void Set(string key, object value)
        {
            key = Constants.CACHE_ITEM_PREFIX + key;
            appCache.Insert(key, value);
        }

        new public object Get(string key)
        {
            object cacheItem = null;
            try
            {
                cacheItem = appCache.Get(Constants.CACHE_ITEM_PREFIX + key);
            }
            catch { }
            return cacheItem;
        }


        private void CacheItemUpdateCallback(string key, CacheItemUpdateReason reason, out object expensiveObject, out CacheDependency dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
        {
            EDDY.IS.Core.Logging.PerformanceLog p = new PerformanceLog(Base.ISApplication.FormsEngine, "Refreshing - " + key, null, null);
            object newObject = null;
            dependency = null;
            slidingExpiration = Cache.NoSlidingExpiration;
            absoluteExpiration = DateTime.Now.AddMinutes((int)Constants.CACHE_EXCEPTION_DEFAULT_EXPIRATION);

            try
            {
                absoluteExpiration = DateTime.Now.AddMinutes((int)PopulateMethods[key].Item1);
                newObject = PopulateMethods[key].Item2.Invoke();
            }
            catch (Exception ex)
            {
                try
                {
                    expensiveObject = this.Get(key);
                }
                catch { }
                ISException isEx = new ISException(ex);
                isEx.Save();

            }
            expensiveObject = newObject;
            p.EndLog(null);
        }

    }
}
