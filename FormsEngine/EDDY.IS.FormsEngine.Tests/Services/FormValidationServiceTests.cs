using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class FormValidationServiceTests
    {
        [Fact]
        public void ValidateFormTest()
        {
            var formInput = new FormInput();

            var mockFormValidationResult = new FormValidationResult()
            {
                Valid = true,
                IsTestLead = false,
                ValidationMessages = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ValidationMessage", "Test Validation Message") }
            };

            var performanceLog = new PerformanceLog();

            var mockFormValidationRepostitory = new Mock<IFormValidationRepository>();
            mockFormValidationRepostitory.Setup(r => r.ValidateForm(It.IsAny<FormInput>(), ref performanceLog)).Returns(mockFormValidationResult);

            var formValidationService = new FormValidationService(mockFormValidationRepostitory.Object);
            FormValidationResult actualResult = formValidationService.ValidateForm(formInput, ref performanceLog);

            Assert.Equal(mockFormValidationResult, actualResult);
        }
    }
}