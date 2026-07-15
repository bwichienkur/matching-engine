using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.MatchingEngine;

namespace EDDY.IS.FormsEngine.Services.Models
{
    public class ManagedChoiceModel : BaseModel
    {
        //public WizardMatchResponse ManagedChoiceProgramResponse { get; set; }
        public List<ProgramWithInstitutionCampus> SmartMatchList { get; set; }
        public List<CampusWithInstitution> SchoolSelectionList { get; set; }
        public List<int> AlreadyAskedTemplateIds { get; set; }
        public int? MaxUserSelections { get; set; }
        public Guid MatchResponseGuid { get; set; }
        public bool HasSmartMatchPrograms { get; set; }
        public bool ProgramWizardInitialLeadValid { get; set; }
        public bool HasUserSelectPrograms { get; set; }
        public bool IsForTemplatePreview { get; set; }
        public bool IsPreCheckEnabled { get; set; }
        public string UserFullName { get; set; }
        public int MaxManagedChoiceUserSelections { get; set; }
        public string MaxManagedChoiceUserSelectionsAlpha { get; set; }
        public string Theme { get; set; }
        public int DisclaimerCounter { get; set; }
        public ExpressConsentCheckDTO ExpressConsentCheck { get; set; }
        public EddyLogosDTO EddyLogos { get; set; }
        public int UserDesiredProgramLevelId { get; set; }
        public MatchingEngine.CampusPreference? CampusSoftPreference { get; set; }
        public bool CampusSoftPreferenceShown { get; set; }
        public string ProgramWizardMessage { get; set; }

        public bool InlineDropDown { get; set; }

        public int? ApplicationId  { get; set; }
        public List<ProgramRankInstitution> ProgramRankInstitutions { get; set; }

        public ManagedChoiceModel()
        {
            this.HasSmartMatchPrograms = false;
            this.HasUserSelectPrograms = false;
            this.IsForTemplatePreview = false;
            this.MaxManagedChoiceUserSelections = 1;
            this.MaxManagedChoiceUserSelectionsAlpha = "One";
            this.DisclaimerCounter = 1;
            this.ExpressConsentCheck = new ExpressConsentCheckDTO();
            this.EddyLogos = new EddyLogosDTO();
            this.ProgramRankInstitutions = new List<ProgramRankInstitution>();
        }

    }
}