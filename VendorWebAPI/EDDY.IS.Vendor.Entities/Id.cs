using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class Id
    {
        int idvalue;
        public int Value
        {
            get
            {
                return idvalue;
            }

            set
            {
                idvalue = value;
            }
        }
    }
}
