using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.Console.Data.Services
{
    public interface IEddyTrackingDataService
    {
        Task StoreAllocationRecord(GSAllocationMaster allocationMaster);
    }
}
