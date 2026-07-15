using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using System.Collections.Generic;
using System.Linq;

namespace EDDY.IS.Vendor.DataAccess
{
    public class VendorResponseMessagesDAO : VendorBaseDAO
    {

        public VendorResponseMessage GetVendorResponseMessageByMessageCode(string messageCode)
        {
            VendorResponseMessage vendorResponseMessage = null;
            try
            {
                vendorResponseMessage = this.getVendorResponseMessageByMessageCode(messageCode);

            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return vendorResponseMessage;
        }

        public List<Entities.ContactStandardControlMapping> GetAllContactStandardControlMappings()
        {
            List<Entities.ContactStandardControlMapping> maps = null;
            try
            {
                maps = this.getAllContactPropertyToStandardControlMappings();

            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return maps;
        }
    }
}
