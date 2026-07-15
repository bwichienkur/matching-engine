using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public class AgentDisallowedLiveTransfer
    {
        public int ClientRelationShipId { get; set; }
        public bool DisallowLiveTransfer { get; set; }
        public bool DisallowForm { get; set; }

        public AgentDisallowedLiveTransfer()
        {

        }

        public AgentDisallowedLiveTransfer(int clientRelationshipId, bool disallowLiveTransfer, bool disallowForm)
        {
            ClientRelationShipId = clientRelationshipId;
            DisallowLiveTransfer = disallowLiveTransfer;
            DisallowForm = disallowForm;
        }
    }
}
