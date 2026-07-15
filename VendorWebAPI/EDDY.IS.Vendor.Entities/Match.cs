using System.Collections.Generic;

namespace EDDY.IS.Vendor.Entities
{
    public class Match
    {
        private InstitutionBase institution;
        private List<ProgramMatch> matchedPrograms;

        public InstitutionBase MatchedInstitution
        {
            get
            {
                return institution;
            }

            set
            {
                institution = value;
            }
        }

    

        public List<ProgramMatch> MatchedPrograms
        {
            get
            {
                return matchedPrograms;
            }

            set
            {
                matchedPrograms = value;
            }
        }
    }
}
