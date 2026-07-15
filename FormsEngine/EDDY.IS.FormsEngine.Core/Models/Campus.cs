using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Models
{
    public class Campus : Institution
    {
        public new int? InstitutionId { get; set; }
        public int CampusId { get; set; }
        public string CampusName { get; set; }
        public string CampusLogoUrl { get; set; }
    }
}
