using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.DataAccess
{
    public class ProgramLevelDAO : VendorBaseDAO
    {
        public Level GetProgramLevel(int programLevelId)
        {
            Level programLevel = null;
            try
            {
                programLevel = this.getAllProgramLevels().Where(p => p.ProgramLevelId == programLevelId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            return programLevel;
        }
    }
}
