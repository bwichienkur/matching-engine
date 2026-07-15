using Xunit;
using EDDY.IS.FormsEngine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Tests.Mocks;
using FluentAssertions;

namespace EDDY.IS.FormsEngine.Core.Services.Tests
{
    public class InstitutionServiceTests
    {
        private readonly FormRequest _formRequest = new FormRequest
        {
            IsBeta = false,
            TrackId = "8E43C61A-7B96-481F-B6E3-6A71A7ABBF95",
            ApplicationId = 1
        };

        private readonly FormInput _formInput = new FormInput
        {
            IsBeta = false,
            TrackId = new Guid("8E43C61A-7B96-481F-B6E3-6A71A7ABBF95"),
            ApplicationId = 1
        };

        [Fact]
        public void GetInstitutionTest_ReturnsInstitutionProvidedByRepository()
        {
            int institutionId = 1;

            _formRequest.InstitutionId = institutionId;

            Institution testInstitution = GetTestInstitutions().First();

            var mockRepository = new Mock<IInstitutionRepository>();
            mockRepository.Setup(m => m.GetInstitution(It.Is<FormRequest>(r => r == _formRequest))).Returns(testInstitution);

            InstitutionService service = GetInstitutionService(mockRepository);
            
            Institution actualResult = service.GetInstitution(_formRequest);
            Institution expectedResult = GetTestInstitutionsExpectedResult().First();

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetInstitutionTest_ReturnsEmptyInstitutionWhenRequestIsNull()
        {
            FormRequest request = null;

            Institution testInstitution = null;

            var mockRepository = new Mock<IInstitutionRepository>();
            mockRepository.Setup(m => m.GetInstitution(It.IsAny<FormRequest>())).Returns(testInstitution);

            InstitutionService service = GetInstitutionService(mockRepository);

            Institution actualResult = service.GetInstitution(request);
            Institution expectedResult = new Institution();

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetInstitutionsTest_ReturnsListOfInstitutionsProvidedByRepository()
        {
            var mockRepository = new Mock<IInstitutionRepository>();
            mockRepository.Setup(m => m.GetInstitutions(It.Is<FormInput>(r => r == _formInput))).Returns(GetTestInstitutions());

            var service = GetInstitutionService(mockRepository);

            List<Institution> actualResult = service.GetInstitutions(_formInput);

            List<Institution> expectedResult = GetTestInstitutionsExpectedResult();

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetInstitutionsTest_ReturnsEmptyListOfInstitutionsWhenRequestIsNull()
        {
            FormInput request = null;

            List<Institution> testInstitutions = null;

            var mockRepository = new Mock<IInstitutionRepository>();
            mockRepository.Setup(m => m.GetInstitutions(It.Is<FormInput>(r => r == request))).Returns(testInstitutions);

            var service = GetInstitutionService(mockRepository);

            List<Institution> actualResult = service.GetInstitutions(request);
            List<Institution> expectedResult = new List<Institution>();

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        private InstitutionService GetInstitutionService(Mock<IInstitutionRepository> mockRepository)
        {
            var logoFormatterService = MockHelper.GetLogoUrlFormattingService();
            return new InstitutionService(mockRepository.Object, logoFormatterService);
        }

        private List<Institution> GetTestInstitutionsExpectedResult()
        {
            return new List<Institution>
            {
                new Institution
                {
                    InstitutionId = 1,
                    InstitutionName = "First Institution",
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/8324/Logo_240x80.gif?1531394702",
                    InstitutionDescription = "This is the first institution",
                    Programs = GetTestPrograms()
                },
                new Institution
                {
                    InstitutionId = 2,
                    InstitutionName = "Second Institution",
                    InstitutionLogoUrl = "https://logo.educationdynamics.com/123/Logo_240x80.gif?123456789",
                    InstitutionDescription = "This is the second institution",
                    Programs = GetTestPrograms()
                }
            };
        }

        private List<Institution> GetTestInstitutions()
        {
            return new List<Institution>
            {
                new Institution
                {
                    InstitutionId = 1,
                    InstitutionName = "First Institution",
                    InstitutionLogoUrl = "/8324/{FILENAME}?1531394702",
                    InstitutionDescription = "This is the first institution",
                    Programs = GetTestPrograms()
                },
                new Institution
                {
                    InstitutionId = 2,
                    InstitutionName = "Second Institution",
                    InstitutionLogoUrl = "/123/{FILENAME}?123456789",
                    InstitutionDescription = "This is the second institution",
                    Programs = GetTestPrograms()
                }
            };
        }

        private List<Program> GetTestPrograms()
        {
            return new List<Program>
            {
                new Program
                {
                    ProgramId = 1,
                    ProgramName = "First Program",
                    ProgramProductId = 11,
                    ProgramTemplateId = 111
                },
                new Program
                {
                    ProgramId = 2,
                    ProgramName = "Second Program",
                    ProgramProductId = 22,
                    ProgramTemplateId = 222
                },
                new Program
                {
                    ProgramId = 3,
                    ProgramName = "Third Program",
                    ProgramProductId = 33,
                    ProgramTemplateId = 333
                }
            };
        }

    }
}