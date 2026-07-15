using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Models
{
    [Serializable]
    public class CampusBasicModel : BaseModel
    {
        public string Logo { get; set; }
        public string CampusName { get; set; }
        public string ProgramName {get; set;}
        public string CampusDescription { get; set; }
        public string ProgramDescription { get; set; }
        public bool   ShowLogo { get; set; }
    }
}