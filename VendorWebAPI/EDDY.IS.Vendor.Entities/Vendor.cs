using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
   public class Vendor
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public bool IsSourceRequired { get; set; }
    }
}
