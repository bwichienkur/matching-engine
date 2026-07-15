using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Business
{
    public class MilitaryStatuses: VendorBase
    {
        private MilitaryStatusDAO militaryStatusDAO = new MilitaryStatusDAO();

        public virtual Entities.MilitaryStatus GetMilitaryStatus(int militaryStatusId)
        {
            Entities.MilitaryStatus militaryStatus = null;
            try
            {
                militaryStatus = militaryStatusDAO.GetMilitaryStatus(militaryStatusId);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return militaryStatus;
        }
    }
}
