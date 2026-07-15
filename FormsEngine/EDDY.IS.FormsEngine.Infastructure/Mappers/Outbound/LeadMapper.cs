using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.LeadEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class LeadMapper
    {
        public List<Lead> MapLeadCreateResponsesToLeads(IEnumerable<LeadCreateResponse> leadCreateResponses)
        {
            var leads = new List<Lead>();

            if (leadCreateResponses != null)
            {
                foreach (var leadCreateResponse in leadCreateResponses)
                {
                    var lead = MapLeadCreateResponseToLead(leadCreateResponse);
                    leads.Add(lead);
                }
            }

            return leads;
        }

        public Lead MapLeadCreateResponseToLead(LeadCreateResponse leadCreateResponse)
        {
            var lead = new Lead();

            if (leadCreateResponse?.Lead != null)
            {
                lead.LeadId = leadCreateResponse.Lead.LeadId;
                lead.ProgramProductId = leadCreateResponse.Lead.ProgramProductId;
                lead.Successful = leadCreateResponse.Success;
                lead.IsTestLead = leadCreateResponse.IsTestLead;
            }

            return lead;
        }
    }
}
