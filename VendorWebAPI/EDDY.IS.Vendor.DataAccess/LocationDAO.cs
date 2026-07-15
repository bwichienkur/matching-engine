using System;
using System.Collections.Generic;
using System.Linq;
using EDDY.IS.Common.ExceptionHandler;

namespace EDDY.IS.Vendor.DataAccess
{
    public class LocationsDAO : VendorBaseDAO
    {
        public List<Entities.PostalCode> GetAllUSPostalCodes()
        {
            List<Entities.PostalCode> postalCodeList = null;
            try
            {
                postalCodeList = this.getAllUSPostalCodes();
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return postalCodeList;
        }

        public List<Entities.PostalCode> GetAllCanadaPostalCodes()
        {
            List<Entities.PostalCode> postalCodeList = null;
            try
            {
                postalCodeList = this.getAllCanadianPostalCodes();
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return postalCodeList;
        }

        public List<Entities.State> GetAllStates()
        {
            List<Entities.State> stateList = null;
            try
            {
                stateList = this.getAllStates().ToList();
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return stateList;
        }
        public List<Entities.Country> GetAllCountries()
        {
            List<Entities.Country> countrList = null;
            try
            {
                countrList = this.getAllCountries().ToList();
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return countrList;
        }
    }
}
