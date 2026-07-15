using System.Collections.Generic;
using System.Runtime.Serialization;
namespace EDDY.IS.Vendor.Entities
{
    [DataContract]
    public class InstitutionBase
    {
        private int institutionId;
        private string institutionName;
        private string institutionDescription;
        private string logoURL;
        private bool isLiveTransfer;
        private List<SchoolAgent> agents;
        private bool isExclusive;

        [DataMember(Order = 0)]
        public int InstitutionId
        {
            get
            {
                return institutionId;
            }

            set
            {
                institutionId = value;
            }
        }
        [DataMember(Order = 1)]
        public string InstitutionName
        {
            get
            {
                return institutionName;
            }

            set
            {
                institutionName = value;
            }
        }
        [DataMember(Order = 2)]
        public string InstitutionDescription
        {
            get
            {
                return institutionDescription;
            }

            set
            {
                institutionDescription = value;
            }
        }
        [DataMember(Order = 3)]
        public string LogoURL
        {
            get
            {
                return logoURL;
            }

            set
            {
                logoURL = value;
            }
        }

        [DataMember(Order = 4)]
        public bool IsLiveTransfer
        {
            get { return isLiveTransfer; }
            set { isLiveTransfer = value; }
        }

        [DataMember(Order = 5)]
        public List<SchoolAgent> Agents
        {
            get { return agents; }
            set { agents = value; }
        }

        [DataMember(Order = 6)]
        public bool IsExclusive
        {
            get { return isExclusive; }
            set { isExclusive = value; }
        }
    }

    [DataContract]
    public class SchoolAgent
    {
        private int agentId;
        private string agentName;

        [DataMember(Order = 1)]
        public int AgentId
        {
            get { return agentId; }
            set { agentId = value; }
        }

        [DataMember(Order = 2)]
        public string AgentName
        {
            get { return agentName; }
            set { agentName = value; }
        }
    }
}
