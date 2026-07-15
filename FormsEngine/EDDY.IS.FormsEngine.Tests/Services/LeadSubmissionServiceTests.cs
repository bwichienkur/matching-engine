using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using Moq;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Tests.TestData;
using EDDY.IS.FormsEngine.Core.Models;
using Match = EDDY.IS.FormsEngine.Core.Models.Match;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.MatchingEngine;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class LeadSubmissionServiceTests
    {
        private static readonly MockMatchesTestData _mockMatchesTestData = new MockMatchesTestData();

        [Fact]
        public void SubmitSchoolPickerWizardTest_NotNull()
        {
            FormInput formInput = null;
            List<Lead> mockLeads = GetMockLeads(formInput, 0);

            SubmissionResponse response = SubmitSchoolPickerWizardLeads(formInput, mockLeads, out _);

            Assert.NotNull(response);
        }

        [Theory]
        [MemberData(nameof(GetTestParameters))]
        public void SubmitSchoolPickerWizardTest(FormInput formInput, int successfulLeadCount, bool success, bool moveToThankYou, bool moveToNoMatch)
        {
            List<Lead> mockLeads = GetMockLeads(formInput, successfulLeadCount);

            SubmissionResponse response = SubmitSchoolPickerWizardLeads(formInput, mockLeads, out Mock<ISessionService> mockSessionService);

            string expectedFullName = $"{formInput.Prospect.FirstName} {formInput.Prospect.LastName}";
            Assert.Equal(expectedFullName, response.UserFullName);
            Assert.Equal(formInput.Prospect.ProspectId, response.ProspectId);
            Assert.Equal(success, response.Success);
            Assert.Equal(moveToThankYou, response.MoveToThankYou);
            Assert.Equal(moveToNoMatch, response.MoveToNoMatch);
            Assert.Equal(mockLeads.Select(l => l).Where(l => l.Successful).Count(), response.SchoolPickerLeadsCreatedCount);
            mockSessionService.Verify(s => s.SetProgramIdsFromLeads(It.Is<string>(i => i.Equals(formInput.FESessionId)), It.Is<List<int>>(l => l.Count == successfulLeadCount)), Times.Once);
            mockSessionService.Verify(s => s.SetLeadIds(It.Is<string>(i => i.Equals(formInput.FESessionId)), It.Is<List<decimal>>(l => l.Count == successfulLeadCount)), Times.Once);
        }

        public static IEnumerable<object[]> GetTestParameters()
        {
            FormInput formInput = GetFormInput();

            int matchCount = formInput.Matches.Count;

            return new List<object[]>
            {
                new object[] { formInput, 0, false, false, true },
                new object[] { formInput, matchCount / 2, true, true, false },
                new object[] { formInput, matchCount, true, true, false }
            };
        }

        private static FormInput GetFormInput()
        {
            var formInput = new FormInput();
            formInput.Matches = _mockMatchesTestData.GetMockMatches(4);
            formInput.FormValidationResult = new FormValidationResult { Valid = true };
            formInput.FESessionId = "8E43C61A-7B96-481F-B6E3-6A71A7ABBF95";
            formInput.Prospect = new Prospect
            {
                FirstName = "Test",
                LastName = "Tester",
                ProspectId = 9842390
            };

            return formInput;
        }
        
        private List<Lead> GetMockLeads(FormInput formInput, int successfulLeads)
        {
            List<Lead> mockLeads = new List<Lead>();
            Queue<bool> successfulStatuses= new Queue<bool>();

            for (int i = 0; i < successfulLeads; i++)
            {
                successfulStatuses.Enqueue(true);
            }

            int matchCount = formInput?.Matches?.Count ?? 0;
            int unsuccessfulLeads = matchCount - successfulLeads;

            for (int x = 0; x < unsuccessfulLeads; x++)
            {
                successfulStatuses.Enqueue(false);
            }


            if (formInput?.Matches?.Count > 0)
            {
                var rand = new Random();
                mockLeads = formInput.Matches.Select(m => new Lead { LeadId = (decimal)rand.Next(), ProgramProductId = m.ProgramProductId, IsTestLead = false, Successful = successfulStatuses.Dequeue() }).ToList();
            }

            return mockLeads;
        }

        private SubmissionResponse SubmitSchoolPickerWizardLeads(FormInput formInput, List<Lead> mockLeads, out Mock<ISessionService> mockSessionService)
        {
            var mockLeadRepository = new Mock<ILeadRepository>();
            mockLeadRepository.Setup(r => r.SaveLeads(It.IsAny<FormInput>())).Returns(mockLeads);

            mockSessionService = new Mock<ISessionService>();
            mockSessionService.Setup(s => s.SetProgramIdsFromLeads(It.IsAny<string>(), It.IsAny<object>()));
            mockSessionService.Setup(s => s.SetLeadIds(It.IsAny<string>(), It.IsAny<object>()));

            var leadSubmissionService = new LeadSubmissionService(mockLeadRepository.Object, mockSessionService.Object);
            return leadSubmissionService.SaveSchoolPickerWizardLeads(formInput);
        }
    }
}