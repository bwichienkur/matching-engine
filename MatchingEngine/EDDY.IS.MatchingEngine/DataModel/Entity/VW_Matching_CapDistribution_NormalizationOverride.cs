using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CapDistribution_NormalizationOverride
    {
        public int CapDistributionId { get; set; }
        public int MarketingUnitId { get; set; }

        public VW_Matching_CapDistribution_NormalizationOverride(IDataReader dr)
        {
            CapDistributionId = System.Convert.ToInt32(dr["CapDistributionId"]);
            MarketingUnitId = System.Convert.ToInt32(dr["MarketingUnitId"]);
        }
    }
}
