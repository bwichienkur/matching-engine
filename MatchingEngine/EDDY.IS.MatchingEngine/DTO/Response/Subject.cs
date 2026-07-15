using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class Subject : BaseMatchEntity
    {
        [DataMember]
        public int SubjectId { get; set; }

        [DataMember]
        public string SubjectName { get; set; }

        public override bool Equals(object obj)
        {
            Subject q = obj as Subject;
            return q != null && q.SubjectId == this.SubjectId && q.SubjectName == this.SubjectName;
        }

        public override int GetHashCode()
        {
            return this.SubjectId.GetHashCode() ^ this.SubjectName.GetHashCode();
        }
    }
}