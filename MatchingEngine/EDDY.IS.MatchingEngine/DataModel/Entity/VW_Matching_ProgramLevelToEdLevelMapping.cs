using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramLevelToEdLevelMapping
    {
        public int ProgramLevelId { get; set; }
        public int EducationLevelId { get; set; }

        public VW_Matching_ProgramLevelToEdLevelMapping(IDataReader dr)
        {
            ProgramLevelId = System.Convert.ToInt32(dr["ProgramLevelId"]);
            EducationLevelId = System.Convert.ToInt32(dr["EducationLevelId"]);
        }
    }
}
