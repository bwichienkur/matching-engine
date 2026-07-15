using EDDY.IS.FormsEngine.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Models
{
    public class Program
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramDescription { get; set; }
        public int ProgramProductId { get; set; }
        public decimal ProgramRankScore { get; set; }
        public int? ProgramTemplateId { get; set; }
        public int? ProductId { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionLogoUrl { get; set; }
        public string InstitutionDescription { get; set; }
        public int? CampusId { get; set; }
        public string CampusName { get; set; }
        public string CampusLogoUrl { get; set; }
        public CampusType? CampusType { get; set; }
    }
}
