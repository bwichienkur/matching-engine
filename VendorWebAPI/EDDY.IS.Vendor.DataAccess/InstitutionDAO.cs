using EDDY.IS.Common.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.DataAccess
{
    public class InstitutionDAO : VendorBaseDAO
    {
        public Entities.Institution GetInstitution(int institutionId)
        {
            Entities.Institution institution = null;
            try
            {
                institution = this.getAllEnabledInstitutions().Where(i => i.InstitutionId == institutionId).FirstOrDefault();
            }
            catch (Exception ex)
            {

                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            return institution;
        }
    }
}
