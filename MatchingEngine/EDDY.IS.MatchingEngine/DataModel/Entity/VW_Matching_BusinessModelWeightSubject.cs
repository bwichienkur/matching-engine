using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_BusinessModelWeightSubject
    {
        public int BusinessModelId { get; set; }
        public string BusinessModelName { get; set; }
        public bool IsDefault { get; set; }
        public bool UseDistanceCliffFormula { get; set; }
        public Nullable<int> DistanceCliffValue { get; set; }
        public Nullable<int> DistanceCapMultiplier { get; set; }
        public Nullable<int> BusinessModelRollupTypeId { get; set; }
        public Nullable<int> BusinessModelTestId { get; set; }
        public int BusinessModelWeightSubjectId { get; set; }
        public bool IsBaseline { get; set; }
        public int PercentToShow { get; set; }
        public int BusinessModelFactorWeightId { get; set; }
        public int BusinessModelFactorId { get; set; }
        public decimal OnlineWeight { get; set; }
        public Nullable<decimal> CampusWeight { get; set; }

        public VW_Matching_BusinessModelWeightSubject(IDataReader dr)
        {
            BusinessModelId = System.Convert.ToInt32(dr["BusinessModelId"]);
            BusinessModelName = System.Convert.ToString(dr["BusinessModelName"]);
            IsDefault = System.Convert.ToBoolean(dr["IsDefault"]);
            BusinessModelWeightSubjectId = System.Convert.ToInt32(dr["BusinessModelWeightSubjectId"]);
            IsBaseline = System.Convert.ToBoolean(dr["IsBaseline"]);
            PercentToShow = System.Convert.ToInt32(dr["PercentToShow"]);
            BusinessModelFactorWeightId = System.Convert.ToInt32(dr["BusinessModelFactorWeightId"]);
            BusinessModelFactorId = System.Convert.ToInt32(dr["BusinessModelFactorId"]);
            OnlineWeight = System.Convert.ToDecimal(dr["OnlineWeight"]);
            UseDistanceCliffFormula = System.Convert.ToBoolean(dr["UseDistanceCliffFormula"]);

            if (!dr.IsDBNull(dr.GetOrdinal("BusinessModelRollupTypeId")))
                BusinessModelRollupTypeId = System.Convert.ToInt32(dr["BusinessModelRollupTypeId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("BusinessModelTestId")))
                BusinessModelTestId = System.Convert.ToInt32(dr["BusinessModelTestId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CampusWeight")))
                CampusWeight = System.Convert.ToDecimal(dr["CampusWeight"]);

            if (!dr.IsDBNull(dr.GetOrdinal("DistanceCliffValue")))
                DistanceCliffValue = System.Convert.ToInt32(dr["DistanceCliffValue"]);

            if (!dr.IsDBNull(dr.GetOrdinal("DistanceCapMultiplier")))
                DistanceCapMultiplier = System.Convert.ToInt32(dr["DistanceCapMultiplier"]);
        }
    }
}
