using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.Base.Util;
using System.Diagnostics;
using EDDY.IS.MatchingEngine.Rules;
using EDDY.IS.Core.Logging.DataModel;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.Constants;
using System.Configuration;
using EDDY.IS.Core.Logging;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.Base;

namespace EDDY.IS.MatchingEngine
{
    public class RulesEngine : Engine
    {
        private bool IsCountryOnly = false;
        public HashSet<Type> excludedRules = new HashSet<Type>();

        RuleInput ruleInput;

        public RulesEngine(DTO.ProspectInput pi, Campaign campaign, IS.Base.ISApplication? app, bool isCountryOnly, EDDY.IS.Core.Logging.PerformanceLog pLog, bool ignoreCapRule = false, LeadCreationType? LeadCreationTypeID = null, List<int> desiredProgramLevelList = null, int? userId = null, bool skipWTRules = false)
            : this(pi, campaign, app, pLog, LeadCreationTypeID, desiredProgramLevelList, userId, skipWTRules)
        {
            IsCountryOnly = isCountryOnly;

            if (ignoreCapRule && ruleInput.RuleAttributes.ContainsKey(RuleAttribute.ExecuteCapRule))
                ruleInput.RuleAttributes[RuleAttribute.ExecuteCapRule] = false;
            else if (ignoreCapRule)
                ruleInput.RuleAttributes.Add(RuleAttribute.ExecuteCapRule, false);

            if (campaign.IgnoreGEORestrictions)
            {
                excludedRules.Add(typeof(Country));
                excludedRules.Add(typeof(State));
                excludedRules.Add(typeof(ZipCode));
            }

            if (campaign.IgnoreJornayaRule)
                excludedRules.Add(typeof(JournayaLeadId));
            
        }

        public RulesEngine(DTO.ProspectInput pi, Campaign campaign, ISApplication? app, EDDY.IS.Core.Logging.PerformanceLog pLog, LeadCreationType? LeadCreationTypeID = null, List<int> desiredProgramLevelList = null, int? userId = null, bool skipWTRules = false)
            : base(pLog)
        {
            ruleInput = new RuleInput();
            excludedRules = new HashSet<Type>();
            ruleInput.prospectData = new Prospect(pi);
            ruleInput.Campaign = campaign;
            ruleInput.DesiredProgramLevelList = desiredProgramLevelList;

            if(!skipWTRules)
                if (app.HasValue && (app.Value == ISApplication.Apollo || app.Value == ISApplication.VendorAPI ))
                    ruleInput.RuleAttributes.Add(RuleAttribute.ExecuteWarmTransferRules, true);

            if (LeadCreationTypeID.HasValue && LeadCreationTypeID.Value == LeadCreationType.WizardSmartMatch)
                ruleInput.RuleAttributes.Add(RuleAttribute.ExecuteSmartMatchRules, true);

            if (LeadCreationTypeID.HasValue)
                ruleInput.RuleAttributes.Add(RuleAttribute.ExecuteLeadSubmitRules, true);

            //EMS Application Campaign
            if (campaign != null && campaign.ApplicationId.HasValue && campaign.ApplicationId.Value == 27)
                ruleInput.RuleAttributes.Add(RuleAttribute.ExecuteEMSRules, true);

            if (pi != null && !String.IsNullOrEmpty(pi.PostalCode))
                ruleInput.ZipCoordinate = GeoCodeProcessor.GetZipCodeCoordinate(pi.PostalCode);

            if (!ruleInput.RuleAttributes.ContainsKey(RuleAttribute.ExecuteCapRule))
                ruleInput.RuleAttributes.Add(RuleAttribute.ExecuteCapRule, true);

            ruleInput.LeadCreationTypeID = LeadCreationTypeID;
            ruleInput.UserID = userId;

        }

        public static CampusZipCodeRuleResponse GetCampusZipCodeRules(int campusId, Guid campaignTrackId)
        {
            CampusZipCodeRuleResponse zipCodeRule = new CampusZipCodeRuleResponse();

            Campaign campaign = Campaign.Get(campaignTrackId);

            if (campaign != null)
            {
                MatchDatabase matchDb = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);
                Dictionary<int, List<ZipCodeInclusionExclusion>> campusZipCodeCache = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<ZipCodeInclusionExclusion>>>(MatchingCacheItem.CampusZipCode);
                Dictionary<int, VW_Matching_ClientCampusProductMappingCache> crCampusList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_ClientCampusProductMappingCache>>(MatchingCacheItem.REClientCampusProductMapping);

                List<int> clientCampusProductMappingIdList = new List<int>();
                HashSet<ZipCodeInclusionExclusion> releventCampusZipCodeList = null;
                GeoCodeProcessor geoProcess = new GeoCodeProcessor();

                List<MatchItem> campusPrograms = matchDb.FilterProgramProducts(null, campusId, null, null, null, null, null, null, null, null, null, null, null, null, null, true, null, null, null, null, null, null, null, null, null, null, null, null, null).Where(mi => campaign.Products.Contains(mi.Match.ProductId)).ToList();

                clientCampusProductMappingIdList = campusPrograms.Select(mi => mi.Match.ClientCampusProductMappingId).Distinct().ToList();

                foreach (int ccpm in clientCampusProductMappingIdList)
                {
                    List<ZipCodeInclusionExclusion> zipCodeList;
                    VW_Matching_ClientCampusProductMappingCache crCampusItem;

                    if (crCampusList.TryGetValue(ccpm, out crCampusItem))
                    {
                        if (crCampusItem.IsZipCodeInclusionExclusionActive.HasValue && crCampusItem.IsZipCodeInclusionExclusionActive.Value)
                        {
                            if (crCampusItem.AllowableRadius.HasValue && !String.IsNullOrWhiteSpace(crCampusItem.RadiusZipCode))
                            {
                                zipCodeList = geoProcess.GetZipCodesWithinRadius(crCampusItem.RadiusZipCode, crCampusItem.AllowableRadius.Value).Select(z => new ZipCodeInclusionExclusion() { IsInclusion = true, ZipCode = z }).ToList();

                                if (releventCampusZipCodeList == null)
                                {
                                    releventCampusZipCodeList = new HashSet<ZipCodeInclusionExclusion>(zipCodeList);
                                }
                                else
                                {
                                    releventCampusZipCodeList.IntersectWith(zipCodeList);
                                }
                            }
                            else
                            {
                                if (releventCampusZipCodeList == null)
                                {
                                    if (campusZipCodeCache.TryGetValue(ccpm, out zipCodeList))
                                        releventCampusZipCodeList = new HashSet<ZipCodeInclusionExclusion>(zipCodeList);
                                }
                                else
                                {
                                    if (campusZipCodeCache.TryGetValue(ccpm, out zipCodeList))
                                        releventCampusZipCodeList.IntersectWith(zipCodeList);
                                }
                            }
                        }
                    }
                }

                if (releventCampusZipCodeList == null || releventCampusZipCodeList.Count == 0)
                    zipCodeRule.ZipCodeRuleType = ZipCodeRuleType.None;
                else if (releventCampusZipCodeList.Any(f => f.IsInclusion == true))
                {
                    zipCodeRule.ZipCodeRuleType = ZipCodeRuleType.Inclusion;
                    zipCodeRule.ZipCodeList = releventCampusZipCodeList.Select(z => z.ZipCode).ToList();
                }
                else
                {
                    zipCodeRule.ZipCodeRuleType = ZipCodeRuleType.Exclusion;
                    zipCodeRule.ZipCodeList = releventCampusZipCodeList.Select(z => z.ZipCode).ToList();
                }
            }

            return zipCodeRule;
        }

        public static List<int> ExecuteInstitutionGroupRule(Dictionary<int, decimal> institutions, bool isSchoolSelection = false, bool removePECGroup = false)
        {
            List<int> institutionsToRemove = new List<int>();

            InstitutionGroupCache institionGroupCache = StaticCacheProxyHost.CacheProxy.Get<InstitutionGroupCache>(MatchingCacheItem.InstitutionGroup);

            var matchingInstitutions = (from i in institutions
                                        join g in institionGroupCache.InstitutionToGroups on i.Key equals g.Key
                                        select new { Institution = new { Id = i.Key, Score = i.Value }, Groups = g.Value });

            if (matchingInstitutions != null && matchingInstitutions.Count() > 0)
            {
                var flattened = from outer in matchingInstitutions
                                from inner in outer.Groups
                                select new { InstitutionId = outer.Institution.Id, Score = outer.Institution.Score, GroupId = inner.Item1, RemoveInSchoolSelection = inner.Item2 };

                var groupedByGroupId = flattened.GroupBy(g => g.GroupId);

                foreach (var group in groupedByGroupId)
                {
                    if (group.Count() > 1)
                    {
                        if (isSchoolSelection && group.Any(g => g.RemoveInSchoolSelection == false))
                        {
                            if (group.Any(g => g.Score == 99999))
                                institutionsToRemove.AddRange(group.Where(g => g.Score != 99999).Select(f => f.InstitutionId));
                        }
                        else
                            institutionsToRemove.AddRange(group.OrderBy(f => f.Score).Take(group.Count() - 1).Select(f => f.InstitutionId)); //only leave the highest score, take the rest
                    }
                }
            }

            if (removePECGroup)
            {
                foreach (var i in institionGroupCache.GroupToInstitutions[3])
                    institutionsToRemove.Remove(i);
            }

            return institutionsToRemove;
        }

        public static List<int> ExecuteCampusZoneRule(List<Tuple<int, int, string, decimal>> campusList) //<int, int, string, decimal> = <institutionId, campusId, CampusName, RankScore>
        {
            List<int> campusesToRemove = new List<int>();

            var groupedByName = campusList.GroupBy(c => new { c.Item1, c.Item3 }).Where(f => f.Count() > 1); //group by instId and campusname

            foreach (var group in groupedByName)
            {
                var campusToKeep = group.OrderByDescending(g => g.Item4).Select(f => f.Item2).First();

                campusesToRemove.AddRange(group.Where(g => g.Item2 != campusToKeep).Select(f => f.Item2));
            }

            return campusesToRemove;
        }

        public RulesResult ExecuteRules(List<MatchItem> m)//, RulesInput rulesInput)
        {
            StartLogDetail("ExecuteRules");

            RulesResult result = new RulesResult();

            List<MatchItem> matches = m.Where(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Paid && mi.FailedValidation == false).ToList();
            List<NoMatch> noMatches = new List<NoMatch>();

            ProcessClientRelationshipRules(ref matches, ref noMatches);
            ProcessClientCampusRules(ref matches, ref noMatches);
            ProcessPsiRules(ref matches, ref noMatches);
            ProcessProgramRules(ref matches, ref noMatches);
            ProcessProgramProductRules(ref matches, ref noMatches);

            //EMS Application Campaign
            var campaign = this.ruleInput.Campaign;
            //do not ping for EMS unless we are matching to POST EMS cr
            if ((campaign != null && campaign.ApplicationId.HasValue && campaign.ApplicationId.Value != 27) ||
                (campaign != null && campaign.ApplicationId.HasValue && campaign.ApplicationId.Value == 27 && matches.Any(c => c.Match.ClientRelationshipId == 9151)))
            {
                ProcessLeadPingRule(ref matches, ref noMatches, campaign);
                //reprocess CR lead cap rules only 

                ProcessLeadPingLeadScoreCapRules(ref matches, ref noMatches);
            }

            //Process template assignment rule
            //matches = m.Where(mi => mi.Match.PaidStatusTypeId != PaidStatusType.Paid).ToList();
            //ProcessProgramProductRules(ref matches, ref noMatches);

            result.MatchedProgramProductList = m;
            result.NoMatchOutput = noMatches;

            EndLogDetail();

            return result;
        }

        public RulesResult ExecuteProgramProductRules(List<MatchItem> m)//, RulesInput rulesInput)
        {
            StartLogDetail("ExecuteRules");

            RulesResult result = new RulesResult();

            List<MatchItem> matches = m.Where(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Paid).ToList();
            List<NoMatch> noMatches = new List<NoMatch>();

            ProcessProgramProductRules(ref matches, ref noMatches);

            result.MatchedProgramProductList = matches;
            result.NoMatchOutput = noMatches;

            EndLogDetail();

            return result;
        }

        private void ProcessLeadPingRule(ref List<MatchItem> matches, ref List<NoMatch> noMatches, Campaign camp)
        {
            StartLogDetail("ProcessLeadPingRule");

            if (RulesEngineFactory.CanProcessLeadPingRule(ruleInput))
            {
                StartLogDetail("RetrieveInstitutionList");
                //if were not EMS do the usual, if we are EMS only get the Post Institution for the EMS Post CR
                var institutionList = camp != null && camp.ApplicationId.HasValue && camp.ApplicationId.Value != 27 ?
                                            (from m in matches
                                             where m.FailedValidation == false
                                             select new { m.Match.InstitutionId, m.Match.CampusId, m.Match.ClientRelationshipId }
                                             ).Distinct().ToList()
                                             :
                                             (from m in matches
                                              where m.FailedValidation == false && m.Match.ClientRelationshipId == 9151
                                              select new { m.Match.InstitutionId, m.Match.CampusId, m.Match.ClientRelationshipId }
                                             ).Distinct().ToList();

                List<LeadPingService.InstitutionConfig> institutions = new List<LeadPingService.InstitutionConfig>();
                List<int> addedInstitutions = new List<int>();

                Boolean BachelorsAvailable = true;

                foreach (var x in institutionList)
                {
                    //added 7/21 check if there are any bachelors programs available (only need to check for Ashford)
                    if (x.InstitutionId == 44 && matches.Where(validation => validation.FailedValidation == false && validation.Match.ProgramLevelId == 3 && validation.Match.InstitutionId == x.InstitutionId).FirstOrDefault() == null)
                        BachelorsAvailable = false;
                    else
                        BachelorsAvailable = true;

                    if (addedInstitutions.Contains(x.InstitutionId))
                    {
                        if (x.InstitutionId == 21 && !institutions.Any(c => c.InstitutionID == x.InstitutionId && c.ClientrelationshipID == x.ClientRelationshipId))
                        {
                            //this is to make sure both crs for AIU institution id 21 get passed to lead ping but not any other crs
                            LeadPingService.InstitutionConfig ic = new LeadPingService.InstitutionConfig();
                            ic.InstitutionID = x.InstitutionId;
                            ic.ClientrelationshipID = x.ClientRelationshipId;
                            ic.CampusConfigs = new LeadPingService.CampusConfig[1] { new LeadPingService.CampusConfig() { CampusID = x.CampusId } };
                            ic.BachelorsAvailable = BachelorsAvailable;
                            institutions.Add(ic);
                            addedInstitutions.Add(x.InstitutionId);
                        }
                        else
                        {
                            //if AIU check against CR if its not AIU just get by institutionid
                            LeadPingService.InstitutionConfig ic = x.InstitutionId == 21 ? institutions.Find(inst => inst.InstitutionID == x.InstitutionId && inst.ClientrelationshipID == x.ClientRelationshipId) : institutions.Find(inst => inst.InstitutionID == x.InstitutionId);
                            ic.ClientrelationshipID = x.ClientRelationshipId;
                            List<LeadPingService.CampusConfig> campuses = new List<LeadPingService.CampusConfig>();
                            campuses.AddRange(ic.CampusConfigs);
                            if (!campuses.Any(c => c.CampusID == x.CampusId))
                            {
                                campuses.Add(new LeadPingService.CampusConfig() { CampusID = x.CampusId });
                            }
                            ic.CampusConfigs = campuses.ToArray();
                        }

                    }
                    else
                    {
                        LeadPingService.InstitutionConfig ic = new LeadPingService.InstitutionConfig();
                        ic.InstitutionID = x.InstitutionId;
                        ic.ClientrelationshipID = x.ClientRelationshipId;
                        ic.CampusConfigs = new LeadPingService.CampusConfig[1] { new LeadPingService.CampusConfig() { CampusID = x.CampusId } };
                        ic.BachelorsAvailable = BachelorsAvailable;
                        institutions.Add(ic);
                        addedInstitutions.Add(x.InstitutionId);
                    }
                }
                EndLogDetail();

                StartLogDetail("ProcessLingPingRule");
                LeadPingRuleResult leadPingResult = RulesEngineFactory.ProcessLingPingRule(institutions, ruleInput, _performanceLog);
                EndLogDetail();

                string warmTransferSilverDupeInstitutionsString = ConfigurationManager.AppSettings["WTSilverDupeInstitutions"];
                string[] warmTransferSilverDupeInstitutionsList = new string[0];

                if (!String.IsNullOrWhiteSpace(warmTransferSilverDupeInstitutionsString))
                    warmTransferSilverDupeInstitutionsList = warmTransferSilverDupeInstitutionsString.Split(',');

                StartLogDetail("RemoveFailedInstitutions");

                if (leadPingResult != null && (leadPingResult.RemovedInstitutions.Count() > 0 || leadPingResult.ScoreInstitution.Count() > 0))
                {
                    Dictionary<int, LeadPingRemoval> crpms = new Dictionary<int, LeadPingRemoval>();
                    //Update the matches to mark the Match Item as Failed Validation
                    foreach (MatchItem mi in matches)
                    {
                        if (mi.FailedValidation) continue;

                        if (leadPingResult.RemovedInstitutions.Count() > 0)
                        {

                            bool isWarmTransfer = Product.IsWarmTransferProduct(mi.Match.ProductId);
                            bool isWarmTransferSilver = Product.IsWarmTransferSilverProduct(mi.Match.ProductId);

                            foreach (LeadPingRuleOutput lpro in leadPingResult.RemovedInstitutions)
                            {
                                if (mi.Match.InstitutionId == lpro.InstitutionId)
                                {
                                    if (lpro.BaseRuleType == BaseRuleDefinitionType.InternalDuplicate)
                                    {
                                        if (!isWarmTransfer)
                                            mi.FailedValidation = true;
                                        else if (isWarmTransfer && lpro.Products.Contains(mi.Match.ProductId))
                                            mi.FailedValidation = true;
                                    }
                                    else if (lpro.BaseRuleType == BaseRuleDefinitionType.ExternalDuplicate)
                                    {
                                        if (mi.Match.InstitutionId == 21) //if AIU
                                        {
                                            //only fail if the cr matches for aiu
                                            if (lpro.ClientRelationshipId == mi.Match.ClientRelationshipId) {
                                                mi.FailedValidation = CheckByEducationLevel(lpro.ProgramLevelIDs, mi.Match.ProgramLevelId);
                                            }
                                        }
                                        else
                                        {
                                            if (isWarmTransferSilver && warmTransferSilverDupeInstitutionsList.Contains(lpro.InstitutionId.ToString()))
                                                mi.FailedValidation = CheckByEducationLevel(lpro.ProgramLevelIDs, mi.Match.ProgramLevelId);
                                            else if (!isWarmTransferSilver)
                                                mi.FailedValidation = CheckByEducationLevel(lpro.ProgramLevelIDs, mi.Match.ProgramLevelId);
                                        }
                                    }
                                    else if (lpro.BaseRuleType == BaseRuleDefinitionType.LeadScore)
                                    {
                                        if ((mi.Match.InstitutionId != 21) || //if not AIU 
                                        (mi.Match.InstitutionId == 21 && (lpro.ClientRelationshipId == mi.Match.ClientRelationshipId || lpro.ClientRelationshipId == 0)))
                                        //if AIU and  cr ids match or if crid is 0 that means its the neustar ping so fail
                                        {
                                            if (lpro.Products != null && lpro.Products.Contains(mi.Match.ProductId))
                                                mi.FailedValidation = true;
                                        }
                                    }


                                    if (mi.FailedValidation)
                                    {
                                        mi.RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Institution, RuleEntityEntityId = mi.Match.InstitutionId, RuleType = (BaseRuleType)lpro.BaseRuleType };

                                        if (!crpms.ContainsKey(mi.Match.ClientRelationProductMappingId))
                                        {
                                            crpms.Add(mi.Match.ClientRelationProductMappingId, new LeadPingRemoval
                                            {
                                                BaseRuleType = lpro.BaseRuleType,
                                                ClientRelationProductMappingId = mi.Match.ClientRelationProductMappingId,
                                                ClientRelationshipId = mi.Match.ClientRelationshipId,
                                                InstitutionId = mi.Match.InstitutionId,
                                                ProductId = mi.Match.ProductId,
                                                RuleName = lpro.RuleName
                                            });
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                        if (leadPingResult.ScoreInstitution.Count() > 0)
                        {

                            var institutionScore = leadPingResult.ScoreInstitution
                                                    .Where(x => x.InstitutionId == mi.Match.InstitutionId &&
                                                    x.ProductIDs != null &&
                                                    x.ProductIDs.Contains(mi.Match.ProductId))
                                                    .FirstOrDefault();
                            if (institutionScore != null)
                            {
                                mi.Score = institutionScore.Score;
                                mi.ScoreId = institutionScore.ScoreId;
                            }
                        }
                    }

                    HashSet<int> processedInstitutions = new HashSet<int>();

                    foreach (var lpr in crpms.Values)
                    {
                        noMatches.Add(new NoMatch()
                        {
                            entityId = lpr.ClientRelationProductMappingId,
                            InstitutionId = lpr.InstitutionId,
                            ClientRelationshipId = lpr.ClientRelationshipId,
                            entityType = EntityMeta.ClientRelationProductMapping,
                            productId = 0,
                            RuleName = lpr.RuleName,
                            RuleId = lpr.RuleId,
                            RuleType = (BaseRuleType)((int)lpr.BaseRuleType)
                        });

                        if ((lpr.ClientRelationshipId == 22 || lpr.ClientRelationshipId == 23) &&
                            (BaseRuleType)((int)lpr.BaseRuleType) == BaseRuleType.LeadScore &&
                            DidIncorrectlyFailPECPing(lpr.ProductId, lpr.RuleName.Substring(lpr.RuleName.Length - 1, 1)))
                        {
                            System.Exception e = new System.Exception("PEC Call Verified Removed");
                            ISException ise = new ISException(e, ruleInput.prospectData, leadPingResult);
                            ise.Save();
                        }
                    }
                }
                EndLogDetail();

            }
            EndLogDetail();
        }

        private bool CheckByEducationLevel(List<Int32> programLevelIds, int matchProgramLevelId) {
            if (programLevelIds != null && programLevelIds.Count > 0) {
                if (programLevelIds.Contains(matchProgramLevelId))
                    return true;
            }
            else
                return true;

            return false;
        }

        private bool DidIncorrectlyFailPECPing(int productId, string score)
        {
            if (ruleInput.Campaign != null && !String.IsNullOrEmpty(ruleInput.Campaign.CECLeadScore) &&
               !ruleInput.Campaign.CECLeadScore.ToLower().Contains(score))
                return false;

            if (productId == 116 &&
               ((score == "a" || score == "b" || score == "c") || ((score == "d" || score == "e" || score == "f") && ruleInput.prospectData.StateId != 6)))
                return true;
            else if ((productId == 59 || productId == 65) && score == "a")
                return true;
            else if ((productId == 60 || productId == 66) && score == "b")
                return true;
            else if ((productId == 61 || productId == 67) && score == "c")
                return true;
            else if ((productId == 62 || productId == 68) && score == "d")
                return true;
            else if ((productId == 63 || productId == 69) && score == "e")
                return true;
            else if ((productId == 64 || productId == 70) && score == "f")
                return true;

            return false;
        }
        private void ProcessProgramRules(ref List<MatchItem> matches, ref List<NoMatch> noMatches)
        {
            StartLogDetail("ProcessProgramRules");

            RulesEngineFactory rf = RuleCacheProcessor.REFactory;

            /*********************************************************************************
             **************** BEGIN PROCESSING Program Level Rules********************
             *********************************************************************************/
            //Get Program List
            StartLogDetail("Get Program List");
            var programList = (from m in matches
                               where m.FailedValidation == false
                               select new ProgramRuleInput()
                               {
                                   ProgramId = m.Match.ProgramId,
                                   ProgramLevelId = m.Match.ProgramLevelId,
                                   InstitutionId = m.Match.InstitutionId
                               }
                               ).DistinctBy(c => c.ProgramId).ToList();
            EndLogDetail();

            List<ProgramRuleInput> removedPrograms = new List<ProgramRuleInput>();

            //Loop through Program Rules
            foreach (Type t in rf.ProgramRuleList)
            {
                StartLogDetail("Process Rule - " + t.Name);
                if (IsCountryOnly && t != typeof(Country))
                    continue;

                if (excludedRules.Contains(t))
                    continue;

                //Process Program Rule
                RulesEngineFactory.ProcessProgramRule(programList, out removedPrograms, t, ruleInput);

                //Removed Programs's that failed validation to avoid reprocessing
                foreach (ProgramRuleInput pri in removedPrograms)
                    programList.Remove(pri);

                //Update the matches to mark the Match Item as Failed Validation
                //StartLogDetail("Updating Failed Validation");
                if (removedPrograms.Count > 0)
                {
                    foreach (ProgramRuleInput removed in removedPrograms)
                    {
                        noMatches.Add(new NoMatch()
                        {
                            entityId = removed.ProgramId,
                            entityType = EntityMeta.Program,
                            RuleName = removed.RuleName,
                            RuleId = removed.RuleId,
                            RuleType = (BaseRuleType)((int)removed.BaseRuleType),
                            InstitutionId = removed.InstitutionId
                        });
                    }

                    var joinedMatches = from m in matches
                                        join r in removedPrograms
                                            on m.Match.ProgramId equals r.ProgramId
                                        select new { m, r };

                    foreach (var match in joinedMatches)
                    {
                        match.m.FailedValidation = true;
                        match.m.RemovalReason = new RemovalReason()
                        {
                            RuleEntity = EntityMeta.Program,
                            RuleEntityEntityId = match.m.Match.ProgramId,
                            RuleType = (BaseRuleType)(int)match.r.BaseRuleType,
                            RuleDetail = match.r.RuleName
                        };
                    }
                }
                //EndLogDetail();

                EndLogDetail();
            }
            EndLogDetail();
        }

        private void ProcessProgramProductRules(ref List<MatchItem> matches, ref List<NoMatch> noMatches)
        {
            StartLogDetail("ProcessProgramProductRules");

            RulesEngineFactory rf = RuleCacheProcessor.REFactory;

            /*********************************************************************************
             **************** BEGIN PROCESSING Program Product Level Rules********************
             *********************************************************************************/
            //Get Program Product List
            StartLogDetail("Get Program Product List");
            var ppList = (from m in matches
                          where m.FailedValidation == false
                          select new ProgramProductRuleInput(m.Match.ProgramId, m.Match.ProductId)
                          {
                              ClientRelationProductMappingId = m.Match.ClientRelationProductMappingId,
                              ClientCampusRelationshipId = m.Match.ClientCampusRelationshipId,
                              PsiId = m.Match.PsiId,
                              ProgramProductId = m.Match.ProgramProductId,
                              IncludeAllZipCodes = m.Match.IncludeAllZipCodes,
                              IsZipCodeExclusion = m.Match.ProgramProductZipCodeExclusion,
                              IsZipCodeInclusion = m.Match.ProgramProductZipCodeInclusion,
                              IsZipCodeInclusionExclusionActive = m.Match.ProgramProductUseZipCodeRules,
                              AllowableRadius = m.Match.ProgramProductAllowableRadius,
                              RadiusZipCode = m.Match.ProgramProductRadiusZipCode,
                              InstitutionId = m.Match.InstitutionId,
                              ClientRelationshipId = m.Match.ClientRelationshipId,
                              ProgramLevelId = m.Match.ProgramLevelId
                          }
                         ).DistinctBy(c => c.ProgramProductId).ToDictionary(o => o.ProgramProductId, o => o);
            EndLogDetail();

            List<ProgramProductRuleInput> removedProgramProducts = new List<ProgramProductRuleInput>();

            //Loop through Program Product Rules
            foreach (Type t in rf.ProgramProductRuleList)
            {
                StartLogDetail("Process Rule - " + t.Name);
                if (IsCountryOnly && t != typeof(Country))
                    continue;

                if (excludedRules.Contains(t))
                    continue;

                //Process Program Product Rule
                RulesEngineFactory.ProcessProgramProductRule(ppList, out removedProgramProducts, t, ruleInput);

                //Removed Program Product's that failed validation to avoid reprocessing
                foreach (ProgramProductRuleInput ppri in removedProgramProducts)
                    ppList.Remove(ppri.ProgramProductId);

                //Update the matches to mark the Match Item as Failed Validation
                //StartLogDetail("Updating Failed Validation");
                if (removedProgramProducts.Count > 0)
                {
                    var joinedMatches = from m in matches
                                        join r in removedProgramProducts
                                           on m.Match.ProgramProductId equals r.ProgramProductId
                                        select new { m, r };

                    foreach (var match in joinedMatches)
                    {
                        match.m.FailedValidation = true;
                        match.m.RemovalReason = new RemovalReason()
                        {
                            RuleEntity = EntityMeta.ProgramProduct,
                            RuleEntityEntityId = match.m.Match.ProgramProductId,
                            RuleType = (BaseRuleType)(int)match.r.BaseRuleType,
                            RuleDetail = match.r.RuleName
                        };
                    }

                    foreach (ProgramProductRuleInput removed in removedProgramProducts)
                    {
                        noMatches.Add(new NoMatch()
                        {
                            entityId = removed.ProgramProductId,
                            entityType = EntityMeta.ProgramProduct,
                            productId = removed.ProductId,
                            RuleName = removed.RuleName,
                            RuleId = removed.RuleId,
                            RuleType = (BaseRuleType)((int)removed.BaseRuleType),
                            InstitutionId = removed.InstitutionId,
                            ClientRelationshipId = removed.ClientRelationshipId,
                            StandardControlCode = removed.StandardControlCode
                        });
                    }
                }
                //EndLogDetail();

                EndLogDetail();
            }
            EndLogDetail();
        }

        private void ProcessPsiRules(ref List<MatchItem> matches, ref List<NoMatch> noMatches)
        {
            StartLogDetail("ProcessPsiRules");
            RulesEngineFactory rf = RuleCacheProcessor.REFactory;

            /*********************************************************************************
             *********** BEGIN PROCESSING Client Campus Relationship Level Rules**************
             *********************************************************************************/
            //Get PSI List
            StartLogDetail("Get PSI List");
            var psiList = (from m in matches
                           where m.FailedValidation == false
                           select new PSIRuleInput()
                           {
                               ClientRelationProductMappingId = m.Match.ClientRelationProductMappingId,
                               ClientCampusRelationshipId = m.Match.ClientCampusRelationshipId,
                               ProductId = m.Match.ProductId,
                               PsiId = m.Match.PsiId,
                               ClientRelationshipId = m.Match.ClientRelationshipId,
                               InstitutionId = m.Match.InstitutionId
                           }
                         ).DistinctBy(c => new { c.PsiId, c.ClientCampusRelationshipId }).ToList();
            EndLogDetail();

            List<PSIRuleInput> removedPsi = new List<PSIRuleInput>();

            //Loop through PSI Rules
            foreach (Type t in rf.PsiRuleList)
            {
                StartLogDetail("Process Rule - " + t.Name);

                if (IsCountryOnly && t != typeof(Country))
                    continue;

                if (excludedRules.Contains(t))
                    continue;

                //Process PSI Rule
                RulesEngineFactory.ProcessPsiRule(psiList, out removedPsi, t, ruleInput);

                //Removed PSI's that failed validation to avoid reprocessing
                foreach (PSIRuleInput pri in removedPsi)
                    psiList.Remove(pri);

                //Update the matches to mark the Match Item as Failed Validation
                //StartLogDetail("Updating Failed Validation");
                if (removedPsi.Count > 0)
                {
                    var joinedMatches = from m in matches
                                        join r in removedPsi
                                           on new { m.Match.PsiId, m.Match.ClientCampusRelationshipId } equals new { r.PsiId, r.ClientCampusRelationshipId }
                                        select new { m, r };

                    foreach (var match in joinedMatches)
                    {
                        match.m.FailedValidation = true;
                        match.m.RemovalReason = new RemovalReason()
                        {
                            RuleEntity = EntityMeta.PSI,
                            RuleEntityEntityId = match.m.Match.PsiId,
                            RuleType = (BaseRuleType)(int)match.r.BaseRuleType,
                            RuleDetail = match.r.RuleName
                        };
                    }

                    foreach (PSIRuleInput removed in removedPsi)
                    {
                        noMatches.Add(new NoMatch()
                        {
                            entityId = removed.PsiId,
                            entityType = EntityMeta.PSI,
                            productId = removed.ProductId,
                            RuleName = removed.RuleName,
                            RuleId = removed.RuleId,
                            RuleType = (BaseRuleType)((int)removed.BaseRuleType),
                            ClientRelationshipId = removed.ClientRelationshipId,
                            InstitutionId = removed.InstitutionId
                        }
                                                    );
                    }
                }
                EndLogDetail();
            }
            EndLogDetail();
        }

        private void ProcessClientCampusRules(ref List<MatchItem> matches, ref List<NoMatch> noMatches)
        {
            StartLogDetail("ProcessClientCampusRules");
            RulesEngineFactory rf = RuleCacheProcessor.REFactory;

            /*********************************************************************************
             *********** BEGIN PROCESSING Client Campus Relationship Level Rules**************
             *********************************************************************************/
            //Get Client Campus Relationship List
            StartLogDetail("Get Client Campus Relationship List");
            var campusList = (from m in matches
                              where m.FailedValidation == false
                              select new ClientRelationshipCampusProductRuleInput()
                              {
                                  ClientRelationProductMappingId = m.Match.ClientRelationProductMappingId,
                                  ClientCampusRelationshipId = m.Match.ClientCampusRelationshipId,
                                  ClientRelationshipId = m.Match.ClientRelationshipId,
                                  InstitutionId = m.Match.InstitutionId,
                                  ProductId = m.Match.ProductId,
                                  ClientCampusProductMappingId = m.Match.ClientCampusProductMappingId,
                                  CampusTypeId = m.Match.CampusCampusTypeId,
                                  CampusPostalCode = m.Match.CampusPostalCode,
                                  CampusType = (CampusType)m.Match.CampusCampusTypeId
                              }
                         ).DistinctBy(c => c.ClientCampusProductMappingId).ToList();
            EndLogDetail();

            List<ClientRelationshipCampusProductRuleInput> removedCampuses = new List<ClientRelationshipCampusProductRuleInput>();

            //Loop through Client Campus Relation Rules
            foreach (Type t in rf.ClientCampusRelationRuleList)
            {
                StartLogDetail("Process Rule - " + t.Name);

                if (IsCountryOnly && t != typeof(Country))
                    continue;

                if (excludedRules.Contains(t))
                    continue;
                //Process Client Campus Relation Rule
                RulesEngineFactory.ProcessCRCampusRule(campusList, out removedCampuses, t, ruleInput);

                //Removed CRCampuses that failed validation to avoid reprocessing
                foreach (ClientRelationshipCampusProductRuleInput crCampusri in removedCampuses)
                    campusList.Remove(crCampusri);

                //Update the matches to mark the Match Item as Failed Validation
                //StartLogDetail("Updating Failed Validation");
                if (removedCampuses.Count > 0)
                {
                    var joinedMatches = from m in matches
                                        join r in removedCampuses
                                         on m.Match.ClientCampusProductMappingId equals r.ClientCampusProductMappingId
                                        select new { m, r };

                    foreach (var match in joinedMatches)
                    {
                        match.m.FailedValidation = true;
                        match.m.RemovalReason = new RemovalReason()
                        {
                            RuleEntity = EntityMeta.ClientCampusProductMapping,
                            RuleEntityEntityId = match.m.Match.ClientCampusProductMappingId,
                            RuleType = (BaseRuleType)(int)match.r.BaseRuleType,
                            RuleDetail = match.r.RuleName
                        };
                    }

                    foreach (ClientRelationshipCampusProductRuleInput removed in removedCampuses)
                    {
                        noMatches.Add(new NoMatch()
                        {
                            entityId = removed.ClientCampusProductMappingId,
                            entityType = EntityMeta.ClientCampusProductMapping,
                            productId = removed.ProductId,
                            RuleName = removed.RuleName,
                            RuleId = removed.RuleId,
                            RuleType = (BaseRuleType)((int)removed.BaseRuleType),
                            ClientRelationshipId = removed.ClientRelationshipId,
                            InstitutionId = removed.InstitutionId
                        });
                    }
                }
                //EndLogDetail();

                EndLogDetail();
            }
            EndLogDetail();
        }

        private void ProcessClientRelationshipRules(ref List<MatchItem> matches, ref List<NoMatch> noMatches)
        {
            StartLogDetail("ProcessClientRelationshipRules");
            RulesEngineFactory rf = RuleCacheProcessor.REFactory;

            /*********************************************************************************
             ************** BEGIN PROCESSING Client Relationship Level Rules******************
             *********************************************************************************/
            //Get Client Relationship List
            StartLogDetail("Get Client Relationship List");
            var crList = (from m in matches
                          where m.FailedValidation == false
                          select new ClientRelationshipProductRuleInput()
                          {
                              ClientRelationProductMappingId = m.Match.ClientRelationProductMappingId,
                              ClientRelationshipId = m.Match.ClientRelationshipId,
                              InstitutionId = m.Match.InstitutionId,
                              ProductId = m.Match.ProductId,
                              ExcludeMatch1plusForFinAid = m.Match.ExcludeMatch1plusForFinAid,
                              RequireJournayaLeadId = m.Match.RequireJournayaLeadId,
                              ScoreId = m.ScoreId
                          }
                         ).DistinctBy(c => c.ClientRelationProductMappingId).ToList();
            EndLogDetail();

            List<ClientRelationshipProductRuleInput> removedCRProducts = new List<ClientRelationshipProductRuleInput>();

            //Loop through Client Relation Rules
            foreach (Type t in rf.ClientRelationRuleList)
            {
                StartLogDetail("Process Rule - " + t.Name);

                if (IsCountryOnly && t != typeof(Country))
                    continue;

                if (excludedRules.Contains(t))
                    continue;

                //Process Client Relation Rule
                RulesEngineFactory.ProcessCRRule(crList, out removedCRProducts, t, ruleInput);

                //Removed CR's that failed validation to avoid reprocessing
                foreach (ClientRelationshipProductRuleInput crpri in removedCRProducts)
                    crList.Remove(crpri);

                //Update the matches to mark the Match Item as Failed Validation
                //StartLogDetail("Updating Failed Validation");
                if (removedCRProducts.Count > 0)
                {
                    var joinedMatches = from m in matches
                                        join r in removedCRProducts
                                          on m.Match.ClientRelationProductMappingId equals r.ClientRelationProductMappingId
                                        select new { m, r };

                    foreach (var match in joinedMatches)
                    {
                        match.m.FailedValidation = true;
                        match.m.RemovalReason = new RemovalReason()
                        {
                            RuleEntity = EntityMeta.ClientRelationProductMapping,
                            RuleEntityEntityId = match.m.Match.ClientRelationProductMappingId,
                            RuleType = (BaseRuleType)(int)match.r.BaseRuleType,
                            RuleDetail = match.r.RuleName
                        };
                    }

                    foreach (ClientRelationshipProductRuleInput removed in removedCRProducts)
                    {
                        noMatches.Add(new NoMatch()
                        {
                            entityId = removed.ClientRelationProductMappingId,
                            ClientRelationshipId = removed.ClientRelationshipId,
                            InstitutionId = removed.InstitutionId,
                            entityType = EntityMeta.ClientRelationProductMapping,
                            productId = removed.ProductId,
                            RuleName = removed.RuleName,
                            RuleId = removed.RuleId,
                            RuleType = (BaseRuleType)((int)removed.BaseRuleType)
                        });
                    }
                }
                //EndLogDetail();

                EndLogDetail();
            }
            EndLogDetail();
        }

        public static List<MatchItem> ProcessClickProgramRules(IEnumerable<MatchItem> matches)
        {
            List<MatchItem> matchesToRemove = new List<MatchItem>();

            if (matches.Any())
            {
                var clickProgramList = (from m in matches
                                        where m.Match.PaidStatusTypeId == PaidStatusType.Paid
                                        select new ClickRuleInput()
                                        {
                                            ClientRelationProductMappingId = m.Match.ClientRelationProductMappingId,
                                            ClientCampusRelationshipId = m.Match.ClientCampusRelationshipId,
                                            ProductId = m.Match.ProductId,
                                            PsiId = m.Match.PsiId
                                        }
                             ).DistinctBy(c => c.PsiId).ToList();

                List<ClickRuleInput> removedClickProgramList = new List<ClickRuleInput>();


                RulesEngineFactory.ProcessClickRule(clickProgramList, out removedClickProgramList);

                foreach (ClickRuleInput pri in removedClickProgramList)
                    matchesToRemove.AddRange(matches.Where(m => m.Match.ClientRelationProductMappingId == pri.ClientRelationProductMappingId
                        && m.Match.ClientCampusRelationshipId == pri.ClientCampusRelationshipId
                        && m.Match.PsiId == pri.PsiId
                        && m.Match.ProductId == pri.ProductId));
            }

            return matchesToRemove;
        }

        private void ProcessLeadPingLeadScoreCapRules(ref List<MatchItem> matches, ref List<NoMatch> noMatches)
        {

            Dictionary<int, LeadPingLeadScore> leadScoreCache = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, LeadPingLeadScore>>(MatchingCacheItem.LeadPingLeadScores);
            List<LeadPingLeadScore> filteredLeadScoreCache = leadScoreCache.Where(m => m.Value.ProductId.HasValue && m.Value.InstitutionId.HasValue).Select(l => l.Value).ToList();
            ProcessClientRelationshipLeadCapRules(ref matches, ref noMatches, filteredLeadScoreCache);
            ProcessCampusLeadCapRules(ref matches, ref noMatches, filteredLeadScoreCache);
            ProcessPSILeadCapRules(ref matches, ref noMatches, filteredLeadScoreCache);
        }

        private void ProcessClientRelationshipLeadCapRules(ref List<MatchItem> matches, ref List<NoMatch> noMatches, List<LeadPingLeadScore> leadScores)
        {


            var crList = (from m in matches
                          join l in leadScores on
                          new { ScoreId = m.ScoreId.GetValueOrDefault(), InstitutionId = m.Match.InstitutionId, m.Match.ProductId }
                          equals
                          new { ScoreId = l.LeadPingLeadScoreId, InstitutionId = l.InstitutionId.GetValueOrDefault(), ProductId = l.ProductId.GetValueOrDefault() }
                          where m.FailedValidation == false && m.ScoreId.HasValue
                          select new ClientRelationshipProductRuleInput()
                          {
                              ClientRelationProductMappingId = m.Match.ClientRelationProductMappingId,
                              ClientRelationshipId = m.Match.ClientRelationshipId,
                              InstitutionId = m.Match.InstitutionId,
                              ProductId = m.Match.ProductId,
                              ExcludeMatch1plusForFinAid = m.Match.ExcludeMatch1plusForFinAid,
                              RequireJournayaLeadId = m.Match.RequireJournayaLeadId,
                              ScoreId = m.ScoreId
                          }
                         ).DistinctBy(c => c.ClientRelationProductMappingId).ToList();

            if (crList.Count > 0)
            {
                List<ClientRelationshipProductRuleInput> removedCRProducts = new List<ClientRelationshipProductRuleInput>();

                RulesEngineFactory.ProcessCRRule(crList, out removedCRProducts, typeof(Rules.LeadCap), ruleInput);

                foreach (ClientRelationshipProductRuleInput crpri in removedCRProducts)
                    crList.Remove(crpri);

                //Update the matches to mark the Match Item as Failed Validation
                //StartLogDetail("Updating Failed Validation");
                if (removedCRProducts.Count > 0)
                {
                    var joinedMatches = from m in matches
                                        join r in removedCRProducts
                                          on m.Match.ClientRelationProductMappingId equals r.ClientRelationProductMappingId
                                        select new { m, r };

                    foreach (var match in joinedMatches)
                    {
                        match.m.FailedValidation = true;
                        match.m.RemovalReason = new RemovalReason()
                        {
                            RuleEntity = EntityMeta.ClientRelationProductMapping,
                            RuleEntityEntityId = match.m.Match.ClientRelationProductMappingId,
                            RuleType = (BaseRuleType)(int)match.r.BaseRuleType,
                            RuleDetail = match.r.RuleName
                        };
                    }

                    foreach (ClientRelationshipProductRuleInput removed in removedCRProducts)
                    {
                        noMatches.Add(new NoMatch()
                        {
                            entityId = removed.ClientRelationProductMappingId,
                            ClientRelationshipId = removed.ClientRelationshipId,
                            InstitutionId = removed.InstitutionId,
                            entityType = EntityMeta.ClientRelationProductMapping,
                            productId = removed.ProductId,
                            RuleName = removed.RuleName,
                            RuleId = removed.RuleId,
                            RuleType = (BaseRuleType)((int)removed.BaseRuleType)
                        });
                    }
                }
            }
        }

        private void ProcessCampusLeadCapRules(ref List<MatchItem> matches, ref List<NoMatch> noMatches, List<LeadPingLeadScore> leadScores)
        {

            var campusList = (from m in matches
                              join l in leadScores on
                     new { ScoreId = m.ScoreId.GetValueOrDefault(), InstitutionId = m.Match.InstitutionId, m.Match.ProductId }
                          equals
                          new { ScoreId = l.LeadPingLeadScoreId, InstitutionId = l.InstitutionId.GetValueOrDefault(), ProductId = l.ProductId.GetValueOrDefault() }
                              where m.FailedValidation == false && m.ScoreId.HasValue
                              select new ClientRelationshipCampusProductRuleInput()
                              {
                                  ClientRelationProductMappingId = m.Match.ClientRelationProductMappingId,
                                  ClientCampusRelationshipId = m.Match.ClientCampusRelationshipId,
                                  ClientRelationshipId = m.Match.ClientRelationshipId,
                                  InstitutionId = m.Match.InstitutionId,
                                  ProductId = m.Match.ProductId,
                                  ClientCampusProductMappingId = m.Match.ClientCampusProductMappingId,
                                  CampusTypeId = m.Match.CampusCampusTypeId,
                                  CampusPostalCode = m.Match.CampusPostalCode,
                                  CampusType = (CampusType)m.Match.CampusCampusTypeId,
                                  ScoreId = m.ScoreId
                              }
                         ).DistinctBy(c => c.ClientCampusProductMappingId).ToList();

            if (campusList.Count > 0)
            {
                List<ClientRelationshipCampusProductRuleInput> removedCampuses = new List<ClientRelationshipCampusProductRuleInput>();

                RulesEngineFactory.ProcessCRCampusRule(campusList, out removedCampuses, typeof(Rules.LeadCap), ruleInput);

                foreach (ClientRelationshipCampusProductRuleInput crCampusri in removedCampuses)
                    campusList.Remove(crCampusri);

                //Update the matches to mark the Match Item as Failed Validation
                //StartLogDetail("Updating Failed Validation");
                if (removedCampuses.Count > 0)
                {
                    var joinedMatches = from m in matches
                                        join r in removedCampuses
                                         on m.Match.ClientCampusProductMappingId equals r.ClientCampusProductMappingId
                                        select new { m, r };

                    foreach (var match in joinedMatches)
                    {
                        match.m.FailedValidation = true;
                        match.m.RemovalReason = new RemovalReason()
                        {
                            RuleEntity = EntityMeta.ClientCampusProductMapping,
                            RuleEntityEntityId = match.m.Match.ClientCampusProductMappingId,
                            RuleType = (BaseRuleType)(int)match.r.BaseRuleType,
                            RuleDetail = match.r.RuleName
                        };
                    }

                    foreach (ClientRelationshipCampusProductRuleInput removed in removedCampuses)
                    {
                        noMatches.Add(new NoMatch()
                        {
                            entityId = removed.ClientCampusProductMappingId,
                            entityType = EntityMeta.ClientCampusProductMapping,
                            productId = removed.ProductId,
                            RuleName = removed.RuleName,
                            RuleId = removed.RuleId,
                            RuleType = (BaseRuleType)((int)removed.BaseRuleType),
                            ClientRelationshipId = removed.ClientRelationshipId,
                            InstitutionId = removed.InstitutionId
                        });
                    }
                }
            }
        }

        private void ProcessPSILeadCapRules(ref List<MatchItem> matches, ref List<NoMatch> noMatches, List<LeadPingLeadScore> leadScores)
        {

            var psiList = (from m in matches
                           join l in leadScores on
 new { ScoreId = m.ScoreId.GetValueOrDefault(), InstitutionId = m.Match.InstitutionId, m.Match.ProductId }
                          equals
                          new { ScoreId = l.LeadPingLeadScoreId, InstitutionId = l.InstitutionId.GetValueOrDefault(), ProductId = l.ProductId.GetValueOrDefault() }
                           where m.FailedValidation == false && m.ScoreId.HasValue

                           select new PSIRuleInput()
                           {
                               ClientRelationProductMappingId = m.Match.ClientRelationProductMappingId,
                               ClientCampusRelationshipId = m.Match.ClientCampusRelationshipId,
                               ProductId = m.Match.ProductId,
                               PsiId = m.Match.PsiId,
                               ClientRelationshipId = m.Match.ClientRelationshipId,
                               InstitutionId = m.Match.InstitutionId,
                               ScoreId = m.ScoreId
                           }
                     ).DistinctBy(c => new { c.PsiId, c.ClientCampusRelationshipId }).ToList();

            if (psiList.Count > 0)
            {
                List<PSIRuleInput> removedPsi = new List<PSIRuleInput>();

                RulesEngineFactory.ProcessPsiRule(psiList, out removedPsi, typeof(Rules.LeadCap), ruleInput);

                foreach (PSIRuleInput pri in removedPsi)
                    psiList.Remove(pri);

                if (removedPsi.Count > 0)
                {
                    var joinedMatches = from m in matches
                                        join r in removedPsi
                                           on new { m.Match.PsiId, m.Match.ClientCampusRelationshipId } equals new { r.PsiId, r.ClientCampusRelationshipId }
                                        select new { m, r };

                    foreach (var match in joinedMatches)
                    {
                        match.m.FailedValidation = true;
                        match.m.RemovalReason = new RemovalReason()
                        {
                            RuleEntity = EntityMeta.PSI,
                            RuleEntityEntityId = match.m.Match.PsiId,
                            RuleType = (BaseRuleType)(int)match.r.BaseRuleType,
                            RuleDetail = match.r.RuleName
                        };
                    }

                    foreach (PSIRuleInput removed in removedPsi)
                    {
                        noMatches.Add(new NoMatch()
                        {
                            entityId = removed.PsiId,
                            entityType = EntityMeta.PSI,
                            productId = removed.ProductId,
                            RuleName = removed.RuleName,
                            RuleId = removed.RuleId,
                            RuleType = (BaseRuleType)((int)removed.BaseRuleType),
                            ClientRelationshipId = removed.ClientRelationshipId,
                            InstitutionId = removed.InstitutionId
                        }
                                                    );
                    }
                }

            }
        }
    }

    public class RulesResult
    {
        public List<MatchItem> MatchedProgramProductList { get; set; }
        public List<NoMatch> NoMatchOutput { get; set; } //set of text for reasons why schools/programs were not matched (failed a rule)

        public RulesResult()
        {
            NoMatchOutput = new List<NoMatch>();
        }
    }

    public class NoMatch
    {
        public int entityId { get; set; }
        public int productId { get; set; }
        public EntityMeta? entityType { get; set; }
        public BaseRuleType RuleType { get; set; }
        public int? RuleId { get; set; }
        public string RuleName { get; set; }
        public string StandardControlCode { get; set; }
        public int? InstitutionId { get; set; }

        public int? ClientRelationshipId { get; set; }
    }

    public class RuleInput
    {
        public Prospect prospectData { get; set; }
        public Campaign Campaign { get; set; }
        public ZipCodeCoordinate ZipCoordinate { get; set; }
        public Dictionary<RuleAttribute, bool> RuleAttributes { get; private set; }

        public LeadCreationType? LeadCreationTypeID { get; set; }

        public List<int> DesiredProgramLevelList { get; set; }
        public int? UserID { get; set; }

        public RuleInput()
        {
            RuleAttributes = new Dictionary<RuleAttribute, bool>();
        }
    }
}
