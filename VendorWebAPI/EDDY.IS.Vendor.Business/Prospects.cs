using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;


namespace EDDY.IS.Vendor.Business
{
    public class Prospects : VendorBase
    {
        public VendorResponseBase Save(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                vendorResponseBase = prospectServiceDAO.SaveProspect(contactRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }
    }
}
