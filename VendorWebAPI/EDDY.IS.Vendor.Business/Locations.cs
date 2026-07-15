using System;
using System.Collections.Generic;
using EDDY.IS.Vendor.DataAccess;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.Utilities;
using System.Linq;

namespace EDDY.IS.Vendor.Business
{
    public class Locations
    {
        private LocationsDAO locationsDAO = new LocationsDAO();
        public List<Entities.PostalCode> GetAllUSPostalCodes()
        {
            List<Entities.PostalCode> postalCodeList = null;
            try
            {
                postalCodeList = locationsDAO.GetAllUSPostalCodes();
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return postalCodeList;
        }

        public List<Entities.PostalCode> GetAllCanadaPostalCodes()
        {
            List<Entities.PostalCode> postalCodeList = null;
            try
            {
                postalCodeList = locationsDAO.GetAllCanadaPostalCodes();
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return postalCodeList;
        }

        public List<Entities.State> GetAllStates()
        {
            List<Entities.State> stateList = null;
            try
            {
                stateList = locationsDAO.GetAllStates();
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return stateList;
        }

        public List<Entities.Country> GetAllCountries()
        {
            List<Entities.Country> countrList = null;
            try
            {
                countrList = locationsDAO.GetAllCountries();
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return countrList;
        }

        public bool PostalCodeExistsWithinState(string postalCode, string stateCode, NorthAmericanCountry country)
        {
            var postalCodeExists = false;
            var uppercaseStateCode = stateCode.ToUpper();

            if (country == NorthAmericanCountry.UnitedStates)
            {
                postalCodeExists = locationsDAO.GetAllUSPostalCodes().Where(pc => pc.PostalCodeString == postalCode && pc.StateCode == uppercaseStateCode).FirstOrDefault() != null;
            }
            else if (country == NorthAmericanCountry.Canada)
            {
                var strippedPostalCode = postalCode.Replace(" ", ""); //strip out spaces from postal code because candian postal codes may contain spaces
                postalCodeExists = locationsDAO.GetAllCanadaPostalCodes().Where(pc => pc.PostalCodeString == strippedPostalCode && pc.StateCode == uppercaseStateCode).FirstOrDefault() != null;
            }

            return postalCodeExists;
        }
    
    }
}
