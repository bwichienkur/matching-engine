using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EDDY.IS.Vendor.Entities
{
	[DataContract]
	public class CampusMicrosite
	{
        private int campusId;
        private string campusName;
        private List<ProgramMicrosite> programs;

        [DataMember(Order = 0)]
        public int CampusId
        {
            get
            {
                return campusId;
            }

            set
            {
                campusId = value;
            }
        }
        [DataMember(Order = 1)]
        public string CampusName
        {
            get
            {
                return campusName;
            }

            set
            {
                campusName = value;
            }
        }

        [DataMember(Order = 2)]
        public List<ProgramMicrosite> Programs
        {
            get
            {
                return programs;
            }

            set
            {
                programs = value;
            }
        }
    }
}
