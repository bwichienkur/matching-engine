using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Tests.TestData
{
    public static class ShortFormSubmissionServiceTestData
    {
        private static object[] GetSuccessfulProgramValidationResponses()
        {
            var programValidationResponses = new List<MatchingEngine.ProgramValidateResponse>();
            programValidationResponses.Add(new MatchingEngine.ProgramValidateResponse() { PassedValidation = true });
            programValidationResponses.Add(new MatchingEngine.ProgramValidateResponse() { PassedValidation = true });

            var expectedResult = true;

            return new object[] {
                programValidationResponses,
                expectedResult
            };
        }

        private static object[] GetFailedProgramValidationResponses()
        {
            var programValidationResponses = new List<MatchingEngine.ProgramValidateResponse>();
            programValidationResponses.Add(new MatchingEngine.ProgramValidateResponse() { PassedValidation = false });
            programValidationResponses.Add(new MatchingEngine.ProgramValidateResponse() { PassedValidation = false });

            var expectedResult = false;

            return new object[] {
                programValidationResponses,
                expectedResult
            };
        }

        private static object[] GetMixedProgramValidationResponsesFalseTrue()
        {
            var programValidationResponses = new List<MatchingEngine.ProgramValidateResponse>();
            programValidationResponses.Add(new MatchingEngine.ProgramValidateResponse() { PassedValidation = false });
            programValidationResponses.Add(new MatchingEngine.ProgramValidateResponse() { PassedValidation = true });

            var expectedResult = false;

            return new object[] {
                programValidationResponses,
                expectedResult
            };
        }

        private static object[] GetMixedProgramValidationResponsesTrueFalse()
        {
            var programValidationResponses = new List<MatchingEngine.ProgramValidateResponse>();
            programValidationResponses.Add(new MatchingEngine.ProgramValidateResponse() { PassedValidation = true });
            programValidationResponses.Add(new MatchingEngine.ProgramValidateResponse() { PassedValidation = false });

            var expectedResult = false;

            return new object[] {
                programValidationResponses,
                expectedResult
            };
        }

        public static IEnumerable<object[]> GetProgramValidationResponses()
        {
            yield return GetSuccessfulProgramValidationResponses();
            yield return GetFailedProgramValidationResponses();
            yield return GetMixedProgramValidationResponsesTrueFalse();
            yield return GetMixedProgramValidationResponsesFalseTrue();
        }
    }
}
