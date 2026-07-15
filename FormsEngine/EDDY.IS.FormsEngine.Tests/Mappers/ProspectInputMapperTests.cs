using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Tests
{
    public class ProspectInputMapperTests
    {
        [Fact]
        public void MapProspectToProspectInputTest_NotNull()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.NotNull(prospectInput);
        }

        [Fact]
        public void MapProspectToProspectInputTest_Address1()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.Address1 = "123 Straight Road";

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.Address1, prospectInput.StreetAddress);
        }

        [Fact]
        public void MapProspectToProspectInputTest_Address2()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.Address2 = "Rainbow Road";

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.Address2, prospectInput.AddressLine2);
        }

        [Fact]
        public void MapProspectToProspectInputTest_City()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.City = "Boca Raton";

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.City, prospectInput.City);
        }

        [Fact]
        public void MapProspectToProspectInputTest_Country()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.CountryId = 1;

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.CountryId, prospectInput.CountryId);
        }

        [Fact]
        public void MapProspectToProspectInputTest_State()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.StateId = 2;

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.StateId, prospectInput.StateId);
        }

        [Fact]
        public void MapProspectToProspectInputTest_Age()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.Age = 25;

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.Age, prospectInput.Age);
        }

        [Fact]
        public void MapProspectToProspectInputTest_EducationLevel()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.EducationLevelId = 2;

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.EducationLevelId, prospectInput.EducationLevelId);
        }

        [Fact]
        public void MapProspectToProspectInputTest_Email()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.Email = "test@test.com";

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.Email, prospectInput.Email);
        }

        [Fact]
        public void MapProspectToProspectInputTest_ExternalLeadId()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.ExternalLeadId = "leadid";

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.ExternalLeadId, prospectInput.ExternalLeadId);
        }

        [Fact]
        public void MapProspectToProspectInputTest_FirstName()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.FirstName = "firstname";

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.FirstName, prospectInput.FirstName);
        }

        [Fact]
        public void MapProspectToProspectInputTest_LastName()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.LastName = "lastname";

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.LastName, prospectInput.LastName);
        }

        [Fact]
        public void MapProspectToProspectInputTest_GPA()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.GPAId = 1;

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.GPAId, prospectInput.GPAKeyValueId);
        }

        [Fact]
        public void MapProspectToProspectInputTest_HSGradutionYear()
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.HSGraduationYear = 1990;

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.HSGraduationYear, prospectInput.HSGraduationYear);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapProspectToProspectInputTest_IsCitizen(bool isCitizen)
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.IsUSCitizen = isCitizen;

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.IsUSCitizen, prospectInput.IsCitizen);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapProspectToProspectInputTest_IsMilitary(bool isMilitary)
        {
            var mapper = new ProspectInputMapper();

            var prospect = new Prospect();
            prospect.IsMilitary = isMilitary;

            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.IsMilitary, prospectInput.IsMilitary);
        }

        [Fact]
        public void MapProspectToProspectInputTest_KVCodeData()
        {
            List<KeyValuePair<string, int>> kvCodeData = new List<KeyValuePair<string, int>>();
            kvCodeData.Add(new KeyValuePair<string, int>("testCode", 1));

            var prospect = new Prospect();
            prospect.KVCodeData = kvCodeData.ToArray();

            var mapper = new ProspectInputMapper();
            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.True(prospect.KVCodeData.SequenceEqual(prospectInput.KVCodeData));
        }

        [Fact]
        public void MapProspectToProspectInputTest_MilitaryStatusId()
        {
            var prospect = new Prospect();
            prospect.MilitaryStatusId = 1;

            var mapper = new ProspectInputMapper();
            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.MilitaryStatusId, prospectInput.MilitaryStatusId);
        }

        [Fact]
        public void MapProspectToProspectInputTest_Phone1()
        {
            var prospect = new Prospect();
            prospect.Phone1 = "5611234567";

            var mapper = new ProspectInputMapper();
            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.Phone1, prospectInput.Phone1);
        }

        [Fact]
        public void MapProspectToProspectInputTest_Phone2()
        {
            var prospect = new Prospect();
            prospect.Phone2 = "5611234567";

            var mapper = new ProspectInputMapper();
            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.Phone2, prospectInput.Phone2);
        }

        [Fact]
        public void MapProspectToProspectInputTest_PostalCode()
        {
            var prospect = new Prospect();
            prospect.PostalCode = "33449";

            var mapper = new ProspectInputMapper();
            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.PostalCode, prospectInput.PostalCode);
        }

        [Fact]
        public void MapProspectToProspectInputTest_YearsTeachingExperienceKeyValueId()
        {
            var prospect = new Prospect();
            prospect.YearsTeachingExperienceKeyValueId = 1;

            var mapper = new ProspectInputMapper();
            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.YearsTeachingExperienceKeyValueId, prospectInput.YearsTeachingExperienceKeyValueId);
        }

        [Fact]
        public void MapProspectToProspectInputTest_YearsWorkExperienceKeyValueId()
        {
            var prospect = new Prospect();
            prospect.YearsWorkExperienceKeyValueId = 1;

            var mapper = new ProspectInputMapper();
            var prospectInput = mapper.MapProspectToProspectInput(prospect);

            Assert.Equal(prospect.YearsWorkExperienceKeyValueId, prospectInput.YearsWorkExperienceKeyValueId);
        }
    }
}
