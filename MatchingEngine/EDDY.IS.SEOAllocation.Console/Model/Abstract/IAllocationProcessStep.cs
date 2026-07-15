using EDDY.IS.SEOAllocation.Console.Data.Services;
using EDDY.IS.SEOAllocation.Console.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.Console.Model
{
    public interface IAllocationProcessStep
    {        
        void DoProcessStep(AllocationWorkingData workingData, IEddyLoggingDataService loggingDataService, IEddyTrackingDataService trackingDataService);
    }
}
