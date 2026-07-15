using EDDY.IS.Base;
using EDDY.IS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// SessionDTO - Class
namespace EDDY.IS.FormsEngine.DTO.Extended
{
    [Serializable]
    public class FormsEngineWorkflowStatus
    {
        public string CurrentPage { get; set; }
        public int CurrentSmartMatches { get; set; }
        public int CurrentUserSelections { get; set; }
        public int TemplateId { get; set; }
        public string LeadDataEncoded { get; set; }
        public string LeadAdditionalDataEncoded { get; set; }
        public int SMLeadsCreatedCount { get; set; }
        public int USLeadsCreatedCount { get; set; }
        public string UserFullName { get; set; }
        public int ProspectId { get; set; }
        public string MatchResponseGuid { get; set; }
        public bool SplitCampusTypeInResults { get; set; }
        public bool UserSmartMatched { get; set; }
        public bool UserShownManagedChoice { get; set; }
        public bool UserSubmittedManagedChoiceSelection { get; set; }
        public bool UserSkippedToConfirmation { get; set; }

        //Program Wizards
        public FormTemplateTypes FormTemplateType { get; set; }
        public int ProgramTemplateId { get; set; }
        public int ProgramProductId { get; set; }
        public int ProductId { get; set; }
        public string ProgramName { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public int? ApplicationId { get; set; }

        public bool UseInternationalTemplate { get; set; }
    }
}
