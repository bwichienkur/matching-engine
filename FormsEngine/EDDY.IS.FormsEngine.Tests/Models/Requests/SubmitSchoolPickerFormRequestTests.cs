using EDDY.IS.FormsEngine.Services.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDDY.IS.FormsEngine.Services.Models.Requests.Tests
{
    public class SubmitSchoolPickerFormRequestTests
    {
        [Fact]
        public void ToDTOTest_TemplateId_Mapped()
        {
            int templateId = 100;

            var request = new SubmitSchoolPickerFormRequest();
            request.TemplateId = templateId;

            var dto = request.ToDTO();

            Assert.Equal(templateId, dto.TemplateId);
        }

        [Theory]
        [InlineData("ORIGINAL")]
        [InlineData("WIZARDPROFESSIONALBOOTSTRAP")]
        [InlineData("PROGRAMWIZARD")]
        public void ToDTOTest_RenderingStrategy_Mapped(string renderingStrategy)
        {
            var request = new SubmitSchoolPickerFormRequest();
            request.RenderingStrategy = renderingStrategy;

            var dto = request.ToDTO();

            Assert.Equal(renderingStrategy, dto.RenderingStrategy);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToDTOTest_IsBeta_Mapped(bool isBeta)
        {
            var request = new SubmitSchoolPickerFormRequest();
            request.IsBeta = isBeta;

            var dto = request.ToDTO();

            Assert.Equal(isBeta, dto.IsBeta);
        }

        [Fact]
        public void ToDTOTest_TrackId_Mapped()
        {
            string trackId = "8E43C61A-7B96-481F-B6E3-6A71A7ABBF95";

            var request = new SubmitSchoolPickerFormRequest();
            request.TrackId = trackId;

            var dto = request.ToDTO();

            Assert.Equal(trackId, dto.TrackId);
        }

        [Fact]
        public void ToDTOTest_SessionId_Mapped()
        {
            string sessionId = "00000000-0000-0000-0000-0000-000000000000";

            var request = new SubmitSchoolPickerFormRequest();
            request.SessionId = sessionId;

            var dto = request.ToDTO();

            Assert.Equal(sessionId, dto.SessionId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(null)]
        public void ToDTOTest_ProspectId_Mapped(int? prospectId)
        {
            var request = new SubmitSchoolPickerFormRequest();
            request.ProspectId = prospectId;

            var dto = request.ToDTO();

            Assert.Equal(prospectId, dto.ProspectId);
        }

        [Fact]
        public void ToDTOTest_LeadData_Mapped()
        {
            string leadData = "LeadData";

            var request = new SubmitSchoolPickerFormRequest();
            request.LeadData = leadData;

            var dto = request.ToDTO();

            Assert.Equal(leadData, dto.LeadData);
        }

        [Fact]
        public void ToDTOTest_AdditionalData_Mapped()
        {
            string additionalData = "AdditionalData";

            var request = new SubmitSchoolPickerFormRequest();
            request.AdditionalData = additionalData;

            var dto = request.ToDTO();

            Assert.Equal(additionalData, dto.AdditionalData);
        }

        [Fact]
        public void ToDTOTest_ProgramSelections_Mapped()
        {
            string programSelections = "selections";

            var request = new SubmitSchoolPickerFormRequest();
            request.ProgramSelections = programSelections;

            var dto = request.ToDTO();

            Assert.Equal(programSelections, dto.ProgramSelections);
        }

        [Fact]
        public void ToDTOTest_ProgramProductSelections_Mapped()
        {
            string programSelections = "selections";

            var request = new SubmitSchoolPickerFormRequest();
            request.ProgramProductSelections = programSelections;

            var dto = request.ToDTO();

            Assert.Equal(programSelections, dto.ProgramProductSelections);
        }
    }
}