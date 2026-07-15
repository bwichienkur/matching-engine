using EDDY.IS.FormsEngine.Core.Mappers;
using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.Core.Models;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Tests
{
    public class DirectoryMatchRequestMapperTests
    {
        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_NotNull()
        {
            var formInput = new FormInput();

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.NotNull(request);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_ProspectInputNotNull()
        {
            var formInput = new FormInput();

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.NotNull(request.ProspectInput);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_CategoriesMapped()
        {
            var formInput = new FormInput();
            formInput.Categories = new List<int> { 1, 2, 3 };

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Categories, request.CategoryList);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_SubcategoriesMapped()
        {
            var formInput = new FormInput();
            formInput.Subcategories = new List<int> { 11, 22, 33 };

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Subcategories, request.SubjectList);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_SpecialtiesMapped()
        {
            var formInput = new FormInput();
            formInput.Specialties = new List<int> { 111, 222, 333 };

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Specialties, request.SpecialtyList);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_ApplicationSetToFormsEngine()
        {
            var formInput = new FormInput();

            DirectoryMatchRequest request = MapRequest(formInput);

            var expectedResult = MatchingEngine.ISApplication.FormsEngine;

            Assert.Equal(expectedResult, request.Application);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(1)]
        [InlineData(0)]
        public void MapFormInputToDirectoryMatchRequestTest_ApplicationIdMapped(int applicationId)
        {
            var formInput = new FormInput();
            formInput.ApplicationId = applicationId;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.ApplicationId, request.ApplicationId);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(0)]
        public void MapFormInputToDirectoryMatchRequestTest_CampusIdMapped(int campusId)
        {
            var formInput = new FormInput();
            formInput.CampusId = campusId;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.CampusId, request.CampusId);
        }

        [Theory]
        [InlineData(36)]
        [InlineData(56)]
        [InlineData(0)]
        public void MapFormInputToDirectoryMatchRequestTest_FeatureIdMapped(int featureId)
        {
            var formInput = new FormInput();
            formInput.FeatureId = featureId;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.FeatureId, request.FeatureId);
        }

        [Theory]
        [InlineData("online")]
        [InlineData("Online")]
        [InlineData("ONLINE")]
        public void MapFormInputToDirectoryMatchRequestTest_OnlineCampusPreferenceMapped(string campusType)
        {
            var formInput = new FormInput();
            formInput.CampusPreference = campusType;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(CampusType.Online, request.CampusType);
            Assert.Null(request.IsHybrid);
        }

        [Theory]
        [InlineData("campus")]
        [InlineData("Campus")]
        [InlineData("CAMPUS")]
        public void MapFormInputToDirectoryMatchRequestTest_GroundCampusPreferenceMapped(string campusType)
        {
            var formInput = new FormInput();
            formInput.CampusPreference = campusType;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(CampusType.Ground, request.CampusType);
            Assert.Null(request.IsHybrid);
        }

        [Theory]
        [InlineData("hybrid")]
        [InlineData("Hybrid")]
        [InlineData("HYBRID")]
        public void MapFormInputToDirectoryMatchRequestTest_HybridCampusPreferenceMapped(string campusType)
        {
            var formInput = new FormInput();
            formInput.CampusPreference = campusType;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Null(request.CampusType);
            Assert.True(request.IsHybrid);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_ProgramIdMapped()
        {
            var formInput = new FormInput();
            formInput.ProgramId = 10000;

            DirectoryMatchRequest request = MapRequest(formInput);

            var expectedResult = new int[] { 10000 };

            Assert.True(expectedResult.SequenceEqual(request.ProgramIdList));
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_ProgramIdsMapped()
        {
            var formInput = new FormInput();
            formInput.ProgramIds = new List<int> { 1000, 2000, 3000 };

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.True(formInput.ProgramIds.SequenceEqual(request.ProgramIdList));
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_InstitutionIdMapped()
        {
            var formInput = new FormInput();
            formInput.InstitutionId = 20000;

            DirectoryMatchRequest request = MapRequest(formInput);

            var expectedResult = new int[] { 20000 };

            Assert.True(expectedResult.SequenceEqual(request.InstitutionIdList));
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_InstitutionIdsMapped()
        {
            var formInput = new FormInput();
            formInput.InstitutionIds = new List<int> { 4000, 6000, 8000 };

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.True(formInput.InstitutionIds.SequenceEqual(request.InstitutionIdList));
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_ProgramLevelIdMapped()
        {
            var formInput = new FormInput();
            formInput.ProgramLevelId = 8000;

            DirectoryMatchRequest request = MapRequest(formInput);

            var expectedResult = new int[] { 8000 };

            Assert.True(expectedResult.SequenceEqual(request.ProgramLevelList));
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_ProgramLevelIdsMapped()
        {
            var formInput = new FormInput();
            formInput.ProgramLevelIds = new List<int> { 10000, 20000, 30000 };

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.True(formInput.ProgramLevelIds.SequenceEqual(request.ProgramLevelList));
        }


        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_TrackIdMapped()
        {
            var formInput = new FormInput();
            formInput.TrackId = new Guid("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95");

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.TrackId, request.TrackGuid);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_Address1()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Address1 = "123 Straight Road";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Address1, request.ProspectInput.StreetAddress);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_Address2()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Address2 = "Rainbow Road";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Address2, request.ProspectInput.AddressLine2);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_City()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.City = "Boca Raton";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.City, request.ProspectInput.City);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_Country()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.CountryId = 1;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.CountryId, request.ProspectInput.CountryId);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_Age()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Age = 25;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Age, request.ProspectInput.Age);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_EducationLevel()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.EducationLevelId = 2;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.EducationLevelId, request.ProspectInput.EducationLevelId);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_Email()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Email = "test@test.com";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Email, request.ProspectInput.Email);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_ExternalLeadId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.ExternalLeadId = "leadid";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.ExternalLeadId, request.ProspectInput.ExternalLeadId);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_FirstName()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.FirstName = "firstname";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.FirstName, request.ProspectInput.FirstName);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_LastName()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.LastName = "lastname";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.LastName, request.ProspectInput.LastName);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_GPA()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.GPAId = 1;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.GPAId, request.ProspectInput.GPAKeyValueId);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_HSGradutionYear()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.HSGraduationYear = 1990;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.HSGraduationYear, request.ProspectInput.HSGraduationYear);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToDirectoryMatchRequestTest_IsCitizen(bool isCitizen)
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.IsUSCitizen = isCitizen;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.IsUSCitizen, request.ProspectInput.IsCitizen);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToDirectoryMatchRequestTest_IsMilitary(bool isMilitary)
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.IsMilitary = isMilitary;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.IsMilitary, request.ProspectInput.IsMilitary);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_KVCodeData()
        {
            List<KeyValuePair<string, int>> kvCodeData = new List<KeyValuePair<string, int>>();
            kvCodeData.Add(new KeyValuePair<string, int>("testCode", 1));

            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.KVCodeData = kvCodeData.ToArray();

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.True(formInput.Prospect.KVCodeData.SequenceEqual(request.ProspectInput.KVCodeData));
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_MilitaryStatusId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.MilitaryStatusId = 1;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.MilitaryStatusId, request.ProspectInput.MilitaryStatusId);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_Phone1()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Phone1 = "5611234567";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Phone1, request.ProspectInput.Phone1);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_Phone2()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.Phone2 = "5611234567";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.Phone2, request.ProspectInput.Phone2);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_PostalCode()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.PostalCode = "33449";

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.PostalCode, request.ProspectInput.PostalCode);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_YearsTeachingExperienceKeyValueId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.YearsTeachingExperienceKeyValueId = 1;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.YearsTeachingExperienceKeyValueId, request.ProspectInput.YearsTeachingExperienceKeyValueId);
        }

        [Fact]
        public void MapFormInputToDirectoryMatchRequestTest_YearsWorkExperienceKeyValueId()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Prospect.YearsWorkExperienceKeyValueId = 1;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.Prospect.YearsWorkExperienceKeyValueId, request.ProspectInput.YearsWorkExperienceKeyValueId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormInputToDirectoryMatchRequestTest_IsBeta(bool isBeta)
        {
            var formInput = new FormInput();
            formInput.IsBeta = isBeta;

            DirectoryMatchRequest request = MapRequest(formInput);

            Assert.Equal(formInput.IsBeta, request.IsBeta);
        }

        private DirectoryMatchRequest MapRequest(FormInput formInput)
        {
            DirectoryMatchRequest request = new DirectoryMatchRequest();
            var mapper = new DirectoryMatchRequestMapper();
            mapper.MapFormInputToDirectoryMatchRequest(request, formInput);
            return request;
        }

    }
}