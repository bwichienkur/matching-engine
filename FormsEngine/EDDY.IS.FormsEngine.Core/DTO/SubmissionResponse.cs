using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.DTO
{
    public class SubmissionResponse
    {
        public Guid? MatchGuid { get; set; }
        public bool Success { get; set; }
        public bool MoveToStart { get; set; }
        public bool MoveToNoMatch { get; set; }
        public bool MoveToThankYou { get; set; }
        public int SMLeadsCreatedCount { get; set; }
        public int USLeadsCreatedCount { get; set; }
        public int SchoolPickerLeadsCreatedCount { get; set; }
        public string UserFullName { get; set; }
        public bool IsTestLead { get; set; }
        public int? ProspectId { get; set; }

    }
}
