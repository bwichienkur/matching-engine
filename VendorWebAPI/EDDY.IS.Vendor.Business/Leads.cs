using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess;
using System.Collections.Generic;
using Newtonsoft.Json;
using EDDY.IS.Vendor.Utilities;

namespace EDDY.IS.Vendor.Business
{
    public class Leads : VendorBase
    {
        public VendorResponseBase SaveLead(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                //Creating objects needed to check program incase of swapped fields 
                Programs programs = new Programs();
                Campuses campuses = new Campuses();
                DirectoryRequest directoryRequest = new DirectoryRequest();
                directoryRequest.APIKey = contactRequest.APIKey;
                directoryRequest.ProgramDelimiter = String.Empty;

                //Get ProgramList & CampusList to check for swapped fields to update contactrequest with the correct ProgramId
                programs.GetProgramsForMicrosite(directoryRequest, out List<ProgramMicrosite> ProgramMicrositeList, true);
                campuses.GetCampusesForMicrosite(directoryRequest, out List<CampusMicrosite> CampusMicrositeList, true);
                contactRequest.ProgramId = formsServiceDAO.CheckCampusProgramForSwappedFields(contactRequest.ProgramId, contactRequest.CampusId, CampusMicrositeList, ProgramMicrositeList);


                vendorResponseBase = formsServiceDAO.SaveLead(contactRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }
        public VendorResponseBase SaveEduMaxLead(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                vendorResponseBase = formsServiceDAO.SaveEduMaxLead(contactRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }
        public VendorResponseBase SaveCallCenterLead(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                vendorResponseBase = formsServiceDAO.SaveCallCenterLead(contactRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }
        public VendorResponseBase GetLeadDetails(LeadDetailsRequest leadDetailsRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                vendorResponseBase = prospectServiceDAO.GetProspectFlowDetailsById(leadDetailsRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }
        public VendorResponseBase SaveLeadNote(LeadDetailsNoteRequest leadDetailsNoteRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                vendorResponseBase = prospectServiceDAO.SaveProspectNote(leadDetailsNoteRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }

        public VendorResponseBase UpdateEMSLead(LeadUpdateRequest leadUpdateRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                vendorResponseBase = dataExchangeServiceDAO.UpdateEMSLead(leadUpdateRequest);
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.BUSINESS_POLICY);
                return dataExchangeServiceDAO.CreateLeadUpdateFailureResponse($"Exception Occurred: {ex.Message}", "msg0116");
            }
            return vendorResponseBase;
        }
    }
}
