using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;

namespace EDDY.IS.Vendor.Business
{
    public class GpFive9 : VendorBase
    {
        public VendorResponseBase RouteLead(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                vendorResponseBase = gpFive9ServiceDAO.RouteLead(contactRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }
    }
}
