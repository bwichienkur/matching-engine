using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CapHierarchy
    {
        public int ClientRelationProductMappingId { get; set; }
        public int CapDurationId { get; set; }
        public int ClientRelationshipId { get; set; }
        public int ProductId { get; set; }
        public bool IsOn { get; set; }
        public int ParentCapDistributionId { get; set; }
        public int CapDistributionId { get; set; }
        public int Level { get; set; }
        public int EntityMetaId { get; set; }
        public int EntityId { get; set; }
        public Nullable<short> CapLimitTypeId { get; set; }
        public Nullable<int> CapReasonCodeId { get; set; }
        public decimal TotalCapAmount { get; set; }
        public decimal CapRoom { get; set; }
        public decimal CapTransactionCount { get; set; }
        public bool IsCurrentlyFree { get; set; }
        public short CapTypeId { get; set; }

        public int? SFProductCodeId { get; set; }
        public bool TreatAsMatch1 { get; set; }
        public VW_Matching_CapHierarchy(IDataReader dr)
        {
            ClientRelationProductMappingId = System.Convert.ToInt32(dr["ClientRelationProductMappingId"]);
            CapDurationId = System.Convert.ToInt32(dr["CapDurationId"]);
            ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);
            ProductId = System.Convert.ToInt32(dr["ProductId"]);
            ParentCapDistributionId = System.Convert.ToInt32(dr["ParentCapDistributionId"]);
            CapDistributionId = System.Convert.ToInt32(dr["CapDistributionId"]);
            Level = System.Convert.ToInt32(dr["Level"]);
            EntityMetaId = System.Convert.ToInt32(dr["EntityMetaId"]);
            EntityId = System.Convert.ToInt32(dr["EntityId"]);
            TotalCapAmount = System.Convert.ToDecimal(dr["TotalCapAmount"]);
            CapRoom = System.Convert.ToDecimal(dr["CapRoom"]);
            CapTransactionCount = System.Convert.ToDecimal(dr["CapTransactionCount"]);
            IsCurrentlyFree = System.Convert.ToBoolean(dr["IsCurrentlyFree"]);
            CapTypeId = System.Convert.ToInt16(dr["CapType"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IsOn")))
                IsOn = System.Convert.ToBoolean(dr["IsOn"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CapLimitTypeId")))
                CapLimitTypeId = System.Convert.ToInt16(dr["CapLimitTypeId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CapReasonCodeId")))
                CapReasonCodeId = System.Convert.ToInt32(dr["CapReasonCodeId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("SFProductCodeId")))
                SFProductCodeId = System.Convert.ToInt32(dr["SFProductCodeId"]);

            TreatAsMatch1 = System.Convert.ToBoolean(dr["TreatAsMatch1"]);
        }
    }
}
