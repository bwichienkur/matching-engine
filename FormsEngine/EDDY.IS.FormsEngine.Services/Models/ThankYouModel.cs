using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Models
{
    [Serializable]
    public class ThankYouModel : BaseModel
    {
        public string Theme { get; set; }
        public string UserFullName { get; set; }
        public List<CampusBasicModel> OnlineCampuses { get; set; }
        public List<CampusBasicModel> GroundCampuses { get; set; }
        public List<int> UserSelectionProgramIdList { get; set; }
        public List<int> SmartMatchProgramIdList { get; set; }
        public bool SubmissionsFailed { get; set; }
        public string LeadList { get; set; }


        public ThankYouModel(string theme)
        {
            Theme = string.IsNullOrWhiteSpace(theme) ? "default" : theme;
        }
    }
}