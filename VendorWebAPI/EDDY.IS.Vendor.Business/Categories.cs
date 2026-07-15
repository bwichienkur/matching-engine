using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess;

namespace EDDY.IS.Vendor.Business
{
    public class Categories : VendorBase
    {
        private CategoryDAO categoryDAO = new CategoryDAO();

        public virtual Category GetCategory(int categoryId)
        {
            Category category = null;
            try
            {
                category = categoryDAO.GetCategory(categoryId);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return category;
        }

        public VendorAPIList GetCategories(DirectoryRequest categoryRequest)
        {
            VendorAPIList vendorAPIList = null;
            try
            {
                vendorAPIList = matchingServiceDAO.GetCategories(categoryRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorAPIList;
        }

        public VendorAPIList GetSubCategories(DirectoryRequest subCategoryRequest)
        {
            VendorAPIList vendorAPIList = null;
            try
            {
                vendorAPIList = matchingServiceDAO.GetSubCategories(subCategoryRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorAPIList;
        }
    }
}
