using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Models
{
    [Serializable]
    public class ProgramDetail : BaseModel
    {
        public int ProductId { get; set; }
        public int ProgramProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InstitutionName { get; set; }
    }
}