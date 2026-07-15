using EDDY.IS.MatchingEngine;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.Service;
using EDDY.IS.SEOAllocation.Console.Data;
using EDDY.IS.SEOAllocation.Console.Data.Services;
using EDDY.IS.SEOAllocation.Console.DTO;
using EDDY.IS.SEOAllocation.Console.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IEddyLoggingDataService loggingDataService = new EddyLoggingDataService();
            IEddyTrackingDataService trackingDataService = new EddyTrackingDataService();
            IAllocationProcessManager allocationProcessManager = new AllocationProcessManager(loggingDataService, trackingDataService);
            allocationProcessManager.ProcessAllocation();
        }
    }
}
