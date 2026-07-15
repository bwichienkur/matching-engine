using EDDY.IS.Common.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.DataAccess
{
    public class MilitaryStatusDAO : VendorBaseDAO
    {
        public Entities.MilitaryStatus GetMilitaryStatus(int militaryStatusId)
        {
            Entities.MilitaryStatus militaryStatus = null;
            try
            {
                militaryStatus = this.getAllEnabledMilitaryStatuses().Where(s => s.MilitaryStatusId == militaryStatusId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            return militaryStatus;
        }
    }
}
