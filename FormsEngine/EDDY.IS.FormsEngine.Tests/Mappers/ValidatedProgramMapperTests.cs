using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.Core.DTO;
using FluentAssertions;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Tests
{
    public class ValidatedProgramMapperTests
    {
        [Fact]
        public void MapProgramValidateResponseToValidatedProgramTest_NotNull()
        {
            var programValidateResponse = new ProgramValidateResponse();

            ValidatedProgram validatedProgram = MapProgramValidateResponseToValidatedProgram(programValidateResponse);

            Assert.NotNull(validatedProgram);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(12345)]
        public void MapProgramValidateResponseToValidatedProgramTest_ProgramId(int? programId)
        {
            var programValidateResponse = new ProgramValidateResponse();
            programValidateResponse.ProgramId = programId;

            ValidatedProgram validatedProgram = MapProgramValidateResponseToValidatedProgram(programValidateResponse);

            Assert.Equal(validatedProgram.ProgramId, programId);
        }

        [Theory]
        [InlineData(100, null)]
        [InlineData(200, 0)]
        [InlineData(123456, 123456)]
        public void MapProgramValidateResponseToValidatedProgramTest_ProgramProductId(int programProductId, int? programProductIdFromResponse)
        {
            var programValidateResponse = new ProgramValidateResponse();
            programValidateResponse.ProgramProductId = programProductIdFromResponse;

            ValidatedProgram validatedProgram = MapProgramValidateResponseToValidatedProgram(programValidateResponse, programProductId);

            Assert.Equal(validatedProgram.ProgramProductId, programProductId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapProgramValidateResponseToValidatedProgramTest_PassedValidation(bool passedValidation)
        {
            var programValidateResponse = new ProgramValidateResponse();
            programValidateResponse.PassedValidation = passedValidation;

            ValidatedProgram validatedProgram = MapProgramValidateResponseToValidatedProgram(programValidateResponse);

            Assert.Equal(validatedProgram.PassedValidation, passedValidation);
        }

        [Theory]
        [InlineData(PaidStatusType.Paid)]
        [InlineData(PaidStatusType.Fraid)]
        [InlineData(PaidStatusType.Free)]
        [InlineData(null)]
        public void MapProgramValidateResponseToValidatedProgramTest_PaidStatusType(PaidStatusType? paidStatusType)
        {
            var programValidateResponse = new ProgramValidateResponse();
            programValidateResponse.PaidStatusTypeId = paidStatusType;

            ValidatedProgram validatedProgram = MapProgramValidateResponseToValidatedProgram(programValidateResponse);

            Assert.Equal(validatedProgram.PaidStatusType, (int?) paidStatusType);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(null)]
        public void MapProgramValidateResponseToValidatedProgramTest_AlternateProgramProductId(int? alternateProgramProductId)
        {
            var programValidateResponse = new ProgramValidateResponse();
            programValidateResponse.AlternateProgramProductId = alternateProgramProductId;

            ValidatedProgram validatedProgram = MapProgramValidateResponseToValidatedProgram(programValidateResponse);

            Assert.Equal(validatedProgram.AlternateProgramProductId, alternateProgramProductId);
        }

        [Fact]
        public void MapProgramValidateResponseToValidatedProgramTest_IsInternalDuplicate()
        {
            var programValidateResponse = new ProgramValidateResponse();
            programValidateResponse.RuleFailures = new RuleFailure[] { new RuleFailure() { RuleFailureType = BaseRuleType.InternalDuplicate } };

            ValidatedProgram validatedProgram = MapProgramValidateResponseToValidatedProgram(programValidateResponse);

            Assert.True(validatedProgram.IsInternalDuplicate);
        }

        [Fact]
        public void MapProgramValidateResponseToValidatedProgramTest_IsExternalDuplicate()
        {
            var programValidateResponse = new ProgramValidateResponse();
            programValidateResponse.RuleFailures = new RuleFailure[] { new RuleFailure() { RuleFailureType = BaseRuleType.ExternalDuplicate } };

            ValidatedProgram validatedProgram = MapProgramValidateResponseToValidatedProgram(programValidateResponse);

            Assert.True(validatedProgram.IsExternalDuplicate);
        }

        [Fact]
        public void MapProgramValidateResponseToValidatedProgramTest_RuleFailures()
        {
            var programValidateResponse = new ProgramValidateResponse();
            programValidateResponse.RuleFailures = new RuleFailure[] {
                new RuleFailure() { RuleFailureType = BaseRuleType.ExternalDuplicate, RuleFailureName = "External Duplicate" },
                new RuleFailure() { RuleFailureType = BaseRuleType.ProgramNotAvailable, RuleFailureName = "Program Not Available" }
            };

            var expectedResult = new List<KeyValuePair<string, string>>();
            foreach (var ruleFailure in programValidateResponse.RuleFailures)
            {
                expectedResult.Add(new KeyValuePair<string, string>(ruleFailure.RuleFailureType.ToString(), ruleFailure.RuleFailureName));
            }

            ValidatedProgram validatedProgram = MapProgramValidateResponseToValidatedProgram(programValidateResponse);

            validatedProgram.RuleFailures.Should().Equal(expectedResult);
        }

        private ValidatedProgram MapProgramValidateResponseToValidatedProgram(ProgramValidateResponse programValidateResponse, int programProductId = 1)
        {
            var mapper = new ValidatedProgramMapper();
            return mapper.MapProgramValidateResponseToValidatedProgram(programProductId, programValidateResponse);
        }
    }
}