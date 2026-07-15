using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.DTO
{
    public class FormValidationResult
    {
        public bool Valid { get; set; }
        public bool IsTestLead { get; set; }
        public List<KeyValuePair<string, string>> ValidationMessages { get; set; }
    }
}
