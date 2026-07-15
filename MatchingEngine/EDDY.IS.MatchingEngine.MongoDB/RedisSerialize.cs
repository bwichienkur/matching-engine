using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.MongoDB
{
    public class RedisSerialize
    {
        protected static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

        public static bool SaveCacheItem<T>(string key, T item)
        {
            var serializer = new NewtonsoftSerializer();
            var cacheClient = new StackExchangeRedisCacheClient(redis, serializer);

            if (!cacheClient.Exists(key))
                cacheClient.Add(key, item);

            return true;
        }

        public static T GetCacheItem<T>(string key)
        {
            var serializer = new NewtonsoftSerializer();
            var cacheClient = new StackExchangeRedisCacheClient(redis, serializer);

            return cacheClient.Get<T>(key);
        }
    }
}
