using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Tests
{
    public class BaseMatchRequestMapperTests
    {
        [Fact]
        public void MapFormInputToBaseMatchRequestTest_NotNull()
        {
            var formInput = new FormInput();

            BaseMatchRequest request = MapRequest(formInput);

            Assert.NotNull(request);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_ApplicationSetToFormsEngine()
        {
            var formInput = new FormInput();

            BaseMatchRequest request = MapRequest(formInput);

            var expectedResult = MatchingEngine.ISApplication.FormsEngine;

            Assert.Equal(expectedResult, request.Application);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(1)]
        [InlineData(0)]
        public void MapFormInputToBaseMatchRequestTest_ApplicationIdMapped(int applicationId)
        {
            var formInput = new FormInput();
            formInput.ApplicationId = applicationId;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.ApplicationId, request.ApplicationId);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_TrackIdMapped()
        {
            var formInput = new FormInput();
            formInput.TrackId = new Guid("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95");

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.TrackId, request.TrackGuid);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_CategoriesMapped()
        {
            var formInput = new FormInput();
            formInput.Categories = new List<int> { 1, 2, 3 };

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Categories, request.CategoryList);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_SubcategoriesMapped()
        {
            var formInput = new FormInput();
            formInput.Subcategories = new List<int> { 11, 22, 33 };

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Subcategories, request.SubjectList);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_SpecialtiesMapped()
        {
            var formInput = new FormInput();
            formInput.Specialties = new List<int> { 111, 222, 333 };

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Specialties, request.SpecialtyList);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_ProgramLevelIdsMapped()
        {
            var formInput = new FormInput();
            formInput.ProgramLevelIds = new List<int> { 10000, 20000, 30000 };

            BaseMatchRequest request = MapRequest(formInput);

            Assert.True(formInput.ProgramLevelIds.SequenceEqual(request.ProgramLevelList));
        }


        [Theory]
        [InlineData("online")]
        [InlineData("Online")]
        [InlineData("ONLINE")]
        public void MapFormInputToBaseMatchRequestTest_OnlineCampusPreferenceMapped(string campusType)
        {
            var formInput = new FormInput();
            formInput.CampusPreference = campusType;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(CampusType.Online, request.CampusType);
            Assert.Null(request.IsHybrid);
        }

        [Theory]
        [InlineData("campus")]
        [InlineData("Campus")]
        [InlineData("CAMPUS")]
        public void MapFormInputToBaseMatchRequestTest_GroundCampusPreferenceMapped(string campusType)
        {
            var formInput = new FormInput();
            formInput.CampusPreference = campusType;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(CampusType.Ground, request.CampusType);
            Assert.Null(request.IsHybrid);
        }

        [Theory]
        [InlineData("hybrid")]
        [InlineData("Hybrid")]
        [InlineData("HYBRID")]
        public void MapFormInputToBaseMatchRequestTest_HybridCampusPreferenceMapped(string campusType)
        {
            var formInput = new FormInput();
            formInput.CampusPreference = campusType;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Null(request.CampusType);
            Assert.True(request.IsHybrid);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_Address1()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Address1 = "123 Straight Road";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Address1, request.ProspectInput.StreetAddress);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_Address2()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Address2 = "Rainbow Road";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Address2, request.ProspectInput.AddressLine2);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_City()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.City = "Boca Raton";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.City, request.ProspectInput.City);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_Country()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.CountryId = 1;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.CountryId, request.ProspectInput.CountryId);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_Age()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Age = 25;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Age, request.ProspectInput.Age);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_EducationLevel()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.EducationLevelId = 2;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.EducationLevelId, request.ProspectInput.EducationLevelId);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_Email()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Email = "test@test.com";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Email, request.ProspectInput.Email);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_ExternalLeadId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.ExternalLeadId = "leadid";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.ExternalLeadId, request.ProspectInput.ExternalLeadId);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_FirstName()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.FirstName = "firstname";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.FirstName, request.ProspectInput.FirstName);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_LastName()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.LastName = "lastname";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.LastName, request.ProspectInput.LastName);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_GPA()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.GPAId = 1;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.GPAId, request.ProspectInput.GPAKeyValueId);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_HSGradutionYear()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.HSGraduationYear = 1990;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.HSGraduationYear, request.ProspectInput.HSGraduationYear);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToBaseMatchRequestTest_IsCitizen(bool isCitizen)
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.IsUSCitizen = isCitizen;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.IsUSCitizen, request.ProspectInput.IsCitizen);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToBaseMatchRequestTest_IsMilitary(bool isMilitary)
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.IsMilitary = isMilitary;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.IsMilitary, request.ProspectInput.IsMilitary);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_KVCodeData()
        {
            List<KeyValuePair<string, int>> kvCodeData = new List<KeyValuePair<string, int>>();
            kvCodeData.Add(new KeyValuePair<string, int>("testCode", 1));

            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.KVCodeData = kvCodeData.ToArray();

            BaseMatchRequest request = MapRequest(formInput);

            Assert.True(formInput.Prospect.KVCodeData.SequenceEqual(request.ProspectInput.KVCodeData));
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_MilitaryStatusId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.MilitaryStatusId = 1;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.MilitaryStatusId, request.ProspectInput.MilitaryStatusId);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_Phone1()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Phone1 = "5611234567";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Phone1, request.ProspectInput.Phone1);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_Phone2()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Phone2 = "5611234567";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Phone2, request.ProspectInput.Phone2);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_PostalCode()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.PostalCode = "33449";

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.PostalCode, request.ProspectInput.PostalCode);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_YearsTeachingExperienceKeyValueId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.YearsTeachingExperienceKeyValueId = 1;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.YearsTeachingExperienceKeyValueId, request.ProspectInput.YearsTeachingExperienceKeyValueId);
        }

        [Fact]
        public void MapFormInputToBaseMatchRequestTest_YearsWorkExperienceKeyValueId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.YearsWorkExperienceKeyValueId = 1;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.YearsWorkExperienceKeyValueId, request.ProspectInput.YearsWorkExperienceKeyValueId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToBaseMatchRequestTest_IsBeta(bool isBeta)
        {
            var formInput = new FormInput();
            formInput.IsBeta = isBeta;

            BaseMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.IsBeta, request.IsBeta);
        }

        private BaseMatchRequest MapRequest(FormInput formInput)
        {
            BaseMatchRequest request = new BaseMatchRequest();
            var mapper = new BaseMatchRequestMapper();
            mapper.MapFormInputToBaseMatchRequest(request, formInput);
            return request;
        }

    }
}