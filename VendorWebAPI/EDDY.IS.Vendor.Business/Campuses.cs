using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess;
using System.Collections.Generic;

namespace EDDY.IS.Vendor.Business
{
    public class Campuses : VendorBase
    {
        private CampusDAO campusDAO = new CampusDAO();

        public virtual Entities.Campus GetCampus(int campusId)
        {
            Entities.Campus campus = null;
            try
            {
                campus = campusDAO.GetCampus(campusId);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return campus;
        }

        public VendorAPIList GetCampuses(DirectoryRequest programRequest)
        {
            VendorAPIList vendorAPIList = null;
            try
            {
                vendorAPIList = matchingServiceDAO.GetCampuses(programRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorAPIList;
        }

        public VendorAPIList GetCampusesForMicrosite(DirectoryRequest campusRequest, out List<CampusMicrosite> campusList, bool campusListRequired = false)
        {
            VendorAPIList vendorAPIList = null;
            campusList = null;
            try
            {
                vendorAPIList = matchingServiceDAO.GetCampusesForMicrosites(campusRequest, out campusList, campusListRequired);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return vendorAPIList;
        }




            public void DeleteProgramsCacheForMicrosite(DirectoryRequest campusRequest)
            {
                try
                {
                    matchingServiceDAO.DeleteProgramsCacheForMicrosites(campusRequest);
                }
                catch (Exception exc)
                {
                    ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
                }

            }

            public void DeleteCampusesCacheForMicrosite(DirectoryRequest campusRequest)
            {
                try
                {
                    matchingServiceDAO.DeleteCampusesCacheForMicrosites(campusRequest);
                }
                catch (Exception exc)
                {
                    ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
                }

            }
    }
}
