using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DTO.Response;

namespace EDDY.IS.MatchingEngine
{
    [Serializable()]
    public class MatchItemInternal : ICloneable
    {
        public MatchItemInternal()
        {
            TermList = new HashSet<int>();
            DurationList = new HashSet<int>();
            WorkTypeList = new HashSet<int>();
            PlacementAudienceList = new HashSet<int>();
            TeachAbroadTypeList = new HashSet<int>();
            ProgramAddressList = new List<Address>();
            CategoryMappings = new List<Tuple<int, int, int, bool>>();
        }
        //public string Key
        //{
        //    get
        //    {
        //        if (this.ProgramTypeId == 1 || this.ProgramTypeId == 4)
        //            return this.ProgramProductId + "_" + this.CategoryId + "_" + this.SubjectId + "_" + this.SpecialtyId;
        //        else
        //            return this.ProgramProductId.ToString();
        //    }
        //}
        public int Key
        {
            get
            {
                return this.ProgramProductId;
            }
        }

        #region from VW_Matching_ProgramProductWithApplicationSubject
        public HashSet<int> MatchingSubjects(List<int> Categories, List<int> Subjects, List<int> Specialties)
        {
            HashSet<int> matchingSubjects = new HashSet<int>();

            foreach (var o in CategoryMappings)
            {
                if ((Categories == null || Categories.Count == 0 || Categories.Contains(o.Item1)) &&
                   (Subjects == null || Subjects.Count == 0 || Subjects.Contains(o.Item2)) &&
                   (Specialties == null || Specialties.Count == 0 || Specialties.Contains(o.Item3)))
                    matchingSubjects.Add(o.Item2);
            }

            return matchingSubjects;
        }

        public HashSet<int> MatchingSpecialties(List<int> Categories, List<int> Subjects, List<int> Specialties)
        {
            HashSet<int> matchingSpecialties = new HashSet<int>();

            foreach (var o in CategoryMappings)
            {
                if ((Categories == null || Categories.Count == 0 || Categories.Contains(o.Item1)) &&
                   (Subjects == null || Subjects.Count == 0 || Subjects.Contains(o.Item2)) &&
                   (Specialties == null || Specialties.Count == 0 || Specialties.Contains(o.Item3)))
                    matchingSpecialties.Add(o.Item3);
            }

            return matchingSpecialties;
        }

        public HashSet<int> MatchingCategories(List<int> Categories, List<int> Subjects, List<int> Specialties)
        {
            HashSet<int> matchingCategories = new HashSet<int>();

            foreach (var o in CategoryMappings)
            {
                if ((Categories == null || Categories.Count == 0 || Categories.Contains(o.Item1)) &&
                   (Subjects == null || Subjects.Count == 0 || Subjects.Contains(o.Item2)) &&
                   (Specialties == null || Specialties.Count == 0 || Specialties.Contains(o.Item3)))
                    matchingCategories.Add(o.Item1);
            }

            return matchingCategories;
        }

        public HashSet<int> Categories()
        {
            HashSet<int> categories = new HashSet<int>();

            foreach (var o in CategoryMappings)
            {
                categories.Add(o.Item1);
            }
            return categories;
        }

        public HashSet<int> Subjects()
        {
            HashSet<int> categories = new HashSet<int>();

            foreach (var o in CategoryMappings)
            {
                categories.Add(o.Item2);
            }
            return categories;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int ProgramProductId { get; set; }

        public int InstitutionId { get; set; }

        public int CampusId { get; set; }

        public int ProgramId { get; set; }

        public int ClientRelationshipId { get; set; }

        public int ClientId { get; set; }

        public int PsiId { get; set; }

        public int ProductId { get; set; }

        public int ClientRelationProductMappingId { get; set; }

        public int ClientCampusRelationshipId { get; set; }

        public int ClientCampusProductMappingId { get; set; }

        public string AccreditationOrganization { get; set; }

        public int ProgramLevelId { get; set; }

        public int ProgramTypeId { get; set; }

        public int ProgramCampusTypeId { get; set; }

        public int CampusCampusTypeId { get; set; }

        //public int ApplicationId { get; set; }

        public int? PrimaryCategoryId { get; set; }

        public int? PrimarySubjectId { get; set; }

        public int? PrimarySpecialtyId { get; set; }
        public List<Tuple<int, int, int, bool>> CategoryMappings { get; set; }

        public bool IncludeAllZipCodes { get; set; }

        public bool ProgramProductUseZipCodeRules { get; set; }

        public int ProgramProductAllowableRadius { get; set; }

        public string ProgramProductRadiusZipCode { get; set; }

        public bool ProgramProductZipCodeInclusion { get; set; }

        public bool SetRankScoreZero { get; set; }

        public bool ProgramProductZipCodeExclusion { get; set; }

        public bool CanSmartMatch { get; set; }

        public string ClickThroughUrl { get; set; }

        public bool IsHybrid { get; set; }

        public bool InquiryDisabled { get; set; }

        public int? ProgramDisplayGroupId { get; set; }

        public bool RequiresSystemTemplateUse { get; set; }

        public bool ShowLeadShare2U { get; set; }

        public bool LeadRefinementEnabled { get; set; }

		public int? CampusOptionGroupId { get; set; }

        public int? AdvertiserId { get; set; }

        /// <summary>
        /// Flag for Route Outbound Live Transfers as Titanium
        /// </summary>
        public bool? UpsellOutboundTitanium { get; set; }
        #endregion

        #region from content cache

        public string InstitutionName { get; set; }

        public DisclaimerType? InstitutionDisclaimerType { get; set; }

        public string InstitutionDisclaimer { get; set; }

        public string CampusName { get; set; }

        public string ProgramName { get; set; }
        public string ProgramDisplayName { get; set; }

        public string DegreeAcronym { get; set; }

        public DisclaimerType? ProgramDisclaimerType { get; set; }

        public string ProgramDisclaimer { get; set; }

        public string ProgramDescription { get; set; }

        public string ProgramShortDescription { get; set; }

        public string GradSchoolsDegreeName { get; set; }

        public string InstitutionDescription { get; set; }

        public string InstitutionDescriptionInternational { get; set; }

        public string CampusAddress1 { get; set; }

        public string CampusAddress2 { get; set; }

        public string CampusCity { get; set; }

        public string CampusStateCode { get; set; }

        public string CampusPostalCode { get; set; }

        public string CampusCountry { get; set; }

        public string CampusCountryName { get; set; }

        public int? CampusCountryId { get; set; }

        public int? CampusStateId { get; set; }

        public int? CampusCityId { get; set; }

        //public bool HasCampusLogo { get; set; }

        public string CampusLogoURL { get; set; }

        public string ProgramLevelName { get; set; }

        public string SubjectName { get; set; }

        public string CategoryName { get; set; }

        //public string SpecialtyName { get; set; }

        public string PhoneNumber { get; set; }

        public string Locale { get; set; }

        public string CampusPhone { get; set; }

        public string CampusFax { get; set; }

        //public bool HasSchoolLogo { get; set; }

        public string SchoolLogoURL { get; set; }

        //public bool HasProgramLogo { get; set; }

        public string ProgramLogoURL { get; set; }

        public int? SchoolId { get; set; }

        public bool IsFaithBased { get; set; }

        public bool HasAdditionalScript { get; set; }

        public List<Address> ProgramAddressList { get; set; }

        public string InstitutionLogoURL { get; set; }

        #endregion

        #region from rules / SRA processing



        public PaidStatusType? PaidStatusTypeId { get; set; }

        public decimal CapRoom { get; set; }

        public decimal eRPL { get; set; }
        public decimal RPL { get; set; }

        public decimal ClickPrice { get; set; }

        #endregion

        public int? TemplateId { get; set; }

        public bool HasClick { get; set; }


        //Images
        public List<Image> InstitutionImageList { get; set; }
        public List<Image> CampusImageList { get; set; }
        public List<Image> ProgramImageList { get; set; }

        //StartDates
        public List<string> InstitutionStartDateList { get; set; }
        public List<string> ProgramStartDateList { get; set; }

        //Tags
        public List<ProgramTag> ProgramTagList { get; set; }

        //SAB filters
        public Nullable<int> LanguageId { get; set; }
        public HashSet<int> TermList { get; set; }
        public HashSet<int> DurationList { get; set; }
        public HashSet<int> WorkTypeList { get; set; }
        public HashSet<int> PlacementAudienceList { get; set; }
        public HashSet<int> TeachAbroadTypeList { get; set; }

        public SFProductCode? SFProductCode_PSI { get; set; }
        public SFProductCode? SFProductCode_CR { get; set; }
        public int? SABSRAPosition_CR { get; set; }
        public int? SABSRAPosition_PSI { get; set; }
        public bool ExcludeMatch1plusForFinAid { get; set; }
        public bool IsNonProfit { get; set; }

        public string ProgramDisplayGroupName { get; set; }
        public string ProgramDisplayGroupDescription { get; set; }
        public string ProgramDisplayGroupShortDescription { get; set; }

        public string ProgramCode { get; set; }
        public bool TreatAsMatch1 { get; set; }
        public bool AllowCrossSell { get; set; }
        public bool RequireJournayaLeadId { get; set; }
        public string CustomTCPA { get; set; }
        public string CustomContactCenterTCPA { get; set; }

        public string ProductGrouping()
        {
            if (Constants.Product.IsWarmTransferProduct(this.ProductId))
                return "LT";
            else
                return "Form";
        }
    }

    public class MatchItem
    {
        public MatchItemInternal Match { get; set; }
        public bool FailedValidation { get; set; }
        public decimal ProgramRankScore { get; set; }
        public bool IsGeotargeted { get; set; }
        public bool AllowedViaLeadScoringUpsell { get; set; }
        public Base.InstitutionLeadTypes InstitutionLeadType { get; set; }
        public List<MatchItemFactorScore> FactorScores { get; set; }
        public RemovalReason RemovalReason { get; set; }
        public Guid? ExternalMatchItemGuid { get; set; }
        public List<Address> FilteredProgramAddressList { get; set; }
        public bool? LeadTypeLeadCapped { get; set; }
        public bool? LeadTypeClickCapped { get; set; }
        public string Score { get; set; }
        public int? ScoreId { get; set; }

        /// <summary>
        /// Flag indicating whether the match institution is from a national online advertiser.
        /// </summary>
        public bool IsNationalOnlineAdvertiser { get; set; }

        public MatchItem(MatchItemInternal mii)
        {
            Match = mii;
            InstitutionLeadType = Base.InstitutionLeadTypes.School;
            FilteredProgramAddressList = mii.ProgramAddressList;
        }

        public MatchItem(MatchItemInternal mii, Base.InstitutionLeadTypes ilt)
        {
            Match = mii;
            InstitutionLeadType = ilt;
            FilteredProgramAddressList = mii.ProgramAddressList;
        }
    }
    public class MatchItemFactorScore
    {
        public FactorType BusinessModelFactorId { get; set; }
        public decimal? FactorValue { get; set; }
        public decimal FactorScore { get; set; }
    }

    public class RemovalReason
    {
        public EntityMeta RuleEntity { get; set; }
        public int? RuleEntityEntityId { get; set; }
        public BaseRuleType RuleType { get; set; }
        public string RuleDetail { get; set; }
    }
}
