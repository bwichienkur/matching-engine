using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Models
{
    public class Lead
    {
        public decimal LeadId { get; set; }
        public bool Successful { get; set; }
        public bool IsTestLead { get; set; }
        public int? ProgramProductId { get; set; }
    }
}
