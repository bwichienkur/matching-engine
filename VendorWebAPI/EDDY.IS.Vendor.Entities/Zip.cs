using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EDDY.IS.Vendor.Entities
{    
    public class ZipResponse
    {
        private int campusId;        
        private List<string> zipCodes;

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
        public List<string> ZipCodes
        {
            get
            {
                return zipCodes;
            }

            set
            {
                zipCodes = value;
            }
        }        
    }
}
