using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.MatchingEngine.Service.App_Code
{
    public class MatchingEngineStartup
    {
        public static void AppInitialize()
        {
            StaticCacheProxyHost.CacheProxy.PreloadEntireCache();
        }
    }
}