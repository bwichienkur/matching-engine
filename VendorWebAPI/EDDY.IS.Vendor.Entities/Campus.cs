using System.Collections.Generic;

namespace EDDY.IS.Vendor.Entities
{
    public class Campus : CampusBase
    {
        private InstitutionBase institution;

        public InstitutionBase Institution
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

    }
}
