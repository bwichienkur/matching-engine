using EDDY.IS.FormsEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.DTO
{
    public class FailedMatchReplacements
    {
        public IEnumerable<Match> FailedMatches { get; set; } = Enumerable.Empty<Match>();
        public IEnumerable<Match> ReplacementMatches { get; set; } = Enumerable.Empty<Match>();
        public string Message { get; set; } = string.Empty;
    }
}
