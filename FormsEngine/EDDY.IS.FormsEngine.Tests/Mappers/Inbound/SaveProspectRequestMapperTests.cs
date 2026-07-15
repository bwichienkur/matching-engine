using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.FormsEngine.Core.Models;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Inbound.Tests
{
    public class SaveProspectRequestMapperTests
    {
        [Fact]
        public void MapFormInputToSaveProspectRequestTest_NotNull()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.NotNull(saveProspectRequest);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectNotNull()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.NotNull(saveProspectRequest.Prospect);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectFlowDetailsNotNull()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.NotNull(saveProspectRequest.ProspectFlowDetails);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormInputToSaveProspectRequestTest_ProspectFlowDetailsTrackIdMapped(string trackId)
        {
            var formInput = GetFormInputWithProspect();

            Guid.TryParse(trackId, out Guid trackGuid);
            formInput.TrackId = trackGuid;

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.TrackId, saveProspectRequest.ProspectFlowDetails.TrackId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95")]
        public void MapFormInputToSaveProspectRequestTest_ProspectTrackingSessionGuidMapped(string sessionId)
        {
            var formInput = GetFormInputWithProspect();
            formInput.SessionId = sessionId;

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Guid? expectedResult = Guid.TryParse(formInput.SessionId, out Guid sessionGuid) ? sessionGuid : (Guid?)null;

            Assert.Equal(expectedResult, saveProspectRequest.ProspectFlowDetails.SessionGuid);
        }

        [Theory]
        [InlineData(17)]
        [InlineData(1)]
        public void MapFormInputToSaveProspectRequestTest_ProspectFlowTypeIdMapped(int prospectFlowTypeId)
        {
            var formInput = GetFormInputWithProspect();
            formInput.ProspectFlowTypeId = prospectFlowTypeId;

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(prospectFlowTypeId, saveProspectRequest.ProspectFlowDetails.ProspectFlowTypeId);
        }

        [Theory]
        [InlineData(new int[] { 1, 10, 21 }, 17, null)]
        [InlineData(new int[] { 7, 9, 12 }, 1, null)]
        [InlineData(new int[] { 8, 15, 26 }, 2, "8,15,26")]
        public void MapFormInputToSaveProspectRequestTest_ProspectFlowDetailsAreaofInterestMapped(int[] categoryIds, int prospectFlowTypeId, string expectedResult)
        {
            var formInput = GetFormInputWithProspect();
            formInput.ProspectFlowTypeId = prospectFlowTypeId;
            formInput.Categories = categoryIds.ToList();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(expectedResult, saveProspectRequest.ProspectFlowDetails.AreaofInterest);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectFirstNameMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.FirstName, saveProspectRequest.Prospect.FirstName);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectLastNameMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.LastName, saveProspectRequest.Prospect.LastName);
        }

        [Theory]
        [InlineData("test@gmail.com", "test@gmail.com")]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData(null, null)]
        public void MapFormInputToSaveProspectRequestTest_ProspectEmailAddressMapped(string emailAddress, string expectedResult)
        {
            var formInput = GetFormInputWithProspect();
            formInput.Prospect.Email = emailAddress;

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(expectedResult, saveProspectRequest.Prospect.Email);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectAddress1Mapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.Address1, saveProspectRequest.Prospect.Address1);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectAddress2Mapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.Address2, saveProspectRequest.Prospect.Address2);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectAgeMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.Age, saveProspectRequest.Prospect.Age);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectPhoneMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.Phone1, saveProspectRequest.Prospect.Phone);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectOtherPhoneMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.Phone2, saveProspectRequest.Prospect.OtherPhone);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectCityMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.City, saveProspectRequest.Prospect.City);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectCountryIdMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.CountryId, saveProspectRequest.Prospect.CountryID);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectStateIdMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.StateId, saveProspectRequest.Prospect.StateID);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectEducationLevelIdMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.EducationLevelId, saveProspectRequest.Prospect.EducationLevelID);
        }

        [Theory]
        [InlineData(2010, 2015, 2010)]
        [InlineData(2010, null, 2010)]
        [InlineData(null, 2015, 2015)]
        [InlineData(null, null, null)]
        public void MapFormInputToSaveProspectRequestTest_ProspectGraduationYearsMapped(int? highSchoolGraduationYear, int? graduationYear, int? expectedGraduationYear)
        {
            var formInput = GetFormInputWithProspect();
            formInput.Prospect.HSGraduationYear = highSchoolGraduationYear;
            formInput.Prospect.GenericGraduationYear = graduationYear;

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(expectedGraduationYear, saveProspectRequest.Prospect.GraduationYear);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectMilitaryStatusIdMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.MilitaryStatusId, saveProspectRequest.Prospect.MilitaryStatusID);
        }

        [Fact]
        public void MapFormInputToSaveProspectRequestTest_ProspectPostalCodeMapped()
        {
            var formInput = GetFormInputWithProspect();

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.PostalCode, saveProspectRequest.Prospect.PostalCode);
        }

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        [InlineData(false, true, true)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        public void MapFormInputToSaveProspectRequestTest_ProspectContactPreferencesMapped(bool preferEmail, bool preferPhone, bool preferText)
        {
            var formInput = GetFormInputWithProspect();
            formInput.Prospect.PreferEmail = preferEmail;
            formInput.Prospect.PreferPhone = preferPhone;
            formInput.Prospect.PreferText = preferText;

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(preferEmail, saveProspectRequest.Prospect.PreferEmail);
            Assert.Equal(preferPhone, saveProspectRequest.Prospect.PreferPhone);
            Assert.Equal(preferText, saveProspectRequest.Prospect.PreferText);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void MapFormInputToSaveProspectRequestTest_ProspectIsUSCitizenMapped(bool? isUSCitizen)
        {
            var formInput = GetFormInputWithProspect();
            formInput.Prospect.IsUSCitizen = isUSCitizen;

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.IsUSCitizen, saveProspectRequest.Prospect.IsUsCitizen);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void MapFormInputToSaveProspectRequestTest_ProspectNeedsFinancialAidMapped(bool? needsFinancialAid)
        {
            var formInput = GetFormInputWithProspect();
            formInput.Prospect.NeedsFinancialAid = needsFinancialAid;

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(formInput.Prospect.NeedsFinancialAid, saveProspectRequest.Prospect.NeedsFinancialAid);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData("immediately", "immediately")]
        public void MapFormInputToSaveProspectRequestTest_ProspectDesiredStartDateMapped(string desiredStartDate, string expectedResult)
        {
            var formInput = GetFormInputWithProspect();
            formInput.Prospect.DesiredStartDate = desiredStartDate;

            SaveProspectRequest saveProspectRequest = MapFormInputToSaveProspectRequest(formInput);

            Assert.Equal(expectedResult, saveProspectRequest.Prospect.DesiredStartDate);
        }

        private SaveProspectRequest MapFormInputToSaveProspectRequest(FormInput formInput)
        {
            var mapper = new SaveProspectRequestMapper();
            return mapper.MapFormInputToSaveProspectRequest(formInput);
        }

        private FormInput GetFormInputWithProspect()
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect
            {
                FirstName = "Test",
                LastName = "Tester",
                Address1 = "123 Straight Road",
                Address2 = "456 Round Road",
                Age = 25,
                Phone1 = "5611234567",
                Phone2 = "5615656566",
                City = "Boca Raton",
                CountryId = 4,
                StateId = 12,
                EducationLevelId = 7,
                MilitaryStatusId = 10,
                PostalCode = "33467"
            };

            return formInput;
        }

    }
}