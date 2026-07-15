using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ClientCampusProductMappingCache
    {
        public int ClientCampusProductMappingId { get; set; }
        public Nullable<int> AllowableRadius { get; set; }
        public string RadiusZipCode { get; set; }
        public Nullable<bool> IsZipCodeInclusionExclusionActive { get; set; }
        public Nullable<bool> IncludeAllCountries { get; set; }
        public Nullable<bool> IncludeAllStates { get; set; }
        public Nullable<bool> IncludeAllProvinces { get; set; }
        public Nullable<bool> IncludeAllZipCodes { get; set; }
        public Nullable<bool> IsZipCodeInclusion { get; set; }
        public Nullable<bool> IsZipCodeExclusion { get; set; }

        public VW_Matching_ClientCampusProductMappingCache(IDataReader dr)
        {
            ClientCampusProductMappingId = System.Convert.ToInt32(dr["ClientCampusProductMappingId"]);
            RadiusZipCode = System.Convert.ToString(dr["RadiusZipCode"]);

            if (!dr.IsDBNull(dr.GetOrdinal("AllowableRadius")))
                AllowableRadius = System.Convert.ToInt32(dr["AllowableRadius"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IsZipCodeInclusionExclusionActive")))
                IsZipCodeInclusionExclusionActive = System.Convert.ToBoolean(dr["IsZipCodeInclusionExclusionActive"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IncludeAllCountries")))
                IncludeAllCountries = System.Convert.ToBoolean(dr["IncludeAllCountries"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IncludeAllStates")))
                IncludeAllStates = System.Convert.ToBoolean(dr["IncludeAllStates"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IncludeAllProvinces")))
                IncludeAllProvinces = System.Convert.ToBoolean(dr["IncludeAllProvinces"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IncludeAllZipCodes")))
                IncludeAllZipCodes = System.Convert.ToBoolean(dr["IncludeAllZipCodes"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IsZipCodeInclusion")))
                IsZipCodeInclusion = System.Convert.ToBoolean(dr["IsZipCodeInclusion"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IsZipCodeExclusion")))
                IsZipCodeExclusion = System.Convert.ToBoolean(dr["IsZipCodeExclusion"]);
        }
    }
}
