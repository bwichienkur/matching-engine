using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class Campus : BaseMatchEntity
    {
        [DataMember]
        public int CampusId { get; set; }

        [DataMember]
        public CampusType? CampusType { get; set; }

        [DataMember]
        public string CampusName { get; set; }

        [DataMember]
        public string Address1 { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public int ProgramCount { get; set; }

        [DataMember]
        public List<Program> ProgramList { get; set; }

        //[DataMember]
        //public bool HasCampusLogo { get; set; }

        [DataMember]
        public string CampusLogoURL { get; set; }

        [DataMember]
        public double? DistanceFromProspect { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Fax { get; set; }

        [DataMember]
        public List<Image> ImageList { get; set; }

        [DataMember]
        public bool IsGeotargeted { get; set; }

        [DataMember]
        public string InstitutionLogoURL { get; set; }
    }
}