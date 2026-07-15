using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class IdValuePair
    {
        [DataMember]
        public int ItemId { get; set; }
        [DataMember]
        public string ItemValue { get; set; }
    }

    [DataContract]
    public class ProgramDetail
    {
        [DataMember]
        public int ProgramId { get; set; }
        [DataMember]
        public int? ProgramDisplayGroupId { get; set; }
        [DataMember]
        public string ProgramDisplayGroupName { get; set; }
        [DataMember]
        public string ProgramDisplayGroupDescription { get; set; }

        [DataMember]
        public int? ProgramProductIdClick { get; set; }
        [DataMember]
        public string ProgramName { get; set; }
        [DataMember]
        public int ProgramTypeId { get; set; }
        [DataMember]
        public int ProgramLevelId { get; set; }
        [DataMember]
        public int InstitutionId { get; set; }
        [DataMember]
        public string InstitutionName { get; set; }
        [DataMember]
        public string ProgramDescription { get; set; }
        [DataMember]
        public string Requirements { get; set; }
        [DataMember]
        public string LearningFormat { get; set; }
        [DataMember]
        public string CreditsRequired { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public string AccreditationDescription { get; set; }
        [DataMember]
        public string InternationalRequirements { get; set; }
        [DataMember]
        public string InternationalTuition { get; set; }
        [DataMember]
        public string AdditionalTuitionInfo { get; set; }
        [DataMember]
        public string FinancialAidInfo { get; set; }
        [DataMember]
        public string AdditionalFinancialAidInfo { get; set; }
        [DataMember]
        public string ProgramLevelName { get; set; }
        [DataMember]
        public string OnCampusRequirements { get; set; }
        [DataMember]
        public string NumberOfStartDates { get; set; }
        [DataMember]
        public string CourseLength { get; set; }
        [DataMember]
        public string CostPerCredit { get; set; }
        [DataMember]
        public DisclaimerType? ProgramDisclaimerType { get; set; }
        [DataMember]
        public string ProgramDisclaimer { get; set; }
        [DataMember]
        public List<IdValuePair> CategoryList { get; set; }
        [DataMember]
        public List<IdValuePair> SubjectList { get; set; }
        [DataMember]
        public List<IdValuePair> SpecialtyList { get; set; }
        [DataMember]
        public bool FailedValidation { get; set; }
        [DataMember]
        public List<IdValuePair> ProgramDisplayGroupProgramList { get; set; }
        [DataMember]
        public string ClickThroughUrl { get; set; }
        [DataMember]
        public PaidStatusType? PaidStatusTypeId { get; set; }
        [DataMember]
        public List<Image> ImageList { get; set; }
        [DataMember]
        public bool? InquiryDisabled { get; set; }
        [DataMember]
        public bool? IsHybrid { get; set; }
        [DataMember]
        public int? SchoolId { get; set; }
        [DataMember]
        public string SchoolName { get; set; }
        //[DataMember]
        //public bool HasSchoolLogo { get; set; }
        [DataMember]
        public string SchoolLogoURL { get; set; }
        //[DataMember]
        //public bool HasProgramLogo { get; set; }
        [DataMember]
        public string ProgramLogoURL { get; set; }
        [DataMember]
        public Campus ProgramCampus { get; set; }
        [DataMember]
        public string OutStateTuition { get; set; }
        [DataMember]
        public string InstateTuition { get; set; }
        [DataMember]
        public string CostPerCreditGrad { get; set; }
        [DataMember]
        public List<Address> ProgramAddressList { get; set; }
        [DataMember]
        public List<int> TermList { get; set; }
        [DataMember]
        public List<int> DurationList { get; set; }
        [DataMember]
        public List<int> WorkTypeList { get; set; }
        [DataMember]
        public List<int> TeachAbroadTypeList { get; set; }
        [DataMember]
        public List<int> PlacementAudienceList { get; set; }
        [DataMember]
        public int? LanguageId { get; set; }
        [DataMember]
        public string Scholarships { get; set; }
        [DataMember]
        public string ScholarshipsDescription { get; set; }
        [DataMember]
        public string SettingDescription { get; set; }
        [DataMember]
        public string ShortDescription { get; set; }
        [DataMember]
        public SFProductCode? ProgramSFProductCode { get; set; }
        [DataMember]
        public string EligibilityRequirements { get; set; }

        [DataMember]
        public int? TemplateId { get; set; }

        [DataMember]
        public string DomesticFinancialAidInfo { get; set; }
        [DataMember]
        public string DomesticFinancialAidDescription { get; set; }
        [DataMember]
        public int CampusIdCanonical { get; set; }
        [DataMember]
        public string CustomTCPA { get; set; }
        [DataMember]
        public string CustomContactCenterTCPA { get; set; }
    }
}
