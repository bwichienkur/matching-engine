using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ClientCampusRelationshipInfo
    {
        public int ClientCampusRelationshipId { get; set; }
        public string CallCenterPhone { get; set; }

        public VW_Matching_ClientCampusRelationshipInfo(IDataReader dr)
        {
            ClientCampusRelationshipId = System.Convert.ToInt32(dr["ClientCampusRelationshipId"]);
            CallCenterPhone = System.Convert.ToString(dr["CallCenterPhone"]);
        }
    }
}
