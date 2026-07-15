using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.FormsEngine.PixelsService;
using System.Threading;
using EDDY.IS.Core.Logging;
using System.Configuration;
using EDDY.IS.FormsEngine.DTO;


namespace EDDY.IS.FormsEngine
{
    public partial class FormsRelatedServices
    {
        private static EDDY.IS.FormsEngine.PixelsService.ServiceClient PixelsService = new ServiceClient();
        const long DELAYED_EXECUTION = 30000;

        #region Pixel Service

        /// <summary>
        /// Pixel service call
        /// </summary>
        /// <param name="TrackID"></param>
        /// <param name="Types"></param>
        /// <param name="ProspectID"></param>
        /// <param name="LeadIDs"></param>
        /// <returns></returns>
        public string GetPixelsData(string TrackID, string[] Types, int ProspectID, int[] LeadIDs)    //(string UniqueLeadId, bool IsCrossSell)
        {
            string result = "";

            try 
            {
                result = PixelsService.GetPixelsWithMultipleTypes(TrackID, Types, ProspectID, LeadIDs);
            }
            catch (Exception ex)
            {
                new ISException(Base.ISApplication.FormsEngine, ex).Save();
            }

            return result;

        }

        #endregion Pixel Service

    }
}
