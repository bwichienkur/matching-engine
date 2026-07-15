using EDDY.IS.MatchingEngine;
using EDDY.IS.MatchingEngine.Service;
using EDDY.IS.SEOAllocation.Console.Data.Services;
using EDDY.IS.SEOAllocation.Console.DTO;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.SEOAllocation.Console.Model
{
    public class AllocationProcessManager : IAllocationProcessManager
    {
        protected IEddyLoggingDataService _loggingDataService;
        protected IEddyTrackingDataService _trackingDataService;

        public AllocationProcessManager(IEddyLoggingDataService loggingDataService, IEddyTrackingDataService trackingDataService)
        {
            _loggingDataService = loggingDataService;
            _trackingDataService = trackingDataService;
        }
        public void ProcessAllocation()
        {
            try
            {
                //Loading ME Cache
                StaticCacheProxyHost.CacheProxy.PreloadEntireCache();                

                //Initial setup
                AllocationWorkingData workingData = new AllocationWorkingData();                
                workingData.eRPLList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, eRPL>>(MatchingCacheItem.SRAeRPL);

                //Make call to GS site
                string gsURL = ConfigurationManager.AppSettings["GS_URL"];
                RestClient client = new RestClient(gsURL);
                RestRequest nodeRequest = new RestRequest("/sitemap/get-nodes", Method.GET);
                var response = client.Execute<Dictionary<int, gsNode>>(nodeRequest);
                workingData.DrupalNodes = new SortedDictionary<int, gsNode>(response.Data);

                //Start process
                Parallel.ForEach(workingData.DrupalNodes, new ParallelOptions { MaxDegreeOfParallelism = 2 }, node =>
                {
                    DrupalNodeProcessStep initialStep = new DrupalNodeProcessStep(node.Key, node.Value);
                    initialStep.DoProcessStep(workingData, _loggingDataService, _trackingDataService);
                });
            }
            catch(Exception ex)
            {
                _loggingDataService.LogException(ex);
            }            
        }
    }
}
