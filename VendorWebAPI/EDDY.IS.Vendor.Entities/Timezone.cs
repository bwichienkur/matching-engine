using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Entities
{
    public class TimezoneRequest
    {
        public Guid APIKey { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }

    }
    public class TimezoneResponse
    {
        public string Timezone { get; set; }
        public string Offset { get; set; }

    }
}