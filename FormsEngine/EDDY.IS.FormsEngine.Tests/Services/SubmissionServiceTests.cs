using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Tests.TestData;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.DTO;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class SubmissionServiceTests
    {
        private readonly MockMatchesTestData _mockMatchesTestData;

        public SubmissionServiceTests()
        {
            _mockMatchesTestData = new MockMatchesTestData();
        }

        [Fact]
        public void SubmitFormTest_ValidatesMatches()
        {
            var formInput = GetFormInputWithMatches();
            var performanceLog = new PerformanceLog();
            var submissionService = GetSubmissionService(performanceLog, out Mock<IMatchValidationService> mockMatchValidationService, out Mock<IFormValidationService> mockFormValidationService, out Mock<IProspectService> mockProspectService, out Mock<ILeadSubmissionService> mockLeadSubmissionService, out Mock<ISessionService> mockSessionService);

            submissionService.SubmitSchoolPickerWizard(formInput, ref performanceLog);

            mockMatchValidationService.Verify(s => s.ValidateMatches(It.IsAny<FormInput>()), Times.Once());
        }

        [Fact]
        public void SubmitFormTest_ValidateForm()
        {
            var formInput = GetFormInputWithMatches();
            var performanceLog = new PerformanceLog();
            var submissionService = GetSubmissionService(performanceLog, out _, out _, out _, out _, out _);

            submissionService.SubmitSchoolPickerWizard(formInput, ref performanceLog);

            Assert.True(formInput.FormValidationResult.Valid);
        }

        [Theory]
        [InlineData(null, 100000)]
        [InlineData(0, 200000)]
        public void SubmitFormTest_SavesNewProspectIfProspectIdNotPresent(int? prospectId, int expectedProspectId)
        {
            var formInput = GetFormInputWithMatches();
            formInput.Prospect = new Prospect { ProspectId = prospectId };

            var performanceLog = new PerformanceLog();
            var submissionService = GetSubmissionService(performanceLog, out Mock<IMatchValidationService> mockMatchValidationService, out Mock<IFormValidationService> mockFormValidationService, out Mock<IProspectService> mockProspectService, out Mock<ILeadSubmissionService> mockLeadSubmissionService, out Mock<ISessionService> mockSessionService);
            mockProspectService.Setup(s => s.SaveProspect(It.IsAny<FormInput>())).Returns(expectedProspectId);

            submissionService.SubmitSchoolPickerWizard(formInput, ref performanceLog);

            mockProspectService.Verify(s => s.SaveProspect(It.IsAny<FormInput>()), Times.Once());
            Assert.Equal(expectedProspectId, formInput.Prospect.ProspectId);
        }

        [Theory]
        [InlineData(100000)]
        [InlineData(1)]
        public void SubmitFormTest_UpdatesProspectIfProspectIdIsPresent(int? prospectId)
        {
            var formInput = GetFormInputWithMatches();
            formInput.Prospect = new Prospect { ProspectId = prospectId };

            var performanceLog = new PerformanceLog();
            var submissionService = GetSubmissionService(performanceLog, out Mock <IMatchValidationService> mockMatchValidationService, out Mock<IFormValidationService> mockFormValidationService, out Mock<IProspectService> mockProspectService, out Mock<ILeadSubmissionService> mockLeadSubmissionService, out Mock<ISessionService> mockSessionService);

            submissionService.SubmitSchoolPickerWizard(formInput, ref performanceLog);

            mockProspectService.Verify(s => s.SaveProspectAsync(It.IsAny<FormInput>()), Times.Once());
        }

        [Fact]
        public void SubmitFormTest_LeadCreationTypeSet()
        {
            var formInput = GetFormInputWithMatches();
            var performanceLog = new PerformanceLog();
            var submissionService = GetSubmissionService(performanceLog, out _, out _, out _, out _, out _);

            submissionService.SubmitSchoolPickerWizard(formInput, ref performanceLog);

            Assert.Equal(LeadCreationType.SchoolPickerUserSelection, formInput.LeadCreationType);
        }

        [Fact]
        public void SubmitFormTest_ProspectFlowIdSet()
        {
            
            var formInput = GetFormInputWithMatches();
            formInput.Prospect = new Prospect();
            var performanceLog = new PerformanceLog();
            int prospectFlowId = 1111;

            var submissionService = GetSubmissionService(performanceLog, out Mock<IMatchValidationService> mockMatchValidationService, out Mock<IFormValidationService> mockFormValidationService, out Mock<IProspectService> mockProspectService, out Mock<ILeadSubmissionService> mockLeadSubmissionService, out Mock<ISessionService> mockSessionService);

            mockSessionService.Setup(s => s.GetProspectFlowId(It.IsAny<string>())).Returns(prospectFlowId);

            submissionService.SubmitSchoolPickerWizard(formInput, ref performanceLog);

            Assert.Equal(prospectFlowId, formInput.ProspectFlowId);
        }

        [Fact]
        public void SubmitFormTest()
        {
            int prospectId = 1;

            var formInput = GetFormInputWithMatches();
            formInput.Prospect = new Prospect
            {
                FirstName = "Test",
                LastName = "Tester",
                ProspectId = prospectId
            };

            var performanceLog = new PerformanceLog();
            var submissionService = GetSubmissionService(performanceLog, out Mock<IMatchValidationService> mockMatchValidationService, out Mock<IFormValidationService> mockFormValidationService, out Mock<IProspectService> mockProspectService, out Mock<ILeadSubmissionService> mockLeadSubmissionService, out Mock<ISessionService> mockSessionService);

            var mockSubmissionResponse = new SubmissionResponse
            {
                ProspectId = prospectId,
                SchoolPickerLeadsCreatedCount = 10,
                Success = true,
                MoveToThankYou = true,
                UserFullName = $"{formInput.Prospect.FirstName} {formInput.Prospect.LastName}"
            };

            mockLeadSubmissionService.Setup(s => s.SaveSchoolPickerWizardLeads(It.IsAny<FormInput>())).Returns(mockSubmissionResponse);

            SubmissionResponse response = submissionService.SubmitSchoolPickerWizard(formInput, ref performanceLog);

            mockLeadSubmissionService.Verify(s => s.SaveSchoolPickerWizardLeads(It.IsAny<FormInput>()), Times.Once());
            Assert.Equal(mockSubmissionResponse, response);
        }

        private SubmissionService GetSubmissionService(PerformanceLog performanceLog, out Mock<IMatchValidationService> mockMatchValidationService, out Mock<IFormValidationService> mockFormValidationService, out Mock<IProspectService> mockProspectService, out Mock<ILeadSubmissionService> mockLeadSubmissionService, out Mock<ISessionService> mockSessionService)
        {
            mockMatchValidationService = new Mock<IMatchValidationService>();
            mockFormValidationService = GetFormValidationServiceMock(performanceLog);
            mockProspectService = new Mock<IProspectService>();
            mockLeadSubmissionService = new Mock<ILeadSubmissionService>();
            mockSessionService = new Mock<ISessionService>();

            return new SubmissionService(mockMatchValidationService.Object, mockFormValidationService.Object, mockProspectService.Object, mockLeadSubmissionService.Object, mockSessionService.Object);
        }

        private Mock<IFormValidationService> GetFormValidationServiceMock(PerformanceLog performanceLog)
        {
            var formValidationResult = new FormValidationResult()
            {
                Valid = true,
                IsTestLead = false,
                ValidationMessages = new List<KeyValuePair<string, string>>()
            };

            var mockFormValidationService = new Mock<IFormValidationService>();
            mockFormValidationService.Setup(s => s.ValidateForm(It.IsAny<FormInput>(), ref performanceLog)).Returns(formValidationResult);

            return mockFormValidationService;
        }

        private FormInput GetFormInputWithMatches()
        {
            return new FormInput
            {
                Matches = _mockMatchesTestData.GetMockMatches(5)
            };
        }
    }
}