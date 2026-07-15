using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramShortDescription
    {
        public int ProgramId { get; set; }

        public string ShortDescription { get; set; }
        public int? CopyFromProgramId { get; set; }

        public VW_Matching_ProgramShortDescription(IDataReader dr)
        {
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            ShortDescription = System.Convert.ToString(dr["ShortDescription"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CopyFromProgramId")))
                CopyFromProgramId = (int)dr.GetValue(dr.GetOrdinal("CopyFromProgramId"));
        }
    }
}
