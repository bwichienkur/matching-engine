using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.Entities;


namespace EDDY.IS.Vendor.DataAccess
{
    public class ZipDAO : VendorBaseDAO
    {
        public Entities.ZipResponse GetZipCodes(DirectoryRequest zipRequest)
        {
            Entities.ZipResponse zip = null;
            try
            {
                zip = this.getZipCodes(zipRequest);
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            return zip;
        }
    }
}