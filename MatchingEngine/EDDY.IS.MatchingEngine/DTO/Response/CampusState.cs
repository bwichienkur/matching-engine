using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class CampusState
    {
        [DataMember]
        public int StateId { get; set; }

        [DataMember]
        public string StateName { get; set; }

        [DataMember]
        public string StateFullName { get; set; }

        public override bool Equals(object obj)
        {
            CampusState q = obj as CampusState;
            return q != null && q.StateId == this.StateId && q.StateName == this.StateName;
        }

        public override int GetHashCode()
        {
            return this.StateId.GetHashCode() ^ this.StateName.GetHashCode();
        }
    }
}
