using EDDY.IS.Common.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.DataAccess
{
    public class SubjectDAO : VendorBaseDAO
    {
        public Entities.Subject GetSubject(int subjectId)
        {
            Entities.Subject subcategory = null;
            try
            {
                subcategory = this.getAllSubjects().Where(s => s.SubjectId == subjectId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            return subcategory;
        }
    }
}
