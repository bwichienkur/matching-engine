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
    public class GetSubjectsAllIfNoneFixture
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
        public void MatchingService_GetSubjectsAllIfNone_ShouldReturnPaidFraidAndFreeGradSchoolsForNoPaidStatusTypeSearch()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.RemoveInvalidEntities = true;
            
            //Act
            SubjectResponse response = matchingService.GetSubjectsAllIfNone(matchRequest);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetSubjectsAllIfNone_ShouldReturnPaidOnlyGradSchoolsForPaidOnlySearch()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Paid);
            
            //Act
            SubjectResponse response = matchingService.GetSubjectsAllIfNone(matchRequest);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetSubjectsAllIfNone_ShouldReturnFraidOnlyGradSchoolsForFraidOnlySearch()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Fraid);
            
            //Act
            SubjectResponse response = matchingService.GetSubjectsAllIfNone(matchRequest);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetSubjectsAllIfNone_ShouldReturnFreeOnlyGradSchoolsForFreeOnlySearch()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Free);

            //Act
            SubjectResponse response = matchingService.GetSubjectsAllIfNone(matchRequest);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetSubjectsAllIfNone_ShouldReturnPaidAndFraidOnlyGradSchoolsForPaidAndFraidOnlySearch()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Paid);
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Fraid);
            
            //Act
            SubjectResponse response = matchingService.GetSubjectsAllIfNone(matchRequest);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetSubjectsAllIfNone_ShouldReturnFraidAndFreeOnlyGradSchoolsForFraidAndFreeOnlySearch()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Fraid);
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Free);

            //Act
            SubjectResponse response = matchingService.GetSubjectsAllIfNone(matchRequest);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetSubjectsAllIfNone_ShouldReturnPaidAndFreeOnlyGradSchoolsForPaidAndFreeOnlySearch()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Paid);
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Free);

            //Act
            SubjectResponse response = matchingService.GetSubjectsAllIfNone(matchRequest);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }
    }
}
