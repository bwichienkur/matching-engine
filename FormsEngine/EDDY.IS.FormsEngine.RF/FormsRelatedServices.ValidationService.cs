using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.ValidationService;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.FormsEngine.PixelsService;
using System.Threading;
using EDDY.IS.Core.Logging;
using System.Configuration;
using EDDY.IS.FormsEngine.DTO;
using System.Diagnostics;
using EDDY.IS.FormsEngine.Caching;

namespace EDDY.IS.FormsEngine
{
    public partial class FormsRelatedServices
    {
        private static ValidationServiceClient ValidationServiceProd = new ValidationServiceClient("BasicHttpBinding_IValidationService");

        #region Validation Service
        
        public string GetCountryCodeByIP(string ip)
        {
            string Result = "";
            try
            {
                Result = ValidationServiceProd.GetCountryCodeByIP(ip);
            }
            catch 
            {
                throw;
            }

            return Result;
        }

        #endregion MatchingEngine Service
    }
}
