using EDDY.IS.MatchingEngine.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class Program : BaseMatchEntity
    {
        [DataMember]
        public int ProgramProductId { get; set; }

        [DataMember]
        public int? ProgramProductIdClick { get; set; }

        [DataMember]
        public int ProgramId { get; set; }

        [DataMember]
        public string ProgramName { get; set; }

        [DataMember]
        public string ProgramDescription { get; set; }

        [DataMember]
        public string ProgramShortDescription { get; set; }

        [DataMember]
        public ProgramType ProgramType { get; set; }

        [DataMember]
        public List<string> StartDateList { get; set; }

        [DataMember]
        public List<ProgramTag> TagList { get; set; }

        [DataMember]
        public List<string> InstitutionStartDateList { get; set; }

        [DataMember]
        public int ProgramLevelId { get; set; }

        [DataMember]
        public string ProgramLevelName { get; set; }

        [DataMember]
        public int? SubjectId { get; set; }

        [DataMember]
        public int? CategoryId { get; set; }

        [DataMember]
        public string SubjectName { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public CampusType ProgramCampusType { get; set; }

        [DataMember]
        public DisclaimerType? ProgramDisclaimerType { get; set; }

        [DataMember]
        public string ProgramDisclaimer { get; set; }

        [DataMember]
        public int? TemplateId { get; set; }

        [DataMember]
        public PaidStatusType? PaidStatusTypeId { get; set; }

        [DataMember]
        public string ClickThroughUrl { get; set; }

        [DataMember]
        public bool? InquiryDisabled { get; set; }

        [DataMember]
        public bool? IsHybrid { get; set; }

        [DataMember]
        public int? ProductId { get; set; }

        [DataMember]
        public List<string> ProgramDisplayGroupProgramLevelNameList { get; set; }

        //[DataMember]
        //public bool HasSchoolLogo { get; set; }

        [DataMember]
        public string SchoolLogoURL { get; set; }

        //[DataMember]
        //public bool HasProgramLogo { get; set; }

        [DataMember]
        public string ProgramLogoURL { get; set; }

        [DataMember]
        public int? SchoolId { get; set; }

        [DataMember]
        public bool ShowTwoULeadShareControl { get; set; }

        [DataMember]
        public bool AllowedViaLeadScoringUpsell { get; set; }

        [DataMember]
        public List<Image> ImageList { get; set; }

        [DataMember]
        public List<Address> AddressList { get; set; }

        [DataMember]
        public SFProductCode? ProgramSFProductCode { get; set; }

        [DataMember]
        public int? ProgramDisplayGroupId { get; set; }

        [DataMember]
        public string ProgramDisplayGroupName { get; set; }

        [DataMember]
        public string ProgramDisplayGroupDescription { get; set; }

        [DataMember]
        public string ProgramDisplayGroupShortDescription { get; set; }
        
        [DataMember]
        public string CampusLogoURL { get; set; }

        [DataMember]
        public string InstitutionLogoURL { get; set; }

        [DataMember]
        public Base.InstitutionLeadTypes? InstitutionType { get; set; }

        [DataMember]
        public Guid? ExternalMatchItemGuid { get; set; }

        [DataMember]
        public List<CampusSlim> CampusSlimList { get; set; }

        [DataMember]
        public string DegreeAcronym { get; set; }

        [DataMember]
        public string TransferNumber { get; set; }

        public int? SABSRAPosition_PSI { get; set; }

		[DataMember]
		public string CampusOptionGroup { get; set; }

        [DataMember]
        public int? CampusOptionGroupPosition { get; set; }

        [DataMember]
        public decimal? RevenuePerLead { get; set; }

        [DataMember]
        public decimal? EffectiveRevenuePerLead { get; set; }
    }
}