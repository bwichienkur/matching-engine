using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ProgramValidateResponse
    {
        [DataMember]
        public bool PassedValidation { get; set; }

        [DataMember]
        public int? AlternateProgramProductId { get; set; } //when validation fails, an alternate active programproductid will be returned for the same program but diff product

        [DataMember]
        public List<RuleFailure> RuleFailures { get; set; }

        [DataMember]
        public PaidStatusType? PaidStatusTypeId { get; set; }

        [DataMember]
        public int? ProgramId { get; set; }

        [DataMember]
        public string ProgramName { get; set; }

        [DataMember]
        public string ProgramDescription { get; set; }

        [DataMember]
        public CampusType? ProgramCampusType { get; set; }

        [DataMember]
        public int? CampusId { get; set; }

        [DataMember]
        public string CampusName { get; set; }

        //[DataMember]
        //public bool? HasCampusLogo { get; set; }

        [DataMember]
        public string CampusLogoURL { get; set; }

        [DataMember]
        public int? InstitutionId { get; set; }

        [DataMember]
        public string InstitutionName { get; set; }

        [DataMember]
        public string InstitutionDescription { get; set; }

        [DataMember]
        public string InstitutionDescriptionInternational { get; set; }

        [DataMember]
        public ProgramType? ProgramType { get; set; }

        [DataMember]
        public int? ProgramProductId { get; set; }

        [DataMember]
        public string InstitutionLogoURL { get; set; }

        [DataMember]
        public bool SkipSchoolSelection { get; set; }

        [DataMember]
        public decimal? RevenuePerLead { get; set; }

        [DataMember]
        public decimal? EffectiveRevenuePerLead { get; set; }

        [DataMember]
        public decimal? LeadPingScoreCPL { get; set; }

        [DataMember]
        public string Score { get; set; }
        [DataMember]
        public int? ScoreId { get; set; }
    }

    public class RuleFailure
    {
        [DataMember]
        public BaseRuleType? RuleFailureType { get; set; }

        [DataMember]
        public string RuleFailureName { get; set; }

        [DataMember]
        public EntityMeta? EntityType { get; set; }

        [DataMember]
        public string StandardControlCode { get; set; }
    }
}
