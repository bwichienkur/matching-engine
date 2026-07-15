using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.SEOAllocation.Console.DTO;
using EDDY.IS.SEOAllocation.Console.Data.Services;
using EDDY.IS.SEOAllocation.Console.Data;

namespace EDDY.IS.SEOAllocation.Console.Model
{
    public class RecordAllocationDetailsStep : IAllocationProcessStep
    {        
        protected GSAllocationMaster AllocationMaster { get; set; }
      
        public RecordAllocationDetailsStep(GSAllocationMaster allocationMaster)
        {         
            AllocationMaster = allocationMaster;          
        }

        public void DoProcessStep(AllocationWorkingData workingData, IEddyLoggingDataService loggingDataService, IEddyTrackingDataService trackingDataService)
        {
            try
            {
                trackingDataService.StoreAllocationRecord(AllocationMaster);
            }
            catch (System.Exception ex)
            {
                loggingDataService.LogException(ex);
            }
        }
    }
}
