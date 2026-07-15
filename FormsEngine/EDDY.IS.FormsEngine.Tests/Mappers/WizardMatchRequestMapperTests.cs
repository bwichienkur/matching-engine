using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.DTO;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Tests
{
    public class WizardMatchRequestMapperTests
    {
        [Fact]
        public void MapFormInputToWizardMatchRequestTest_NotNull()
        {
            var formInput = new FormInput();

            WizardMatchRequest request = MapRequest(formInput);

            Assert.NotNull(request);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_ProspectInputNotNull()
        {
            var formInput = new FormInput();

            WizardMatchRequest request = MapRequest(formInput);

            Assert.NotNull(request.ProspectInput);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_CategoriesMapped()
        {
            var formInput = new FormInput();
            formInput.Categories = new List<int> { 1, 2, 3 };

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Categories, request.CategoryList);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_SubcategoriesMapped()
        {
            var formInput = new FormInput();
            formInput.Subcategories = new List<int> { 11, 22, 33 };

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Subcategories, request.SubjectList);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_SpecialtiesMapped()
        {
            var formInput = new FormInput();
            formInput.Specialties = new List<int> { 111, 222, 333 };

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Specialties, request.SpecialtyList);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_ApplicationSetToFormsEngine()
        {
            var formInput = new FormInput();

            WizardMatchRequest request = MapRequest(formInput);

            var expectedResult = MatchingEngine.ISApplication.FormsEngine;

            Assert.Equal(expectedResult, request.Application);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(1)]
        [InlineData(0)]
        public void MapFormInputToWizardMatchRequestTest_ApplicationIdMapped(int applicationId)
        {
            var formInput = new FormInput();
            formInput.ApplicationId = applicationId;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.ApplicationId, request.ApplicationId);
        }

        [Theory]
        [InlineData("online")]
        [InlineData("Online")]
        [InlineData("ONLINE")]
        public void MapFormInputToWizardMatchRequestTest_OnlineCampusPreferenceMapped(string campusType)
        {
            var formInput = new FormInput();
            formInput.CampusPreference = campusType;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(CampusType.Online, request.CampusType);
            Assert.Null(request.IsHybrid);
        }

        [Theory]
        [InlineData("campus")]
        [InlineData("Campus")]
        [InlineData("CAMPUS")]
        public void MapFormInputToWizardMatchRequestTest_GroundCampusPreferenceMapped(string campusType)
        {
            var formInput = new FormInput();
            formInput.CampusPreference = campusType;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(CampusType.Ground, request.CampusType);
            Assert.Null(request.IsHybrid);
        }

        [Theory]
        [InlineData("hybrid")]
        [InlineData("Hybrid")]
        [InlineData("HYBRID")]
        public void MapFormInputToWizardMatchRequestTest_HybridCampusPreferenceMapped(string campusType)
        {
            var formInput = new FormInput();
            formInput.CampusPreference = campusType;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Null(request.CampusType);
            Assert.True(request.IsHybrid);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_ProgramLevelIdMapped()
        {
            var formInput = new FormInput();
            formInput.ProgramLevelId = 8000;

            WizardMatchRequest request = MapRequest(formInput);

            var expectedResult = new int[] { 8000 };

            Assert.True(expectedResult.SequenceEqual(request.ProgramLevelList));
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_ProgramLevelIdsMapped()
        {
            var formInput = new FormInput();
            formInput.ProgramLevelIds = new List<int> { 10000, 20000, 30000 };

            WizardMatchRequest request = MapRequest(formInput);

            Assert.True(formInput.ProgramLevelIds.SequenceEqual(request.ProgramLevelList));
        }


        [Fact]
        public void MapFormInputToWizardMatchRequestTest_TrackIdMapped()
        {
            var formInput = new FormInput();
            formInput.TrackId = new Guid("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95");

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.TrackId, request.TrackGuid);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_Address1()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Address1 = "123 Straight Road";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Address1, request.ProspectInput.StreetAddress);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_Address2()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Address2 = "Rainbow Road";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Address2, request.ProspectInput.AddressLine2);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_City()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.City = "Boca Raton";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.City, request.ProspectInput.City);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_Country()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.CountryId = 1;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.CountryId, request.ProspectInput.CountryId);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_Age()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Age = 25;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Age, request.ProspectInput.Age);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_EducationLevel()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.EducationLevelId = 2;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.EducationLevelId, request.ProspectInput.EducationLevelId);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_Email()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Email = "test@test.com";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Email, request.ProspectInput.Email);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_ExternalLeadId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.ExternalLeadId = "leadid";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.ExternalLeadId, request.ProspectInput.ExternalLeadId);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_FirstName()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.FirstName = "firstname";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.FirstName, request.ProspectInput.FirstName);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_LastName()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.LastName = "lastname";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.LastName, request.ProspectInput.LastName);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_GPA()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.GPAId = 1;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.GPAId, request.ProspectInput.GPAKeyValueId);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_HSGradutionYear()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.HSGraduationYear = 1990;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.HSGraduationYear, request.ProspectInput.HSGraduationYear);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToWizardMatchRequestTest_IsCitizen(bool isCitizen)
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.IsUSCitizen = isCitizen;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.IsUSCitizen, request.ProspectInput.IsCitizen);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToWizardMatchRequestTest_IsMilitary(bool isMilitary)
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.IsMilitary = isMilitary;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.IsMilitary, request.ProspectInput.IsMilitary);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_KVCodeData()
        {
            List<KeyValuePair<string, int>> kvCodeData = new List<KeyValuePair<string, int>>();
            kvCodeData.Add(new KeyValuePair<string, int>("testCode", 1));

            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.KVCodeData = kvCodeData.ToArray();

            WizardMatchRequest request = MapRequest(formInput);

            Assert.True(formInput.Prospect.KVCodeData.SequenceEqual(request.ProspectInput.KVCodeData));
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_MilitaryStatusId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.MilitaryStatusId = 1;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.MilitaryStatusId, request.ProspectInput.MilitaryStatusId);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_Phone1()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Phone1 = "5611234567";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Phone1, request.ProspectInput.Phone1);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_Phone2()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Phone2 = "5611234567";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Phone2, request.ProspectInput.Phone2);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_PostalCode()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.PostalCode = "33449";

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.PostalCode, request.ProspectInput.PostalCode);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_YearsTeachingExperienceKeyValueId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.YearsTeachingExperienceKeyValueId = 1;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.YearsTeachingExperienceKeyValueId, request.ProspectInput.YearsTeachingExperienceKeyValueId);
        }

        [Fact]
        public void MapFormInputToWizardMatchRequestTest_YearsWorkExperienceKeyValueId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.YearsWorkExperienceKeyValueId = 1;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.YearsWorkExperienceKeyValueId, request.ProspectInput.YearsWorkExperienceKeyValueId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToWizardMatchRequestTest_IsBeta(bool isBeta)
        {
            var formInput = new FormInput();
            formInput.IsBeta = isBeta;

            WizardMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.IsBeta, request.IsBeta);
        }


        private WizardMatchRequest MapRequest(FormInput formInput)
        {
            WizardMatchRequest request = new WizardMatchRequest();
            var mapper = new WizardMatchRequestMapper();
            mapper.MapFormInputToWizardMatchRequest(request, formInput);
            return request;
        }
    }
}