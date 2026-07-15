using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;

namespace EDDY.IS.Vendor.Business
{
    public class Levels : VendorBase
    {
        public VendorAPIList GetLevels(DirectoryRequest programRequest)
        {
            VendorAPIList vendorAPIList = null;
            try
            {
                vendorAPIList = matchingServiceDAO.GetLevels(programRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorAPIList;
        }
    }

}
