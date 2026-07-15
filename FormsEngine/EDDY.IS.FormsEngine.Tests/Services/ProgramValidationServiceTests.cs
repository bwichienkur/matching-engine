using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.Services;
using EDDY.IS.FormsEngine.Tests.TestData;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class ProgramValidationServiceTests
    {
        private readonly MockMatchesTestData _mockMatchesTestData;

        public ProgramValidationServiceTests()
        {
            _mockMatchesTestData = new MockMatchesTestData();
        }

        [Theory]
        [MemberData(nameof(GetMockValidProgramResponses))]
        public void GetProgramIdsThatFailedValidation(IEnumerable<bool> mockResponses)
        {
            var mockMatches = _mockMatchesTestData.GetMockMatches(3);

            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Matches = mockMatches;

            var validateProgramResponses = new Queue<ValidatedProgram>();
            var expectedResult = GetProgramsThatFailedValidationExpectedResult(mockResponses, formInput.Matches, validateProgramResponses);

            var mockProgramValidationRepository = new Mock<IProgramValidationRepository>();
            mockProgramValidationRepository.Setup(r => r.ValidateProgram(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Prospect>(), It.IsAny<bool>())).Returns(validateProgramResponses.Dequeue);

            var programValidationService = new ProgramValidationService(mockProgramValidationRepository.Object);

            var programIdsThatFailedValidation = programValidationService.GetProgramIdsThatFailedValidation(formInput);

            programIdsThatFailedValidation.Should().BeEquivalentTo(expectedResult.Select(p => p.ProgramId));
        }

        [Theory]
        [MemberData(nameof(GetMockValidProgramResponses))]
        public void GetProgramsThatFailedValidation(IEnumerable<bool> mockResponses)
        {
            var mockMatches = _mockMatchesTestData.GetMockMatches(3);

            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Matches = mockMatches;

            var validateProgramResponses = new Queue<ValidatedProgram>();
            var expectedResult = GetProgramsThatFailedValidationExpectedResult(mockResponses, formInput.Matches, validateProgramResponses);

            var programsThatFailedValidation = GetProgramsThatFailedValidationFromService(formInput, validateProgramResponses);

            programsThatFailedValidation.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetMockValidProgramResponses))]
        public void GetProgramsThatFailedValidation_NullFormInput(IEnumerable<bool> mockResponses)
        {
            var mockMatches = _mockMatchesTestData.GetMockMatches(3);

            FormInput formInput = null;

            var validateProgramResponses = new Queue<ValidatedProgram>();
            var expectedResult = GetProgramsThatFailedValidationExpectedResult(mockResponses, formInput?.Matches, validateProgramResponses);

            var programsThatFailedValidation = GetProgramsThatFailedValidationFromService(formInput, validateProgramResponses);

            programsThatFailedValidation.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetMockValidProgramResponses))]
        public void GetProgramsThatFailedValidation_MatchesNull(IEnumerable<bool> mockResponses)
        {
            var mockMatches = _mockMatchesTestData.GetMockMatches(3);

            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Matches = null;

            var validateProgramResponses = new Queue<ValidatedProgram>();
            var expectedResult = GetProgramsThatFailedValidationExpectedResult(mockResponses, formInput.Matches, validateProgramResponses);

            var programsThatFailedValidation = GetProgramsThatFailedValidationFromService(formInput, validateProgramResponses);

            programsThatFailedValidation.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetMockValidProgramResponses))]
        public void GetValidatedProgramsTest(IEnumerable<bool> mockResponses)
        {
            var formInput = new FormInput();
            formInput.Prospect = new Prospect();
            formInput.Matches = _mockMatchesTestData.GetMockMatches(3);

            var validateProgramResponses = new Queue<ValidatedProgram>();
            var expectedResult = GetValidatedProgramsExpectedResult(mockResponses, formInput.Matches, validateProgramResponses);

            var mockProgramValidationRepository = new Mock<IProgramValidationRepository>();
            mockProgramValidationRepository.Setup(r => r.ValidateProgram(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Prospect>(), It.IsAny<bool>())).Returns(validateProgramResponses.Dequeue);

            var programValidationService = new ProgramValidationService(mockProgramValidationRepository.Object);

            var actualResult = programValidationService.GetValidatedPrograms(formInput);

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        private List<ValidatedProgram> GetValidatedProgramsExpectedResult(IEnumerable<bool> mockValidationResponses, List<Models.Match> matches, Queue<ValidatedProgram> validateProgramResponses)
        {
            var validatedPrograms = new List<ValidatedProgram>();

            if (matches?.Count > 0)
            {
                for (int i = 0; i < mockValidationResponses.Count() && i < matches.Count; i++)
                {
                    bool passedValidation = mockValidationResponses.ElementAt(i);
                    var match = matches.ElementAt(i);

                    ValidatedProgram validatedProgram = new ValidatedProgram()
                    {
                        PassedValidation = passedValidation,
                        ProgramId = match.ProgramId,
                        ProgramProductId = match.ProgramProductId
                    };

                    validateProgramResponses.Enqueue(validatedProgram);
                    validatedPrograms.Add(validatedProgram);
                }
            }

            return validatedPrograms;
        }

        private List<Program> GetProgramsThatFailedValidationExpectedResult(IEnumerable<bool> mockValidationResponses, List<Models.Match> matches, Queue<ValidatedProgram> validateProgramResponses)
        {
            var programs = new List<Program>();

            if (matches?.Count > 0)
            {
                for (int i = 0; i < mockValidationResponses.Count(); i++)
                {
                    bool passedValidation = mockValidationResponses.ElementAt(i);

                    ValidatedProgram validatedProgram = new ValidatedProgram() { PassedValidation = passedValidation };

                    validateProgramResponses.Enqueue(validatedProgram);

                    if (!passedValidation)
                    {
                        var match = matches[i];
                        var program = new Program
                        {
                            ProgramId = match.ProgramId,
                            ProgramProductId = match.ProgramProductId,
                            InstitutionId = match.InstitutionId,
                        };
                        programs.Add(program);
                    }
                }
            }

            return programs;
        }

        private List<Program> GetProgramsThatFailedValidationFromService(FormInput formInput, Queue<ValidatedProgram> validateProgramResponses)
        {
            var mockProgramValidationRepository = new Mock<IProgramValidationRepository>();
            mockProgramValidationRepository.Setup(r => r.ValidateProgram(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Prospect>(), It.IsAny<bool>())).Returns(validateProgramResponses.Dequeue);

            var programValidationService = new ProgramValidationService(mockProgramValidationRepository.Object);

            return programValidationService.GetProgramsThatFailedValidation(formInput);
        }

        public static IEnumerable<object[]> GetMockValidProgramResponses()
        {
            return new[]
            {
                new object[] { new bool[] { true, true, true } },
                new object[] { new bool[] { false, true, true } },
                new object[] { new bool[] { false, false, true } },
                new object[] { new bool[] { false, false, false } },
                new object[] { new bool[] { true, false, false } },
                new object[] { new bool[] { true, true, false } }
            };
        }

    }
}