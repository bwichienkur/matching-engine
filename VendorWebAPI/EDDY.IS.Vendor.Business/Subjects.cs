using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess;
using EDDY.IS.Vendor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.Business
{
    public class Subjects : VendorBase
    {
        private SubjectDAO subjectDAO = new SubjectDAO();

        public virtual Subject GetSubject(int subjectId)
        {
            Subject subject = null;
            try
            {
                subject = subjectDAO.GetSubject(subjectId);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return subject;
        }
    }
}
