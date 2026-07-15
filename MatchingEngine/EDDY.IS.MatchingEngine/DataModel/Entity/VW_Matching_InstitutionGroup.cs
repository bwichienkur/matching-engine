using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_InstitutionGroup
    {
        public int InstitutionId { get; set; }
        public int InstitutionGroupId { get; set; }

        public bool RemoveInSchoolSelection { get; set; }
        public bool AllowMultiSelectForInstitutionGroup { get; set; }

        public VW_Matching_InstitutionGroup(IDataReader dr)
        {
            InstitutionId = System.Convert.ToInt32(dr["InstitutionId"]);
            InstitutionGroupId = System.Convert.ToInt32(dr["InstitutionGroupId"]);
            RemoveInSchoolSelection = System.Convert.ToBoolean(dr["RemoveInSchoolSelection"]);
            AllowMultiSelectForInstitutionGroup = System.Convert.ToBoolean(dr["AllowMultiSelectForInstitutionGroup"]);
        }
    }
}
