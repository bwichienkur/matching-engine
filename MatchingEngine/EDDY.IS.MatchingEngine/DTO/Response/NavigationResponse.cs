using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DTO
{
    [DataContract]
    public class NavigationResponse : BaseMatchResponse
    {
        [DataMember]
        public List<ProgramLevel> ProgramLevels { get; set; }

        [DataMember]
        public List<Category> Categories { get; set; }

        [DataMember]
        public List<Subject> Subjects { get; set; }

        [DataMember]
        public List<Specialty> Specialties { get; set; }

        [DataMember]
        public List<CampusType> CampusTypes { get; set; }

        [DataMember]
        public List<CampusCountry> Countries { get; set; }

        [DataMember]
        public List<CampusState> States { get; set; }

        [DataMember]
        public List<City> Cities { get; set; }

        [DataMember]
        public bool Hybrid { get; set; }

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

        public NavigationResponse()
        {
            Categories = new List<Category>();
            ProgramLevels = new List<ProgramLevel>();
            Subjects = new List<Subject>();
            Specialties = new List<Specialty>();
            CampusTypes = new List<CampusType>();
            Countries = new List<CampusCountry>();
            States = new List<CampusState>();
            Cities = new List<City>();
            TermList = new List<int>();
            DurationList = new List<int>();
            WorkTypeList = new List<int>();
            TeachAbroadTypeList = new List<int>();
            PlacementAudienceList = new List<int>();
            LanguageList = new List<int>();
        }
    }
}
