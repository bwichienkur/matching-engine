using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess;

namespace EDDY.IS.Vendor.Business
{
    public class Institutions : VendorBase
    {
        private InstitutionDAO institutionDAO = new InstitutionDAO();

        public virtual Institution GetInstitution(int institutionId)
        {
            Institution institution = null;
            try
            {
                institution = institutionDAO.GetInstitution(institutionId);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return institution;
        }

        public VendorAPIList GetInstitutions(DirectoryRequest programRequest)
        {
            VendorAPIList vendorAPIList = null;
            try
            {
                vendorAPIList = matchingServiceDAO.GetInstitutions(programRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorAPIList;
        }
    }
}
