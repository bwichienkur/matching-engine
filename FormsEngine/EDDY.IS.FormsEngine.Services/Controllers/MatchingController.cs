using System;
using System.Web.Mvc;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.Services.Controllers.Base;
using EDDY.IS.Util.HTMLExtensions;
using EDDY.IS.FormsEngine.DTO;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    /// <summary>
    /// AN Note: To be depracated after ProgramTemplates are removed.
    /// Do not add other functionality.
    /// </summary>
    public class MatchingController : MatchingControllerBase
    {
        [HttpGet]
        public ActionResult GetFormPrograms(int InstitutionId, int? ProgramId, bool IsBeta, string TrackId, string FormFilterValues, string DeviceId, int? ApplicationId, string AlternativeTemplates)
        {
            string Result = null;
            try
            {
                Result = GetProgramTemplateProgramsDDL(InstitutionId, ProgramId, IsBeta, TrackId, FormFilterValues, ApplicationId, DeviceId, AlternativeTemplates, false, ProgramOptGroup.Default);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetCategories(bool IsBeta, string TrackId, string FormFilterValues, int? ApplicationId)
        {
            CategoryResponse Result = null;
            try
            {
                Result = FormsEngineService.RelatedServices.GetCategories(BuildDirectoryMatchRequest(IsBeta, TrackId, FormFilterValues, true, EntitySortMethod.Alphabetical, null,ApplicationId, null), IsBeta);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetProgramLevels(bool IsBeta, string TrackId, string FormFilterValues, int? ApplicationId)
        {
            ProgramLevelResponse Result = null;
            try
            {
                Result = FormsEngineService.RelatedServices.GetProgramLevels(BuildDirectoryMatchRequest(IsBeta, TrackId, FormFilterValues, true, EntitySortMethod.Alphabetical, null, ApplicationId, null), IsBeta);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetSubCategories(bool IsBeta, string TrackId, string FormFilterValues, int? ApplicationId)
        {
            SubjectResponse Result = null;
            try
            {
                Result = FormsEngineService.RelatedServices.GetSubCategories(BuildDirectoryMatchRequest(IsBeta, TrackId, FormFilterValues, true, EntitySortMethod.Alphabetical, null, ApplicationId, null), IsBeta);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

     
        [HttpGet]
        public ActionResult CountryCheck(int ProgramProductId, int CountryId, string CountryCode, bool IsBeta)
        {
            bool Result = true;
            try
            {
                Result = FormsEngineService.RelatedServices.CountryCheck(ProgramProductId, CountryId, IsBeta);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult GetProgramDetail(int ProgramId, bool IsBeta, System.Guid TrackId)
        {
            ProgramDetailResponse Result = null;
            try
            {
                Result = FormsEngineService.RelatedServices.GetProgramDetails(ProgramId, IsBeta, TrackId);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetProgram(int ProgramId, bool IsBeta, System.Guid TrackId)
        {
            ProgramResponse Result = null;
            try
            {
                Result = FormsEngineService.RelatedServices.GetPrograms(ProgramId, IsBeta, TrackId);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetProgramsForCounter(bool IsBeta, string TrackId, string LeadData, bool InitialCall, int? ApplicationId) 
        {
            int Result = 0;
            try
            {
                Result = FormsEngineService.RelatedServices.GetProgramsForCounter(InitialCall,BuildDirectoryMatchRequestForCounter(IsBeta, TrackId, LeadData, true, EntitySortMethod.Alphabetical, 1, 1, ApplicationId, null), IsBeta);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetCampaignDetailByTrackId(string TrackId)
        {
            CampaignDetailDTO Result = new CampaignDetailDTO();
            Result.AdditionalQuestionsOnlyInSchoolSelection = false;
            Result.MaxSmartMatchCount = 3;
            try
            {
                Guid TrackGuid;
                if (Guid.TryParse(TrackId, out TrackGuid))
                {
                    Result = FormsEngineService.RelatedServices.GetCampaignDetailByTrackId(TrackGuid);
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return new JsonpResult(Result);
        }

    } 
}
