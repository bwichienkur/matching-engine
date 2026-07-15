using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_Application
    {
        public int ApplicationId { get; set; }
        public bool AllowClicks { get; set; }
        public bool NonPaidTreatedAsCapped { get; set; }

        public VW_Matching_Application(IDataReader dr)
        {
            ApplicationId = System.Convert.ToInt32(dr["ApplicationId"]);

            AllowClicks = System.Convert.ToBoolean(dr["AllowClicks"]);
            NonPaidTreatedAsCapped = System.Convert.ToBoolean(dr["NonPaidTreatedAsCapped"]);
        }
    }
}
