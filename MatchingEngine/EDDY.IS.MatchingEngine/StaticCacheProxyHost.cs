using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public static class StaticCacheProxyHost
    {
        private static Lazy<MatchingEngineCache> _cacheProxy = new Lazy<MatchingEngineCache>();

        public static MatchingEngineCache CacheProxy
        {
            get
            {
                return _cacheProxy.Value;
            }
        }
    }

    public static class StaticSettings
    {
        public static bool IsBeta
        {
            get
            {
                if (ConfigurationManager.AppSettings["IsBeta"] == "false")
                    return false;
                else
                    return true;
            }
        }
    }
}
