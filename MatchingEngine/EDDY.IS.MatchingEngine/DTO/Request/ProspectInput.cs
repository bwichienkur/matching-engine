using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class ProspectInput
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Phone1 { get; set; }

        [DataMember]
        public string Phone2 { get; set; }

        [DataMember]
        public string StreetAddress { get; set; }

        [DataMember]
        public string AddressLine2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public int? Age { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public int? StateId { get; set; }

        [DataMember]
        public int? CountryId { get; set; }

        [DataMember]
        public int? EducationLevelId { get; set; }

        [DataMember]
        public int? HSGraduationYear { get; set; }

        [DataMember]
        public bool? IsCitizen { get; set; }

        [DataMember]
        public int? GPAKeyValueId { get; set; }

        [DataMember]
        public int? YearsWorkExperienceKeyValueId { get; set; }

        [DataMember]
        public int? YearsTeachingExperienceKeyValueId { get; set; }

        [DataMember]
        public bool? IsMilitary { get; set; }

        [DataMember]
        public int? MilitaryStatusId { get; set; }

        [DataMember]
        public List<KeyValuePair<string, int>> KVCodeData { get; set; }

        [DataMember]
        public string ExternalLeadId { get; set; }

        [DataMember]
        public bool? IsMobileNumber { get; set; }

        [DataMember]
        public Guid? CampaignTrackId { get; set; }

        [DataMember]
        public string TrackingSessionGUID { get; set; }

        [DataMember]
        public string CecUniqueId { get; set; }

        [DataMember]
        public List<KeyValuePair<string, string>> DynamicFields { get; set; }
    }
}