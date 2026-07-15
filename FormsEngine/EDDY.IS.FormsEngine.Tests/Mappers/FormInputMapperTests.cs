using Xunit;
using EDDY.IS.FormsEngine.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using FluentAssertions;
using System.Web;
using System.Collections.Specialized;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;
using Match = EDDY.IS.FormsEngine.Core.Models.Match;
using EDDY.IS.FormsEngine.Tests.TestData;

namespace EDDY.IS.FormsEngine.Core.Mappers.Tests
{
    public class FormInputMapperTests
    {
        [Fact]
        public void MapFormRequestToFormInput_NotNull()
        {
            var formInput = MapRequest();
            Assert.NotNull(formInput);
        }

        [Theory]
        [InlineData("Campus=1", 1)]
        [InlineData("campus=2", 2)]
        public void MapFormRequestToFormInput_CampusIdMapped(string leadData, int expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.CampusId);
        }

        [Fact]
        public void MapFormRequestToFormInput_CampusIdIsNull()
        {
            var formInput = MapRequest();

            int? expectedResult = null;
            Assert.Equal(expectedResult, formInput.CampusId);
        }

        [Fact]
        public void MapFormRequestToFormInput_FeatureIdIsNull()
        {
            var formInput = MapRequest();

            int? expectedResult = null;
            Assert.Equal(expectedResult, formInput.FeatureId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(null)]
        public void MapFormRequestToFormInput_FeatureIdIsMapped(int? featureId)
        {
            var request = new FormRequest();
            request.FeatureId = featureId;

            var formInput = MapRequest(request);
            Assert.Equal(featureId, formInput.FeatureId);
        }

        [Fact]
        public void MapFormRequestToFormInput_ProgramIdIsNull()
        {
            var formInput = MapRequest();
            int? expectedResult = null;
            Assert.Equal(expectedResult, formInput.ProgramId);
        }

        [Theory]
        [InlineData("Program_Of_Interest=1", 1)]
        [InlineData("program_of_interest=2", 2)]
        public void MapFormRequestToFormInput_ProgramIdIsMapped(string leadData, int expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.ProgramId);
        }

        [Fact]
        public void MapFormRequestToFormInput_InstitutionIdIsNull()
        {
            var formInput = MapRequest();
            int? expectedResult = null;
            Assert.Equal(expectedResult, formInput.InstitutionId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(null)]
        public void MapFormRequestToFormInput_InstitutionIdIsMapped(int? institutionId)
        {
            var request = new FormRequest();
            request.InstitutionId = institutionId;

            var formInput = MapRequest(request);

            Assert.Equal(institutionId, formInput.InstitutionId);
        }

        [Fact]
        public void MapFormRequestToFormInput_CategoryIdsIsEmpty()
        {
            var formInput = MapRequest();
            List<int> expectedResult = new List<int>();
            Assert.True(expectedResult.SequenceEqual(formInput.Categories));
        }

        [Theory]
        [InlineData("Categories=1,2,3", new int[] { 1, 2, 3 })]
        [InlineData("categories=4,5,6", new int[] { 4, 5, 6 })]
        public void MapFormRequestToFormInput_CategoryIdsAreMapped(string leadData, int[] expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.True(expectedResult.ToList().SequenceEqual(formInput.Categories));
        }

        [Fact]
        public void MapFormRequestToFormInput_SubcategoryIdsIsEmpty()
        {
            var formInput = MapRequest();
            List<int> expectedResult = new List<int>();
            Assert.True(expectedResult.SequenceEqual(formInput.Subcategories));
        }

        [Theory]
        [InlineData("SubCategories=11,22,33", new int[] { 11, 22, 33 })]
        [InlineData("subcategories=44,55,66", new int[] { 44, 55, 66 })]
        public void MapFormRequestToFormInput_SubcategoryIdsAreMapped(string leadData, int[] expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.True(expectedResult.ToList().SequenceEqual(formInput.Subcategories));
        }

        [Fact]
        public void MapFormRequestToFormInput_SpecialtyIdsIsEmpty()
        {
            var formInput = MapRequest();
            List<int> expectedResult = new List<int>();
            Assert.True(expectedResult.SequenceEqual(formInput.Specialties));
        }

        [Theory]
        [InlineData("Specialties=111,222,333", new int[] { 111, 222, 333 })]
        [InlineData("specialties=777,888,999", new int[] { 777, 888, 999 })]
        public void MapFormRequestToFormInput_SpecialtyIdsAreMapped(string leadData, int[] expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.True(expectedResult.ToList().SequenceEqual(formInput.Specialties));
        }

        [Fact]
        public void MapFormRequestToFormInput_ProspectIsNotNull()
        {
            var formInput = MapRequest();
            Assert.NotNull(formInput.Prospect);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(25)]
        [InlineData(123454)]
        public void MapFormRequestToFormInput_ProspectIdIsMapped(int? prospectId)
        {
            var formRequest = new FormRequest();
            formRequest.ProspectId = prospectId;

            var formInput = MapRequest(formRequest);
            
            Assert.Equal(prospectId, formInput.Prospect.ProspectId);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("Age=25", 25)]
        [InlineData("age=23", 23)]
        public void MapFormRequestToFormInput_ProspectAgeIsMapped(string leadData, int? expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.Age);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("State-key=2", 2)]
        [InlineData("state-key=5", 5)]
        public void MapFormRequestToFormInput_ProspectStateIdIsMapped(string additionalData, int? expectedResult)
        {
            var request = new FormRequest()
            {
                AdditionalData = additionalData
            };

            var formInput = MapRequest(request);

            Assert.Equal(expectedResult, formInput.Prospect.StateId);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("Country-key=2", 2)]
        [InlineData("country-key=2", 2)]
        public void MapFormRequestToFormInput_ProspectCountryIdIsMapped(string additionalData, int? expectedResult)
        {
            var request = new FormRequest()
            {
                AdditionalData = additionalData
            };

            var formInput = MapRequest(request);

            Assert.Equal(expectedResult, formInput.Prospect.CountryId);
        }

        [Theory]
        [InlineData("Country=US&State=FL", "Country-key=2&State-key=15", 4, 10)]
        [InlineData("Country=US&State=FL", "Country-key=2", 4, 10)]
        [InlineData("Country=US&State=FL", "State-key=15", 4, 10)]
        [InlineData("Country=US&State=FL", "", 4, 10)]
        [InlineData("", "Country-key=2&State-key=15", 2, 15)]
        [InlineData("Country=US", "Country-key=2&State-key=15", 2, 15)]
        [InlineData("State=FL", "Country-key=2&State-key=15", 2, 15)]
        [InlineData("", "Country-key=2", 2, null)]
        [InlineData("", "State-key=15", null, 15)]
        [InlineData("", "", null, null)]

        public void MapFormRequestToFormInput_ProspectCountryAndStateIdsAreMapped(string leadData, string leadAdditionalData, int? expectedCountryId, int? expectedStateId)
        {
            var request = new FormRequest()
            {
                LeadData = leadData,
                AdditionalData = leadAdditionalData
            };

            var mockLocation = new Location { CountryId = 4, StateId = 10 };

            var mockIPAddressService = GetMockIPAddressService();
            var mockLocationValidationService = new Mock<ILocationValidationService>();
            mockLocationValidationService.Setup(s => s.GetValidLocation(It.IsAny<string>(), It.IsAny<string>())).Returns(mockLocation);
            var mockHttpContext = GetHttpContextMock();

            var mapper = new FormInputMapper(mockIPAddressService.Object, mockLocationValidationService.Object);
            var formInput = mapper.MapFormRequestToFormInput(request, mockHttpContext.Object);

            Assert.Equal(expectedCountryId, formInput.Prospect.CountryId);
            Assert.Equal(expectedStateId, formInput.Prospect.StateId);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("Highest_Level_Of_Education_Completed=5", 5)]
        [InlineData("highest_level_of_education_completed=9", 9)]
        public void MapFormRequestToFormInput_ProspectEducationLevelIdIsMapped(string leadData, int? expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.EducationLevelId);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("GPA-key=1", 1)]
        [InlineData("gpa-key=2", 2)]
        public void MapFormRequestToFormInput_ProspectGPAIdIsMapped(string additionalData, int? expectedResult)
        {
            var request = new FormRequest()
            {
                AdditionalData = additionalData
            };

            var formInput = MapRequest(request);

            Assert.Equal(expectedResult, formInput.Prospect.GPAId);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("Year_Of_Highest_Education_Completed=2010", 2010)]
        [InlineData("year_of_highest_education_completed=2000", 2000)]
        public void MapFormRequestToFormInput_ProspectHSGraduationYearIsMapped(string leadData, int? expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.HSGraduationYear);
        }

        [Theory]
        [InlineData("Preferred_methods_of_contact=email", true, false, false)]
        [InlineData("Preferred_methods_of_contact=phone", false, true, false)]
        [InlineData("Preferred_methods_of_contact=text", false, false, true)]
        [InlineData("Preferred_methods_of_contact=email,phone", true, true, false)]
        [InlineData("Preferred_methods_of_contact=email,phone,text", true, true, true)]
        [InlineData("Preferred_methods_of_contact=phone,text", false, true, true)]
        [InlineData("Preferred_Methods_Of_Contact=EMAIL", true, false, false)]
        [InlineData("Preferred_Methods_Of_Contact=PHONE", false, true, false)]
        [InlineData("Preferred_Methods_Of_Contact=TEXT", false, false, true)]
        [InlineData("Preferred_Methods_Of_Contact=EMAIL,PHONE", true, true, false)]
        [InlineData("Preferred_Methods_Of_Contact=EMAIL,PHONE,TEXT", true, true, true)]
        [InlineData("Preferred_Methods_Of_Contact=PHONE,TEXT", false, true, true)]
        public void MapFormRequestToFormInput_ProspectContactPreferencesMapped(string leadData, bool preferEmail, bool preferPhone, bool preferText)
        {
            var formInput = MapRequest(leadData);

            Assert.Equal(preferEmail, formInput.Prospect.PreferEmail);
            Assert.Equal(preferPhone, formInput.Prospect.PreferPhone);
            Assert.Equal(preferText, formInput.Prospect.PreferText);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("GraduationYear=2005", 2005)]
        [InlineData("graduationyear=1997", 1997)]
        public void MapFormRequestToFormInput_ProspectGenericGraduationYearIsMapped(string leadData, int? expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.GenericGraduationYear);
        }

        [Theory]
        [InlineData("")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormRequestToFormInput_ProspectExternalLeadIdIsMapped(string leadIdToken)
        {
            var request = new FormRequest
            {
                LeadIdToken = leadIdToken
            };

            var formInput = MapRequest(request);
            Assert.Equal(leadIdToken, formInput.Prospect.ExternalLeadId);
        }

        [Theory]
        [InlineData("us_citizen=yes", true)]
        [InlineData("us_citizen=Yes", true)]
        [InlineData("us_citizen=YES", true)]
        [InlineData("us_citizen=no", false)]
        [InlineData("us_citizen=No", false)]
        [InlineData("us_citizen=NO", false)]
        [InlineData("us_citizen=", null)]
        [InlineData("US_Citizen=yes", true)]
        [InlineData("US_Citizen=Yes", true)]
        [InlineData("US_Citizen=YES", true)]
        [InlineData("US_Citizen=no", false)]
        [InlineData("US_Citizen=No", false)]
        [InlineData("US_Citizen=NO", false)]
        [InlineData("US_Citizen=", null)]
        [InlineData("", null)]
        public void MapFormRequestToFormInput_ProspectIsCitizenMapped(string leadData, bool? expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.IsUSCitizen);
        }

        [Theory]
        [InlineData("financialaid=yes", false)]
        [InlineData("financialaid=Yes", false)]
        [InlineData("financialaid=YES", false)]
        [InlineData("financialaid=no", true)]
        [InlineData("financialaid=No", true)]
        [InlineData("financialaid=NO", true)]
        [InlineData("financialaid=", null)]
        [InlineData("FINANCIALAID=yes", false)]
        [InlineData("FINANCIALAID=Yes", false)]
        [InlineData("FINANCIALAID=YES", false)]
        [InlineData("FINANCIALAID=no", true)]
        [InlineData("FINANCIALAID=No", true)]
        [InlineData("FINANCIALAID=NO", true)]
        [InlineData("FINANCIALAID=", null)]
        [InlineData("", null)]
        public void MapFormRequestToFormInput_ProspectNeedsFinancialAidMapped(string leadData, bool? expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.NeedsFinancialAid);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("desired_start_date=", "")]
        [InlineData("desired_start_date=immediately", "immediately")]
        [InlineData("Desired_Start_Date=", "")]
        [InlineData("Desired_Start_Date=immediately", "immediately")]
        public void MapFormRequestToFormInput_ProspectDesiredStartDateMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.DesiredStartDate);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("Military_Affiliation=", null)]
        [InlineData("Military_Affiliation=100", true)]
        [InlineData("Military_Affiliation=126", false)]
        [InlineData("military_affiliation=", null)]
        [InlineData("military_affiliation=100", true)]
        [InlineData("military_affiliation=126", false)]
        public void MapFormRequestToFormInput_ProspectIsMilitaryMapped(string leadData, bool? expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.IsMilitary);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("Military_Affiliation=", null)]
        [InlineData("Military_Affiliation=0", null)]
        [InlineData("Military_Affiliation=100", 100)]
        [InlineData("Military_Affiliation=126", 126)]
        [InlineData("military_affiliation=", null)]
        [InlineData("military_affiliation=0", null)]
        [InlineData("military_affiliation=100", 100)]
        [InlineData("military_affiliation=126", 126)]
        public void MapFormRequestToFormInput_ProspectMilitaryStatusIdIsMapped(string leadData, int? expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.MilitaryStatusId);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("postal_code=33467", "33467")]
        [InlineData("Postal_Code=33449", "33449")]
        public void MapFormRequestToFormInput_ProspectPostalCodeIsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.PostalCode);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("Years_Of_Teaching_Experience-key=4", 4)]
        [InlineData("years_of_teaching_experience-key=6", 6)]
        public void MapFormRequestToFormInput_ProspectYearsTeachingExperienceKeyValueIdIsMapped(string additionalData, int? expectedResult)
        {
            var request = new FormRequest()
            {
                AdditionalData = additionalData
            };

            var formInput = MapRequest(request);

            Assert.Equal(expectedResult, formInput.Prospect.YearsTeachingExperienceKeyValueId);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("Years_Of_Work_Experience-key=6", 6)]
        [InlineData("years_of_work_experience-key=8", 8)]
        public void MapFormRequestToFormInput_ProspectYearsWorkExperienceKeyValueIdIsMapped(string additionalData, int? expectedResult)
        {
            var request = new FormRequest()
            {
                AdditionalData = additionalData
            };

            var formInput = MapRequest(request);

            Assert.Equal(expectedResult, formInput.Prospect.YearsWorkExperienceKeyValueId);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("city=bocaraton", "bocaraton")]
        [InlineData("City=boca raton", "boca raton")]
        public void MapFormRequestToFormInput_ProspectCityIsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.City);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Address=564 Round Rd", "564 Round Rd")]
        [InlineData("address=5646 round road", "5646 round road")]
        public void MapFormRequestToFormInput_ProspectAddress1IsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.Address1);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Address_2=123 Straight Rd", "123 Straight Rd")]
        [InlineData("address_2=1231 straight road", "1231 straight road")]
        public void MapFormRequestToFormInput_ProspectAddress2IsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.Address2);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Email=tester@yahoo.com", "tester@yahoo.com")]
        [InlineData("email=test@gmail.com", "test@gmail.com")]
        public void MapFormRequestToFormInput_ProspectEmailIsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.Email);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("First_Name=tester", "tester")]
        [InlineData("first_name=test", "test")]
        public void MapFormRequestToFormInput_ProspectFirstNameIsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.FirstName);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Last_Name=test", "test")]
        [InlineData("last_name=tester", "tester")]
        public void MapFormRequestToFormInput_ProspectLastNameIsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.LastName);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("phone=5611234567", "5611234567")]
        [InlineData("phone=561-123-4567", "5611234567")]
        [InlineData("phone=561/123/4567", "5611234567")]
        [InlineData("phone=(561)-123-4567", "5611234567")]
        [InlineData("phone=(561)(123)(4567)", "5611234567")]
        [InlineData("phone=15611234567", "15611234567")]
        [InlineData("phone=1561-123-4567", "15611234567")]
        [InlineData("phone=1561/123/4567", "15611234567")]
        [InlineData("phone=1(561)-123-4567", "15611234567")]
        [InlineData("phone=1(561)(123)(4567)", "15611234567")]
        public void MapFormRequestToFormInput_ProspectPhone1IsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.Phone1);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("alternate_phone=3611234567", "3611234567")]
        [InlineData("alternate_phone=361-123-4567", "3611234567")]
        [InlineData("alternate_phone=361/123/4567", "3611234567")]
        [InlineData("alternate_phone=(361)-123-4567", "3611234567")]
        [InlineData("alternate_phone=(361)(123)(4567)", "3611234567")]
        [InlineData("alternate_phone=13611234567", "13611234567")]
        [InlineData("alternate_phone=1361-123-4567", "13611234567")]
        [InlineData("alternate_phone=1361/123/4567", "13611234567")]
        [InlineData("alternate_phone=1(361)-123-4567", "13611234567")]
        [InlineData("alternate_phone=1(361)(123)(4567)", "13611234567")]
        public void MapFormRequestToFormInput_ProspectPhone2IsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.Prospect.Phone2);
        }

        [Theory]
        [MemberData(nameof(GetPhoneTestCasesWithCountryParams), "phone")]
        public void MapFormRequestToFormInput_ProspectPhone1IsMapped_USOrCanada(string additionalData, string leadData, string expectedResult)
        {
            var request = new FormRequest
            {
                LeadData = leadData,
                AdditionalData = additionalData
            };

            var formInput = MapRequest(request);

            Assert.Equal(expectedResult, formInput.Prospect.Phone1);
        }

        [Theory]
        [MemberData(nameof(GetPhoneTestCasesWithCountryParams), "alternate_phone")]
        public void MapFormRequestToFormInput_ProspectPhone2IsMapped_USOrCanada(string additionalData, string leadData, string expectedResult)
        {
            var request = new FormRequest
            {
                LeadData = leadData,
                AdditionalData = additionalData
            };

            var formInput = MapRequest(request);

            Assert.Equal(expectedResult, formInput.Prospect.Phone2);
        }

        [Theory]
        [InlineData("TestKVCodeData-key=1&K12-key=2")]
        public void MapFormRequestToFormInput_KVCodeDataWithK12Set(string additionalData)
        {
            var request = new FormRequest
            {
                AdditionalData = additionalData
            };

            var formInput = MapRequest(request);

            var kvCodeDataExpectedResult = new List<KeyValuePair<string, int>>();
            kvCodeDataExpectedResult.Add(new KeyValuePair<string, int>("TestKVCodeData", 1));
            kvCodeDataExpectedResult.Add(new KeyValuePair<string, int>("K12", 2));

            Assert.True(formInput.Prospect.KVCodeData.OrderBy(i => i.Key).SequenceEqual(kvCodeDataExpectedResult.OrderBy(i => i.Key).ToArray()));
        }

        [Theory]
        [InlineData("TestKVCodeData-key=1")]
        public void MapFormRequestToFormInput_KVCodeDataWithoutK12Set(string additionalData)
        {
            var request = new FormRequest
            {
                AdditionalData = additionalData
            };

            var formInput = MapRequest(request);

            var kvCodeDataExpectedResult = new List<KeyValuePair<string, int>>();
            kvCodeDataExpectedResult.Add(new KeyValuePair<string, int>("TestKVCodeData", 1));
            kvCodeDataExpectedResult.Add(new KeyValuePair<string, int>("K12", 23));

            Assert.True(formInput.Prospect.KVCodeData.OrderBy(i => i.Key).SequenceEqual(kvCodeDataExpectedResult.OrderBy(i => i.Key).ToArray()));
        }

        [Theory]
        [InlineData("EMSCampusPreference=online&EMSLearningPreferenceAndLocations=hybrid&CampusPreference=campus", "online")]
        [InlineData("EMSCampusPreference=hybrid&EMSLearningPreferenceAndLocations=campus&CampusPreference=online", "hybrid")]
        [InlineData("EMSCampusPreference=campus&EMSLearningPreferenceAndLocations=online&CampusPreference=hybrid", "campus")]
        [InlineData("EMSLearningPreferenceAndLocations=hybrid&CampusPreference=campus", "hybrid")]
        [InlineData("EMSLearningPreferenceAndLocations=campus&CampusPreference=online", "campus")]
        [InlineData("EMSLearningPreferenceAndLocations=online&CampusPreference=hybrid", "online")]
        [InlineData("CampusPreference=campus", "campus")]
        [InlineData("CampusPreference=online", "online")]
        [InlineData("CampusPreference=hybrid", "hybrid")]
        [InlineData("emscampuspreference=online&emslearningpreferenceandlocations=hybrid&campuspreference=campus", "online")]
        [InlineData("emscampuspreference=hybrid&emslearningpreferenceandlocations=campus&campuspreference=online", "hybrid")]
        [InlineData("emscampuspreference=campus&emslearningpreferenceandlocations=online&campuspreference=hybrid", "campus")]
        [InlineData("emslearningpreferenceandlocations=hybrid&campuspreference=campus", "hybrid")]
        [InlineData("emslearningpreferenceandlocations=campus&campuspreference=online", "campus")]
        [InlineData("emslearningpreferenceandlocations=online&campuspreference=hybrid", "online")]
        [InlineData("campuspreference=campus", "campus")]
        [InlineData("campuspreference=online", "online")]
        [InlineData("campuspreference=hybrid", "hybrid")]
        public void MapFormRequestToFormInput_CampusPreferenceMapped(string parameters, string expectedValue)
        {
            var formInput = MapRequest(parameters);
            Assert.Equal(expectedValue, formInput.CampusPreference);
        }


        [Theory]
        [InlineData("")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormRequestToFormInput_TrackIdIsMapped(string trackId)
        {
            var request = new FormRequest
            {
                TrackId = trackId
            };

            var formInput = MapRequest(request);
            Guid.TryParse(trackId, out Guid expectedResult);

            Assert.Equal(expectedResult, formInput.TrackId);
        }


        [Theory]
        [InlineData(20)]
        [InlineData(0)]
        public void MapFormRequestToFormInput_ApplicationIdIsMapped(int applicationId)
        {
            var request = new FormRequest
            {
                ApplicationId = applicationId
            };

            var formInput = MapRequest(request);
            Assert.Equal(applicationId, formInput.ApplicationId);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormRequestToFormInput_IsBetaMapped(bool isBeta)
        {
            var request = new FormRequest
            {
                IsBeta = isBeta
            };

            var formInput = MapRequest(request);
            Assert.Equal(isBeta, formInput.IsBeta);
        }

        [Theory]
        [InlineData("school_picker={\"1571\":{\"institutionId\":1571,\"programId\":298453,\"programProductId\":617425,\"programTemplateId\":2},\"4462\":{\"institutionId\":4462,\"programId\":244982,\"programProductId\":626396,\"programTemplateId\":2},\"5128\":{\"institutionId\":5128,\"programId\":176009,\"programProductId\":180443,\"programTemplateId\":4}}")]
        [InlineData("School_Picker={\"1571\":{\"institutionId\":1571,\"programId\":298453,\"programProductId\":617425,\"programTemplateId\":2},\"4462\":{\"institutionId\":4462,\"programId\":244982,\"programProductId\":626396,\"programTemplateId\":2},\"5128\":{\"institutionId\":5128,\"programId\":176009,\"programProductId\":180443,\"programTemplateId\":4}}")]
        public void MapFormRequestToFormInput_MatchesMapped(string parameters)
        {
            List<Match> expectedMatches = new List<Match>
            {
                { new Match() { InstitutionId = 1571, ProgramId = 298453, ProgramProductId = 617425, ProgramTemplateId = 2 } },
                { new Match() { InstitutionId = 4462, ProgramId = 244982, ProgramProductId = 626396, ProgramTemplateId = 2} },
                { new Match() { InstitutionId = 5128, ProgramId = 176009, ProgramProductId = 180443, ProgramTemplateId = 4 } }
            };

            var formInput = MapRequest(parameters);

            expectedMatches.Should().BeEquivalentTo(formInput.Matches);
        }


        [Theory]
        [InlineData("school_picker={}")]
        [InlineData("School_Picker={}")]
        public void MapFormRequestToFormInput_NoMatchesMapped(string parameters)
        {
            var formInput = MapRequest(parameters);
            Assert.True(formInput.Matches.Count == 0);
        }

        [Theory]
        [InlineData("TestParam=TestValue")]
        public void MapFormRequestToFormInput_LeadDataMapped(string leadData)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(leadData, formInput.LeadData);
        }

        [Theory]
        [InlineData("TestAdditionalParam=TestAdditionalValue")]
        public void MapFormRequestToFormInput_LeadAdditionalDataMapped(string leadAdditionalData)
        {
            var formRequest = new FormRequest();
            formRequest.AdditionalData = leadAdditionalData;

            var formInput = MapRequest(formRequest);

            Assert.Equal(leadAdditionalData, formInput.LeadAdditionalData);
        }

        [Theory]
        [InlineData("HTTP_REFERER", "TestReferer")]
        public void MapFormRequestToFormInput_HttpRefererMapped(string serverVariableName, string serverVariableValue)
        {
            var mockHttpContext = GetHttpContextMock(serverVariableName, serverVariableValue);
            var formInput = MapRequest(mockHttpContext);

            Assert.Equal(serverVariableValue, formInput.HttpReferer);
        }

        [Theory]
        [InlineData("HTTP_USER_AGENT", "TestUserAgent")]
        public void MapFormRequestToFormInput_UserAgentMapped(string serverVariableName, string serverVariableValue)
        {
            var mockHttpContext = GetHttpContextMock(serverVariableName, serverVariableValue);
            var formInput = MapRequest(mockHttpContext);

            Assert.Equal(serverVariableValue, formInput.UserAgent);
        }

        [Theory]
        [InlineData("0.0.0.0")]
        public void MapFormRequestToFormInput_IpAddressMapped(string ipAddress)
        {
            FormRequest request = new FormRequest();

            var mockIPAddressService = GetMockIPAddressService(ipAddress);
            var mockLocationValidationService = new Mock<ILocationValidationService>();
            var mockHttpContext = GetHttpContextMock();

            var mapper = new FormInputMapper(mockIPAddressService.Object, mockLocationValidationService.Object);
            var formInput = mapper.MapFormRequestToFormInput(request, mockHttpContext.Object);
            

            Assert.Equal(ipAddress, formInput.IpAddress);
        }

        [Theory]
        [InlineData(100)]
        public void MapFormRequestToFormInput_TemplateIdMapped(int templateId)
        {
            FormRequest request = new FormRequest();
            request.TemplateId = templateId;

            var formInput = MapRequest(request);

            Assert.Equal(templateId, formInput.TemplateId);
        }


        [Theory]
        [InlineData("")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormRequestToFormInput_SessionIdIsMapped(string sessionId)
        {
            var request = new FormRequest
            {
                SessionId = sessionId
            };

            var formInput = MapRequest(request);

            Assert.Equal(sessionId, formInput.SessionId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormRequestToFormInput_FESessionIdIsMapped(string feSessionId)
        {
            var request = new FormRequest
            {
                FESessionId = feSessionId
            };

            var formInput = MapRequest(request);

            Assert.Equal(feSessionId, formInput.FESessionId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormRequestToFormInput_MatchResponseGuidIsMapped(string matchResponseGuid)
        {
            var request = new FormRequest
            {
                MatchGuid = matchResponseGuid
            };

            var formInput = MapRequest(request);

            Guid.TryParse(matchResponseGuid, out Guid expectedResult);

            Assert.Equal(expectedResult, formInput.MatchResponseGuid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormRequestToFormInput_LimboAlternativeCampaignTrackIdIsMapped(string limboAlternativeCampaignTrackId)
        {
            var request = new FormRequest
            {
                LimboAlternativeCampaignTrackid = limboAlternativeCampaignTrackId
            };

            var formInput = MapRequest(request);

            Guid.TryParse(limboAlternativeCampaignTrackId, out Guid expectedResult);

            Assert.Equal(expectedResult, formInput.LimboAlternativeCampaignTrackId);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void MapFormRequestToFormInput_LimboAlternativeCampaignTrackIdUtilizedIsMapped(bool limboAlternativeCampaignTrackIdUtilized)
        {
            var request = new FormRequest
            {
                LimboAlternativeCampaignTrackidUtilized = limboAlternativeCampaignTrackIdUtilized
            };

            var formInput = MapRequest(request);

            Assert.Equal(limboAlternativeCampaignTrackIdUtilized, formInput.LimboAlternativeCampaignTrackIdUtilized);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("10")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormRequestToFormInput_InitialLeadIdIsMapped(string initialLeadId)
        {
            var request = new FormRequest
            {
                InitialLeadId = initialLeadId
            };

            var formInput = MapRequest(request);

            Assert.Equal(initialLeadId, formInput.InitialLeadId);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("AdvisorId=25", 25)]
        [InlineData("advisorid=23", 23)]
        public void MapFormRequestToFormInput_AdvisorIdIsMapped(string leadData, int? expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.AdvisorId);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Country=US", "US")]
        [InlineData("Country=us", "US")]
        [InlineData("country=CA", "CA")]
        [InlineData("country=ca", "CA")]
        public void MapFormRequestToFormInput_CountryCodeIsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.CountryCode);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("State=FL", "FL")]
        [InlineData("State=fl", "FL")]
        [InlineData("state=NY", "NY")]
        [InlineData("state=ny", "NY")]
        public void MapFormRequestToFormInput_StateCodeIsMapped(string leadData, string expectedResult)
        {
            var formInput = MapRequest(leadData);
            Assert.Equal(expectedResult, formInput.StateCode);
        }

        [Theory]
        [InlineData(27, 17)]
        [InlineData(7, 1)]
        [InlineData(9, 1)]
        [InlineData(0, 0)]
        public void MapFormRequestToFormInput_ProspectFlowTypeIdIsMapped(int applicationId, int expectedResult)
        {
            var formRequest = new FormRequest
            {
                ApplicationId = applicationId
            };

            var formInput = MapRequest(formRequest);

            Assert.Equal(expectedResult, formInput.ProspectFlowTypeId);
        }

        private FormInput MapRequest(string leadData)
        {
            var request = new FormRequest
            {
                LeadData = leadData
            };

            var formInput = MapRequest(request);

            return formInput;
        }

        private FormInput MapRequest()
        {
            FormRequest request = new FormRequest();
            var formInput = MapRequest(request);
            return formInput;
        }

        private FormInput MapRequest(FormRequest request)
        {
            var mockIPAddressService = GetMockIPAddressService();
            var mockLocationValidationService = new Mock<ILocationValidationService>();
            var mockHttpContext = GetHttpContextMock();

            var mapper = new FormInputMapper(mockIPAddressService.Object, mockLocationValidationService.Object);
            var formInput = mapper.MapFormRequestToFormInput(request, mockHttpContext.Object);
            return formInput;
        }

        private FormInput MapRequest(Mock<HttpContextBase> mockHttpContext)
        {
            FormRequest request = new FormRequest();

            var mockIPAddressService = GetMockIPAddressService();
            var mockLocationValidationService = new Mock<ILocationValidationService>();

            var mapper = new FormInputMapper(mockIPAddressService.Object, mockLocationValidationService.Object);
            var formInput = mapper.MapFormRequestToFormInput(request, mockHttpContext.Object);
            return formInput;
        }

        private Mock<IIPAddressService> GetMockIPAddressService(string ipAddress = "")
        {
            var mockIPAddressService = new Mock<IIPAddressService>();
            mockIPAddressService.Setup(s => s.GetIPAddress(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(ipAddress);

            return mockIPAddressService;
        }

        private Mock<HttpContextBase> GetHttpContextMock()
        {
            var mockContext = new Mock<HttpContextBase>();
            var mockRequest = GetHttpRequestMock();
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            return mockContext;
        }

        private Mock<HttpRequestBase> GetHttpRequestMock()
        {
            var mockRequest = new Mock<HttpRequestBase>();
            mockRequest.SetupGet(r => r.ServerVariables).Returns(new NameValueCollection());
            return mockRequest;
        }

        private Mock<HttpContextBase> GetHttpContextMock(string serverVariableName, string serverVariableValue)
        {
            var mockContext = new Mock<HttpContextBase>();
            var mockRequest = GetHttpRequestMock(serverVariableName, serverVariableValue);
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            return mockContext;
        }

        private Mock<HttpRequestBase> GetHttpRequestMock(string serverVariableName, string serverVariableValue)
        {
            var mockRequest = new Mock<HttpRequestBase>();
            mockRequest.SetupGet(r => r.ServerVariables).Returns(new NameValueCollection { { serverVariableName, serverVariableValue } });
            return mockRequest;
        }

        public static IEnumerable<object[]> GetPhoneTestCasesWithCountryParams(string phoneParamName)
        {
            var parameters = new List<object[]>();

            var countryCodes = new List<string> { "US", "CA" };
            var countryIds = new List<string> { "4", "5" };
            Dictionary<string, string> phoneNumbers = GetDirtyAndCleanPhoneNumberPairs();

            parameters.AddRange(GetPhoneTestParametersWithCountryCodeInLeadData(phoneNumbers, countryCodes, phoneParamName));
            parameters.AddRange(GetPhoneTestParametersWithCountryIdInLeadAdditionalData(phoneNumbers, countryIds, phoneParamName));

            return parameters;
        }

        private static List<object[]> GetPhoneTestParametersWithCountryCodeInLeadData(Dictionary<string, string> phoneNumbers, List<string> countryCodes, string phoneParamName)
        {
            var parameters = new List<object[]>();

            foreach (var countryCode in countryCodes)
            {
                foreach (var phoneNumberPair in phoneNumbers)
                {
                    var phoneNumberInputParam = phoneNumberPair.Key;
                    var cleanPhoneNumberExpectedResult = phoneNumberPair.Value;

                    parameters.Add(new object[] { "", $"Country={countryCode}&{phoneParamName}={phoneNumberInputParam}", cleanPhoneNumberExpectedResult });
                }
            }

            return parameters;
        }

        private static List<object[]> GetPhoneTestParametersWithCountryIdInLeadAdditionalData(Dictionary<string, string> phoneNumbers, List<string> countryIds, string phoneParamName)
        {
            var parameters = new List<object[]>();

            foreach (var countryId in countryIds)
            {
                foreach (var phoneNumberPair in phoneNumbers)
                {
                    var phoneNumberInputParam = phoneNumberPair.Key;
                    var cleanPhoneNumberExpectedResult = phoneNumberPair.Value;

                    parameters.Add(new object[] { $"Country-key={countryId}", $"{phoneParamName}={phoneNumberInputParam}", cleanPhoneNumberExpectedResult });
                }
            }

            return parameters;
        }

        private static Dictionary<string, string> GetDirtyAndCleanPhoneNumberPairs()
        {
            return new Dictionary<string, string>
            {
                { "5611234567", "5611234567" },
                { "561-123-4567", "5611234567" },
                { "561/123/4567", "5611234567" },
                { "(561)-123-4567", "5611234567" },
                { "(561)(123)(4567)", "5611234567" },
                { "15611234567", "5611234567" },
                { "56112345677", "5611234567" },
                { "1561-123-4567", "5611234567" },
                { "1561/123/4567", "5611234567" },
                { "1(561)-123-4567", "5611234567" },
                { "1(561)(123)(4567)", "5611234567" }
            };
        }
    }
}