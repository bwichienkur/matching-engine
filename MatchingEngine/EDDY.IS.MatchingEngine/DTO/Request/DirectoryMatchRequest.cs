using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class DirectoryMatchRequest : BaseMatchRequest
    {
        [DataMember]
        public int? PageNumber { get; set; }

        [DataMember]
        public bool? RemoveInvalidEntities { get; set; }
        
        [DataMember]
        public bool? IgnoreTrackingTag { get; set; }

        [DataMember]
        public int? MaxNestedProgramCount { get; set; }

        [DataMember]
        public List<int> InstitutionIdList { get; set; }

        [DataMember]
        public List<int> ProgramIdList { get; set; }

        [DataMember]
        public int? CampusId { get; set; }

		[DataMember]
		public int? CampusOptionGroupId { get; set; }

        [DataMember]
        public GeoTarget GeoTarget { get; set; }

        [DataMember]
        public EntitySortMethod SortMethod { get; set; }

        [DataMember]
        public int? FeatureId { get; set; }

        [DataMember]
        public bool? IncludeProgramGroupRollup { get; set; }

        [DataMember]
        public bool? IncludeOnlyClickPrograms { get; set; }

        [DataMember]
        public int? ProgramGroupId { get; set; }

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
        public List<int> LanguageList { get; set; }

        [DataMember]
        public bool IncludeImages { get; set; }

        [DataMember]
        public LeadCreationType? LeadCreationType { get; set; }

        [DataMember]
        public List<int> ProgramPaidStatusList { get; set; }

        [DataMember]
        public List<int> AdvertiserList { get; set; }

        [DataMember]
        public List<int> ClientRelationshipList { get; set; }
    }

    public enum GetInstitutionCampusOption
    {
        NoCampus = 0,
        CampusOn2ndLevel = 1,
        CampusOn3rdLevel = 2
    }
}
