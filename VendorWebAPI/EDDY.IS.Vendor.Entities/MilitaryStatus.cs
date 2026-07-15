using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class MilitaryStatus
    {
        private int militaryStatusId;
        private string militaryStatusName;
        private string militaryStatusDescription;
        private string legacyMilitaryStatusName;
        private bool isEnabled;

        public int MilitaryStatusId
        {
            get
            {
                return militaryStatusId;
            }
            set
            {
                militaryStatusId = value;
            }
        }

        public string MilitaryStatusName
        {
            get
            {
                return militaryStatusName;
            }
            set
            {
                militaryStatusName = value;
            }
        }

        public string MilitaryStatusDescription
        {
            get
            {
                return militaryStatusDescription;
            }
            set
            {
                militaryStatusDescription = value;
            }
        }

        public string LegacyMilitaryStatusName
        {
            get
            {
                return legacyMilitaryStatusName;
            }
            set
            {
                legacyMilitaryStatusName = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }




    }
}
