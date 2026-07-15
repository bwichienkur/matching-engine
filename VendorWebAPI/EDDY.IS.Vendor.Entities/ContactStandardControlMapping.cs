using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class ContactStandardControlMapping
    {
        private string contactPropertyName;
        private string standardControlName;

        public string ContactPropertyName
        {
            get
            {
                return contactPropertyName;
            }

            set
            {
                contactPropertyName = value;
            }
        }

        public string StandardControlName
        {
            get
            {
                return standardControlName;
            }

            set
            {
                standardControlName = value;
            }
        }
    }
}
