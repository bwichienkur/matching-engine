using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramValidationRuleCache
    {
        public int pvbrid { get; set; }
        public string BaseRule { get; set; }
        public int pvrid { get; set; }
        public string RuleName { get; set; }
        public Nullable<decimal> EntityValue { get; set; }
        public string Code { get; set; }
        public Nullable<int> kvcodedataid { get; set; }
        public int? programid { get; set; }
        public int? productid { get; set; }
        public bool IsStatic { get; set; }
        public bool IsLowerBound { get; set; }
        public bool IsUpperBound { get; set; }
        public bool UtilizeAcademicYear { get; set; }

        public int programproductid { get; set; }

        public VW_Matching_ProgramValidationRuleCache(IDataReader dr)
        {
            pvbrid = System.Convert.ToInt32(dr["pvbrid"]);
            BaseRule = System.Convert.ToString(dr["BaseRule"]);
            pvrid = System.Convert.ToInt32(dr["pvrid"]);
            RuleName = System.Convert.ToString(dr["RuleName"]);
            Code = System.Convert.ToString(dr["Code"]);

            if (!dr.IsDBNull(dr.GetOrdinal("programid")))
                programid = System.Convert.ToInt32(dr["programid"]);

            if (!dr.IsDBNull(dr.GetOrdinal("productid")))
                productid = System.Convert.ToInt32(dr["productid"]);

            if (!dr.IsDBNull(dr.GetOrdinal("EntityValue")))
                EntityValue = System.Convert.ToDecimal(dr["EntityValue"]);

            if (!dr.IsDBNull(dr.GetOrdinal("kvcodedataid")))
                kvcodedataid = System.Convert.ToInt32(dr["kvcodedataid"]);

            IsStatic = Convert.ToBoolean(dr["IsStatic"]);
            IsLowerBound = Convert.ToBoolean(dr["IsLowerBound"]);
            IsUpperBound = Convert.ToBoolean(dr["IsUpperBound"]);
            UtilizeAcademicYear = Convert.ToBoolean(dr["UtilizeAcademicYear"]);
            programproductid = System.Convert.ToInt32(dr["programproductid"]);
        }
    }
}
