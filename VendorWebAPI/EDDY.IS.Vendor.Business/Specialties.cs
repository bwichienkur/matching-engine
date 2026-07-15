using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess;
using EDDY.IS.Vendor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Business
{
    public class Specialties : VendorBase
    {
        private SubjectDAO subjectDAO = new SubjectDAO();

        public VendorAPIList GetSpecialties(DirectoryRequest specialtyRequest)
        {
            VendorAPIList vendorAPIList = null;
            try
            {
                vendorAPIList = matchingServiceDAO.GetSpecialties(specialtyRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorAPIList;
        }
    }
}
