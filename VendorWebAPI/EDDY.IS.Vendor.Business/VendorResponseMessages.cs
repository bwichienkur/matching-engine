using System;

using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.DataAccess;
using EDDY.IS.Common.ExceptionHandler;
using System.Collections.Generic;
using System.Linq;
namespace EDDY.IS.Vendor.Business
{
    public class VendorResponseMessages : VendorBase
    {
        private VendorResponseMessagesDAO vendorResponseMessagesDAO = new VendorResponseMessagesDAO();
        public VendorResponseMessage GetVendorResponseMessageByMessageCode(string messageCode)
        {
            VendorResponseMessage vendorResponseMessage = null;
            try
            {
                vendorResponseMessage = vendorResponseMessagesDAO.GetVendorResponseMessageByMessageCode(messageCode);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseMessage;
        }

        public List<Entities.ContactStandardControlMapping> GetAllContactStandardControlMappings()
        {
            List<Entities.ContactStandardControlMapping> maps = null;
            try
            {
                maps = vendorResponseMessagesDAO.GetAllContactStandardControlMappings();

            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return maps;
        }
    }
}
