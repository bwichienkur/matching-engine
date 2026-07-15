using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.UnitTests.MatchingService
{
    [TestClass]
    public class GetFacetedNavigationFixture
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
        public void MatchingService_GetFacetedNavigation_ShouldReturnCitiesInEnglandWhenPassedEngland()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 20;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("2308D190-B71C-49E0-BE16-FE739BEB3648");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramTypeList = new List<ProgramType>();
            matchRequest.ProgramTypeList.Add(ProgramType.StudyAbroad);
            matchRequest.GeoTarget = new GeoTarget() { CountryList = new List<int>() { 80 } };
            matchRequest.SFProductCodes = new List<SFProductCode>();
            matchRequest.SFProductCodes.Add(SFProductCode.SAB_CPL);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            Assert.AreEqual(1, response.Countries.Count());
            Assert.IsTrue(response.Countries.Any(x => x.CountryName.Equals("United Kingdom")));
            Assert.IsTrue(response.Countries.Any(x => x.CountryId == 80));
            //Test State List
            Assert.AreEqual(3, response.States.Count());
            Assert.IsTrue(response.States.Any(x => x.StateName.Equals("Scotland")));
            Assert.IsTrue(response.States.Any(x => x.StateId == 1031));
            Assert.IsTrue(response.States.Any(x => x.StateName.Equals("Northern I")));
            Assert.IsTrue(response.States.Any(x => x.StateId == 1030));
            Assert.IsTrue(response.States.Any(x => x.StateName.Equals("England")));
            Assert.IsTrue(response.States.Any(x => x.StateId == 1029));
            //Test City List                        
            Assert.AreEqual(11, response.Cities.Count());
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Belfast")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 13098));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Edinburgh")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 38813));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Glasgow")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 30640));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Leeds")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 58704));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Loddon")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 40588));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("London")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 91827));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Manchester")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 76240));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Ormskirk")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 77001));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Oxford")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 105990));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Reading")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 107593));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Stirling")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 117454));
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnCitiesInFranceWhenPassedFrance()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 20;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("2308D190-B71C-49E0-BE16-FE739BEB3648");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramTypeList = new List<ProgramType>();
            matchRequest.ProgramTypeList.Add(ProgramType.StudyAbroad);
            matchRequest.GeoTarget = new GeoTarget() { CountryList = new List<int>() { 78 } };
            matchRequest.SFProductCodes = new List<SFProductCode>();
            matchRequest.SFProductCodes.Add(SFProductCode.SAB_CPL);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            Assert.AreEqual(1, response.Countries.Count());
            Assert.IsTrue(response.Countries.Any(x => x.CountryName.Equals("France")));
            Assert.IsTrue(response.Countries.Any(x => x.CountryId == 78));
            //Test State List
            Assert.AreEqual(0, response.States.Count());
            //Test City List            
            Assert.AreEqual(14, response.Cities.Count());
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Aix-En-Provence")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 571));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Arles")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 5231));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Caen")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 22556));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Cannes")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 14962));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Dijon")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 45412));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Grenoble")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 32552));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Lille")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 39371));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Lyon")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 61970));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Marseille")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 89449));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Nantes")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 88566));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Nice")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 105272));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Paris")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 106777));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Reims")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 56009));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Riviere")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 56604));
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnCitiesInChinaWhenPassedChina()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 20;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("2308D190-B71C-49E0-BE16-FE739BEB3648");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramTypeList = new List<ProgramType>();
            matchRequest.ProgramTypeList.Add(ProgramType.StudyAbroad);
            matchRequest.GeoTarget = new GeoTarget() { CountryList = new List<int>() { 52 } };
            matchRequest.SFProductCodes = new List<SFProductCode>();
            matchRequest.SFProductCodes.Add(SFProductCode.SAB_CPL);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            Assert.AreEqual(1, response.Countries.Count());
            Assert.IsTrue(response.Countries.Any(x => x.CountryName.Equals("China")));
            Assert.IsTrue(response.Countries.Any(x => x.CountryId == 52));
            //Test State List
            Assert.AreEqual(0, response.States.Count());
            //Test City List            
            Assert.AreEqual(3, response.Cities.Count());
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Beijing")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 11862));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Shanghai")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 115872));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Suzhou")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 117938));
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnCitiesInGermanyWhenPassedGermany()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 20;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("2308D190-B71C-49E0-BE16-FE739BEB3648");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramTypeList = new List<ProgramType>();
            matchRequest.ProgramTypeList.Add(ProgramType.StudyAbroad);
            matchRequest.GeoTarget = new GeoTarget() { CountryList = new List<int>() { 60 } };
            matchRequest.SFProductCodes = new List<SFProductCode>();
            matchRequest.SFProductCodes.Add(SFProductCode.SAB_CPL);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            Assert.AreEqual(1, response.Countries.Count());
            Assert.IsTrue(response.Countries.Any(x => x.CountryName.Equals("Germany")));
            Assert.IsTrue(response.Countries.Any(x => x.CountryId == 60));
            //Test State List
            Assert.AreEqual(0, response.States.Count());
            //Test City List            
            Assert.AreEqual(4, response.Cities.Count());
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Berlin")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 7724));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Berlin")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 9763));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Freiburg")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 24985));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Reutlingen")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 109965));
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnMultiCountryWhenPassedMultiCountry()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 20;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("2308D190-B71C-49E0-BE16-FE739BEB3648");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramTypeList = new List<ProgramType>();
            matchRequest.ProgramTypeList.Add(ProgramType.StudyAbroad);
            matchRequest.GeoTarget = new GeoTarget() { CountryList = new List<int>() { 256 } };
            matchRequest.SFProductCodes = new List<SFProductCode>();
            matchRequest.SFProductCodes.Add(SFProductCode.SAB_CPL);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            Assert.AreEqual(1, response.Countries.Count());
            Assert.IsTrue(response.Countries.Any(x => x.CountryName.Equals("Multi-Country Programs")));
            Assert.IsTrue(response.Countries.Any(x => x.CountryId == 256));
            //Test State List
            Assert.AreEqual(0, response.States.Count());
            //Test City List            
            Assert.AreEqual(1, response.Cities.Count());
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Multi-Country Programs")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 133926));
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnAllCountriesAndCitiesWhenNoCountryPassedIn()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 20;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("2308D190-B71C-49E0-BE16-FE739BEB3648");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramTypeList = new List<ProgramType>();
            matchRequest.ProgramTypeList.Add(ProgramType.StudyAbroad);
            matchRequest.GeoTarget = null;
            matchRequest.SFProductCodes = new List<SFProductCode>();
            matchRequest.SFProductCodes.Add(SFProductCode.SAB_CPL);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            Assert.IsTrue(response.Countries.Count() > 0);
            //Test State List
            Assert.IsTrue(response.States.Count() > 0);
            //Test City List            
            Assert.IsTrue(response.Cities.Count() == 0);            
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnCitiesWhenEnglandPassedIn()
        {
            //http://www.studyabroad.com/landing-page?type=4&country=1029_s&city=91827
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 20;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("2308D190-B71C-49E0-BE16-FE739BEB3648");
            matchRequest.RemoveInvalidEntities = true;
            matchRequest.ProgramTypeList = new List<ProgramType>();
            matchRequest.ProgramTypeList.Add(ProgramType.StudyAbroad);
            matchRequest.GeoTarget = new GeoTarget() { CountryList = new List<int>() { 80 }, CityList = new List<int>() { 91827 } };
            matchRequest.SFProductCodes = new List<SFProductCode>();
            matchRequest.SFProductCodes.Add(SFProductCode.SAB_CPL);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);
            
            //Assert
            //Test Country List
            Assert.AreEqual(1, response.Countries.Count());
            Assert.IsTrue(response.Countries.Any(x => x.CountryName.Equals("United Kingdom")));
            Assert.IsTrue(response.Countries.Any(x => x.CountryId == 80));
            //Test State List
            Assert.AreEqual(1, response.States.Count());
            Assert.IsTrue(response.States.Any(x => x.StateName.Equals("England")));
            Assert.IsTrue(response.States.Any(x => x.StateId == 1029));
            //Test City List                        
            Assert.AreEqual(2, response.Cities.Count()); 
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("London")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 91827));
            Assert.IsTrue(response.Cities.Any(x => x.CityName.Equals("Manchester")));
            Assert.IsTrue(response.Cities.Any(x => x.CityId == 76240));
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnPaidFraidAndFreeCitiesWhenNoFilterPassedIn()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");

            matchRequest.ProgramLevelList = new List<int>();
            matchRequest.ProgramLevelList.Add(7);
            matchRequest.ProgramLevelList.Add(8);
            matchRequest.ProgramLevelList.Add(11);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            //Assert.AreEqual(1, response.Countries.Count());           
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnPaidFraidAndFreeCities()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");

            matchRequest.ProgramLevelList = new List<int>();
            matchRequest.ProgramLevelList.Add(7);
            matchRequest.ProgramLevelList.Add(8);
            matchRequest.ProgramLevelList.Add(11);

            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Free);
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Paid);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            //Assert.AreEqual(1, response.Countries.Count());           
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnPaidAndFreeCities()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");

            matchRequest.ProgramLevelList = new List<int>();
            matchRequest.ProgramLevelList.Add(7);
            matchRequest.ProgramLevelList.Add(8);
            matchRequest.ProgramLevelList.Add(11);

            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Free);
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Paid);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            //Assert.AreEqual(1, response.Countries.Count());           
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnFraidAndFreeCities()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");

            matchRequest.ProgramLevelList = new List<int>();
            matchRequest.ProgramLevelList.Add(7);
            matchRequest.ProgramLevelList.Add(8);
            matchRequest.ProgramLevelList.Add(11);

            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Free);
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Paid);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            //Assert.AreEqual(1, response.Countries.Count());           
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnPaidAndFraidCities()
        {            
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");

            matchRequest.ProgramLevelList = new List<int>();
            matchRequest.ProgramLevelList.Add(7);
            matchRequest.ProgramLevelList.Add(8);
            matchRequest.ProgramLevelList.Add(11);

            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Fraid);
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Paid);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            //Assert.AreEqual(1, response.Countries.Count());           
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnPaidCities()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");

            matchRequest.ProgramLevelList = new List<int>();
            matchRequest.ProgramLevelList.Add(7);
            matchRequest.ProgramLevelList.Add(8);
            matchRequest.ProgramLevelList.Add(11);

            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Paid);

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            //Assert.AreEqual(1, response.Countries.Count());           
        }

        [TestMethod]
        public void MatchingService_GetFacetedNavigation_ShouldReturnFraidCities()
        {
            //Arrange
            DirectoryMatchRequest matchRequest = new DirectoryMatchRequest();
            matchRequest.PageNumber = 0;
            matchRequest.MaxResultsCount = 0;
            matchRequest.ApplicationId = 7;
            matchRequest.SortMethod = EntitySortMethod.RankScore;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");

            matchRequest.ProgramLevelList = new List<int>();
            matchRequest.ProgramLevelList.Add(7);
            matchRequest.ProgramLevelList.Add(8);
            matchRequest.ProgramLevelList.Add(11);

            matchRequest.ProgramPaidStatusList = new List<int>();
            matchRequest.ProgramPaidStatusList.Add((int)PaidStatusType.Fraid);            

            //Act
            NavigationResponse response = matchingService.GetFacetedNavigation(matchRequest);

            //Assert
            //Test Country List
            //Assert.AreEqual(1, response.Countries.Count());           
        }
    }
}
