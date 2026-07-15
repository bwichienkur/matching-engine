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
    public class GetFormProgramsFixtures
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
        public void MatchingService_GetFormPrograms_ShouldReturnConsolidatedListOfProgramsForMedaille()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.TrackGuid = new Guid("F0FD9FA0-39D8-408C-9F22-0968DA3EFEB5");
            matchRequest.InstitutionIdList = new int[] { 2642 }.ToList();
            matchRequest.LeadCreationType = LeadCreationType.InstitutionFormInitial;
            matchRequest.ApplicationId = 27;
            
            //Act
            FormProgramResponse response = matchingService.GetFormPrograms(matchRequest);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }
    }
}
