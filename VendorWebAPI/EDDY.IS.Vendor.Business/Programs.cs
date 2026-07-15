using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess;
using System.Collections.Generic;

namespace EDDY.IS.Vendor.Business
{
    public class Programs : VendorBase
    {
        private ProgramDAO programDAO = new ProgramDAO();

        public virtual Entities.Program GetProgram(int programId)
        {
            Entities.Program program = null;
            try
            {
                program = programDAO.GetProgram(programId);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return program;
        }

        public VendorAPIList GetPrograms(DirectoryRequest programRequest)
        {
            VendorAPIList vendorAPIList = null;
            try
            {
                vendorAPIList = matchingServiceDAO.GetPrograms(programRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorAPIList;
        }

        public Entities.Program GetProgramDetails(DirectoryRequest programDetailsRequest)
        {
            Entities.Program program = null;
            try
            {
                program = matchingServiceDAO.GetProgramDetails(programDetailsRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return program;
        }

        public Entities.FormTemplate GetProgramForm(ContactRequest programFormRequest)
        {
            Entities.FormTemplate form = null;
            try
            {
                form = formsServiceDAO.GetFormTemplate(programFormRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return form;
        }

        //public Entities.ProgramFormValidationRules GetProgramFormValidationRules(ContactRequest programFormRequest)
        //{
        //    Entities.ProgramFormValidationRules programFormValidationRules = null;
        //    try
        //    {
        //        programFormValidationRules = formsServiceDAO.GetProgramFormValidationRules(programFormRequest);
        //    }
        //    catch (Exception exc)
        //    {
        //        ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
        //    }
        //    return programFormValidationRules;
        //}

        public VendorResponseBase GetProgramMatches(ContactRequest contactRequest)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                vendorResponseBase = formsServiceDAO.GetProgramMatches(contactRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorResponseBase;
        }

        public AllocationStatus CheckAllocation(DirectoryRequest allocationRequest)
        {
            AllocationStatus allocationStatus = null;
            try
            {
                allocationStatus = matchingServiceDAO.CheckAllocation(allocationRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return allocationStatus;
        }

        public VendorAPIList GetProgramsForMicrosite(DirectoryRequest programRequest, out List<ProgramMicrosite> programList, bool programListRequired = false)
        {
            VendorAPIList vendorAPIList = null;
            programList = null;
            try
            {          
                vendorAPIList = matchingServiceDAO.GetProgramsForMicrosites(programRequest, out programList, programListRequired);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorAPIList;
        }


    }
}
