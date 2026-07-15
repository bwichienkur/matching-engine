using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ProgramLevel : BaseMatchEntity
    {
        [DataMember]
        public int ProgramLevelId { get; set; }

        [DataMember]
        public string ProgramLevelName { get; set; }

        public override bool Equals(object obj)
        {
            ProgramLevel q = obj as ProgramLevel;
            return q != null && q.ProgramLevelId == this.ProgramLevelId && q.ProgramLevelName == this.ProgramLevelName;
        }

        public override int GetHashCode()
        {
            return this.ProgramLevelId.GetHashCode() ^ this.ProgramLevelName.GetHashCode();
        }
    }
}