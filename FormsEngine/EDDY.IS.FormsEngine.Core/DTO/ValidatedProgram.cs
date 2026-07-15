using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.DTO
{
    public class ValidatedProgram
    {
        public int? ProgramId { get; set; }
        public int? ProgramProductId { get; set; }
        public bool PassedValidation { get; set; }
        public bool IsInternalDuplicate { get; set; }
        public bool IsExternalDuplicate { get; set; }
        public int? PaidStatusType { get; set; }
        public int? AlternateProgramProductId { get; set; }
        public string Score { get; set; }
        public int? ScoreId { get; set; }
        public List<KeyValuePair<string, string>> RuleFailures { get; set; } = new List<KeyValuePair<string, string>>();
    }
}
