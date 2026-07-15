using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class CategoryWithSubject : Category
    {
        [DataMember]
        public List<SubjectWithProduct> SubjectList { get; set; }
    }
}
