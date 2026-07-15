using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Core;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ProgramWithInstitutionCampus : Program
    {
        public ProgramWithInstitutionCampus()
        {

        }
        public ProgramWithInstitutionCampus(Program program)
        {
            this.AddressList = program.AddressList;
            this.AllowedViaLeadScoringUpsell = program.AllowedViaLeadScoringUpsell;
            this.CategoryId = program.CategoryId;
            this.ClickThroughUrl = program.ClickThroughUrl;
            this.FailedValidation = program.FailedValidation;
            //this.HasSchoolLogo = program.HasSchoolLogo;
            //this.HasProgramLogo = program.HasProgramLogo;
            this.SchoolLogoURL = program.SchoolLogoURL;
            this.ProgramLogoURL = program.ProgramLogoURL;
            this.ImageList = program.ImageList;
            this.InquiryDisabled = program.InquiryDisabled;
            this.IsHybrid = program.IsHybrid;
            this.PaidStatusTypeId = program.PaidStatusTypeId;
            this.ProductId = program.ProductId;
            this.ProgramCampusType = program.ProgramCampusType;
            this.ProgramDescription = program.ProgramDescription;
            this.ProgramDisclaimer = program.ProgramDisclaimer;
            this.ProgramDisclaimerType = program.ProgramDisclaimerType;
            this.ProgramDisplayGroupProgramLevelNameList = program.ProgramDisplayGroupProgramLevelNameList;
            this.ProgramId = program.ProgramId;
            this.ProgramLevelId = program.ProgramLevelId;
            this.ProgramLevelName = program.ProgramLevelName;
            this.ProgramName = program.ProgramName;
            this.ProgramProductId = program.ProgramProductId;
            this.ProgramProductIdClick = program.ProgramProductIdClick;
            this.ProgramRankScore = program.ProgramRankScore;
            this.ProgramSFProductCode = program.ProgramSFProductCode;
            this.ProgramShortDescription = program.ProgramShortDescription;
            this.ProgramType = program.ProgramType;
            this.SchoolId = program.SchoolId;
            this.ShowTwoULeadShareControl = program.ShowTwoULeadShareControl;
            this.SubjectId = program.SubjectId;
            this.TemplateId = program.TemplateId;
            this.ProgramDisplayGroupId = program.ProgramDisplayGroupId;
            this.ProgramDisplayGroupName = program.ProgramDisplayGroupName;
            this.ProgramDisplayGroupShortDescription = program.ProgramDisplayGroupShortDescription;
            this.ProgramDisplayGroupDescription = program.ProgramDisplayGroupDescription;
            this.CampusLogoURL = program.CampusLogoURL;
            this.InstitutionLogoURL = program.InstitutionLogoURL;
            this.ExternalMatchItemGuid = program.ExternalMatchItemGuid;
            this.InstitutionType = program.InstitutionType;
            this.StartDateList = program.StartDateList;
            this.InstitutionStartDateList = program.InstitutionStartDateList;
            this.DegreeAcronym = program.DegreeAcronym;
        }
        [DataMember]
        public bool RequiresSystemTemplateUse { get; set; }

        [DataMember]
        public bool Is2USchool { get; set; }

        [DataMember]
        public int InstitutionId { get; set; }

        [DataMember]
        public string InstitutionName { get; set; }

        [DataMember]
        public DisclaimerType? InstitutionDisclaimerType { get; set; }

        [DataMember]
        public string InstitutionDisclaimer { get; set; }

        [DataMember]
        public string InstitutionDescription { get; set; }

        [DataMember]
        public string InstitutionDescriptionInternational { get; set; }

        [DataMember]
        public int CampusId { get; set; }

        [DataMember]
        public CampusType CampusType { get; set; }

        [DataMember]
        public string CampusName { get; set; }

        [DataMember]
        public string CampusAddress1 { get; set; }

        [DataMember]
        public string CampusAddress2 { get; set; }

        [DataMember]
        public string CampusCity { get; set; }

        [DataMember]
        public string CampusState { get; set; }

        [DataMember]
        public string CampusPostalCode { get; set; }

        [DataMember]
        public int CampusCountryId { get; set; }

        [DataMember]
        public string CampusCountryCode { get; set; }

        [DataMember]
        public string CampusCountryName { get; set; }

        [DataMember]
        public string CampusPhone { get; set; }

        [DataMember]
        public string CampusFax { get; set; }

        [DataMember]
        public string AccreditationOrganization { get; set; }

        [DataMember]
        public bool RemonetizationRestriction { get; set; }        

        public bool TreatAsMatch1 { get; set; }

        [DataMember]
        public int ClientRelationshipId { get; set; }
        [DataMember]
        public string CustomTCPA { get; set; }
    }
}
