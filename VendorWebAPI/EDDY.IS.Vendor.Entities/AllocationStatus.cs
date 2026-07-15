using System;
using System.Collections.Generic;

namespace EDDY.IS.Vendor.Entities
{
    public class AllocationStatus
    {
        bool hasAllocation;

        private List<int> categoryIds;

        private List<int> subjectIds;

        private List<int> levelIds;

        public bool HasAllocation
        {
            get
            {
                return hasAllocation;
            }

            set
            {
                hasAllocation = value;
            }
        }

        public List<int> CheckedCategoryIds
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
        public List<int> CheckedSubjectIds
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
        public List<int> CheckedLevelIds
        {
            get
            {
                return levelIds;
            }

            set
            {
                levelIds = value;
            }
        }
    }
}
