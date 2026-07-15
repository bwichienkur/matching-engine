using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Core.DTO;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Outbound.Tests
{
    public class FormValidationResultMapperTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapAPIValidationResultDTOToFormValiationResultTest_ValidMapped(bool valid)
        {
            APIValidationResultDTO apiValidationResult = new APIValidationResultDTO { Valid = valid };

            FormValidationResult formValidationResult = MapAPIValidationResultDTOToFormValiationResult(apiValidationResult);

            Assert.Equal(valid, formValidationResult.Valid);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapAPIValidationResultDTOToFormValiationResultTest_TestLeadMapped(bool testLead)
        {
            APIValidationResultDTO apiValidationResult = new APIValidationResultDTO { IsTestLead = testLead };

            FormValidationResult formValidationResult = MapAPIValidationResultDTOToFormValiationResult(apiValidationResult);

            Assert.Equal(testLead, formValidationResult.IsTestLead);
        }

        [Fact]
        public void MapAPIValidationResultDTOToFormValiationResultTest_ValidationMessagesMapped()
        {
            APIValidationResultDTO apiValidationResult = new APIValidationResultDTO
            {
                ValidationMessages = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("TestLead", "Lead Is A Test") }
            };

            FormValidationResult formValidationResult = MapAPIValidationResultDTOToFormValiationResult(apiValidationResult);

            Assert.Equal(apiValidationResult.ValidationMessages, formValidationResult.ValidationMessages);
        }

        private FormValidationResult MapAPIValidationResultDTOToFormValiationResult(APIValidationResultDTO apiValidationResult)
        {
            var mapper = new FormValidationResultMapper();
            return mapper.MapAPIValidationResultDTOToFormValiationResult(apiValidationResult);
        }

    }
}