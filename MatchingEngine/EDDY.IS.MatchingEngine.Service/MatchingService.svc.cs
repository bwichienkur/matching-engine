using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using EDDY.IS.Core.Logging;
using EDDY.IS.MatchingEngine.DTO;
using Newtonsoft.Json;
using EDDY.IS.Base;
using EDDY.IS.MatchingEngine.Constants;
using System.Configuration;

namespace EDDY.IS.MatchingEngine.Service
{
    public class MatchingService : IMatchingService
    {
        private void ApplyReservationRestrictions(List<MatchItem> matches, Campaign c, int tierLevel)
        {
            Dictionary<int, LeadScoreReservation> reservations = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, LeadScoreReservation>>(MatchingCacheItem.LeadScoreReservationConfigurations);

            foreach (var r in reservations.Values)
                r.ProcessReservationRules(c, matches, tierLevel);
        }

        public string GetProgramDescription(int programId)
        {
            List<ProgramContent> pc = StaticCacheProxyHost.CacheProxy.Get<List<ProgramContent>>(MatchingCacheItem.ProgramContent);

            var p = (from l in pc
                     where l.ProgramId == programId
                     select l.ProgramDescription).FirstOrDefault();

            return p;
        }

        private bool EligibleForWarmTransferTitanium(List<ProgramWithInstitutionCampus> programs)
        {
            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);
            //bool existsRemonentizationRestriction = false;

            foreach(var p in programs)
            {
                if (p.ProductId == (int)EDDY.IS.MatchingEngine.Constants.ProductType.Match1Exclusive || p.ProductId == (int)EDDY.IS.MatchingEngine.Constants.ProductType.Match1Plus)
                    return false;

                //if (p.RemonetizationRestriction)
                //    existsRemonentizationRestriction = true;
            }

            return true;
        }

        private ProgramWithInstitutionCampus GetWarmTransferTitaniumMatch(List<ProgramWithInstitutionCampus> smPrograms, WizardMatchRequest request, PerformanceLog pLog)
        {
            ProgramWithInstitutionCampus match = null;

            HashSet<int> smatchMatchInstitutions = new HashSet<int>();
            smatchMatchInstitutions.UnionWith(smPrograms.Select(sm => sm.InstitutionId));

            MatchingEngine me = new MatchingEngine(pLog);

            pLog.StartLogDetail("MatchingEngine.GetWarmTransferTitaniumMatches");
            MatchResult matches = me.GetWarmTransferTitaniumMatches(request);
            pLog.EndLogDetail();

            pLog.StartLogDetail("MatchAggregator.CreateWarmTransferTitaniumMatch");
            match = MatchAggregator.CreateWarmTransferTitaniumMatch(matches, request, smatchMatchInstitutions);
            pLog.EndLogDetail();
            return match;
        }

        public WizardMatchResponse GetWizardMatches(WizardMatchRequest wizardMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog pLog = new Core.Logging.PerformanceLog();
            pLog.StartLog(Base.ISApplication.MatchingEngine, "GetWizardMatches", wizardMatchRequest.Application, wizardMatchRequest);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            WizardMatchResponse wizardResponse = new WizardMatchResponse();

            bool limboAlternativeTrackIdUtilized = false;
            Guid? limboAlternativeTrackId = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(pLog);

                MatchResult results = engine.GetWizardMatches(wizardMatchRequest);

                limboAlternativeTrackId = results.ChosenCampaign.LimboAlternativeTrackId;

                if (limboAlternativeTrackId.HasValue && results.MatchItemList.Count == 0)
                {
                    limboAlternativeTrackIdUtilized = true;
                    wizardMatchRequest.TrackGuid = limboAlternativeTrackId.Value;
                    results = engine.GetWizardMatches(wizardMatchRequest);
                }

                if(wizardMatchRequest.LeadScoringInput != null && wizardMatchRequest.LeadScoringInput.LeadScoringTierLevel.HasValue)
                    ApplyReservationRestrictions(results.MatchItemList, results.ChosenCampaign, wizardMatchRequest.LeadScoringInput.LeadScoringTierLevel.Value);

                wizardResponse = MatchAggregator.CreateWizardResponse(results, wizardMatchRequest, pLog);
               
                if(wizardResponse.SmartMatchList != null && wizardResponse.SmartMatchList.Count > 0 && wizardResponse.SmartMatchList.Count < results.ChosenCampaign.MaxSmartMatchCount)
                {
                    if(EligibleForWarmTransferTitanium(wizardResponse.SmartMatchList))
                    {
                        ProgramWithInstitutionCampus match = GetWarmTransferTitaniumMatch(wizardResponse.SmartMatchList, wizardMatchRequest, pLog);

                        if(match != null)
                            wizardResponse.SmartMatchList.Add(match);
                    }
                }
                wizardResponse.LimboAlternativeTrackIdUtilized = limboAlternativeTrackIdUtilized;
                wizardResponse.LimboAlternativeTrackId = limboAlternativeTrackId;
                wizardResponse.MaxUserSelections = results.ChosenCampaign.MaxSubmissionCount;

                Guid? leadScoringGuid = null;

                //if (wizardMatchRequest.LeadScoringInput != null)
                //{
                //    BaseRuleType? ruleType;

                //    leadScoringGuid = wizardMatchRequest.LeadScoringInput.LeadScoringGuid;

                //    if ((wizardResponse.SmartMatchList == null || wizardResponse.SmartMatchList.Count == 0) && (wizardResponse.SchoolSelectionList == null || wizardResponse.SchoolSelectionList.Count == 0))
                //        if (!results.ChosenCampaign.PassesLeadScoring(wizardMatchRequest.LeadScoringInput.LeadScoringTierLevel, out ruleType))
                //            wizardResponse.WizardLimboReason = LimboReason.LeadScoringMinimumTierLevel;
                //}
                sw.Stop();

                Task.Run(() => MatchPersister.PersistMatchRequest(
                                                    wizardMatchRequest, wizardResponse, results,
                                                    ResponseType.Wizard, "GetWizardMatches", sw.ElapsedMilliseconds, leadScoringGuid));
            }
            catch (Exception ex)
            {
                try
                {
                    wizardResponse.MatchResponseGuid = Guid.NewGuid();
                    ISException isEx = new ISException(ex, wizardMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            pLog.EndLog(wizardResponse);

            return wizardResponse;
        }

        public TemplateMatchResponse GetTemplatesForMatches(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetTemplatesForMatches", directoryMatchRequest.Application, directoryMatchRequest);

            TemplateMatchResponse templateResponse = new TemplateMatchResponse();

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                MatchResult results = null;

                if (directoryMatchRequest.LeadCreationType.HasValue)
                {
                    results = engine.GetWizardMatches(new WizardMatchRequest(directoryMatchRequest));
                }
                else
                {
                    directoryMatchRequest.RemoveInvalidEntities = true;
                    results = engine.GetDirectoryMatches(directoryMatchRequest);
                }

                templateResponse = MatchAggregator.CreateTemplateMatchResponse(results);
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(templateResponse);

            return templateResponse;
        }

        public CampusTypeMatchResponse GetCampusTypes(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetCampusTypes", directoryMatchRequest.Application, directoryMatchRequest);

            CampusTypeMatchResponse campusTypeResponse = new CampusTypeMatchResponse();

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                directoryMatchRequest.RemoveInvalidEntities = true;

                MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                campusTypeResponse = MatchAggregator.CreateCampusTypeMatchResponse(results, directoryMatchRequest);

            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(campusTypeResponse);

            return campusTypeResponse;
        }

        public FormProgramResponse GetFormPrograms(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetFormPrograms", directoryMatchRequest.Application, directoryMatchRequest);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            FormProgramResponse formProgramResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                int? programId = directoryMatchRequest.ProgramIdList == null || directoryMatchRequest.ProgramIdList.Count == 0 ? (int?)null : directoryMatchRequest.ProgramIdList.First();

                MatchResult results = engine.GetFormProgramMatches(directoryMatchRequest);

                log.StartLogDetail("MatchAggregator.CreateFormProgramResponse");
                formProgramResponse = MatchAggregator.CreateFormProgramResponse(results, programId);
                log.EndLogDetail();

                sw.Stop();
                Task.Run(() => MatchPersister.PersistMatchRequest(
                                                    directoryMatchRequest, formProgramResponse, results,
                                                    ResponseType.FormProgram, "GetFormPrograms", sw.ElapsedMilliseconds, null)
                                            );
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(formProgramResponse);

            return formProgramResponse;
        }

        public CrossSellProgramResponse GetProgramsForCrossSell(CrossSellMatchRequest crossSellRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetProgramsForCrossSell", crossSellRequest.Application, crossSellRequest);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            CrossSellProgramResponse programResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);
                CrossSellMatchResult results = engine.GetCrossSellMatches(crossSellRequest);

                if (crossSellRequest.LeadScoringInput != null && crossSellRequest.LeadScoringInput.LeadScoringTierLevel.HasValue)
                    ApplyReservationRestrictions(results.MatchItemList, results.ChosenCampaign, crossSellRequest.LeadScoringInput.LeadScoringTierLevel.Value);

                int? maxResults = null;

                if (crossSellRequest.MaxResultsCount.HasValue && results.MaxProgramsToDisplay.HasValue)
                    maxResults = crossSellRequest.MaxResultsCount.Value < results.MaxProgramsToDisplay.Value ? crossSellRequest.MaxResultsCount.Value : results.MaxProgramsToDisplay.Value;
                else
                    maxResults = crossSellRequest.MaxResultsCount ?? 0;

                log.StartLogDetail("MatchAggregator.CreateCrossSellResponse");

                //programResponse = MatchAggregator.CreateCrossSellResponse(results, maxResults, crossSellRequest.FormInstitutionId, crossSellRequest.LeadScoringInput == null ? null : crossSellRequest.LeadScoringInput.LeadScoringTierLevel, crossSellRequest.TrackGuid, crossSellRequest.InitialLeadSuccess);
                //TFS-101295 - If the main list has room, create backfill response and add it to the list.
                //Start with the specialty list first
                programResponse = MatchAggregator.CreateCrossSellResponse(results, results.CrossSellSpecialtyMatchItemList, maxResults, crossSellRequest.FormInstitutionId, crossSellRequest.LeadScoringInput == null ? null : crossSellRequest.LeadScoringInput.LeadScoringTierLevel, crossSellRequest.TrackGuid, crossSellRequest.InitialLeadSuccess);
                programResponse.InitialLeadValidMessage = results.CrossSellMappingList == null || results.CrossSellMappingList.Count == 0 ? "" : results.CrossSellMappingList.First().InitialLeadValidMessage;
                programResponse.InitialLeadInvalidMessage = results.CrossSellMappingList == null || results.CrossSellMappingList.Count == 0 ? "" : results.CrossSellMappingList.First().InitialLeadInvalidMessage;
                programResponse.MaxProgramsToDisplay = results.MaxProgramsToDisplay;
                programResponse.MaxUserSelections = results.MaxUserSelections;

                programResponse.IsPreCheckEnabled = results.ChosenCampaign.HasPreCheck;
                programResponse.IsCrossSellALternateList = results.ChosenCampaign.IsCrossSellALternateList;
                log.EndLogDetail();
                
                //TFS-101295 - If the main list has room, create backfill response and add it to the list.
                //If there is room, add from subject list and category list
                if (programResponse.ProgramList.Count < maxResults)
                {                    
                    CrossSellProgramResponse subjectFillProgramResponse = MatchAggregator.CreateCrossSellResponse(results, results.CrossSellSubjectMatchItemList, maxResults, crossSellRequest.FormInstitutionId, crossSellRequest.LeadScoringInput == null ? null : crossSellRequest.LeadScoringInput.LeadScoringTierLevel, crossSellRequest.TrackGuid, crossSellRequest.InitialLeadSuccess);                    
                    
                    //Fill from subject list
                    foreach(ProgramWithInstitutionCampus backfillProgram in subjectFillProgramResponse.ProgramList)
                    {
                        if (!programResponse.ProgramList.Any(x => x.InstitutionId == backfillProgram.InstitutionId))
                        {
                            programResponse.ProgramList.Add(backfillProgram);                            
                            if (programResponse.ProgramList.Count == maxResults)
                                break;
                        }
                    }

                    //If still not enough, fill from category list
                    if (programResponse.ProgramList.Count < maxResults)
                    {
                        CrossSellProgramResponse categoryFillProgramResponse = MatchAggregator.CreateCrossSellResponse(results, results.CrossSellCategoryMatchItemList, maxResults, crossSellRequest.FormInstitutionId, crossSellRequest.LeadScoringInput == null ? null : crossSellRequest.LeadScoringInput.LeadScoringTierLevel, crossSellRequest.TrackGuid, crossSellRequest.InitialLeadSuccess);

                        foreach (ProgramWithInstitutionCampus backfillProgram in categoryFillProgramResponse.ProgramList)
                        {
                            if (!programResponse.ProgramList.Any(x => x.InstitutionId == backfillProgram.InstitutionId))
                            {
                                programResponse.ProgramList.Add(backfillProgram);
                                if (programResponse.ProgramList.Count == maxResults)
                                    break;
                            }
                        }
                    }
                                        
                    programResponse.ResultCount = programResponse.ProgramList.Count;
                }

                sw.Stop();

                Guid? leadScoringGuid = null;

                if (crossSellRequest.LeadScoringInput != null)
                    leadScoringGuid = crossSellRequest.LeadScoringInput.LeadScoringGuid;

                Task.Run(() => MatchPersister.PersistMatchRequest(
                                                    crossSellRequest, programResponse, results,
                                                    ResponseType.CrossSell, "GetProgramsForCrossSell", sw.ElapsedMilliseconds, leadScoringGuid)
                                            );
            }
            catch (Exception ex)
            {
                try
                {
                    programResponse.MatchResponseGuid = Guid.NewGuid();
                    ISException isEx = new ISException(ex, crossSellRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(programResponse);
            return programResponse;
        }

        public AdServerClientRelationshipResponse GetAdServerClientRelationships(AdServerMatchRequest adServerMatchRequest)
        {
            AdServerClientRelationshipResponse adServerResponse = null;

            try
            {
                EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
                log.StartLog(Base.ISApplication.MatchingEngine, "GetAdServerClientRelationships", adServerMatchRequest.Application, adServerMatchRequest);

                Stopwatch sw = new Stopwatch();
                sw.Start();

                MatchingEngine engine = new MatchingEngine(log);

                MatchResult results = engine.GetDirectoryMatches(adServerMatchRequest);

                log.StartLogDetail("MatchAggregator.CreateAdServerClientResponse");
                //TODO: better code :)

                adServerResponse = MatchAggregator.CreateAdServerClientResponse(results, EntitySortMethod.RankScore, adServerMatchRequest); 
                log.EndLogDetail();

                log.EndLog(adServerResponse);
                sw.Stop();
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, adServerMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            return adServerResponse;
        }

        public InstitutionResponse GetInstitutions(DirectoryMatchRequest directoryMatchRequest, GetInstitutionCampusOption campusOption)
        {
            InstitutionResponse institutionResponse = null;

            try
            {
                EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
                log.StartLog(Base.ISApplication.MatchingEngine, "GetInstitutions", directoryMatchRequest.Application, directoryMatchRequest);
                
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Campaign campaign = Campaign.Get(directoryMatchRequest.TrackGuid);
                MatchingEngine engine = new MatchingEngine(log);

                MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                bool isValidGeoTarget = false;

                if (directoryMatchRequest.GeoTarget != null && ((directoryMatchRequest.GeoTarget.PostalCode != null && directoryMatchRequest.GeoTarget.RadiusFromPostalCode.HasValue) || (directoryMatchRequest.GeoTarget.StateList != null && directoryMatchRequest.GeoTarget.StateList.Count > 0)))
                    isValidGeoTarget = true;

                log.StartLogDetail("MatchAggregator.CreateInstitutionResponse");
                
                if (directoryMatchRequest.ApplicationId == 20)
                    institutionResponse = MatchAggregator.CreateInstitutionResponseSAB(results, directoryMatchRequest.SortMethod, directoryMatchRequest.MaxNestedProgramCount, directoryMatchRequest.MaxResultsCount, directoryMatchRequest.PageNumber, campusOption, directoryMatchRequest.IncludeProgramGroupRollup, directoryMatchRequest.ApplicationId, isValidGeoTarget, directoryMatchRequest.IncludeImages, campaign, engine, log);
                else
                    institutionResponse = MatchAggregator.CreateInstitutionResponse(results, directoryMatchRequest.SortMethod, directoryMatchRequest.MaxNestedProgramCount, directoryMatchRequest.MaxResultsCount, directoryMatchRequest.PageNumber, campusOption, directoryMatchRequest.IncludeProgramGroupRollup, directoryMatchRequest.ApplicationId, isValidGeoTarget, directoryMatchRequest.IncludeImages, campaign, engine, log);
                log.EndLogDetail();

                log.EndLog(institutionResponse);
                sw.Stop();

                Task.Run(() => MatchPersister.PersistMatchRequest(
                                                    directoryMatchRequest, institutionResponse, results,
                                                    ResponseType.DirectoryListing, "GetInstitutions", sw.ElapsedMilliseconds, null, campusOption)
                                            );
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            return institutionResponse;
        }

        public ProgramResponse GetPrograms(DirectoryMatchRequest directoryMatchRequest, bool includeProgramDetail)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetPrograms", directoryMatchRequest.Application, directoryMatchRequest, includeProgramDetail);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ProgramResponse programResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                if (directoryMatchRequest.ApplicationId == 20 && directoryMatchRequest.SortMethod == EntitySortMethod.RankScore)
                    directoryMatchRequest.SortMethod = EntitySortMethod.SAB;

                log.StartLogDetail("MatchAggregator.CreateProgramResponse");
                programResponse = MatchAggregator.CreateProgramResponse(results, directoryMatchRequest.SortMethod, directoryMatchRequest.MaxResultsCount, directoryMatchRequest.PageNumber, includeProgramDetail, directoryMatchRequest.IncludeProgramGroupRollup, directoryMatchRequest.ApplicationId, directoryMatchRequest.IncludeImages);
                log.EndLogDetail();

                sw.Stop();
                Task.Run(() => MatchPersister.PersistMatchRequest(
                                                    directoryMatchRequest, programResponse, results,
                                                    ResponseType.DirectoryListing, "GetPrograms", sw.ElapsedMilliseconds, null)
                                            );
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest, includeProgramDetail);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(programResponse);
            return programResponse;
        }

        public CampusResponse GetCampuses(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetCampuses", directoryMatchRequest.Application, directoryMatchRequest);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            CampusResponse campusResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                log.StartLogDetail("MatchAggregator.CreateCampusResponse");

                bool isValidGeoTarget = false;

                if (directoryMatchRequest.GeoTarget != null && ((directoryMatchRequest.GeoTarget.PostalCode != null && directoryMatchRequest.GeoTarget.RadiusFromPostalCode.HasValue) || (directoryMatchRequest.GeoTarget.StateList != null && directoryMatchRequest.GeoTarget.StateList.Count > 0)))
                    isValidGeoTarget = true;

                campusResponse = MatchAggregator.CreateCampusResponse(results, directoryMatchRequest.SortMethod, directoryMatchRequest.MaxNestedProgramCount, directoryMatchRequest.MaxResultsCount, directoryMatchRequest.PageNumber, directoryMatchRequest.IncludeProgramGroupRollup, directoryMatchRequest.ApplicationId, isValidGeoTarget, directoryMatchRequest.IncludeImages);
                log.EndLogDetail();

                sw.Stop();

                Task.Run(() => MatchPersister.PersistMatchRequest(directoryMatchRequest, campusResponse, results, ResponseType.DirectoryListing, "GetCampuses", sw.ElapsedMilliseconds, null));
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(campusResponse);
            return campusResponse;
        }

        private NeoDirectoryMatchRequest CreateDirectoryRequestFromNeoRequest(BaseMatchRequest neoRequest)
        {
            NeoDirectoryMatchRequest directoryRequest = new NeoDirectoryMatchRequest();

            directoryRequest.ApplicationId = neoRequest.ApplicationId;
            directoryRequest.Application = neoRequest.Application;
            directoryRequest.CampusType = neoRequest.CampusType;
            directoryRequest.CategoryList = neoRequest.CategoryList;
            directoryRequest.ProgramLevelList = neoRequest.ProgramLevelList;
            directoryRequest.ProgramTypeList = neoRequest.ProgramTypeList;
            directoryRequest.ProspectInput = neoRequest.ProspectInput;
            directoryRequest.RemoveInvalidEntities = true;
            directoryRequest.SortMethod = EntitySortMethod.RankScore;
            directoryRequest.SubjectList = neoRequest.SubjectList;
            directoryRequest.TrackGuid = neoRequest.TrackGuid;
            directoryRequest.LeadCreationType = LeadCreationType.Advising;

            if (neoRequest.GetType() == typeof(ApolloCampusRequest))
                directoryRequest.UserId = ((ApolloCampusRequest)neoRequest).UserId;
            else if (neoRequest.GetType() == typeof(NeoMatchRequest))
                directoryRequest.UserId = ((NeoMatchRequest)neoRequest).UserId;

            return directoryRequest;
        }

        public NeoResponse GetNeoResponse(NeoMatchRequest neoRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new PerformanceLog(Base.ISApplication.MatchingEngine, "GetNeoResponse", neoRequest.Application, neoRequest);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            NeoResponse response = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                if (neoRequest.ExpectedStartDateValue.HasValue && !String.IsNullOrEmpty(neoRequest.ExpectedStartDateCode))
                {
                    neoRequest.ProspectInput.KVCodeData = new List<KeyValuePair<string, int>>();
                    neoRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>(neoRequest.ExpectedStartDateCode, neoRequest.ExpectedStartDateValue.Value));
                }

                if (neoRequest.HasComputer.HasValue)
                {
                    if (neoRequest.ProspectInput.KVCodeData == null)
                        neoRequest.ProspectInput.KVCodeData = new List<KeyValuePair<string, int>>();

                    if (neoRequest.HasComputer.Value)
                        neoRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("Computer_Internet", 22));
                    else
                        neoRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>("Computer_Internet", 23));
                }

                MatchResult results = engine.GetDirectoryMatches(CreateDirectoryRequestFromNeoRequest(neoRequest), neoRequest.SearchTerm);

                log.StartLogDetail("MatchAggregator.CreateNeoResponse");
                response = MatchAggregator.CreateNeoResponse(results, neoRequest, log);
                log.EndLogDetail();

                sw.Stop();
                ThreadPool.QueueUserWorkItem(o => MatchPersister.PersistMatchRequest(
                                                    neoRequest, response, results,
                                                    ResponseType.Wizard, "GetNeoResponse", sw.ElapsedMilliseconds, null)
                                            );
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, neoRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(response);
            return response;
        }

        public ApolloCampusResponse GetApolloCampuses(ApolloCampusRequest apolloRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new PerformanceLog(Base.ISApplication.MatchingEngine, "GetApolloCampuses", apolloRequest.Application, apolloRequest);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ApolloCampusResponse response = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                if(apolloRequest.ExpectedStartDateValue.HasValue && !String.IsNullOrEmpty(apolloRequest.ExpectedStartDateCode))
                {
                    apolloRequest.ProspectInput.KVCodeData = new List<KeyValuePair<string, int>>();
                    apolloRequest.ProspectInput.KVCodeData.Add(new KeyValuePair<string, int>(apolloRequest.ExpectedStartDateCode, apolloRequest.ExpectedStartDateValue.Value));
                }

                MatchResult results = engine.GetDirectoryMatches(CreateDirectoryRequestFromNeoRequest(apolloRequest), apolloRequest.SearchTerm);

                log.StartLogDetail("MatchAggregator.CreateApolloCampusResponse");
                response = MatchAggregator.CreateApolloCampusResponse(results, apolloRequest, log);
                log.EndLogDetail();

                sw.Stop();
                ThreadPool.QueueUserWorkItem(o => MatchPersister.PersistMatchRequest(
                                                    apolloRequest, response, results,
                                                    ResponseType.Wizard, "GetApolloCampuses", sw.ElapsedMilliseconds, null)
                                            );
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, apolloRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(response);
            return response;
        }

        public SiteMapResponse GetSiteMapGeoInfo(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetSiteMapGeoInfo", directoryMatchRequest.Application, directoryMatchRequest);

            SiteMapResponse mapResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                //don't need to sort this response, so we want to skip SRA logic to improve performance
                directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                log.StartLogDetail("MatchAggregator.CreateSiteMapResponse");
                mapResponse = MatchAggregator.CreateSiteMapResponse(results);
                log.EndLogDetail();
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(mapResponse);

            return mapResponse;
        }

        public NavigationResponse GetFacetedNavigation(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetFacetedNavigation", directoryMatchRequest.Application, directoryMatchRequest);

            NavigationResponse navResponse = null;

            try
            {
                string key = "NAV_" + JsonConvert.SerializeObject(directoryMatchRequest);

                if (System.Web.HttpRuntime.Cache[key] == null)
                {
                    MatchingEngine engine = new MatchingEngine(log);

                    //don't need to sort this response, so we want to skip SRA logic to improve performance
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                    log.StartLogDetail("MatchAggregator.CreateFacetedNavigationResponse");
                    navResponse = MatchAggregator.CreateFacetedNavigationResponse(results, directoryMatchRequest);
                    log.EndLogDetail();
                    System.Web.HttpRuntime.Cache.Insert(key, navResponse, null, DateTime.Now.Add(new TimeSpan(0, 15, 0)), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                else
                {
                    navResponse = (NavigationResponse)System.Web.HttpRuntime.Cache[key];
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(navResponse);
            return navResponse;
        }

        public CategoryResponse GetCategories(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetCategories", directoryMatchRequest.Application, directoryMatchRequest);

            CategoryResponse categoryResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                string key = "CAT_" + JsonConvert.SerializeObject(directoryMatchRequest);

                if (System.Web.HttpRuntime.Cache[key] == null)
                {
                    //don't need to sort this response, so we want to skip SRA logic to improve performance
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                    log.StartLogDetail("MatchAggregator.CreateCategoryResponse");
                    categoryResponse = MatchAggregator.CreateCategoryResponse(results, directoryMatchRequest);
                    log.EndLogDetail();
                    System.Web.HttpRuntime.Cache.Insert(key, categoryResponse, null, DateTime.Now.Add(new TimeSpan(0, 15, 0)), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                else
                {
                    categoryResponse = (CategoryResponse)System.Web.HttpRuntime.Cache[key];
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(categoryResponse);
            return categoryResponse;
        }

        public CategoryResponse GetCategoriesWithSubjects(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetCategoriesWithSubjects", directoryMatchRequest.Application, directoryMatchRequest);

            CategoryResponse categoryResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                string key = "CATSUB_" + JsonConvert.SerializeObject(directoryMatchRequest);

                if (System.Web.HttpRuntime.Cache[key] == null)
                {
                    //don't need to sort this response, so we want to skip SRA logic to improve performance
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                    log.StartLogDetail("MatchAggregator.CreateCategoryWithSubjectsResponse");
                    categoryResponse = MatchAggregator.CreateCategoryWithSubjectsResponse(results, directoryMatchRequest);
                    log.EndLogDetail();
                    System.Web.HttpRuntime.Cache.Insert(key, categoryResponse, null, DateTime.Now.Add(new TimeSpan(0, 15, 0)), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                else
                {
                    categoryResponse = (CategoryResponse)System.Web.HttpRuntime.Cache[key];
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(categoryResponse);
            return categoryResponse;
        }

        public SubjectResponse GetSubjects(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetSubjects", directoryMatchRequest.Application, directoryMatchRequest);

            SubjectResponse subjectResponse = null;

            try
            {
                string key = "SUB_" + JsonConvert.SerializeObject(directoryMatchRequest);

                if (System.Web.HttpRuntime.Cache[key] == null)
                {
                    MatchingEngine engine = new MatchingEngine(log);

                    //don't need to sort this response, so we want to skip SRA logic to improve performance
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                    log.StartLogDetail("MatchAggregator.CreateSubjectResponse");
                    subjectResponse = MatchAggregator.CreateSubjectResponse(results, directoryMatchRequest);
                    log.EndLogDetail();
                    System.Web.HttpRuntime.Cache.Insert(key, subjectResponse, null, DateTime.Now.Add(new TimeSpan(0, 15, 0)), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                else
                {
                    subjectResponse = (SubjectResponse)System.Web.HttpRuntime.Cache[key];
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(subjectResponse);
            return subjectResponse;
        }

        public CategoryResponse GetCategoriesAllIfNone(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetCategoriesAllIfNone", directoryMatchRequest.Application, directoryMatchRequest);

            CategoryResponse categoryResponse = null;

            try
            {
                string key = "CATALL_" + JsonConvert.SerializeObject(directoryMatchRequest);

                if (System.Web.HttpRuntime.Cache[key] == null)
                {
                    MatchingEngine engine = new MatchingEngine(log);

                    //don't need to sort this response, so we want to skip SRA logic to improve performance
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    MatchResult results = engine.GetDirectoryMatchesAllIfNone(directoryMatchRequest);

                    log.StartLogDetail("MatchAggregator.CreateCategoryResponse");
                    categoryResponse = MatchAggregator.CreateCategoryResponse(results, directoryMatchRequest);
                    log.EndLogDetail();
                    System.Web.HttpRuntime.Cache.Insert(key, categoryResponse, null, DateTime.Now.Add(new TimeSpan(0, 15, 0)), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                else
                {
                    categoryResponse = (CategoryResponse)System.Web.HttpRuntime.Cache[key];
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(categoryResponse);
            return categoryResponse;
        }


        public SubjectResponse GetSubjectsAllIfNone(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetSubjectsAllIfNone", directoryMatchRequest.Application, directoryMatchRequest);

            SubjectResponse subjectResponse = null;

            try
            {
                string key = "SUBALL_" + JsonConvert.SerializeObject(directoryMatchRequest);

                if (System.Web.HttpRuntime.Cache[key] == null)
                {
                    MatchingEngine engine = new MatchingEngine(log);

                    //don't need to sort this response, so we want to skip SRA logic to improve performance
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    MatchResult results = engine.GetDirectoryMatchesAllIfNone(directoryMatchRequest);

                    log.StartLogDetail("MatchAggregator.CreateSubjectResponse");
                    subjectResponse = MatchAggregator.CreateSubjectResponse(results, directoryMatchRequest);
                    log.EndLogDetail();
                    System.Web.HttpRuntime.Cache.Insert(key, subjectResponse, null, DateTime.Now.Add(new TimeSpan(0, 15, 0)), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                else
                {
                    subjectResponse = (SubjectResponse)System.Web.HttpRuntime.Cache[key];
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(subjectResponse);
            return subjectResponse;
        }

        public SpecialtyResponse GetSpecialties(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetSpecialties", directoryMatchRequest.Application, directoryMatchRequest);

            SpecialtyResponse specialtyResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                //don't need to sort this response, so we want to skip SRA logic to improve performance
                directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                log.StartLogDetail("MatchAggregator.CreateSpecialtyResponse");
                specialtyResponse = MatchAggregator.CreateSpecialtyResponse(results, directoryMatchRequest);
                log.EndLogDetail();
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(specialtyResponse);
            return specialtyResponse;
        }

        public SpecialtyResponse GetSpecialtiesAllIfNone(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetSpecialtiesAllIfNone", directoryMatchRequest.Application, directoryMatchRequest);

            SpecialtyResponse specialtyResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                //don't need to sort this response, so we want to skip SRA logic to improve performance
                directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                MatchResult results = engine.GetDirectoryMatchesAllIfNone(directoryMatchRequest);

                log.StartLogDetail("MatchAggregator.CreateSpecialtyResponse");
                specialtyResponse = MatchAggregator.CreateSpecialtyResponse(results, directoryMatchRequest);
                log.EndLogDetail();
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(specialtyResponse);
            return specialtyResponse;
        }

        public ProgramLevelResponse GetProgramLevels(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetProgramLevels", directoryMatchRequest.Application, directoryMatchRequest);

            ProgramLevelResponse programLevelResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                MatchResult results = engine.GetDirectoryMatches(directoryMatchRequest);

                //don't need to sort this response, so we want to skip SRA logic to improve performance
                directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                log.StartLogDetail("MatchAggregator.CreateProgramLevelResponse");
                programLevelResponse = MatchAggregator.CreateProgramLevelResponse(results);
                log.EndLogDetail();
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(programLevelResponse);
            return programLevelResponse;
        }

        public ProgramLevelResponse GetProgramLevelsAllIfNone(DirectoryMatchRequest directoryMatchRequest)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetProgramLevelsAllIfNone", directoryMatchRequest.Application, directoryMatchRequest);

            ProgramLevelResponse programLevelResponse = null;

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                //don't need to sort this response, so we want to skip SRA logic to improve performance
                directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                MatchResult results = engine.GetDirectoryMatchesAllIfNone(directoryMatchRequest);

                log.StartLogDetail("MatchAggregator.CreateProgramLevelResponse");
                programLevelResponse = MatchAggregator.CreateProgramLevelResponse(results);
                log.EndLogDetail();
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, directoryMatchRequest);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(programLevelResponse);
            return programLevelResponse;
        }

        public InstitutionDetailResponse GetInstitutionDetails(int applicationId, int institutionId, Guid TrackGuid)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetInstitutionDetails", null, institutionId, TrackGuid);

            InstitutionDetailResponse instDetailResponse = new InstitutionDetailResponse();

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                instDetailResponse.InstitutionDetails = engine.GetInstitutionDetail(TrackGuid, institutionId);
                instDetailResponse.MatchResponseGuid = Guid.NewGuid();
                instDetailResponse.ResultCount = instDetailResponse.InstitutionDetails == null ? 0 : 1;
                
                //TODO: fix this hack!!!              
                if (institutionId == 226)
                {
                    instDetailResponse.InstitutionDetails.BillingRuleList = new List<string>();
                    instDetailResponse.InstitutionDetails.BillingRuleList.Add("FlatFee");
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, applicationId, institutionId, TrackGuid);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(instDetailResponse);
            return instDetailResponse;
        }

        public ProgramDetailResponse GetProgramDetails(int applicationId, int programId, Guid TrackGuid, bool? includeProgramGroupRollup, int? campusId)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetProgramDetails", null, programId, TrackGuid, includeProgramGroupRollup);

            ProgramDetailResponse programDetailResponse = new ProgramDetailResponse();

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

                if (applicationId > 0)
                {
                    programDetailResponse.ProgramDetails = engine.GetProgramDetails(applicationId, TrackGuid, programId, includeProgramGroupRollup, campusId);
                }
                else
                { 
                    programDetailResponse.ProgramDetails = engine.GetProgramDetails(TrackGuid, programId, includeProgramGroupRollup, campusId);
                }
                programDetailResponse.MatchResponseGuid = Guid.NewGuid();
                programDetailResponse.ResultCount = programDetailResponse.ProgramDetails == null ? 0 : 1;
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, applicationId, programId, TrackGuid);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(programDetailResponse);

            return programDetailResponse;
        }

        public ProgramDetailResponse GetProgramDisplayGroupDetails(int applicationId, int programDisplayGroupId, Guid TrackGuid)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetProgramDisplayGroupDetails", null, programDisplayGroupId, TrackGuid);

            ProgramDetailResponse programDetailResponse = new ProgramDetailResponse();

            try
            {
                MatchingEngine engine = new MatchingEngine(log);

               programDetailResponse.ProgramDetails = engine.GetProgramGroupDetails(applicationId, TrackGuid, programDisplayGroupId);

                programDetailResponse.MatchResponseGuid = Guid.NewGuid();
                programDetailResponse.ResultCount = programDetailResponse.ProgramDetails == null ? 0 : 1;
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, applicationId, programDisplayGroupId, TrackGuid);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(programDetailResponse);

            return programDetailResponse;
        }

        public List<ProgramRuleDefinition> GetRulesForProgramProduct(int ProgramProductId)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(ISApplication.MatchingEngine, "GetRulesForProgramProduct", null, ProgramProductId);
            List<ProgramRuleDefinition> ret = new List<ProgramRuleDefinition>();
            try
            {
                ret = MatchAggregator.GetRulesForProgramProduct(ProgramProductId);
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, ProgramProductId);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(ret);
            return ret;
        }

        private ProgramValidateResponse CreatePvrForFailure(List<MatchItem> matches, string ruleName, BaseRuleType? ruleType, EntityMeta? entityType)
        {
            ProgramValidateResponse pvr = new ProgramValidateResponse();

            pvr.PassedValidation = false;

            if (matches != null && matches.Count() > 0)
            {
                pvr.PaidStatusTypeId = matches.FirstOrDefault().Match.PaidStatusTypeId;
                pvr.ProgramType = (ProgramType)matches.FirstOrDefault().Match.ProgramTypeId;
                pvr.Score = matches.FirstOrDefault().Score;
                pvr.ScoreId = matches.FirstOrDefault().ScoreId;
            }
            else
            {
                pvr.PaidStatusTypeId = PaidStatusType.Paid;
                pvr.ProgramType = ProgramType.FullDegree;
            }

            RuleFailure rf = new RuleFailure();
            pvr.RuleFailures = new List<RuleFailure>();
            rf.RuleFailureName = ruleName;
            rf.RuleFailureType = ruleType;
            rf.EntityType = entityType;
            pvr.RuleFailures.Add(rf);

            return pvr;
        }

		private ProgramValidateResponse CreatePvrFromMatch(MatchItem m)
		{
			ProgramValidateResponse pvr = new ProgramValidateResponse();

            pvr.PassedValidation = true;
			pvr.PaidStatusTypeId = m.Match.PaidStatusTypeId;
			pvr.CampusId = m.Match.CampusId;
			pvr.CampusName = m.Match.CampusName;
			//pvr.HasCampusLogo = m.HasCampusLogo;
			pvr.CampusLogoURL = m.Match.CampusLogoURL;
			pvr.InstitutionDescription = m.Match.InstitutionDescription;
			pvr.InstitutionDescriptionInternational = m.Match.InstitutionDescriptionInternational;
			pvr.InstitutionId = m.Match.InstitutionId;
			pvr.InstitutionName = m.Match.InstitutionName;
			pvr.ProgramCampusType = (CampusType)m.Match.ProgramCampusTypeId;
			pvr.ProgramDescription = m.Match.ProgramDescription;
			pvr.ProgramId = m.Match.ProgramId;
			pvr.ProgramName = m.Match.ProgramName;
			pvr.ProgramType = (ProgramType)m.Match.ProgramTypeId;
			pvr.InstitutionLogoURL = m.Match.InstitutionLogoURL;
            pvr.SkipSchoolSelection = !m.FailedValidation && ((m.Match.TreatAsMatch1 && m.Match.ProductId != (int)ProductType.Lead_GS) || (m.Match.ProductId == (int)ProductType.Match1Exclusive));
            pvr.Score = m.Score;
            pvr.ScoreId = m.ScoreId;
            if (m.ScoreId.HasValue && m.ScoreId.GetValueOrDefault() > 0)
            {
                MatchingEngine me = new MatchingEngine();
                decimal cpl = me.GetLeadPingLeadScoreCPL(
                     m.Match.ProductId,
                     m.Match.InstitutionId,
                     m.Match.CampusCampusTypeId,
                     m.ScoreId.GetValueOrDefault()
                    );
                pvr.LeadPingScoreCPL = cpl;
            }

            return pvr;
		}

        public ProgramValidateResponse ValidateProgram(ProgramValidateRequest request)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "ValidateProgram", request.Application, request);

            ProgramValidateResponse pvr = new ProgramValidateResponse();

            try
            {
                MatchingEngine me = new MatchingEngine(log);
                MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

                //Perform EPLite Upsell if parameters passed in.
                if (request.LookForEPUpsell.HasValue && request.LookForEPUpsell.Value == true && request.ProgramProductId != 0)
                {
                    MatchItem epLiteUpsell = me.FindUpsellProgramProduct(request.ProgramProductId, request, request.TrackGuid, ProductType.EPLite);

                    if(epLiteUpsell != null)
                    {
                        pvr = CreatePvrFromMatch(epLiteUpsell);
                        pvr.AlternateProgramProductId = epLiteUpsell.Match.ProgramProductId;
                        return pvr;
                    }
                }

                // Perform Titanium upsell from warm transfer products in the outbound channel
                if (request.ProgramProductId != 0)
                {
                    MatchItem mi = md.GetMatchItemByProgramProductId(request.ProgramProductId);
                    Campaign campaign = Campaign.Get(request.TrackGuid);

                    if (mi?.Match.UpsellOutboundTitanium == true
                        && mi.Match.ProductId != (int)ProductType.WarmTransferTitanium
                        && campaign?.SubChannelId == (int)SubChannel.Outbound
                        && (mi.Match.ProductId == (int)ProductType.WarmTransfer_Silver || mi.Match.ProductId == (int)ProductType.LiveTransfer || mi.Match.ProductId == (int)ProductType.LiveTransfer_Tier2)
                        && Guid.TryParse(ConfigurationManager.AppSettings["WTTitaniumUpsellTrackId"], out Guid titaniumTrackId))
                    {
                        MatchItem titaniumUpsell = me.FindUpsellProgramProduct(request.ProgramProductId, request, titaniumTrackId, ProductType.WarmTransferTitanium);

                        if (titaniumUpsell != null)
                        {
                            pvr = CreatePvrFromMatch(titaniumUpsell);
                            pvr.AlternateProgramProductId = titaniumUpsell.Match.ProgramProductId;
                            return pvr;
                        }
                    }
                    else
                    {
                        MatchItem titaniumSMPUpsell = FindTitaniumSMPUpsell(me, request, mi, request.ProgramProductId);
                        if (titaniumSMPUpsell != null)
                        {
                            pvr = CreatePvrFromMatch(titaniumSMPUpsell);
                            pvr.AlternateProgramProductId = titaniumSMPUpsell.Match.ProgramProductId;
                            return pvr;
                        }
                    }

                }

                Tuple<RulesResult, int> rr = me.GetRulesResult(request.ProgramProductId, request.ProspectInput, false, request.TrackGuid, request.Application, request.IgnoreCaps, request.LeadCreationType, true);

                if (rr.Item1.MatchedProgramProductList != null && rr.Item1.MatchedProgramProductList.Count() > 0 && !rr.Item1.MatchedProgramProductList.Any(pp => pp.FailedValidation == true))
                {
                    if (rr.Item1.MatchedProgramProductList.Where(pp => pp.Match.InquiryDisabled == false).Count() > 0)
                    {
                        pvr = CreatePvrFromMatch(rr.Item1.MatchedProgramProductList.FirstOrDefault());
                    }
                    else
                    {
                        pvr = CreatePvrForFailure(rr.Item1.MatchedProgramProductList, "Inquiry Disabled", BaseRuleType.ProgramNotAvailable, EntityMeta.ProgramProduct);
                    }
                }
                else
                {
                    bool checkAlternateProgram = true;
                    
                    if (rr.Item1.MatchedProgramProductList != null && rr.Item1.MatchedProgramProductList.Any())
                    {
                        var mp = rr.Item1.MatchedProgramProductList.FirstOrDefault();
                        if (mp != null && mp.FailedValidation && (mp.RemovalReason.RuleType == BaseRuleType.Spam || mp.RemovalReason.RuleType == BaseRuleType.SpamReportingOnly))
                        {
                            checkAlternateProgram = false;
                            pvr = CreatePvrForFailure(rr.Item1.MatchedProgramProductList, rr.Item1.NoMatchOutput[0].RuleName, rr.Item1.NoMatchOutput[0].RuleType, rr.Item1.NoMatchOutput[0].entityType);
                        }
                    }

                    if (checkAlternateProgram)
                    {
                        MatchItem match = me.FindAlternateProgramProduct(rr.Item2, request);

                        int? alternateProgramProductId = match == null ? (int?)null : (int?)match.Match.ProgramProductId;

                        if (alternateProgramProductId.HasValue)
                        {
                            pvr = CreatePvrFromMatch(match);
                            pvr.AlternateProgramProductId = alternateProgramProductId;
                        }
                        else
                        {
                            pvr = CreatePvrForFailure(rr.Item1.MatchedProgramProductList, rr.Item1.NoMatchOutput[0].RuleName, rr.Item1.NoMatchOutput[0].RuleType, rr.Item1.NoMatchOutput[0].entityType);
                        }
                    }
                    
                }  
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, request);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(pvr);

            return pvr;
        }


        /// <summary>
        /// this method will validate the program product specifically against the additional questions
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ProgramProductValidateResponse ValidateProgramProducts(ProgramProductValidateRequest request)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(ISApplication.MatchingEngine, "ValidateProgram", request.Application, request);

            ProgramProductValidateResponse ppvr = new ProgramProductValidateResponse();
            ppvr.ProgramProductValidateResults = new List<ProgramValidateResponse>();
            try
            {
                MatchingEngine me = new MatchingEngine(log);
                foreach (int ppid in request.ProgramProductIds)
                {
                    ProgramValidateResponse pvr = new ProgramValidateResponse();
                    Tuple<RulesResult, int> rr = me.GetProgramProductRulesResult(ppid, request.ProspectInput, request.TrackGuid, request.Application, request.LeadCreationType);

                    if (rr.Item1.MatchedProgramProductList != null && rr.Item1.MatchedProgramProductList.Count() > 0 && !rr.Item1.MatchedProgramProductList.Any(pp => pp.FailedValidation == true))
                    {

                        if (rr.Item1.MatchedProgramProductList.Where(pp => pp.Match.InquiryDisabled == false).Count() > 0)
                        {
                            pvr.PassedValidation = true;
                            pvr.PaidStatusTypeId = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.PaidStatusTypeId;
                            pvr.CampusId = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.CampusId;
                            pvr.CampusName = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.CampusName;
                            //pvr.HasCampusLogo = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.HasCampusLogo;
                            pvr.CampusLogoURL = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.CampusLogoURL;
                            pvr.InstitutionDescription = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.InstitutionDescription;
                            pvr.InstitutionDescriptionInternational = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.InstitutionDescriptionInternational;
                            pvr.InstitutionId = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.InstitutionId;
                            pvr.InstitutionName = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.InstitutionName;
                            pvr.ProgramCampusType = (CampusType)rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.ProgramCampusTypeId;
                            pvr.ProgramDescription = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.ProgramDescription;
                            pvr.ProgramId = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.ProgramId;
                            pvr.ProgramName = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.ProgramName;
                            pvr.ProgramType = (ProgramType)rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.ProgramTypeId;
                            pvr.ProgramProductId = ppid;
                            pvr.InstitutionLogoURL = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.InstitutionLogoURL;
                        }
                        else
                        {
                            pvr.PassedValidation = false;
                            pvr.ProgramProductId = ppid;

                            if (rr.Item1.MatchedProgramProductList != null && rr.Item1.MatchedProgramProductList.Count() > 0)
                            {
                                pvr.PaidStatusTypeId = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.PaidStatusTypeId;
                                pvr.ProgramType = (ProgramType)rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.ProgramTypeId;
                            }
                            else
                            {
                                pvr.PaidStatusTypeId = PaidStatusType.Paid;
                                pvr.ProgramType = ProgramType.FullDegree;
                            }

                            RuleFailure rf = new RuleFailure();
                            pvr.RuleFailures = new List<RuleFailure>();
                            rf.RuleFailureName = "Inquiry Disabled";
                            rf.RuleFailureType = BaseRuleType.ProgramNotAvailable;
                            rf.EntityType = EntityMeta.ProgramProduct;
                            pvr.RuleFailures.Add(rf);
                        }
                    }
                    else
                    {
                        pvr.PassedValidation = false;
                        pvr.ProgramProductId = ppid;

                        if (rr.Item1.MatchedProgramProductList != null && rr.Item1.MatchedProgramProductList.Count() > 0)
                        {
                            pvr.PaidStatusTypeId = rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.PaidStatusTypeId;
                            pvr.ProgramType = (ProgramType)rr.Item1.MatchedProgramProductList.FirstOrDefault().Match.ProgramTypeId;
                        }
                        else
                        {
                            pvr.PaidStatusTypeId = PaidStatusType.Paid;
                            pvr.ProgramType = ProgramType.FullDegree;
                        }

                        RuleFailure rf = new RuleFailure();
                        pvr.RuleFailures = new List<RuleFailure>();
                        rf.RuleFailureName = rr.Item1.NoMatchOutput[0].RuleName;
                        rf.RuleFailureType = rr.Item1.NoMatchOutput[0].RuleType;
                        rf.EntityType = rr.Item1.NoMatchOutput[0].entityType;
                        rf.StandardControlCode = rr.Item1.NoMatchOutput[0].StandardControlCode;
                        pvr.InstitutionId = rr.Item1.NoMatchOutput[0].InstitutionId;
                        pvr.RuleFailures.Add(rf);


                    }
                    ppvr.ProgramProductValidateResults.Add(pvr);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, request);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(ppvr);

            return ppvr;
        }

        public ProgramValidateResponse ValidateCountryAndProgram(CountryValidateRequest request)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "ValidateCountryAndProgram", request.Application, request);

            ProgramValidateResponse pvr = new ProgramValidateResponse();

            try
            {
                MatchingEngine me = new MatchingEngine(log);
                Tuple<RulesResult, int> rr = me.GetRulesResult(request.ProgramProductId, new DTO.ProspectInput() { CountryId = request.CountryId }, true, null, request.Application, false, null);

                if (rr.Item1.NoMatchOutput.Count() == 0)
                    pvr.PassedValidation = true;
                else
                {
                    pvr.PassedValidation = false;
                    RuleFailure rf = new RuleFailure();
                    pvr.RuleFailures = new List<RuleFailure>();
                    rf.RuleFailureName = rr.Item1.NoMatchOutput[0].RuleName;
                    rf.RuleFailureType = rr.Item1.NoMatchOutput[0].RuleType;
                    pvr.RuleFailures.Add(rf);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, request);
                    isEx.Save();
                }
                catch { }
            }


            log.EndLog(pvr);

            return pvr;
        }

        public CampusZipCodeRuleResponse GetCampusZipCodeRules(int campusId, Guid trackGuid)
        {
            CampusZipCodeRuleResponse zipResponse = new CampusZipCodeRuleResponse();

            try
            {
                zipResponse = RulesEngine.GetCampusZipCodeRules(campusId, trackGuid);
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, campusId, trackGuid);
                    isEx.Save();
                }
                catch { }
            }

            return zipResponse;
        }

        public void RefreshCacheItem(MatchingCacheItem key)
        {
            StaticCacheProxyHost.CacheProxy.RemoveItem(key);
        }

        public void RemoveCacheItem(string key)
        {
            StaticCacheProxyHost.CacheProxy.RemoveItem(key);
        }

        public CampaignDetailResponse GetCampaignDetailByTrackID(Guid TrackID)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetCampaignDetailByTrackID", null, TrackID);

            CampaignDetailResponse campaignDetailResponse = null;

            try
            {
                MatchingEngine TheMatchingEngine = new MatchingEngine(log);

                log.StartLogDetail("MatchingEngine.GetCampaignDetailByTrackID");
                campaignDetailResponse = TheMatchingEngine.GetCampaignDetailByTrackID(TrackID);
                log.EndLogDetail();
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, TrackID);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(campaignDetailResponse);
            return campaignDetailResponse;
        }

        public ApiProgramResponse GetApiProgram(int programId, Guid trackGuid)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "GetApiProgram", Base.ISApplication.VendorAPI, programId, trackGuid);

            ApiProgramResponse api = new ApiProgramResponse();

            try
            {
                MatchingEngine me = new MatchingEngine(log);
                MatchResult mr = me.GetDirectoryMatches(new DirectoryMatchRequest { TrackGuid = trackGuid, ProgramIdList = new List<int> { programId } });

                api = MatchAggregator.CreateApiProgramResponse(mr);
                api.MatchResponseGuid = Guid.NewGuid();
                api.ResultCount = 1;
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex);
                    isEx.Save();
                }
                catch { }
            }
            return api;
        }

        public ProgramValidateResponse ValidateAPIProgram(ProgramValidateRequest request)
        {
            EDDY.IS.Core.Logging.PerformanceLog log = new Core.Logging.PerformanceLog();
            log.StartLog(Base.ISApplication.MatchingEngine, "ValidateAPIProgram", request.Application, request);

            ProgramValidateResponse pvr = new ProgramValidateResponse();

            try
            {
                MatchingEngine me = new MatchingEngine(log);
                ValidateProgramMatchResult mr = me.GetValidateProgramMatchResult(request.ProgramProductId, request.ProgramId, 
                                                                                 request.CampusId, request.ProspectInput,
                                                                                 request.TrackGuid, request.Application, request.IgnoreCaps,
                                                                                 request.LeadCreationType, request.AgentId);

                if (mr.ResultType == ValidateProgramMatchResultType.MatchExists)
                {
                    MatchItem mi = mr.MatchItemList.FirstOrDefault();

                    MatchItem titaniumSMPUpsell = FindTitaniumSMPUpsell(me, request, mi, mi.Match.ProgramProductId);
                    if (titaniumSMPUpsell != null)
                        mi = titaniumSMPUpsell;

                    pvr.PassedValidation = true;
                    pvr.PaidStatusTypeId = mi.Match.PaidStatusTypeId;
                    pvr.CampusId = mi.Match.CampusId;
                    pvr.CampusName = mi.Match.CampusName;
                    pvr.CampusLogoURL = mi.Match.CampusLogoURL;
                    pvr.InstitutionDescription = mi.Match.InstitutionDescription;
                    pvr.InstitutionDescriptionInternational = mi.Match.InstitutionDescriptionInternational;
                    pvr.InstitutionId = mi.Match.InstitutionId;
                    pvr.InstitutionName = mi.Match.InstitutionName;
                    pvr.ProgramCampusType = (CampusType)mi.Match.ProgramCampusTypeId;
                    pvr.ProgramDescription = mi.Match.ProgramDescription;
                    pvr.ProgramId = mi.Match.ProgramId;
                    pvr.ProgramName = mi.Match.ProgramName;
                    pvr.ProgramType = (ProgramType)mi.Match.ProgramTypeId;
                    pvr.ProgramProductId = mi.Match.ProgramProductId;
                    pvr.InstitutionLogoURL = mi.Match.InstitutionLogoURL;
                    pvr.RevenuePerLead = mi.Match.RPL;
                    pvr.EffectiveRevenuePerLead = mi.Match.eRPL;

                }
                else
                {
                    pvr.PassedValidation = false;
                    pvr.PaidStatusTypeId = PaidStatusType.Paid;
                    pvr.ProgramType = ProgramType.FullDegree;

                    RuleFailure rf = new RuleFailure();
                    pvr.RuleFailures = new List<RuleFailure>();
                    switch (mr.ResultType)
                    {
                        case ValidateProgramMatchResultType.CampaignCappedOut:
                            rf.RuleFailureName = "Capped Out at the Campaign Level";
                            rf.RuleFailureType = BaseRuleType.CampaignCapReached;
                            break;
                        case ValidateProgramMatchResultType.CampaignInactive:
                            rf.RuleFailureName = "Campaign is Not Active or Failed Scoring Criteria";
                            rf.RuleFailureType = BaseRuleType.CampaignInactive;
                            break;
                        case ValidateProgramMatchResultType.CampaignLeadScore:
                            rf.RuleFailureName = "Campaign Failed Lead Scoring Requirements";
                            rf.RuleFailureType = BaseRuleType.LeadScoringMinimumTierLevel;
                            break;
                        case ValidateProgramMatchResultType.InquiryDisabled:
                            rf.RuleFailureName = "Inquiry Disabled";
                            rf.RuleFailureType = BaseRuleType.ProgramNotAvailable;
                            break;
                        case ValidateProgramMatchResultType.NoPaidPrograms:
                            rf.RuleFailureName = "No Paid Programs";
                            rf.RuleFailureType = BaseRuleType.ProgramNotAvailable;
                            break;
                        case ValidateProgramMatchResultType.ProgramNotAvailable:
                            rf.RuleFailureName = "Program Not Available";
                            rf.RuleFailureType = BaseRuleType.ProgramNotAvailable;
                            break;
                        case ValidateProgramMatchResultType.CampaignRestriction:
                            rf.RuleFailureName = "Campaign Restriction";
                            rf.RuleFailureType = BaseRuleType.CampaignRestriction;
                            break;
                        default:
                            foreach (var f in mr.RulesResult.NoMatchOutput)
                            {
                                RuleFailure r = new RuleFailure();
                                r.EntityType = f.entityType;
                                r.RuleFailureName = f.RuleName;
                                r.RuleFailureType = f.RuleType;
                                pvr.RuleFailures.Add(r);
                            }
                            break;
                    }

                    if (rf != null)
                    {
                        if (rf != null)
                        {
                            if (!string.IsNullOrEmpty(rf.RuleFailureName))
                            {
                                pvr.RuleFailures.Add(rf);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, request);
                    isEx.Save();
                }
                catch { }
            }

            log.EndLog(pvr);

            return pvr;
        }
    
        public MatchItem FindTitaniumSMPUpsell(MatchingEngine me, ProgramValidateRequest request, MatchItem mi, int programProductId)
        {
            MatchItem titaniumSMPUpsell = null;

            if (me != null 
                && mi != null 
                && mi.Match != null 
                && request != null)
            {
                if (mi.Match.ProductId != (int)ProductType.WarmTransferTitanium
                    && request.AgentId.HasValue
                    && Product.IsWarmTransferProduct(mi.Match.ProductId)
                    && Guid.TryParse(ConfigurationManager.AppSettings["WTTitaniumUpsellTrackId"], out Guid titaniumSMPTrackId))
                {
                    Dictionary<int, List<SchoolAgent>> crAgents = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<SchoolAgent>>>(MatchingCacheItem.ClientRelationContacts);
                    Dictionary<int, List<SchoolAgent>> ccAgents = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<SchoolAgent>>>(MatchingCacheItem.ClientCampusContacts);
                    SchoolAgent agent = null;

                    if (ccAgents.ContainsKey(mi.Match.ClientCampusRelationshipId))
                        agent = ccAgents[mi.Match.ClientCampusRelationshipId].Where(a => a.AgentId == request.AgentId.Value).FirstOrDefault();
                    if (agent == null && crAgents.ContainsKey(mi.Match.ClientRelationshipId))
                        agent = crAgents[mi.Match.ClientRelationshipId].Where(a => a.AgentId == request.AgentId.Value).FirstOrDefault();

                    if (agent != null && agent.AgentName.ToLower() == "did not answer")
                    {
                        titaniumSMPUpsell = me.FindUpsellProgramProduct(programProductId, request, titaniumSMPTrackId, ProductType.WarmTransferTitaniumSMP);
                    }
                }
                else if (mi.Match.ProductId == (int)ProductType.WarmTransferTitaniumSMP)
                {
                    return mi;
                }
            }

            return titaniumSMPUpsell;
        }
    }
}
