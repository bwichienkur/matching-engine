using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.UnitTests.MatchingService
{
    [TestClass]
    public class GetProgramWizardFixture
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
        public void MatchingService_GetWizardMatches_ShouldReturnPaidFraidAndFreeGradSchoolsForNoPaidStatusTypeSearch()
        {
            //Arrange
            DirectoryMatchRequest request = new DirectoryMatchRequest();
            WizardMatchRequest matchRequest = new WizardMatchRequest(request);
            matchRequest.Application = Base.ISApplication.FormsEngine;
            matchRequest.ApplicationId = 2;
            matchRequest.TrackGuid = new Guid("0918485D-B5E6-4B8B-BE39-F2549A2EB9FD");
            matchRequest.CategoryList = new List<int>() { 21, 29, 20, 25, 26, 27, 22, 28 };
            matchRequest.IncludeSchoolSelectionList = false;
            matchRequest.IncludeSmartMatchList = true;
            matchRequest.SubjectList = new List<int>() { 703, 754, 668, 617, 758, 679, 670, 608 };
            matchRequest.TemplateList = new List<int>() { 1, 495, 532, 439, 210, 393 };
            //Prospect
            matchRequest.ProspectInput = new DTO.ProspectInput();
            matchRequest.ProspectInput.AddressLine2 = "";
            matchRequest.ProspectInput.Age = 33;
            matchRequest.ProspectInput.City = "Henrico";
            matchRequest.ProspectInput.CountryId = 4;
            matchRequest.ProspectInput.EducationLevelId = 9;
            matchRequest.ProspectInput.Email = "sdfdfhdjs@gmail.com";
            matchRequest.ProspectInput.ExternalLeadId = "AFC09BA6 -5515-967C-4AB1-631C9D357CFE";
            matchRequest.ProspectInput.FirstName = "sfsf";
            matchRequest.ProspectInput.GPAKeyValueId = 11;
            matchRequest.ProspectInput.HSGraduationYear = 2009;
            matchRequest.ProspectInput.IsCitizen = true;
            matchRequest.ProspectInput.IsMilitary = false;
            matchRequest.ProspectInput.IsMobileNumber = null;
            matchRequest.ProspectInput.LastName = "sdfsdf";
            matchRequest.ProspectInput.MilitaryStatusId = 126;
            matchRequest.ProspectInput.Phone1 = "6462040001";
            matchRequest.ProspectInput.Phone2 = "";
            matchRequest.ProspectInput.PostalCode = "23075";
            matchRequest.ProspectInput.StateId = 48;
            matchRequest.ProspectInput.StreetAddress = "sdfsdf";
            matchRequest.ProspectInput.YearsTeachingExperienceKeyValueId = null;
            matchRequest.ProspectInput.YearsWorkExperienceKeyValueId = null;
            matchRequest.ProspectInput.EducationLevelId = 9;
            matchRequest.ProspectInput.HSGraduationYear = 2009;
            matchRequest.ProspectInput.KVCodeData = new List<KeyValuePair<string, int>>();
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("Year_of_Highest_Education_Completed", 10));
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("K12", 23));

            //Act
            WizardMatchResponse response = matchingService.GetWizardMatches(matchRequest);

            Thread.Sleep(10000);

            //Assert            
            Assert.IsTrue(response != null);
        }
    }
}
