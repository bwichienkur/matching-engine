using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.LeadEngine;

namespace TestLead
{
    class Program
    {
        static void Main(string[] args)
        {

            LeadEngine l = new LeadEngine();
            Dictionary<string, string> leadData = new Dictionary<string, string>();

            leadData.Add("programid", "1");
            leadData.Add("csrid", "1");
            leadData.Add("timetostartinweeks", "");
            leadData.Add("trackingsessionguid", "C0C0C0-");
            
            var test = l.CreateLeadDTO(new EDDY.IS.LeadEngine.DTO.LeadCreateRequest() { IsBeta = true, TemplateId = 1, RawPostDataId = 0, ProgramProductId = 1, LeadData = leadData });
            
        }
    }
}
