using EDDY.IS.Base.Util;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.UnitTests.MatchingService
{
    [TestClass]
    public class GetProgramsForCrossSellFixture
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
        public void MatchingService_GetProgramsForCrossSell_ShouldReturnProgramsForCrossSell()
        {
            string leadDataStr = "Postal_Code%3D33496%26Program_Of_Interest%3D194692%26Email%3DTest%40test.com%26First_Name%3DTEst%26Last_Name%3DBlah%26Address%3D9901%20Bedford%20Dr%26City%3D%26State%3D%26Country%3D%26Phone%3D%26leadid_token%3D92A2CD17-A93A-E27E-D9B7-6DF99AAB4B48%26AffiliateId%3D%26Form%20Type%3DWizard%26Source%3DSEO%20Site%26FormLeadUrl%3Dhttp%253A%252F%252Fcycle.gradschools.local%252Fform%252F482%252F194692%26CampusSoftPreferenceShown%3Dfalse%26CampusPreferenceShown%3Dtrue";
            string leadAdditionalDataStr = "%26Theme%3Ddefault";

            //Arrange
            CrossSellMatchRequest matchRequest = new CrossSellMatchRequest();
            matchRequest.Application = Base.ISApplication.FormsEngine;
            matchRequest.ApplicationId = 7;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");            
            matchRequest.FormProgramProductId = 270950;
            matchRequest.FormInstitutionId = 482;
            matchRequest.MaxResultsCount = 10;
            matchRequest.FormTemplateId = 509;
            matchRequest.CampusType = CampusType.Online;
            matchRequest.FormDefaultTemplateId = 1;
            matchRequest.TrackingDeviceGuid = null;
            matchRequest.InitialLeadSuccess = false;
            matchRequest.LeadCreationType = LeadCreationType.ProgramWizardUserSelection;

            Dictionary<string, string> leadData = BuildCaseInsensitiveDictionary(leadDataStr);
            Dictionary<string, string> leadAdditionalData = BuildCaseInsensitiveDictionary(leadAdditionalDataStr);

            DTO.ProspectInput prospect = BuildProspectInput(leadData, leadAdditionalData, null);
            matchRequest.ProspectInput = prospect;

            //Act
            CrossSellProgramResponse response = matchingService.GetProgramsForCrossSell(matchRequest);

            //Giving the async logging thread time to finish otherwise, its aborted after this method runs prematurely and does not log anything.
            Thread.Sleep(20000);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetProgramsForCrossSell_ShouldReturnProgramsForCrossSell_2()
        {
            string leadDataStr = @"{""AddressLine2"":"""",""Age"":30,""City"":""Hoboken"",""CountryId"":4,""EducationLevelId"":10,""Email"":""z5xc4z5cxz @test.com"",""ExternalLeadId"":""A72F65BF - A422 - 576C - 491B - 3DA6E8259D50"",""FirstName"":""wrwer"",""GPAKeyValueId"":11,""HSGraduationYear"":2000,""IsCitizen"":true,""IsMilitary"":false,""IsMobileNumber"":null,""key"":""Highest_Level_of_Education_Completed"",""value"":10,""key"":""Year_of_Highest_Education_Completed"",""value"":2000,""key"":""Desired_Start_Date"",""value"":1,""key"":""us_citizen"",""value"":22,""key"":""Military_Affiliation"",""value"":126,""key"":""Undergraduate_Degree_Grad"",""value"":22,""key"":""K12"",""value"":23,""LastName"":""werwer"",""MilitaryStatusId"":126,""Phone1"":""9545541320"",""Phone2"":"""",""PostalCode"":""07030"",""StateId"":32,""StreetAddress"":""werwer"",""YearsTeachingExperienceKeyValueId"":null,""YearsWorkExperienceKeyValueId"":20}";
            
            //Arrange
            CrossSellMatchRequest matchRequest = new CrossSellMatchRequest();
            matchRequest.Application = Base.ISApplication.FormsEngine;
            matchRequest.ApplicationId = 7;
            matchRequest.TrackGuid = new Guid("7c4d5762-6af9-4374-84f3-4eda8cb108b7");
            matchRequest.FormProgramProductId = 631996;
            matchRequest.FormInstitutionId = 8611;
            matchRequest.MaxResultsCount = 10;
            matchRequest.FormTemplateId = 277;
            matchRequest.CampusType = null;
            matchRequest.FormDefaultTemplateId = 1;
            matchRequest.TrackingDeviceGuid = new Guid("9744c493-e30f-473b-9ca1-da432d4a1f46");
            matchRequest.InitialLeadSuccess = true;
            matchRequest.LeadCreationType = LeadCreationType.ProgramWizardUserSelection;
            Dictionary<string, string> leadData = JsonConvert.DeserializeObject<Dictionary<string, string>>(leadDataStr);
            Dictionary<string, string> leadAdditionalData = new Dictionary<string, string>();

            DTO.ProspectInput prospect = BuildProspectInput(leadData, leadAdditionalData, null);
            matchRequest.ProspectInput = prospect;

            //Act
            CrossSellProgramResponse response = matchingService.GetProgramsForCrossSell(matchRequest);

            //Giving the async logging thread time to finish otherwise, its aborted after this method runs prematurely and does not log anything.
            Thread.Sleep(7500);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetProgramsForCrossSell_ShouldReturnProgramsForCrossSell_3()
        {
            string leadDataStr = @"{""Application"":4,""ApplicationId"":7,""CampusList"":null,""CampusType"":null,""CategoryList"":null,""IsBeta"":false,""IsHybrid"":null,""IsNonProfit"":null,""MaxResultsCount"":10,""ProgramLevelList"":null,""ProgramTypeList"":null,""LastName"":""ertert"",""MilitaryStatusId"":126,""Phone1"":""9545463131"",""Phone2"":"""",""PostalCode"":""07030"",""StateId"":32,""StreetAddress"":""ertert"",""YearsTeachingExperienceKeyValueId"":null,""YearsWorkExperienceKeyValueId"":20,""SFProductCodes"":null,""SpecialtyList"":null,""SubjectList"":null,""TrackGuid"":""fefac89d-6ca3-41c0-a940-56d14926bcfa"",""TrackingDeviceGuid"":""ed926b38-d23a-4ad1-8fd8-a7b2b331ccd3"",""FormDefaultTemplateId"":1,""FormInstitutionId"":8611,""FormProgramProductId"":631996,""FormTemplateId"":277,""InitialLeadSuccess"":true,""LeadCreationType"":10,""LeadScoringInput"":null,""AddressLine2"":"""",""Age"":30,""City"":""Hoboken"",""CountryId"":4,""EducationLevelId"":10,""Email"":""s6f4s5fd4s @gmail.com"",""ExternalLeadId"":""5626CF86-CE81-5A57-D6F9-4D491FE249B3"",""FirstName"":""etert"",""GPAKeyValueId"":11,""HSGraduationYear"":2000,""IsCitizen"":true,""IsMilitary"":false,""IsMobileNumber"":null,""Highest_Level_of_Education_Completed-key"":10,""Year_of_Highest_Education_Completed-key"":2000,""Desired_Start_Date-key"":1,""us_citizen-key"":22,""Military_Affiliation-key"":126,""Undergraduate_Degree_Grad-key"":22,""K12-key"":23}";

            //Arrange
            CrossSellMatchRequest matchRequest = new CrossSellMatchRequest();
            matchRequest.Application = Base.ISApplication.FormsEngine;
            matchRequest.ApplicationId = 7;
            matchRequest.TrackGuid = new Guid("fefac89d-6ca3-41c0-a940-56d14926bcfa");
            matchRequest.FormProgramProductId = 631996;
            matchRequest.FormInstitutionId = 8611;
            matchRequest.MaxResultsCount = 10;
            matchRequest.FormTemplateId = 277;
            matchRequest.CampusType = null;
            matchRequest.FormDefaultTemplateId = 1;
            matchRequest.TrackingDeviceGuid = new Guid("9744c493-e30f-473b-9ca1-da432d4a1f46");
            matchRequest.InitialLeadSuccess = true;
            matchRequest.LeadCreationType = LeadCreationType.ProgramWizardUserSelection;
            Dictionary<string, string> leadData = JsonConvert.DeserializeObject<Dictionary<string, string>>(leadDataStr);
            Dictionary<string, string> leadAdditionalData = new Dictionary<string, string>();

            DTO.ProspectInput prospect = BuildProspectInput(leadData, leadAdditionalData, null);
            matchRequest.ProspectInput = prospect;

            //Act
            CrossSellProgramResponse response = matchingService.GetProgramsForCrossSell(matchRequest);

            //Giving the async logging thread time to finish otherwise, its aborted after this method runs prematurely and does not log anything.
            Thread.Sleep(7500);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetProgramsForCrossSell_ShouldReturnProgramsForCrossSell_4()
        {
            //Arrange
            CrossSellMatchRequest matchRequest = new CrossSellMatchRequest();
            matchRequest.Application = Base.ISApplication.FormsEngine;
            matchRequest.ApplicationId = 7;
            matchRequest.TrackGuid = new Guid("351c687f-e7a2-42f8-a3aa-31b4d4c792f2");
            matchRequest.FormProgramProductId = 596544;
            matchRequest.FormInstitutionId = 272;
            matchRequest.MaxResultsCount = 10;
            matchRequest.FormTemplateId = 277;
            matchRequest.CampusType = null;
            matchRequest.FormDefaultTemplateId = 1;
            matchRequest.TrackingDeviceGuid = new Guid("70d574fe-142f-4b1b-9114-6599027517a1");
            matchRequest.InitialLeadSuccess = true;
            matchRequest.LeadCreationType = LeadCreationType.ProgramWizardUserSelection;
            
            //Prospect
            matchRequest.ProspectInput = new DTO.ProspectInput();
            matchRequest.ProspectInput.AddressLine2 = "";
            matchRequest.ProspectInput.Age = 32;
            matchRequest.ProspectInput.City = "Clifton";
		    matchRequest.ProspectInput.CountryId = 4;
            matchRequest.ProspectInput.EducationLevelId = 9;
            matchRequest.ProspectInput.Email = "sdfdfhdjs @gmail.com";
		    matchRequest.ProspectInput.ExternalLeadId = "AFC09BA6 -5515-967C-4AB1-631C9D357CFE";
		    matchRequest.ProspectInput.FirstName = "sfsf";
		    matchRequest.ProspectInput.GPAKeyValueId = 11;
            matchRequest.ProspectInput.HSGraduationYear = 2009;
            matchRequest.ProspectInput.IsCitizen = true;
            matchRequest.ProspectInput.IsMilitary = false;
            matchRequest.ProspectInput.IsMobileNumber = null;
            matchRequest.ProspectInput.LastName = "sdfsdf";
		    matchRequest.ProspectInput.MilitaryStatusId = 126;
            matchRequest.ProspectInput.Phone1 = "9545535132";
		    matchRequest.ProspectInput.Phone2 = "";
		    matchRequest.ProspectInput.PostalCode = "07012";
		    matchRequest.ProspectInput.StateId = 32;
            matchRequest.ProspectInput.StreetAddress = "sdfsdf";
		    matchRequest.ProspectInput.YearsTeachingExperienceKeyValueId = null;
            matchRequest.ProspectInput.YearsWorkExperienceKeyValueId = null;
            matchRequest.ProspectInput.EducationLevelId = 9;
            matchRequest.ProspectInput.HSGraduationYear = 2009;
            matchRequest.ProspectInput.KVCodeData = new List<KeyValuePair<string, int>>();
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("Year_of_Highest_Education_Completed", 1));
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("K12", 23));

            //Act
            CrossSellProgramResponse response = matchingService.GetProgramsForCrossSell(matchRequest);

            //Giving the async logging thread time to finish otherwise, its aborted after this method runs prematurely and does not log anything.
            Thread.Sleep(7500);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetProgramsForCrossSell_ShouldReturnProgramsForCrossSell_5()
        {
            //Arrange
            CrossSellMatchRequest matchRequest = new CrossSellMatchRequest();
            matchRequest.Application = Base.ISApplication.FormsEngine;
            matchRequest.ApplicationId = 7;
            matchRequest.TrackGuid = new Guid("fefac89d-6ca3-41c0-a940-56d14926bcfa");
            matchRequest.FormProgramProductId = 631996;
            matchRequest.FormInstitutionId = 8611;
            matchRequest.MaxResultsCount = 10;
            matchRequest.FormTemplateId = 277;
            matchRequest.CampusType = null;
            matchRequest.FormDefaultTemplateId = 1;            
            matchRequest.InitialLeadSuccess = true;
            matchRequest.LeadCreationType = LeadCreationType.ProgramWizardUserSelection;

            //Prospect
            matchRequest.ProspectInput = new DTO.ProspectInput();
            matchRequest.ProspectInput.AddressLine2 = "";
            matchRequest.ProspectInput.Age = 32;
            matchRequest.ProspectInput.City = "Clifton";
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
            matchRequest.ProspectInput.Phone1 = "9545535132";
            matchRequest.ProspectInput.Phone2 = "";
            matchRequest.ProspectInput.PostalCode = "07012";
            matchRequest.ProspectInput.StateId = 32;
            matchRequest.ProspectInput.StreetAddress = "sdfsdf";
            matchRequest.ProspectInput.YearsTeachingExperienceKeyValueId = null;
            matchRequest.ProspectInput.YearsWorkExperienceKeyValueId = null;
            matchRequest.ProspectInput.EducationLevelId = 9;
            matchRequest.ProspectInput.HSGraduationYear = 2009;
            matchRequest.ProspectInput.KVCodeData = new List<KeyValuePair<string, int>>();
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("Year_of_Highest_Education_Completed", 1));
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("K12", 23));

            //Act
            CrossSellProgramResponse response = matchingService.GetProgramsForCrossSell(matchRequest);

            //Giving the async logging thread time to finish otherwise, its aborted after this method runs prematurely and does not log anything.
            Thread.Sleep(7500);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetProgramsForCrossSell_ShouldReturnProgramsForCrossSell_6()
        {
            //Arrange
            CrossSellMatchRequest matchRequest = new CrossSellMatchRequest();
            matchRequest.Application = Base.ISApplication.FormsEngine;
            matchRequest.ApplicationId = 2;
            matchRequest.TrackGuid = new Guid("AC3EFB29-F41B-4987-961D-91F2BA013D72");
            matchRequest.FormProgramProductId = 631996;
            matchRequest.FormInstitutionId = 8937;
            matchRequest.MaxResultsCount = 10;
            matchRequest.FormTemplateId = 2;
            matchRequest.CampusType = null;
            matchRequest.FormDefaultTemplateId = 1;
            matchRequest.InitialLeadSuccess = true;
            matchRequest.LeadCreationType = LeadCreationType.ProgramWizardInitial;

            //Prospect
            matchRequest.ProspectInput = new DTO.ProspectInput();
            matchRequest.ProspectInput.AddressLine2 = "";
            matchRequest.ProspectInput.Age = 32;
            matchRequest.ProspectInput.City = "Clifton";
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
            matchRequest.ProspectInput.Phone1 = "9545583131";
            matchRequest.ProspectInput.Phone2 = "";
            matchRequest.ProspectInput.PostalCode = "07030";
            matchRequest.ProspectInput.StateId = 32;
            matchRequest.ProspectInput.StreetAddress = "sdfsdf";
            matchRequest.ProspectInput.YearsTeachingExperienceKeyValueId = null;
            matchRequest.ProspectInput.YearsWorkExperienceKeyValueId = null;
            matchRequest.ProspectInput.EducationLevelId = 9;
            matchRequest.ProspectInput.HSGraduationYear = 2009;
            matchRequest.ProspectInput.KVCodeData = new List<KeyValuePair<string, int>>();
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("Year_of_Highest_Education_Completed", 1));
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("K12", 23));

            //Act
            CrossSellProgramResponse response = matchingService.GetProgramsForCrossSell(matchRequest);

            //Giving the async logging thread time to finish otherwise, its aborted after this method runs prematurely and does not log anything.
            Thread.Sleep(7500);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetProgramsForCrossSell_SAB_ShouldReturnProgramsForCrossSell_1()
        {
            //Arrange
            CrossSellMatchRequest matchRequest = new CrossSellMatchRequest();
            matchRequest.Application = Base.ISApplication.FormsEngine;
            matchRequest.ApplicationId = 20;
            matchRequest.TrackGuid = new Guid("2308D190-B71C-49E0-BE16-FE739BEB3648");
            matchRequest.FormProgramProductId = 520924;
            matchRequest.FormInstitutionId = 5229;
            matchRequest.MaxResultsCount = 10;
            matchRequest.FormTemplateId = 475;
            matchRequest.CampusType = null;
            matchRequest.FormDefaultTemplateId = 1;
            matchRequest.InitialLeadSuccess = true;
            matchRequest.LeadCreationType = LeadCreationType.ProgramWizardInitial;

            //Prospect
            matchRequest.ProspectInput = new DTO.ProspectInput();
            matchRequest.ProspectInput.AddressLine2 = "";
            matchRequest.ProspectInput.Age = 32;
            matchRequest.ProspectInput.City = "Clifton";
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
            matchRequest.ProspectInput.Phone1 = "9545583131";
            matchRequest.ProspectInput.Phone2 = "";
            matchRequest.ProspectInput.PostalCode = "07030";
            matchRequest.ProspectInput.StateId = 32;
            matchRequest.ProspectInput.StreetAddress = "sdfsdf";
            matchRequest.ProspectInput.YearsTeachingExperienceKeyValueId = null;
            matchRequest.ProspectInput.YearsWorkExperienceKeyValueId = null;
            matchRequest.ProspectInput.EducationLevelId = 9;
            matchRequest.ProspectInput.HSGraduationYear = 2009;
            matchRequest.ProspectInput.KVCodeData = new List<KeyValuePair<string, int>>();
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("Year_of_Highest_Education_Completed", 1));
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("K12", 23));

            //Act
            CrossSellProgramResponse response = matchingService.GetProgramsForCrossSell(matchRequest);

            //Giving the async logging thread time to finish otherwise, its aborted after this method runs prematurely and does not log anything.
            Thread.Sleep(7500);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        [TestMethod]
        public void MatchingService_GetProgramsForCrossSell_SAB_ShouldReturnProgramsForCrossSell_2()
        {
            //Arrange
            CrossSellMatchRequest matchRequest = new CrossSellMatchRequest();
            matchRequest.Application = Base.ISApplication.FormsEngine;
            matchRequest.ApplicationId = 7;
            matchRequest.TrackGuid = new Guid("351C687F-E7A2-42F8-A3AA-31B4D4C792F2");
            matchRequest.FormProgramProductId = 509874;
            matchRequest.FormInstitutionId = 5404;
            matchRequest.MaxResultsCount = 10;
            matchRequest.FormTemplateId = 3;
            matchRequest.CampusType = null;
            matchRequest.FormDefaultTemplateId = 1;
            matchRequest.InitialLeadSuccess = true;
            matchRequest.LeadCreationType = LeadCreationType.ProgramWizardUserSelection;

            //Prospect
            matchRequest.ProspectInput = new DTO.ProspectInput();
            matchRequest.ProspectInput.AddressLine2 = "";
            matchRequest.ProspectInput.Age = 32;
            matchRequest.ProspectInput.City = "Clifton";
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
            matchRequest.ProspectInput.Phone1 = "9545531310";
            matchRequest.ProspectInput.Phone2 = "";
            matchRequest.ProspectInput.PostalCode = "33162";
            matchRequest.ProspectInput.StateId = 32;
            matchRequest.ProspectInput.StreetAddress = "sdfsdf";
            matchRequest.ProspectInput.YearsTeachingExperienceKeyValueId = null;
            matchRequest.ProspectInput.YearsWorkExperienceKeyValueId = null;
            matchRequest.ProspectInput.EducationLevelId = 9;
            matchRequest.ProspectInput.HSGraduationYear = 2009;
            matchRequest.ProspectInput.KVCodeData = new List<KeyValuePair<string, int>>();
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("Year_of_Highest_Education_Completed", 1));
            matchRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("K12", 23));

            //Act
            CrossSellProgramResponse response = matchingService.GetProgramsForCrossSell(matchRequest);

            //Giving the async logging thread time to finish otherwise, its aborted after this method runs prematurely and does not log anything.
            Thread.Sleep(7500);

            //Assert            
            Assert.IsTrue(response.ResultCount > 0);
        }

        public Dictionary<string, string> BuildCaseInsensitiveDictionary(string KeyValueString)
        {
            Dictionary<string, string> Result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] fieldArray;

            if (string.IsNullOrEmpty(KeyValueString))
            {
                return Result;
            }

            fieldArray = Split(KeyValueString, '&', '\0', false);

            foreach (var Field in fieldArray)
            {
                string[] KeyValue = Field.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (KeyValue.Length == 2 && !Result.ContainsKey(KeyValue[0]))
                {
                    Result.Add(KeyValue[0], KeyValue[1] == null ? "" : DecodeSpecialCharacters(KeyValue[1]));
                }
                else if (KeyValue.Length == 2 && Result.ContainsKey(KeyValue[0])) // support for multiple field values
                {
                    if (!Result[KeyValue[0]].Contains(KeyValue[1]))
                    {
                        Result[KeyValue[0]] = Result[KeyValue[0]] + "," + KeyValue[1];
                    }
                }
            }
            return Result;
        }

        public string DecodeSpecialCharacters(string KeyValueString)
        {
            string Result = KeyValueString != null ? KeyValueString : string.Empty;
            return Result.Replace("@@amp@@", "&").Replace("@@equal@@", "=").Replace("amp;", "&");
        }

        public string[] Split(string expression, char delimiter, char qualifier, bool ignoreCase)
        {
            if (ignoreCase)
            {
                expression = expression.ToLower();
                delimiter = char.ToLower(delimiter);
                qualifier = char.ToLower(qualifier);
            }

            int len = expression.Length;
            char symbol;
            List<string> list = new List<string>();
            string newField = null;

            for (int begin = 0; begin < len; ++begin)
            {
                symbol = expression[begin];
                if (symbol == delimiter || symbol == '\n')
                {
                    list.Add(string.Empty);
                }
                else
                {
                    newField = null;
                    int end = begin;
                    for (end = begin; end < len; ++end)
                    {
                        symbol = expression[end];
                        if (symbol == qualifier)
                        {
                            // bypass the unsplitable block of text 
                            bool foundClosingSymbol = false;
                            for (end = end + 1; end < len; ++end)
                            {
                                symbol = expression[end];
                                if (symbol == qualifier) { foundClosingSymbol = true; break; }
                            }
                            if (false == foundClosingSymbol)
                            {
                                throw new ArgumentException("expression contains an unclosed qualifier symbol");
                            }
                            continue;
                        }
                        if (symbol == delimiter || symbol == '\n')
                        {
                            newField = expression.Substring(begin, end - begin);
                            begin = end;
                            break;
                        }
                    }
                    if (newField == null)
                    {
                        newField = expression.Substring(begin);
                        begin = end;
                    }
                    if (newField.StartsWith("\""))
                        newField = newField.Substring(1, newField.Length - 2);

                    list.Add(newField);
                }
            }
            return list.ToArray();
        }

        private DTO.ProspectInput BuildProspectInput(Dictionary<string, string> LeadData, Dictionary<string, string> LeadAdditionalData, bool? IsMobileNumber)
        {
            Dictionary<string, string> AdditionalData = new Dictionary<string, string>(LeadAdditionalData, StringComparer.OrdinalIgnoreCase);

            //Prospect Input
            DTO.ProspectInput Prospect = new DTO.ProspectInput();
            Prospect.Age = GetFieldValue("Age", LeadData, null);
            Prospect.CountryId = GetFieldValue("Country-key", AdditionalData, null, true);
            Prospect.EducationLevelId = GetFieldValue("Highest_Level_of_Education_Completed", LeadData, null);
            Prospect.GPAKeyValueId = GetFieldValue("GPA-key", AdditionalData, null, true);
            Prospect.HSGraduationYear = GetFieldValue("Year_of_Highest_Education_Completed", LeadData, null);
            Prospect.ExternalLeadId = GetFieldValue("leadid_token", LeadData);
            string UsCitizen = GetFieldValue("us_citizen", LeadData);
            Prospect.IsCitizen = string.IsNullOrWhiteSpace(UsCitizen) ? (bool?)null : UsCitizen.Contains("Yes");
            string Military = GetFieldValue("Military_Affiliation", LeadData);
            int MilitaryStatusId = 0;
            if (!string.IsNullOrWhiteSpace(Military))
            {
                if (!int.TryParse(Military, out MilitaryStatusId))
                {
                    MilitaryStatusId = 0;
                }
            }
            Prospect.IsMilitary = string.IsNullOrWhiteSpace(Military) ? (bool?)null : Military != "126";
            Prospect.MilitaryStatusId = MilitaryStatusId == 0 ? (int?)null : MilitaryStatusId;
            Prospect.PostalCode = GetFieldValue("Postal_Code", LeadData);
            Prospect.StateId = GetFieldValue("State-key", AdditionalData, null, true);
            Prospect.YearsTeachingExperienceKeyValueId = GetFieldValue("Years_of_Teaching_Experience-key", AdditionalData, null, true);
            Prospect.YearsWorkExperienceKeyValueId = GetFieldValue("Years_of_Work_Experience-key", AdditionalData, null, true);

            //Extended prospect input fields
            Prospect.City = GetFieldValue("city", LeadData);
            Prospect.AddressLine2 = GetFieldValue("address_2", LeadData);
            Prospect.Email = GetFieldValue("email", LeadData);
            Prospect.FirstName = GetFieldValue("first_name", LeadData);
            Prospect.LastName = GetFieldValue("last_name", LeadData);
            Prospect.Phone1 = GetFieldValue("phone", LeadData);
            Prospect.Phone2 = GetFieldValue("alternate_phone", LeadData);
            Prospect.StreetAddress = GetFieldValue("address", LeadData);
            Prospect.IsMobileNumber = IsMobileNumber;


            //Phone Number cleanup
            bool IsUS = !Prospect.CountryId.HasValue ? false : Prospect.CountryId == 4;
            bool IsCanada = !Prospect.CountryId.HasValue ? false : Prospect.CountryId == 5;
            bool IsUSorCanada = IsUS || IsCanada;


            Prospect.Phone1 = string.IsNullOrEmpty(Prospect.Phone1) ? "" : Prospect.Phone1.CleanPhoneNumber(IsUSorCanada);
            Prospect.Phone2 = string.IsNullOrEmpty(Prospect.Phone2) ? "" : Prospect.Phone2.CleanPhoneNumber(IsUSorCanada);


            //Additional fields
            bool K12Found = false;
            List<KeyValuePair<string, int>> kvdata = new List<KeyValuePair<string, int>>();
            foreach (var item in AdditionalData)
            {
                if (item.Key.Contains("-key"))
                {
                    int value = 0;
                    if (int.TryParse(item.Value, out value))
                    {
                        var code = item.Key.Replace("-key", "");
                        kvdata.Add(new KeyValuePair<string, int>(code, value));
                        K12Found = code == "K12" ? true : K12Found;
                    }
                }
            }
            if (!K12Found)
            {
                kvdata.Add(new KeyValuePair<string, int>("K12", 23));
            }
            if (kvdata.Count > 0)
            {
                Prospect.KVCodeData = kvdata.ToList();
            }
            return Prospect;
        }

        public int? GetFieldValue(string key, Dictionary<string, string> values, int? DefaultValue, bool RemoveFromDictionary = false)
        {
            int? result = DefaultValue;
            string val = GetFieldValue(key, values);
            if (!string.IsNullOrWhiteSpace(val))
            {
                int intValue = 0;
                result = int.TryParse(val, out intValue) ? intValue : DefaultValue;
                if (RemoveFromDictionary)
                {
                    values.Remove(key);
                }
            }
            return result;
        }

        public string GetFieldValue(string key, Dictionary<string, string> values, bool RemoveFromDictionary = false)
        {
            string result = "";
            if (values.ContainsKey(key))
            {
                result = values[key];
                if (RemoveFromDictionary)
                {
                    values.Remove(key);
                }
            }
            return result;
        }
    }
}
