using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public class InstitutionCrCampusCollection
    {
        public HashSet<int> CampusIds { get; set; }
        public int ClientRelationshipId { get; set; }
        public Boolean BachelorsAvailable { get; set; }

        public InstitutionCrCampusCollection()
        {
            CampusIds = new HashSet<int>();
            BachelorsAvailable = false;
        }

    }
}
