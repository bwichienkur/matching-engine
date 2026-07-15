using EDDY.IS.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.Console.Data.Services
{
    public class EddyLoggingDataService : IEddyLoggingDataService
    {
        public void LogException(System.Exception ex)
        {
            ISException isException = new ISException(ex);
            isException.Save();
        }
    }
}
