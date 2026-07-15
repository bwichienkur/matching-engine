using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Models
{
    public class Match
    {
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionLogoUrl { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int ProgramProductId { get; set; }
        public int? ProgramTemplateId { get; set; }
        public bool PassedValidation { get; set; }
        public bool IsInternalDuplicate { get; set; }
        public bool IsExternalDuplicate { get; set; }
        public int? PaidStatusType { get; set; }
        public int? AlternativeProgramProductId { get; set; }
        public string Score { get; set; }
        public int? ScoreId { get; set; }
        public List<KeyValuePair<string, string>> RuleFailures { get; set; } = new List<KeyValuePair<string, string>>();

        public Match() { }

        public Match(Campus campus, Program program)
        {
            InstitutionId = campus.InstitutionId ?? 0;
            InstitutionName = campus.InstitutionName;
            InstitutionLogoUrl = campus.InstitutionLogoUrl;
            ProgramId = program.ProgramId;
            ProgramName = program.ProgramName;
            ProgramProductId = program.ProgramProductId;
            ProgramTemplateId = program.ProgramTemplateId;
        }
    }
}
