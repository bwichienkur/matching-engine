using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDDY.IS.MatchingEngine.DTO;
using System.Configuration;
using System.Collections.Generic;
using EDDY.IS.MatchingEngine.Service;
using EDDY.IS.MatchingEngine;
using EDDY.IS.SEOAllocation.Console.Data;
using EDDY.IS.SEOAllocation.Console.Enum;
using EDDY.IS.SEOAllocation.Console.DTO;
using EDDY.IS.SEOAllocation.Console.Model;
using EDDY.IS.SEOAllocation.Console.Data.Services;
using EDDY.IS.SEOAllocation.UnitTests.Mocks;

namespace EDDY.IS.SEOAllocation.UnitTests
{
    [TestClass]
    public class DrupalNodeProcessStepFixture
    {
        [AssemblyInitialize] //Per Assembly
        //[ClassInitialize] //Per Class
        //[TestInitialize] //Per Test Method
        public static void Setup(TestContext testContext)
        {
            StaticCacheProxyHost.CacheProxy.PreloadEntireCache();
        }

        [TestMethod]
        public void DrupalNodeProcessStep_DoProcessStep_ShouldOnlyReturnOnlineInstitutionsForOnlineNodes()
        {
            //Arrange
            gsNode node = new gsNode();
            node.campus_type = new List<int>() { 1 };
            node.category = new List<int>() { 20 };
            node.subject = new List<int>();
            node.specialty = new List<int>();
            node.level = new List<int>() { 11 };
            node.url = "https://www.gradschools.com/certificate/fine-arts-design/online";

            AllocationWorkingData workingData = new AllocationWorkingData();
            workingData.eRPLList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, eRPL>>(MatchingCacheItem.SRAeRPL);

            IEddyLoggingDataService loggingDataService = new MockLoggingDataService();
            IEddyTrackingDataService trackingDataService = new MockTrackingDataService();

            //Act
            IAllocationProcessStep drupalNodeProcessStep = new DrupalNodeProcessStep(22502, node);
            drupalNodeProcessStep.DoProcessStep(workingData, loggingDataService, trackingDataService);

            //Assert
            GSAllocationMaster allocationMaster = ((MockTrackingDataService)trackingDataService).Allocations.FirstOrDefault();
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 1).Count() >= 0);
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 2).Count() == 0);
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 3).Count() == 0);
        }


        [TestMethod]
        public void DrupalNodeProcessStep_DoProcessStep_ShouldOnlyReturnGroundInstitutionsForOnCampusNodes()
        {
            //Arrange
            gsNode node = new gsNode();
            node.campus_type = new List<int>() { 2 };
            node.category = new List<int>() { 20 };
            node.subject = new List<int>();
            node.specialty = new List<int>();
            node.level = new List<int>() { 11 };
            node.url = "https://www.gradschools.com/certificate/fine-arts-design/on-campus";

            AllocationWorkingData workingData = new AllocationWorkingData();
            workingData.eRPLList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, eRPL>>(MatchingCacheItem.SRAeRPL);

            IEddyLoggingDataService loggingDataService = new MockLoggingDataService();
            IEddyTrackingDataService trackingDataService = new MockTrackingDataService();

            //Act
            IAllocationProcessStep drupalNodeProcessStep = new DrupalNodeProcessStep(22502, node);
            drupalNodeProcessStep.DoProcessStep(workingData, loggingDataService, trackingDataService);

            //Assert
            GSAllocationMaster allocationMaster = ((MockTrackingDataService)trackingDataService).Allocations.FirstOrDefault();
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 1).Count() == 0);
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 2).Count() >= 0);
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 3).Count() == 0);
        }

        [TestMethod]
        public void DrupalNodeProcessStep_DoProcessStep_ShouldReturnOnlineAndGroundInstitutionsForHybridNodes()
        {
            //Arrange
            gsNode node = new gsNode();
            node.campus_type = new List<int>() { 3 };
            node.category = new List<int>() { 20 };
            node.subject = new List<int>();
            node.specialty = new List<int>();
            node.level = new List<int>() { 11 };
            node.url = "https://www.gradschools.com/certificate/fine-arts-design/hybrid";

            AllocationWorkingData workingData = new AllocationWorkingData();
            workingData.eRPLList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, eRPL>>(MatchingCacheItem.SRAeRPL);

            IEddyLoggingDataService loggingDataService = new MockLoggingDataService();
            IEddyTrackingDataService trackingDataService = new MockTrackingDataService();

            //Act
            IAllocationProcessStep drupalNodeProcessStep = new DrupalNodeProcessStep(22502, node);
            drupalNodeProcessStep.DoProcessStep(workingData, loggingDataService, trackingDataService);

            //Assert
            GSAllocationMaster allocationMaster = ((MockTrackingDataService)trackingDataService).Allocations.FirstOrDefault();
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 1).Count() >= 0);
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 2).Count() >= 0);
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 3).Count() >= 0);
        }

        [TestMethod]
        public void DrupalNodeProcessStep_DoProcessStep_ShouldReturnOnlineAndGroundInstitutionsForNonSpecifiedNodes()
        {
            //Arrange
            gsNode node = new gsNode();
            node.category = new List<int>() { 20 };
            node.subject = new List<int>();
            node.specialty = new List<int>();
            node.level = new List<int>() { 11 };
            node.url = "https://www.gradschools.com/certificate/fine-arts-design";

            AllocationWorkingData workingData = new AllocationWorkingData();
            workingData.eRPLList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, eRPL>>(MatchingCacheItem.SRAeRPL);

            IEddyLoggingDataService loggingDataService = new MockLoggingDataService();
            IEddyTrackingDataService trackingDataService = new MockTrackingDataService();

            //Act
            IAllocationProcessStep drupalNodeProcessStep = new DrupalNodeProcessStep(22502, node);
            drupalNodeProcessStep.DoProcessStep(workingData, loggingDataService, trackingDataService);

            //Assert
            GSAllocationMaster allocationMaster = ((MockTrackingDataService)trackingDataService).Allocations.FirstOrDefault();
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 1).Count() >= 0);
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 2).Count() >= 0);
            Assert.IsTrue(allocationMaster.GSAllocationDetails.Where(x => x.ProgramCampusTypeId == 3).Count() >= 0);
        }
    }
}
