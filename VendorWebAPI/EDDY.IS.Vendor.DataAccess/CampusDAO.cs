using EDDY.IS.Common.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.DataAccess
{
    public class CampusDAO : VendorBaseDAO
    {
        public Entities.Campus GetCampus(int campusId)
        {
            Entities.Campus campus = null;
            try
            {
                campus = this.getAllEnabledCampuses().Where(c => c.CampusId == campusId).FirstOrDefault();
            }
            catch(Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            return campus;
        }
    }
}
