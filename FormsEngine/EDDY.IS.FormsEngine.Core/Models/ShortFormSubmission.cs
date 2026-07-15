using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Models
{
    public class ShortFormSubmission
    {
        public Prospect Prospect { get; set; }
        public Dictionary<int, SchoolPickerUserSelection> SchoolPickerUserSelections { get; set; }
        public string TrackId { get; set; }
        public bool IsBeta { get; set; }
    }
}
