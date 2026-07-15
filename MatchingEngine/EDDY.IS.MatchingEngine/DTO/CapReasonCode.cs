using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    public enum CapReasonCode
    {
        Cap_Limit_Reached = 1,
        Normalization_Limit_Reached = 2,
        Day_Parting_Limit_Reached = 3,
        Week_Parting_Limit_Reached = 4,
        Parent_Cap_Limit_Reached = 5,
        Forcefully_Capping_Out = 6,
        LeadType_Lead_Capped = 7,
        LeadType_Click_Capped = 8
    }
}
