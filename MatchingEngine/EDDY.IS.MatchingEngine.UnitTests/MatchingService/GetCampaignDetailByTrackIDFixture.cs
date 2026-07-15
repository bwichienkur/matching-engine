using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.UnitTests.MatchingService
{
    [TestClass]
    public class GetCampaignDetailByTrackIDFixture
    {
        IMatchingService matchingService = new EDDY.IS.MatchingEngine.Service.MatchingService();

        //[AssemblyInitialize] //Per Assembly
        [ClassInitialize] //Per Class
        //[TestInitialize] //Per Test Method
        public static void Setup(TestContext testContext)
        {
            StaticCacheProxyHost.CacheProxy.PreloadEntireCache();
        }

        [TestMethod]
        public void GetCampaignDetailByTrackID_Should_Not_Explode()
        {
            //Arrange
            Guid testTrackId = new Guid("FEFAC89D-6CA3-41C0-A940-56D14926BCFA");
        
            //Act
            CampaignDetailResponse response = matchingService.GetCampaignDetailByTrackID(testTrackId);

            //Assert           
        }
    }
}
