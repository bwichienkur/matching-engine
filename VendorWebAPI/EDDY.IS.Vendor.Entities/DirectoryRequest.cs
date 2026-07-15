using System.Collections.Generic;
namespace EDDY.IS.Vendor.Entities
{
    public class DirectoryRequest : VendorRequestBase
    {
       

        private int institutionId;

    

        private int campusId;

        private List<int> campusIds;

        private string campusType;

        private string postalCode;

        private List<QuestionAnswer> prospectFilters;

        private int programId;

        private int featureId;
        private int educationLevelId;
        private string programDelimiter;

        public int FeatureId
        {
            get
            {
                return featureId;
            }
            set
            {
                featureId = value;
            }
        }

        public int InstitutionId
        {
            get
            {
                return institutionId;
            }

            set
            {
                institutionId = value;
            }
        }

     

        public int CampusId
        {
            get
            {
                return campusId;
            }

            set
            {
                campusId = value;
            }
        }

        public string CampusType
        {
            get
            {
                return campusType;
            }

            set
            {
                campusType = value;
            }
        }

        public string PostalCode
        {
            get
            {
                return postalCode;
            }

            set
            {
                postalCode = value;
            }
        }

        public List<QuestionAnswer> ProspectFilters
        {
            get
            {
                return prospectFilters;
            }

            set
            {
                prospectFilters = value;
            }
        }
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

        public List<int> CampusIds
        {
            get
            {
                return campusIds;
            }
            set
            {
                campusIds = value;
            }
        }

        public int EducationLevelId
        {
            get
            {
                return educationLevelId;
            }

            set
            {
                educationLevelId = value;
            }
        }

        public string ProgramDelimiter
        {
            get
            {
                return programDelimiter;
            }

            set
            {
                programDelimiter = value;
            }
        }
    }
}
