using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Models
{
    public class Institution
    {
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionDescription { get; set; }
        public string InstitutionAccreditation { get; set; }
        public string InstitutionLogoUrl { get; set; }
        public List<Program> Programs { get; set; }
        public decimal ProgramRankScore { get; set; }
    }
}
