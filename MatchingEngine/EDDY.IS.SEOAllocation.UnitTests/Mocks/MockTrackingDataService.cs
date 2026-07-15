using EDDY.IS.SEOAllocation.Console.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.SEOAllocation.Console.Data;

namespace EDDY.IS.SEOAllocation.UnitTests.Mocks
{
    public class MockTrackingDataService : IEddyTrackingDataService
    {
        public IList<GSAllocationMaster> Allocations = new List<GSAllocationMaster>();

        public async Task StoreAllocationRecord(GSAllocationMaster allocationMaster)
        {
            await Task.Factory.StartNew(() => Allocations.Add(allocationMaster));            
        }
    }
}
