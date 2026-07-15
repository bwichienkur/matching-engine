using EDDY.IS.SEOAllocation.Console.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.UnitTests.Mocks
{
    public class MockLoggingDataService : IEddyLoggingDataService
    {
        public IList<Exception> Exceptions = new List<Exception>();

        public void LogException(Exception ex)
        {
            Exceptions.Add(ex);
        }
    }
}

