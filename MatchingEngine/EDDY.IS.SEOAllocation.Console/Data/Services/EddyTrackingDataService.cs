using EDDY.IS.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.Console.Data.Services
{
    public class EddyTrackingDataService : IEddyTrackingDataService
    {        
        public async Task StoreAllocationRecord(GSAllocationMaster allocationMaster)
        {
            try
            {
                using (var context = new GSAllocationContextContainer())
                {
                    context.GSAllocationMasters.Add(allocationMaster);
                    await context.SaveChangesAsync();
                }
            }
            catch(System.Exception ex)
            {
                ISException isException = new ISException(ex);
                isException.Save();
            }
        }
    }
}
