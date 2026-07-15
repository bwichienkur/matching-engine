using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;

namespace EDDY.IS.Vendor.DataAccess
{
    public class VendorCampaignsDAO : VendorBaseDAO
    {
        public VendorCampaign GetVendorCampaignByTrackId(Guid trackId)
        {
            VendorCampaign vendorCampaign = null;
            try
            {
                vendorCampaign = this.getCampaignByTrackId(trackId);

            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return vendorCampaign;
        }


    }
}
