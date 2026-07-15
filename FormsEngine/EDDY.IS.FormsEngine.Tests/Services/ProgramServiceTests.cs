using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.Enums;
using FluentAssertions;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class ProgramServiceTests
    {
        [Fact]
        public void GetProgramsTest_FormInput()
        {
            var formInput = new FormInput();
            formInput.TrackId = new Guid("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95");
            formInput.ApplicationId = 1;
            formInput.IsBeta = false;
            var programIds = new List<int> { 100, 500 };

            var mockGetProgramResponse = GetMockGetProgramsResponse();

            var mockProgramRepository = new Mock<IProgramRepository>();
            mockProgramRepository.Setup(r => r.GetPrograms(It.Is<Guid>(g => g.Equals(formInput.TrackId)), It.Is<int>(a => a.Equals(formInput.ApplicationId)), It.Is<bool>(b => b.Equals(formInput.IsBeta)), It.Is<IEnumerable<int>>(e => e.Equals(programIds)), It.IsAny<bool>())).Returns(mockGetProgramResponse);
            var programService = new ProgramService(mockProgramRepository.Object);

            var programs = programService.GetPrograms(formInput, programIds);

            programs.Should().BeEquivalentTo(mockGetProgramResponse);
        }

        [Fact]
        public void GetProgramsTest_FormInput_NullParameters()
        {
            FormInput formInput = null;
            List<int> programIds = null;

            var mockGetProgramResponse = new List<Program>();

            var mockProgramRepository = new Mock<IProgramRepository>();
            //mockProgramRepository.Setup(r => r.GetPrograms(It.IsAny<FormInput>(), It.IsAny<IEnumerable<int>>(), It.IsAny<bool>())).Returns(mockGetProgramResponse);
            var programService = new ProgramService(mockProgramRepository.Object);

            var programs = programService.GetPrograms(formInput, programIds);

            programs.Should().BeEquivalentTo(mockGetProgramResponse);
            mockProgramRepository.Verify(r => r.GetPrograms(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<IEnumerable<int>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void GetProgramsTest_FormInput_EmptyProgramIdsList()
        {
            var formInput = new FormInput();
            formInput.TrackId = new Guid("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95");
            formInput.ApplicationId = 1;
            formInput.IsBeta = false;
            List<int> programIds = new List<int>();

            var mockProgramRepository = new Mock<IProgramRepository>();
            var programService = new ProgramService(mockProgramRepository.Object);

            var programs = programService.GetPrograms(formInput, programIds);

            var expectedResult = new List<Program>();
            programs.Should().BeEquivalentTo(expectedResult);
            mockProgramRepository.Verify(r => r.GetPrograms(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<IEnumerable<int>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void GetProgramsTest_FormInput_ApplicationIdIsZero()
        {
            var formInput = new FormInput();
            formInput.TrackId = new Guid("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95");
            formInput.ApplicationId = 0;
            formInput.IsBeta = false;
            var programIds = new List<int> { 100, 500 };

            var mockProgramRepository = new Mock<IProgramRepository>();
            var programService = new ProgramService(mockProgramRepository.Object);

            var programs = programService.GetPrograms(formInput, programIds);

            var expectedResult = new List<Program>();
            programs.Should().BeEquivalentTo(expectedResult);
            mockProgramRepository.Verify(r => r.GetPrograms(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<IEnumerable<int>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void GetProgramsTest_FormRequest()
        {
            var formRequest = new FormRequest();
            formRequest.TrackId = "8E43C61A-7B96-481F-B6E3-6A71A7ABBF95";
            formRequest.ApplicationId = 1;
            var programIds = new List<int> { 100, 500 };
            var trackGuid = new Guid(formRequest.TrackId);

            var mockGetProgramResponse = GetMockGetProgramsResponse();

            var mockProgramRepository = new Mock<IProgramRepository>();
            mockProgramRepository.Setup(r => r.GetPrograms(It.Is<Guid>(g => g.Equals(trackGuid)), It.Is<int>(a => a.Equals(formRequest.ApplicationId)), It.Is<bool>(b => b.Equals(formRequest.IsBeta)), It.Is<IEnumerable<int>>(e => e.Equals(programIds)), It.IsAny<bool>())).Returns(mockGetProgramResponse);
            var programService = new ProgramService(mockProgramRepository.Object);

            var programs = programService.GetPrograms(formRequest, programIds);

            programs.Should().BeEquivalentTo(mockGetProgramResponse);
        }

        [Fact]
        public void GetProgramsTest_FormRequest_NullParameters()
        {
            FormRequest formRequest = null;
            List<int> programIds = null;

            var mockGetProgramResponse = new List<Program>();

            var mockProgramRepository = new Mock<IProgramRepository>();
            var programService = new ProgramService(mockProgramRepository.Object);

            var programs = programService.GetPrograms(formRequest, programIds);

            programs.Should().BeEquivalentTo(mockGetProgramResponse);
            mockProgramRepository.Verify(r => r.GetPrograms(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<IEnumerable<int>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void GetProgramsTest_FormRequest_EmptyProgramIdsList()
        {
            var formRequest = new FormRequest();
            formRequest.TrackId = "8E43C61A-7B96-481F-B6E3-6A71A7ABBF95";
            formRequest.ApplicationId = 1;
            formRequest.IsBeta = false;
            List<int> programIds = new List<int>();

            var mockProgramRepository = new Mock<IProgramRepository>();
            var programService = new ProgramService(mockProgramRepository.Object);

            var programs = programService.GetPrograms(formRequest, programIds);

            var expectedResult = new List<Program>();
            programs.Should().BeEquivalentTo(expectedResult);
            mockProgramRepository.Verify(r => r.GetPrograms(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<IEnumerable<int>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void GetProgramsTest_FormRequest_ApplicationIdIsZero()
        {
            var formRequest = new FormRequest();
            formRequest.TrackId = "8E43C61A-7B96-481F-B6E3-6A71A7ABBF95";
            formRequest.ApplicationId = 0;
            formRequest.IsBeta = false;
            var programIds = new List<int> { 100, 500 };

            var mockProgramRepository = new Mock<IProgramRepository>();
            var programService = new ProgramService(mockProgramRepository.Object);

            var programs = programService.GetPrograms(formRequest, programIds);

            var expectedResult = new List<Program>();
            programs.Should().BeEquivalentTo(expectedResult);
            mockProgramRepository.Verify(r => r.GetPrograms(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<IEnumerable<int>>(), It.IsAny<bool>()), Times.Never);
        }

        private List<Program> GetMockGetProgramsResponse()
        {
            return new List<Program>
            {
                new Program
                {
                    ProgramId = 100,
                    ProgramName = "First Test Program",
                    ProgramDescription = "The First Test Program",
                    ProductId = 200,
                    CampusId = 300,
                    CampusName = "First Test Campus",
                    CampusType = CampusType.Ground,
                    CampusLogoUrl = "www.firsttestcampuslogourl.com",
                    InstitutionId = 400,
                    InstitutionName = "First Test Institution",
                    InstitutionLogoUrl = "www.testinstitutionlogourl.com",
                    InstitutionDescription = "The First Test Institution"
                },
                new Program
                {
                    ProgramId = 500,
                    ProgramName = "Second Test Program",
                    ProgramDescription = "The Second Test Program",
                    ProductId = 600,
                    CampusId = 700,
                    CampusName = "Second Test Campus",
                    CampusType = CampusType.Online,
                    CampusLogoUrl = "www.secondtestcampuslogourl.com",
                    InstitutionId = 800,
                    InstitutionName = "Test Institution",
                    InstitutionLogoUrl = "www.testinstitutionlogourl.com",
                    InstitutionDescription = "Just A Test Institution"
                }
            };
        }

    }
}