using System.Collections.Generic;
using System.Runtime.Serialization;
namespace EDDY.IS.Vendor.Entities
{
    [DataContract]
    public class ProgramInstitution : InstitutionBase
    {
      
        private List<CampusBase> campuses;

        [DataMember(Order = 5)]
        public List<CampusBase> Campuses
        {
            get
            {
                return campuses;
            }

            set
            {
                campuses = value;
            }
        }
    }
}
