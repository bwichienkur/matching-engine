using EDDY.IS.Base;
using EDDY.IS.Core.Logging;
using EDDY.IS.ExternalMatch.Base;
using EDDY.IS.MatchingEngine.Constants;
using EDDY.IS.MatchingEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public class ThirdPartyMatchProcessor
    {
        BaseMatchRequest _matchRequest;
        PerformanceLog _pLog;
        Campaign _campaign;
        List<ProgramWithInstitutionCampus> _previousMatchList;
        UserSelectionResult _userSelectionResult;

        public ThirdPartyMatchProcessor(BaseMatchRequest request, PerformanceLog pLog, List<ProgramWithInstitutionCampus> previousMatchList)
        {
            _matchRequest = request;
            _pLog = pLog;
            _campaign = Campaign.Get(request.TrackGuid);
            _previousMatchList = previousMatchList;
            _userSelectionResult = null;
        }

        private bool CanCallAggregatorAPIs()
        {
            if (_matchRequest == null)
                return false;
            else if (_matchRequest.ProspectInput == null)
                return false;
            else if (!_matchRequest.ProspectInput.EducationLevelId.HasValue ||
                     !_matchRequest.ProspectInput.HSGraduationYear.HasValue ||
                     _matchRequest.CategoryList == null || _matchRequest.CategoryList.Count == 0)
                return false;
            else
                return true;
                    
        }
        public ThirdPartyMatchProcessor()
        {  }

        public List<ProgramWithInstitutionCampus> GetSmartMatchList()
        {
            List<ProgramWithInstitutionCampus> smList = new List<ProgramWithInstitutionCampus>();

            _pLog.StartLogDetail("ThirdPartyMatchProcessor.DetermineLeadTypeForSmartMatch");
            InstitutionLeadTypes? thirdPartyLeadType = DetermineLeadTypeForSmartMatch();
            _pLog.EndLogDetail();

            if (thirdPartyLeadType != null)
            {
                List<MatchItem> finalMatchList = new List<MatchItem>();

                _pLog.StartLogDetail("ThirdPartyMatchProcessor.GetMatchItemList");
                List<MatchItem> matchItemList = GetMatchItemList(thirdPartyLeadType.Value);
                _pLog.EndLogDetail();

                if (matchItemList != null && matchItemList.Any())
                {
                    _pLog.StartLogDetail("ThirdPartyMatchProcessor.GetApiMatchList");
                    if (thirdPartyLeadType == InstitutionLeadTypes.ThirdPartyApiMatch)
                        finalMatchList = GetApiMatchList(matchItemList, _campaign.MaxSmartMatchCount, LeadCreationType.WizardSmartMatchThirdParty);
                    else
                        finalMatchList = matchItemList; //Limbo
                    _pLog.EndLogDetail();

                    _pLog.StartLogDetail("ThirdPartyMatchProcessor.MapPrograms");
                    foreach (var programItem in finalMatchList)
                    {
                        ProgramWithInstitutionCampus program = MatchItemMapper.MapToProgramWithInstitutionCampus(programItem, null, null, false, thirdPartyLeadType.Value);

                        smList.Add(program);
                    }
                    _pLog.EndLogDetail();
                }
                
            }

            return smList;
        }

        public List<MatchItem> GetMatchItems(LeadCreationType leadCreationType)
        {
            List<MatchItem> finalMatchList = new List<MatchItem>();

            InstitutionLeadTypes thirdPartyLeadType = InstitutionLeadTypes.ThirdPartyApiMatch;

            int? matchesAllowed = null;

            if (_campaign.ActiveCampaign && !_campaign.IsCampaignCapped())
            {
                //must be school selection (not Apollo)
                if (leadCreationType == LeadCreationType.WizardUserSelectionThirdParty)
                {
                    //check campaign setting for school selection
                    if (!(_campaign.CampaignAPIMatchBehavior == CampaignAPIMatchBehavior.SchoolSelectionOnly || _campaign.CampaignAPIMatchBehavior == CampaignAPIMatchBehavior.SmartMatchAndSchoolSelection))
                        return finalMatchList;

                    //if school selection, then use school selection max value, if apollo request, then no max value
                    matchesAllowed = _campaign.MaxItemsDisplayed.Value;
                }

                List<MatchItem> matchItemList = GetMatchItemList(thirdPartyLeadType);

                if (matchItemList != null && matchItemList.Any())
                {
                    finalMatchList = GetApiMatchList(matchItemList, matchesAllowed, leadCreationType);
                }
            }
       
            return finalMatchList;
        }

        private List<MatchItem> GetApiMatchList(List<MatchItem> matchList, int? matchesRequested, LeadCreationType leadcreationType)
        {
            List<ApiMatchResult> apiMatchResults = new List<ApiMatchResult>();
            List<MatchItem> finalMatchList = new List<MatchItem>();

            if (CanCallAggregatorAPIs())
            {
                HashSet<int> allProducts = _campaign.GetAllAllowedProducts(_matchRequest.Application);

                List<MatchItem> filteredApiMatchList = new List<MatchItem>(matchList.Where(m => allProducts.Contains(m.Match.ProductId)));

                List<EddyInstitution> matchedInstitutionList = new List<EddyInstitution>();

                if (_previousMatchList != null && _previousMatchList.Any())
                    matchedInstitutionList = _previousMatchList.Select(f => new EddyInstitution() { InstitutionName = f.InstitutionName, InstitutionId = f.InstitutionId }).ToList();

                if (_userSelectionResult != null && _userSelectionResult.InstitutionList != null && _userSelectionResult.InstitutionList.Any())
                    matchedInstitutionList.AddRange(_userSelectionResult.InstitutionList);

                if (filteredApiMatchList.Any())
                {
                    apiMatchResults = ExternalMatchServiceClient.GetMatches(_matchRequest, matchesRequested, filteredApiMatchList, matchedInstitutionList, _campaign);

                    var programProductMatchList = from a in apiMatchResults
                                                  join m in filteredApiMatchList
                                                  on new { a.ClientRelationshipId, a.InstitutionCode } equals new { m.Match.ClientRelationshipId, InstitutionCode = m.Match.ProgramCode }
                                                  select new { ProgramProductId = m.Match.ProgramProductId, ApiMatch = a };

                    int counter = 0;

                    if (leadcreationType == LeadCreationType.WizardSmartMatchThirdParty && _previousMatchList != null)
                        counter = _previousMatchList.Count();

                    foreach (var ppMatch in programProductMatchList)
                    {
                        var matchItemFromThirdParty = filteredApiMatchList.Where(mi => mi.Match.ProgramProductId == ppMatch.ProgramProductId).First();

                        MatchItem newMatchItem = new MatchItem((MatchItemInternal)matchItemFromThirdParty.Match.Clone());

                        newMatchItem.Match.InstitutionName = ppMatch.ApiMatch.InstitutionName;
                        newMatchItem.Match.ProgramName = ppMatch.ApiMatch.ProgramName;
                        newMatchItem.ExternalMatchItemGuid = ppMatch.ApiMatch.ExternalMatchItemGuid;

                        newMatchItem.InstitutionLeadType = InstitutionLeadTypes.ThirdPartyApiMatch;
                        newMatchItem.FailedValidation = false;
                        newMatchItem.AllowedViaLeadScoringUpsell = false;
                        newMatchItem.ProgramRankScore = 0;
                        newMatchItem.AllowedViaLeadScoringUpsell = false;
                        newMatchItem.Match.RPL = ppMatch.ApiMatch.VendorCPL.HasValue ? ppMatch.ApiMatch.VendorCPL.Value : 0;

                        finalMatchList.Add(newMatchItem);

                        counter++;

                        if (matchesRequested.HasValue && counter >= matchesRequested)
                            break;
                    }
                }
            }

            return finalMatchList;
        }

        private List<MatchItem> GetMatchItemList(InstitutionLeadTypes leadType)
        {
            List<MatchItem> matchItemList = GetFullMatchItemList(leadType);

            List<MatchItem> finalMatchList = null;

            if (matchItemList != null && matchItemList.Count > 0)
            {
                //Core.Logging.PerformanceLog pLog = new Core.Logging.PerformanceLog(Base.ISApplication.MatchingEngine, "Rules", null);
                RulesEngine re = new RulesEngine(_matchRequest.ProspectInput, _campaign, _matchRequest.Application, false, _pLog, false, LeadCreationType.WizardSmartMatchThirdParty);
                RulesResult rr = re.ExecuteRules(matchItemList);

                matchItemList = rr.MatchedProgramProductList.Where(item => item.InstitutionLeadType == leadType && item.FailedValidation == false).ToList();

                if (_previousMatchList != null && _previousMatchList.Any())
                {
                    var instIdList = _previousMatchList.Select(p => p.InstitutionId);

                    matchItemList = matchItemList.Where(item => (!instIdList.Contains(item.Match.InstitutionId)) || (item.InstitutionLeadType == InstitutionLeadTypes.ThirdPartyApiMatch && instIdList.Contains(item.Match.InstitutionId))).ToList();
                }

                if (matchItemList.Any())
                    finalMatchList = matchItemList;
            }

            return finalMatchList;
        }

        private List<MatchItem> GetFullMatchItemList(InstitutionLeadTypes leadType)
        {
            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);
            return md.GetThirdPartyMatches(new InstitutionLeadTypes[] { leadType });
        }

        public MatchItem GetSingleThirdPartyMatchItem(int programProductid)
        {
            MatchItem mi = null;

            List<MatchItem> matchItemList = GetFullMatchItemList(InstitutionLeadTypes.ThirdPartyApiMatch);

            if(matchItemList != null)
            {
                mi = matchItemList.Where(m => m.Match.ProgramProductId == programProductid).FirstOrDefault();
            }

            return mi;
        }

        private InstitutionLeadTypes? DetermineLeadTypeForSmartMatch()
        {
            if (LimboAllowed())
                return InstitutionLeadTypes.Limbo;
            else
            {
                if(_previousMatchList.Count < _campaign.MaxSmartMatchCount
                    && !_previousMatchList.Any(m => Product.ExclusiveProducts.Contains(m.ProductId.Value))
                    && !_previousMatchList.Any(m => m.TreatAsMatch1))
                {
                    if (_campaign.CampaignAPIMatchBehavior == CampaignAPIMatchBehavior.SchoolSelectionOnly)
                        return null;

                    DetermineUserSelections();

                    if (_campaign.CampaignAPIMatchBehavior == CampaignAPIMatchBehavior.SmartMatchAndSchoolSelection
                        || _campaign.CampaignAPIMatchBehavior == CampaignAPIMatchBehavior.SmatchMatchAnyScenario)
                        return InstitutionLeadTypes.ThirdPartyApiMatch;

                    //For CampaignAPIMatchBehavior.SmartMatchOnlyIfHasEddySmartMatchOrSchoolSelection - allow API matches if smart match from eddy also
                    if (_previousMatchList.Count > 0)
                        return InstitutionLeadTypes.ThirdPartyApiMatch;                    

                    //For CampaignAPIMatchBehavior.SmartMatchOnlyIfHasEddySmartMatchOrSchoolSelection - allow API matches if there will be EDDY school selection
                    if (_userSelectionResult.HasUserSelections)
                        return InstitutionLeadTypes.ThirdPartyApiMatch;
                    else
                        return null;
                }
                return null;
            }
        }

        private bool LimboAllowed()
        {
            if((_previousMatchList == null || _previousMatchList.Count == 0) && _campaign.AllowRemonetization)
            {
                DetermineUserSelections();

                if (_userSelectionResult.HasUserSelections)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        private void DetermineUserSelections()
        {
            if (_userSelectionResult != null)
                return;

            MatchingEngine engine = new MatchingEngine(new Core.Logging.PerformanceLog());

            _userSelectionResult = new UserSelectionResult();

            List<EddyInstitution> instNameList = new List<EddyInstitution>();

            List<int> tempTemplateList = null;

            WizardMatchRequest request = (WizardMatchRequest)_matchRequest;

            if (request.TemplateList != null)
                tempTemplateList = new List<int>(request.TemplateList);

            request.IncludeSmartMatchList = false;
            request.IncludeSchoolSelectionList = true;
            request.LeadCreationType = LeadCreationType.WizardUserSelection;
            request.TemplateList = null;

            MatchResult results = engine.GetWizardMatches(request);

            //switch back to orginal values for ME request logging accuracy
            request.IncludeSmartMatchList = true;
            request.IncludeSchoolSelectionList = false;
            request.LeadCreationType = LeadCreationType.WizardSmartMatch;
            request.TemplateList = tempTemplateList;

            var schoolSelectionList = results.MatchItemList.Where(mi => mi.FailedValidation == false);

            if (schoolSelectionList.Count() > 0)
            {
                _userSelectionResult.InstitutionList = schoolSelectionList.GroupBy(mi => mi.Match.InstitutionId).Select(mi => new EddyInstitution() { InstitutionName = mi.First().Match.InstitutionName, InstitutionId = mi.First().Match.InstitutionId }).ToList();
                _userSelectionResult.HasUserSelections = true;
            }
            else
                _userSelectionResult.HasUserSelections = false;

            return;
        }

    }

    public class UserSelectionResult
    {
        public bool HasUserSelections { get; set; }

        public List<EddyInstitution> InstitutionList { get; set; }
    }
}
