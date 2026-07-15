using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ThirdPartyMatches
    {
        public int ProgramProductId { get; set; }
        public int InstitutionId { get; set; }
        public int CampusId { get; set; }
        public int CampusTypeId { get; set; }
        public int ProgramId { get; set; }
        public int ProgramTypeId { get; set; }
        public int ClientRelationshipId { get; set; }
        public int ProductId { get; set; }
        public int ClientRelationProductMappingId { get; set; }
        public int ClientCampusRelationshipId { get; set; }
        public int ClientCampusProductMappingId { get; set; }
        public int PsiId { get; set; }
        public Nullable<int> InstitutionLeadTypeId { get; set; }
        public string ProgramCode { get; set; }
        public VW_Matching_ThirdPartyMatches(IDataReader dr)
        {
            ProgramProductId = System.Convert.ToInt32(dr["ProgramProductId"]);
            InstitutionId = System.Convert.ToInt32(dr["InstitutionId"]);
            CampusId = System.Convert.ToInt32(dr["CampusId"]);
            CampusTypeId = System.Convert.ToInt32(dr["CampusCampusTypeId"]);
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            ProgramTypeId = System.Convert.ToInt32(dr["ProgramTypeId"]);
            ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);
            ProductId = System.Convert.ToInt32(dr["ProductId"]);
            ClientRelationProductMappingId = System.Convert.ToInt32(dr["ClientRelationProductMappingId"]);
            ClientCampusRelationshipId = System.Convert.ToInt32(dr["ClientCampusRelationshipId"]);
            ClientCampusProductMappingId = System.Convert.ToInt32(dr["ClientCampusProductMappingId"]);
            PsiId = System.Convert.ToInt32(dr["PsiId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("InstitutionLeadTypeId")))
                InstitutionLeadTypeId = System.Convert.ToInt32(dr["InstitutionLeadTypeId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProgramCode")))
                ProgramCode = System.Convert.ToString(dr["ProgramCode"]);
        }
    }
}
