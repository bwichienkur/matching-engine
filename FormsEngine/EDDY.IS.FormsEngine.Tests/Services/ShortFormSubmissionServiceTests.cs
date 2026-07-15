using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.DTO.ShortFormSubmission;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.Tests.TestData;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class ShortFormSubmissionServiceTests
    {

        //[Theory]
        //[MemberData(nameof(ShortFormSubmissionServiceTestData.GetProgramValidationResponses), MemberType = typeof(ShortFormSubmissionServiceTestData))]
        //public void SubmitFormTest_ProgramValidationResponses(List<ProgramValidateResponse> programValidationResponses, bool expectedResult)
        //{
        //    var externalProgramValidationServiceMock = new Mock<IProgramValidation>();

        //    var mockResponseQueue = new Queue<ProgramValidateResponse>();

        //    foreach (var programValidationResponse in programValidationResponses)
        //    {
        //        mockResponseQueue.Enqueue(programValidationResponse);
        //    }

        //    externalProgramValidationServiceMock.Setup(m => m.ValidateBusinessRulesForProgram(It.IsAny<ProgramValidateRequest>(), It.IsAny<bool>()))
        //        .Returns(mockResponseQueue.Dequeue);

        //    var programValidationService = new ProgramValidationService(externalProgramValidationServiceMock.Object);

        //    ShortFormSubmissionRequestDTO dto = GetTestRequestDTO();

        //    var submissionService = new ShortFormSubmissionService(programValidationService);
        //    var response = submissionService.SubmitForm(dto);

        //    Assert.Equal(expectedResult, response.Successful);
        //}


        //private ShortFormSubmissionRequestDTO GetTestRequestDTO()
        //{
        //    ShortFormSubmissionRequestDTO dto = new ShortFormSubmissionRequestDTO();
        //    dto.ProgramSelections = "{\"8601\":302054,\"8603\":302056}";
        //    dto.ProgramProductSelections = "{\"8601\":631540,\"8603\":631542}";
        //    return dto;
        //}

    }
}