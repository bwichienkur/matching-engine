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
using System.Diagnostics;
using EDDY.IS.FormsEngine.Caching;


namespace EDDY.IS.FormsEngine
{
    public partial class FormsRelatedServices
    {
        private static MatchingServiceClient MatchingServiceProd = new MatchingServiceClient("BasicHttpBinding_IMatchingService");
        private static MatchingServiceClient MatchingServiceBeta = new MatchingServiceClient("BasicHttpBinding_IMatchingService_Beta");


        #region MatchingEngine Service
        /// <summary>
        /// Calls Matching Engine service to get programs for the ddl
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public FormProgramResponse GetFormPrograms(DirectoryMatchRequest Request, bool IsBeta)
        {
            FormProgramResponse Response = new FormProgramResponse();

            try
            {
                if (!IsBeta)
                {
                    Response = MatchingServiceProd.GetFormPrograms(Request);
                }
                else
                {
                    Response = MatchingServiceBeta.GetFormPrograms(Request);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Response;
        }

        public CategoryResponse GetCategories(DirectoryMatchRequest Request, bool IsBeta)
        {
            CategoryResponse Response = new CategoryResponse();

            try
            {
                if (!IsBeta)
                {
                    Response = MatchingServiceProd.GetCategoriesAllIfNone(Request);
                }
                else
                {
                    Response = MatchingServiceBeta.GetCategoriesAllIfNone(Request);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Response;
        }

        public ProgramLevelResponse GetProgramLevels(DirectoryMatchRequest Request, bool IsBeta)
        {
            ProgramLevelResponse Response = new ProgramLevelResponse();

            try
            {
                if (!IsBeta)
                {
                    Response = MatchingServiceProd.GetProgramLevelsAllIfNone(Request);
                }
                else
                {
                    Response = MatchingServiceBeta.GetProgramLevelsAllIfNone(Request);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Response;
        }

        public SubjectResponse GetSubCategories(DirectoryMatchRequest Request, bool IsBeta)
        {
            SubjectResponse Response = new SubjectResponse();

            try
            {
                if (!IsBeta)
                {
                    Response = MatchingServiceProd.GetSubjectsAllIfNone(Request);
                }
                else
                {
                    Response = MatchingServiceBeta.GetSubjectsAllIfNone(Request);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Response;
        }

        public SpecialtyResponse GetSpecialties(DirectoryMatchRequest Request, bool IsBeta)
        {
            SpecialtyResponse Response = new SpecialtyResponse();

            try
            {
                if (!IsBeta)
                {
                    Response = MatchingServiceProd.GetSpecialtiesAllIfNone(Request);
                }
                else
                {
                    Response = MatchingServiceBeta.GetSpecialtiesAllIfNone(Request);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Response;
        }

        /// <summary>
        /// Gets the list of matched template ids based on inputs
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public TemplateMatchResponse GetTemplatesForMatches(DirectoryMatchRequest Request, bool IsBeta)
        {
            TemplateMatchResponse Response = new TemplateMatchResponse();

            try
            {
                if (!IsBeta)
                {
                    Response = MatchingServiceProd.GetTemplatesForMatches(Request);
                }
                else
                {
                    Response = MatchingServiceBeta.GetTemplatesForMatches(Request);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Response;
        }

        public WizardMatchResponse GetWizardMatches(WizardMatchRequest Request, bool IsBeta)
        {
            WizardMatchResponse Response = new WizardMatchResponse();

            try
            {
                if (!IsBeta)
                {
                    Response = MatchingServiceProd.GetWizardMatches(Request);
                }
                else
                {
                    Response = MatchingServiceBeta.GetWizardMatches(Request);
                }

            }
            catch (Exception)
            {
                throw;
            }

            return Response;
        }

        /// <summary>
        /// Dynamic Campus soft preference control
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="IsBeta"></param>
        /// <returns></returns>
        public CampusTypeMatchResponse GetCampusTypes(DirectoryMatchRequest Request, bool IsBeta)
        {
            CampusTypeMatchResponse Response = new CampusTypeMatchResponse();

            try
            {
                if (!IsBeta)
                {
                    Response = MatchingServiceProd.GetCampusTypes(Request);
                }
                else
                {
                    Response = MatchingServiceBeta.GetCampusTypes(Request);
                }

            }
            catch (Exception)
            {
                throw;
            }

            return Response;
        }

        public ProgramDetailResponse GetProgramDetails(int ProgramId, bool IsBeta, System.Guid TrackId)
        {
            ProgramDetailResponse Result = new ProgramDetailResponse();

            try
            {
                if (!IsBeta)
                {
                    Result = MatchingServiceProd.GetProgramDetails(0, ProgramId, TrackId, null, null);
                }
                else
                {
                    Result = MatchingServiceBeta.GetProgramDetails(0, ProgramId, TrackId, null, null);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Result;
        }

        public ProgramResponse GetPrograms(int ProgramId, bool IsBeta, System.Guid TrackId)
        {
            ProgramResponse Result = new ProgramResponse();

            try
            {
                if (!IsBeta)
                {
                    Result = MatchingServiceProd.GetPrograms(new DirectoryMatchRequest() { SortMethod = EntitySortMethod.RankScore, TrackGuid = TrackId, ProgramIdList = new int[]{ ProgramId } }, true);
                }
                else
                {
                    Result = MatchingServiceBeta.GetPrograms(new DirectoryMatchRequest() { SortMethod = EntitySortMethod.RankScore, TrackGuid = TrackId, ProgramIdList = new int[] { ProgramId } }, true);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Result;
        }

        public int GetProgramsForCounter(bool InitialCall, DirectoryMatchRequest Request, bool IsBeta)
        {
            Stopwatch stopWatch = new Stopwatch();
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.GetProgramsForCounter", null, Request, IsBeta);
            Log.StartLogDetail("FormsEngine.GetProgramsForCounter");
            MatchingEngine.ProgramResponse ProgramResponse = null;
            int Result = 0;



            string Key = string.Format(Constants.PROGRAMCOUNTER_CACHE_KEY, Request.TrackGuid.ToString());

            //cached results
            if (InitialCall && FormsEngineCacheProxy.Cache.Get(Key) != null)
            {
                Result = FormsEngineCacheProxy.Cache.Get<int>(Key);
            }
            else
            {
                stopWatch.Start();
                if (!IsBeta)
                {
                    ProgramResponse = MatchingServiceProd.GetPrograms(Request, false);
                }
                else
                {
                    ProgramResponse = MatchingServiceBeta.GetPrograms(Request, false);
                }
                stopWatch.Stop();
                if (ProgramResponse != null)
                {
                    Result = ProgramResponse.ResultCount;
                }
            }

            if (InitialCall && ProgramResponse !=null)
            {
                int Expiration;
                if (!Int32.TryParse(ConfigurationManager.AppSettings.Get("InitialProgramCounterCacheTimeMinutes"), out Expiration)) 
                {
                    Expiration = 20;
                }
                FormsEngineCacheProxy.Cache.Set(Key, ProgramResponse.ResultCount, Expiration);
            }

            if (stopWatch.ElapsedMilliseconds > DELAYED_EXECUTION)
            {
                new ISException(new Exception(string.Format("FormsEngine.GetProgramsForCounter.Matching Engine Delayed execution exception {0} ms.", stopWatch.ElapsedMilliseconds)), Request, IsBeta).Save();
            }



            Log.EndLogDetail();
            Log.EndLog(Result);
            return Result;
        }

        public ProgramValidateResponse ValidateProgram(ProgramValidateRequest Request, bool IsBeta)
        {
            ProgramValidateResponse Result = new ProgramValidateResponse();
            try
            {
                if (IsBeta == false)
                {
                    Result = MatchingServiceProd.ValidateProgram(Request);
                }
                else
                {
                    Result = MatchingServiceBeta.ValidateProgram(Request);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }

            return Result;
        }

        /// <summary>
        /// Check Country rules per programproductid
        /// </summary>
        /// <param name="ProgramProductId"></param>
        /// <param name="CountryId"></param>
        /// <param name="IsBeta"></param>
        /// <returns></returns>
        public bool CountryCheck(int ProgramProductId, int CountryId, bool IsBeta)
        {
            bool Result = true;
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "FormsEngine.CountryCheck", null, ProgramProductId, CountryId, IsBeta);
            Log.StartLogDetail("FormsEngine.CountryCheck");
            try
            {
                CountryValidateRequest Request = new CountryValidateRequest();
                Request.Application = MatchingEngine.ISApplication.FormsEngine;
                Request.ProgramProductId = ProgramProductId;
                Request.CountryId = CountryId;
                if (!IsBeta)
                {
                    Result = MatchingServiceProd.ValidateCountryAndProgram(Request).PassedValidation;
                }
                else
                {
                    Result = MatchingServiceBeta.ValidateCountryAndProgram(Request).PassedValidation;
                }
            }
            catch (Exception)
            {
                throw;
            }

            Log.EndLogDetail();
            Log.EndLog(Result);
            return Result;
        }


        public CampaignDetailDTO GetCampaignDetailByTrackId(Guid TrackID)
        {
            CampaignDetailDTO Result = new CampaignDetailDTO();
            //Set defaults
            Result.AdditionalQuestionsOnlyInSchoolSelection = false;
            Result.AdditionalQuestionsFromSmartMatch = false;
            Result.MaxSubmissionCount = 1;
            Result.MaxSmartMatchCount = 3;
            Result.IsCallCenter = false;
            Result.UseInternationalTemplate = false;

            Stopwatch stopWatch = new Stopwatch();
            PerformanceLog Log = new PerformanceLog(Base.ISApplication.FormsEngine, "GetCampaignDetailByTrackId", null, null, null, TrackID);
            Log.StartLog(Base.ISApplication.FormsEngine, "GetCampaignDetailByTrackId", null, TrackID);
            Log.StartLogDetail("FormsEngine.MatchingServiceProd.GetCampaignDetailByTrackId");
            stopWatch.Start();
            CampaignDetailResponse CampaignResult = MatchingServiceProd.GetCampaignDetailByTrackID(TrackID);
            stopWatch.Stop();
            Log.EndLogDetail();
            Log.EndLog(CampaignResult);

            if (stopWatch.ElapsedMilliseconds > DELAYED_EXECUTION)
            {
                new ISException(new Exception(string.Format("FormsEngine.GetCampaignDetailByTrackID.Matching Engine Delayed execution exception {0} ms.", stopWatch.ElapsedMilliseconds)), TrackID).Save();
            }

            if (CampaignResult != null)
            {
                Result.MaxSubmissionCount = CampaignResult.MaxSubmissionCount ?? 1;
                Result.MaxSmartMatchCount = CampaignResult.MaxSmartMatchCount.HasValue ? CampaignResult.MaxSmartMatchCount.Value : 3;
                Result.IsCallCenter = CampaignResult.IsCallCenter;
                Result.CampaignTCPAMessageName = CampaignResult.CampaignTCPAMessageName;
                Result.ProgramWizardAdditionalQuestionsFlowType = Convert.ToInt32(CampaignResult.ProgramWizardAdditionalQuestionsFlowType);
                Result.AllowRemonetization = CampaignResult.AllowRemonetization;
                Result.UseInternationalTemplate = CampaignResult.UseInternationalTemplate;
                Result.ChannelId = CampaignResult.ChannelId;
                Result.AllowExitPops = CampaignResult.AllowExitPops;
                Result.OpenMailProfileId = CampaignResult.OpenMailProfileId;
                //TODO: Update when ME chance is in place
                Result.HasXVerify = CampaignResult.HasXVerify;
                switch (CampaignResult.AdditionalQuestionsFlowType)
                {
                    // Additional questions on the Wizard and not only from smart matches (default)
                    case MatchingEngine.AdditionalQuestionsFlowType.WizardAnyMatch:
                        Result.AdditionalQuestionsOnlyInSchoolSelection = false;
                        Result.AdditionalQuestionsFromSmartMatch = false;
                        break;
                    //Additional questions only in School Selection
                    case MatchingEngine.AdditionalQuestionsFlowType.SchoolSelectionOnly:
                        Result.AdditionalQuestionsOnlyInSchoolSelection = true;
                        Result.AdditionalQuestionsFromSmartMatch = false;
                        break;
                    //Additional questions on the wizard and from smart matches
                    case MatchingEngine.AdditionalQuestionsFlowType.WizardSmartMatchOnly:
                        Result.AdditionalQuestionsOnlyInSchoolSelection = false;
                        Result.AdditionalQuestionsFromSmartMatch = true;
                        break;
                }
            }

            return Result;
        }


        public CrossSellProgramResponse GetProgramsForCrossSell(CrossSellMatchRequest Request, bool IsBeta) 
        {
            CrossSellProgramResponse Result = null;

            if (!IsBeta)
            {
                Result = MatchingServiceProd.GetProgramsForCrossSell(Request);
            }
            else
            {
                Result = MatchingServiceBeta.GetProgramsForCrossSell(Request);
            }

            return Result;
        }

        public ProgramValidateResponse ValidateAPIProgram(ProgramValidateRequest Request, bool IsBeta)
        {
            ProgramValidateResponse Result = new ProgramValidateResponse();
            try
            {
                if (IsBeta == false)
                {
                    Result = MatchingServiceProd.ValidateAPIProgram(Request);
                }
                else
                {
                    Result = MatchingServiceBeta.ValidateAPIProgram(Request);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Result;
        }


        public NavigationResponse GetFacetedNavigation(DirectoryMatchRequest Request, bool IsBeta)
        {
            NavigationResponse Result = null;

            if (!IsBeta)
            {
                Result = MatchingServiceProd.GetFacetedNavigation(Request);
            }
            else
            {
                Result = MatchingServiceBeta.GetFacetedNavigation(Request);
            }

            return Result;
        }

        public CampusResponse GetCampuses(DirectoryMatchRequest Request, bool IsBeta)
        {
            CampusResponse Result = new CampusResponse();

            if (!IsBeta)
            {
                Result = MatchingServiceProd.GetCampuses(Request);
            }
            else
            {
                Result = MatchingServiceBeta.GetCampuses(Request);
            }

            return Result;
        }

        public InstitutionResponse GetInstitutions(DirectoryMatchRequest Request, bool IsBeta)
        {
            InstitutionResponse Result = new InstitutionResponse();

            if (!IsBeta)
            {
                Result = MatchingServiceProd.GetInstitutions(Request, GetInstitutionCampusOption.NoCampus);
            }
            else
            {
                Result = MatchingServiceBeta.GetInstitutions(Request, GetInstitutionCampusOption.NoCampus);
            }

            return Result;
        }

        #endregion MatchingEngine Service
    }
}
