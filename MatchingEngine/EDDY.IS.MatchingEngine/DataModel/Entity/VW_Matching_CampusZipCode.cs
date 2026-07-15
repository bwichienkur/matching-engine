using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CampusZipCode
    {
        public int ClientCampusProductMappingId { get; set; }
        public string ZipCode { get; set; }
        public Nullable<bool> IsZipCodeInclusion { get; set; }
        public Nullable<bool> IsZipCodeExclusion { get; set; }

        public VW_Matching_CampusZipCode(IDataReader dr)
        {
            ClientCampusProductMappingId = System.Convert.ToInt32(dr["ClientCampusProductMappingId"]);
            ZipCode = System.Convert.ToString(dr["ZipCode"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IsZipCodeInclusion")))
                IsZipCodeInclusion = System.Convert.ToBoolean(dr["IsZipCodeInclusion"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IsZipCodeExclusion")))
                IsZipCodeExclusion = System.Convert.ToBoolean(dr["IsZipCodeExclusion"]);
        }
    }
}
