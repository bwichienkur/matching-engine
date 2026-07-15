using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using EDDY.IS.MatchingEngine.BusinessLayer;
using EDDY.IS.MatchingEngine.DTO;
//using EDDY.IS.MatchingEngine.Service;
using EDDY.IS.Base.Util;
using System.Diagnostics;
using System.Configuration;
using EDDY.IS.Core.Logging.DataModel;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.Core;
using EDDY.IS.Base;
using EDDY.IS.MatchingEngine.Constants;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine
{
    public class MatchingEngine : Engine
    {
        public MatchingEngine()
        { }

        public MatchingEngine(EDDY.IS.Core.Logging.PerformanceLog perf)
            : base(perf)
        { }

        public ProgramDetail GetProgramDetails(int applicationId, Guid TrackGuid, int programId, bool? includeProgramGroupRollup, int? campusId)
        {
            return GetProgramDetail(applicationId, TrackGuid, programId, includeProgramGroupRollup, campusId);
        }

        public ProgramDetail GetProgramDetails(Guid TrackGuid, int programId, bool? includeProgramGroupRollup, int? campusId)
        {
            return GetProgramDetail(0, TrackGuid, programId, includeProgramGroupRollup, campusId);
        }

        public ProgramDetail GetProgramGroupDetails(int applicationId, Guid TrackGuid, int programDisplayGroupId)
        {
            Dictionary<int, ProgramDisplayGroup> cacheDisplayGroupList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, ProgramDisplayGroup>>(MatchingCacheItem.ProgramDisplayGroups);

            ProgramDisplayGroup cachedGroup = null;

            ProgramDetail programDetail = null;

            if (cacheDisplayGroupList.TryGetValue(programDisplayGroupId, out cachedGroup))
            {
                MatchResult mr2 = GetDirectoryMatches(new DirectoryMatchRequest()
                {
                    TrackGuid = TrackGuid,
                    ProgramGroupId = programDisplayGroupId,
                    RemoveInvalidEntities = false,
                    SortMethod = EntitySortMethod.Alphabetical
                }, setApplicationFromCampaign: true);

                if (mr2 != null && mr2.MatchItemList.Any())
                {
                    programDetail = GetProgramDetail(applicationId, TrackGuid, mr2.MatchItemList.First().Match.ProgramId, false, null);

                    programDetail.ProgramDisplayGroupName = cachedGroup.GroupName;

                    if (!String.IsNullOrWhiteSpace(cachedGroup.GroupDescription))
                        programDetail.ProgramDisplayGroupDescription = cachedGroup.GroupDescription;

                    programDetail.ProgramDisplayGroupProgramList = mr2.MatchItemList.Select(mi => new Tuple<int, string>(mi.Match.ProgramId, mi.Match.GradSchoolsDegreeName)).Distinct().Select(item => new IdValuePair() { ItemId = item.Item1, ItemValue = item.Item2 }).ToList();
                }
            }

            return programDetail;
        }

        private ProgramDetail GetProgramDetail(int applicationId, Guid TrackGuid, int programId, bool? includeProgramGroupRollup, int? campusId)
        {
            StartLogDetail("GetProgramDetail");

            ProgramDetail programDetail = null;
            Campaign c = Campaign.Get(TrackGuid);

            if (c != null && c.ActiveCampaign && !c.IsCampaignCapped())
            {
                List<CampusContent> campusContent = StaticCacheProxyHost.CacheProxy.Get<List<CampusContent>>(MatchingCacheItem.CampusContent);

                programDetail = ContentDataService.GetProgramContentDetail(c.ApplicationId.Value, programId);

                if (programDetail != null)
                {
                    MatchResult mr = GetDirectoryMatches(new DirectoryMatchRequest()
                    {
                        TrackGuid = TrackGuid,
                        ProgramIdList = new List<int>() { programId },
                        RemoveInvalidEntities = false,
                        SortMethod = EntitySortMethod.RankScore
                    }, setApplicationFromCampaign: true);


                    if (mr.MatchItemList != null && mr.MatchItemList.Count > 0)
                    {
                        mr.MatchItemList = mr.MatchItemList.OrderBy(f => f.FailedValidation).ThenByDescending(f => f.Match.PaidStatusTypeId).ThenByDescending(f => f.ProgramRankScore).ToList();

                        MatchItem topMatchItem = mr.MatchItemList.First();

                        programDetail.FailedValidation = topMatchItem.FailedValidation;

                        if (programDetail.ProgramCampus != null)
                            programDetail.ProgramCampus.CampusType = (CampusType)mr.MatchItemList.First().Match.CampusCampusTypeId;

                        var clickProgramList = mr.MatchItemList.Where(mi => mi.Match.HasClick && mi.FailedValidation == false);

                        if (clickProgramList != null && clickProgramList.Any())
                        {
                            var matchesToRemove = RulesEngine.ProcessClickProgramRules(clickProgramList);

                            if (matchesToRemove.Any())
                                clickProgramList = clickProgramList.Except(matchesToRemove).ToList();

                            //clickProgramList = RulesEngine.ProcessClickProgramRules(clickProgramList.ToList());
                            if (clickProgramList != null && clickProgramList.Any())
                            {
                                programDetail.ProgramProductIdClick = clickProgramList.First().Match.ProgramProductId;
                                programDetail.ClickThroughUrl = clickProgramList.First().Match.ClickThroughUrl;
                            }
                        }

                        programDetail.ProgramLevelName = topMatchItem.Match.ProgramLevelName;

                        if (String.IsNullOrEmpty(programDetail.ProgramDisclaimer))
                        {
                            programDetail.ProgramDisclaimer = topMatchItem.Match.InstitutionDisclaimer;
                            programDetail.ProgramDisclaimerType = topMatchItem.Match.InstitutionDisclaimerType;
                        }

                        programDetail.InstitutionName = topMatchItem.Match.InstitutionName;

                        programDetail.PaidStatusTypeId = topMatchItem.Match.PaidStatusTypeId;
                        programDetail.IsHybrid = topMatchItem.Match.IsHybrid;
                        programDetail.InquiryDisabled = topMatchItem.Match.InquiryDisabled;
                        programDetail.TemplateId = topMatchItem.Match.TemplateId;
                        programDetail.ProgramDisplayGroupId = topMatchItem.Match.ProgramDisplayGroupId;
                        programDetail.ProgramDescription = topMatchItem.Match.ProgramDescription;
                        programDetail.ShortDescription = topMatchItem.Match.ProgramShortDescription;
                        programDetail.CustomTCPA = topMatchItem.Match.CustomTCPA;
                        programDetail.CustomContactCenterTCPA = topMatchItem.Match.CustomContactCenterTCPA;

                        //programDetail.CampusIdCanonical = mr.MatchItemList.OrderBy(mi => mi.Match.CampusId).First().Match.CampusId;

                        if (applicationId == 7) //gradschools
                        {
                            var joinedTopCampus = from mi in mr.MatchItemList
                                                  join st in StaticInternalObject.States_GradSchools_Priority
                                                  on mi.Match.CampusStateId equals st.Key
                                                  orderby st.Value
                                                  select mi.Match.CampusId;

                            if (joinedTopCampus != null && joinedTopCampus.Any())
                                programDetail.CampusIdCanonical = joinedTopCampus.FirstOrDefault();
                            else
                                programDetail.CampusIdCanonical = mr.MatchItemList.OrderBy(mi => mi.Match.CampusId).First().Match.CampusId;
                        }

                        Campus programCampus = new Campus();

                        bool useProgramCampus = true;

                        if (campusId.HasValue)
                        {
                            var campusList = mr.MatchItemList.Where(mi => mi.Match.CampusId == campusId.Value);

                            if (campusList != null && campusList.Any())
                            {
                                programCampus.Address1 = campusList.First().Match.CampusAddress1;
                                programCampus.Address2 = campusList.First().Match.CampusAddress2;
                                programCampus.City = campusList.First().Match.CampusCity;
                                programCampus.State = campusList.First().Match.CampusStateCode;
                                programCampus.PostalCode = campusList.First().Match.CampusPostalCode;
                                programCampus.Phone = campusList.First().Match.CampusPhone;
                                programCampus.Fax = campusList.First().Match.CampusFax;
                                programCampus.CountryCode = campusList.First().Match.CampusCountry;
                                programCampus.CountryName = campusList.First().Match.CampusCountryName;
                                //programCampus.HasCampusLogo = campusList.First().Match.HasCampusLogo;
                                programCampus.CampusLogoURL = campusList.First().Match.CampusLogoURL;
                                programCampus.InstitutionLogoURL = campusList.First().Match.InstitutionLogoURL;
                                programCampus.CampusId = campusList.First().Match.CampusId;
                                programCampus.CampusName = campusList.First().Match.CampusName;
                                programCampus.CampusType = (CampusType)campusList.First().Match.CampusCampusTypeId;

                                useProgramCampus = false;
                            }
                        }

                        if (useProgramCampus)
                        {
                            programCampus.Address1 = topMatchItem.Match.CampusAddress1;
                            programCampus.Address2 = topMatchItem.Match.CampusAddress2;
                            programCampus.City = topMatchItem.Match.CampusCity;
                            programCampus.State = topMatchItem.Match.CampusStateCode;
                            programCampus.PostalCode = topMatchItem.Match.CampusPostalCode;
                            programCampus.Phone = topMatchItem.Match.CampusPhone;
                            programCampus.Fax = topMatchItem.Match.CampusFax;
                            programCampus.CountryCode = topMatchItem.Match.CampusCountry;
                            programCampus.CountryName = topMatchItem.Match.CampusCountryName;
                            //programCampus.HasCampusLogo = topMatchItem.Match.HasCampusLogo;
                            programCampus.CampusLogoURL = topMatchItem.Match.CampusLogoURL;
                            programCampus.InstitutionLogoURL = topMatchItem.Match.InstitutionLogoURL;
                            programCampus.CampusId = topMatchItem.Match.CampusId;
                            programCampus.CampusName = topMatchItem.Match.CampusName;
                            programCampus.CampusType = (CampusType)topMatchItem.Match.CampusCampusTypeId;
                        }

                        programDetail.ProgramCampus = programCampus;

                        if (topMatchItem.Match.ProgramAddressList != null)
                            programDetail.ProgramAddressList = topMatchItem.Match.ProgramAddressList;
                        else
                            programDetail.ProgramAddressList = new List<Address>();

                        if (mr.MatchItemList.First().Match.ProgramDisplayGroupId != null)
                        {
                            Dictionary<int, ProgramDisplayGroup> cacheDisplayGroupList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, ProgramDisplayGroup>>(MatchingCacheItem.ProgramDisplayGroups);

                            ProgramDisplayGroup cachedGroup = null;

                            if (cacheDisplayGroupList.TryGetValue(topMatchItem.Match.ProgramDisplayGroupId.Value, out cachedGroup))
                            {
                                programDetail.ProgramDisplayGroupName = cachedGroup.GroupName;

                                if (!String.IsNullOrWhiteSpace(cachedGroup.GroupDescription))
                                    programDetail.ProgramDisplayGroupDescription = cachedGroup.GroupDescription;

                                //MatchResult mr2 = GetDirectoryMatches(new DirectoryMatchRequest()
                                //{
                                //    TrackGuid = TrackGuid,
                                //    ProgramGroupId = topMatchItem.Match.ProgramDisplayGroupId.Value,
                                //    RemoveInvalidEntities = false,
                                //    SortMethod = EntitySortMethod.Alphabetical
                                //}, setApplicationFromCampaign: true);

                                ///programDetail.ProgramDisplayGroupProgramList = mr2.MatchItemList.Select(mi => new Tuple<int, string>(mi.Match.ProgramId, mi.Match.GradSchoolsDegreeName)).Distinct().Select(item => new IdValuePair() { ItemId = item.Item1, ItemValue = item.Item2 }).ToList();
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(topMatchItem.Match.GradSchoolsDegreeName))
                            programDetail.ProgramDisplayGroupProgramList = new List<IdValuePair>() { new IdValuePair() { ItemId = topMatchItem.Match.ProgramId, ItemValue = topMatchItem.Match.GradSchoolsDegreeName } };

                        Dictionary<EntityMeta, Dictionary<int, List<Image>>> images = StaticCacheProxyHost.CacheProxy.Get<Dictionary<EntityMeta, Dictionary<int, List<Image>>>>(MatchingCacheItem.Images);

                        if (images != null)
                        {
                            if (images.ContainsKey(EntityMeta.Program) && images[EntityMeta.Program].ContainsKey(programId))
                                programDetail.ImageList = new List<Image>(images[EntityMeta.Program][programId]);
                            else if (images.ContainsKey(EntityMeta.Campus) && images[EntityMeta.Campus].ContainsKey(topMatchItem.Match.CampusId))
                                programDetail.ImageList = new List<Image>(images[EntityMeta.Campus][topMatchItem.Match.CampusId]);
                            else if (images.ContainsKey(EntityMeta.Institution) && images[EntityMeta.Institution].ContainsKey(topMatchItem.Match.InstitutionId))
                                programDetail.ImageList = new List<Image>(images[EntityMeta.Institution][topMatchItem.Match.InstitutionId]);
                            else
                                programDetail.ImageList = new List<Image>();
                        }
                        else
                            programDetail.ImageList = new List<Image>();


                        var miWithSubject = mr.MatchItemList.Where(mi => mi.Match.ProgramTypeId == (int)ProgramType.FullDegree || mi.Match.ProgramTypeId == (int)ProgramType.StudyAbroad);

                        if (miWithSubject.Count() > 0)
                        {
                            List<SubjectContent> subjectContent = StaticCacheProxyHost.CacheProxy.Get<List<SubjectContent>>(MatchingCacheItem.SubjectContent);
                            List<CategoryContent> categoryContent = StaticCacheProxyHost.CacheProxy.Get<List<CategoryContent>>(MatchingCacheItem.CategoryContent);
                            List<SpecialtyContent> specialtyContent = StaticCacheProxyHost.CacheProxy.Get<List<SpecialtyContent>>(MatchingCacheItem.SpecialtyContent);

                            programDetail.SubjectList = (from mi in miWithSubject
                                                         join sc in subjectContent
                                                         on mi.Match.PrimarySubjectId equals sc.SubjectId
                                                         select new IdValuePair() { ItemId = sc.SubjectId, ItemValue = sc.SubjectName }
                                                        ).ToList();
                            programDetail.CategoryList = (from mi in miWithSubject
                                                          join sc in categoryContent
                                                          on mi.Match.PrimaryCategoryId equals sc.CategoryId
                                                          select new IdValuePair() { ItemId = sc.CategoryId, ItemValue = sc.CategoryName }
                                                        ).ToList();
                            programDetail.SpecialtyList = (from mi in miWithSubject
                                                           join sc in specialtyContent
                                                           on mi.Match.PrimarySpecialtyId equals sc.SpecialtyId
                                                           select new IdValuePair() { ItemId = sc.SpecialtyId, ItemValue = sc.SpecialtyName }
                                                        ).ToList();
                        }

                        programDetail.TermList = topMatchItem.Match.TermList.ToList();
                        programDetail.DurationList = topMatchItem.Match.DurationList.ToList();
                        programDetail.PlacementAudienceList = topMatchItem.Match.PlacementAudienceList.ToList();
                        programDetail.TeachAbroadTypeList = topMatchItem.Match.TeachAbroadTypeList.ToList();
                        programDetail.WorkTypeList = topMatchItem.Match.WorkTypeList.ToList();
                        programDetail.LanguageId = topMatchItem.Match.LanguageId;
                        programDetail.ProgramSFProductCode = topMatchItem.Match.SFProductCode_PSI;
                    }
                }
            }
            EndLogDetail();

            return programDetail;
        }

        public InstitutionDetail GetInstitutionDetail(Guid TrackGuid, int institutionId)
        {
            StartLogDetail("GetInstitutionDetail");

            InstitutionDetail instDetail = null;
            Campaign c = Campaign.Get(TrackGuid);

            if (c.ActiveCampaign && !c.IsCampaignCapped() && c.ApplicationId.HasValue)
            {
                Dictionary<int, InstitutionContent> institutionContent = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, InstitutionContent>>(MatchingCacheItem.InstitutionContent);

                if (institutionContent.ContainsKey(institutionId))
                {
                    instDetail = institutionContent[institutionId].CreateDetail();
                }
            }

            if (instDetail != null)
            {
                MatchResult mr = GetDirectoryMatches(new DirectoryMatchRequest()
                {
                    TrackGuid = TrackGuid,
                    InstitutionIdList = new List<int>() { institutionId },
                    RemoveInvalidEntities = false,
                    SortMethod = EntitySortMethod.Alphabetical
                }, setApplicationFromCampaign: true);

                if (mr != null && mr.MatchItemList.Any())
                {
                    instDetail.IsDisabled = false;

                    instDetail.ImageList = mr.MatchItemList.First().Match.InstitutionImageList;

                    if (!mr.MatchItemList.Any(mi => mi.FailedValidation == false))
                        instDetail.FailedValidation = true;
                    else
                        instDetail.FailedValidation = false;

                    if (mr.MatchItemList.Any(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Paid))
                        instDetail.PaidStatusTypeId = PaidStatusType.Paid;
                    else if (mr.MatchItemList.Any(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Fraid))
                        instDetail.PaidStatusTypeId = PaidStatusType.Fraid;
                    else
                        instDetail.PaidStatusTypeId = PaidStatusType.Free;

                    if (mr.MatchItemList.Any(mi => (mi.Match.PaidStatusTypeId == PaidStatusType.Paid || mi.Match.PaidStatusTypeId == PaidStatusType.Fraid) && mi.Match.InquiryDisabled == false))
                        instDetail.OnlyClicksPrograms = false;
                    else
                        instDetail.OnlyClicksPrograms = true;
                }
                else
                {
                    instDetail.IsDisabled = true;
                    instDetail.FailedValidation = true;
                }
            }

            EndLogDetail();
            return instDetail;
        }

        public MatchItem FindUpsellProgramProduct(int programProductId, ProgramValidateRequest pvRequest, Guid trackGuid, ProductType upsellProduct)
        {
            MatchItem match = null;
            DirectoryMatchRequest request = new DirectoryMatchRequest();

            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);
            MatchItem singleMatch = md.GetMatchItemByProgramProductId(programProductId);

            if (singleMatch != null)
            {
                request.ProspectInput = pvRequest.ProspectInput;
                request.ProgramIdList = new List<int>() { singleMatch.Match.ProgramId };
                request.TrackGuid = trackGuid;
                request.Application = pvRequest.Application;

                List<MatchItem> matchItemList = GetFilteredProgramProduct(request, productList: new List<int>() { (int)upsellProduct });
                Campaign c = Campaign.Get(trackGuid);
                ProcessRules(matchItemList, request.ProspectInput, c, request.Application, pvRequest.LeadCreationType);

                List<MatchItem> matchItems = matchItemList.Where(mi => mi.FailedValidation == false).ToList();

                match = matchItems.FirstOrDefault();
            }

            return match;
        }

        public MatchItem FindAlternateProgramProduct(int programId, ProgramValidateRequest pvRequest)
        {
            MatchItem match = null;
            MatchItem initialMatch = null;
            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);
            initialMatch = md.GetMatchItemByProgramProductId(pvRequest.ProgramProductId);

            DirectoryMatchRequest request = new DirectoryMatchRequest();

            request.ProspectInput = pvRequest.ProspectInput;

            request.ProgramIdList = new List<int>() { programId };
            request.TrackGuid = pvRequest.TrackGuid;
            request.Application = pvRequest.Application;

            MatchResult result = GetBaseMatches(request, LeadCreationTypeID: pvRequest.LeadCreationType);

            List<MatchItem> matchItems = result.MatchItemList.Where(mi => mi.FailedValidation == false).ToList();

            if (initialMatch != null)
            {
                if (Product.IsWarmTransferProduct(initialMatch.Match.ProductId))
                    matchItems = matchItems.Where(mi => Product.IsWarmTransferProduct(mi.Match.ProductId)).ToList();
                else
                    matchItems = matchItems.Where(mi => !Product.IsWarmTransferProduct(mi.Match.ProductId)).ToList();
            }

            match = matchItems.FirstOrDefault();

            return match;
        }

        public Tuple<RulesResult, int> GetRulesResult(int programProductId, DTO.ProspectInput pi, bool isCountryOnly, Guid? TrackGuid, IS.Base.ISApplication app, bool ignoreCaps, LeadCreationType? leadCreationType, bool skipWTRules = false)
        {
            StartLogDetail("GetRulesResult");
            RulesResult rr;
            int programId = 0;

            Campaign c = null;
            if (TrackGuid.HasValue && TrackGuid != Guid.Empty)
                c = Campaign.Get(TrackGuid.Value);

            BaseRuleType? ruleType;

            if (c == null || c.IsValid(out ruleType))
            {
                StartLogDetail("GetMatchItem");
                MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

                MatchItem singleMatch = md.GetMatchItemByProgramProductId(programProductId);
                List<MatchItem> matches;

                if (singleMatch == null)
                    matches = new List<MatchItem> { };
                else
                    matches = new List<MatchItem> { singleMatch };
                EndLogDetail();

                programId = matches.Select(m => m.Match.ProgramId).FirstOrDefault();

                StartLogDetail("ApplyCampaignFilters");
                if (c != null)
                {
                    HashSet<int> allProducts = c.GetAllAllowedProducts(app);
                    matches.RemoveAll(m => !allProducts.Contains(m.Match.ProductId));
                    matches = c.ApplyCampaignFilters(matches, app, _performanceLog);
                    matches = matches.Where(mi => mi.FailedValidation == false).ToList();
                }
                EndLogDetail();

                if (matches.Count > 0)
                {
                    if (matches.First().Match.PaidStatusTypeId.Value == PaidStatusType.Paid)
                    {
                        RulesEngine re = new RulesEngine(pi, c, app, isCountryOnly, _performanceLog, ignoreCaps, leadCreationType, skipWTRules: skipWTRules);
                        rr = re.ExecuteRules(matches);
                    }
                    else
                    {
                        rr = new RulesResult();
                        rr.NoMatchOutput = new List<NoMatch>();
                        rr.MatchedProgramProductList = matches;
                    }
                }
                else
                {
                    bool isThirdPartyMatch = false;

                    rr = new RulesResult();

                    if (c.Products != null)
                    {
                        if (c.Products.Count > 0 && c.Products.Contains(93))
                        {
                            ThirdPartyMatchProcessor processor = new ThirdPartyMatchProcessor();

                            MatchItem mi = processor.GetSingleThirdPartyMatchItem(programProductId);

                            if (mi != null)
                            {
                                rr.NoMatchOutput = new List<NoMatch>();
                                rr.MatchedProgramProductList = new List<MatchItem>() { mi };
                                isThirdPartyMatch = true;
                            }
                        }
                    }

                    if (!isThirdPartyMatch)
                    {
                        rr.NoMatchOutput = new List<NoMatch>();
                        rr.NoMatchOutput.Add(new NoMatch() { RuleType = BaseRuleType.ProgramNotAvailable, RuleName = "Program Not Available" });
                    }
                }
            }
            else
            {
                NoMatch NoMatchItem = new NoMatch();
                rr = new RulesResult();
                rr.NoMatchOutput = new List<NoMatch>();

                if (ruleType.HasValue)
                {
                    NoMatchItem.RuleType = (BaseRuleType)ruleType;
                }

                //create a no match item for missing campaign or capped
                if (ruleType.HasValue && ruleType == BaseRuleType.CampaignCapReached)
                {
                    NoMatchItem.RuleName = "Capped Out at the Campaign Level";
                }
                else if (ruleType.HasValue && ruleType == BaseRuleType.CampaignInactive)
                {
                    //campaign passed was not active or failed lead scoring 
                    NoMatchItem.RuleName = "Campaign is Not Active or Failed Scoring Criteria";

                }
                else
                {
                    //campaign passed failed lead scoring
                    NoMatchItem.RuleName = "Campaign Failed Lead Scoring Requirements";

                }

                rr.NoMatchOutput.Add(NoMatchItem);
            }
            EndLogDetail();
            return new Tuple<RulesResult, int>(rr, programId);
        }
        public Tuple<RulesResult, int> GetProgramProductRulesResult(int programProductId, DTO.ProspectInput pi, Guid? TrackGuid, EDDY.IS.Base.ISApplication app, LeadCreationType? leadCreationType)
        {
            StartLogDetail("GetRulesResult");
            RulesResult rr;
            int programId = 0;

            Campaign c = null;
            if (TrackGuid.HasValue && TrackGuid != Guid.Empty)
                c = Campaign.Get(TrackGuid.Value);

            BaseRuleType? ruleType;

            if (c == null || c.IsValid(null, out ruleType))
            {
                StartLogDetail("GetMatchItem");
                MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

                MatchItem singleMatch = md.GetMatchItemByProgramProductId(programProductId);
                List<MatchItem> matches;

                if (singleMatch == null)
                    matches = new List<MatchItem> { };
                else
                    matches = new List<MatchItem> { singleMatch };
                EndLogDetail();

                programId = matches.Select(m => m.Match.ProgramId).FirstOrDefault();

                StartLogDetail("ApplyCampaignFilters");
                if (c != null)
                {
                    HashSet<int> allProducts = c.GetAllAllowedProducts(app);
                    matches.RemoveAll(m => !allProducts.Contains(m.Match.ProductId));
                    matches = c.ApplyCampaignFilters(matches, app, _performanceLog);
                }
                EndLogDetail();

                if (matches.Count > 0)
                {
                    if (matches.First().Match.PaidStatusTypeId.Value == PaidStatusType.Paid)
                    {
                        RulesEngine re = new RulesEngine(pi, c, app, _performanceLog, leadCreationType);
                        rr = re.ExecuteProgramProductRules(matches);
                    }
                    else
                    {
                        rr = new RulesResult();
                        rr.NoMatchOutput = new List<NoMatch>();
                        rr.MatchedProgramProductList = matches;
                    }
                }
                else
                {
                    rr = new RulesResult();
                    rr.NoMatchOutput = new List<NoMatch>();
                    rr.NoMatchOutput.Add(new NoMatch() { RuleType = BaseRuleType.ProgramNotAvailable, RuleName = "Program Not Available" });
                }
            }
            else
            {
                NoMatch NoMatchItem = new NoMatch();
                rr = new RulesResult();
                rr.NoMatchOutput = new List<NoMatch>();

                if (ruleType.HasValue)
                {
                    NoMatchItem.RuleType = (BaseRuleType)ruleType;
                }

                //create a no match item for missing campaign or capped
                if (ruleType.HasValue && ruleType == BaseRuleType.CampaignCapReached)
                {
                    NoMatchItem.RuleName = "Capped Out at the Campaign Level";
                }
                else if (ruleType.HasValue && ruleType == BaseRuleType.CampaignInactive)
                {
                    //campaign passed was not active or failed lead scoring 
                    NoMatchItem.RuleName = "Campaign is Not Active or Failed Scoring Criteria";

                }
                else
                {
                    //campaign passed failed lead scoring
                    NoMatchItem.RuleName = "Campaign Failed Lead Scoring Requirements";

                }

                rr.NoMatchOutput.Add(NoMatchItem);
            }
            EndLogDetail();
            return new Tuple<RulesResult, int>(rr, programId);
        }
        public CrossSellMatchResult GetCrossSellMatches(CrossSellMatchRequest crossSellMatchRequest)
        {
            StartLogDetail("GetCrossSellMatches");

            CrossSellMatchResult matchResults = new CrossSellMatchResult();
            matchResults.CrossSellCategoryMatchItemList = new List<MatchItem>();
            matchResults.CrossSellSubjectMatchItemList = new List<MatchItem>();
            matchResults.CrossSellSpecialtyMatchItemList = new List<MatchItem>();
            Campaign c = Campaign.Get(crossSellMatchRequest.TrackGuid);

            matchResults.ChosenCampaign = c;
            matchResults.MaxProgramsToDisplay = c.MaxItemsDisplayed;
            matchResults.MaxUserSelections = c.MaxSubmissionCount;

            BaseRuleType? ruleType;

            //int? leadScoringTierLevel = crossSellMatchRequest.LeadScoringInput == null ? null : crossSellMatchRequest.LeadScoringInput.LeadScoringTierLevel;

            if (c.IsValid(out ruleType))
            {
                CrossSellProcessor xSellProcessor = new CrossSellProcessor(_performanceLog);

                MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

                MatchItem programProduct = md.GetMatchItemByProgramProductId(crossSellMatchRequest.FormProgramProductId);

                //Don't cross sell for SAB CPL unless the initial lead failed
                if (programProduct != null
                    && ((crossSellMatchRequest.InitialLeadSuccess && programProduct.Match.ProductId != (int)ProductType.Exclusive) || !crossSellMatchRequest.InitialLeadSuccess)
                    && (programProduct.Match.SFProductCode_PSI != SFProductCode.SAB_CPC || programProduct.Match.PaidStatusTypeId != PaidStatusType.Paid)
                    && ((crossSellMatchRequest.InitialLeadSuccess && programProduct.Match.AllowCrossSell) || !crossSellMatchRequest.InitialLeadSuccess))
                {
                    List<int> allCampaignAllowedProducts = c.GetAllAllowedProducts(crossSellMatchRequest.Application).ToList();
                    List<MatchItem> matchItemList = GetFilteredProgramProduct(crossSellMatchRequest, productList: allCampaignAllowedProducts);

                    matchItemList = c.ApplyCampaignFilters(matchItemList, crossSellMatchRequest.Application, _performanceLog);
                    matchResults.RulesResult = ProcessRules(matchItemList, crossSellMatchRequest.ProspectInput, c, crossSellMatchRequest.Application, crossSellMatchRequest.LeadCreationType);

                    CrossSellResults internalResults = xSellProcessor.ExecuteCrossSellRules(matchItemList, crossSellMatchRequest.FormInstitutionId, programProduct, crossSellMatchRequest.ProspectInput.EducationLevelId, crossSellMatchRequest.InitialLeadSuccess, crossSellMatchRequest.ProspectInput.KVCodeData);

                    //matchItemList = internalResults.CategoryMatchItemList;
                    matchResults.IsSAB = internalResults.IsSAB;
                    matchResults.CrossSellMappingList = internalResults.CrossSellMappingList;
                    matchResults.TemplateProgramCampusType = internalResults.TemplateProgramCampusType;

                    //TFS-101295 - Apply SRA scores to new lists
                    //matchResults.ChosenBusinessModel = ApplySRAScores(crossSellMatchRequest, c, matchItemList);
                    //matchResults.MatchItemList = matchItemList;
                    //Apply SRA to new lists, but only get business model for most specific lists with results
                    if (internalResults.SpecialtyMatchItemList.Any())
                    {
                        matchResults.ChosenBusinessModel = ApplySRAScores(crossSellMatchRequest, c, internalResults.SpecialtyMatchItemList);
                        matchResults.CrossSellSpecialtyMatchItemList = internalResults.SpecialtyMatchItemList;

                        ApplySRAScores(crossSellMatchRequest, c, internalResults.SubjectMatchItemList);
                        matchResults.CrossSellSubjectMatchItemList = internalResults.SubjectMatchItemList;

                        ApplySRAScores(crossSellMatchRequest, c, internalResults.CategoryMatchItemList);
                        matchResults.CrossSellCategoryMatchItemList = internalResults.CategoryMatchItemList;
                    }
                    else
                    {
                        if (internalResults.SubjectMatchItemList.Any())
                        {
                            matchResults.ChosenBusinessModel = ApplySRAScores(crossSellMatchRequest, c, internalResults.SubjectMatchItemList);
                            matchResults.CrossSellSubjectMatchItemList = internalResults.SubjectMatchItemList;

                            ApplySRAScores(crossSellMatchRequest, c, internalResults.CategoryMatchItemList);
                            matchResults.CrossSellCategoryMatchItemList = internalResults.CategoryMatchItemList;
                        }
                        else
                        {
                            matchResults.ChosenBusinessModel = ApplySRAScores(crossSellMatchRequest, c, internalResults.CategoryMatchItemList);
                            matchResults.CrossSellCategoryMatchItemList = internalResults.CategoryMatchItemList;
                        }
                    }

                    matchResults.MatchItemList = new List<MatchItem>();
                    matchResults.MatchItemList.AddRange(matchResults.CrossSellSpecialtyMatchItemList);
                    matchResults.MatchItemList.AddRange(matchResults.CrossSellSubjectMatchItemList);
                    matchResults.MatchItemList.AddRange(matchResults.CrossSellCategoryMatchItemList);
                }
                else
                    matchResults.MatchItemList = new List<MatchItem>();
            }
            else
            {
                matchResults.MatchItemList = new List<MatchItem>();
            }

            EndLogDetail();
            return matchResults;
        }

        public MatchResult GetFormProgramMatches(DirectoryMatchRequest directoryMatchRequest)
        {
            int? programId = directoryMatchRequest.ProgramIdList == null || directoryMatchRequest.ProgramIdList.Count == 0 ? (int?)null : directoryMatchRequest.ProgramIdList.First();
            directoryMatchRequest.ProgramIdList = null;
            var singleProgramMatches = new List<MatchItem>();

            directoryMatchRequest.RemoveInvalidEntities = false;

            MatchResult matchResults = GetDirectoryMatches(directoryMatchRequest, setApplicationFromCampaign: true);

            List<MatchItem> miListFiltered = matchResults.MatchItemList.Where(m => m.Match.PaidStatusTypeId == PaidStatusType.Paid &&
                                                                               m.Match.InquiryDisabled == false && m.FailedValidation == false).ToList();

            if (programId.HasValue && programId.Value > 0 && !miListFiltered.Any(m => m.Match.ProgramId == programId.Value))
            {
                //remove so no geo validation is done
                if (directoryMatchRequest.ProspectInput != null)
                {
                    directoryMatchRequest.ProspectInput.PostalCode = null;
                    directoryMatchRequest.ProspectInput.StateId = null;
                    directoryMatchRequest.ProspectInput.CountryId = null;
                }

                singleProgramMatches = GetProgramMatchesForForm(directoryMatchRequest, matchResults.ChosenCampaign, programId.Value);

                //var singleProgramMatches = matchResults.MatchItemList.Where(mi => mi.Match.ProgramId == programId.Value);

                miListFiltered.AddRange(singleProgramMatches);
            }

            if (miListFiltered.Any(m => m.Match.PaidStatusTypeId == PaidStatusType.Paid &&
                                     m.Match.InquiryDisabled == false && m.FailedValidation == false))
            {
                matchResults.MatchItemList = miListFiltered;
            }

            //add the specifc program match back again in-case it was filterd out during the matching process
            if (matchResults.MatchItemList.Count(m => m.Match.ProgramId == programId) < 1)
            {
                matchResults.MatchItemList.AddRange(singleProgramMatches);
            }

            return matchResults;
        }

        public MatchResult GetWarmTransferTitaniumMatches(BaseMatchRequest baseMatchRequest)
        {
            MatchResult matchResult = new MatchResult();
            matchResult.ChosenCampaign = Campaign.Get(baseMatchRequest.TrackGuid);

            List<MatchItem> matchItemList = GetFilteredProgramProduct(baseMatchRequest, productList: new List<int> { (int)Constants.ProductType.WarmTransferTitanium });
            //Remove all matches but Warm Transfer Titanium
            //matchItemList.RemoveAll(o => o.Match.ProductId != (int)Constants.ProductType.WarmTransferTitanium);

            //matchItemList = matchResult.ChosenCampaign.ApplyCampaignFilters(matchItemList, baseMatchRequest.Application, false, _performanceLog);
            RulesEngine rulesEngine = new RulesEngine(baseMatchRequest.ProspectInput, matchResult.ChosenCampaign
                                                      , baseMatchRequest.Application, false, _performanceLog, false
                                                      , LeadCreationType.WizardSmartMatch);

            //rulesEngine.excludedRules.Add(typeof(Rules.Campaign));
            RulesResult rulesResult = rulesEngine.ExecuteRules(matchItemList);

            matchResult.MatchItemList = rulesResult.MatchedProgramProductList.Where(mi => mi.FailedValidation == false).ToList();
            matchResult.RulesResult = rulesResult;

            matchResult.ChosenBusinessModel = ApplySRAScores(baseMatchRequest, matchResult.ChosenCampaign, matchResult.MatchItemList);

            return matchResult;
        }

        private MatchResult GetBaseMatches(BaseMatchRequest baseMatchRequest, string searchTerm = null, bool setApplicationFromCampaign = false, LeadCreationType? LeadCreationTypeID = null, List<int> desiredProgramLevelList = null, bool? ignoreTrackingTag = false)
        {
            StartLogDetail("GetBaseMatches");

            MatchResult matchResult = new MatchResult();

            matchResult.ChosenCampaign = Campaign.Get(baseMatchRequest.TrackGuid);

            BaseRuleType? ruleType = new BaseRuleType();

            if (matchResult.ChosenCampaign != null && matchResult.ChosenCampaign.IsValid(out ruleType) && (!ignoreTrackingTag.HasValue || (ignoreTrackingTag.HasValue && ignoreTrackingTag.Value == false)))
            {
                if (setApplicationFromCampaign)
                    baseMatchRequest.ApplicationId = matchResult.ChosenCampaign.ApplicationId.Value;

                List<int> allCampaignAllowedProducts = matchResult.ChosenCampaign.GetAllAllowedProducts(baseMatchRequest.Application).ToList();
                List<MatchItem> matchItemList = GetFilteredProgramProduct(baseMatchRequest, searchTerm, allCampaignAllowedProducts);

                //TODO: remove for SAB
                if (baseMatchRequest.ApplicationId != 20)
                    matchItemList = matchItemList.Where(mi => (ProgramType)mi.Match.ProgramTypeId == ProgramType.FullDegree).ToList();

                StartLogDetail("ApplyCampaignFilters");

                matchItemList = matchResult.ChosenCampaign.ApplyCampaignFilters(matchItemList, baseMatchRequest.Application, _performanceLog);
                EndLogDetail();

                int? userId = null;
                if (baseMatchRequest is WizardMatchRequest)
                    userId = ((WizardMatchRequest)baseMatchRequest).UserId;
                else if (baseMatchRequest is NeoDirectoryMatchRequest)
                    userId = ((NeoDirectoryMatchRequest)baseMatchRequest).UserId;

                matchResult.RulesResult = ProcessRules(matchItemList, baseMatchRequest.ProspectInput, matchResult.ChosenCampaign, baseMatchRequest.Application, LeadCreationTypeID, desiredProgramLevelList, userId);

                matchResult.MatchItemList = matchItemList;
            }
            else if (ignoreTrackingTag.HasValue && ignoreTrackingTag.Value == true)
            {
                baseMatchRequest.ApplicationId = GetApplicationIdFromSettingsWhenIgnoreTrackingTag();
                
                List<int> allCampaignAllowedProducts = GetAllCampaignAllowedProductsWhenIgnoreTrackingTag();
                List<MatchItem> matchItemList = GetFilteredProgramProduct(baseMatchRequest, searchTerm, allCampaignAllowedProducts);
                
                //TODO: remove for SAB
                matchItemList = matchItemList.Where(mi => (ProgramType)mi.Match.ProgramTypeId == ProgramType.FullDegree).ToList();
                matchResult.MatchItemList = matchItemList;
            }
            else
            {
                matchResult.MatchItemList = new List<MatchItem>();

            }

            EndLogDetail();

            return matchResult;
        }

        private List<int> GetAllCampaignAllowedProductsWhenIgnoreTrackingTag()
        {
            var campaignAllowedProductsWhenIgnoreTrackingTagFromSettings = new List<int>();

            var campaignIdsFromSettings =
                ConfigurationManager.AppSettings["ListOfCommaSeparatedCampaignIdToUseWhenIgnoreTrackingId"];
            
            if (!string.IsNullOrWhiteSpace(campaignIdsFromSettings))
            {
                var listOfCampaignIdsFromSettings = campaignIdsFromSettings.Split(',');
                foreach (var campaignIdFromSetting in listOfCampaignIdsFromSettings)
                {
                    if (int.TryParse(campaignIdFromSetting, out int parsedCampaignIdFromSettings))
                    {
                        campaignAllowedProductsWhenIgnoreTrackingTagFromSettings.Add(parsedCampaignIdFromSettings);
                    }
                }

                if (campaignAllowedProductsWhenIgnoreTrackingTagFromSettings.Any())
                {
                    return campaignAllowedProductsWhenIgnoreTrackingTagFromSettings;
                }
            }

            return new List<int> {1, 2, 8, 9};
        }

        private int GetApplicationIdFromSettingsWhenIgnoreTrackingTag()
        {
            int defaultApplicationIdFromSettingsWhenIgnoreTrackingTag = 9;//education connection
            var applicationIdFromSettings = ConfigurationManager.AppSettings["ApplicationIdToUseWhenIgnoreTrackingId"]; 

            if (int.TryParse(applicationIdFromSettings, out int parsedApplicationId))
            {
                defaultApplicationIdFromSettingsWhenIgnoreTrackingTag = parsedApplicationId;
            }

            return defaultApplicationIdFromSettingsWhenIgnoreTrackingTag;
        }

        public MatchResult GetWizardMatches(WizardMatchRequest wizardMatchRequest)
        {
            StartLogDetail("GetWizardMatches");
            MatchResult matchResult = new MatchResult();

            int? leadScoringTierLevel = wizardMatchRequest.LeadScoringInput == null ? null : wizardMatchRequest.LeadScoringInput.LeadScoringTierLevel;

            matchResult = GetBaseMatches(wizardMatchRequest, LeadCreationTypeID: wizardMatchRequest.LeadCreationType, desiredProgramLevelList: wizardMatchRequest.DesiredProgramLevelList);

            //Erick: Added InquiryDisabled and Paid filters here instead of aggregator
            matchResult.MatchItemList = matchResult.MatchItemList.Where(mi => mi.Match.InquiryDisabled == false && mi.Match.PaidStatusTypeId == PaidStatusType.Paid).ToList();

            if (wizardMatchRequest.LeadCreationType == LeadCreationType.WizardSmartMatch)
            {
                matchResult.MatchItemList = CampaignRestrictedTemplateProcessor.ApplyFilter(wizardMatchRequest.TrackGuid, matchResult.MatchItemList);



                //Erick: For Smart Match filter out program level if specified.
                if (wizardMatchRequest.DesiredProgramLevelList != null && wizardMatchRequest.DesiredProgramLevelList.Any())
                {
                    matchResult.MatchItemList = matchResult.MatchItemList.Where(mi => wizardMatchRequest.DesiredProgramLevelList.Contains(mi.Match.ProgramLevelId)).ToList();
                }

                matchResult.MatchItemList = matchResult.MatchItemList.Where(mi => mi.Match.CanSmartMatch == true || mi.Match.ProductId == (int)ProductType.ThirdPartyAPI).ToList();

                //Ensure BrandElect product can't smart match
                matchResult.MatchItemList = matchResult.MatchItemList.Where(mi => mi.Match.ProductId != (int)ProductType.BrandElect).ToList();
            }

            //Don't let BrandElect product in user selection if there were smart matches
            if (wizardMatchRequest.LeadCreationType == LeadCreationType.WizardUserSelection && wizardMatchRequest.SmartMatchedInstituionIdList != null && wizardMatchRequest.SmartMatchedInstituionIdList.Any())
            {
                matchResult.MatchItemList = matchResult.MatchItemList.Where(mi => mi.Match.ProductId != (int)ProductType.BrandElect).ToList();
            }

            if (wizardMatchRequest.GeoTarget != null)
            {
                GeoCodeProcessor geo = new GeoCodeProcessor(this._performanceLog);
                matchResult.MatchItemList = geo.ExecuteGeoFilter(matchResult.MatchItemList, wizardMatchRequest.GeoTarget, wizardMatchRequest.ApplicationId, matchResult.ChosenCampaign, null);
            }

            matchResult.ChosenBusinessModel = ApplySRAScores(wizardMatchRequest, matchResult.ChosenCampaign, matchResult.MatchItemList, wizardMatchRequest.BoostSpecialtyList);

            EndLogDetail();
            return matchResult;
        }

        public MatchResult GetDirectoryMatches(DirectoryMatchRequest directoryMatchRequest, string searchTerm = null, bool setApplicationFromCampaign = false)
        {
            StartLogDetail("GetDirectoryMatches");

            MatchResult matchResult = this.GetBaseMatches(directoryMatchRequest, searchTerm, setApplicationFromCampaign, directoryMatchRequest.LeadCreationType, null, directoryMatchRequest.IgnoreTrackingTag);

            if (matchResult.MatchItemList != null && matchResult.MatchItemList.Count > 0)
            {
                if (directoryMatchRequest.SortMethod == EntitySortMethod.RankScore)
                    matchResult.ChosenBusinessModel = ApplySRAScores(directoryMatchRequest, matchResult.ChosenCampaign, matchResult.MatchItemList);

                if (directoryMatchRequest.RemoveInvalidEntities.HasValue && directoryMatchRequest.RemoveInvalidEntities.Value)
                    matchResult.MatchItemList = matchResult.MatchItemList.Where(mi => mi.FailedValidation == false).ToList();
                else
                    matchResult.MatchItemList = matchResult.MatchItemList.Where(mi => mi.FailedValidation == false || (mi.FailedValidation == true && mi.RemovalReason.RuleType != BaseRuleType.CampaignRestriction)).ToList();

                if (directoryMatchRequest.GeoTarget != null)
                {
                    GeoCodeProcessor geo = new GeoCodeProcessor(this._performanceLog);
                    matchResult.MatchItemList = geo.ExecuteGeoFilter(matchResult.MatchItemList, directoryMatchRequest.GeoTarget, directoryMatchRequest.ApplicationId, matchResult.ChosenCampaign, directoryMatchRequest.MaxResultsCount);
                }

                if (directoryMatchRequest.IncludeOnlyClickPrograms == true || (directoryMatchRequest.Application.HasValue && directoryMatchRequest.Application.Value == Base.ISApplication.VendorAPI))
                    matchResult.MatchItemList = matchResult.MatchItemList.Where(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Paid).ToList();

                matchResult.MatchItemList = ProcessClickRemovals(matchResult.MatchItemList);
            }
            EndLogDetail();
            return matchResult;
        }

        private List<MatchItem> ProcessClickRemovals(List<MatchItem> matchItems)
        {
            List<MatchItem> matchesToRemove = RulesEngine.ProcessClickProgramRules(matchItems.Where(mi => mi.Match.HasClick && mi.Match.InquiryDisabled));

            if (matchesToRemove.Any())
                matchItems = matchItems.Except(matchesToRemove).ToList();

            int clicksOnlyBecauseCapped = matchItems.Where(mi => mi.RemovalReason != null && mi.RemovalReason.RuleType == BaseRuleType.LeadCap && mi.RemovalReason.RuleDetail == "LeadType Lead Capped").Update(mi =>
            {
                mi.LeadTypeLeadCapped = true;
                mi.FailedValidation = false;
            });

            int markClickCapped = matchItems.Where(mi => mi.RemovalReason != null && mi.RemovalReason.RuleType == BaseRuleType.LeadCap && mi.RemovalReason.RuleDetail == "LeadType Click Capped").Update(mi =>
            {
                mi.LeadTypeClickCapped = true;
            });

            int removedClickOnlyCapped = matchItems.RemoveAll(mi => mi.Match.HasClick && mi.Match.InquiryDisabled && mi.FailedValidation == true);

            int removedFreeClickOnly = matchItems.RemoveAll(mi => mi.Match.ClickPrice > 0 && !String.IsNullOrWhiteSpace(mi.Match.ClickThroughUrl) && mi.Match.InquiryDisabled && mi.Match.PaidStatusTypeId == PaidStatusType.Free);

            return matchItems;
        }

        private List<MatchItem> GetProgramMatchesForForm(DirectoryMatchRequest directoryMatchRequest, Campaign chosenCampaign, int programId)
        {
            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

            List<MatchItem> programMatches = md.GetMatchItemsByProgramId(programId);
            programMatches = chosenCampaign.ApplyCampaignFilters(programMatches, directoryMatchRequest.Application, ProductFilterType.Full, _performanceLog);

            if (programMatches.Any(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Paid))
            {
                RulesResult rulesResult = ProcessRules(programMatches, directoryMatchRequest.ProspectInput, chosenCampaign, null);

                programMatches = rulesResult.MatchedProgramProductList;
            }

            ApplySRAScores(directoryMatchRequest, chosenCampaign, programMatches);

            return programMatches;
        }

        public MatchResult GetDirectoryMatchesAllIfNone(DirectoryMatchRequest directoryMatchRequest)
        {
            StartLogDetail("GetDirectoryMatchesAllIfNone");
            MatchResult matchResult = new MatchResult();

            matchResult = GetDirectoryMatches(directoryMatchRequest);

            if (matchResult == null || matchResult.ChosenCampaign == null || matchResult.ChosenCampaign.ApplicationId.HasValue == false)
            {
                matchResult.MatchItemList = new List<MatchItem>();
            }
            else if (matchResult.MatchItemList == null || matchResult.MatchItemList.Count == 0)
            {
                StartLogDetail("GetAll.GetFilteredProgramProduct");
                //matchResult.ChosenCampaign = Campaign.Get(directoryMatchRequest.TrackGuid);

                directoryMatchRequest.ApplicationId = matchResult.ChosenCampaign.ApplicationId.Value;

                matchResult.MatchItemList = GetFilteredProgramProduct(directoryMatchRequest, productList: matchResult.ChosenCampaign.GetAllAllowedProducts(directoryMatchRequest.Application).ToList());
                EndLogDetail();

                if (matchResult.MatchItemList == null || !matchResult.MatchItemList.Any())
                {
                    if (matchResult.ChosenCampaign.Products != null)
                    {
                        if (matchResult.ChosenCampaign.Products.Count == 1 && matchResult.ChosenCampaign.Products[0] == 93) //only do this if there is one product allowed in the campaign and its Third Party API
                        {
                            matchResult.MatchItemList = GetFilteredProgramProduct(directoryMatchRequest, productList: new List<int>() { 1 }); //just use Premier, it always has a lot of programs
                        }
                    }
                }
            }

            EndLogDetail();
            return matchResult;
        }

        private BusinessModel ApplySRAScores(BaseMatchRequest request, Campaign campaign, List<MatchItem> matchItemList, List<int> specialties = null)
        {
            StartLogDetail("ApplySRAScores");

            SchoolRankingEngine srEngine = new SchoolRankingEngine(_performanceLog);

            DTO.ProspectInput prospectInput = request.ProspectInput;
            int? singleSubjectId = request.SubjectList != null && request.SubjectList.Count == 1 ? (int?)request.SubjectList.First() : null;
            int? singleCategoryId = request.CategoryList != null && request.CategoryList.Count == 1 ? (int?)request.CategoryList.First() : null;
            int? singleProgramLevelId = request.ProgramLevelList != null && request.ProgramLevelList.Count == 1 ? (int?)request.ProgramLevelList.First() : null;
            string postalCode = null;
            CampusPreference? campusPref = null;
            int? stateId = prospectInput != null ? prospectInput.StateId : null;

            //check if geotarget postalcode is present
            if (request is DirectoryMatchRequest)
            {
                GeoTarget geo = ((DirectoryMatchRequest)request).GeoTarget;

                postalCode = geo != null ? geo.PostalCode : null;
            }
            if (request is WizardMatchRequest)
            {
                campusPref = ((WizardMatchRequest)request).CampusPreference;
            }


            //prefer prospect input (form data) postal code, if not there, then use geo postal code, if no geo then null
            postalCode = prospectInput != null && !String.IsNullOrWhiteSpace(prospectInput.PostalCode) ? prospectInput.PostalCode : postalCode;

            BusinessModel rankingModel = srEngine.PickRankingModel(campaign, prospectInput, request.ApplicationId, singleSubjectId, singleCategoryId, singleProgramLevelId, request.TrackingDeviceGuid);

            List<FactorAggregate> factorAggList;

            List<MatchItem> scoredMatchItems = srEngine.GenerateProgramRankScores(rankingModel, matchItemList, campusPref, request.CampusType, postalCode, request.ApplicationId, stateId, out factorAggList, specialties);

            rankingModel.FactorAggregateList = factorAggList;

            EndLogDetail();

            return rankingModel;
        }

        private List<MatchItem> GetFilteredProgramProduct(BaseMatchRequest request, string searchTerm = null, List<int> productList = null)
        {
            StartLogDetail("GetFilteredProgramProduct");

            List<MatchItem> matchItemList = null;

            matchItemList = FilterProgramProducts(request, searchTerm, productList);

            if (request is DirectoryMatchRequest)
            {
                int? featureId = ((DirectoryMatchRequest)request).FeatureId;

                if (featureId.HasValue)
                {
                    StartLogDetail("FeatureProcessor.FilterByFeatureId");
                    matchItemList = FeatureProcessor.FilterByFeatureId(matchItemList, featureId.Value);
                    EndLogDetail();
                }
            }

            if (matchItemList != null && matchItemList.Count > 0 && String.IsNullOrEmpty(matchItemList[0].Match.ProgramName))
            {
                StartLogDetail("MatchingContentData.JoinContentData");
                MatchingContentData.JoinContentData((from m in matchItemList select m.Match).ToList());
                EndLogDetail();
            }

            EndLogDetail();
            return matchItemList;
        }

        private RulesResult ProcessRules(List<MatchItem> matchItemList, DTO.ProspectInput prospect, Campaign campaign, IS.Base.ISApplication? app, LeadCreationType? LeadCreationTypeID = null, List<int> desiredProgramLevelList = null, int? userId = null, bool skipWTRules = false)
        {
            RulesEngine rulesEngine = new RulesEngine(prospect, campaign, app, false, _performanceLog, false, LeadCreationTypeID, desiredProgramLevelList, userId, skipWTRules);

            RulesResult rulesResult = rulesEngine.ExecuteRules(matchItemList);

            //matchItemList = rulesResult.MatchedProgramProductList;

            return rulesResult;
        }

        private List<MatchItem> FilterProgramProducts(BaseMatchRequest matchRequest, string searchTerm, List<int> productList)
        {
            List<MatchItem> matchItemList = null;

            List<int> institutionIdList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).InstitutionIdList : null;
            int? campusId = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).CampusId : null;
            int? campusOptionGroupId = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).CampusOptionGroupId : null;
            List<int> advertiserList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).AdvertiserList : null;
            List<int> crList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).ClientRelationshipList : null;
            List<int> programIdList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).ProgramIdList : null;
            List<int> templateList = matchRequest.GetType() == typeof(WizardMatchRequest) ? ((WizardMatchRequest)matchRequest).TemplateList : null;
            bool? onlyClick = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).IncludeOnlyClickPrograms : null;
            int? programGroupId = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).ProgramGroupId : null;
            List<int> paidStatusList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).ProgramPaidStatusList : null;

            //SAB filters
            List<int> workTypeList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).WorkTypeList : null;
            List<int> durationList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).DurationList : null;
            List<int> termList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).TermList : null;
            List<int> placementAudienceList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).PlacementAudienceList : null;
            List<int> teachAbroadTypeList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).TeachAbroadTypeList : null;
            List<int> languageList = matchRequest.GetType() == typeof(DirectoryMatchRequest) ? ((DirectoryMatchRequest)matchRequest).LanguageList : null;
            List<int> sfProductCodes = null;

            if (matchRequest.SFProductCodes != null && matchRequest.SFProductCodes.Any())
            {
                sfProductCodes = new List<int>();
                foreach (SFProductCode p in matchRequest.SFProductCodes)
                    sfProductCodes.Add((int)p);
            }

            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

            if (md != null)
            {
                StartLogDetail("FilterProgramProducts - In Memory");
                matchItemList = md.FilterProgramProducts(institutionIdList, campusId, programIdList, matchRequest.ProgramLevelList,
                                                         matchRequest.ProgramTypeList != null ? matchRequest.ProgramTypeList.Cast<int>().ToList() : null,
                                                         matchRequest.CampusType, matchRequest.CategoryList, matchRequest.SubjectList, matchRequest.SpecialtyList,
                                                         templateList, searchTerm, matchRequest.CampusList, matchRequest.IsHybrid, onlyClick, programGroupId, matchRequest.PaidOnly,
                                                         workTypeList, termList, durationList, placementAudienceList, teachAbroadTypeList, languageList, productList, sfProductCodes,
                                                         paidStatusList, matchRequest.IsNonProfit, campusOptionGroupId, advertiserList, crList, _performanceLog);


                EndLogDetail();
            }

            return matchItemList;
        }

        public CampaignDetailResponse GetCampaignDetailByTrackID(Guid TrackID)
        {
            StartLogDetail("GetCampaignDetailByTrackID");
            CampaignDetailResponse campaignDetailResponse = new DTO.CampaignDetailResponse();

            campaignDetailResponse = SetCampaignDetail(Campaign.Get(TrackID));

            EndLogDetail();
            return campaignDetailResponse;
        }

        private CampaignDetailResponse SetCampaignDetail(Campaign campaign)
        {
            if (campaign == null) return null;

            CampaignDetailResponse campaignDetailResponse = new CampaignDetailResponse();
            campaignDetailResponse.TrackID = campaign.TrackId;
            campaignDetailResponse.MaxSmartMatchCount = campaign.MaxSmartMatchCount;
            campaignDetailResponse.MaxSubmissionCount = campaign.MaxSubmissionCount;
            campaignDetailResponse.AllowExitPops = campaign.AllowsExitPop;
            campaignDetailResponse.AdditionalQuestionsFlowType = campaign.AdditionQuestionsFlowType;
            campaignDetailResponse.ProgramWizardAdditionalQuestionsFlowType = campaign.ProgramWizardAdditionQuestionsFlowType;
            campaignDetailResponse.IsCallCenter = campaign.CampaignTypeId == 4 ? true : false;
            campaignDetailResponse.CampaignTCPAMessageName = campaign.CampaignTCPAMessageName;
            campaignDetailResponse.AllowRemonetization = campaign.AllowRemonetization;
            campaignDetailResponse.UseInternationalTemplate = campaign.UseInternationalTemplate;
            campaignDetailResponse.ChannelId = campaign.ChannelId;
            campaignDetailResponse.AllowLeaveBehinds = campaign.AllowsLeaveBehind;
            campaignDetailResponse.HasXVerify = campaign.HasXVerify;
            campaignDetailResponse.OpenMailProfileId = campaign.OpenMailProfileId;
            return campaignDetailResponse;
        }

        //Added for Partner API
        public ValidateProgramMatchResult GetValidateProgramMatchResult(int programProductId, int? programId, int? campusId, DTO.ProspectInput pi, Guid? TrackGuid, IS.Base.ISApplication app, bool ignoreCaps, LeadCreationType? leadCreationType, int? agentId)
        {
            StartLogDetail("GetValidateProgramMatchResult");
            ValidateProgramMatchResult mr = new ValidateProgramMatchResult();

            Campaign c = null;

            if (TrackGuid.HasValue && TrackGuid != Guid.Empty)
                c = Campaign.Get(TrackGuid.Value);

            BaseRuleType? ruleType = null;
            if (c == null || c.IsValid(out ruleType))
            {
                List<MatchItem> matches = GetMatchItemsForValidateProgram(programProductId, programId, campusId);

                //If no agent id passed them filter out WT matches.
                matches = Product.FilterWTMatchItems(agentId.HasValue, matches);

                //if (!agentId.HasValue)
                //    matches = matches.Where(m => !Product.IsWarmTransferProduct(m.Match.ProductId)).ToList();
                //else
                //    matches = matches.Where(m => Product.IsWarmTransferProduct(m.Match.ProductId)).ToList();

                matches = c.ApplyCampaignFilters(matches, app, ProductFilterType.Full, _performanceLog);

                if (matches.Count == 0)
                {
                    mr.ResultType = ValidateProgramMatchResultType.ProgramNotAvailable;
                }
                else if (matches.All(m => m.RemovalReason != null))
                {
                    mr.ResultType = ValidateProgramMatchResultType.CampaignRestriction;
                }
                else
                {
                    if (matches.Any(m => m.Match.PaidStatusTypeId == PaidStatusType.Paid))
                    {
                        if (matches.Any(m => m.Match.InquiryDisabled == false))
                        {
                            mr.RulesResult = ProcessRules(matches, pi, c, app, leadCreationType, skipWTRules: true);

                            if (matches.Any(m => m.FailedValidation == false))
                            {
                                mr.ResultType = ValidateProgramMatchResultType.MatchExists;
                                ApplySRAScores(new BaseMatchRequest() { ProspectInput = pi, TrackGuid = TrackGuid.Value }, c, matches);
                                matches.Sort(MatchItemComparerFactory.Create(MatchItemSortType.RankScore));
                                mr.MatchItemList = matches;
                            }
                            else
                            {
                                mr.ResultType = ValidateProgramMatchResultType.RuleFailure;
                            }
                        }
                        else
                        {
                            mr.ResultType = ValidateProgramMatchResultType.InquiryDisabled;
                        }
                    }
                    else
                    {
                        mr.ResultType = ValidateProgramMatchResultType.NoPaidPrograms;
                    }
                }
            }
            else
            {
                mr.ResultType = GetResultType(ruleType);
            }
            return mr;
        }

        private List<MatchItem> GetMatchItemsForValidateProgram(int programProductId, int? submittedProgramId, int? campusId)
        {
            List<MatchItem> matches = null;
            MatchItem matchItemForProgramProduct = null;
            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

            if (programProductId > 0)
            {
                matchItemForProgramProduct = md.GetMatchItemByProgramProductId(programProductId);
                matches = new List<MatchItem> { matchItemForProgramProduct };
            }
            else if (submittedProgramId.HasValue)
            {
                if (submittedProgramId.Value != 0)
                    matches = md.GetMatchItemsByProgramId(submittedProgramId.Value);

                if (campusId.HasValue && campusId.Value != 0 && matches.Any(m => m.Match.CampusId == campusId.Value))
                    matches = matches.Where(m => m.Match.CampusId == campusId.Value).ToList();
            }

            return matches;
        }

        private ValidateProgramMatchResultType GetResultType(BaseRuleType? ruleType)
        {
            //create a no match item for missing campaign or capped
            if (ruleType.HasValue && ruleType == BaseRuleType.CampaignCapReached)
                return ValidateProgramMatchResultType.CampaignCappedOut;
            else if (ruleType.HasValue && ruleType == BaseRuleType.CampaignInactive)
                return ValidateProgramMatchResultType.CampaignInactive;
            else
                return ValidateProgramMatchResultType.CampaignLeadScore;
        }

        public decimal GetLeadPingLeadScoreCPL(int productId, int institutionId, int campusTypeId, int leadPingLeadScoreId)
        {
            decimal cpl = 0;
            LeadPingLeadScoreCPL leadPingLeadScoreCPL = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, LeadPingLeadScoreCPL>>(MatchingCacheItem.LeadPingLeadScoreCPLs).Select(l => l.Value).Where(m => m.ProductId == productId && m.InstitutionId == institutionId && m.CampusTypeId == campusTypeId && m.LeadPingLeadScoreId == leadPingLeadScoreId).FirstOrDefault();
            if (leadPingLeadScoreCPL != null)
            {
                cpl = leadPingLeadScoreCPL.CPL;
            }
            return cpl;
        }
    }
}
