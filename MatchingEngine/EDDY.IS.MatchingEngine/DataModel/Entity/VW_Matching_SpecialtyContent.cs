using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_SpecialtyContent
    {
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }

        public VW_Matching_SpecialtyContent(IDataReader dr)
        {
            SpecialtyId = System.Convert.ToInt32(dr["SpecialtyId"]);
            SpecialtyName = System.Convert.ToString(dr["SpecialtyName"]);
        }
    }
}
