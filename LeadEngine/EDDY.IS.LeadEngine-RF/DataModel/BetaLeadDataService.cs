using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.LeadEngine.Mapper;

namespace EDDY.IS.LeadEngine.DataModel
{
    public class BetaLeadDataService
    {
        public BetaLead SaveBetaLead(LeadDTO Entity)
        {
            BetaLead bl = Converter.ConvertLeadTOToBetaDataModel(Entity);
            using (Nexus_Lead_Entities Context = new Nexus_Lead_Entities())
            {
                Context.BetaLeads.Add(bl);
                Context.SaveChanges();
            }
            return bl;
        }

    }
}
