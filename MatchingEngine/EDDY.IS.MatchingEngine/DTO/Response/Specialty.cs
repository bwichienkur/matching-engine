using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class Specialty : BaseMatchEntity
    {
        [DataMember]
        public int SpecialtyId { get; set; }

        [DataMember]
        public string SpecialtyName { get; set; }

        [DataMember]
        public int SubjectId { get; set; }

        public override bool Equals(object obj)
        {
            Specialty q = obj as Specialty;
            return q != null && q.SpecialtyId == this.SpecialtyId && q.SpecialtyName == this.SpecialtyName;
        }

        public override int GetHashCode()
        {
            return this.SpecialtyId.GetHashCode() ^ this.SpecialtyName.GetHashCode();
        }
    }
}