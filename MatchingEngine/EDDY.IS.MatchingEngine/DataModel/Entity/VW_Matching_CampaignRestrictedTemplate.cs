using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CampaignRestrictedTemplate
    {
        public Guid TrackId { get; set; }
        public int TemplateId { get; set; }



        public VW_Matching_CampaignRestrictedTemplate(IDataReader dr)
        {
            TrackId = Guid.Parse(System.Convert.ToString(dr["TrackId"]));
            TemplateId = System.Convert.ToInt32(dr["TemplateId"]);
        }
    }
}
