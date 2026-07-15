using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ClientRelationAgentDisallowedLiveTransfer
    {
        public int UserId { get; set; }
        public int ClientRelationshipId { get; set; }
        public bool DisallowLiveTransfer { get; set; }
        public bool DisallowForm { get; set; }

        public VW_Matching_ClientRelationAgentDisallowedLiveTransfer(IDataReader dr)
        {
            UserId = Convert.ToInt32(dr["UserId"]);
            ClientRelationshipId = Convert.ToInt32(dr["ClientRelationshipId"]);
            DisallowLiveTransfer = Convert.ToBoolean(dr["DisallowLiveTransfer"]);
            DisallowForm = Convert.ToBoolean(dr["DisallowForm"]);
        }
    }
}