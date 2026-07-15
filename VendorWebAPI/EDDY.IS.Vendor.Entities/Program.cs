using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace EDDY.IS.Vendor.Entities
{
    [DataContract]
    public class Program
    {
       
        //private CampusBase campus;
        private ProgramCategory category;
        //private SubCategory subCategory;
        private string directoryURL;
        private List<ProgramEducationLevel> educationLevels;
        private string formURL;
        private int formTemplateId;
        //private bool allCountriesAvailable;
        private List<Country> availableCountries;
        //private bool allStatesAvailable;
        private List<State> availableStates;
        private LevelBase programLevel;
        private string logoURL;
        private int programId;
        private string programDescription;
        private string programName;
        private InstitutionBase institution;
        private bool isZipcodeIncluded;
        private bool isEnabled;
        private decimal estimatedRevShare;

        //public bool AllCountriesAvailable
        //{
        //    get
        //    {
        //        return allCountriesAvailable;
        //    }

        //    set
        //    {
        //        allCountriesAvailable = value;
        //    }
        //}
        [JsonIgnore]
        [DataMember(Order = 11)]
        public List<Country> AvailableCountries
        {
            get
            {
                return availableCountries;
            }

            set
            {
                availableCountries = value;
            }
        }
        //public bool AllStatesAvailable
        //{
        //    get
        //    {
        //        return allStatesAvailable;
        //    }

        //    set
        //    {
        //        allStatesAvailable = value;
        //    }
        //}
        [JsonIgnore]
        [DataMember(Order = 10)]
        public List<State> AvailableStates
        {
            get
            {
                return availableStates;
            }

            set
            {
                availableStates = value;
            }
        }

        //public CampusBase Campus
        //{
        //    get
        //    {
        //        return campus;
        //    }

        //    set
        //    {
        //        campus = value;
        //    }
        //}
        [DataMember(Order = 5)]
        public ProgramCategory Category
        {
            get
            {
                return category;
            }

            set
            {
                category = value;
            }
        }

        //public SubCategory SubCategory
        //{
        //    get
        //    {
        //        return subCategory;
        //    }

        //    set
        //    {
        //        subCategory = value;
        //    }
        //}
        [DataMember(Order = 8)]
        public string DirectoryURL
        {
            get
            {
                return directoryURL;
            }

            set
            {
                directoryURL = value;
            }
        }
        [DataMember(Order = 4)]
        public List<ProgramEducationLevel> EducationLevels
        {
            get
            {
                return educationLevels;
            }

            set
            {
                educationLevels = value;
            }
        }
        [JsonIgnore]
        [DataMember(Order = 7)]
        public string FormURL
        {
            get
            {
                return formURL;
            }

            set
            {
                formURL = value;
            }
        }


        [DataMember(Order = 3)]
        public LevelBase ProgramLevel
        {
            get
            {
                return programLevel;
            }

            set
            {
                programLevel = value;
            }
        }
        [JsonIgnore]
        [DataMember(Order = 6)]
        public string LogoURL
        {
            get
            {
                return logoURL;
            }

            set
            {
                logoURL = value;
            }
        }
        [DataMember(Order = 2)]
        public string ProgramDescription
        {
            get
            {
                return programDescription;
            }

            set
            {
                programDescription = value;
            }
        }
        [DataMember(Order = 0)]
        public int ProgramId
        {
            get
            {
                return programId;
            }

            set
            {
                programId = value;
            }
        }
        [DataMember(Order = 1)]
        public string ProgramName
        {
            get
            {
                return programName;
            }

            set
            {
                programName = value;
            }
        }
        [DataMember(Order = 9)]
        public InstitutionBase Institution
        {
            get
            {
                return institution;
            }

            set
            {
                institution = value;
            }
        }
        //[DataMember(Order = 9)]
        //public int FormTemplateId
        //{
        //    get
        //    {
        //        return formTemplateId;
        //    }

        //    set
        //    {
        //        formTemplateId = value;
        //    }
        //}

        public bool IsZipcodeIncluded
        {
            get
            {
                return isZipcodeIncluded;
            }

            set
            {
                isZipcodeIncluded = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }
        [DataMember(Order = 3)]
        public decimal EstimatedRevShare
        {
            get
            {
                return estimatedRevShare;
            }

            set
            {
                estimatedRevShare = value;
            }
        }
    }
}
