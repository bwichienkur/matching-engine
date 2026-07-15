using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class InstitutionDetail
    {
        [DataMember]
        public int InstitutionId { get; set; }
        [DataMember]
        public string InstitutionName { get; set; }
        [DataMember]
        public string InstitutionDescription { get; set; }
        [DataMember]
        public string InstitutionDescriptionInternational { get; set; }
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string Address2 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string Zip { get; set; }
        [DataMember]
        public string CarnegieClassification { get; set; }
        [DataMember]
        public string CalendarTypeOnline { get; set; }
        [DataMember]
        public string LearningFormatOnline { get; set; }
        [DataMember]
        public string GeographicComments { get; set; }
        //[DataMember]
        //public string InstitutionType { get; set; }
        [DataMember]
        public string InstitutionalAccreditationType { get; set; }
        [DataMember]
        public string LoansOffered { get; set; }
        [DataMember]
        public string OnCampusRequirements { get; set; }
        [DataMember]
        public string ScholarshipOffered { get; set; }
        [DataMember]
        public string RegistrationDetails { get; set; }
        [DataMember]
        public string ComputerRequirement { get; set; }
        [DataMember]
        public string TotalEnrollment { get; set; }
        [DataMember]
        public DisclaimerType? InstitutionDisclaimerType { get; set; }
        [DataMember]
        public string InstitutionDisclaimer { get; set; }
        [DataMember]
        public bool FailedValidation { get; set; }
        [DataMember]
        public string Locale { get; set; }
        [DataMember]
        public string AcademicYearBeginMonth { get; set; }
        [DataMember]
        public string AcademicYearBeginDay { get; set; }
        [DataMember]
        public string AccreditationOrganization { get; set; }
        [DataMember]
        public List<string> BillingRuleList { get; set; }
        [DataMember]
        public List<Image> ImageList { get; set; }
        [DataMember]
        public string AdHeader { get; set; }
        [DataMember]
        public string AdDescription { get; set; }
        [DataMember]
        public bool IsDisabled { get; set; }
        [DataMember]
        public PaidStatusType PaidStatusTypeId { get; set; }
        [DataMember]
        public bool OnlyClicksPrograms { get; set; }
        [DataMember]
        public string InstitutionLogoURL { get; set; }
        [DataMember]
        public List<string> StartDateList { get; set; }
    }
}
