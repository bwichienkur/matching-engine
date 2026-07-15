using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.Interfaces;
using Moq;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class ProspectServiceTests
    {
        [Theory]
        [InlineData(123356)]
        public void SaveProspectTest(int expectedProspectId)
        {
            var formInput = new FormInput();
            var mockProspectRepository = new Mock<IProspectRepository>();
            mockProspectRepository.Setup(r => r.SaveProspect(It.IsAny<FormInput>())).Returns(expectedProspectId);

            var prospectService = new ProspectService(mockProspectRepository.Object);
            var actualProspectId = prospectService.SaveProspect(formInput);

            Assert.Equal(expectedProspectId, actualProspectId);
        }

        [Fact]
        public void SaveProspectAsyncTest()
        {
            var formInput = new FormInput();
            var mockProspectRepository = new Mock<IProspectRepository>();
            var prospectService = new ProspectService(mockProspectRepository.Object);

            prospectService.SaveProspectAsync(formInput);

            mockProspectRepository.Verify(r => r.SaveProspectAsync(It.IsAny<FormInput>()), Times.Once());
        }
    }
}