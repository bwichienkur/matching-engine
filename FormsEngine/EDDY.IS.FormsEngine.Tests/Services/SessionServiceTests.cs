using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using FluentAssertions;
using EDDY.IS.FormsEngine.DTO.Extended;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class SessionServiceTests
    {
        private const string _feSessionId = "8E43C61A-7B96-481F-B6E3-6A71A7ABBF95";

        [Fact]
        public void GetProspectFlowIdTest()
        {
            int prospectFlowId = 1111;

            var mockSessionRepository = new Mock<ISessionRepository>();
            mockSessionRepository.Setup(r => r.GetProspectFlowId(It.IsAny<string>())).Returns(prospectFlowId);

            var sessionService = new SessionService(mockSessionRepository.Object);

            int result = sessionService.GetProspectFlowId(_feSessionId);

            Assert.Equal(prospectFlowId, result);
        }

        [Fact]
        public void SetProgramIdsFromLeadsTest()
        {
            var mockSessionRepository = new Mock<ISessionRepository>();
            var sessionService = new SessionService(mockSessionRepository.Object);

            var programIds = new List<int> { 942092, 834834, 843298 };

            sessionService.SetProgramIdsFromLeads(_feSessionId, programIds);

            mockSessionRepository.Verify(r => r.SetProgramIdsFromLeads(It.Is<string>(s => s.Equals(_feSessionId)), It.Is<object>(o => o.Equals(programIds))), Times.Once);
        }

        [Theory]
        [InlineData(new int[] { 100, 200, 300 }, new int[] { 100, 200, 300 })]
        [InlineData(new int[0], new int[0])]
        [InlineData(null, new int[0])]
        [InlineData(0, new int[0])]
        [InlineData("nonsense", new int[0])]
        public void GetProgramIdsFromLeadsTest(object mockResponse, IEnumerable<int> expectedResult)
        {
            var mockSessionRepository = new Mock<ISessionRepository>();
            mockSessionRepository.Setup(r => r.GetProgramIdsFromLeads(_feSessionId)).Returns(mockResponse);
            var sessionService = new SessionService(mockSessionRepository.Object);

            List<int> programIds = sessionService.GetProgramIdsFromLeads(_feSessionId);

            programIds.Should().BeEquivalentTo(expectedResult.ToList());
            mockSessionRepository.Verify(r => r.GetProgramIdsFromLeads(It.Is<string>(s => s.Equals(_feSessionId))), Times.Once);
        }

        [Fact]
        public void SetLeadIdsTest()
        {
            var mockSessionRepository = new Mock<ISessionRepository>();
            var sessionService = new SessionService(mockSessionRepository.Object);

            var leadIds = new List<int> { 942092, 834834, 843298 };

            sessionService.SetLeadIds(_feSessionId, leadIds);

            mockSessionRepository.Verify(r => r.SetLeadIds(It.Is<string>(s => s.Equals(_feSessionId)), It.Is<object>(o => o.Equals(leadIds))), Times.Once);
        }

        [Theory]
        [InlineData(new int[] { 100, 200, 300 }, new int[] { 100, 200, 300 })]
        [InlineData(new int[0], new int[0])]
        [InlineData(null, new int[0])]
        [InlineData(0, new int[0])]
        [InlineData("nonsense", new int[0])]
        public void GetLeadIdsTest(object mockResponse, IEnumerable<int> expectedResult)
        {
            var mockEnumerableResponses = mockResponse as IEnumerable<int>;
            mockResponse = mockEnumerableResponses != null ? mockEnumerableResponses.Select(i => (decimal)i) : mockResponse;

            var mockSessionRepository = new Mock<ISessionRepository>();
            mockSessionRepository.Setup(r => r.GetLeadIds(_feSessionId)).Returns(mockResponse);
            var sessionService = new SessionService(mockSessionRepository.Object);

            List<decimal> leadIds = sessionService.GetLeadIds(_feSessionId);

            leadIds.Should().BeEquivalentTo(expectedResult.Select(r => (decimal)r).ToList());
            mockSessionRepository.Verify(r => r.GetLeadIds(It.Is<string>(s => s.Equals(_feSessionId))), Times.Once);
        }

        [Fact]
        public void GetUserFullNameTest_NotNull()
        {
            FormsEngineWorkflowStatus mockRepositoryResponse = null;
            var sessionService = GetUserFullNameSessionService(mockRepositoryResponse);

            string userFullName = sessionService.GetUserFullName(_feSessionId);

            Assert.NotNull(userFullName);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("   ", "")]
        [InlineData("Test Tester", "Test Tester")]
        public void GetUserFullNameTest(string userFullName, string expectedResult)
        {
            FormsEngineWorkflowStatus mockRepositoryResponse = new FormsEngineWorkflowStatus
            {
                UserFullName = userFullName
            };

            var sessionService = GetUserFullNameSessionService(mockRepositoryResponse);

            string actualResult = sessionService.GetUserFullName(_feSessionId);

            Assert.Equal(expectedResult, actualResult);
        }


        private SessionService GetUserFullNameSessionService(FormsEngineWorkflowStatus mockRepositoryResponse)
        {
            var mockSessionRepository = new Mock<ISessionRepository>();
            mockSessionRepository.Setup(r => r.GetFormsEngineWorkflowStatus(It.Is<string>(i => i.Equals(_feSessionId)))).Returns(mockRepositoryResponse);
            return new SessionService(mockSessionRepository.Object);
        }

    }
}