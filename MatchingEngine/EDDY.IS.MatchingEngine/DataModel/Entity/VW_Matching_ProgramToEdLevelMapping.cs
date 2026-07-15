using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramToEdLevelMapping
    {
        public int ProgramProductId { get; set; }
        public int EducationLevelId { get; set; }

        public VW_Matching_ProgramToEdLevelMapping(IDataReader dr)
        {
            ProgramProductId = System.Convert.ToInt32(dr["ProgramProductId"]);
            EducationLevelId = System.Convert.ToInt32(dr["EducationLevelId"]);
        }
    }
}
