using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers.Inbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Inbound.Tests
{
    public class APIValidationResultMapperTests
    {
        [Fact]
        public void MapFormValidationResultToApiValidationResultTest_NotNull()
        {
            var formValidationResult = new FormValidationResult();

            var mapper = new APIValidationResultMapper();
            var apiValidationResult = mapper.MapFormValidationResultToApiValidationResult(formValidationResult);

            Assert.NotNull(apiValidationResult);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormValidationResultToApiValidationResultTest_ValidMapped(bool valid)
        {
            var formValidationResult = new FormValidationResult();
            formValidationResult.Valid = valid;

            var mapper = new APIValidationResultMapper();
            var apiValidationResult = mapper.MapFormValidationResultToApiValidationResult(formValidationResult);

            Assert.Equal(valid, apiValidationResult.Valid);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapFormValidationResultToApiValidationResultTest_IsTestLeadMapped(bool isTestLead)
        {
            var formValidationResult = new FormValidationResult();
            formValidationResult.IsTestLead = isTestLead;

            var mapper = new APIValidationResultMapper();
            var apiValidationResult = mapper.MapFormValidationResultToApiValidationResult(formValidationResult);

            Assert.Equal(isTestLead, apiValidationResult.IsTestLead);
        }

        [Fact]
        public void MapFormValidationResultToApiValidationResultTest_ValidationMessagesMapped()
        {
            var formValidationResult = new FormValidationResult();
            formValidationResult.ValidationMessages = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ValidationMessage", "Validation Failed") };

            var mapper = new APIValidationResultMapper();
            var apiValidationResult = mapper.MapFormValidationResultToApiValidationResult(formValidationResult);

            Assert.Equal(formValidationResult.ValidationMessages, apiValidationResult.ValidationMessages);
        }
    }
}