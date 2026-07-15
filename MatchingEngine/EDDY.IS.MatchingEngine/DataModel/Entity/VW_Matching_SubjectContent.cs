using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_SubjectContent
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

        public VW_Matching_SubjectContent(IDataReader dr)
        {
            SubjectId = System.Convert.ToInt32(dr["SubjectId"]);
            SubjectName = System.Convert.ToString(dr["SubjectName"]);
        }
    }
}
