using EDDY.IS.FormsEngine.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Models
{
    public class ThankYouPageLineItem
    {
        public string LogoUrl { get; set; }
        public bool ShouldShowLogo { get; set; }
        public CampusType CampusType { get; set; }
        public string CampusName { get; set; }
        public string CampusDescription { get; set; }
        public string ProgramName { get; set; }
        public string ProgramDescription { get; set; }
    }
}
