using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    [KnownType(typeof(CategoryWithSubject))]
    public class CategoryResponse : BaseMatchResponse
    {
        [DataMember]
        public List<Category> CategoryList { get; set; }
    }
}