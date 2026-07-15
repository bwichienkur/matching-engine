using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramLevelContent
    {
        public int ProgramLevelId { get; set; }
        public string ProgramLevelName { get; set; }

        public VW_Matching_ProgramLevelContent(IDataReader dr)
        {
            ProgramLevelId = System.Convert.ToInt32(dr["ProgramLevelId"]);
            ProgramLevelName = System.Convert.ToString(dr["ProgramLevelName"]);
        }
    }
}
