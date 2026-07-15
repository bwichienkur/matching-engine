using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_LeadScoreReservationSupplyUnitDefinition
    {
        public int LeadScoreReservationSupplyUnitId { get; set;}
        public string SupplyUnitName { get; set; }
        public int ProgramLevelId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubjectId { get; set; }
        public int? SpecialtyId { get; set; }
        public int? ExcludedClientRelationshipId { get; set; }

        public VW_Matching_LeadScoreReservationSupplyUnitDefinition(IDataReader dr)
        {
            LeadScoreReservationSupplyUnitId = System.Convert.ToInt32(dr["LeadScoreReservationSupplyUnitId"]);
            SupplyUnitName = System.Convert.ToString(dr["SupplyUnitName"]);
            ProgramLevelId = System.Convert.ToInt32(dr["ProgramLevelId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CategoryId")))
                CategoryId = System.Convert.ToInt32(dr["CategoryId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("SubjectId")))
                SubjectId = System.Convert.ToInt32(dr["SubjectId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("SpecialtyId")))
                SpecialtyId = System.Convert.ToInt32(dr["SpecialtyId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ExcludedClientRelationshipId")))
                ExcludedClientRelationshipId = System.Convert.ToInt32(dr["ExcludedClientRelationshipId"]);

        }
    }
}
