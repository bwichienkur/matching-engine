using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_Product
    {
        public int ProductId { get; set; }
        public Nullable<int> ProductQualityRanking { get; set; }
        public bool IsFree { get; set; }
        public bool AllowClicks { get; set; }
        public bool AllowInquiryDisabled { get; set; }
        public bool AllowFree { get; set; }
        public bool AllowFraid { get; set; }
        public bool IsWTAllowed { get; set; }
        public bool IsSMPAllowed { get; set; }
        public bool IsEMSProduct { get; set; }
        public VW_Matching_Product(IDataReader dr)
        {
            ProductId = System.Convert.ToInt32(dr["ProductId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProductQualityRanking")))
                ProductQualityRanking = System.Convert.ToInt32(dr["ProductQualityRanking"]);

            IsFree = System.Convert.ToBoolean(dr["IsFree"]);
            AllowClicks = System.Convert.ToBoolean(dr["AllowClicks"]);
            AllowInquiryDisabled = System.Convert.ToBoolean(dr["AllowInquiryDisabled"]);
            AllowFraid = System.Convert.ToBoolean(dr["AllowFraid"]);
            AllowFree = System.Convert.ToBoolean(dr["AllowFree"]);
            IsWTAllowed = System.Convert.ToBoolean(dr["IsWTAllowed"]);
            IsSMPAllowed = System.Convert.ToBoolean(dr["IsSMPAllowed"]);
            IsEMSProduct = System.Convert.ToBoolean(dr["IsEMSProduct"]);
        }
    }
}
