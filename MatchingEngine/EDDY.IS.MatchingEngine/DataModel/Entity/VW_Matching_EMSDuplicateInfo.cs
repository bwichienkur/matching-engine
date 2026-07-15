using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_EMSDuplicateInfo
    {
        public int ProgramId { get; set; }

        public int ClientRelationshipId { get; set; }

        public int ProgramProductId { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DaysElapsed { get; set; }
        public int CrDuplicateLookback { get; set; }

        public VW_Matching_EMSDuplicateInfo(IDataReader dr)
        {
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            ProgramProductId = System.Convert.ToInt32(dr["ProgramProductId"]);
            ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);

            Email = System.Convert.ToString(dr["EmailAddress"]);
            Phone1 = System.Convert.ToString(dr["Phone1"]);
            Phone2 = System.Convert.ToString(dr["Phone2"]);
            FirstName = System.Convert.ToString(dr["FirstName"]);
            LastName = System.Convert.ToString(dr["LastName"]);

            DaysElapsed = System.Convert.ToInt32(dr["DaysElapsed"]);
            CrDuplicateLookback = System.Convert.ToInt32(dr["LeadLookupDuration"]);
        }
    }
}
