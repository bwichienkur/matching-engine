using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramStartDate
    {
        public int ProgramId { get; set; }
        public DateTime StartDate { get; set; }

        public VW_Matching_ProgramStartDate(IDataReader dr)
        {
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            StartDate = System.Convert.ToDateTime(dr["StartDate"]);
        }
    }
}
