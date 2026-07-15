using System;
using System.Collections.Generic;

namespace EDDY.IS.Vendor.Entities
{
    public class VendorAPIList
    {
        //private int startIndex;
        private int startPage;
        private int pageSize;
        private int totalItems;
        private int totalPages;
        private List<Object> itemList;

        //public int StartIndex
        //{
        //    get
        //    {
        //        return startIndex;
        //    }

        //    set
        //    {
        //        startIndex = value;
        //    }
        //}

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

        public List<Object> ItemList
        {
            get
            {
                return itemList;
            }

            set
            {
                itemList = value;
            }
        }
    }
}
