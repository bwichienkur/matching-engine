using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_BusinessModelCriteriaGroup
    {
        public int BusinessModelId { get; set; }
        public int BusinessModelCriteriaGroupId { get; set; }
        public int BusinessModelCriteriaId { get; set; }
        public string Operand { get; set; }
        public Nullable<int> PriorityOrder { get; set; }
        public int BusinessModelCriteriaValueId { get; set; }
        public string CriteriaValue { get; set; }

        public VW_Matching_BusinessModelCriteriaGroup(IDataReader dr)
        {
            BusinessModelId = System.Convert.ToInt32(dr["BusinessModelId"]);
            BusinessModelCriteriaGroupId = System.Convert.ToInt32(dr["BusinessModelCriteriaGroupId"]);
            BusinessModelCriteriaId = System.Convert.ToInt32(dr["BusinessModelCriteriaId"]);
            Operand = System.Convert.ToString(dr["Operand"]);
            BusinessModelCriteriaValueId = System.Convert.ToInt32(dr["BusinessModelCriteriaValueId"]);
            CriteriaValue = System.Convert.ToString(dr["CriteriaValue"]);

            if (!dr.IsDBNull(dr.GetOrdinal("PriorityOrder")))
                PriorityOrder = System.Convert.ToInt32(dr["PriorityOrder"]);
        }
    }
}
