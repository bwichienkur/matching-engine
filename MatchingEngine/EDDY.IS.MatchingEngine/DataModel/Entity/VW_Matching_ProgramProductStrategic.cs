using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramProductStrategic
    {
        public int ProgramProductId { get; set; }
        public Nullable<bool> Strategic { get; set; }

        public VW_Matching_ProgramProductStrategic(IDataReader dr)
        {
            ProgramProductId = System.Convert.ToInt32(dr["ProgramProductId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("Strategic")))
                Strategic = System.Convert.ToBoolean(dr["Strategic"]);
        }
    }
}
