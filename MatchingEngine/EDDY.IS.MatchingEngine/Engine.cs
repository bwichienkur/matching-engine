using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Core.Logging.DataModel;

namespace EDDY.IS.MatchingEngine
{
    public class Engine
    {
        protected EDDY.IS.Core.Logging.PerformanceLog _performanceLog;

        public Engine() { }

        public Engine(EDDY.IS.Core.Logging.PerformanceLog pLog)
        {
            _performanceLog = pLog;
        }

        protected void StartLogDetail(string methodName)
        {
            if (_performanceLog != null)
            {
                _performanceLog.StartLogDetail(methodName);
            }
        }

        protected void EndLogDetail()
        {
            if (_performanceLog != null)
            {
                _performanceLog.EndLogDetail();
            }
        }
    }
}
