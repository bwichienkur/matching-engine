using Xunit;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.FormsEngine.Core.Models;
using FluentAssertions;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Outbound.Tests
{
    public class LeadMapperTests
    {
        [Fact]
        public void MapLeadCreateResponsesToLeadsTest_NotNull()
        {
            List<LeadCreateResponse> leadCreateResponses = null;

            var mapper = new LeadMapper();
            List<Lead> leads = mapper.MapLeadCreateResponsesToLeads(leadCreateResponses);

            Assert.NotNull(leads);
        }

        [Fact]
        public void MapLeadCreateResponsesToLeadsTest()
        {
            var leadCreateResponses = new List<LeadCreateResponse>
            {
                new LeadCreateResponse
                {
                    Lead = new LeadDTO { LeadId = 843290, ProgramProductId = 789328 },
                    IsTestLead = true,
                    Success = true
                },
                new LeadCreateResponse
                {
                    Lead = new LeadDTO { LeadId = 487946, ProgramProductId = 985666 },
                    IsTestLead = false,
                    Success = true
                },
                new LeadCreateResponse
                {
                    Lead = new LeadDTO { LeadId = 456554 },
                    IsTestLead = false,
                    Success = false
                }
            };

            var mapper = new LeadMapper();
            List<Lead> leads = mapper.MapLeadCreateResponsesToLeads(leadCreateResponses);

            var expectedResult = new List<Lead>
            {
                new Lead { LeadId = 843290, ProgramProductId = 789328, IsTestLead = true, Successful = true },
                new Lead { LeadId = 487946, ProgramProductId = 985666, IsTestLead = false, Successful = true },
                new Lead { LeadId = 456554, IsTestLead = false, Successful = false }
            };

            leads.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void MapLeadCreateResponseToLeadTest_NotNull()
        {
            LeadCreateResponse leadCreateResponse = null;

            var mapper = new LeadMapper();
            Lead lead = mapper.MapLeadCreateResponseToLead(leadCreateResponse);

            Assert.NotNull(lead);
        }

        [Theory]
        [InlineData(12341)]
        public void MapLeadCreateResponseToLeadTest_LeadIdIsMapped(decimal leadId)
        {
            var leadCreateResponse = new LeadCreateResponse();
            leadCreateResponse.Lead = new LeadDTO
            {
                LeadId = leadId
            };

            var mapper = new LeadMapper();
            Lead lead = mapper.MapLeadCreateResponseToLead(leadCreateResponse);

            Assert.Equal(leadId, lead.LeadId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapLeadCreateResponseToLeadTest_SuccessfulIsMapped(bool successful)
        {
            var leadCreateResponse = new LeadCreateResponse();
            leadCreateResponse.Lead = new LeadDTO();
            leadCreateResponse.Success = successful;

            var mapper = new LeadMapper();
            Lead lead = mapper.MapLeadCreateResponseToLead(leadCreateResponse);

            Assert.Equal(successful, lead.Successful);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MapLeadCreateResponseToLeadTest_IsTestLeadMapped(bool isTestLead)
        {
            var leadCreateResponse = new LeadCreateResponse();
            leadCreateResponse.Lead = new LeadDTO();
            leadCreateResponse.IsTestLead = isTestLead;

            var mapper = new LeadMapper();
            Lead lead = mapper.MapLeadCreateResponseToLead(leadCreateResponse);

            Assert.Equal(isTestLead, lead.IsTestLead);
        }

        [Theory]
        [InlineData(12341)]
        [InlineData(null)]
        public void MapLeadCreateResponseToLeadTest_ProgramProductIdIsMapped(int? programProductId)
        {
            var leadCreateResponse = new LeadCreateResponse();
            leadCreateResponse.Lead = new LeadDTO
            {
                ProgramProductId = programProductId
            };

            var mapper = new LeadMapper();
            Lead lead = mapper.MapLeadCreateResponseToLead(leadCreateResponse);

            Assert.Equal(programProductId, lead.ProgramProductId);
        }
    }
}