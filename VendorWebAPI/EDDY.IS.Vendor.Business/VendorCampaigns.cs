using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.DataAccess;
using EDDY.IS.Common.ExceptionHandler;

namespace EDDY.IS.Vendor.Business
{
    public class VendorCampaigns : VendorBase
    {
        private VendorCampaignsDAO vendorCampaignDAO = new VendorCampaignsDAO();
        public VendorCampaign GetVendorCampaignByTrackId(Guid trackId)
        {
            VendorCampaign vendorCampaign = null;
            try
            {
                vendorCampaign = vendorCampaignDAO.GetVendorCampaignByTrackId(trackId);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorCampaign;
        }
    }
}
