using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.LeadEngine.DataModel
{
    [DataContract]
    public partial class BetaLeadDTO
    {
        [DataMember]
        public decimal LeadId { get; set; }
        [DataMember]
        public long RawPostDataId { get; set; }
        [DataMember]
        public decimal SessionInternalId { get; set; }
        [DataMember]
        public decimal VisitorInternalId { get; set; }
        [DataMember]
        public Nullable<decimal> FormUniqueId { get; set; }
        [DataMember]
        public Nullable<int> ChannelID { get; set; }
        [DataMember]
        public Nullable<int> VendorId { get; set; }
        [DataMember]
        public Nullable<int> BusinessModelId { get; set; }
        [DataMember]
        public Nullable<int> ClientRelationshipId { get; set; }
        [DataMember]
        public Nullable<int> PSIId { get; set; }
        [DataMember]
        public Nullable<int> ProgramProductId { get; set; }
        [DataMember]
        public Nullable<int> ProgramId { get; set; }
        [DataMember]
        public Nullable<int> ProgramTypeID { get; set; }
        [DataMember]
        public Nullable<int> ProgramLevelID { get; set; }
        [DataMember]
        public Nullable<int> ProductID { get; set; }
        [DataMember]
        public Nullable<int> ApplicationID { get; set; }
        [DataMember]
        public Nullable<int> CategoryId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Address1 { get; set; }
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
        public string EmailAddress { get; set; }
        [DataMember]
        public string Phone1 { get; set; }
        [DataMember]
        public string Phone2 { get; set; }
        [DataMember]
        public Nullable<int> TimeToStartInWeeks { get; set; }
        [DataMember]
        public string AdditionalFields { get; set; }
        [DataMember]
        public Nullable<int> DeliveryDefinitionId { get; set; }
        [DataMember]
        public int RealtimeDeliveryStatusId { get; set; }
        [DataMember]
        public Nullable<System.Guid> DeliveryEngineMachineKey { get; set; }
        [DataMember]
        public Nullable<bool> IsEnabled { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public int CreatedBy { get; set; }
        [DataMember]
        public System.DateTime UpdatedDate { get; set; }
        [DataMember]
        public int UpdatedBy { get; set; }
        [DataMember]
        public string Prefix { get; set; }
        [DataMember]
        public Nullable<int> Age { get; set; }
        [DataMember]
        public Nullable<int> YearHighestEduCompleted { get; set; }
        [DataMember]
        public string HighestLevelOfEdu { get; set; }
        [DataMember]
        public string Military { get; set; }
        [DataMember]
        public string MethodOfContact { get; set; }
        [DataMember]
        public string StartDate { get; set; }
        [DataMember]
        public string LegacyLeadId { get; set; }
        [DataMember]
        public System.Guid RowGuid { get; set; }
        [DataMember]
        public Nullable<System.Guid> TrackId { get; set; }
        [DataMember]
        public string InitialLeadId { get; set; }
        [DataMember]
        public string CMID { get; set; }
        [DataMember]
        public string AffiliateId { get; set; }
        [DataMember]
        public string UID { get; set; }
        [DataMember]
        public string ExternalLeadId { get; set; }
        [DataMember]
        public string Tsource { get; set; }
        [DataMember]
        public Nullable<System.Guid> TrackingSessionGUID { get; set; }
        [DataMember]
        public string DeliveryEngineMachineName { get; set; }
        [DataMember]
        public Nullable<int> CampusApplicationId { get; set; }
        [DataMember]
        public Nullable<int> CampusProgramId { get; set; }
        [DataMember]
        public Nullable<int> ClientCampusRelationshipId { get; set; }
        [DataMember]
        public Nullable<int> InstitutionApplicationId { get; set; }
        [DataMember]
        public Nullable<int> ProgramApplicationId { get; set; }
        [DataMember]
        public Nullable<int> CampusId { get; set; }
        [DataMember]
        public Nullable<System.Guid> MatchResponseGuid { get; set; }
        [DataMember]
        public Nullable<int> SubmissionId { get; set; }
        [DataMember]
        public Nullable<int> LeadCreationTypeId { get; set; }
        [DataMember]
        public Nullable<int> ProspectFlowStatusHistoryId { get; set; }
        [DataMember]
        public Nullable<int> ChatIntegrationSessionId { get; set; }
        [DataMember]
        public Nullable<int> ProspectId { get; set; }
        [DataMember]
        public Nullable<int> ClientRelationContactId { get; set; }
    }
}
