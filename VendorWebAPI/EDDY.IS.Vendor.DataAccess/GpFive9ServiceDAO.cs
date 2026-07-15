using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Logging;
using EDDY.IS.Vendor.DataAccess.GpFive9Service;

namespace EDDY.IS.Vendor.DataAccess
{
    public class GpFive9ServiceDAO : VendorBaseDAO
    {
        public VendorResponseBase RouteLead(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                GpFive9ServiceClient gpFive9ServiceClient = new GpFive9ServiceClient();
                ProcessRequest request = new ProcessRequest();

                if (!String.IsNullOrEmpty(contactRequest.GlassPanelLeadId))
                {
                    request.GpLeadId = contactRequest.GlassPanelLeadId;
                }
                else {
                    request.GpLeadId = contactRequest.LastName + "_" + contactRequest.Phone;
                }

                request.DialerListName = contactRequest.DialerListName;
                request.FirstName = contactRequest.FirstName;
                request.LastName = contactRequest.LastName;
                request.ModificationDate = !String.IsNullOrEmpty(contactRequest.ModificationDate) ? Convert.ToDateTime(contactRequest.ModificationDate) : DateTime.Now;
                request.PhoneNumber1 = contactRequest.Phone;
                request.PhoneNumber2 = contactRequest.AlternatePhone;
                request.State = contactRequest.State;
                request.ProspectRouteId = 14;
                request.Street = contactRequest.Address;
                request.City = contactRequest.City;
                request.ZipCode = contactRequest.PostalCode;
                request.Email = contactRequest.Email;
                request.ProgramOfInterest = contactRequest.ProgramOfInterest;
                request.Company = contactRequest.Company;

                if (gpFive9ServiceClient.ProcessFive9Route(request))
                {
                    vendorResponseBase = new VendorResponseBase();
                    vendorResponseBase.IsSuccessful = true;                   
                }
            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            return vendorResponseBase;
        }
    }
}
