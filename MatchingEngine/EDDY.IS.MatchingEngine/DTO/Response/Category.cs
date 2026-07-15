using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class Category : BaseMatchEntity
    {
        [DataMember]
        public int CategoryId { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

        public override bool Equals(object obj)
        {
            Category q = obj as Category;
            return q != null && q.CategoryId == this.CategoryId && q.CategoryName == this.CategoryName;
        }

        public override int GetHashCode()
        {
            return this.CategoryId.GetHashCode() ^ this.CategoryName.GetHashCode();
        }
    }
}