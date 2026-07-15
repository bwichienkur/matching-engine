using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Models;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class LocationValidationServiceTests
    {
        [Theory]
        [InlineData("US", "FL", 1, 2)]
        [InlineData("US", "FL", null, 3)]
        [InlineData("US", "FL", 4, null)]
        [InlineData("US", "FL", null, null)]
        public void GetValidLocationTest(string countryCode, string stateCode, int? countryId, int? stateId)
        {
            var mockReturnCountryIdAndStateId = new Tuple<int?, int?>(countryId, stateId);

            var mockLocationValidationRepository = new Mock<ILocationValidationRepository>();
            mockLocationValidationRepository.Setup(r => r.GetValidCountryIdAndStateId(It.IsAny<string>(), It.IsAny<string>())).Returns(mockReturnCountryIdAndStateId);

            var locationValidationService = new LocationValidationService(mockLocationValidationRepository.Object);
            Location location = locationValidationService.GetValidLocation(countryCode, stateCode);

            Assert.Equal(countryId, location.CountryId);
            Assert.Equal(stateId, location.StateId);
        }

        [Theory]
        [InlineData(null, "", "")]
        [InlineData("33411", "127.0.0.1", "33411")]
        public void GetPostalCodeTest(string mockPostalCode, string ipAddress, string expectedResult)
        {
            var mockLocationValidationRepository = new Mock<ILocationValidationRepository>();
            mockLocationValidationRepository.Setup(r => r.GetPostalCode(It.Is<string>(s => s.Equals(ipAddress)))).Returns(mockPostalCode);
            var locationValidationService = new LocationValidationService(mockLocationValidationRepository.Object);

            var postalCode = locationValidationService.GetPostalCode(ipAddress);

            Assert.Equal(expectedResult, postalCode);
        }
    }
}