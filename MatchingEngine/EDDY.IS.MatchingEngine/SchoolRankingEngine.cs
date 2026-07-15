using System;
using System.Collections.Generic;
using System.Linq;
using EDDY.IS.Base.Util;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.Core.Logging;
using System.Diagnostics;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.Core;
using EDDY.IS.MatchingEngine.Constants;

namespace EDDY.IS.MatchingEngine
{
    public class SchoolRankingEngine : Engine
    {
        private Dictionary<int, eRPL> _eRPLList;
        private List<Strategic> _strategicList;
        private List<CampusContent> _campusList;

        //private readonly decimal _gsClickPriceWeight = 0.8M;
        //private readonly decimal _gsRPLWeight = 0.2M;
        //private readonly int _gsApplicationId = 7;

        private List<BusinessModel> BusinessModelList
        {
            get
            {
                return StaticCacheProxyHost.CacheProxy.Get<List<BusinessModel>>(MatchingCacheItem.SRADefinition);
            }
        }

        private Dictionary<int, eRPL> eRPLList
        {
            get
            {
                if (_eRPLList == null)
                    _eRPLList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, eRPL>>(MatchingCacheItem.SRAeRPL);

                return _eRPLList;
            }
        }

        private List<Strategic> StrategicList
        {
            get
            {
                if (_strategicList == null)
                    _strategicList = StaticCacheProxyHost.CacheProxy.Get<List<Strategic>>(MatchingCacheItem.SRAStrategic);

                return _strategicList;
            }
        }

        private List<CampusContent> CampusList
        {
            get
            {
                if (_campusList == null)
                    _campusList = StaticCacheProxyHost.CacheProxy.Get<List<CampusContent>>(MatchingCacheItem.CampusContent);

                return _campusList;
            }
        }

        public SchoolRankingEngine()
        { }

        public SchoolRankingEngine(EDDY.IS.Core.Logging.PerformanceLog pLog)
            : base(pLog)
        { }

        public static List<Strategic> GetStrategicValues()
        {
            List<Strategic> stratList = new List<Strategic>();

            List<VW_Matching_ProgramProductStrategic> databaseStrats = SchoolRankingDataService.GetAllStrategic();

            foreach (var stratItem in databaseStrats)
            {
                Strategic singleStrat = new Strategic();

                singleStrat.ProgramProductId = stratItem.ProgramProductId;
                singleStrat.StrategicValue = stratItem.Strategic.Value;

                stratList.Add(singleStrat);
            }

            return stratList;
        }

        public static Dictionary<int, eRPL> GetLeadCurrentRPL()
        {
            Dictionary<int, eRPL> eRPLList = new Dictionary<int, eRPL>();

            List<VW_Matching_ProgramProductRPL> rplList = SchoolRankingDataService.GetAllProgramProductRPL();
            Dictionary<int, VW_Matching_ProgramProductScrubRate> scrubRateList = SchoolRankingDataService.GetAllProgramProductScrubRate();

            foreach (var rpl in rplList)
            {
                if (scrubRateList.ContainsKey(rpl.ProgramProductId))
                {
                    var scrubRate = scrubRateList[rpl.ProgramProductId];

                    eRPL singleRPL = new eRPL();

                    singleRPL.ProgramProductId = rpl.ProgramProductId;
                    singleRPL.RPL = rpl.RPL;
                    singleRPL.ClickPrice = rpl.ClickPrice;
                    singleRPL.ScrubRate = scrubRate.ScrubRate;
                    singleRPL.eRPLFinal = singleRPL.RPL * (1 - (singleRPL.ScrubRate / 100));

                    eRPLList.Add(rpl.ProgramProductId, singleRPL);
                }
                else
                    eRPLList.Add(rpl.ProgramProductId, new eRPL() { ProgramProductId = rpl.ProgramProductId, RPL = rpl.RPL, ClickPrice = rpl.ClickPrice, ScrubRate = 0, eRPLFinal = rpl.RPL });
            }

            return eRPLList;
        }

        public static Dictionary<int, List<StatePricing>> GetStatePricing()
        {
            Dictionary<int, List<StatePricing>> priceDict = new Dictionary<int, List<StatePricing>>();

            Dictionary<int, Dictionary<int, VW_Matching_ProgramProductPriceByState>> rplList = SchoolRankingDataService.GetAllProgramProductPriceByState();

            foreach (var rpl in rplList)
            {
                List<StatePricing> priceList = new List<StatePricing>();

                foreach (var state in rpl.Value)
                {
                    StatePricing singlePrice = new StatePricing();

                    singlePrice.StateId = state.Value.StateId;
                    singlePrice.RPL = state.Value.RPL;
                    singlePrice.ClickPrice = state.Value.ClickPrice;
                    singlePrice.ScrubRate = state.Value.ScrubRate;
                    singlePrice.eRPLFinal = singlePrice.RPL * (1 - (singlePrice.ScrubRate / 100));

                    priceList.Add(singlePrice);
                }

                priceDict.Add(rpl.Key, priceList);
            }

            return priceDict;
        }

        public static Dictionary<int, decimal> GetSABPSIeRPL()
        {
            Dictionary<int, decimal> eRPCList = new Dictionary<int, decimal>();

            List<VW_Matching_SABPSIeRPC> dbRPCList = SchoolRankingDataService.GetAllSABPSIeRPC();

            foreach(var rpc in dbRPCList)
            {
                if(!eRPCList.ContainsKey(rpc.PsiId))
                    eRPCList.Add(rpc.PsiId, rpc.eRPC);
                else
                {
                    try
                    {
                        Exception ex = new Exception(String.Format("GetSABPSIeRPL duplicate key PsiId = {0}", rpc.PsiId));
                        ISException isEx = new ISException(ex);
                        isEx.Save();
                    }
                    catch { }
                }
            }

            return eRPCList;
        }

        public static List<BusinessModel> GetActiveBusinessModels()
        {
            List<BusinessModel> businessModels = new List<BusinessModel>();

            List<VW_Matching_BusinessModelWeightSubject> weightSubjects = SchoolRankingDataService.GetAllBusinessModelWeightSubject();

            List<VW_Matching_BusinessModelCriteriaGroup> criteriaGroups = SchoolRankingDataService.GetAllBusinessModelCriteriaGroup();

            var groupedByBmId = weightSubjects.GroupBy(bm => bm.BusinessModelId);

            foreach (var businessModel in groupedByBmId)
            {
                var firstBm = businessModel.First();

                BusinessModel bm = new BusinessModel();
                bm.BusinessModelId = firstBm.BusinessModelId;
                bm.BusinessModelName = firstBm.BusinessModelName;
                bm.IsDefault = firstBm.IsDefault;
                bm.UseDistanceCliffFormula = firstBm.UseDistanceCliffFormula;
                bm.DistanceCliffValue = firstBm.DistanceCliffValue;
                bm.DistanceCapMultiplier = firstBm.DistanceCapMultiplier;
                bm.BusinessModelTestId = firstBm.BusinessModelTestId;
                bm.RollupType = (BusinessModelRollupType)firstBm.BusinessModelRollupTypeId;
                bm.WeightSubjectList = new List<WeightSubject>();
                bm.CriteriaGroupList = new List<CriteriaGroup>();

                var groupedByWSId = businessModel.GroupBy(ws => ws.BusinessModelWeightSubjectId);

                foreach (var weightSubject in groupedByWSId)
                {
                    var firstWS = weightSubject.First();

                    WeightSubject ws = new WeightSubject();
                    ws.WeightSubjectId = firstWS.BusinessModelWeightSubjectId;
                    ws.IsBaseline = firstWS.IsBaseline;
                    ws.PercentToShow = firstWS.PercentToShow;
                    ws.FactorWeightList = new List<FactorWeight>();

                    foreach (var factorWeight in weightSubject)
                    {
                        FactorWeight fw = new FactorWeight();
                        fw.FactorWeightId = factorWeight.BusinessModelFactorWeightId;
                        fw.FactorType = (FactorType)factorWeight.BusinessModelFactorId;
                        fw.CampusWeight = factorWeight.CampusWeight.Value;
                        fw.OnlineWeight = factorWeight.OnlineWeight;
                        ws.FactorWeightList.Add(fw);
                    }

                    bm.WeightSubjectList.Add(ws);
                }

                var bmForCriteriaGroup = criteriaGroups.Where(cg => cg.BusinessModelId == bm.BusinessModelId);

                var groupedByCriteriaGroup = bmForCriteriaGroup.GroupBy(cg => cg.BusinessModelCriteriaGroupId);

                foreach (var criteriaGroup in groupedByCriteriaGroup)
                {
                    var firstCG = criteriaGroup.First();

                    CriteriaGroup cg = new CriteriaGroup();
                    cg.CriteriaGroupId = firstCG.BusinessModelCriteriaGroupId;
                    cg.CriteriaList = new List<Criteria>();

                    var groupedByCriteria = criteriaGroup.GroupBy(c => c.BusinessModelCriteriaId);

                    foreach (var criteria in groupedByCriteria)
                    {
                        var firstCriteria = criteria.First();

                        Criteria c = new Criteria();
                        c.CriteriaId = firstCriteria.BusinessModelCriteriaId;
                        c.CriteriaType = (CriteriaType)firstCriteria.PriorityOrder;
                        c.Operand = firstCriteria.Operand;
                        c.CriteriaValueList = new List<CriteriaValue>();

                        foreach (var criteriaValue in criteria)
                        {
                            CriteriaValue cv = new CriteriaValue();
                            cv.CriteriaValueId = criteriaValue.BusinessModelCriteriaValueId;
                            cv.Value = criteriaValue.CriteriaValue;

                            c.CriteriaValueList.Add(cv);
                        }

                        cg.CriteriaList.Add(c);
                    }

                    bm.CriteriaGroupList.Add(cg);
                }

                businessModels.Add(bm);
            }

            return businessModels;
        }

        public BusinessModel PickRankingModel(Campaign campaign, DTO.ProspectInput prospect, int applicationId, int? subjectId, int? categoryId, int? programLevelId, Guid? trackingDeviceGuid)
        {
            StartLogDetail("PickRankingModel");

            BusinessModel selectedBusinessModel = null;

            //First, order all input received from match request - in priority
            SortedDictionary<CriteriaType, string> orderedMatchInput = OrderMatchInput(campaign, prospect, applicationId, subjectId, categoryId, programLevelId); //<CriteriaType, actual value for type from match request input>

            SortedDictionary<CriteriaType, List<int>> criteriaGroupMatches = new SortedDictionary<CriteriaType, List<int>>(); //<CriteriaType, list of criteria groupids that have a criteria defined which matched that type and value>

            //Next, find any matches where a input datapoint matches with criteria defined in an active criteria group
            foreach (var inputItem in orderedMatchInput)
            {
                List<int> matchForInput = null;

                if (inputItem.Key == CriteriaType.Age)
                {
                    var criteriaGroupsWithAge = (from bm in BusinessModelList
                                                 from cg in bm.CriteriaGroupList
                                                 from c in cg.CriteriaList
                                                 where c.CriteriaType == CriteriaType.Age
                                                 select new { CriteriaGroupId = cg.CriteriaGroupId, Operand = c.Operand, AgeValue = Convert.ToInt32(c.CriteriaValueList.First().Value) });

                    var groupedByOperand = criteriaGroupsWithAge.GroupBy(cgwa => cgwa.Operand);

                    foreach (var operandGroup in groupedByOperand)
                    {
                        if (operandGroup.First().Operand == "=")
                        {
                            //matchForInput = criteriaGroupsWithAge.Where(f => Convert.ToInt32(inputItem.Value) == f.AgeValue).Select(cg => cg.CriteriaGroupId).ToList();

                            matchForInput = operandGroup.Where(og => Convert.ToInt32(inputItem.Value) == og.AgeValue).Select(cg => cg.CriteriaGroupId).ToList();
                        }
                        else if (operandGroup.First().Operand == ">")
                        {
                            //matchForInput = criteriaGroupsWithAge.Where(f => Convert.ToInt32(inputItem.Value) > f.AgeValue).Select(cg => cg.CriteriaGroupId).ToList();

                            matchForInput = operandGroup.Where(og => Convert.ToInt32(inputItem.Value) > og.AgeValue).Select(cg => cg.CriteriaGroupId).ToList();
                        }
                        else if (operandGroup.First().Operand == "<")
                        {
                            //matchForInput = criteriaGroupsWithAge.Where(f => Convert.ToInt32(inputItem.Value) < f.AgeValue).Select(cg => cg.CriteriaGroupId).ToList();

                            matchForInput = operandGroup.Where(og => Convert.ToInt32(inputItem.Value) < og.AgeValue).Select(cg => cg.CriteriaGroupId).ToList();
                        }

                        if (matchForInput != null && matchForInput.Count() > 0)
                        {
                            if (!criteriaGroupMatches.ContainsKey(inputItem.Key))
                                criteriaGroupMatches.Add(inputItem.Key, matchForInput);
                            else
                            {
                                List<int> inputMatches = criteriaGroupMatches[inputItem.Key];

                                inputMatches.AddRange(matchForInput);

                                criteriaGroupMatches[inputItem.Key] = inputMatches.Distinct().ToList();
                            }
                        }
                    }
                }
                else
                {
                    matchForInput = (from bm in BusinessModelList
                                     from cg in bm.CriteriaGroupList
                                     from c in cg.CriteriaList
                                     where c.CriteriaValueList.Any(f => f.Value == inputItem.Value) && c.CriteriaType == inputItem.Key
                                     select cg.CriteriaGroupId).ToList();

                    if (matchForInput != null && matchForInput.Count() > 0)
                        criteriaGroupMatches.Add(inputItem.Key, matchForInput);
                }
            }

            Dictionary<int, int> fullyMatchedGroups = new Dictionary<int, int>(); //<criteriaGroupId, number of criteria items matched in group>

            //Group the resulting matches by the criteria groupid
            var groupedByCriteriaGroup = criteriaGroupMatches.Values.SelectMany(list => list.Select(item => item)).GroupBy(cgid => cgid);

            //Then, count the number of items in each matched group, and check if the number matches the number of criteria items in the actual group definined in database
            foreach (var criteriaGroup in groupedByCriteriaGroup)
            {
                int criteriaGroupId = criteriaGroup.First();
                int numberOfMatches = criteriaGroup.Count();

                int totalNumberOfItemsInGroup = (from bm in BusinessModelList
                                                 from cg in bm.CriteriaGroupList
                                                 where cg.CriteriaGroupId == criteriaGroupId
                                                 select cg.CriteriaList).First().Count();

                //if these are equal, that means this criteria group was fully satisfied (meaning each of its items matched something from the input)
                if (numberOfMatches == totalNumberOfItemsInGroup)
                    fullyMatchedGroups.Add(criteriaGroupId, numberOfMatches);
            }

            //nothing matched, must take default business model
            if (fullyMatchedGroups.Count() == 0)
            {
                selectedBusinessModel = (from bm in BusinessModelList
                                         where bm.IsDefault == true
                                         select bm).First();
            }//only one matched, pick that one
            else if (fullyMatchedGroups.Count() == 1)
            {
                selectedBusinessModel = (from bm in BusinessModelList
                                         from cg in bm.CriteriaGroupList
                                         where cg.CriteriaGroupId == fullyMatchedGroups.First().Key
                                         select bm).First();
            }
            else
            {
                //if multiple criteria groups are fully matched, need to determine the best one, which is the one which has the most criteria items in it

                //order and group by the number of items in each group
                var orderedGroups = fullyMatchedGroups.OrderByDescending(mg => mg.Value).GroupBy(mg => mg.Value);

                var topGroup = orderedGroups.First();

                //if only one group has the top number of items in it, pick that one
                if (topGroup.Count() == 1)
                {
                    selectedBusinessModel = (from bm in BusinessModelList
                                             from cg in bm.CriteriaGroupList
                                             where cg.CriteriaGroupId == topGroup.First().Key
                                             select bm).First();
                }
                else //multiple groups have the same number of items, need to pick the "best" match (determined by priority order of CriteriaTypes (reflected in enum numbering)
                {
                    var listOfCriteriaGroupIds = topGroup.Select(c => c.Key).ToList();

                    var matchedGroupsWithSortedCriteriaTypes = (from bm in BusinessModelList
                                                                from cg in bm.CriteriaGroupList
                                                                from c in cg.CriteriaList
                                                                where listOfCriteriaGroupIds.Contains(cg.CriteriaGroupId)
                                                                select new { CriteriaGroupId = cg.CriteriaGroupId, CriteriaTypeList = cg.CriteriaList.Select(cl => cl.CriteriaType).OrderBy(ct => ct).ToList() }).DistinctBy(id => id.CriteriaGroupId).ToList();

                    bool foundDifference = false;
                    int startIndex = 0;
                    int numberOfItemsInTopCriteriaGroup = topGroup.First().Value;
                    int bestCriteriaGroupId = 0;

                    while (!foundDifference && startIndex < numberOfItemsInTopCriteriaGroup)
                    {
                        var singleCriteriaType = matchedGroupsWithSortedCriteriaTypes.Select(f => new { CriteriaGroupId = f.CriteriaGroupId, CriteriaType = f.CriteriaTypeList[startIndex] });

                        var findMax = singleCriteriaType.OrderBy(ct => ct.CriteriaType).GroupBy(ct => ct.CriteriaType);

                        if (findMax.First().Count() == 1)
                        {
                            bestCriteriaGroupId = findMax.First().First().CriteriaGroupId;
                            foundDifference = true;
                        }
                        else
                        {
                            List<int> cgIds = findMax.First().Select(f => f.CriteriaGroupId).ToList();

                            matchedGroupsWithSortedCriteriaTypes = matchedGroupsWithSortedCriteriaTypes.Where(cg => cgIds.Contains(cg.CriteriaGroupId)).ToList();
                        }

                        startIndex++;
                    }

                    if (bestCriteriaGroupId != 0)
                    {
                        selectedBusinessModel = (from bm in BusinessModelList
                                                 from cg in bm.CriteriaGroupList
                                                 where cg.CriteriaGroupId == bestCriteriaGroupId
                                                 select bm).First();
                    }
                    else
                    {
                        //TODO: ERROR?? - something def wrong here
                        //just in case, pick the default model so life goes on

                        selectedBusinessModel = (from bm in BusinessModelList
                                                 where bm.IsDefault == true
                                                 select bm).First();
                    }
                }
            }

            BusinessModel returnedBusinessModel = selectedBusinessModel.Clone();

            DetermineWeightSubject(returnedBusinessModel, trackingDeviceGuid);

            EndLogDetail();

            return returnedBusinessModel;
        }

        private SortedDictionary<CriteriaType, string> OrderMatchInput(Campaign campaign, DTO.ProspectInput prospect, int applicationId, int? subjectId, int? categoryId, int? programLevelId)
        {
            StartLogDetail("OrderMatchInput");

            SortedDictionary<CriteriaType, string> orderedInput = new SortedDictionary<CriteriaType, string>();

            if (prospect != null)
            {
                if (prospect.Age.HasValue)
                    orderedInput.Add(CriteriaType.Age, prospect.Age.ToString());
                if (prospect.StateId.HasValue)
                    orderedInput.Add(CriteriaType.State, prospect.StateId.ToString());
                if (prospect.EducationLevelId.HasValue)
                    orderedInput.Add(CriteriaType.EducationLevel, prospect.EducationLevelId.ToString());
                if (prospect.IsMilitary.HasValue)
                    orderedInput.Add(CriteriaType.IsMilitary, Convert.ToInt32(prospect.IsMilitary.Value).ToString());
            }

            if (campaign != null)
            {
                orderedInput.Add(CriteriaType.Channel, campaign.ChannelId.ToString());
                orderedInput.Add(CriteriaType.Campaign, campaign.CampaignId.ToString());
                orderedInput.Add(CriteriaType.Vendor, campaign.VendorId.ToString());
            }

            if (subjectId.HasValue)
                orderedInput.Add(CriteriaType.Subject, subjectId.ToString());

            if (categoryId.HasValue)
                orderedInput.Add(CriteriaType.Category, categoryId.ToString());

            if (programLevelId.HasValue)
                orderedInput.Add(CriteriaType.ProgramLevel, programLevelId.ToString());

            //orderedInput.Add(CriteriaType.UrlReferrer, "");

            orderedInput.Add(CriteriaType.Application, applicationId.ToString());

            EndLogDetail();
            return orderedInput;
        }

        public List<MatchItem> GenerateProgramRankScores(BusinessModel rankingModel, List<MatchItem> matchItemList, CampusPreference? campusPref, CampusType? campusType, string leadPostalCode, int applicationId, int? leadStateId, out List<FactorAggregate> factorAggList, List<int> specialties = null)
        {
            StartLogDetail("GenerateProgramRankScores");

            StartLogDetail("Match Item Filter");
            List<MatchItem> programsToRank = matchItemList.Where(mi => (mi.FailedValidation == false && mi.Match.PaidStatusTypeId != PaidStatusType.Free) || (mi.FailedValidation == true && mi.RemovalReason != null && mi.RemovalReason.RuleType == BaseRuleType.LeadCap && mi.RemovalReason.RuleDetail == "LeadType Lead Capped")).DistinctBy(mi => mi.Match.ProgramProductId).ToList();
            EndLogDetail();

            Dictionary<int, FactorValueSet> factorValues = GetRankFactorValues(programsToRank, leadPostalCode, leadStateId, specialties);

            factorAggList = new List<FactorAggregate>();

            if (factorValues != null && factorValues.Count > 0)
            {
                //Remove match1 ProgramProducts from Max calculation.
                HashSet<int> match1ProgramProducts = new HashSet<int>(matchItemList.Where(mi => mi.Match.ProductId == (int)ProductType.Match1Exclusive || mi.Match.ProductId == (int)ProductType.Match1Plus).Select(m => m.Match.ProgramProductId));
                Dictionary<int, FactorValueSet> factorValuesForMax = factorValues.Where(m => !match1ProgramProducts.Contains(m.Key)).ToDictionary(f => f.Key, f => f.Value);

                //if the set of programs is ONLY match1, then have to use them
                if(factorValuesForMax == null || !factorValuesForMax.Any())
                    factorValuesForMax = factorValues.ToDictionary(f => f.Key, f => f.Value);

                KeyValuePair<int,FactorValueSet> maxCapRoom = new KeyValuePair<int, FactorValueSet>();
                KeyValuePair<int, FactorValueSet> maxeRPL = new KeyValuePair<int, FactorValueSet>();
                KeyValuePair<int, FactorValueSet> maxClickPrice = new KeyValuePair<int, FactorValueSet>();
                KeyValuePair<int, FactorValueSet> maxCampusDistance = new KeyValuePair<int, FactorValueSet>();
                KeyValuePair<int, FactorValueSet> minCampusDistance = new KeyValuePair<int, FactorValueSet>();

                if (factorValuesForMax != null && factorValuesForMax.Any())
                {
                    maxCapRoom = factorValuesForMax.OrderByDescending(f => f.Value.CapRoom).FirstOrDefault();
                    maxeRPL = factorValuesForMax.OrderByDescending(f => f.Value.eRPL).FirstOrDefault(); 
                    maxClickPrice = factorValuesForMax.OrderByDescending(f => f.Value.ClickPrice).FirstOrDefault(); 
                    maxCampusDistance = factorValuesForMax.OrderByDescending(f => f.Value.CampusDistance).FirstOrDefault(); 
                    minCampusDistance = factorValuesForMax.Where(f => f.Value.CampusDistance.HasValue).OrderBy(f => f.Value.CampusDistance).FirstOrDefault();

                    if (maxCampusDistance.Equals(default(KeyValuePair<int, FactorValueSet>)))
                        maxCampusDistance = new KeyValuePair<int, FactorValueSet>(0, new FactorValueSet() { ClickPrice = 0, CampusDistance = null, CapRoom = 0, eRPL = 0, Strategic = false });
                    if (minCampusDistance.Equals(default(KeyValuePair<int, FactorValueSet>)))
                        minCampusDistance = new KeyValuePair<int, FactorValueSet>(0, new FactorValueSet() { ClickPrice = 0, CampusDistance = null, CapRoom = 0, eRPL = 0, Strategic = false });

                    maxCapRoom.Value.CapRoom = maxCapRoom.Value.CapRoom == 0 ? 0.01M : maxCapRoom.Value.CapRoom;
                    maxeRPL.Value.eRPL = maxeRPL.Value.eRPL == 0 ? 0.01M : maxeRPL.Value.eRPL;
                    maxClickPrice.Value.ClickPrice = maxClickPrice.Value.ClickPrice == 0 ? 0.01M : maxClickPrice.Value.ClickPrice;
                }

                factorAggList.Add(BuildFactorAggregate(FactorType.Cap, BusinessModelAggregateType.Max, maxCapRoom, maxCapRoom.Value.CapRoom));
                factorAggList.Add(BuildFactorAggregate(FactorType.eRPL, BusinessModelAggregateType.Max, maxeRPL, maxeRPL.Value.eRPL));
                factorAggList.Add(BuildFactorAggregate(FactorType.RPC, BusinessModelAggregateType.Max, maxClickPrice, maxClickPrice.Value.ClickPrice));
                factorAggList.Add(BuildFactorAggregate(FactorType.CampusDistance, BusinessModelAggregateType.Max, maxCampusDistance, maxCampusDistance.Value.CampusDistance));
                factorAggList.Add(BuildFactorAggregate(FactorType.CampusDistance, BusinessModelAggregateType.Min, minCampusDistance, minCampusDistance.Value.CampusDistance));

                StartLogDetail("GetAllWeights");
                Dictionary<CampusType, Dictionary<FactorType, decimal>> allWeights = GetAllWeights(rankingModel);
                EndLogDetail();

                Dictionary<int, Tuple<decimal, List<MatchItemFactorScore>>> programProductScore = new Dictionary<int, Tuple<decimal, List<MatchItemFactorScore>>>();

                Dictionary<int, VW_Matching_Application> applications = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_Application>>(MatchingCacheItem.Applications);

                VW_Matching_Application singleApp = null;

                applications.TryGetValue(applicationId, out singleApp);

                StartLogDetail("ScoreProgramProduct");
                foreach (var matchItem in programsToRank)
                {
                    Tuple<decimal, List<MatchItemFactorScore>> score = ScoreProgramProduct(matchItem, factorValues, rankingModel, allWeights, maxCapRoom.Value.CapRoom, maxeRPL.Value.eRPL, maxClickPrice.Value.ClickPrice, maxCampusDistance.Value.CampusDistance, minCampusDistance.Value.CampusDistance, singleApp, !String.IsNullOrWhiteSpace(leadPostalCode));

                    programProductScore.Add(matchItem.Match.ProgramProductId, score);
                }
                EndLogDetail();

                StartLogDetail("UpdateScore - MatchItemList");
                var pairedProgramProducts = (from mi in matchItemList
                                             join ps in programProductScore
                                             on mi.Match.ProgramProductId equals ps.Key
                                             select new { mi, ps }).ToList();
                Random rand = new Random();

                pairedProgramProducts.ForEach(pp =>
                {
                    pp.mi.ProgramRankScore = Math.Round(pp.ps.Value.Item1, 5);

                    int randomInt = rand.Next(1,9999);

                    decimal postFix = Convert.ToDecimal("0.00000" + randomInt.ToString());

                    pp.mi.ProgramRankScore = pp.mi.ProgramRankScore;// + postFix;

                    pp.mi.FactorScores = pp.ps.Value.Item2;
                });
                EndLogDetail();
            }

            EndLogDetail();

            return matchItemList;
        }

        private FactorAggregate BuildFactorAggregate(FactorType factorType, BusinessModelAggregateType aggType, KeyValuePair<int, FactorValueSet> factorInfo, decimal? aggValue)
        {
            FactorAggregate agg = new FactorAggregate();

            agg.BusinessModelAggregateTypeId = aggType;
            agg.BusinessModelFactorId = factorType;
            agg.ProgramProductId = factorInfo.Key;
            agg.AggregateValue = aggValue;

            List<FactorAggregateValue> aggValueList = new List<FactorAggregateValue>();

            aggValueList.Add(new FactorAggregateValue() { BusinessModelFactorId = FactorType.Cap, AggregateValue = factorInfo.Value.CapRoom });
            aggValueList.Add(new FactorAggregateValue() { BusinessModelFactorId = FactorType.eRPL, AggregateValue = factorInfo.Value.eRPL });
            aggValueList.Add(new FactorAggregateValue() { BusinessModelFactorId = FactorType.RPC, AggregateValue = factorInfo.Value.ClickPrice });
            aggValueList.Add(new FactorAggregateValue() { BusinessModelFactorId = FactorType.CampusDistance, AggregateValue = factorInfo.Value.CampusDistance });

            agg.FactorAggregateValueList = aggValueList;

            return agg;
        }

        private void DetermineWeightSubject(BusinessModel rankingModel, Guid? trackingDeviceGuid)
        {
            StartLogDetail("DetermineWeightSubject");

            int weightSubjectId = 0;

            if (!rankingModel.BusinessModelTestId.HasValue)
            {
                if (rankingModel.WeightSubjectList.Any(ws => ws.IsBaseline == true))
                    weightSubjectId = rankingModel.WeightSubjectList.Where(ws => ws.IsBaseline == true).First().WeightSubjectId;
                else
                    weightSubjectId = rankingModel.WeightSubjectList.First().WeightSubjectId;
            }
            else
            {
                weightSubjectId = GetWeightSubjectIdForTest(rankingModel, trackingDeviceGuid);
            }

            EndLogDetail();

            rankingModel.ChoosenWeightSubjectId = weightSubjectId;
        }

        private int GetWeightSubjectIdForTest(BusinessModel rankingModel, Guid? trackingDeviceGuid)
        {
            int weightSubjectId;

            weightSubjectId = GetRandomWeightSubjectIdForTest(rankingModel);

            if (trackingDeviceGuid.HasValue)
                weightSubjectId = GetWeightSubjectIdFromCache(trackingDeviceGuid.Value, rankingModel, weightSubjectId);

            return weightSubjectId;
        }

        private int GetWeightSubjectIdFromCache(Guid trackingDeviceGuid, BusinessModel rankingModel, int randomWeightSubjectId)
        {
            StartLogDetail("GetWeightSubjectIdFromCache");

            int returnedWeightSubjectId = 0;

            try
            {
                BusinessModelTestCacheItem item = new BusinessModelTestCacheItem() { TrackingDeviceGuid = trackingDeviceGuid, BusinessModelId = rankingModel.BusinessModelId, WeightSubjectId = randomWeightSubjectId };

                //check the local in-memory cache
                BusinessModelTestCacheItem cachedItem = (BusinessModelTestCacheItem)StaticCacheProxyHost.CacheProxy.Get(item.CacheItemKeyName);

                if (cachedItem == null)
                {
                    StartLogDetail("GetWeightSubjectIdFromDatabase");
                    //if it doesn't exist, get it from the database cache, this method will check the db if it exists, if so, it will return it, if not, it will insert the random weightsubjectid into the db and return it
                    returnedWeightSubjectId = SchoolRankingDataService.GetWeightSubjectIdFromDatabase(item);
                    EndLogDetail();

                    item.WeightSubjectId = returnedWeightSubjectId;

                    //set the cache with the database value
                    StaticCacheProxyHost.CacheProxy.Set(item.CacheItemKeyName, item);
                }
                else
                    returnedWeightSubjectId = cachedItem.WeightSubjectId;

                //finally, need to check if this weightsubject still exists, if not, then take the random one
                if (!rankingModel.WeightSubjectList.Where(ws => ws.WeightSubjectId == returnedWeightSubjectId).Any())
                {
                    returnedWeightSubjectId = randomWeightSubjectId;

                    if (cachedItem != null)
                        cachedItem.WeightSubjectId = randomWeightSubjectId;
                }
            }
            catch (Exception ex)
            {
                //if anythign fails here, return the "random" weight subject
                returnedWeightSubjectId = randomWeightSubjectId;

                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            EndLogDetail();

            return returnedWeightSubjectId;
        }



        private Dictionary<CampusType, Dictionary<FactorType, decimal>> GetAllWeights(BusinessModel rankingModel)
        {
            Dictionary<CampusType, Dictionary<FactorType, decimal>> allWeights = new Dictionary<CampusType, Dictionary<FactorType, decimal>>();
            WeightSubject weights = null;

            decimal onlineCapWeight = 0;
            decimal onlineeRPLWeight = 0;
            decimal onlineRPCWeight = 0;
            decimal campusRPCWeight = 0;
            decimal campusCapWeight = 0;
            decimal campuseRPLWeight = 0;
            decimal campusDistanceWeight = 0;

            //if (!rankingModel.BusinessModelTestId.HasValue)
            //{
            //    if (rankingModel.WeightSubjectList.Any(ws => ws.IsBaseline == true))
            //        weights = rankingModel.WeightSubjectList.Where(ws => ws.IsBaseline == true).First();
            //    else
            //        weights = rankingModel.WeightSubjectList.First();
            //}
            //else
            //{
            //    weights = GetRandomWeightSubjectForTest(rankingModel);
            //}
            //rankingModel.ChoosenWeightGroupId = weights.WeightSubjectId;

            weights = rankingModel.WeightSubjectList.Where(ws => ws.WeightSubjectId == rankingModel.ChoosenWeightSubjectId).FirstOrDefault();

            allWeights.Add(CampusType.Online, new Dictionary<FactorType, decimal>());
            allWeights.Add(CampusType.Ground, new Dictionary<FactorType, decimal>());

            onlineCapWeight = weights.FactorWeightList.Where(fw => fw.FactorType == FactorType.Cap).First().OnlineWeight / 100;
            onlineeRPLWeight = weights.FactorWeightList.Where(fw => fw.FactorType == FactorType.eRPL).First().OnlineWeight / 100;
            onlineRPCWeight = weights.FactorWeightList.Where(fw => fw.FactorType == FactorType.RPC).First().OnlineWeight / 100;
            campusCapWeight = weights.FactorWeightList.Where(fw => fw.FactorType == FactorType.Cap).First().CampusWeight / 100;
            campuseRPLWeight = weights.FactorWeightList.Where(fw => fw.FactorType == FactorType.eRPL).First().CampusWeight / 100;
            campusRPCWeight = weights.FactorWeightList.Where(fw => fw.FactorType == FactorType.RPC).First().CampusWeight / 100;
            campusDistanceWeight = weights.FactorWeightList.Where(fw => fw.FactorType == FactorType.CampusDistance).First().CampusWeight / 100;


            allWeights[CampusType.Online].Add(FactorType.Cap, onlineCapWeight);
            allWeights[CampusType.Online].Add(FactorType.eRPL, onlineeRPLWeight);
            allWeights[CampusType.Online].Add(FactorType.RPC, onlineRPCWeight);
            allWeights[CampusType.Online].Add(FactorType.CampusDistance, 0);

            allWeights[CampusType.Ground].Add(FactorType.Cap, campusCapWeight);
            allWeights[CampusType.Ground].Add(FactorType.eRPL, campuseRPLWeight);
            allWeights[CampusType.Ground].Add(FactorType.RPC, campusRPCWeight);
            allWeights[CampusType.Ground].Add(FactorType.CampusDistance, campusDistanceWeight);

            return allWeights;
        }

                                                         //ScoreProgramProduct(matchItem,          factorValues,                                 rankingModel,                                                                      allWeights, maxCapRoom.Value.CapRoom, maxeRPL.Value.eRPL, maxClickPrice.Value.ClickPrice, maxCampusDistance.Value.CampusDistance, minCampusDistance.Value.CampusDistance, singleApp);
        private Tuple<decimal, List<MatchItemFactorScore>> ScoreProgramProduct(MatchItem matchItem, Dictionary<int, FactorValueSet> factorValues, BusinessModel rankingModel, Dictionary<CampusType, Dictionary<FactorType, decimal>> allWeights, decimal maxCapRoom, decimal maxeRPL, decimal maxClickPrice, decimal? maxCampusDistance, decimal? minCampusDistance, VW_Matching_Application application, bool hasPostalCode)
        {
            FactorValueSet factorValueItem = factorValues[matchItem.Match.ProgramProductId];
            Tuple<decimal, List<MatchItemFactorScore>> finalRankScore;
            decimal totalScore = 0;
            List<MatchItemFactorScore> factorScores = new List<MatchItemFactorScore>();

            CampusType campusTypeForWeights = CampusType.Online;
            decimal? localCampusDistance = factorValueItem.CampusDistance;

            //per Sandesh - if we are using the distancecliff values, always treat all programs as ground
            if (rankingModel.UseDistanceCliffFormula && rankingModel.DistanceCliffValue.HasValue && rankingModel.DistanceCapMultiplier.HasValue && hasPostalCode)
            {
                if(maxCampusDistance.HasValue && minCampusDistance.HasValue) //if all programs are online, then max/min will have no value, must keep SRA to use online weights
                    campusTypeForWeights = CampusType.Ground;

                if((CampusType)matchItem.Match.CampusCampusTypeId == CampusType.Online)
                    localCampusDistance = maxCampusDistance;
            }
            else
                campusTypeForWeights = (CampusType)matchItem.Match.CampusCampusTypeId;

            if (factorValueItem != null)
            {
                decimal campusDistanceCalc = 0;

                if (maxCampusDistance.HasValue && maxCampusDistance.Value > 0 && minCampusDistance.HasValue && localCampusDistance.HasValue)
                {
                    if (maxCampusDistance.Value == 0)
                        campusDistanceCalc = 1;
                    else
                    {
                        decimal factorValueCampusDistance = localCampusDistance.Value;

                        if ((CampusType)matchItem.Match.CampusCampusTypeId == CampusType.Online)
                            factorValueCampusDistance = rankingModel.DistanceCliffValue.Value;

                        if (rankingModel.UseDistanceCliffFormula && rankingModel.DistanceCliffValue.HasValue && rankingModel.DistanceCliffValue > 0)
                        {
                            decimal distanceCap = (decimal)(rankingModel.DistanceCapMultiplier.Value * rankingModel.DistanceCliffValue.Value);
                            decimal maxDistance = Math.Min(Math.Max(maxCampusDistance.Value, (decimal)rankingModel.DistanceCliffValue.Value), distanceCap);
                            decimal minDistance = Math.Min(minCampusDistance.Value, (decimal)rankingModel.DistanceCliffValue.Value);                         

                            campusDistanceCalc = (maxDistance + minDistance - Math.Min(factorValueCampusDistance, distanceCap))
                                                  /
                                                  maxDistance;
                        }
                        else
                        {
                            //formula via Rakhee TFS#54990
                            campusDistanceCalc = ((maxCampusDistance.Value + minCampusDistance.Value - localCampusDistance.Value)  / maxCampusDistance.Value);
                            //campusDistanceCalc = (1 - factorValueItem.CampusDistance.Value / maxCampusDistance.Value); //original formula
                        }
                    }
                }

                decimal eRPLScore;
                decimal rpcScore;
                decimal eRPLWeight = allWeights[campusTypeForWeights][FactorType.eRPL];
                decimal rpcWeight = allWeights[campusTypeForWeights][FactorType.RPC];

                if (application != null && application.AllowClicks)
                {
                    if (matchItem.Match.PaidStatusTypeId.HasValue && matchItem.Match.PaidStatusTypeId.Value != PaidStatusType.Free)
                    {
                        bool hasClick = factorValueItem.ClickPrice > 0 && !String.IsNullOrWhiteSpace(matchItem.Match.ClickThroughUrl) && matchItem.Match.ClickThroughUrl.Length > 0;

                        if (factorValueItem.eRPL > 0 && hasClick) //program has both click and lead
                        {
                            //eRPLScore = ((factorValueItem.eRPL / maxeRPL) * _gsRPLWeight + (factorValueItem.ClickPrice / maxClickPrice) * _gsClickPriceWeight) * eRPLWeight;
                            eRPLScore = (factorValueItem.eRPL / maxeRPL) * eRPLWeight;
                            rpcScore = (factorValueItem.ClickPrice / maxClickPrice) * rpcWeight;
                        }
                        else if (factorValueItem.eRPL > 0 && !hasClick) //program just has lead
                        {
                            //eRPLScore = ((factorValueItem.eRPL / maxeRPL) * _gsRPLWeight + (maxClickPrice / maxClickPrice) * _gsClickPriceWeight) * eRPLWeight;
                            eRPLScore = (factorValueItem.eRPL / maxeRPL) * eRPLWeight;
                            rpcScore = ((maxClickPrice * 0.5M) / maxClickPrice) * rpcWeight; //requested by Sandesh 
                        }
                        else if (hasClick) //program just has click
                        {
                            //eRPLScore = ((0 / maxeRPL) * _gsRPLWeight + (factorValueItem.ClickPrice / maxClickPrice) * _gsClickPriceWeight) * eRPLWeight;
                            eRPLScore = (0 / maxeRPL) * eRPLWeight;
                            rpcScore = (factorValueItem.ClickPrice / maxClickPrice) * rpcWeight;
                        }
                        else //0 RPL and 0 Click Price - something is wrong!
                        {
                            eRPLScore = 0;
                            rpcScore = 0;
                        }
                    }
                    else
                    {
                        eRPLScore = 0;
                        rpcScore = 0;
                    }
                }
                else
                {
                    eRPLScore = (factorValueItem.eRPL / maxeRPL) * eRPLWeight;
                    rpcScore = 0;
                }

                decimal capScore = ((factorValueItem.CapRoom / maxCapRoom) * allWeights[campusTypeForWeights][FactorType.Cap]);
                decimal distanceScore = (campusDistanceCalc * allWeights[campusTypeForWeights][FactorType.CampusDistance]);

                factorScores.Add(new MatchItemFactorScore() { BusinessModelFactorId = FactorType.RPC, FactorScore = rpcScore, FactorValue = factorValueItem.ClickPrice });
                factorScores.Add(new MatchItemFactorScore() { BusinessModelFactorId = FactorType.eRPL, FactorScore = eRPLScore, FactorValue = factorValueItem.eRPL });
                factorScores.Add(new MatchItemFactorScore() { BusinessModelFactorId = FactorType.Cap, FactorScore = capScore, FactorValue = factorValueItem.CapRoom });
                factorScores.Add(new MatchItemFactorScore() { BusinessModelFactorId = FactorType.CampusDistance, FactorScore = distanceScore, FactorValue = factorValueItem.CampusDistance });
                factorScores.Add(new MatchItemFactorScore() { BusinessModelFactorId = FactorType.Strategic, FactorScore = Convert.ToInt32(factorValueItem.Strategic), FactorValue = Convert.ToInt32(factorValueItem.Strategic) });

                totalScore = eRPLScore + rpcScore + capScore + distanceScore + Convert.ToInt32(factorValueItem.Strategic);
            }

            finalRankScore = new Tuple<decimal, List<MatchItemFactorScore>>(totalScore, factorScores);

            return finalRankScore;
        }

        private int GetRandomWeightSubjectIdForTest(BusinessModel businessModel)
        {
            WeightSubject choosenWeightSubject = null;
            Dictionary<WeightSubject, Tuple<int, int>> percentToShow = new Dictionary<WeightSubject, Tuple<int, int>>(); //<weightSubjectId, <startrange, endrange>>
            Random rand = new Random();
            int startRandRange = 1;

            int diceRoll = rand.Next(1, 100);

            foreach (WeightSubject weightSubject in businessModel.WeightSubjectList)
            {
                percentToShow.Add(weightSubject, new Tuple<int, int>(startRandRange, startRandRange + weightSubject.PercentToShow - 1));
                startRandRange += weightSubject.PercentToShow;
            }

            choosenWeightSubject = percentToShow.Where(p => p.Value.Item1 <= diceRoll && p.Value.Item2 >= diceRoll).First().Key;

            return choosenWeightSubject.WeightSubjectId;
        }

        private Dictionary<int, FactorValueSet> GetRankFactorValues(List<MatchItem> matchItemList, string leadPostalCode, int? leadStateId = null, List<int> specialties = null)
        {
            StartLogDetail("GetRankFactorValues");

            Dictionary<int, FactorValueSet> factorValues = new Dictionary<int, FactorValueSet>();
            Dictionary<int, decimal?> campusDistanceList = new Dictionary<int, decimal?>();

            Dictionary<int, List<StatePricing>> statePriceDict = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<StatePricing>>>(MatchingCacheItem.StatePricing);

            foreach (var matchItem in matchItemList.DistinctBy(mi => mi.Match.ProgramProductId))
            {
                //decimal capRoom = GetCapRoom(matchItem.ClientRelationProductMappingId);
                //Tuple<decimal, decimal> eRPLAndClickPrice = GeteRPLAndClickPrice(matchItem.ProgramProductId);
                //decimal eRPL = eRPLAndClickPrice.Item1;
                //decimal clickPrice = eRPLAndClickPrice.Item2;

                decimal eRPL = matchItem.Match.eRPL;
                bool strategic = GetStrategic(matchItem.Match.ProgramProductId);
                decimal? campusDistance = null;

                //Some specialties will be passed to trigger strategic factor (boost priority)
                if (specialties != null) 
                {
                    if(matchItem.Match.PrimarySpecialtyId.HasValue && specialties.Contains(matchItem.Match.PrimarySpecialtyId.Value))
                    {
                        strategic = true;
                    }
                }

                if (!String.IsNullOrWhiteSpace(leadPostalCode) && (CampusType)matchItem.Match.ProgramCampusTypeId == CampusType.Ground)
                {
                    if (!campusDistanceList.TryGetValue(matchItem.Match.CampusId, out campusDistance))
                    {
                        campusDistance = GetCampusDistance(matchItem.Match.CampusId, leadPostalCode);
                        campusDistanceList.Add(matchItem.Match.CampusId, campusDistance);
                    }
                }

                if(leadStateId.HasValue)
                {
                    if(statePriceDict.ContainsKey(matchItem.Match.ProgramProductId))
                    {
                        StatePricing statePrice = statePriceDict[matchItem.Match.ProgramProductId].Where(pr => pr.StateId == leadStateId.Value).FirstOrDefault();

                        if (statePrice != null)
                            eRPL = statePrice.eRPLFinal;
                    }
                }

                factorValues.Add(matchItem.Match.ProgramProductId, new FactorValueSet() { CapRoom = matchItem.Match.CapRoom, eRPL = eRPL, ClickPrice = matchItem.Match.ClickPrice, CampusDistance = campusDistance, Strategic = strategic });
            }

            EndLogDetail();

            return factorValues;
        }

        private decimal? GetCampusDistance(int campusId, string leadPostalCode)
        {
            decimal? campusDistanceValue = null;

            CampusContent foundCampus = CampusList.Where(c => c.CampusId == campusId && c.PostalCode != null).FirstOrDefault();

            if (foundCampus != default(CampusContent) && (foundCampus.CountryId == 4 || foundCampus.CountryId == 5)) //only US or Canada
            {
                GeoCodeProcessor geo = new GeoCodeProcessor();
                double? distance = geo.GetDistanceBetweenZipCodes(leadPostalCode, foundCampus.PostalCode);

                if (distance.HasValue)
                    campusDistanceValue = Convert.ToDecimal(distance);
            }

            return campusDistanceValue;
        }

        //private decimal GetCapRoom(int clientRelationProductMappingId)
        //{
        //    decimal capRoomValue = default(decimal);
        //    Dictionary<int, Cap> caps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Cap>>(MatchingCacheItem.RECaps);

        //    if (caps.ContainsKey(clientRelationProductMappingId))
        //    {
        //        Cap c = caps[clientRelationProductMappingId];
        //        capRoomValue = c.CapPercentFromCap;
        //    }

        //    return capRoomValue;
        //}

        //private Tuple<decimal, decimal> GeteRPLAndClickPrice(int programProductId)
        //{
        //    Tuple<decimal, decimal> eRPLAndClickPriceValue = new Tuple<decimal,decimal>(default(decimal), default(decimal));

        //    if (eRPLList.ContainsKey(programProductId))
        //    {
        //        var rpl = eRPLList[programProductId];
        //        eRPLAndClickPriceValue = new Tuple<decimal, decimal>(rpl.eRPLFinal, rpl.ClickPrice);
        //    }

        //    return eRPLAndClickPriceValue;
        //}

        private bool GetStrategic(int programProductId)
        {
            bool strategicValue = false;

            Strategic foundStrat = StrategicList.Where(s => s.ProgramProductId == programProductId).FirstOrDefault();

            if (foundStrat != default(Strategic))
                strategicValue = foundStrat.StrategicValue;

            return strategicValue;
        }
    }
}
