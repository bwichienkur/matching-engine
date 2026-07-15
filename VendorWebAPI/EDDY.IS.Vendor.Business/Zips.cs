using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Vendor.DataAccess;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.Entities;

namespace EDDY.IS.Vendor.Business
{
    public class Zips : VendorBase
    {
        private ZipDAO zipDAO = new ZipDAO();

        public virtual Entities.ZipResponse GetZipCodes(DirectoryRequest zipRequest)
        {
            Entities.ZipResponse zip = null;
            try
            {
                zip = zipDAO.GetZipCodes(zipRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return zip;
        }
    }
}
