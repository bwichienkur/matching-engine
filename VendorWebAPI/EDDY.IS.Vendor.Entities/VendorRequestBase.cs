using System;
using System.Collections.Generic;
namespace EDDY.IS.Vendor.Entities
{
    public class VendorRequestBase
    {
        private Guid apiKey;

        private List<int> categoryIds;

        private List<int> subjectIds;

        private List<int> specialtyIds;

        private List<int> programlevelIds;

        private int pageSize;

      

        private int startPage;

        private int totalItems;

        private int totalPages;

        private bool includeAdditionalProgramQuestions;

        private bool isBeta;



        public Guid APIKey
        {
            get
            {
                return apiKey;
            }

            set
            {
                apiKey = value;
            }
        }
       
        public List<int> CategoryIds
        {
            get
            {
                return categoryIds;
            }

            set
            {
                categoryIds = value;
            }
        }
        public List<int> SubjectIds
        {
            get
            {
                return subjectIds;
            }

            set
            {
                subjectIds = value;
            }
        }
        public List<int> SpecialtyIds
        {
            get
            {
                return specialtyIds;
            }

            set
            {
                specialtyIds = value;
            }
        }
        public List<int> ProgramLevelIds
        {
            get
            {
                return programlevelIds;
            }

            set
            {
                programlevelIds = value;
            }
        }

        public int PageSize
        {
            get
            {
                return pageSize;
            }

            set
            {
                pageSize = value;
            }
        }

       

        public int StartPage
        {
            get
            {
                return startPage;
            }

            set
            {
                startPage = value;
            }
        }

        public int TotalItems
        {
            get
            {
                return totalItems;
            }

            set
            {
                totalItems = value;
            }
        }

        public int TotalPages
        {
            get
            {
                return totalPages;
            }

            set
            {
                totalPages = value;
            }
        }

        public bool IncludeAdditionalProgramQuestions
        {
            get
            {
                return includeAdditionalProgramQuestions;
            }

            set
            {
                includeAdditionalProgramQuestions = value;
            }
        }
        public bool IsBeta
        {
            get
            {
                return isBeta;
            }

            set
            {
                isBeta = value;
            }
        }

    }
}
