using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.Core.Logging;
using EDDY.IS.MatchingEngine.Logging;
using System.Configuration;
using EDDY.IS.MatchingEngine.Logging.DataModel;
using System.IO;
using EDDY.IS.MatchingEngine.Constants;
using Newtonsoft.Json;

namespace EDDY.IS.MatchingEngine
{
    public enum ResponseType
    {
        DirectoryListing = 1,
        CrossSell = 2,
        Wizard = 3,
        FormProgram = 4
    }

    public enum MatchResponseType
    {
        SmartMatch = 1,
        SchoolSelection = 2,
        CrossSell = 3,
        Directory = 4
    }

    public static class MatchPersister
    {
        public static void PersistMatchRequest(BaseMatchRequest request,
                                               BaseMatchResponse response,
                                               MatchResult matchResults,
                                               ResponseType rt,
                                               string methodName,
                                               long elapsedProcessingTime,
                                               Guid? leadScoringGuid,
                                               GetInstitutionCampusOption? instGroupingType = null)
        {
            MatchResponse mr = new MatchResponse();
            EDDY.IS.Core.Logging.PerformanceLog log = new PerformanceLog(Base.ISApplication.MatchingEngine, "PersistMatchRequest", request.Application, request);
            try
            {
                if (!StaticSettings.IsBeta && MatchResponseLoggingSettings.LogMatchResponse)
                {
                    if (matchResults.ChosenBusinessModel != null && matchResults.ChosenBusinessModel.ChoosenWeightSubjectId.HasValue)
                        mr.BusinessModelWeightSubjectId = matchResults.ChosenBusinessModel.ChoosenWeightSubjectId.Value;
                    else
                        mr.BusinessModelWeightSubjectId = 0;                  
           
                    mr.CreatedDate = DateTime.Now;
                    mr.MatchResponseGUID = response.MatchResponseGuid;
                    mr.ProcessRequestMilliseconds = (int)elapsedProcessingTime;
                    mr.ServerName = System.Environment.MachineName;
                    mr.LeadScoringGuid = leadScoringGuid;
                    mr.RequestMethodName = methodName;
                    mr.RequestType = (int)rt;

                    if (matchResults.ChosenBusinessModel != null)
                    {
                        mr.DistanceCliffValue = matchResults.ChosenBusinessModel.DistanceCliffValue;
                        mr.DistanceCapMultiplier = matchResults.ChosenBusinessModel.DistanceCapMultiplier;
                    }

                    mr.InstitutionGroupingType = (int?)instGroupingType;

                    if (methodName != "GetPrograms")
                        mr.IsRollup = true;

                    Campaign campaign = Campaign.Get(request.TrackGuid);

                    if (request.TrackingDeviceGuid.HasValue)
                        mr.TrackingDeviceGUID = request.TrackingDeviceGuid.Value;

                    if(request.ProspectInput != null)
                    {
                        if (GeoCodeProcessor.IsValidZipCode(request.ProspectInput.PostalCode))
                            mr.ProspectZipAvailable = true;
                        else
                            mr.ProspectZipAvailable = false;
                    }

                    log.StartLogDetail("Retrieve Request Input");
                    if (LogRequest(methodName) && MatchResponseLoggingSettings.LogRequestInput)
                    {
                        string requestInputJSON = request.ToJSON();
                        mr.RequestInput = requestInputJSON.Substring(0, requestInputJSON.Length > 4000 ? 4000 : requestInputJSON.Length);
                    }
                    log.EndLogDetail();

                    log.StartLogDetail("Save Match Response");
                    MatchResponseDataService.SaveMatchResponse(mr);
                    log.EndLogDetail();

                    log.StartLogDetail("Create Match Displayed");
                    if (response != null && LogRequest(methodName) && MatchResponseLoggingSettings.LogDisplayedEntities)
                    {
                        int? logRandomDisplayedPercentage = MatchResponseLoggingSettings.LogRandomDisplayedPercentage;
                        bool shouldLog = true;

                        if (methodName != "GetWizardMatches" && methodName != "GetProgramsForCrossSell")
                        {
                            if (logRandomDisplayedPercentage.HasValue)
                            {
                                Random rand = new Random();
                                int startRandRange = 1;

                                int diceRoll = rand.Next(startRandRange, 100);

                                if (diceRoll > logRandomDisplayedPercentage.Value)
                                    shouldLog = false;
                            }
                        }

                        if (shouldLog)
                        {
                            AddDisplayedEntities(mr, response, matchResults);

                            if (matchResults.ChosenBusinessModel != null && matchResults.ChosenBusinessModel.FactorAggregateList != null)
                                mr.MatchResponseFactorAggregates = GetFactorAggregates(matchResults.ChosenBusinessModel.FactorAggregateList);
                        }
                    }
                    log.EndLogDetail();

                    //log.StartLogDetail("Create LogAvailablePrograms/RemovalReasons");
                    //if (response is WizardMatchResponse)
                    //{
                    //    if (MatchResponseLoggingSettings.LogAvailablePrograms)
                    //        AddAvailablePrograms(mr, (WizardMatchResponse)response, matchResults, (WizardMatchRequest)request);
                    //    if (MatchResponseLoggingSettings.LogRemovalReasons)
                    //        AddRemovalReasons(mr, matchResults, request);
                    //}
                    //else if (response is CrossSellProgramResponse)
                    //{
                    //    if (MatchResponseLoggingSettings.LogAvailablePrograms)
                    //        AddAvailablePrograms(mr, (CrossSellProgramResponse)response, matchResults, (CrossSellMatchRequest)request);
                    //    if (MatchResponseLoggingSettings.LogRemovalReasons)
                    //        AddRemovalReasons(mr, matchResults, request);
                    //}
                    //log.EndLogDetail();

                    //if (LogRequest(methodName) && MatchResponseLoggingSettings.LogDiscardedEntities)
                    //    AddDiscardedEntities(mr, matchResults.RulesResult);
                    log.StartLogDetail("Create MatchResponseSearch");
                    if ((response is WizardMatchResponse || response is NeoResponse) && MatchResponseLoggingSettings.LogSearches)
                    {
                        AddSearch(mr, response, matchResults, request);
                    }
                    log.EndLogDetail();

                    //log.StartLogDetail("Save AvailablePrograms");
                    //if (mr.MatchResponseAvailablePrograms.Count > 0)
                    //    MatchResponseDataService.SaveMatchResponseAvailablePrograms(mr.MatchResponseAvailablePrograms);
                    //log.EndLogDetail();

                    //log.StartLogDetail("Save RemovalReasonJsons");
                    //if (mr.MatchResponseRemovalReasonJsons.Count > 0)
                    //    MatchResponseDataService.SaveMatchResponseRemovalReasonJsons(mr.MatchResponseRemovalReasonJsons);
                    //log.EndLogDetail();

                    //log.StartLogDetail("Save RemovalReasons");
                    //if (mr.MatchResponseRemovalReasons.Count > 0)
                    //    MatchResponseDataService.SaveMatchResponseRemovalReasons(mr.MatchResponseRemovalReasons);
                    //log.EndLogDetail();

                    log.StartLogDetail("Save Search");
                    if (mr.MatchResponseSearches.Count > 0)
                        MatchResponseDataService.SaveMatchResponseSearch(mr.MatchResponseSearches);
                    log.EndLogDetail();

                    log.StartLogDetail("Save MatchResponseDisplayed");
                    if (mr.MatchResponseDisplayeds.Count > 0)
                        MatchResponseDataService.SaveMatchResponseDisplayed(mr.MatchResponseDisplayeds);
                    log.EndLogDetail();
                }
            }
            catch (Exception ex) 
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            log.EndLog(null);
        }

        private static void AddSearch(MatchResponse mr, BaseMatchResponse bmr, MatchResult matchResults, BaseMatchRequest request)
        {
            if (bmr is WizardMatchResponse)
                AddWizardSearch(mr, (WizardMatchResponse)bmr, matchResults, (WizardMatchRequest)request);
            else if (bmr is NeoResponse)
                AddNeoSearch(mr, (NeoResponse)bmr, matchResults, (NeoMatchRequest)request);
        }


        private static void AddWizardSearch(MatchResponse mr, WizardMatchResponse wmr, MatchResult matchResult, WizardMatchRequest request)
        {
            HashSet<int> vendorsAllowed = StaticCacheProxyHost.CacheProxy.Get<HashSet<int>>(MatchingCacheItem.MatchResponseSearchVendorAllowed);

            MatchResponseSearch mrs = CreateWizardSearchRequest(request);
            mrs.MatchResponseId = mr.MatchResponseId;
            List<CampusWithInstitution> availCampuses = FindAvailableWizardCampus(wmr, matchResult);
            mrs.ReturnedSchools = (wmr.SmartMatchList != null ? wmr.SmartMatchList.Count : 0) + 
                                  (wmr.SchoolSelectionList != null ? wmr.SchoolSelectionList.Count : 0) +
                                  (wmr.ThirdPartyMatchList != null ? wmr.ThirdPartyMatchList.Count : 0);

            mrs.AvailableSchools = availCampuses.Count;

            mrs.RemovedSchools = RemovedWizardSchools(wmr, availCampuses, matchResult);
            AddWizardSearchResults(ref mrs, wmr, availCampuses);

            if (request.Application == Base.ISApplication.FormsEngine)
            {                
                AddMatchSearchRemovals(ref mrs, matchResult);
            }
            else if (request.Application == Base.ISApplication.VendorAPI)
            {
                Campaign c = Campaign.Get(request.TrackGuid);

                if(vendorsAllowed != null && c.VendorId.HasValue && vendorsAllowed.Contains(c.VendorId.Value))
                    AddMatchSearchRemovals(ref mrs, matchResult);
            }

            mr.MatchResponseSearches.Add(mrs);
        }

        private static int RemovedWizardSchools(WizardMatchResponse wmr, List<CampusWithInstitution> availCampuses, MatchResult mr)
        {
            HashSet<int> availSchools = new HashSet<int>();
            availSchools.UnionWith(availCampuses.Select(a => a.InstitutionId.Value));

            if (wmr.SmartMatchList != null)
                availSchools.UnionWith(wmr.SmartMatchList.Select(sm => sm.InstitutionId));

            if (wmr.SchoolSelectionList != null)
                availSchools.UnionWith(wmr.SchoolSelectionList.Select(ss => ss.InstitutionId.Value));

            if (wmr.ThirdPartyMatchList != null)
                availSchools.UnionWith(wmr.ThirdPartyMatchList.Select(tp => tp.InstitutionId));

            HashSet<int> removedSchools = new HashSet<int>();
            if (mr.RulesResult != null && mr.RulesResult.NoMatchOutput != null)
                removedSchools.UnionWith(mr.RulesResult.NoMatchOutput.Select(n => n.InstitutionId.Value));

            removedSchools.ExceptWith(availSchools);

            return removedSchools.Count;
        }

        private static List<CampusWithInstitution> FindAvailableWizardCampus(WizardMatchResponse wmr, MatchResult matchResult)
        {
            List<CampusWithInstitution> campuses = new List<CampusWithInstitution>();
            HashSet<int> matchedInstitutions = new HashSet<int>();

            if (wmr.SmartMatchList != null)
                matchedInstitutions.UnionWith(wmr.SmartMatchList.Select(sm => sm.InstitutionId));

            if (wmr.SchoolSelectionList != null)
                matchedInstitutions.UnionWith(wmr.SchoolSelectionList.Select(ss => ss.InstitutionId.Value));

            if (wmr.ThirdPartyMatchList != null)
                matchedInstitutions.UnionWith(wmr.ThirdPartyMatchList.Select(tp => tp.InstitutionId));

            var mCampuses = matchResult.MatchItemList.Where(m => m.FailedValidation == false &&
                                                                !matchedInstitutions.Contains(m.Match.InstitutionId))
                                .GroupBy(g => new { g.Match.InstitutionId });

            foreach (var m in mCampuses)
            {
                CampusWithInstitution nc = new CampusWithInstitution();
                MatchItem mi = m.FirstOrDefault();
                nc.CampusId = mi.Match.CampusId;
                nc.InstitutionId = mi.Match.InstitutionId;
                nc.IsLiveTransfer = Product.IsWarmTransferProduct(mi.Match.ProductId);
                campuses.Add(nc);
            }
            return campuses;
        }

        private static void AddWizardSearchResults(ref MatchResponseSearch mrs, WizardMatchResponse wmr, List<CampusWithInstitution> campuses)
        {
            int position = 1;

            if (wmr.SmartMatchList != null)
            {
                foreach (var m in wmr.SmartMatchList)
                {
                    MatchResponseSearchResult mrsr = new MatchResponseSearchResult();

                    mrsr.CampusId = m.CampusId;
                    mrsr.InstitutionId = m.InstitutionId;
                    mrsr.IsLT = false;
                    mrsr.Position = position++;
                    mrsr.Displayed = true;

                    mrs.MatchResponseSearchResults.Add(mrsr);
                }
            }

            if (wmr.SchoolSelectionList != null)
            {
                foreach (var m in wmr.SchoolSelectionList)
                {
                    MatchResponseSearchResult mrsr = new MatchResponseSearchResult();

                    mrsr.CampusId = m.CampusId;
                    mrsr.InstitutionId = m.InstitutionId.HasValue ? m.InstitutionId.Value : 0;
                    mrsr.IsLT = m.IsLiveTransfer;
                    mrsr.Position = position++;
                    mrsr.Displayed = true;

                    mrs.MatchResponseSearchResults.Add(mrsr);
                }
            }

            if (wmr.ThirdPartyMatchList != null)
            {
                foreach (var m in wmr.ThirdPartyMatchList)
                {
                    MatchResponseSearchResult mrsr = new MatchResponseSearchResult();

                    mrsr.CampusId = m.CampusId;
                    mrsr.InstitutionId = m.InstitutionId;
                    mrsr.IsLT = false;
                    mrsr.Position = position++;
                    mrsr.Displayed = true;

                    mrs.MatchResponseSearchResults.Add(mrsr);
                }
            }

            foreach (var m in campuses)
            {
                MatchResponseSearchResult mrsr = new MatchResponseSearchResult();

                mrsr.CampusId = m.CampusId;
                mrsr.InstitutionId = m.InstitutionId.Value;
                mrsr.IsLT = m.IsLiveTransfer;
                mrsr.Position = 0;
                mrsr.Displayed = false;

                mrs.MatchResponseSearchResults.Add(mrsr);
            }
        }

        private static void AddMatchSearchRemovals(ref MatchResponseSearch mrs, MatchResult mr)
        {
            if (mr.RulesResult != null && mr.RulesResult.NoMatchOutput != null)
            {
                foreach (var nm in mr.RulesResult.NoMatchOutput)
                {
                    mrs.MatchResponseSearchRemovals.Add(CreateMatchSearchRemoval(nm));
                }
            }
        }

        private static MatchResponseSearchRemoval CreateMatchSearchRemoval(NoMatch nm)
        {
            MatchResponseSearchRemoval mrsr = new MatchResponseSearchRemoval();
            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

            mrsr.BaseRuleType = Enum.GetName(typeof(BaseRuleType), nm.RuleType);
            
            if (nm.RuleId.HasValue)
                mrsr.RuleId = nm.RuleId.Value;
            else
                mrsr.RuleId = 0;

            if (!String.IsNullOrWhiteSpace(nm.RuleName))
                mrsr.RuleName = nm.RuleName.Substring(0, nm.RuleName.Length > 100? 100 : nm.RuleName.Length);
            else
                mrsr.RuleName = "";

            switch (nm.entityType)
            {
                case EntityMeta.ClientRelationProductMapping:
                    mrsr.ClientRelationProductMappingId = nm.entityId;
                    break;
                case EntityMeta.ClientCampusProductMapping:
                    mrsr.ClientCampusProductMappingId = nm.entityId;
                    break;
                case EntityMeta.PSI:
                    mrsr.PsiId = nm.entityId;
                    break;
                case EntityMeta.ProgramProduct:
                    mrsr.ProgramProductId = nm.entityId;
                    break;
                case EntityMeta.Program:
                    mrsr.ProgramId = nm.entityId;
                    break;
            }

            if (nm.InstitutionId.HasValue)
                mrsr.InstitutionId = nm.InstitutionId.Value;
            else
                mrsr.InstitutionId = 0;

            if (nm.ClientRelationshipId.HasValue)
                mrsr.ClientRelationshipId = nm.ClientRelationshipId.Value;
            else
                mrsr.ClientRelationshipId = 0;

            return mrsr;
        }
        private static MatchResponseSearch CreateWizardSearchRequest(WizardMatchRequest request)
        {
            MatchResponseSearch mrs = new MatchResponseSearch();

            if (request.Application.HasValue)
                mrs.CallingApplication = Enum.GetName(typeof(Base.ISApplication), request.Application.Value);
            else
                mrs.CallingApplication = Enum.GetName(typeof(Base.ISApplication), Base.ISApplication.FormsEngine);

            if(request.ApplicationId > 0)
                mrs.ApplicationId = request.ApplicationId;

            if(request.CampusType.HasValue)
                mrs.CampusTypeId = (int)request.CampusType.Value;

            mrs.LeadCreationTypeId = (int)request.LeadCreationType;

            if (request.SmartMatchedInstituionIdList != null && request.SmartMatchedInstituionIdList.Count > 0)
                mrs.SmartMatchedInstitutionList = string.Join(",", request.SmartMatchedInstituionIdList);

            mrs.TrackId = request.TrackGuid;
            mrs.DialerKey = request.DialerKey;
            mrs.TSR = request.TSR;

            if (request.SubjectList != null && request.SubjectList.Count > 0)
                mrs.SubjectList = string.Join(",", request.SubjectList);
            
            if(request.CategoryList != null && request.CategoryList.Count > 0)
                mrs.CategoryList = string.Join(",", request.CategoryList);

            if (request.SpecialtyList != null && request.SpecialtyList.Count > 0)
                mrs.SpecialtyList = string.Join(",", request.SpecialtyList);

            AddProspectInput(request.ProspectInput, ref mrs);
            return mrs;
        }

        private static MatchResponseSearch CreateNeoSearchRequest(NeoMatchRequest request)
        {
            MatchResponseSearch mrs = new MatchResponseSearch();

            if (request.Application.HasValue)
                mrs.CallingApplication = Enum.GetName(typeof(Base.ISApplication), request.Application.Value);

            if (request.ApplicationId > 0)
                mrs.ApplicationId = request.ApplicationId;

            if (request.CampusType.HasValue)
                mrs.CampusTypeId = (int)request.CampusType.Value;

            mrs.LeadCreationTypeId = (int)LeadCreationType.Advising;

            mrs.TrackId = request.TrackGuid;
            mrs.DialerKey = request.DialerKey;
            mrs.TSR = request.TSR;

            if (request.SubjectList != null && request.SubjectList.Count > 0)
                mrs.SubjectList = string.Join(",", request.SubjectList);

            if (request.CategoryList != null && request.CategoryList.Count > 0)
                mrs.CategoryList = string.Join(",", request.CategoryList);

            if (request.SpecialtyList != null && request.SpecialtyList.Count > 0)
                mrs.SpecialtyList = string.Join(",", request.SpecialtyList);

            AddProspectInput(request.ProspectInput, ref mrs);
            return mrs;
        }

        private static void AddProspectInput(DTO.ProspectInput pi, ref MatchResponseSearch mrs)
        {
            if (pi != null)
            {
                mrs.Age = pi.Age;
                mrs.City = pi.City;
                mrs.CountryId = pi.CountryId;
                mrs.EducationLevelId = pi.EducationLevelId;
                mrs.Email = pi.Email;
                mrs.ExternalLeadId = pi.ExternalLeadId;
                mrs.FirstName = pi.FirstName;
                mrs.HSGraduationYear = pi.HSGraduationYear;
                mrs.IsCitzen = pi.IsCitizen;
                mrs.IsMilitary = pi.IsMilitary;
                mrs.LastName = pi.LastName;
                mrs.MilitaryStatusId = pi.MilitaryStatusId;
                mrs.Phone1 = pi.Phone1;
                mrs.PostalCode = pi.PostalCode;
                mrs.StateId = pi.StateId;
                mrs.StreetAddress = pi.StreetAddress;

                if (pi.KVCodeData != null)
                {
                    foreach (var kv in pi.KVCodeData)
                    {
                        MatchResponseSearchKVData kvdata = new MatchResponseSearchKVData();

                        kvdata.Key = kv.Key;
                        kvdata.KVCodeDataId = kv.Value;

                        mrs.MatchResponseSearchKVDatas.Add(kvdata);
                    }
                }
            }
        }

        private static void AddNeoSearch(MatchResponse mr, NeoResponse nr, MatchResult matchResult, NeoMatchRequest nmr)
        {
            MatchResponseSearch mrs = CreateNeoSearchRequest(nmr);
            mrs.MatchResponseId = mr.MatchResponseId;
            List<NeoCampus> availCampuses = FindAvailableNeoCampus(nr, matchResult, nmr);
            mrs.ReturnedSchools = (nr.FormList != null ? nr.FormList.Count : 0) + (nr.LiveTransferList != null ? nr.LiveTransferList.Count : 0);
            mrs.AvailableSchools = availCampuses.Count;
            mrs.RemovedSchools = RemovedNeoSchools(nr, availCampuses, matchResult);
            AddMatchSearchRemovals(ref mrs, matchResult);
            AddNeoSearchResults(ref mrs, nr, availCampuses);
            mr.MatchResponseSearches.Add(mrs);
        }

        private static int RemovedNeoSchools(NeoResponse nr, List<NeoCampus> availCampuses, MatchResult mr)
        {
            HashSet<int> availSchools = new HashSet<int>();
            availSchools.UnionWith(availCampuses.Select(a => a.InstitutionId.Value));

            if (nr.FormList != null)
                availSchools.UnionWith(nr.FormList.Select(f => f.InstitutionId.Value));

            if (nr.LiveTransferList != null)
                availSchools.UnionWith(nr.LiveTransferList.Select(l => l.InstitutionId.Value));

            HashSet<int> removedSchools = new HashSet<int>();
            if(mr.RulesResult != null && mr.RulesResult.NoMatchOutput != null)
                removedSchools.UnionWith(mr.RulesResult.NoMatchOutput.Select(n => n.InstitutionId.Value));

            removedSchools.ExceptWith(availSchools);

            return removedSchools.Count;
        }

        private static List<NeoCampus> FindAvailableNeoCampus(NeoResponse nr, MatchResult matchResult, NeoMatchRequest request)
        {
            GeoCodeProcessor gcp = new GeoCodeProcessor();
            List<NeoCampus> campuses = new List<NeoCampus>();
            HashSet<int> matchedInstitutions = new HashSet<int>();

            if (nr.FormList != null)
                matchedInstitutions.UnionWith(nr.FormList.Select(f => f.InstitutionId.Value));

            if (nr.LiveTransferList != null)
                matchedInstitutions.UnionWith(nr.LiveTransferList.Select(l => l.InstitutionId.Value));

            var mCampuses = matchResult.MatchItemList.Where(m => m.FailedValidation == false &&
                                                                 m.Match.PaidStatusTypeId == PaidStatusType.Paid && 
                                                                 m.Match.InquiryDisabled == false &&
                                                                !matchedInstitutions.Contains(m.Match.InstitutionId))
                                .GroupBy(g => new { g.Match.InstitutionId });

            foreach (var m in mCampuses)
            {
                NeoCampus nc = new NeoCampus();
                MatchItem mi = m.FirstOrDefault();
                nc.CampusId = mi.Match.CampusId;
                nc.InstitutionId = mi.Match.InstitutionId;
                nc.IsLiveTransfer = Product.IsWarmTransferProduct(mi.Match.ProductId);
                nc.CampusType = (CampusType)mi.Match.CampusCampusTypeId;

                if(request.ProspectInput != null && mi.Match.CampusCampusTypeId == 2)
                    nc.DistanceFromProspect = gcp.GetDistanceBetweenZipCodes(mi.Match.CampusPostalCode, request.ProspectInput.PostalCode);

                if (nc.CampusType == CampusType.Online || nc.DistanceFromProspect < 100)
                    campuses.Add(nc);
            }
            return campuses;
        }

        private static void AddNeoSearchResults(ref MatchResponseSearch mrs, NeoResponse nr, List<NeoCampus> campuses)
        {
            int position = 1;

            if (nr.LiveTransferList != null)
            {
                foreach (var m in nr.LiveTransferList)
                {
                    MatchResponseSearchResult mrsr = new MatchResponseSearchResult();

                    mrsr.CampusId = m.CampusId;
                    mrsr.InstitutionId = m.InstitutionId.HasValue ? m.InstitutionId.Value : 0;
                    mrsr.IsLT = true;
                    mrsr.Position = position++;
                    mrsr.Displayed = true;

                    mrs.MatchResponseSearchResults.Add(mrsr);
                }
            }

            if (nr.FormList != null)
            {
                foreach (var m in nr.FormList)
                {
                    MatchResponseSearchResult mrsr = new MatchResponseSearchResult();

                    mrsr.CampusId = m.CampusId;
                    mrsr.InstitutionId = m.InstitutionId.HasValue ? m.InstitutionId.Value : 0;
                    mrsr.IsLT = false;
                    mrsr.Position = position++;
                    mrsr.Displayed = true;

                    mrs.MatchResponseSearchResults.Add(mrsr);
                }
            }

            foreach (var m in campuses)
            {
                MatchResponseSearchResult mrsr = new MatchResponseSearchResult();

                mrsr.CampusId = m.CampusId;
                mrsr.InstitutionId = m.InstitutionId.Value;
                mrsr.IsLT = m.IsLiveTransfer;
                mrsr.Position = 0;
                mrsr.Displayed = false;

                mrs.MatchResponseSearchResults.Add(mrsr);
            }
        }

        private static bool LogRequest(string methodName)
        {
            if (ConfigurationManager.AppSettings["MatchResponseLogging_" + methodName] != null)
            {
                if (ConfigurationManager.AppSettings["MatchResponseLogging_" + methodName] == "1")
                    return true;
                else
                    return false;
            }

            return false;
        }

        private static void AddDisplayedEntities(MatchResponse mr, BaseMatchResponse bmr, MatchResult matchResults)
        {
            if (bmr is CampusResponse)
                AddDisplayedEntities(mr, (CampusResponse)bmr, matchResults);
            else if (bmr is CrossSellProgramResponse)
                AddDisplayedEntities(mr, (CrossSellProgramResponse)bmr, matchResults);
            else if (bmr is ProgramResponse)
                AddDisplayedEntities(mr, (ProgramResponse)bmr, matchResults);
            else if (bmr is InstitutionResponse)
                AddDisplayedEntities(mr, (InstitutionResponse)bmr, matchResults);
            else if (bmr is WizardMatchResponse)
                AddDisplayedEntities(mr, (WizardMatchResponse)bmr, matchResults);
            else if (bmr is ApolloCampusResponse)
                AddDisplayedEntities(mr, (ApolloCampusResponse)bmr, matchResults);
        }

        private static List<MatchResponseFactorAggregate> GetFactorAggregates(List<FactorAggregate> factorAggregateList)
        {
            List<MatchResponseFactorAggregate> mrFaList = new List<MatchResponseFactorAggregate>();

            foreach(FactorAggregate fa in factorAggregateList)
            {
                List<MatchResponseFactorAggregateValue> mrFaValueList = new List<MatchResponseFactorAggregateValue>();

                foreach(FactorAggregateValue fav in fa.FactorAggregateValueList)
                {
                    mrFaValueList.Add(new MatchResponseFactorAggregateValue() { BusinessModelFactorId = (int)fav.BusinessModelFactorId, AggregateValue = fav.AggregateValue });
                }

                mrFaList.Add(new MatchResponseFactorAggregate() { AggregateValue = fa.AggregateValue, BusinessModelFactorId = (int)fa.BusinessModelFactorId, MatchResponseFactorAggregateTypeId = (int)fa.BusinessModelAggregateTypeId, ProgramProductId = fa.ProgramProductId, MatchResponseFactorAggregateValues = mrFaValueList });
            }

            return mrFaList;
        }

        private static List<MatchResponseDisplayedFactorScore> ProgramFactorScores(int programProductId, MatchResult mr)
        {
            List<MatchResponseDisplayedFactorScore> factorScores = new List<MatchResponseDisplayedFactorScore>();

            if (MatchResponseLoggingSettings.LogDisplayedFactorScores)
            {
                MatchItem mi = (from m in mr.MatchItemList
                                where m.Match.ProgramProductId == programProductId
                                select m).FirstOrDefault();

                if (mi != null && mi.FactorScores != null)
                {
                    foreach (MatchItemFactorScore fs in mi.FactorScores)
                    {
                        if (fs != default(MatchItemFactorScore))
                        {
                            MatchResponseDisplayedFactorScore fsdto = new MatchResponseDisplayedFactorScore();
                            fsdto.BusinessModelFactorId = (int)fs.BusinessModelFactorId;
                            fsdto.FactorScore = fs.FactorScore;
                            fsdto.FactorValue = fs.FactorValue;

                            factorScores.Add(fsdto);
                        }
                    }
                }
            }

            return factorScores;
        }

        private static MatchResponseDisplayed CreateMatchResponseDisplayed(Campus campus, MatchResult matchResults, ref int index)
        {
            MatchResponseDisplayed c = new MatchResponseDisplayed();
            c.CollectionIndex = index++;
            c.EntityId = campus.CampusId;
            c.EntityMetaId = (int)EntityMeta.Campus;
            c.SRAScore = campus.ProgramRankScore;

            int programIndex = 1;
            c.ChildMatchResponseDisplayed = new List<MatchResponseDisplayed>();
            c.MatchResponseTypeId = (int)MatchResponseType.SchoolSelection;
            foreach (Program program in campus.ProgramList)
            {
                MatchResponseDisplayed p = new MatchResponseDisplayed();
                p.CollectionIndex = programIndex++;
                p.EntityMetaId = (int)EntityMeta.ProgramProduct;
                p.EntityId = program.ProgramProductId;
                p.SRAScore = program.ProgramRankScore;
                p.MatchResponseDisplayedFactorScores = ProgramFactorScores(p.EntityId, matchResults);
                p.MatchResponseTypeId = (int)MatchResponseType.SchoolSelection;
                c.ChildMatchResponseDisplayed.Add(p);
            }

            return c;
        }

        private static void AddDisplayedEntities(MatchResponse mr, ApolloCampusResponse acr, MatchResult matchResults)
        {
            mr.MatchResponseDisplayeds = new List<MatchResponseDisplayed>();

            int index = 1;
            if (acr.ProductGroupResponseList != null)
            {
                foreach (ApolloProductGroupResponse apgr in acr.ProductGroupResponseList)
                {
                    foreach (ApolloCampus campus in apgr.OnlineCampusList)
                    {
                        MatchResponseDisplayed c = CreateMatchResponseDisplayed(campus, matchResults, ref index);
                        c.MatchResponseId = mr.MatchResponseId;
                        mr.MatchResponseDisplayeds.Add(c);
                    }

                    foreach (ApolloCampus campus in apgr.GroundCampusList)
                    {
                        MatchResponseDisplayed c = CreateMatchResponseDisplayed(campus, matchResults, ref index);
                        c.MatchResponseId = mr.MatchResponseId;
                        mr.MatchResponseDisplayeds.Add(c);
                    }
                }
            }
        }


        private static void AddDisplayedEntities(MatchResponse mr, WizardMatchResponse wr, MatchResult matchResults)
        {
            mr.MatchResponseDisplayeds = new List<MatchResponseDisplayed>();

            int index = 1;
            if (wr.SchoolSelectionList != null)
            {
                foreach (CampusWithInstitution campus in wr.SchoolSelectionList)
                {
                    MatchResponseDisplayed c = CreateMatchResponseDisplayed(campus, matchResults, ref index);
                    c.MatchResponseId = mr.MatchResponseId;
                    mr.MatchResponseDisplayeds.Add(c);
                }
            }

            if (wr.SmartMatchList != null)
            {
                index = 1;
                foreach (ProgramWithInstitutionCampus program in wr.SmartMatchList)
                {
                    MatchResponseDisplayed c = new MatchResponseDisplayed();
                    c.CollectionIndex = index++;
                    c.EntityId = program.ProgramProductId;
                    c.EntityMetaId = (int)EntityMeta.ProgramProduct;
                    c.SRAScore = program.ProgramRankScore;
                    c.MatchResponseDisplayedFactorScores = ProgramFactorScores(c.EntityId, matchResults);
                    c.MatchResponseTypeId = (int)MatchResponseType.SmartMatch;
                    c.MatchResponseId = mr.MatchResponseId;
                    mr.MatchResponseDisplayeds.Add(c);
                }
            }
        }

        private static void AddDisplayedEntities(MatchResponse mr, CampusResponse cr, MatchResult matchResults)
        {
            mr.MatchResponseDisplayeds = new List<MatchResponseDisplayed>();

            int index = 1;
            foreach (CampusWithInstitution campus in cr.CampusList)
            {
                MatchResponseDisplayed c = CreateMatchResponseDisplayed(campus, matchResults, ref index);
                c.MatchResponseId = mr.MatchResponseId;
                mr.MatchResponseDisplayeds.Add(c);
            }
        }

        private static void AddDisplayedEntities(MatchResponse mr, CrossSellProgramResponse cr, MatchResult matchResults)
        {
            mr.MatchResponseDisplayeds = new List<MatchResponseDisplayed>();

            int index = 1;
            foreach (ProgramWithInstitutionCampus program in cr.ProgramList)
            {
                MatchResponseDisplayed c = new MatchResponseDisplayed();
                c.CollectionIndex = index++;
                c.EntityId = program.ProgramProductId;
                c.EntityMetaId = (int)EntityMeta.ProgramProduct;
                c.SRAScore = program.ProgramRankScore;
                c.MatchResponseDisplayedFactorScores = ProgramFactorScores(c.EntityId, matchResults);
                c.MatchResponseTypeId = (int)MatchResponseType.CrossSell;
                c.MatchResponseId = mr.MatchResponseId;
                mr.MatchResponseDisplayeds.Add(c);
            }
        }

        private static void AddDisplayedEntities(MatchResponse mr, ProgramResponse cr, MatchResult matchResults)
        {
            mr.MatchResponseDisplayeds = new List<MatchResponseDisplayed>();

            int index = 1;
            foreach (Program program in cr.ProgramList)
            {
                MatchResponseDisplayed c = new MatchResponseDisplayed();
                c.CollectionIndex = index++;
                c.EntityId = program.ProgramProductId;
                c.EntityMetaId = (int)EntityMeta.ProgramProduct;
                c.SRAScore = program.ProgramRankScore;
                c.MatchResponseDisplayedFactorScores = ProgramFactorScores(c.EntityId, matchResults);
                c.MatchResponseTypeId = (int)MatchResponseType.Directory;
                c.MatchResponseId = mr.MatchResponseId;
                mr.MatchResponseDisplayeds.Add(c);
            }
        }

        private static void AddDisplayedEntities(MatchResponse mr, InstitutionResponse cr, MatchResult matchResults)
        {
            mr.MatchResponseDisplayeds = new List<MatchResponseDisplayed>();

            int index = 1;
            foreach (Institution school in cr.InstitutionList)
            {
                MatchResponseDisplayed s = new MatchResponseDisplayed();
                s.CollectionIndex = index++;
                s.EntityId = school.InstitutionId;
                s.EntityMetaId = (int)EntityMeta.Institution;
                s.SRAScore = school.ProgramRankScore;
                s.MatchResponseTypeId = (int)MatchResponseType.Directory;
                s.MatchResponseId = mr.MatchResponseId;
                if (school is InstitutionWithCampus)
                {
                    int campusIndex = 1;
                    s.ChildMatchResponseDisplayed = new List<MatchResponseDisplayed>();

                    foreach (Campus campus in ((InstitutionWithCampus)school).CampusList)
                    {
                        MatchResponseDisplayed c = new MatchResponseDisplayed();
                        c.CollectionIndex = campusIndex++;
                        c.EntityId = campus.CampusId;
                        c.EntityMetaId = (int)EntityMeta.Campus;
                        c.SRAScore = campus.ProgramRankScore;

                        int programIndex = 1;
                        c.ChildMatchResponseDisplayed = new List<MatchResponseDisplayed>();
                        c.MatchResponseTypeId = (int)MatchResponseType.Directory;

                        foreach (Program program in campus.ProgramList)
                        {
                            MatchResponseDisplayed p = new MatchResponseDisplayed();
                            p.CollectionIndex = programIndex++;
                            p.EntityMetaId = (int)EntityMeta.ProgramProduct;
                            p.EntityId = program.ProgramProductId;
                            p.SRAScore = program.ProgramRankScore;
                            p.MatchResponseDisplayedFactorScores = ProgramFactorScores(p.EntityId, matchResults);
                            p.MatchResponseTypeId = (int)MatchResponseType.Directory;
                            c.ChildMatchResponseDisplayed.Add(p);
                        }

                        s.ChildMatchResponseDisplayed.Add(c);
                    }
                }
                else if (school is InstitutionWithProgram)
                {
                    if (school is InstitutionWithProgram)
                    {
                        int programIndex = 1;
                        s.ChildMatchResponseDisplayed = new List<MatchResponseDisplayed>();

                        foreach (Program program in ((InstitutionWithProgram)school).ProgramList)
                        {
                            MatchResponseDisplayed p = new MatchResponseDisplayed();
                            p.MatchResponseTypeId = (int)MatchResponseType.Directory;
                            p.CollectionIndex = programIndex++;
                            p.EntityMetaId = (int)EntityMeta.ProgramProduct;
                            p.EntityId = program.ProgramProductId;
                            p.SRAScore = program.ProgramRankScore;
                            p.MatchResponseDisplayedFactorScores = new List<MatchResponseDisplayedFactorScore>(ProgramFactorScores(p.EntityId, matchResults));
                            s.ChildMatchResponseDisplayed.Add(p);

                            if(program.CampusSlimList != null && program.CampusSlimList.Any())
                            {
                                int campusIndex = 1;

                                foreach(var campusSlim in program.CampusSlimList)
                                {
                                    MatchResponseDisplayed c = new MatchResponseDisplayed();
                                    c.CollectionIndex = campusIndex++;
                                    c.EntityId = campusSlim.CampusId;
                                    c.EntityMetaId = (int)EntityMeta.Campus;
                                    c.SRAScore = 0;                                                                       
                                    c.MatchResponseTypeId = (int)MatchResponseType.Directory;
                                    p.ChildMatchResponseDisplayed.Add(c);
                                }
                            }
                        }
                    }
                }

                mr.MatchResponseDisplayeds.Add(s);
            }
        }

        private static bool IsTestEmail(DTO.ProspectInput pi)
        {
            if (pi != null && !String.IsNullOrEmpty(pi.Email) && pi.Email.EndsWith("@test.com", StringComparison.OrdinalIgnoreCase))
                return true;
            else
                return false;
        }

        private static bool IsMatch1Plus(WizardMatchResponse wr)
        {
            if (wr.SmartMatchList != null && wr.SmartMatchList.Count == 1 && wr.SmartMatchList[0].ProductId == (int)ProductType.Match1Plus)
                return true;
            else
                return false;
        }

        private static void AddAvailablePrograms(MatchResponse mr, WizardMatchResponse wr, MatchResult matchResults, WizardMatchRequest request)
        {
            if(request != null && wr != null && matchResults != null && matchResults.MatchItemList != null && matchResults.MatchItemList.Count > 0)
            {
                if(IsTestEmail(request.ProspectInput) || IsMatch1Plus(wr))
                {
                    int programIndex = 1;

                    foreach(var m in matchResults.MatchItemList)
                    {
                        MatchResponseAvailableProgram p = new MatchResponseAvailableProgram();
                        p.ProgramProductId = m.Match.ProgramProductId;
                        p.SRAScore = m.ProgramRankScore;
                        p.CollectionIndex = programIndex++;
                        p.MatchResponseTypeId = (int)MatchResponseType.SmartMatch;
                        p.MatchResponseAvailableProgramFactorScores = new List<MatchResponseAvailableProgramFactorScore>(AvailableProgramFactorScores(m));
                        mr.MatchResponseAvailablePrograms.Add(p);
                    }
                }
            }
        }

        private static void AddAvailablePrograms(MatchResponse mr, CrossSellProgramResponse cspr, MatchResult matchResults, CrossSellMatchRequest request)
        {
            if (request != null && cspr != null && matchResults != null && matchResults.MatchItemList != null && matchResults.MatchItemList.Count > 0)
            {
                if (IsTestEmail(request.ProspectInput))
                {
                    int programIndex = 1;

                    foreach (var m in matchResults.MatchItemList)
                    {
                        MatchResponseAvailableProgram p = new MatchResponseAvailableProgram();
                        p.ProgramProductId = m.Match.ProgramProductId;
                        p.SRAScore = m.ProgramRankScore;
                        p.CollectionIndex = programIndex++;
                        p.MatchResponseTypeId = (int)MatchResponseType.SmartMatch;
                        p.MatchResponseAvailableProgramFactorScores = new List<MatchResponseAvailableProgramFactorScore>(AvailableProgramFactorScores(m));
                        mr.MatchResponseAvailablePrograms.Add(p);
                    }
                }
            }
        }

        private static List<MatchResponseAvailableProgramFactorScore> AvailableProgramFactorScores(MatchItem mi)
        {
            List<MatchResponseAvailableProgramFactorScore> factorScores = new List<MatchResponseAvailableProgramFactorScore>();

            if (mi != null && mi.FactorScores != null)
            {
                foreach (MatchItemFactorScore fs in mi.FactorScores)
                {
                    if (fs != default(MatchItemFactorScore))
                    {
                        MatchResponseAvailableProgramFactorScore fsdto = new MatchResponseAvailableProgramFactorScore();
                        fsdto.BusinessModelFactorId = (int)fs.BusinessModelFactorId;
                        fsdto.FactorScore = fs.FactorScore;
                        fsdto.FactorValue = fs.FactorValue;

                        factorScores.Add(fsdto);
                    }
                }
            }

            return factorScores;
        }
        //private static void AddRemovalReasons(MatchResponse mr, MatchResult matchResults, BaseMatchRequest request)
        //{
        //    if (request != null && matchResults != null && matchResults.MatchItemList != null && matchResults.MatchItemList.Count > 0)
        //    {
        //        if (IsTestEmail(request.ProspectInput))
        //        {
        //            var reasons = matchResults.MatchItemList.Where(mi => mi.FailedValidation == true && mi.RemovalReason != null).Select(s => new { ProgramProductId = s.Match.ProgramProductId, RuleEntity = s.RemovalReason.RuleEntity, RuleEntityEntityId = s.RemovalReason.RuleEntityEntityId, RuleType = s.RemovalReason.RuleType, RuleDetail = s.RemovalReason.RuleDetail });

        //            ICollection<MatchResponseRemovalReason> removalReasonList = new List<MatchResponseRemovalReason>();

        //            foreach (var reason in reasons)
        //            {
        //                MatchResponseRemovalReason r = new MatchResponseRemovalReason();
        //                r.ProgramProductId = reason.ProgramProductId;
        //                r.EntityMetaId = (int)reason.RuleEntity;
        //                r.RuleEntityId = reason.RuleEntityEntityId;
        //                r.RuleTypeId = (int)reason.RuleType;
        //                r.RuleDetail = reason.RuleDetail;
        //                r.MatchResponseId = mr.MatchResponseId;
        //                removalReasonList.Add(r);
        //            }

        //            if (MatchResponseLoggingSettings.LogAsJson)
        //                mr.MatchResponseRemovalReasonJsons.Add(new MatchResponseRemovalReasonJson() { MatchResponseId = mr.MatchResponseId, JsonObject = JsonConvert.SerializeObject(removalReasonList).Replace(",\"MatchResponse\":null", "").Replace("\"MatchResponseRemovalReasonId\":0", "").Replace(",\"MatchResponseId\":0,", "") });
        //            else
        //                mr.MatchResponseRemovalReasons = removalReasonList;
        //        }
        //    }
        //}

        //private static void AddDiscardedEntities(MatchResponse mr, RulesResult rr)
        //{
        //    if (rr != null && rr.NoMatchOutput != null)
        //    {
        //        mr.MatchResponseDiscardedEntities = new List<MatchResponseDiscardedEntity>();
        //        foreach (NoMatch nm in rr.NoMatchOutput)
        //        {
        //            MatchResponseDiscardedEntity discard = new MatchResponseDiscardedEntity();
        //            discard.EntityId = nm.entityId;
        //            discard.EntityMetaId = (int)nm.entityType;
        //            discard.ProgramValidationBaseRuleId = (int)nm.RuleType;
        //            discard.ProgramValidationRuleId = nm.RuleId;
        //            if (nm.productId != 0)
        //                discard.ProductId = nm.productId;

        //            mr.MatchResponseDiscardedEntities.Add(discard);
        //        }
        //    }
        //}
    }
}
