using EDDY.IS.FormsEngine.Core.DTO.ShortFormSubmission;
using EDDY.IS.FormsEngine.Core.Mappers;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Services.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDDY.IS.FormsEngine.Entities.Mappers.Tests
{
    public class ShortFormSubmissionMapperTests
    {
        [Fact]
        public void MapShortFormSubmissionTest_NotNull()
        {
            var dto = new ShortFormSubmissionRequestDTO();

            var mapper = new ShortFormSubmissionMapper();
            ShortFormSubmission submission = mapper.MapShortFormSubmission(dto);

            Assert.NotNull(submission);
        }

        [Fact]
        public void MapShortFormSubmissionTest_SchoolPickerUserSelections()
        {
            Dictionary<int, SchoolPickerUserSelection> expectedSelections = new Dictionary<int, SchoolPickerUserSelection>
            {
                { 8601, new SchoolPickerUserSelection() { InstitutionId = 8601, ProgramId = 302054, ProgramProductId = 631540 } },
                { 8603, new SchoolPickerUserSelection() { InstitutionId = 8603, ProgramId = 302056, ProgramProductId = 631542 } }
            };

            var dto = new ShortFormSubmissionRequestDTO();
            dto.ProgramSelections = "{\"8601\":302054,\"8603\":302056}";
            dto.ProgramProductSelections = "{\"8601\":631540,\"8603\":631542}";

            var mapper = new ShortFormSubmissionMapper();
            ShortFormSubmission submission = mapper.MapShortFormSubmission(dto);

            foreach (var selection in expectedSelections)
            {
                submission.SchoolPickerUserSelections.TryGetValue(selection.Key, out SchoolPickerUserSelection foundSelection);
                Assert.True(foundSelection.InstitutionId == selection.Value.InstitutionId && foundSelection.ProgramId == selection.Value.ProgramId && foundSelection.ProgramProductId == selection.Value.ProgramProductId);
            }
        }

        [Fact]
        public void MapShortFormSubmissionTest_TrackId()
        {
            var dto = new ShortFormSubmissionRequestDTO();
            dto.TrackId = "A0A51A62-5E1B-497B-9E31-18984C6F6CD8";

            var mapper = new ShortFormSubmissionMapper();
            ShortFormSubmission submission = mapper.MapShortFormSubmission(dto);

            Assert.Equal(dto.TrackId, submission.TrackId);
        }

        [Fact]
        public void MapShortFormSubmissionTest_Prospect_NotNull()
        {
            var dto = new ShortFormSubmissionRequestDTO();
            
            var mapper = new ShortFormSubmissionMapper();
            ShortFormSubmission submission = mapper.MapShortFormSubmission(dto);

            Assert.NotNull(submission.Prospect);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapShortFormSubmissionTest_IsBeta(bool isBeta)
        {
            var dto = new ShortFormSubmissionRequestDTO();
            dto.IsBeta = isBeta;

            var mapper = new ShortFormSubmissionMapper();
            ShortFormSubmission submission = mapper.MapShortFormSubmission(dto);

            Assert.Equal(isBeta, submission.IsBeta);
        }

    }
}