using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess.MatchingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Vendor.DataAccess
{
    public class ProgramDAO : VendorBaseDAO
    {
        private readonly MatchingServiceClient matchingServiceClient = new MatchingServiceClient();

        public Entities.Program GetProgram(int programId)
        {
            Entities.Program program = null;
            try
            {
                program = this.getAllEnabledPrograms().Where(p => p.ProgramId == programId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            return program;
        }
    }
}
