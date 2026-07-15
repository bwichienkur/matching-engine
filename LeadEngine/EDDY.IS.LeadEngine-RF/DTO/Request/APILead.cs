using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.LeadEngine.DTO.Request
{
    [DataContract]
    public class APILead
    {
        [DataMember]
        public string Prefix { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Address1 { get; set; }

        [DataMember]
        public string DialerKey { get; set; }

        [DataMember]
        public string TSR { get; set; }

        [DataMember]
        public int? UserId { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string StateProvince { get; set; }

        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public string USCitizen { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public string Phone1 { get; set; }

        [DataMember]
        public string Phone2 { get; set; }

        [DataMember]
        public Nullable<int> Age { get; set; }

        [DataMember]
        public Nullable<int> YearHighestEduCompleted { get; set; }

        [DataMember]
        public string HighestLevelOfEdu { get; set; }

        [DataMember]
        public string Military { get; set; }

        [DataMember]
        public string StartDate { get; set; }

        [DataMember]
        public List<AdditionalField> AdditionalFields { get; set; }

        [DataMember]
        public Nullable<int> ProspectFlowStatusHistoryId { get; set; }
        
        [DataMember]
        public Nullable<int> ChatIntegrationSessionId { get; set; }

        [DataMember]
        public List<int> Categories { get; set; }

        [DataMember]
        public List<int> SubCategories { get; set; }

        [DataMember]
        public List<int> Specialties { get; set; }

        [DataMember]
        public Nullable<int> DesiredDegreeLevel { get; set; }

        [DataMember]
        public Guid LeadId_Token { get; set; }

        [DataMember]
        public Nullable<Guid> ExternalMatchItemGuid { get; set; }

        [DataMember]
        public bool PreValidatedProgram { get; set; }

        [DataMember]
        public int? ClientRelationContactId { get; set; }

        [DataMember]
        public int? ProspectFlowId { get; set; }

        [DataMember]
        public int ChannelId { get; set; }
        [DataMember]
        public int SubChannelId { get; set; }
        [DataMember]
        public decimal? EstimatedRevShare { get; set; }

        [DataMember]
        public string InstitutionName { get; set; }

        [DataMember]
        public bool ValidateTCPA { get; set; }
    }

    [DataContract]
    public class AdditionalField
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
