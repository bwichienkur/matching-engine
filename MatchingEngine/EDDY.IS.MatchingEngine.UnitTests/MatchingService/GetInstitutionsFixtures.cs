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
    public class GetInstitutionsFixtures
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
        public void MatchingService_GetInstitutions_ShouldReturnListOfInstitutions()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.ApplicationId = 20;
            matchRequest.ProgramTypeList = new List<ProgramType>() { ProgramType.FullDegree };
            matchRequest.TrackGuid = new Guid("2308d190-b71c-49e0-be16-fe739beb3648");
            matchRequest.InstitutionIdList = new int[] { 2642 }.ToList();            

            //Act
            InstitutionResponse response = matchingService.GetInstitutions(matchRequest, GetInstitutionCampusOption.NoCampus);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetInstitutions_InstitutionReturnCountShouldMatchActualInstitutionsReturnedWhenNoPagesPassedIn()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 15;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.MaxNestedProgramCount = 10;
            matchRequest.CampusType = CampusType.Online;
            matchRequest.ProgramLevelList = new List<int>() { 8, 11, 7 };
            matchRequest.CategoryList = new List<int>() { 21, 23, 27 };
            matchRequest.SubjectList = new List<int>() { 614 };
            matchRequest.SpecialtyList = new List<int>() { 1211 };
            matchRequest.IncludeProgramGroupRollup = true;

            //Act
            InstitutionResponse response = matchingService.GetInstitutions(matchRequest, GetInstitutionCampusOption.CampusOn2ndLevel);

            //Assert            
            Assert.IsTrue(response.ResultCount <= matchRequest.MaxResultsCount);
        }

        [TestMethod]
        public void MatchingService_GetInstitutions_InstitutionReturnCountShouldBeAtleastGreaterThanActualInstitutionsReturnedWhenMultiPagesPassedIn()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 1;
            matchRequest.MaxResultsCount = 10;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.MaxNestedProgramCount = 10;
            matchRequest.CampusType = CampusType.Online;
            matchRequest.ProgramLevelList = new List<int>() { 8, 11, 7 };
            matchRequest.CategoryList = new List<int>() { 21, 23, 27 };
            matchRequest.SubjectList = new List<int>() { 614 };
            matchRequest.SpecialtyList = new List<int>() { 1211 };
            matchRequest.IncludeProgramGroupRollup = true;

            //Act
            InstitutionResponse response = matchingService.GetInstitutions(matchRequest, GetInstitutionCampusOption.CampusOn2ndLevel);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }
    }
}
