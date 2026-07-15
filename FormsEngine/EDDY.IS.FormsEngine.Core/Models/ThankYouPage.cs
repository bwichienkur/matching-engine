using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Models
{
    public class ThankYouPage
    {
        public string Theme { get; set; } = "default";
        public string UserFullName { get; set; }
        public List<ThankYouPageLineItem> LineItems { get; set; } = new List<ThankYouPageLineItem>();
        public List<int> UserSelectionProgramIdList { get; set; } = new List<int>();
        public List<int> SmartMatchProgramIdList { get; set; } = new List<int>();
        public List<int> SchoolPickerMatchProgramIdList { get; set; } = new List<int>();
        public bool SubmissionsFailed { get; set; }
        public string LeadList { get; set; }

        public ThankYouPage()
        {
        }

        public ThankYouPage(string theme)
        {
            Theme = string.IsNullOrWhiteSpace(theme) ? "default" : theme;
        }
    }
}
