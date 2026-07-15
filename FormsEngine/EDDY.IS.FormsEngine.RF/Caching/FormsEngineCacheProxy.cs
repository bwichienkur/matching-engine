using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Caching
{
    public static class FormsEngineCacheProxy
    {
        private static Lazy<FormsEngineCache> _cacheProxy = new Lazy<FormsEngineCache>();

        public static FormsEngineCache Cache
        {
            get 
            {
                return _cacheProxy.Value;
            }
        }
    }
}
