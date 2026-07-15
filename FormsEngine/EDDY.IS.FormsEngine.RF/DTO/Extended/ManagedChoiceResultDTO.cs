using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    [Serializable]
    [DataContract]
    public class ManagedChoiceResultDTO
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string RenderedManagedChoice { get; set; }

        [DataMember]
        public int SmartMatchProgramCount { get; set; }

        [DataMember]
        public int UserSelectSchoolCount { get; set; }

        [DataMember]
        public int MaxManagedChoiceUserSelections { get; set; }

        [DataMember]
        public bool MoveToStart { get; set; }
        
        [DataMember]
        public bool MoveToThankYou { get; set; }

        [DataMember]
        public bool MoveToNoMatch { get; set; }

        [DataMember]
        public int SMLeadsCreatedCount { get; set; }

        [DataMember]
        public int USLeadsCreatedCount { get; set; }

        [DataMember]
        public string UserFullName { get; set; }

        [DataMember]
        public bool IsTestLead { get; set; }

        [DataMember]
        public int? ProspectId { get; set; }

        [DataMember]
        public Guid? MatchGuid { get; set; }

        [DataMember]
        public bool IsProgramWizardResult { get; set; }

        [DataMember]
        public bool ProgramWizardInitialProgramValid { get; set; }

        [DataMember]
        public int ProgramWizardInitialProgramPaidStatusTypeId { get; set; }

        [DataMember]
        public int ProgramWizardInitialValidLeadId { get; set; }
    }
}