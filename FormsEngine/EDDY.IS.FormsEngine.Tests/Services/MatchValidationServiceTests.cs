using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Models;
using Match = EDDY.IS.FormsEngine.Core.Models.Match;
using EDDY.IS.FormsEngine.Tests.TestData;
using FluentAssertions;
using EDDY.IS.FormsEngine.Core.DTO;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class MatchValidationServiceTests
    {
        private readonly MockMatchesTestData _mockMatchesTestData;

        public MatchValidationServiceTests()
        {
            _mockMatchesTestData = new MockMatchesTestData();
        }

        [Fact]
        public void GetMatchesThatFailedValidationTest_AllMatchesFail()
        {
            var formInput = GetFormInput();
            var failingMatches = formInput.Matches;

            List<Match> failedMatches = GetMatchesThatFailedValidation(formInput, failingMatches);

            failedMatches.Should().BeEquivalentTo(formInput.Matches);
        }

        [Fact]
        public void GetMatchesThatFailedValidationTest_NoMatchesFail()
        {
            var formInput = GetFormInput();
            var failingMatches = new List<Match>();

            List<Match> failedMatches = GetMatchesThatFailedValidation(formInput, failingMatches);

            failedMatches.Should().BeEquivalentTo(new List<Match>());
        }

        [Fact]
        public void GetMatchesThatFailedValidationTest_OneMatchFails()
        {
            var formInput = GetFormInput();
            List<Match> failingMatches = formInput.Matches.Select(m => m).Take(1).ToList();

            List<Match> failedMatches = GetMatchesThatFailedValidation(formInput, failingMatches);

            failedMatches.Should().BeEquivalentTo(failingMatches);
        }

        [Fact]
        public void GetMatchesThatFailedValidationTest_SomeMatchesFails()
        {
            var formInput = GetFormInput();
            List<Match> failingMatches = formInput.Matches.Select(m => m).Take(3).ToList();

            List<Match> failedMatches = GetMatchesThatFailedValidation(formInput, failingMatches);

            failedMatches.Should().BeEquivalentTo(failingMatches);
        }

        [Fact]
        public void GetMatchesThatPassedValidationTest_AllMatchesPass()
        {
            var formInput = GetFormInput();
            List<Match> failingMatches = new List<Match>();
            List<Match> matchesThatShouldvePassedValidation = formInput.Matches;

            List<Match> matchesThatPassedValidation = GetMatchesThatPassValidation(formInput, failingMatches);

            matchesThatPassedValidation.Should().BeEquivalentTo(matchesThatShouldvePassedValidation);
        }

        [Fact]
        public void GetMatchesThatPassedValidationTest_NoMatchesPass()
        {
            var formInput = GetFormInput();
            List<Match> failingMatches = formInput.Matches;
            List<Match> matchesThatShouldvePassedValidation = new List<Match>();

            List<Match> matchesThatPassedValidation = GetMatchesThatPassValidation(formInput, failingMatches);

            matchesThatPassedValidation.Should().BeEquivalentTo(matchesThatShouldvePassedValidation);
        }

        [Fact]
        public void GetMatchesThatPassedValidationTest_OneMatchPass()
        {
            var formInput = GetFormInput();
            List<Match> failingMatches = formInput.Matches.Take(4).ToList();
            List<Match> matchesThatShouldPassedValidation = formInput.Matches.Skip(4).Take(1).ToList();

            List<Match> matchesThatPassedValidation = GetMatchesThatPassValidation(formInput, failingMatches);

            matchesThatPassedValidation.Should().BeEquivalentTo(matchesThatShouldPassedValidation);
        }

        [Fact]
        public void GetMatchesThatPassedValidationTest_SomeMatchesPass()
        {
            var formInput = GetFormInput();
            List<Match> failingMatches = formInput.Matches.Take(3).ToList();
            List<Match> matchesThatShouldPassedValidation = formInput.Matches.Skip(3).Take(2).ToList();

            List<Match> matchesThatPassedValidation = GetMatchesThatPassValidation(formInput, failingMatches);

            matchesThatPassedValidation.Should().BeEquivalentTo(matchesThatShouldPassedValidation);
        }

        [Fact]
        public void ValidatedMatchesTest()
        {
            var matches = new List<Match>
            {
                new Match { InstitutionId = 8618, ProgramName = "Paid_Category_Program", ProgramId = 302601, ProgramProductId = 634949, ProgramTemplateId = 2 },
                new Match { InstitutionId = 8620, ProgramName = "Paid_Specialty_A_Program", ProgramId = 302603, ProgramProductId = 634951, ProgramTemplateId = 2 },
                new Match { InstitutionId = 8614, ProgramName = "Program_GSMatch1", ProgramId = 302597, ProgramProductId = 634945, ProgramTemplateId = 1 },
                new Match { InstitutionId = 8617, ProgramName = "Program_Match1Plus", ProgramId = 302600, ProgramProductId = 634948, ProgramTemplateId = 1 }
            };

            var validatedPrograms = new List<ValidatedProgram>
            {
                new ValidatedProgram { ProgramId = 302601, ProgramProductId = 634949, PassedValidation = false, IsExternalDuplicate = false, IsInternalDuplicate = false, PaidStatusType = 1, RuleFailures = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ProgramNotAvailable", "Program Not Available") } },
                new ValidatedProgram { ProgramId = 302603, ProgramProductId = 634951, PassedValidation = false, IsExternalDuplicate = true, IsInternalDuplicate = false, PaidStatusType = 2, RuleFailures = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ExternalDuplicate", "External Duplicate") } },
                new ValidatedProgram { ProgramId = 302597, ProgramProductId = 634945, PassedValidation = false, IsExternalDuplicate = false, IsInternalDuplicate = true, PaidStatusType = 3, RuleFailures = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("InternalDuplicate", "Internal Duplicate") } },
                new ValidatedProgram { ProgramId = 302600, ProgramProductId = 634948, PassedValidation = true, IsExternalDuplicate = false, IsInternalDuplicate = false, PaidStatusType = 3, AlternateProgramProductId = 564654 }
            };

            var formInput = new FormInput();
            formInput.Matches = matches;

            var mockProgramValidationService = new Mock<IProgramValidationService>();
            mockProgramValidationService.Setup(s => s.GetValidatedPrograms(It.IsAny<FormInput>())).Returns(validatedPrograms);

            var matchValidationService = new MatchValidationService(mockProgramValidationService.Object);
            matchValidationService.ValidateMatches(formInput);


            var expectedResult = new List<Match>
            {
                new Match { InstitutionId = 8618, ProgramName = "Paid_Category_Program", ProgramId = 302601, ProgramProductId = 634949, ProgramTemplateId = 2, PassedValidation = false, IsExternalDuplicate = false, IsInternalDuplicate = false, PaidStatusType = 1, RuleFailures = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ProgramNotAvailable", "Program Not Available") } },
                new Match { InstitutionId = 8620, ProgramName = "Paid_Specialty_A_Program", ProgramId = 302603, ProgramProductId = 634951, ProgramTemplateId = 2, PassedValidation = false, IsExternalDuplicate = true, IsInternalDuplicate = false, PaidStatusType = 2, RuleFailures = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ExternalDuplicate", "External Duplicate") } },
                new Match { InstitutionId = 8614, ProgramName = "Program_GSMatch1", ProgramId = 302597, ProgramProductId = 634945, ProgramTemplateId = 1, PassedValidation = false, IsExternalDuplicate = false, IsInternalDuplicate = true, PaidStatusType = 3, RuleFailures = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("InternalDuplicate", "Internal Duplicate") } },
                new Match { InstitutionId = 8617, ProgramName = "Program_Match1Plus", ProgramId = 302600, ProgramProductId = 634948, ProgramTemplateId = 1, PassedValidation = true, IsExternalDuplicate = false, IsInternalDuplicate = false, PaidStatusType = 3, AlternativeProgramProductId = 564654 }
            };

            formInput.Matches.Should().BeEquivalentTo(expectedResult);
        }

        private FormInput GetFormInput()
        {
            return new FormInput
            {
                Matches = _mockMatchesTestData.GetMockMatches(5)
            };
        }

        private List<Match> GetMatchesThatFailedValidation(FormInput formInput, List<Match> failingMatches)
        {
            var matchValidationService = GetMatchValidationService(formInput, failingMatches);
            return matchValidationService.GetMatchesThatFailedValidation(formInput);
        }

        private List<Match> GetMatchesThatPassValidation(FormInput formInput, List<Match> failingMatches)
        {
            var matchValidationService = GetMatchValidationService(formInput, failingMatches);
            return matchValidationService.GetMatchesThatPassedValidation(formInput);
        }

        private MatchValidationService GetMatchValidationService(FormInput formInput, List<Match> failingMatches)
        {
            IEnumerable<int> failingProgramIds = failingMatches.Select(m => m.ProgramId);

            var mockProgramValidationService = new Mock<IProgramValidationService>();
            mockProgramValidationService.Setup(s => s.GetProgramIdsThatFailedValidation(It.IsAny<FormInput>())).Returns(failingProgramIds);

            return new MatchValidationService(mockProgramValidationService.Object);
        }
    }
}