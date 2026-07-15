using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_CrossSellProgramLevelMapping
    {
        public int EducationLevelId { get; set; }
        public int InitialProgramLevelId { get; set; }
        public int SearchProgramLevelId { get; set; }
        public int Priority { get; set; }
        public string InitialLeadValidMessage { get; set; }
        public string InitialLeadInvalidMessage { get; set; }

        public VW_Matching_CrossSellProgramLevelMapping(IDataReader dr)
        {
            EducationLevelId = System.Convert.ToInt32(dr["EducationLevelId"]);
            InitialProgramLevelId = System.Convert.ToInt32(dr["InitialProgramLevelId"]);
            SearchProgramLevelId = System.Convert.ToInt32(dr["SearchProgramLevelId"]);
            Priority = System.Convert.ToInt32(dr["Priority"]);
            InitialLeadValidMessage = System.Convert.ToString(dr["InitialLeadValidMessage"]);
            InitialLeadInvalidMessage = System.Convert.ToString(dr["InitialLeadInvalidMessage"]);
        }
    }
}
