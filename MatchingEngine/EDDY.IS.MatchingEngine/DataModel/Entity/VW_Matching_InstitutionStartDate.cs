using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_InstitutionStartDate
    {
        public int InstitutionId { get; set; }
        public DateTime StartDate { get; set; }

        public VW_Matching_InstitutionStartDate(IDataReader dr)
        {
            InstitutionId = System.Convert.ToInt32(dr["InstitutionId"]);
            StartDate = System.Convert.ToDateTime(dr["StartDate"]);
        }
    }
}
