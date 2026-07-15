using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EDDY.IS.Base.Util;
using EDDY.IS.MatchingEngine.DTO;
using System.Configuration;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.MatchingEngine.Constants;
using EDDY.IS.ExternalMatch.Base;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.MatchingEngine
{
    public static class MatchAggregator
    {
        private static Dictionary<int, List<string>> RollupProgramDisplayGroups(List<MatchItem> matchResults)
        {
            Dictionary<int, List<string>> groupResults = new Dictionary<int, List<string>>();

            Dictionary<int, ProgramDisplayGroup> cacheDisplayGroupList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, ProgramDisplayGroup>>(MatchingCacheItem.ProgramDisplayGroups);
            HashSet<int> programIdsToRemove = new HashSet<int>();

            var groupedByProgramGroup = matchResults.GroupBy(mi => mi.Match.ProgramDisplayGroupId);

            if (groupedByProgramGroup.Count() > 0)
            {
                foreach (var progGroup in groupedByProgramGroup)
                {
                    if (progGroup.First().Match.ProgramDisplayGroupId.HasValue && cacheDisplayGroupList.ContainsKey(progGroup.First().Match.ProgramDisplayGroupId.Value)) //cacheDisplayGroupList.Any(cd => cd.ProgramDisplayGroupId == progGroup.First().ProgramDisplayGroupId))
                    {
                        //ProgramDisplayGroup cachedGroup = cacheDisplayGroupList.Where(cd => cd.ProgramDisplayGroupId == progGroup.First().ProgramDisplayGroupId).First();
                        ProgramDisplayGroup cachedGroup = cacheDisplayGroupList[progGroup.First().Match.ProgramDisplayGroupId.Value];

                        if (progGroup.DistinctBy(m => m.Match.ProgramLevelId).Count() > 1)
                        {
                            foreach (var singleProgram in progGroup)
                            {
                                singleProgram.Match.ProgramDisplayGroupName = cachedGroup.GroupName;

                                if (!String.IsNullOrWhiteSpace(cachedGroup.GroupDescription))
                                    singleProgram.Match.ProgramDisplayGroupDescription = cachedGroup.GroupDescription;

                                if (!String.IsNullOrWhiteSpace(cachedGroup.GroupShortDescription))
                                    singleProgram.Match.ProgramDisplayGroupShortDescription = cachedGroup.GroupShortDescription;
                            }

                            groupResults.Add(progGroup.First().Match.ProgramId, progGroup.Select(mi => mi.Match.ProgramLevelName).Distinct().ToList());

                            //var programIdsToRemove = progGroup.Where(mi => mi.ProgramId != progGroup.First().ProgramId).Select(mi => mi.ProgramId);
                            foreach (MatchItem program in progGroup.ToList())
                            {
                                if (program.Match.ProgramId != progGroup.First().Match.ProgramId)
                                    programIdsToRemove.Add(program.Match.ProgramId);
                            }
                        }
                        //matchResults.RemoveAll(mi => programIdsToRemove.Contains(mi.ProgramId));
                    }
                }
            }

            matchResults.RemoveAll(mi => programIdsToRemove.Contains(mi.Match.ProgramId));
            return groupResults;
        }

        private static List<MatchItem> RemoveDuplicates<TDistinctSelector>(List<MatchItem> matchResults, MatchItemSortType sortType, Func<MatchItem, TDistinctSelector> distinctSelector)
        {
            if (sortType != MatchItemSortType.None)
                matchResults.Sort(MatchItemComparerFactory.Create(sortType));

            return matchResults.DistinctBy(distinctSelector).ToList();
        }

        private static decimal CalculateRolledupRankScore(IEnumerable<BaseMatchEntity> entityList, BusinessModel businessModel, out bool failedValidation)
        {
            decimal rankScore;

            if (!entityList.Any(p => p.FailedValidation == false))
            {
                failedValidation = true;
                rankScore = 0;
            }
            else
            {
                failedValidation = false;

                if (businessModel != null)
                {
                    BusinessModelRollupType rollupType = businessModel.RollupType;

                    switch (rollupType)
                    {
                        case BusinessModelRollupType.TopScore:
                            rankScore = entityList.Max(p => p.ProgramRankScore);
                            break;
                        case BusinessModelRollupType.AverageScore:
                            rankScore = entityList.Average(p => p.ProgramRankScore);
                            break;
                        case BusinessModelRollupType.MedianScore:
                            rankScore = entityList.Select(p => p.ProgramRankScore).Median();
                            break;
                        default:
                            rankScore = 0;
                            break;
                    }
                }
                else
                    rankScore = 0;

            }

            return rankScore;
        }

        private static List<T> PaginateResults<T>(List<T> entityList, int? maxResultsCount, int? pageNumber)
        {
            List<T> resultList;

            if (pageNumber.HasValue && pageNumber.Value > -1 && maxResultsCount.HasValue && maxResultsCount > 1)
            {
                resultList = entityList.Skip(pageNumber.Value * maxResultsCount.Value).Take(maxResultsCount.Value).ToList();
            }
            else if (maxResultsCount.HasValue && maxResultsCount > 0)
            {
                resultList = entityList.Take(maxResultsCount.Value).ToList();
            }
            else
                resultList = entityList;

            return resultList;
        }

        private static void ExecuteApplicationRules(ref List<MatchItem> matchItems, int applicationId)
        {
            Dictionary<int, VW_Matching_Application> applications = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_Application>>(MatchingCacheItem.Applications);

            VW_Matching_Application singleApp = null;

            applications.TryGetValue(applicationId, out singleApp);

            if (singleApp == null || !singleApp.AllowClicks)
                matchItems = matchItems.Where(f => f.Match.InquiryDisabled != true).ToList();

            if (singleApp == null || singleApp.NonPaidTreatedAsCapped)
            {
                var fraidFreeList = matchItems.Where(f => f.Match.PaidStatusTypeId == PaidStatusType.Fraid || f.Match.PaidStatusTypeId == PaidStatusType.Free).ToList();

                fraidFreeList.ForEach(pp =>
                {
                    pp.FailedValidation = true;
                });
            }
        }

        private static Dictionary<int, MatchItem> GetAllClickProgramsInSet(List<MatchItem> matchList)
        {
            //Dictionary<int, int> clickProgramProduct = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, int>>(MatchingCacheItem.ClickProgramProducts);

            //var returnClicks = from p in matchList
            //                   join cp in clickProgramProduct
            //                   on p.ProgramProductId equals cp.Key
            //                   select p;

            //return returnClicks.DistinctBy(f => f.ProgramId).ToDictionary(f => f.ProgramId, f => f); 

            var clickItems = filterClickProgramsByCap(matchList.Where(mi => mi.Match.HasClick).ToList());

            if (clickItems != null && clickItems.Any())
                return clickItems.DistinctBy(f => f.Match.ProgramId).ToDictionary(f => f.Match.ProgramId, f => f);
            else
                return new Dictionary<int, MatchItem>();
        }

        public static SiteMapResponse CreateSiteMapResponse(MatchResult matchResult)
        {
            SiteMapResponse mapResponse = new SiteMapResponse();
            mapResponse.MatchResponseGuid = Guid.NewGuid();
            mapResponse.ResultCount = matchResult.MatchItemList.Count;
            mapResponse.SiteMapCountryList = new List<SiteMapCountry>();
            mapResponse.SiteMapStateList = new List<SiteMapState>();

            var countryGrouping = matchResult.MatchItemList.GroupBy(mi => mi.Match.CampusCountryId);

            foreach (var country in countryGrouping)
            {
                if (country.Count() > 0 && country.First().Match.CampusCountryId.HasValue)
                {
                    SiteMapCountry smCountry = new SiteMapCountry();

                    smCountry.CountryId = country.First().Match.CampusCountryId.Value;
                    smCountry.PaidProgramCount = country.Where(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Paid).Count();
                    smCountry.FraidProgramCount = country.Where(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Fraid).Count();
                    smCountry.FreeProgramCount = country.Where(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Free).Count();

                    mapResponse.SiteMapCountryList.Add(smCountry);
                }
            }

            var stateGrouping = matchResult.MatchItemList.Where(mi => mi.Match.CampusCountry == "US" || mi.Match.CampusCountry == "CA").GroupBy(g => g.Match.CampusStateId);

            foreach (var state in stateGrouping)
            {
                if (state.Count() > 0 && state.First().Match.CampusStateId.HasValue)
                {
                    SiteMapState smState = new SiteMapState();

                    smState.StateId = state.First().Match.CampusStateId.Value;

                    var matchItemsToMarkGround = state.Where(mi => mi.Match.CampusCampusTypeId == (int)CampusType.Ground && mi.Match.CampusStateId.Value == smState.StateId);

                    GeoCodeProcessor.MarkInAreaMatchItems(matchItemsToMarkGround.ToList(), state.ToList());

                    smState.PaidInAreaProgramCount = state.Where(mi => mi.Match.PaidStatusTypeId == PaidStatusType.Paid && mi.IsGeotargeted).Count();

                    mapResponse.SiteMapStateList.Add(smState);
                }
            }

            return mapResponse;
        }

        public static NavigationResponse CreateFacetedNavigationResponse(MatchResult matchResult, DirectoryMatchRequest directoryMatchRequest)
        {
            List<SubjectContent> subjectContent = StaticCacheProxyHost.CacheProxy.Get<List<SubjectContent>>(MatchingCacheItem.SubjectContent);
            List<CategoryContent> categoryContent = StaticCacheProxyHost.CacheProxy.Get<List<CategoryContent>>(MatchingCacheItem.CategoryContent);
            List<SpecialtyContent> specialtyContent = StaticCacheProxyHost.CacheProxy.Get<List<SpecialtyContent>>(MatchingCacheItem.SpecialtyContent);

            NavigationResponse navResponse = new NavigationResponse();
            navResponse.MatchResponseGuid = Guid.NewGuid();
            HashSet<int> CategoryIds = new HashSet<int>();
            List<Category> Categories = new List<Category>();
            HashSet<int> SubjectIds = new HashSet<int>();
            List<Subject> Subjects = new List<Subject>();
            HashSet<int> ProgramLevelIds = new HashSet<int>();
            List<ProgramLevel> ProgramLevels = new List<ProgramLevel>();
            HashSet<int> SpecialtyIds = new HashSet<int>();
            List<Specialty> Specialties = new List<Specialty>();
            HashSet<int> CountryIds = new HashSet<int>();
            List<CampusCountry> Countries = new List<CampusCountry>();
            HashSet<int> StateIds = new HashSet<int>();
            List<CampusState> States = new List<CampusState>();
            List<CampusType> CampusTypes = new List<CampusType>();
            HashSet<int> CityIds = new HashSet<int>();
            List<City> Cities = new List<City>();
            bool isHybridResult = false;

            HashSet<int> TermList = new HashSet<int>();
            HashSet<int> DurationList = new HashSet<int>();
            HashSet<int> WorkTypeList = new HashSet<int>();
            HashSet<int> TeachAbroadTypeList = new HashSet<int>();
            HashSet<int> PlacementAudienceList = new HashSet<int>();
            HashSet<int> LanguageList = new HashSet<int>();

            List<MatchItem> matchResults = matchResult.MatchItemList;

            foreach (MatchItem mi in matchResults)
            {
                if (!ProgramLevelIds.Contains(mi.Match.ProgramLevelId))
                {
                    ProgramLevelIds.Add(mi.Match.ProgramLevelId);
                    ProgramLevels.Add(new ProgramLevel() { ProgramLevelId = mi.Match.ProgramLevelId, ProgramLevelName = mi.Match.ProgramLevelName });
                }

                foreach (int categoryId in mi.Match.MatchingCategories(directoryMatchRequest.CategoryList, directoryMatchRequest.SubjectList, directoryMatchRequest.SpecialtyList))
                {
                    if (!CategoryIds.Contains(categoryId))
                    {
                        CategoryIds.Add(categoryId);
                        Categories.Add(new Category() { CategoryId = categoryId, FailedValidation = mi.FailedValidation, ProgramRankScore = 0 });
                    }
                }

                foreach (int subjectId in mi.Match.MatchingSubjects(directoryMatchRequest.CategoryList, directoryMatchRequest.SubjectList, directoryMatchRequest.SpecialtyList))
                {
                    if (!SubjectIds.Contains(subjectId))
                    {
                        SubjectIds.Add(subjectId);
                        Subjects.Add(new Subject() { SubjectId = subjectId, FailedValidation = mi.FailedValidation, ProgramRankScore = 0 });
                    }
                }

                foreach (int specialtyId in mi.Match.MatchingSpecialties(directoryMatchRequest.CategoryList, directoryMatchRequest.SubjectList, directoryMatchRequest.SpecialtyList))
                {
                    if (!SpecialtyIds.Contains(specialtyId))
                    {
                        SpecialtyIds.Add(specialtyId);
                        Specialties.Add(new Specialty() { SpecialtyId = specialtyId, SubjectId = mi.Match.PrimarySubjectId.HasValue ? mi.Match.PrimarySubjectId.Value : 0 });
                    }
                }

                if (CampusTypes.Count < 2 && CampusTypes.Contains((CampusType)mi.Match.ProgramCampusTypeId) == false)
                    CampusTypes.Add((CampusType)mi.Match.ProgramCampusTypeId);

                if (mi.Match.CampusCountryId.HasValue && !CountryIds.Contains(mi.Match.CampusCountryId.Value))
                {
                    CountryIds.Add(mi.Match.CampusCountryId.Value);
                    Countries.Add(new CampusCountry() { CountryId = mi.Match.CampusCountryId.Value, CountryName = mi.Match.CampusCountryName });
                }

                if (directoryMatchRequest.ApplicationId == 20 && mi.FilteredProgramAddressList != null && mi.FilteredProgramAddressList.Any())
                {
                    mi.FilteredProgramAddressList.ForEach(pa =>
                    {
                        if (pa.CountryId.HasValue && !CountryIds.Contains(pa.CountryId.Value))
                        {
                            CountryIds.Add(pa.CountryId.Value);
                            Countries.Add(new CampusCountry() { CountryId = pa.CountryId.Value, CountryName = pa.CountryName });
                        }

                        if (pa.CountryId.HasValue && pa.StateId.HasValue && (pa.CountryCode == "US" || pa.CountryCode == "CA" || pa.CountryCode == "GB") &&
                            !StateIds.Contains(pa.StateId.Value))
                        {
                            StateIds.Add(pa.StateId.Value);
                            States.Add(new CampusState() { StateId = pa.StateId.Value, StateName = pa.StateProvinceCode, StateFullName = StaticInternalObject.States[pa.StateId.Value] });
                        }

                        if (directoryMatchRequest.GeoTarget != null &&
                            ((directoryMatchRequest.GeoTarget.CountryList != null &&
                            directoryMatchRequest.GeoTarget.CountryList.Count > 0) ||
                            (directoryMatchRequest.GeoTarget.StateList != null &&
                            directoryMatchRequest.GeoTarget.StateList.Count > 0)) &&
                            pa.CityId.HasValue && !CityIds.Contains(pa.CityId.Value))
                        {
                            CityIds.Add(pa.CityId.Value);
                            Cities.Add(new City() { CityId = pa.CityId.Value, CityName = pa.City, StateId = pa.StateId.Value });
                        }
                    });
                }

                if (mi.Match.CampusStateId.HasValue && !StateIds.Contains(mi.Match.CampusStateId.Value))
                {
                    if (mi.Match.CampusCountry == "US" || mi.Match.CampusCountry == "CA" || mi.Match.CampusCountry == "GB")
                    {
                        StateIds.Add(mi.Match.CampusStateId.Value);
                        States.Add(new CampusState() { StateId = mi.Match.CampusStateId.Value, StateName = mi.Match.CampusStateCode, StateFullName = StaticInternalObject.States[mi.Match.CampusStateId.Value] });
                    }
                }

                if (directoryMatchRequest.GeoTarget != null &&
                    ((directoryMatchRequest.GeoTarget.CountryList != null &&
                    directoryMatchRequest.GeoTarget.CountryList.Count > 0) ||
                    (directoryMatchRequest.GeoTarget.StateList != null &&
                    directoryMatchRequest.GeoTarget.StateList.Count > 0)) &&
                    mi.Match.CampusCityId.HasValue && !CityIds.Contains(mi.Match.CampusCityId.Value))
                {
                    CityIds.Add(mi.Match.CampusCityId.Value);
                    Cities.Add(new City() { CityId = mi.Match.CampusCityId.Value, CityName = mi.Match.CampusCity, StateId = mi.Match.CampusStateId.Value });
                }

                if (!isHybridResult && mi.Match.IsHybrid)
                    isHybridResult = mi.Match.IsHybrid;

                if (mi.Match.TermList.Count > 0)
                    TermList.UnionWith(mi.Match.TermList);

                if (mi.Match.DurationList.Count > 0)
                    DurationList.UnionWith(mi.Match.DurationList);

                if (mi.Match.WorkTypeList.Count > 0)
                    WorkTypeList.UnionWith(mi.Match.WorkTypeList);

                if (mi.Match.TeachAbroadTypeList.Count > 0)
                    TeachAbroadTypeList.UnionWith(mi.Match.TeachAbroadTypeList);

                if (mi.Match.PlacementAudienceList.Count > 0)
                    PlacementAudienceList.UnionWith(mi.Match.PlacementAudienceList);

                if (mi.Match.LanguageId.HasValue)
                    LanguageList.Add(mi.Match.LanguageId.Value);
            }

            navResponse.TermList = TermList.ToList();
            navResponse.DurationList = DurationList.ToList();
            navResponse.WorkTypeList = WorkTypeList.ToList();
            navResponse.TeachAbroadTypeList = TeachAbroadTypeList.ToList();
            navResponse.PlacementAudienceList = PlacementAudienceList.ToList();
            navResponse.LanguageList = LanguageList.ToList();

            if (Categories.Any())
            {
                var pairedCategories = (from i in Categories
                                        join sc in categoryContent
                                        on i.CategoryId equals sc.CategoryId
                                        select new { i, sc }).ToList();

                pairedCategories.ForEach(pi => pi.i.CategoryName = pi.sc.CategoryName);
                navResponse.Categories = Categories.OrderBy(s => s.CategoryName).ToList();
            }
            if (Subjects.Any())
            {
                var pairedSubjects = (from i in Subjects
                                      join sc in subjectContent
                                      on i.SubjectId equals sc.SubjectId
                                      select new { i, sc }).ToList();

                pairedSubjects.ForEach(pi => pi.i.SubjectName = pi.sc.SubjectName);
                navResponse.Subjects = Subjects.OrderBy(s => s.SubjectName).ToList();
            }

            if (Specialties.Any())
            {
                var pairedSpecialties = (from i in Specialties
                                         join sc in specialtyContent
                                         on i.SpecialtyId equals sc.SpecialtyId
                                         select new { i, sc }).ToList();

                pairedSpecialties.ForEach(pi => pi.i.SpecialtyName = pi.sc.SpecialtyName);
                navResponse.Specialties = Specialties.OrderBy(s => s.SpecialtyName).ToList();
            }

            navResponse.States = States.OrderBy(s => s.StateName).ToList();
            navResponse.ProgramLevels = ProgramLevels.OrderBy(s => s.ProgramLevelName).ToList();
            navResponse.Countries = Countries.OrderBy(s => s.CountryName).ToList();
            navResponse.Cities = Cities.OrderBy(c => c.CityName).ToList();
            navResponse.CampusTypes = CampusTypes;
            navResponse.Hybrid = isHybridResult;

            return navResponse;
        }

        public static CategoryResponse CreateCategoryResponse(MatchResult matchResult, DirectoryMatchRequest request)
        {
            List<CategoryContent> categoryContent = StaticCacheProxyHost.CacheProxy.Get<List<CategoryContent>>(MatchingCacheItem.CategoryContent);
            CategoryResponse categoryResponse = new CategoryResponse();
            categoryResponse.MatchResponseGuid = Guid.NewGuid();
            List<Category> categoryList = new List<Category>();

            List<MatchItem> matchResults = matchResult.MatchItemList;
            Dictionary<int, bool> matchingCategories = new Dictionary<int, bool>();

            foreach (var match in matchResults)
            {
                foreach (int i in match.Match.MatchingCategories(request.CategoryList, request.SubjectList, request.SpecialtyList))
                {
                    if (!matchingCategories.ContainsKey(i))
                        matchingCategories.Add(i, match.FailedValidation);
                    else if (match.FailedValidation == false)
                        matchingCategories[i] = false; //on purpose to make sure if it didn't fail validation to mark it as so.                        
                }

            }

            if (matchingCategories.Count > 0)
            {
                var pairedCategories = (from i in matchingCategories.Keys
                                        join sc in categoryContent
                                        on i equals sc.CategoryId
                                        select sc).ToDictionary(o => o.CategoryId, o => o.CategoryName);

                foreach (var i in matchingCategories.Keys)
                {
                    Category s = new Category() { CategoryId = i, FailedValidation = matchingCategories[i], CategoryName = pairedCategories[i], ProgramRankScore = 0 };
                    categoryList.Add(s);
                }

                categoryResponse.ResultCount = categoryList.Count();
                categoryResponse.CategoryList = categoryList.OrderBy(s => s.CategoryName).ToList();
            }
            else
            {
                categoryResponse.ResultCount = 0;
                categoryResponse.CategoryList = new List<Category>();
            }

            return categoryResponse;
        }

        public static CategoryResponse CreateCategoryWithSubjectsResponse(MatchResult matchResult, DirectoryMatchRequest request)
        {
            CategoryResponse categoryResponse = new CategoryResponse();
            categoryResponse.MatchResponseGuid = Guid.NewGuid();
            List<Category> categoryList = new List<Category>();
            List<SubjectContent> subjectContent = StaticCacheProxyHost.CacheProxy.Get<List<SubjectContent>>(MatchingCacheItem.SubjectContent);
            List<CategoryContent> categoryContent = StaticCacheProxyHost.CacheProxy.Get<List<CategoryContent>>(MatchingCacheItem.CategoryContent);
            List<MatchItem> matchResults = matchResult.MatchItemList;
            GeoCodeProcessor gcp = new GeoCodeProcessor();

            //then remove dupes for the same program/campus but different product/client (also ranked in order to pick best one)
            //matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore , distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId });
            Dictionary<int, HashSet<int>> categorySubjects = new Dictionary<int, HashSet<int>>();
            Dictionary<string, bool> postalCodes = new Dictionary<string, bool>();

            foreach (var m in matchResult.MatchItemList.Where(m => m.FailedValidation == false &&
                                                              m.Match.PaidStatusTypeId == PaidStatusType.Paid &&
                                                              m.Match.InquiryDisabled == false))
            {
                bool process = false;

                if (m.Match.CampusCampusTypeId == (int)CampusType.Online)
                {
                    process = true;
                }
                else if (request.ProspectInput != null && m.Match.CampusCampusTypeId == (int)CampusType.Ground)
                {
                    if (postalCodes.ContainsKey(m.Match.CampusPostalCode))
                    {
                        if (postalCodes[m.Match.CampusPostalCode])
                            process = true;
                    }
                    else
                    {
                        if (gcp.GetDistanceBetweenZipCodes(request.ProspectInput.PostalCode, m.Match.CampusPostalCode) < 100)
                        {
                            process = true;
                            postalCodes.Add(m.Match.CampusPostalCode, true);
                        }
                        else
                        {
                            process = false;
                            postalCodes.Add(m.Match.CampusPostalCode, false);
                        }
                    }
                }

                if (process)
                {
                    HashSet<int> mCategories = m.Match.Categories();
                    foreach (int i in mCategories)
                    {
                        if (categorySubjects.ContainsKey(i))
                        {
                            categorySubjects[i].UnionWith(m.Match.Subjects());
                        }
                        else
                        {
                            categorySubjects.Add(i, new HashSet<int>());
                            categorySubjects[i].UnionWith(m.Match.Subjects());

                            CategoryWithSubject cat = new CategoryWithSubject()
                            {
                                CategoryId = i,
                                FailedValidation = false,
                                ProgramRankScore = 0,
                                SubjectList = new List<SubjectWithProduct>()
                            };

                            foreach (var c in categoryContent)
                            {
                                if (c.CategoryId == cat.CategoryId)
                                    cat.CategoryName = c.CategoryName;
                            }

                            categoryList.Add(cat);
                        }
                    }
                }
            }

            foreach (CategoryWithSubject cat in categoryList)
            {
                HashSet<int> subjects = categorySubjects[cat.CategoryId];

                foreach (int subj in subjects)
                {
                    cat.SubjectList.Add(new SubjectWithProduct()
                    {
                        SubjectId = subj,
                        FailedValidation = false,
                        ProgramRankScore = 0,
                        CampusProducts = new List<int>(),
                        OnlineProducts = new List<int>(),
                        ProgramLevels = new List<ProgramLevel>(),
                        WTCount = 1,
                        SMPCount = 1
                    });
                }

                if (cat.SubjectList.Any())
                {
                    var pairedSubjects = (from i in cat.SubjectList
                                          join sc in subjectContent
                                          on i.SubjectId equals sc.SubjectId
                                          select new { i, sc }).ToList();

                    pairedSubjects.ForEach(pi => pi.i.SubjectName = pi.sc.SubjectName);
                    cat.SubjectList = cat.SubjectList.OrderBy(s => s.SubjectName).ToList();
                }
            }
            //var groupedByCategoryId = matchResults.GroupBy(g => g.Match.PrimaryCategoryId);

            //foreach (var category in groupedByCategoryId)
            //{
            //    MatchItem firstCategory = category.First();

            //    bool failedValidation = !category.Any(c => c.FailedValidation == false);

            //    CategoryWithSubject cat = new CategoryWithSubject() { CategoryId = firstCategory.Match.PrimaryCategoryId.Value, FailedValidation = failedValidation, ProgramRankScore = 0, SubjectList = new List<SubjectWithProduct>() };

            //    var groupedBySubjectId = category.GroupBy(g => g.Match.PrimarySubjectId);
            //    foreach (var subject in groupedBySubjectId)
            //    {
            //        MatchItem firstSubject = subject.First();

            //        bool f = !subject.Any(s => s.FailedValidation == false);
            //        List<int> onlineProducts = subject.Where(s => s.FailedValidation == false && s.Match.CampusCampusTypeId == 1).DistinctBy(p => p.Match.ProductId).Select(o => o.Match.ProductId).ToList();
            //        List<int> campusProducts = subject.Where(s => s.FailedValidation == false && s.Match.CampusCampusTypeId == 2).DistinctBy(p => p.Match.ProductId).Select(o => o.Match.ProductId).ToList();
            //        List<ProgramLevel> programLevels = new List<ProgramLevel>();
            //        int wtcount = subject.Where(s => s.FailedValidation == false && Product.IsWarmTransferProduct(s.Match.ProductId)).DistinctBy(p => p.Match.InstitutionId).Select(s => s.Match.InstitutionId).Count();
            //        int smpcount = subject.Where(s => s.FailedValidation == false && !Product.IsWarmTransferProduct(s.Match.ProductId)).DistinctBy(p => p.Match.InstitutionId).Select(s => s.Match.InstitutionId).Count();

            //        var groupedByProgramLevelId = subject.GroupBy(g => g.Match.ProgramLevelId);
            //        foreach (var level in groupedByProgramLevelId)
            //        {
            //            MatchItem firstLevel = level.First();

            //            programLevels.Add(new ProgramLevel() { ProgramLevelId = firstLevel.Match.ProgramLevelId, ProgramLevelName = firstLevel.Match.ProgramLevelName });
            //        }

            //        cat.SubjectList.Add(new SubjectWithProduct()
            //        {
            //            SubjectId = firstSubject.Match.PrimarySubjectId.Value,
            //            FailedValidation = f,
            //            ProgramRankScore = 0,
            //            CampusProducts = campusProducts,
            //            OnlineProducts = onlineProducts,
            //            ProgramLevels = programLevels,
            //            WTCount = wtcount,
            //            SMPCount = smpcount
            //        });
            //    }

            //    if (cat.SubjectList.Any())
            //    {
            //        var pairedSubjects = (from i in cat.SubjectList
            //                              join sc in subjectContent
            //                              on i.SubjectId equals sc.SubjectId
            //                              select new { i, sc }).ToList();

            //        pairedSubjects.ForEach(pi => pi.i.SubjectName = pi.sc.SubjectName);
            //        cat.SubjectList = cat.SubjectList.OrderBy(s => s.SubjectName).ToList();
            //    }

            //    categoryList.Add(cat);
            //}

            //if (categoryList.Any())
            //{
            //    var pairedCategories = (from i in categoryList
            //                            join sc in categoryContent
            //                            on i.CategoryId equals sc.CategoryId
            //                            select new { i, sc }).ToList();

            //    pairedCategories.ForEach(pi => pi.i.CategoryName = pi.sc.CategoryName);
            //    categoryResponse.CategoryList = categoryList.OrderBy(s => s.CategoryName).ToList();
            //}
            categoryResponse.CategoryList = categoryList.OrderBy(s => s.CategoryName).ToList();
            categoryResponse.ResultCount = categoryList.Count();

            return categoryResponse;
        }

        public static SubjectResponse CreateSubjectResponse(MatchResult matchResult, DirectoryMatchRequest request)
        {
            SubjectResponse subjectResponse = new SubjectResponse();
            subjectResponse.MatchResponseGuid = Guid.NewGuid();
            List<Subject> subjectList = new List<Subject>();
            List<MatchItem> matchResults = matchResult.MatchItemList;
            List<SubjectContent> subjectContent = StaticCacheProxyHost.CacheProxy.Get<List<SubjectContent>>(MatchingCacheItem.SubjectContent);
            Dictionary<int, bool> matchingSubjects = new Dictionary<int, bool>();

            foreach (var match in matchResults)
            {
                foreach (int i in match.Match.MatchingSubjects(request.CategoryList, request.SubjectList, request.SpecialtyList))
                {
                    if (!matchingSubjects.ContainsKey(i))
                        matchingSubjects.Add(i, match.FailedValidation);
                    else if (match.FailedValidation == false)
                        matchingSubjects[i] = false; //on purpose to make sure if it didn't fail validation to mark it as so.                        
                }

            }

            if (matchingSubjects.Count > 0)
            {
                var pairedSubjects = (from i in matchingSubjects.Keys
                                      join sc in subjectContent
                                      on i equals sc.SubjectId
                                      select sc).ToDictionary(o => o.SubjectId, o => o.SubjectName);

                foreach (var i in matchingSubjects.Keys)
                {
                    Subject s = new Subject() { SubjectId = i, FailedValidation = matchingSubjects[i], SubjectName = pairedSubjects[i], ProgramRankScore = 0 };
                    subjectList.Add(s);
                }

                subjectResponse.ResultCount = subjectList.Count();
                subjectResponse.SubjectList = subjectList.OrderBy(s => s.SubjectName).ToList();
            }
            else
            {
                subjectResponse.ResultCount = 0;
                subjectResponse.SubjectList = new List<Subject>();
            }

            return subjectResponse;
        }

        public static SpecialtyResponse CreateSpecialtyResponse(MatchResult matchResult, DirectoryMatchRequest request)
        {
            SpecialtyResponse specialtyResponse = new SpecialtyResponse();
            specialtyResponse.MatchResponseGuid = Guid.NewGuid();
            List<SpecialtyContent> specialtyContent = StaticCacheProxyHost.CacheProxy.Get<List<SpecialtyContent>>(MatchingCacheItem.SpecialtyContent);
            List<Specialty> specialtyList = new List<Specialty>();
            Dictionary<int, bool> matchingSpecialties = new Dictionary<int, bool>();
            List<MatchItem> matchResults = matchResult.MatchItemList;

            foreach (var match in matchResults)
            {
                foreach (int i in match.Match.MatchingSpecialties(request.CategoryList, request.SubjectList, request.SpecialtyList))
                {
                    if (!matchingSpecialties.ContainsKey(i))
                        matchingSpecialties.Add(i, match.FailedValidation);
                    else if (match.FailedValidation == false)
                        matchingSpecialties[i] = false; //on purpose to make sure if it didn't fail validation to mark it as so.                        
                }

            }

            if (matchingSpecialties.Count > 0)
            {
                var pairedSpecialties = (from i in matchingSpecialties.Keys
                                         join sc in specialtyContent
                                         on i equals sc.SpecialtyId
                                         select sc).ToDictionary(o => o.SpecialtyId, o => o.SpecialtyName);

                foreach (var i in matchingSpecialties.Keys)
                {
                    Specialty s = new Specialty() { SpecialtyId = i, FailedValidation = matchingSpecialties[i], SpecialtyName = pairedSpecialties[i], ProgramRankScore = 0 };
                    specialtyList.Add(s);
                }

                specialtyResponse.ResultCount = specialtyList.Count();
                specialtyResponse.SpecialtyList = specialtyList.OrderBy(s => s.SpecialtyName).ToList();
            }
            else
            {
                specialtyResponse.ResultCount = 0;
                specialtyResponse.SpecialtyList = new List<Specialty>();
            }

            return specialtyResponse;
        }

        public static ProgramLevelResponse CreateProgramLevelResponse(MatchResult matchResult)
        {
            ProgramLevelResponse programLevelResponse = new ProgramLevelResponse();
            programLevelResponse.MatchResponseGuid = Guid.NewGuid();
            List<ProgramLevel> programLevelList = new List<ProgramLevel>();
            List<MatchItem> matchResults = matchResult.MatchItemList;

            var groupedByProgramLevelId = matchResults.GroupBy(g => g.Match.ProgramLevelId);

            foreach (var programLevel in groupedByProgramLevelId)
            {
                MatchItem firstProgramLevel = programLevel.First();

                bool failedValidation = !programLevel.Any(c => c.FailedValidation == false);

                programLevelList.Add(new ProgramLevel() { ProgramLevelId = firstProgramLevel.Match.ProgramLevelId, ProgramLevelName = firstProgramLevel.Match.ProgramLevelName, FailedValidation = failedValidation, ProgramRankScore = 0 });
            }

            programLevelResponse.ResultCount = programLevelList.Count();
            programLevelResponse.ProgramLevelList = programLevelList.OrderBy(s => s.ProgramLevelName).ToList();

            return programLevelResponse;
        }

        public static TemplateMatchResponse CreateTemplateMatchResponse(MatchResult templateMatchResult)
        {
            TemplateMatchResponse templateResponse = new TemplateMatchResponse();
            templateResponse.MatchResponseGuid = Guid.NewGuid();

            templateResponse.TemplateIdList = templateMatchResult.MatchItemList.Where(mi => mi.FailedValidation == false).Select(mi => mi.Match.TemplateId).OfType<int>().Distinct().ToList();

            templateResponse.ResultCount = templateResponse.TemplateIdList.Count();

            return templateResponse;
        }

        public static CampusTypeMatchResponse CreateCampusTypeMatchResponse(MatchResult campusTypeMatchResult, DirectoryMatchRequest request)
        {
            CampusTypeMatchResponse templateResponse = new CampusTypeMatchResponse();
            templateResponse.MatchResponseGuid = Guid.NewGuid();

            templateResponse.CampusTypeList = new List<CampusType>();
            string postalCode = request.ProspectInput != null ? request.ProspectInput.PostalCode : null;

            List<ProgramWithInstitutionCampus> onlineSM = GetSmartMatchList(campusTypeMatchResult.MatchItemList, 1, postalCode, CampusType.Online, null, campusTypeMatchResult.ChosenCampaign);
            List<ProgramWithInstitutionCampus> campusSM = GetSmartMatchList(campusTypeMatchResult.MatchItemList, 1, postalCode, CampusType.Ground, null, campusTypeMatchResult.ChosenCampaign);

            if (onlineSM.Count > 0)
                templateResponse.CampusTypeList.Add(CampusType.Online);
            if (campusSM.Count > 0)
                templateResponse.CampusTypeList.Add(CampusType.Ground);

            return templateResponse;
        }

        public static ProgramWithInstitutionCampus CreateWarmTransferTitaniumMatch(MatchResult titaniumMatchResult, WizardMatchRequest wizardRequest, HashSet<int> smartMatchInstitutions)
        {
            ProgramWithInstitutionCampus program = null;
            //TemplateCacheItem programTemplateAssignments = StaticCacheProxyHost.CacheProxy.Get<TemplateCacheItem>(MatchingCacheItem.REProgramProductTemplateAssignment);
            InstitutionGroupCache groupCache = StaticCacheProxyHost.CacheProxy.Get<InstitutionGroupCache>(MatchingCacheItem.InstitutionGroup);

            List<int> tempInstitutions = new List<int>();

            if (smartMatchInstitutions != null)
                tempInstitutions.AddRange(smartMatchInstitutions);
            else
                smartMatchInstitutions = new HashSet<int>();

            foreach (int instId in tempInstitutions)
            {
                if (groupCache.InstitutionToGroups.ContainsKey(instId))
                {
                    foreach (var group in groupCache.InstitutionToGroups[instId])
                    {
                        if (groupCache.GroupToInstitutions.ContainsKey(group.Item1))
                            smartMatchInstitutions.UnionWith(groupCache.GroupToInstitutions[group.Item1]);
                    }
                }
            }

            List<MatchItem> matchResults = titaniumMatchResult.MatchItemList.Where(
                                                    m => m.Match.PaidStatusTypeId == PaidStatusType.Paid
                                                    && m.Match.InquiryDisabled == false).ToList();

            if (wizardRequest.SmartMatchedInstituionIdList != null)
                matchResults.RemoveAll(m => wizardRequest.SmartMatchedInstituionIdList.Contains(m.Match.InstitutionId));

            if (smartMatchInstitutions != null)
                matchResults.RemoveAll(m => smartMatchInstitutions.Contains(m.Match.InstitutionId));

            //first remove duplicate program product ids (due to including subject/category results)
            matchResults = RemoveDuplicates(matchResults, MatchItemSortType.None, distinct => distinct.Match.ProgramProductId);

            //then remove dupes for the same program/campus but different product/client
            matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId });

            if (matchResults.Count > 0)
                program = MatchItemMapper.MapToProgramWithInstitutionCampus(matchResults[0], null, null, false);

            return program;
        }

        public static WizardMatchResponse CreateWizardResponse(MatchResult wizardMatchResult, WizardMatchRequest wizardRequest, PerformanceLog pLog)
        {
            WizardMatchResponse wizardResponse = new WizardMatchResponse();
            wizardResponse.MatchResponseGuid = Guid.NewGuid();
            List<int> originalSmartMatchedInstituionIdList = wizardRequest.SmartMatchedInstituionIdList;

            wizardResponse.DuplicateForInstitutionList = new List<Institution>();

            if (wizardMatchResult.RulesResult != null)
            {
                List<int> externalDupeList = wizardMatchResult.RulesResult.NoMatchOutput.Where(n => n.RuleType == BaseRuleType.ExternalDuplicate).DistinctBy(i => i.entityId).Select(i => i.entityId).ToList();

                if (externalDupeList != null && externalDupeList.Count > 0)
                {
                    var dupedOutInstitutions = wizardMatchResult.MatchItemList.Where(mi => externalDupeList.Contains(mi.Match.InstitutionId)).DistinctBy(mi => mi.Match.InstitutionId).Select(i => new Institution() { InstitutionId = i.Match.InstitutionId, InstitutionName = i.Match.InstitutionName }).ToList();

                    wizardResponse.DuplicateForInstitutionList.AddRange(dupedOutInstitutions);
                }
            }

            List<MatchItem> matchResults = wizardMatchResult.MatchItemList.Where(mi => mi.FailedValidation == false).ToList();

            //first remove duplicate program product ids (due to including subject/category results)
            matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => distinct.Match.ProgramProductId);

            //then filter lead scoring products
            //LeadScoringProcessor lsProcessor = new LeadScoringProcessor();
            int? leadScoringTierLevel = wizardRequest.LeadScoringInput != null ? wizardRequest.LeadScoringInput.LeadScoringTierLevel : null;
            //List<MatchItem> leadScoreResults = lsProcessor.Execute(matchResults, wizardRequest.LeadScoringInput != null ? wizardRequest.LeadScoringInput.LeadScoringTierLevel : null, wizardRequest.TrackGuid, wizardRequest.Application);
            //matchResults = leadScoreResults;

            //int totalMatches = matchResults.Count;

            if (wizardRequest.IncludeSmartMatchList && wizardMatchResult.ChosenCampaign.MaxSmartMatchCount.HasValue && wizardMatchResult.ChosenCampaign.MaxSmartMatchCount > 0)
            {
                wizardResponse.SmartMatchList = GetSmartMatches(matchResults, GetSmartMatchesAllowed(wizardMatchResult, wizardRequest, leadScoringTierLevel), wizardRequest.CampusPreference, wizardRequest.ProspectInput.PostalCode, wizardMatchResult.ChosenCampaign, wizardRequest.ProspectInput.EducationLevelId);

                if (wizardRequest.SmartMatchedInstituionIdList == null)
                    wizardRequest.SmartMatchedInstituionIdList = wizardResponse.SmartMatchList.Select(sm => sm.InstitutionId).ToList();
                else
                    wizardRequest.SmartMatchedInstituionIdList.AddRange(wizardResponse.SmartMatchList.Select(sm => sm.InstitutionId));
            }

            matchResults = RemoveDuplicates(matchResults, MatchItemSortType.None, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId, ProductGroup = distinct.Match.ProductGrouping() });

            if (wizardResponse.SmartMatchList != null && wizardResponse.SmartMatchList.Any(m => !m.FailedValidation && ((m.TreatAsMatch1 && m.ProductId != (int)ProductType.Lead_GS) || (m.ProductId == (int)ProductType.Match1Exclusive))))
            {
                //this match result has a match one exclusive product and should definitely skip school selection
                wizardResponse.SkipSchoolSelection = true;
            }

            if (wizardRequest.IncludeSchoolSelectionList && wizardMatchResult.ChosenCampaign.MaxItemsDisplayed.HasValue && wizardMatchResult.ChosenCampaign.MaxItemsDisplayed > 0)
                wizardResponse.SchoolSelectionList = GetSchoolSelections(matchResults, wizardRequest, wizardMatchResult, pLog);

            wizardRequest.SmartMatchedInstituionIdList = originalSmartMatchedInstituionIdList; //this list gets changed inside the method, but need to restore it back so its accurate when its saved to the matchresponse table
            //Zack - commented this out because it would not include all the results, need this for logging
            //wizardMatchResult.MatchItemList = matchResults;

            if (wizardRequest.IncludeSmartMatchList && wizardRequest.IncludeThirdPartyMatchList)
            {
                ThirdPartyMatchProcessor processor = new ThirdPartyMatchProcessor(wizardRequest, pLog, wizardResponse.SmartMatchList);

                wizardResponse.ThirdPartyMatchList = processor.GetSmartMatchList();
            }
            else
                wizardResponse.ThirdPartyMatchList = new List<ProgramWithInstitutionCampus>();

            return wizardResponse;
        }

        private static int GetSmartMatchesAllowed(MatchResult wizardMatchResult, WizardMatchRequest wizardRequest, int? leadScoringTierLevel)
        {
            int smAllowed;

            int campaignAllowed = wizardMatchResult.ChosenCampaign.MaxSmartMatchCount ?? 0;

            if (campaignAllowed > 0)
            {
                int totalSaved = Math.Max(wizardRequest.TotalLeadsSaved, wizardRequest.TotalSmartMatchesSaved);

                smAllowed = Math.Max(campaignAllowed - totalSaved, 0);
            }
            else
                smAllowed = campaignAllowed;

            return smAllowed;
        }

        private static List<CampusWithInstitution> GetSchoolSelections(List<MatchItem> matchItemList, WizardMatchRequest wizardRequest, MatchResult wizardMatchResult, PerformanceLog pLog)
        {
            Dictionary<int, List<SchoolAgent>> crAgents = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<SchoolAgent>>>(MatchingCacheItem.ClientRelationContacts);
            Dictionary<int, List<SchoolAgent>> ccAgents = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<SchoolAgent>>>(MatchingCacheItem.ClientCampusContacts);
            Campaign c = Campaign.Get(wizardRequest.TrackGuid);

            List<CampusWithInstitution> schoolSelectionList = new List<CampusWithInstitution>();
            int selections = 0;

            //remove previously matched institutions
            if (wizardRequest.SmartMatchedInstituionIdList != null && wizardRequest.SmartMatchedInstituionIdList.Count > 0)
                matchItemList = matchItemList.Where(mi => !wizardRequest.SmartMatchedInstituionIdList.Contains(mi.Match.InstitutionId)).ToList();

            //Filter out Match1Exclusive Product
            matchItemList = matchItemList.Where(mi => mi.Match.ProductId != (int)ProductType.Match1Exclusive && mi.Match.ProductId != (int)ProductType.Match1Plus).ToList();

            //TemplateCacheItem programTemplateAssignments = StaticCacheProxyHost.CacheProxy.Get<TemplateCacheItem>(MatchingCacheItem.REProgramProductTemplateAssignment);

            //Zack: removed per Yichen on 9/18/19
            //if (wizardRequest.ProspectInput.EducationLevelId.HasValue && wizardRequest.DesiredProgramLevelList != null && wizardRequest.DesiredProgramLevelList.Any())
            //{
            //    CrossSellProcessor xSell = new CrossSellProcessor(null);

            //    List<CrossSellMapping> xSellMapping = xSell.GetSearchProgramLevels(wizardRequest.ProspectInput.EducationLevelId.Value, wizardRequest.DesiredProgramLevelList);

            //    var allowedProgramLevels = xSellMapping.Select(xs => xs.SearchProgramLevelId).Distinct();

            //    matchItemList = matchItemList.Where(mi => allowedProgramLevels.Contains(mi.Match.ProgramLevelId)).ToList();
            //}

            //START - Third Party API Match logic
            if (wizardRequest.Application != Base.ISApplication.VendorAPI || matchItemList.Count == 0)
            {
                List<ProgramWithInstitutionCampus> previousMatchList = new List<ProgramWithInstitutionCampus>();

                Dictionary<int, InstitutionContent> instContent = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, InstitutionContent>>(MatchingCacheItem.InstitutionContent);

                if (wizardRequest.SmartMatchedInstituionIdList != null)
                {
                    foreach (int instId in wizardRequest.SmartMatchedInstituionIdList)
                    {
                        if (wizardRequest.ThirdPartySmartMatchList.Where(tpsm => tpsm.InstitutionId == instId).Any())
                        {
                            foreach (ProgramWithInstitutionCampus thirdPartyInst in wizardRequest.ThirdPartySmartMatchList.Where(tpsm => tpsm.InstitutionId == instId).ToList())
                                previousMatchList.Add(new ProgramWithInstitutionCampus() { InstitutionId = thirdPartyInst.InstitutionId, InstitutionName = thirdPartyInst.InstitutionName });
                        }
                        else
                        {
                            previousMatchList.Add(new ProgramWithInstitutionCampus() { InstitutionId = instContent[instId].InstitutionId, InstitutionName = instContent[instId].InstitutionName });
                        }
                    }
                }

                if (matchItemList != null && matchItemList.Any())
                {
                    List<ProgramWithInstitutionCampus> ssList = matchItemList.DistinctBy(mi => mi.Match.InstitutionId).Select(m => new ProgramWithInstitutionCampus() { InstitutionName = m.Match.InstitutionName, InstitutionId = m.Match.InstitutionId }).ToList();
                    previousMatchList.AddRange(ssList);
                }

                ThirdPartyMatchProcessor processor = new ThirdPartyMatchProcessor(wizardRequest, pLog, previousMatchList);

                if (wizardRequest.Application == Base.ISApplication.VendorAPI)
                    matchItemList.AddRange(processor.GetMatchItems(LeadCreationType.HostAndPost));
                else
                    matchItemList.AddRange(processor.GetMatchItems(LeadCreationType.WizardUserSelectionThirdParty));
            }
            //END - Third Party API Match logic

            var groupedByCampus = matchItemList.GroupBy(g => new { g.Match.CampusId, g.Match.InstitutionName, ProductGroup = g.Match.ProductGrouping() });
            bool isContactCenter = c.ChannelId == 13; //Contact Center Services

            GeoCodeProcessor geoProc = new GeoCodeProcessor();

            foreach (var campusItem in groupedByCampus)
            {
                CampusWithInstitution campus = new CampusWithInstitution();
                MatchItem firstCampus = campusItem.First();

                //only pick the first campus per institution
                if (schoolSelectionList.Select(ss => new { ss.InstitutionName, ss.IsLiveTransfer }).Contains(new { firstCampus.Match.InstitutionName, IsLiveTransfer = Product.IsWarmTransferProduct(firstCampus.Match.ProductId) }))
                    continue;

                campus.CampusId = firstCampus.Match.CampusId;
                campus.CampusType = (CampusType)firstCampus.Match.CampusCampusTypeId;
                campus.CampusName = firstCampus.Match.CampusName;
                campus.Address1 = firstCampus.Match.CampusAddress1;
                campus.Address2 = firstCampus.Match.CampusAddress2;
                campus.City = firstCampus.Match.CampusCity;
                campus.State = firstCampus.Match.CampusStateCode;
                campus.PostalCode = firstCampus.Match.CampusPostalCode;
                campus.CountryCode = firstCampus.Match.CampusCountry;
                campus.Phone = firstCampus.Match.CampusPhone;
                campus.Fax = firstCampus.Match.CampusFax;
                campus.ProgramCount = campusItem.Count();
                campus.Is2USchool = ConfigurationManager.AppSettings["Enable2UTCPAChange"] == "true" && firstCampus.Match.ClientId == 349 ? true : false;
                campus.InstitutionId = firstCampus.Match.InstitutionId;
                campus.InstitutionName = isContactCenter && firstCampus.Match.InstitutionId == 21 ? "American InterContinental University, a member of the AIU System" : firstCampus.Match.InstitutionName;
                campus.InstitutionDescription = firstCampus.Match.InstitutionDescription;
                campus.InstitutionDescriptionInternational = firstCampus.Match.InstitutionDescriptionInternational;
                campus.InstitutionDisclaimerType = firstCampus.Match.InstitutionDisclaimerType;
                campus.InstitutionDisclaimer = firstCampus.Match.InstitutionDisclaimer;
                campus.DistanceFromProspect = geoProc.GetDistanceBetweenZipCodes(campus.PostalCode, wizardRequest.ProspectInput.PostalCode);
                campus.CampusLogoURL = firstCampus.Match.CampusLogoURL;
                campus.ProgramList = new List<Program>();
                campus.InstitutionLogoURL = firstCampus.Match.InstitutionLogoURL;
                campus.StartDateList = firstCampus.Match.InstitutionStartDateList;
                campus.CustomTCPA = firstCampus.Match.CustomTCPA;
                campus.CustomContactCenterTCPA = firstCampus.Match.CustomContactCenterTCPA;

                bool isWT = Product.IsWarmTransferProduct(firstCampus.Match.ProductId);

                if (isWT)
                {
                    campus.IsLiveTransfer = true;
                    campus.SchoolAgents = new List<SchoolAgent>();

                    if (ccAgents.ContainsKey(firstCampus.Match.ClientCampusRelationshipId))
                        campus.SchoolAgents.AddRange(ccAgents[firstCampus.Match.ClientCampusRelationshipId]);
                    else if (crAgents.ContainsKey(firstCampus.Match.ClientRelationshipId))
                        campus.SchoolAgents.AddRange(crAgents[firstCampus.Match.ClientRelationshipId]);
                }

                foreach (var programItem in campusItem)
                {
                    Program program = new Program()
                    {
                        ProgramProductId = programItem.Match.ProgramProductId,
                        ProgramId = programItem.Match.ProgramId,
                        ProgramName = programItem.Match.ProgramName,
                        DegreeAcronym = programItem.Match.DegreeAcronym,
                        ProgramType = (ProgramType)programItem.Match.ProgramTypeId,
                        ProgramLevelId = programItem.Match.ProgramLevelId,
                        ProgramLevelName = programItem.Match.ProgramLevelName,
                        SubjectId = programItem.Match.PrimarySubjectId,
                        CategoryId = programItem.Match.PrimaryCategoryId,
                        ProgramCampusType = (CampusType)programItem.Match.ProgramCampusTypeId,
                        ProgramRankScore = programItem.ProgramRankScore,
                        FailedValidation = programItem.FailedValidation,
                        PaidStatusTypeId = programItem.Match.PaidStatusTypeId,
                        ClickThroughUrl = programItem.Match.ClickThroughUrl,
                        ProductId = programItem.Match.ProductId,
                        //Below "hack" done to reduce message size but still allow the school selection page to know if a description/disclaimer exists
                        //ProgramDisclaimerType = programItem.ProgramDisclaimerType,
                        ProgramDisclaimer = string.IsNullOrWhiteSpace(programItem.Match.ProgramDisclaimer) ? "0" : "1",
                        ProgramDescription = string.IsNullOrWhiteSpace(programItem.Match.ProgramDescription) ? "0" : "1",
                        ShowTwoULeadShareControl = ConfigurationManager.AppSettings["Enable2ULeadShare"] == "true" ? programItem.Match.ShowLeadShare2U : false,
                        AllowedViaLeadScoringUpsell = programItem.AllowedViaLeadScoringUpsell,
                        TemplateId = programItem.Match.TemplateId,
                        SchoolLogoURL = programItem.Match.SchoolLogoURL,
                        ProgramLogoURL = programItem.Match.ProgramLogoURL,
                        CampusLogoURL = programItem.Match.CampusLogoURL,
                        InstitutionLogoURL = programItem.Match.InstitutionLogoURL,
                        ExternalMatchItemGuid = programItem.ExternalMatchItemGuid,
                        InstitutionType = programItem.InstitutionLeadType,
                        TransferNumber = isWT ? LiveTransferNumberProcessor.GetLiveTransferNumber(programItem, wizardRequest.ProspectInput) : "",
                        EffectiveRevenuePerLead = programItem.Match.eRPL,
                        RevenuePerLead = programItem.Match.RPL

                    };

                    //int templateId;

                    //if (programTemplateAssignments.ProgramProductToTemplateAssignments.TryGetValue(programItem.Match.ProgramProductId, out templateId))
                    //    program.TemplateId = templateId;

                    campus.ProgramList.Add(program);
                }

                bool failedValidation;

                decimal rankScore = CalculateRolledupRankScore(campus.ProgramList, wizardMatchResult.ChosenBusinessModel, out failedValidation);

                campus.FailedValidation = failedValidation;
                campus.ProgramRankScore = rankScore;

                schoolSelectionList.Add(campus);
                selections++;
            }

            //InstitutionGroups
            var instGroupRuleInput = schoolSelectionList.DistinctBy(d => d.InstitutionId).Select(f => new { InstitutionId = f.InstitutionId.Value, f.ProgramRankScore }).ToDictionary(k => k.InstitutionId, v => v.ProgramRankScore);

            if (wizardRequest.SmartMatchedInstituionIdList != null && wizardRequest.SmartMatchedInstituionIdList.Count > 0)
            {
                foreach (var instId in wizardRequest.SmartMatchedInstituionIdList)
                {
                    if (!instGroupRuleInput.ContainsKey(instId))
                        instGroupRuleInput.Add(instId, 99999);
                }
            }

            List<int> institutionsToRemove = new List<int>();
            //Do execute institution group rule if requests are from Inside Academics, LeadVision or Call Center Partners
            if (c.VendorId != 12093 && c.VendorId != 12547 && !(c.SubChannelId == 8 && wizardRequest.Application == Base.ISApplication.VendorAPI))
                institutionsToRemove = RulesEngine.ExecuteInstitutionGroupRule(instGroupRuleInput, true, c.AllowPECDoubleMatch);

            schoolSelectionList = schoolSelectionList.Where(pl => !institutionsToRemove.Contains(pl.InstitutionId.Value)).ToList();
            //InstitutionGroups

            int maxToDisplay = wizardMatchResult.ChosenCampaign.MaxItemsDisplayed.Value;

            if (wizardRequest.SplitCampusTypeInResults.HasValue && wizardRequest.SplitCampusTypeInResults.Value)
            {
                var tempListOnline = schoolSelectionList.Where(ss => ss.CampusType == CampusType.Online).OrderByDescending(ss => ss.ProgramRankScore).Take(maxToDisplay);

                var tempListCampus = schoolSelectionList.Where(ss => ss.CampusType == CampusType.Ground).OrderByDescending(ss => ss.ProgramRankScore).Take(maxToDisplay);

                schoolSelectionList = tempListOnline.Union(tempListCampus).ToList();
            }
            else
            {
                //backfill logic
                if (wizardRequest.CampusPreference.HasValue && wizardRequest.CampusPreference.Value == CampusPreference.Online)
                    schoolSelectionList = schoolSelectionList.OrderBy(pl => pl.CampusType).ThenByDescending(pl => pl.ProgramRankScore).Take(maxToDisplay).ToList();
                else if (wizardRequest.CampusPreference.HasValue && wizardRequest.CampusPreference.Value == CampusPreference.Ground)
                    schoolSelectionList = schoolSelectionList.OrderByDescending(pl => pl.CampusType).ThenByDescending(pl => pl.ProgramRankScore).Take(maxToDisplay).ToList();
                else
                    schoolSelectionList = schoolSelectionList.OrderByDescending(pl => pl.ProgramRankScore).Take(maxToDisplay).ToList();
            }
            return schoolSelectionList;
        }

        private static ProgramWithInstitutionCampus GetMatch1ExclusiveSmartMatchList(List<MatchItem> matchItemList, int numberOfSmartMatches, CampusPreference? campusPref)
        {
            List<MatchItem> filteredMatches = matchItemList;

            //filter to the campus type and remove match 1 exclusive match items.
            if (campusPref.HasValue && campusPref.Value == CampusPreference.Online)
                filteredMatches = filteredMatches.Where(mi => mi.Match.ProgramCampusTypeId == (int)CampusType.Online).ToList();

            var groupedByInst = filteredMatches.Where(mi => mi.Match.CanSmartMatch == true).GroupBy(g => g.Match.InstitutionId).ToList();

            if (groupedByInst != null && groupedByInst.Count() > 0)
            {
                //InstitutionGroups
                var instGroupRuleInput = groupedByInst.Select(f => new { f.First().Match.InstitutionId, f.First().ProgramRankScore }).ToDictionary(k => k.InstitutionId, v => v.ProgramRankScore);

                List<int> institutionsToRemove = RulesEngine.ExecuteInstitutionGroupRule(instGroupRuleInput);

                groupedByInst = groupedByInst.Where(pl => !institutionsToRemove.Contains(pl.First().Match.InstitutionId)).ToList();

                if (groupedByInst[0].First().Match.ProductId == (int)ProductType.Match1Exclusive || groupedByInst[0].First().Match.ProductId == (int)ProductType.Match1Plus || groupedByInst[0].First().Match.TreatAsMatch1)
                {
                    if (groupedByInst.Count() > 1)
                    {
                        MatchItem mi = groupedByInst[0].First();

                        int programCounter = 0;
                        decimal cumulativeRPL = 0;

                        foreach (var g in groupedByInst)
                        {
                            if (programCounter >= numberOfSmartMatches)
                                break;

                            if (g.First().Match.ProductId != (int)ProductType.Match1Exclusive && g.First().Match.ProductId != (int)ProductType.Match1Plus && !g.First().Match.TreatAsMatch1)
                            {
                                programCounter++;
                                cumulativeRPL += g.First().Match.eRPL;
                            }
                        }

                        if (mi.Match.eRPL >= cumulativeRPL)
                        {
                            return MatchItemMapper.MapToProgramWithInstitutionCampus(mi, null, null, false);
                        }
                    }
                    else if (groupedByInst.Count() == 1)
                    {
                        return MatchItemMapper.MapToProgramWithInstitutionCampus(groupedByInst[0].First(), null, null, false);
                    }
                }
            }

            return null;
        }

        private static List<ProgramWithInstitutionCampus> GetSmartMatchList(List<MatchItem> matchItemList, int numberOfSmartMatches, string leadPostalCode, CampusType campusType, List<int> excludedInstitionList, Campaign campaign)
        {
            List<ProgramWithInstitutionCampus> smartMatchList = new List<ProgramWithInstitutionCampus>();

            List<MatchItem> filteredMatches = matchItemList;
            int smartMatches = 0;

            if (excludedInstitionList != null && excludedInstitionList.Count() > 0)
                filteredMatches = filteredMatches.Where(mi => !excludedInstitionList.Contains(mi.Match.InstitutionId)).ToList();

            //Filter to the campus type and remove match1exclusive programs
            filteredMatches = filteredMatches.Where(mi => mi.Match.ProgramCampusTypeId == (int)campusType && mi.Match.ProductId != (int)ProductType.Match1Exclusive && mi.Match.ProductId != (int)ProductType.Match1Plus && !mi.Match.TreatAsMatch1).ToList();

            var groupedByInst = filteredMatches.Where(mi => mi.Match.CanSmartMatch == true).GroupBy(g => g.Match.InstitutionId).ToList();

            if (groupedByInst != null && groupedByInst.Count() > 0)
            {
                //InstitutionGroups
                var instGroupRuleInput = groupedByInst.Select(f => new { f.First().Match.InstitutionId, f.First().ProgramRankScore }).ToDictionary(k => k.InstitutionId, v => v.ProgramRankScore);

                if (excludedInstitionList != null && excludedInstitionList.Count > 0)
                {
                    foreach (var instId in excludedInstitionList)
                        instGroupRuleInput.Add(instId, 99999);
                }

                List<int> institutionsToRemove = RulesEngine.ExecuteInstitutionGroupRule(instGroupRuleInput, removePECGroup : campaign.AllowPECDoubleMatch);

                groupedByInst = groupedByInst.Where(pl => !institutionsToRemove.Contains(pl.First().Match.InstitutionId)).ToList();
                //InstitutionGroups
            }

            foreach (var institutionItem in groupedByInst)
            {
                if (smartMatches >= numberOfSmartMatches)
                    break;

                //Only the first program (top ranked) per institution should be sold!
                MatchItem programItem = institutionItem.First();

                ProgramWithInstitutionCampus program = MatchItemMapper.MapToProgramWithInstitutionCampus(programItem, null, null, false);

                smartMatchList.Add(program);

                smartMatches++;
            }

            return smartMatchList;
        }

        private static List<ProgramWithInstitutionCampus> GetSmartMatches(List<MatchItem> matchItemList, int numberOfSmartMatches, CampusPreference? campusPref, string leadPostalCode, Campaign campaign, int? educationLevelId)
        {
            List<ProgramWithInstitutionCampus> smartMatchList = new List<ProgramWithInstitutionCampus>();

            //Zack: not needed, redundant with code in GetWizardMatches method
            //if (desiredProgramLevelList != null && desiredProgramLevelList.Any())
            //    matchItemList = matchItemList.Where(mi => desiredProgramLevelList.Contains(mi.Match.ProgramLevelId)).ToList();

            if (campusPref.HasValue && campusPref.Value == CampusPreference.Ground)
            {
                smartMatchList = GetSmartMatchList(matchItemList, numberOfSmartMatches, leadPostalCode, CampusType.Ground, null, campaign);
            }
            else
            {
                //Determine whether to do Match1Exclusive or not
                ProgramWithInstitutionCampus exclusiveSM = GetMatch1ExclusiveSmartMatchList(matchItemList, numberOfSmartMatches, campusPref);

                if (exclusiveSM == null)
                {
                    if (campusPref.HasValue && campusPref.Value == CampusPreference.Online)
                    {
                        smartMatchList = GetSmartMatchList(matchItemList, numberOfSmartMatches, leadPostalCode, CampusType.Online, null, campaign);
                    }
                    else //Both Preference
                    {
                        // 1 Ground SM
                        smartMatchList = GetSmartMatchList(matchItemList, 1, leadPostalCode, CampusType.Ground, null, campaign);

                        List<int> matchedInstitutions = null;

                        int numberOfGroundMatches = 0;

                        if (smartMatchList != null)
                        {
                            //if campaign setting active, then the first ground match should not count against the total number of smart matches allowed
                            if (!campaign.CampusAddAdditionalSmartMatch)
                                numberOfGroundMatches = smartMatchList.Count();

                            matchedInstitutions = smartMatchList.Select(sm => sm.InstitutionId).Distinct().ToList();
                        }

                        // The rest online SM
                        smartMatchList.AddRange(GetSmartMatchList(matchItemList, numberOfSmartMatches - numberOfGroundMatches, leadPostalCode, CampusType.Online, matchedInstitutions, campaign));

                        // If haven't filled the number of possible matches, go back to ground
                        if (smartMatchList.Count() < numberOfSmartMatches && numberOfGroundMatches > 0)
                        {
                            matchedInstitutions = smartMatchList.Select(sm => sm.InstitutionId).Distinct().ToList();

                            smartMatchList.AddRange(GetSmartMatchList(matchItemList, numberOfSmartMatches - smartMatchList.Count(), leadPostalCode, CampusType.Ground, matchedInstitutions, campaign));
                        }
                    }
                }
                else
                {
                    smartMatchList.Add(exclusiveSM);
                }
            }
            return smartMatchList;
        }

        public static FormProgramResponse CreateFormProgramResponse(MatchResult matchResult, int? programId)
        {
			Dictionary<int, VW_Matching_CampusOptionGroupContent> campusOptionGroups = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_CampusOptionGroupContent>>(MatchingCacheItem.CampusOptionGroups);

			FormProgramResponse formProgramResponse = new FormProgramResponse();
            formProgramResponse.MatchResponseGuid = Guid.NewGuid();

            //first remove duplicate program product ids (due to including subject/category results)
            List<MatchItem> matchResults = RemoveDuplicates(matchResult.MatchItemList, MatchItemSortType.None, distinct => distinct.Match.ProgramProductId);

            MatchItem requestedProgramItem = null;

            if (programId.HasValue)
                requestedProgramItem = matchResults.Where(mi => mi.Match.ProgramId == programId.Value).FirstOrDefault();

            //then remove dupes for the same program but different product/client (also ranked in order to pick best one)            
            matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => distinct.Match.ProgramDisplayName);

            List<FormProgram> formProgramList = new List<FormProgram>();

            //Dedupe by name can remove the requested programid, need to add it back if this happens
            if (requestedProgramItem != null && !matchResults.Any(mi => mi.Match.ProgramId == programId.Value))
            {
                matchResults.RemoveAll(mi => mi.Match.ProgramName == requestedProgramItem.Match.ProgramName);
                matchResults.Add(requestedProgramItem);
            }

            foreach (var programItem in matchResults)
            {
                FormProgram formProgram = new FormProgram();

                if (programItem.Match.ClientId == 349)
                    formProgram.Is2USchool = ConfigurationManager.AppSettings["Enable2UTCPAChange"] == "true" ? true : false;

                formProgram.ProgramId = programItem.Match.ProgramId;
                formProgram.ProgramLevelId = programItem.Match.ProgramLevelId;
                formProgram.ProgramLevelName = programItem.Match.ProgramLevelName;
                //TFS-101062 - Updating ME form program response to return display name rather than program name.
                formProgram.ProgramName = programItem.Match.ProgramDisplayName;
                formProgram.ProductId = programItem.Match.ProductId;
                formProgram.ProgramProductId = programItem.Match.ProgramProductId;
                formProgram.CampusType = (CampusType)programItem.Match.CampusCampusTypeId;
                formProgram.PaidStatusTypeId = programItem.Match.PaidStatusTypeId;
                formProgram.RequiresSystemTemplateUse = programItem.Match.RequiresSystemTemplateUse;
                formProgram.ShowTwoULeadShareControl = ConfigurationManager.AppSettings["Enable2ULeadShare"] == "true" ? programItem.Match.ShowLeadShare2U : false;
                formProgram.TemplateId = programItem.Match.TemplateId;
                formProgram.ProgramRankScore = programItem.ProgramRankScore;
				formProgram.CampusOptionGroup = programItem.Match.CampusOptionGroupId.HasValue ? campusOptionGroups[programItem.Match.CampusOptionGroupId.Value].CampusOptionGroupName : "";
				formProgram.CampusOptionGroupPosition = programItem.Match.CampusOptionGroupId.HasValue ? campusOptionGroups[programItem.Match.CampusOptionGroupId.Value].Position : new Nullable<int>();
                if (string.IsNullOrEmpty(formProgramResponse.CustomTCPA))
                {
                    formProgramResponse.CustomTCPA = programItem.Match.CustomTCPA;
                }
                //int templateId;
                //if (templateCache.ProgramProductToTemplateAssignments.TryGetValue(programItem.Match.ProgramProductId, out templateId))
                //    formProgram.TemplateId = templateId;

                formProgramList.Add(formProgram);
            }

            formProgramResponse.FormProgramList = formProgramList;
            formProgramResponse.ResultCount = formProgramList.Count();

            return formProgramResponse;
        }

        public static CrossSellProgramResponse CreateCrossSellResponse(CrossSellMatchResult xSellMatchResult, List<MatchItem> matchResults, int? maxResultsCount, int initialLeadInstitutionId, int? leadScoringTierLevel, Guid campaignTrackGuid, bool initialLeadSuccess)
        {
            CrossSellProgramResponse crossSellResponse = new CrossSellProgramResponse();
            crossSellResponse.MatchResponseGuid = Guid.NewGuid();

            //moved to crossSell processor
            //List<MatchItem> matchResults = xSellMatchResult.MatchItemList.Where(m => m.Match.PaidStatusTypeId == PaidStatusType.Paid &&
            //                                                                         m.Match.ProductId != (int)ProductType.Match1Exclusive &&
            //                                                                         m.Match.ProductId != (int)ProductType.Match1Plus &&
            //                                                                         m.Match.InquiryDisabled == false &&
            //                                                                         m.FailedValidation == false).ToList();

            //List<MatchItem> matchResults = xSellMatchResult.MatchItemList;
            //first remove duplicate program product ids (due to including subject/category results)
            matchResults = RemoveDuplicates(matchResults, MatchItemSortType.None, distinct => distinct.Match.ProgramProductId);

            //TemplateCacheItem templateCache = StaticCacheProxyHost.CacheProxy.Get<TemplateCacheItem>(MatchingCacheItem.REProgramProductTemplateAssignment);

            //then filter lead scoring products
            //LeadScoringProcessor lsProcessor = new LeadScoringProcessor();
            //List<MatchItem> leadScoreResults = lsProcessor.Execute(matchResults, leadScoringTierLevel, campaignTrackGuid, null);
            //matchResults = leadScoreResults;

            //then remove dupes for the same program/campus but different product/client (also ranked in order to pick best one)
            matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId });

            List<int> programLevelOrderList = null;

            if (xSellMatchResult.CrossSellMappingList != null && xSellMatchResult.CrossSellMappingList.Count > 0)
                programLevelOrderList = xSellMatchResult.CrossSellMappingList.OrderBy(xs => xs.Priority).Select(f => f.SearchProgramLevelId).ToList();

            if (programLevelOrderList != null && programLevelOrderList.Count > 1)
            {
                var campusMatchResults = new List<MatchItem>();
                var onlineMatchResults = new List<MatchItem>();

                foreach (int programLevel in programLevelOrderList)
                {
                    var campusListToAdd = matchResults.Where(mr => mr.Match.ProgramLevelId == programLevel && mr.Match.ProgramCampusTypeId == (int)CampusType.Ground).OrderByDescending(pl => pl.ProgramRankScore);

                    campusMatchResults.AddRange(campusListToAdd);

                    var onlineListToAdd = matchResults.Where(mr => mr.Match.ProgramLevelId == programLevel && mr.Match.ProgramCampusTypeId == (int)CampusType.Online).OrderByDescending(pl => pl.ProgramRankScore);

                    onlineMatchResults.AddRange(onlineListToAdd);
                }

                matchResults.Clear();

                //TFS #82820 - if initial lead fails, keep that institution and show it first in the list
                if (!initialLeadSuccess)
                    matchResults.AddRange(matchResults.Where(mr => mr.Match.InstitutionId == initialLeadInstitutionId).OrderByDescending(pl => pl.ProgramRankScore));

                if (xSellMatchResult.TemplateProgramCampusType == CampusType.Online)
                {
                    matchResults.AddRange(onlineMatchResults);
                    matchResults.AddRange(campusMatchResults);
                }
                else
                {
                    matchResults.AddRange(campusMatchResults);
                    matchResults.AddRange(onlineMatchResults);
                }
            }
            else
            {
                if (xSellMatchResult.IsSAB)
                {
                    matchResults = matchResults.OrderBy(mi => mi.Match.SFProductCode_PSI).ThenByDescending(pl => pl.ProgramRankScore).ToList();
                }
                else
                {
                    if (xSellMatchResult.TemplateProgramCampusType == CampusType.Online)
                        matchResults = matchResults.OrderBy(pl => pl.Match.ProgramCampusTypeId).ThenByDescending(pl => pl.ProgramRankScore).ToList();
                    else
                        matchResults = matchResults.OrderByDescending(pl => pl.Match.ProgramCampusTypeId).ThenByDescending(pl => pl.ProgramRankScore).ToList();
                }
            }

            List<ProgramWithInstitutionCampus> programList = new List<ProgramWithInstitutionCampus>();

            var groupedByInst = matchResults.GroupBy(g => g.Match.InstitutionId);

            foreach (var institutionItem in groupedByInst)
            {
                //Only the first program (top ranked) per institution should be sold!
                MatchItem programItem = institutionItem.First();

                ProgramWithInstitutionCampus program = MatchItemMapper.MapToProgramWithInstitutionCampus(programItem, null, null, false);

                programList.Add(program);
            }

            //InstitutionGroups
            var instGroupRuleInput = programList.Select(f => new { f.InstitutionId, f.ProgramRankScore }).ToDictionary(k => k.InstitutionId, v => v.ProgramRankScore);

            if (instGroupRuleInput.ContainsKey(initialLeadInstitutionId))
                instGroupRuleInput[initialLeadInstitutionId] = 99999;
            else
                instGroupRuleInput.Add(initialLeadInstitutionId, 99999);

            List<int> institutionsToRemove = RulesEngine.ExecuteInstitutionGroupRule(instGroupRuleInput);

            programList = programList.Where(pl => !institutionsToRemove.Contains(pl.InstitutionId)).ToList();
            //InstitutionGroups

            programList = PaginateResults(programList, maxResultsCount, null);

            crossSellResponse.ProgramList = programList;
            crossSellResponse.ResultCount = programList.Count();

            return crossSellResponse;
        }

        public static ProgramResponse CreateProgramResponse(MatchResult matchResult, EntitySortMethod sortMethod, int? maxResultsCount, int? pageNumber, bool? includeProgramDetails, bool? includeProgramGroupRollup, int applicationId, bool includeImages)
        {
            ProgramResponse programResponse = new ProgramResponse();
            programResponse.MatchResponseGuid = Guid.NewGuid();
            //first remove duplicate program product ids (due to including subject/category results)
            List<MatchItem> matchResults = RemoveDuplicates(matchResult.MatchItemList, MatchItemSortType.None, distinct => distinct.Match.ProgramProductId);

            ExecuteApplicationRules(ref matchResults, applicationId);

            //need to get click programs from the set before removing duplicate programs
            Dictionary<int, MatchItem> clickPrograms = GetAllClickProgramsInSet(matchResults);

            //then remove dupes for the same program/campus but different product/client (also ranked in order to pick best one)
            switch (sortMethod)
            {
                case EntitySortMethod.SAB:
                    matchResults = RemoveDuplicates(matchResults, MatchItemSortType.SAB, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId });
                    break;
                case EntitySortMethod.RankScore:
                    matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId });
                    break;
                case EntitySortMethod.Alphabetical:
                    matchResults = RemoveDuplicates(matchResults, MatchItemSortType.FailedValidationPaidStatusType, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId });
                    matchResults = matchResults.OrderBy(mi => mi.Match.ProgramName).ToList();
                    break;
                default:
                    break;
            }

            Dictionary<int, List<string>> programGroupList = null;

            if (includeProgramGroupRollup.HasValue && includeProgramGroupRollup.Value)
                programGroupList = RollupProgramDisplayGroups(matchResults);

            RemoveDuplicateOnlineCampus(matchResults);

            List<Program> programList = new List<Program>();
            int inAreaPaidCount = 0;

            foreach (var programItem in matchResults)
            {
                Program program;

                if (includeProgramDetails.HasValue && includeProgramDetails.Value)
                {
                    program = MatchItemMapper.MapToProgramWithInstitutionCampus(programItem, programGroupList, clickPrograms, includeImages);
                }
                else
                {
                    program = MatchItemMapper.MapToProgram(programItem, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School);
                }

                programList.Add(program);

                if (programItem.IsGeotargeted && programItem.Match.PaidStatusTypeId == PaidStatusType.Paid)
                    inAreaPaidCount++;
            }

            programResponse.ResultCount = programList.Count();
            programList = PaginateResults(programList, maxResultsCount, pageNumber);
            programResponse.ProgramList = programList;
            programResponse.PaidInAreaProgramCount = inAreaPaidCount;

            return programResponse;
        }

        public static void RemoveDuplicateOnlineCampus(List<MatchItem> matchResults)
        {
            var grouping = matchResults.Where(mi => mi.Match.CampusCampusTypeId == (int)CampusType.Online).DistinctBy(mi => new { mi.Match.InstitutionId, mi.Match.CampusId }).GroupBy(mi => mi.Match.InstitutionId);

            List<int> campusIdsToRemove = new List<int>();

            foreach (var group in grouping)
                if (group.Count() > 1)
                    campusIdsToRemove.AddRange(group.Skip(1).Select(mi => mi.Match.CampusId));

            if (campusIdsToRemove.Any())
                matchResults.RemoveAll(mi => campusIdsToRemove.Contains(mi.Match.CampusId));
        }

        public static CampusResponse CreateCampusResponse(MatchResult matchResult, EntitySortMethod sortMethod, int? maxNestedProgramCount, int? maxResultsCount, int? pageNumber, bool? includeProgramGroupRollup, int applicationId, bool isValidGeoTarget, bool includeImages)
        {
            CampusResponse campusResponse = new CampusResponse();
            campusResponse.MatchResponseGuid = Guid.NewGuid();

            //first remove duplicate program product ids (due to including subject/category results)
            List<MatchItem> matchResults = RemoveDuplicates(matchResult.MatchItemList, MatchItemSortType.None, distinct => distinct.Match.ProgramProductId);

            ExecuteApplicationRules(ref matchResults, applicationId);

            //need to get click programs from the set before removing duplicate programs
            Dictionary<int, MatchItem> clickPrograms = GetAllClickProgramsInSet(matchResults);

            //then remove dupes for the same program/campus but different product/client (also ranked in order to pick best one)
            switch (sortMethod)
            {
                case EntitySortMethod.RankScore:
                    matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId });
                    break;
                case EntitySortMethod.Alphabetical:
                    matchResults = RemoveDuplicates(matchResults, MatchItemSortType.FailedValidationPaidStatusType, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId }); //sort by failed validation so that if any programs in the list are not failed, they show up first
                    break;
                default:
                    break;
            }

            Dictionary<int, List<string>> programGroupList = null;

            if (includeProgramGroupRollup.HasValue && includeProgramGroupRollup.Value)
                programGroupList = RollupProgramDisplayGroups(matchResults);

            RemoveDuplicateOnlineCampus(matchResults);

            List<CampusWithInstitution> campusList = new List<CampusWithInstitution>();

            var groupedByCampus = matchResults.GroupBy(g => g.Match.CampusId);

            Dictionary<int, short> institutionCampusCount = new Dictionary<int, short>();
            short nbrGroundCampusAllowed = 2;

            foreach (var campusItem in groupedByCampus)
            {
                CampusWithInstitution campus = new CampusWithInstitution();
                MatchItem firstCampus = campusItem.First();

                if (applicationId == 7 && !isValidGeoTarget)
                {
                    short count = 0;

                    if (institutionCampusCount.TryGetValue(firstCampus.Match.InstitutionId, out count))
                    {
                        if ((CampusType)firstCampus.Match.CampusCampusTypeId == CampusType.Ground)
                        {
                            if (count >= nbrGroundCampusAllowed)
                                continue;
                            else
                                institutionCampusCount[firstCampus.Match.InstitutionId] = count++;
                        }
                    }
                    else
                    {
                        if ((CampusType)firstCampus.Match.CampusCampusTypeId == CampusType.Ground)
                            institutionCampusCount.Add(firstCampus.Match.InstitutionId, 1);
                    }
                }

                campus.CampusId = firstCampus.Match.CampusId;
                campus.CampusType = (CampusType)firstCampus.Match.CampusCampusTypeId;
                campus.CampusName = firstCampus.Match.CampusName;
                campus.Address1 = firstCampus.Match.CampusAddress1;
                campus.Address2 = firstCampus.Match.CampusAddress2;
                campus.City = firstCampus.Match.CampusCity;
                campus.State = firstCampus.Match.CampusStateCode;
                campus.PostalCode = firstCampus.Match.CampusPostalCode;
                campus.CountryCode = firstCampus.Match.CampusCountry;
                campus.CountryName = firstCampus.Match.CampusCountryName;
                campus.Phone = firstCampus.Match.CampusPhone;
                campus.Fax = firstCampus.Match.CampusFax;
                campus.ProgramCount = campusItem.Count();
                campus.Is2USchool = ConfigurationManager.AppSettings["Enable2UTCPAChange"] == "true" && firstCampus.Match.ClientId == 349 ? true : false;
                campus.InstitutionId = firstCampus.Match.InstitutionId;
                campus.InstitutionName = firstCampus.Match.InstitutionName;
                //campus.HasCampusLogo = firstCampus.Match.HasCampusLogo;
                campus.CampusLogoURL = firstCampus.Match.CampusLogoURL;
                campus.InstitutionDescription = firstCampus.Match.InstitutionDescription;
                campus.InstitutionDescriptionInternational = firstCampus.Match.InstitutionDescriptionInternational;
                campus.IsGeotargeted = firstCampus.IsGeotargeted;
                campus.InstitutionLogoURL = firstCampus.Match.InstitutionLogoURL;

                if (includeImages)
                    campus.ImageList = firstCampus.Match.CampusImageList;

                campus.ProgramList = new List<Program>();

                if (!maxNestedProgramCount.HasValue)
                {
                    foreach (var programItem in campusItem)
                    {
                        var program = MatchItemMapper.MapToProgram(programItem, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School);

                        campus.ProgramList.Add(program);
                    }
                }
                else if (maxNestedProgramCount.HasValue && maxNestedProgramCount > 1)
                {
                    int programsAdded = 0;

                    foreach (var programItem in campusItem)
                    {
                        if (programsAdded >= maxNestedProgramCount)
                            break;
                        else
                        {
                            var program = MatchItemMapper.MapToProgram(programItem, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School);

                            campus.ProgramList.Add(program);

                            programsAdded++;
                        }
                    }
                }
                else
                {
                    var program = MatchItemMapper.MapToProgram(firstCampus, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School);

                    campus.ProgramList.Add(program);
                }

                bool failedValidation;

                decimal rankScore = CalculateRolledupRankScore(campus.ProgramList, matchResult.ChosenBusinessModel, out failedValidation);

                campus.FailedValidation = failedValidation;
                campus.ProgramRankScore = rankScore;

                campusList.Add(campus);
            }

            switch (sortMethod)
            {
                case EntitySortMethod.RankScore:
                    if (isValidGeoTarget && applicationId == 7)
                        campusList = campusList.OrderByDescending(mi => mi.IsGeotargeted).ThenBy(a => a.FailedValidation).ThenByDescending(c => c.ProgramList[0].PaidStatusTypeId).ThenByDescending(cl => cl.ProgramRankScore).ToList();
                    else
                        campusList = campusList.OrderBy(a => a.FailedValidation).ThenByDescending(c => c.ProgramList[0].PaidStatusTypeId).ThenByDescending(cl => cl.ProgramRankScore).ToList();
                    break;
                case EntitySortMethod.Alphabetical:
                    campusList = campusList.OrderBy(cl => cl.CampusName).ToList();
                    break;
                default:
                    break;
            }

            List<int> campusesToRemove = RulesEngine.ExecuteCampusZoneRule(campusList.Select(f => new Tuple<int, int, string, decimal>(f.InstitutionId.Value, f.CampusId, f.CampusName, f.ProgramRankScore)).ToList());

            campusList = campusList.Where(cl => !campusesToRemove.Contains(cl.CampusId)).ToList();

            campusResponse.ResultCount = campusList.Count();
            campusResponse.ProgramResultCount = campusList.Sum(pc => pc.ProgramCount);
            campusList = PaginateResults(campusList, maxResultsCount, pageNumber);

            campusResponse.CampusList = campusList;

            return campusResponse;
        }

        public static InstitutionResponse CreateInstitutionResponseSAB(MatchResult matchResult, EntitySortMethod sortMethod, int? maxNestedProgramCount, int? maxResultsCount, int? pageNumber, GetInstitutionCampusOption campusOption, bool? includeProgramGroupRollup, int applicationId, bool isValidGeoTarget, bool includeImages, Campaign campaign, MatchingEngine engine, EDDY.IS.Core.Logging.PerformanceLog pLog)
        {
            InstitutionResponse instResponse = new InstitutionResponse();
            instResponse.MatchResponseGuid = Guid.NewGuid();
            //first remove duplicate program product ids (due to including subject/category results)

            pLog.StartLogDetail("RemoveDuplicate");
            List<MatchItem> matchResults = RemoveDuplicates(matchResult.MatchItemList, MatchItemSortType.None, distinct => distinct.Match.ProgramProductId);
            pLog.EndLogDetail();

            pLog.StartLogDetail("ExecuteApplicationRules");
            ExecuteApplicationRules(ref matchResults, applicationId);
            pLog.EndLogDetail();

            //need to get click programs from the set before removing duplicate programs
            pLog.StartLogDetail("GetAllClickProgramsInSet");
            Dictionary<int, MatchItem> clickPrograms = GetAllClickProgramsInSet(matchResults);
            pLog.EndLogDetail();

            //then remove dupes for the same program/campus but different product/client (also ranked in order to pick best one)
            pLog.StartLogDetail("SortMatchItems");
            switch (sortMethod)
            {
                case EntitySortMethod.RankScore:
                    matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId });
                    break;
                case EntitySortMethod.Alphabetical:
                    matchResults = RemoveDuplicates(matchResults, MatchItemSortType.FailedValidationPaidStatusType, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId }); //sort by failed validation so that if any programs in the list are not failed, they show up first
                    break;
                default:
                    break;
            }
            pLog.EndLogDetail();

            pLog.StartLogDetail("RollupProgramDisplayGroups");
            Dictionary<int, List<string>> programGroupList = null;

            if (includeProgramGroupRollup.HasValue && includeProgramGroupRollup.Value)
                programGroupList = RollupProgramDisplayGroups(matchResults);
            pLog.EndLogDetail();

            pLog.StartLogDetail("AggregateResults");
            List<Institution> instList = new List<Institution>();

            var groupedBySABTier = matchResults.GroupBy(g => g.Match.SFProductCode_PSI);

            foreach (var paidStatusItem in groupedBySABTier)
            {
                var groupedByInst = paidStatusItem.GroupBy(g => g.Match.InstitutionId);

                foreach (var instItem in groupedByInst)
                {
                    MatchItem firstInstitution = instItem.First();

                    InstitutionWithProgram institution = new InstitutionWithProgram();

                    List<MatchItem> programList = instItem.DistinctBy(i => i.Match.ProgramId).ToList();

                    institution.InstitutionId = firstInstitution.Match.InstitutionId;
                    institution.InstitutionName = firstInstitution.Match.InstitutionName;
                    institution.InstitutionDescription = firstInstitution.Match.InstitutionDescription;
                    institution.InstitutionDescriptionInternational = firstInstitution.Match.InstitutionDescriptionInternational;
                    institution.InstitutionDisclaimerType = firstInstitution.Match.InstitutionDisclaimerType;
                    institution.InstitutionDisclaimer = firstInstitution.Match.InstitutionDisclaimer;
                    institution.IsGeotargeted = firstInstitution.IsGeotargeted;
                    institution.SABSRAPosition_CR = firstInstitution.Match.SABSRAPosition_CR;
                    institution.ProgramCount = programList.Count;
                    institution.ProgramList = new List<Program>();
                    institution.InstitutionLogoURL = firstInstitution.Match.InstitutionLogoURL;
                    institution.StartDateList = firstInstitution.Match.InstitutionStartDateList;
                    institution.IsNationalOnlineAdvertiser = firstInstitution.IsNationalOnlineAdvertiser;

                    if (includeImages)
                        institution.ImageList = firstInstitution.Match.InstitutionImageList;

                    if (firstInstitution.Match.SFProductCode_CR.HasValue)
                    {
                        institution.InstitutionSFProductCodeList = new List<SFProductCode>();
                        institution.InstitutionSFProductCodeList.Add(firstInstitution.Match.SFProductCode_CR.Value);
                    }

                    if (campaign.AllowRevShareRPL)
                    {
                        var estimatedRevShare = campaign.CalculateRevShareByERPL
                            ? firstInstitution.Match.eRPL
                            : firstInstitution.Match.RPL;
                        estimatedRevShare = firstInstitution.ScoreId.HasValue && firstInstitution.ScoreId > 0
                            ? engine.GetLeadPingLeadScoreCPL(firstInstitution.Match.ProductId, firstInstitution.Match.InstitutionId, firstInstitution.Match.CampusId, firstInstitution.ScoreId.Value)
                            : estimatedRevShare;

                        institution.EstimatedRevShare = campaign.RevenueSharePercentage.HasValue
                               ? (estimatedRevShare * campaign.RevenueSharePercentage) / 100
                               : estimatedRevShare;
                    }

                    int maxProgramsToDisplay = programList.Count;
                    int programsAdded = 0;

                    if (maxNestedProgramCount.HasValue)
                        maxProgramsToDisplay = maxNestedProgramCount.Value;

                    foreach (var programItem in programList)
                    {
                        if (programsAdded >= maxProgramsToDisplay)
                            break;

                        var program = MatchItemMapper.MapToProgram(programItem, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School);

                        institution.ProgramList.Add(program);

                        programsAdded++;
                    }

                    //TFS100414 - ME Update Listings for State Pages - if all the programs for the institution are inquiry disabled then set inquiry disabled at institution level to true 
                    institution.InquiryDisabled = programList.All(p => p.Match.InquiryDisabled);

                    bool failedValidation;

                    decimal rankScore = CalculateRolledupRankScore(institution.ProgramList, matchResult.ChosenBusinessModel, out failedValidation);

                    institution.FailedValidation = failedValidation;

                    institution.ProgramRankScore = rankScore;

                    instList.Add(institution);
                }
            }

            pLog.EndLogDetail();

            pLog.StartLogDetail("Resort");
            switch (sortMethod)
            {
                case EntitySortMethod.RankScore:
                    instList.Sort(new InstitutionComparerSAB());
                    break;
                case EntitySortMethod.Alphabetical:
                    instList = instList.OrderBy(cl => cl.InstitutionName).ToList();
                    break;
                default:
                    break;
            }
            pLog.EndLogDetail();

            pLog.StartLogDetail("PaginateResults");
            instResponse.ResultCount = instList.Count();
            instList = PaginateResults(instList, maxResultsCount, pageNumber);
            instResponse.ProgramResultCount = instList.Sum(pc => pc.ProgramCount);
            pLog.EndLogDetail();

            instResponse.InstitutionList = instList;

            return instResponse;
        }

        public static AdServerClientRelationshipResponse CreateAdServerClientResponse(MatchResult matchResult, EntitySortMethod sortMethod, AdServerMatchRequest matchRequest)
        {
            AdServerClientRelationshipResponse adServerResponse = new AdServerClientRelationshipResponse();
            adServerResponse.MatchResponseGuid = Guid.NewGuid();
            adServerResponse.CRList = new List<AdServerClientRelationship>();

            List<MatchItem> matchResults = RemoveDuplicates(matchResult.MatchItemList, MatchItemSortType.RankScore, distinct => distinct.Match.ProgramProductId);
            ExecuteApplicationRules(ref matchResults, matchRequest.ApplicationId);

            var groupedByCR = matchResults.GroupBy(g => g.Match.ClientRelationshipId);

            HashSet<int> crFilterList = null;

            if (matchRequest.CRList != null) crFilterList = new HashSet<int>(matchRequest.CRList);

            foreach (var cr in groupedByCR)
            {
                var crMatch = cr.First();

                if (crFilterList != null && (crFilterList.Count == 0 || crFilterList.Contains(crMatch.Match.ClientRelationshipId)))
                {
                    var groupedByPSI = cr.GroupBy(g => g.Match.PsiId);

                    AdServerClientRelationship adCr = new AdServerClientRelationship()
                    {
                        ClientRelationshipId = crMatch.Match.ClientRelationshipId,
                        InstitutionId = crMatch.Match.InstitutionId,
                        InstitutionName = crMatch.Match.InstitutionName,
                        PsiList = new List<AdServerPsi>(),
                        InstitutionLogoURL = crMatch.Match.InstitutionLogoURL
                    };

                    foreach (var psi in groupedByPSI)
                    {
                        AdServerPsi p = new AdServerPsi();
                        p.PsiId = psi.First().Match.PsiId;

                        foreach (var pp in psi)
                        {
                            if (pp.Match.HasClick)
                            {
                                p.ClickProgramProductList.Add(new AdServerClickProgramProduct()
                                {
                                    ClickPrice = pp.Match.ClickPrice,
                                    ClickThroughURL = pp.Match.ClickThroughUrl,
                                    ProgramProductId = pp.Match.ProgramProductId,
                                    ProgramId = pp.Match.ProgramId
                                });
                            }
                        }
                        adCr.PsiList.Add(p);
                    }

                    adServerResponse.CRList.Add(adCr);
                }
            }

            adServerResponse.CRList = PaginateResults(adServerResponse.CRList, matchRequest.MaxResultsCount, matchRequest.PageNumber);

            return adServerResponse;
        }

        //(matchResult, sortMethod, maxNestedProgramCount, maxResultsCount, pageNumber, includeCampusWithInstitution)
        public static InstitutionResponse CreateInstitutionResponse(MatchResult matchResult, EntitySortMethod sortMethod, int? maxNestedProgramCount, int? maxResultsCount, int? pageNumber, GetInstitutionCampusOption campusOption, bool? includeProgramGroupRollup, int applicationId, bool isValidGeoTarget, bool includeImages, Campaign campaign, MatchingEngine engine, EDDY.IS.Core.Logging.PerformanceLog pLog)
        {
            InstitutionResponse instResponse = new InstitutionResponse();
            instResponse.MatchResponseGuid = Guid.NewGuid();
            //first remove duplicate program product ids (due to including subject/category results)

            pLog.StartLogDetail("RemoveDuplicate");
            List<MatchItem> matchResults = RemoveDuplicates(matchResult.MatchItemList, MatchItemSortType.None, distinct => distinct.Match.ProgramProductId);
            pLog.EndLogDetail();

            pLog.StartLogDetail("ExecuteApplicationRules");
            ExecuteApplicationRules(ref matchResults, applicationId);
            pLog.EndLogDetail();

            //need to get click programs from the set before removing duplicate programs
            pLog.StartLogDetail("GetAllClickProgramsInSet");
            Dictionary<int, MatchItem> clickPrograms = GetAllClickProgramsInSet(matchResults);
            pLog.EndLogDetail();

            //then remove dupes for the same program/campus but different product/client (also ranked in order to pick best one)
            pLog.StartLogDetail("SortMatchItems");
            switch (sortMethod)
            {
                case EntitySortMethod.RankScore:
                    matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId });
                    break;
                case EntitySortMethod.Alphabetical:
                    matchResults = RemoveDuplicates(matchResults, MatchItemSortType.FailedValidationPaidStatusType, distinct => new { distinct.Match.ProgramId, distinct.Match.CampusId }); //sort by failed validation so that if any programs in the list are not failed, they show up first
                    break;
                default:
                    break;
            }
            pLog.EndLogDetail();

            pLog.StartLogDetail("RollupProgramDisplayGroups");
            Dictionary<int, List<string>> programGroupList = null;

            if (includeProgramGroupRollup.HasValue && includeProgramGroupRollup.Value)
                programGroupList = RollupProgramDisplayGroups(matchResults);
            pLog.EndLogDetail();

            pLog.StartLogDetail("RemoveDuplicateOnlineCampus");
            RemoveDuplicateOnlineCampus(matchResults);
            pLog.EndLogDetail();

            pLog.StartLogDetail("AggregateResults");
            List<Institution> instList = new List<Institution>();

            var groupedByPaidStatusType = matchResults.GroupBy(g => g.Match.PaidStatusTypeId);

            foreach (var paidStatusItem in groupedByPaidStatusType)
            {
                var groupedByInst = paidStatusItem.GroupBy(g => g.Match.InstitutionId);

                if (campusOption == GetInstitutionCampusOption.CampusOn2ndLevel)
                {
                    foreach (var instItem in groupedByInst)
                    {
                        MatchItem firstInstitution = instItem.First();

                        InstitutionWithCampus institution = new InstitutionWithCampus();

                        institution.InstitutionId = firstInstitution.Match.InstitutionId;
                        institution.InstitutionName = firstInstitution.Match.InstitutionName;
                        institution.InstitutionDescription = firstInstitution.Match.InstitutionDescription;
                        institution.InstitutionDescriptionInternational = firstInstitution.Match.InstitutionDescriptionInternational;
                        institution.InstitutionDisclaimerType = firstInstitution.Match.InstitutionDisclaimerType;
                        institution.InstitutionDisclaimer = firstInstitution.Match.InstitutionDisclaimer;
                        institution.IsGeotargeted = firstInstitution.IsGeotargeted;
                        institution.ProgramCount = instItem.Count();
                        institution.CampusList = new List<Campus>();
                        institution.InstitutionLogoURL = firstInstitution.Match.InstitutionLogoURL;
                        institution.StartDateList = firstInstitution.Match.InstitutionStartDateList;
                        institution.IsNationalOnlineAdvertiser = firstInstitution.IsNationalOnlineAdvertiser;

                        if (includeImages)
                            institution.ImageList = firstInstitution.Match.InstitutionImageList;

                        if (firstInstitution.Match.SFProductCode_CR.HasValue)
                        {
                            institution.InstitutionSFProductCodeList = new List<SFProductCode>();
                            institution.InstitutionSFProductCodeList.Add(firstInstitution.Match.SFProductCode_CR.Value);
                        }

                        if (campaign.AllowRevShareRPL) {
                            var estimatedRevShare = campaign.CalculateRevShareByERPL
                                ? firstInstitution.Match.eRPL
                                : firstInstitution.Match.RPL;
                            estimatedRevShare = firstInstitution.ScoreId.HasValue && firstInstitution.ScoreId > 0
                                ? engine.GetLeadPingLeadScoreCPL(firstInstitution.Match.ProductId, firstInstitution.Match.InstitutionId, firstInstitution.Match.CampusId, firstInstitution.ScoreId.Value)
                                : estimatedRevShare;

                            institution.EstimatedRevShare = campaign.RevenueSharePercentage.HasValue
                                   ? (estimatedRevShare * campaign.RevenueSharePercentage) / 100
                                   : estimatedRevShare;
                        }

                        institution.AdvertiserId = firstInstitution.Match.AdvertiserId;
                        institution.CRId = firstInstitution.Match.ClientRelationshipId;

                        List<Program> fullProgramList = new List<Program>();

                        var groupedByCampus = instItem.GroupBy(i => i.Match.CampusId);

                        foreach (var campusItem in groupedByCampus)
                        {
                            MatchItem firstCampus = campusItem.First();

                            Campus campus = new Campus();

                            campus.CampusId = firstCampus.Match.CampusId;
                            campus.CampusType = (CampusType)firstCampus.Match.CampusCampusTypeId;
                            campus.CampusName = firstCampus.Match.CampusName;
                            campus.Address1 = firstCampus.Match.CampusAddress1;
                            campus.Address2 = firstCampus.Match.CampusAddress2;
                            campus.City = firstCampus.Match.CampusCity;
                            campus.State = firstCampus.Match.CampusStateCode;
                            campus.PostalCode = firstCampus.Match.CampusPostalCode;
                            campus.CountryCode = firstCampus.Match.CampusCountry;
                            campus.CountryName = firstCampus.Match.CampusCountryName;
                            campus.Fax = firstCampus.Match.CampusFax;
                            campus.Phone = firstCampus.Match.CampusPhone;
                            //campus.HasCampusLogo = firstCampus.Match.HasCampusLogo;
                            campus.CampusLogoURL = firstCampus.Match.CampusLogoURL;
                            campus.InstitutionLogoURL = firstCampus.Match.InstitutionLogoURL;

                            if (includeImages)
                                campus.ImageList = firstCampus.Match.CampusImageList;

                            campus.ProgramCount = campusItem.Count();
                            campus.ProgramList = new List<Program>();

                            if (!maxNestedProgramCount.HasValue)
                            {
                                foreach (var programItem in campusItem)
                                {
                                    var program = MatchItemMapper.MapToProgram(programItem, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School);

                                    campus.ProgramList.Add(program);
                                    fullProgramList.Add(program);
                                }
                            }
                            else if (maxNestedProgramCount.HasValue && maxNestedProgramCount > 1)
                            {
                                int programsAdded = 0;

                                foreach (var programItem in campusItem)
                                {
                                    if (programsAdded >= maxNestedProgramCount)
                                        break;

                                    var program = MatchItemMapper.MapToProgram(programItem, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School);

                                    campus.ProgramList.Add(program);
                                    fullProgramList.Add(program);

                                    programsAdded++;
                                }
                            }
                            else
                            {
                                var program = MatchItemMapper.MapToProgram(firstCampus, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School);

                                campus.ProgramList.Add(program);
                                fullProgramList.Add(program);
                            }

                            bool campusFailedValidation;

                            decimal campusRankScore = CalculateRolledupRankScore(campus.ProgramList, matchResult.ChosenBusinessModel, out campusFailedValidation);

                            campus.FailedValidation = campusFailedValidation;
                            campus.ProgramRankScore = campusRankScore;

                            institution.CampusList.Add(campus);

                            List<int> campusesToRemove = RulesEngine.ExecuteCampusZoneRule(institution.CampusList.Select(f => new Tuple<int, int, string, decimal>(institution.InstitutionId, f.CampusId, f.CampusName, f.ProgramRankScore)).ToList());

                            institution.CampusList = institution.CampusList.Where(cl => !campusesToRemove.Contains(cl.CampusId)).ToList();
                        }

                        //TFS100414 - ME Update Listings for State Pages - if all the programs for the institution are inquiry disabled then set inquiry disabled at institution level to true 
                        institution.InquiryDisabled = instItem.DistinctBy(i => i.Match.ProgramId).All(p => p.Match.InquiryDisabled);

                        bool instFailedValidation;

                        decimal instRankScore = CalculateRolledupRankScore(fullProgramList, matchResult.ChosenBusinessModel, out instFailedValidation);

                        institution.FailedValidation = instFailedValidation;
                        institution.ProgramRankScore = instRankScore;

                        instList.Add(institution);
                    }
                }
                else
                {
                    foreach (var instItem in groupedByInst)
                    {
                        MatchItem firstInstitution = instItem.First();

                        InstitutionWithProgram institution = new InstitutionWithProgram();

                        institution.InstitutionId = firstInstitution.Match.InstitutionId;
                        institution.InstitutionName = firstInstitution.Match.InstitutionName;
                        institution.InstitutionDescription = firstInstitution.Match.InstitutionDescription;
                        institution.InstitutionDescriptionInternational = firstInstitution.Match.InstitutionDescriptionInternational;
                        institution.InstitutionDisclaimerType = firstInstitution.Match.InstitutionDisclaimerType;
                        institution.InstitutionDisclaimer = firstInstitution.Match.InstitutionDisclaimer;
                        institution.IsGeotargeted = firstInstitution.IsGeotargeted;
                        institution.ProgramCount = instItem.DistinctBy(i => i.Match.ProgramId).Count();
                        institution.ProgramList = new List<Program>();
                        institution.InstitutionLogoURL = firstInstitution.Match.InstitutionLogoURL;
                        institution.StartDateList = firstInstitution.Match.InstitutionStartDateList;
                        if (includeImages)
                            institution.ImageList = firstInstitution.Match.InstitutionImageList;

                        if (firstInstitution.Match.SFProductCode_CR.HasValue)
                        {
                            institution.InstitutionSFProductCodeList = new List<SFProductCode>();
                            institution.InstitutionSFProductCodeList.Add(firstInstitution.Match.SFProductCode_CR.Value);
                        }

                        institution.AdvertiserId = firstInstitution.Match.AdvertiserId;
                        institution.CRId = firstInstitution.Match.ClientRelationshipId;

                        if (!maxNestedProgramCount.HasValue)
                        {
                            foreach (var programItem in instItem.DistinctBy(i => i.Match.ProgramId))
                            {
                                List<CampusSlim> campusSlimList = null;

                                if (campusOption == GetInstitutionCampusOption.CampusOn3rdLevel)
                                    campusSlimList = GenerateCampusSlimList(instItem.Where(i => i.Match.ProgramId == programItem.Match.ProgramId));

                                var program = MatchItemMapper.MapToProgram(programItem, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School, campusSlimList);

                                institution.ProgramList.Add(program);
                            }
                        }
                        else if (maxNestedProgramCount.HasValue && maxNestedProgramCount > 1)
                        {
                            int programsAdded = 0;

                            foreach (var programItem in instItem.DistinctBy(i => i.Match.ProgramId))
                            {
                                if (programsAdded >= maxNestedProgramCount)
                                    break;

                                List<CampusSlim> campusSlimList = null;

                                if (campusOption == GetInstitutionCampusOption.CampusOn3rdLevel)
                                    campusSlimList = GenerateCampusSlimList(instItem.Where(i => i.Match.ProgramId == programItem.Match.ProgramId));

                                var program = MatchItemMapper.MapToProgram(programItem, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School, campusSlimList);

                                institution.ProgramList.Add(program);

                                programsAdded++;
                            }
                        }
                        else
                        {

                            List<CampusSlim> campusSlimList = null;

                            if (campusOption == GetInstitutionCampusOption.CampusOn3rdLevel)
                                campusSlimList = GenerateCampusSlimList(instItem.Where(i => i.Match.ProgramId == instItem.First().Match.ProgramId));

                            var program = MatchItemMapper.MapToProgram(firstInstitution, programGroupList, clickPrograms, includeImages, Base.InstitutionLeadTypes.School, campusSlimList);

                            institution.ProgramList.Add(program);
                        }

                        //TFS100414 - ME Update Listings for State Pages - if all the programs for the institution are inquiry disabled then set inquiry disabled at institution level to true 
                        institution.InquiryDisabled = instItem.DistinctBy(i => i.Match.ProgramId).All(p => p.Match.InquiryDisabled);

                        bool failedValidation;

                        decimal rankScore = CalculateRolledupRankScore(institution.ProgramList, matchResult.ChosenBusinessModel, out failedValidation);

                        institution.FailedValidation = failedValidation;

                        institution.ProgramRankScore = rankScore;

                        instList.Add(institution);
                    }
                }
            }
            pLog.EndLogDetail();

            pLog.StartLogDetail("Resort");
            switch (sortMethod)
            {
                case EntitySortMethod.RankScore:
                    if (applicationId == 7)
                        instList.Sort(new InstitutionComparerGS(campusOption, isValidGeoTarget, applicationId));
                    else
                        instList.Sort(new InstitutionComparerStandard(campusOption, isValidGeoTarget, applicationId));
                    break;
                case EntitySortMethod.Alphabetical:
                    instList = instList.OrderBy(cl => cl.InstitutionName).ToList();
                    break;
                default:
                    break;
            }
            pLog.EndLogDetail();

            pLog.StartLogDetail("PaginateResults");
            instResponse.ResultCount = instList.Count();
            instList = PaginateResults(instList, maxResultsCount, pageNumber);
            instResponse.ProgramResultCount = instList.Sum(pc => pc.ProgramCount);
            pLog.EndLogDetail();

            instResponse.InstitutionList = instList;

            return instResponse;
        }

        private static List<CampusSlim> GenerateCampusSlimList(IEnumerable<MatchItem> programs)
        {
            List<CampusSlim> campusSlimList = new List<CampusSlim>();

            List<MatchItem> campusList = programs.DistinctBy(p => p.Match.CampusId).ToList();

            foreach (var campus in campusList)
            {
                CampusSlim cs = new CampusSlim();

                cs.CampusId = campus.Match.CampusId;
                cs.CampusType = (CampusType)campus.Match.CampusCampusTypeId;
                cs.City = campus.Match.CampusCity;
                cs.State = campus.Match.CampusStateCode;
                cs.CountryCode = campus.Match.CampusCountry;
                cs.CountryName = campus.Match.CampusCountryName;

                campusSlimList.Add(cs);
            }

            return campusSlimList;
        }

        public static ApolloCampusResponse CreateApolloCampusResponse(MatchResult matchResult, ApolloCampusRequest request, PerformanceLog pLog)
        {
            ApolloCampusResponse campusResponse = new ApolloCampusResponse();
            campusResponse.MatchResponseGuid = Guid.NewGuid();
            //TemplateCacheItem templateCache = StaticCacheProxyHost.CacheProxy.Get<TemplateCacheItem>(MatchingCacheItem.REProgramProductTemplateAssignment);
            HashSet<int> subjects = new HashSet<int>();
            GeoCodeProcessor gcp = new GeoCodeProcessor();
            InstitutionGroupCache groupCache = StaticCacheProxyHost.CacheProxy.Get<InstitutionGroupCache>(MatchingCacheItem.InstitutionGroup);

            matchResult.MatchItemList = matchResult.MatchItemList.Where(m => m.Match.PaidStatusTypeId == PaidStatusType.Paid && m.Match.InquiryDisabled == false).ToList();

            List<ProgramWithInstitutionCampus> smpList = matchResult.MatchItemList.DistinctBy(mi => mi.Match.InstitutionId).Select(m => new ProgramWithInstitutionCampus() { InstitutionName = m.Match.InstitutionName, InstitutionId = m.Match.InstitutionId }).ToList();

            ThirdPartyMatchProcessor processor = new ThirdPartyMatchProcessor(request, pLog, smpList);

            List<MatchItem> thirdPartyMatches = processor.GetMatchItems(LeadCreationType.Advising);

            if (thirdPartyMatches != null)
                matchResult.MatchItemList.AddRange(thirdPartyMatches);

            foreach (ProductGroupRequest req in request.ProductGroupList)
            {
                ApolloProductGroupResponse productGroupResponse = new ApolloProductGroupResponse();

                //clone matchResults
                List<MatchItem> matchResults = new List<MatchItem>();

                //filter out products not in request group
                foreach (var mi in matchResult.MatchItemList)
                {
                    if (req.Products.Contains(mi.Match.ProductId))
                        matchResults.Add(mi);
                }

                //then remove dupes for the same program/campus but different product/client (also ranked in order to pick best one)
                matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => new { distinct.Match.ProgramId, distinct.Match.ProgramName, distinct.Match.CampusId });

                List<ApolloCampus> onlineCampusList = new List<ApolloCampus>();
                List<ApolloCampus> groundCampusList = new List<ApolloCampus>();

                var groupedByInstitution = matchResults.GroupBy(g => new { g.Match.InstitutionId, g.Match.InstitutionName });

                foreach (var institutionItem in groupedByInstitution)
                {
                    var groupedByCampusType = institutionItem.GroupBy(ct => ct.Match.CampusCampusTypeId);

                    foreach (var campusTypeItem in groupedByCampusType)
                    {
                        var groupedByCampus = campusTypeItem.GroupBy(c => c.Match.CampusId);

                        #region Create ApolloCampus Object
                        ApolloCampus campus = new ApolloCampus();
                        var firstCampusInGroup = groupedByCampus.First();
                        MatchItem firstCampus = firstCampusInGroup.First();

                        if (req.ProductGroupIdentifier.StartsWith("WT"))
                        {
                            //Add Phone Number for WT Campuses
                            campus.PhoneNumber = firstCampus.Match.PhoneNumber;
                        }

                        //Ground
                        if (request.ProspectInput != null && firstCampus.Match.CampusCampusTypeId == 2)
                        {
                            campus.Distance = gcp.GetDistanceBetweenZipCodes(firstCampus.Match.CampusPostalCode, request.ProspectInput.PostalCode);
                        }

                        campus.CampusId = firstCampus.Match.CampusId;
                        campus.CampusType = (CampusType)firstCampus.Match.CampusCampusTypeId;
                        campus.CampusName = firstCampus.Match.CampusName;
                        campus.Address1 = firstCampus.Match.CampusAddress1;
                        campus.Address2 = firstCampus.Match.CampusAddress2;
                        campus.City = firstCampus.Match.CampusCity;
                        campus.State = firstCampus.Match.CampusStateCode;
                        campus.PostalCode = firstCampus.Match.CampusPostalCode;
                        campus.CountryCode = firstCampus.Match.CampusCountry;
                        campus.Phone = firstCampus.Match.CampusPhone;
                        campus.Fax = firstCampus.Match.CampusFax;
                        campus.ProgramCount = firstCampusInGroup.Count();
                        campus.InstitutionId = firstCampus.Match.InstitutionId;
                        campus.InstitutionName = firstCampus.Match.InstitutionName;
                        //campus.HasCampusLogo = firstCampus.Match.HasCampusLogo;
                        campus.CampusLogoURL = firstCampus.Match.CampusLogoURL;
                        campus.IsFaithBased = firstCampus.Match.IsFaithBased;
                        campus.HasAdditionalScript = firstCampus.Match.HasAdditionalScript;
                        campus.InstitutionLogoURL = firstCampus.Match.InstitutionLogoURL;
                        //campus.ex
                        campus.ProgramList = new List<Program>();

                        if (campus.InstitutionId.HasValue && groupCache.InstitutionToGroups.ContainsKey(campus.InstitutionId.Value))
                        {
                            campus.InstitutionGroupId = groupCache.InstitutionToGroups[campus.InstitutionId.Value][0].Item1;

                            if (groupCache.GroupsThatAllowMultipleSelections.Contains(campus.InstitutionGroupId.Value))
                                campus.AllowMultiSelectForInstitutionGroup = true;
                            else
                                campus.AllowMultiSelectForInstitutionGroup = false;
                        }

                        int programCount = 0;
                        foreach (var programItem in firstCampusInGroup)
                        {
                            Program program = new Program()
                            {
                                ProgramProductId = programItem.Match.ProgramProductId,
                                ProgramId = programItem.Match.ProgramId,
                                ProgramName = programItem.Match.ProgramName,
                                DegreeAcronym = programItem.Match.DegreeAcronym,
                                ProgramDescription = String.IsNullOrEmpty(programItem.Match.ProgramDescription) ? "" : "true",
                                ProgramLevelName = programItem.Match.ProgramLevelName,
                                ProgramType = (ProgramType)programItem.Match.ProgramTypeId,
                                ProgramLevelId = programItem.Match.ProgramLevelId,
                                SubjectId = programItem.Match.PrimarySubjectId,
                                CategoryId = programItem.Match.PrimaryCategoryId,
                                ProgramCampusType = (CampusType)programItem.Match.ProgramCampusTypeId,
                                //ProgramDisclaimerType = programItem.ProgramDisclaimerType,
                                //ProgramDisclaimer = programItem.ProgramDisclaimer,
                                ProgramRankScore = programItem.ProgramRankScore,
                                FailedValidation = programItem.FailedValidation,
                                ProductId = programItem.Match.ProductId,
                                PaidStatusTypeId = programItem.Match.PaidStatusTypeId,
                                TemplateId = programItem.Match.TemplateId,
                                SchoolLogoURL = programItem.Match.SchoolLogoURL,
                                ProgramLogoURL = programItem.Match.ProgramLogoURL,
                                CampusLogoURL = programItem.Match.CampusLogoURL,
                                InstitutionLogoURL = programItem.Match.InstitutionLogoURL,
                                ExternalMatchItemGuid = programItem.ExternalMatchItemGuid,
                                InstitutionType = programItem.InstitutionLeadType
                            };

                            //int templateId;

                            //if (templateCache.ProgramProductToTemplateAssignments.TryGetValue(programItem.Match.ProgramProductId, out templateId))
                            //    program.TemplateId = templateId;

                            if (request.MaxNestedProgramCount.HasValue && ++programCount <= request.MaxNestedProgramCount.Value)
                                campus.ProgramList.Add(program);
                            else
                                break;

                            if (programItem.Match.PrimarySubjectId != null)
                                subjects.Add(programItem.Match.PrimarySubjectId.Value);

                        }

                        #endregion

                        bool failedValidation;

                        decimal rankScore = CalculateRolledupRankScore(campus.ProgramList, matchResult.ChosenBusinessModel, out failedValidation);

                        campus.FailedValidation = failedValidation;


                        //if the lead is a edmc wt dupe then set the campus rank score to 0, goal is to put this campus at the bottom of the match list
                        if (firstCampus.Match.SetRankScoreZero || firstCampus.InstitutionLeadType == Base.InstitutionLeadTypes.ThirdPartyApiMatch) //external match ranks 0 as it should be at the end
                        {
                            campus.ProgramRankScore = 0;
                        }
                        else
                        {
                            campus.ProgramRankScore = rankScore;
                        }

                        campus.ProgramList = campus.ProgramList.OrderByDescending(pl => pl.ProgramName).ToList();

                        if (campus.CampusType == CampusType.Ground)
                            groundCampusList.Add(campus);
                        else
                            onlineCampusList.Add(campus);
                    }
                }

                //Institution Group Filtering for SMP only
                //Seperate Calls for Online and Ground
                if (req.ProductGroupIdentifier == "SMP")
                {
                    var groupedByInstGround = groundCampusList.GroupBy(gc => gc.InstitutionId.Value).ToList();
                    var groupedByInstOnline = onlineCampusList.GroupBy(gc => gc.InstitutionId.Value).ToList();

                    //if (groupedByInstGround != null && groupedByInstGround.Count() > 0)
                    //{

                    //    var instGroupRuleInput = groupedByInstGround.Select(f => new { f.First().InstitutionId.Value, f.First().ProgramRankScore }).ToDictionary(k => k.Value, v => v.ProgramRankScore);
                    //    List<int> institutionsToRemove = RulesEngine.ExecuteInstitutionGroupRule(instGroupRuleInput);
                    //    groundCampusList = groundCampusList.Where(pl => !institutionsToRemove.Contains(pl.InstitutionId.Value)).ToList();

                    //}

                    //if (groupedByInstOnline != null && groupedByInstOnline.Count() > 0)
                    //{

                    //    var instGroupRuleInput = groupedByInstOnline.Select(f => new { f.First().InstitutionId.Value, f.First().ProgramRankScore }).ToDictionary(k => k.Value, v => v.ProgramRankScore);
                    //    List<int> institutionsToRemove = RulesEngine.ExecuteInstitutionGroupRule(instGroupRuleInput);
                    //    onlineCampusList = onlineCampusList.Where(pl => !institutionsToRemove.Contains(pl.InstitutionId.Value)).ToList();

                    //}
                }

                //Re-sort WT results for Fixed Rank manipulation.
                if (req.ProductGroupIdentifier.StartsWith("WT"))
                {
                    onlineCampusList = onlineCampusList.OrderByDescending(cl => cl.ProgramRankScore).ToList();
                    groundCampusList = groundCampusList.OrderByDescending(cl => cl.ProgramRankScore).ToList();
                }

                groundCampusList = PaginateResults(groundCampusList, request.MaxCampusCountPerGrouping, 0);
                onlineCampusList = PaginateResults(onlineCampusList, request.MaxCampusCountPerGrouping, 0);

                productGroupResponse.GroundCampusList = groundCampusList;
                productGroupResponse.OnlineCampusList = onlineCampusList;
                productGroupResponse.groupIdentifier = req.ProductGroupIdentifier;

                campusResponse.ProductGroupResponseList.Add(productGroupResponse);

                if (!String.IsNullOrEmpty(request.SearchTerm))
                    campusResponse.SubjectsFromTextSearch.AddRange(subjects);
            }
            return campusResponse;
        }
        private static List<NeoCampus> CreateNeoCampusList(MatchResult matchResult, NeoMatchRequest request, bool isWT, ref HashSet<int> subjects)
        {
            GeoCodeProcessor gcp = new GeoCodeProcessor();
            InstitutionGroupCache groupCache = StaticCacheProxyHost.CacheProxy.Get<InstitutionGroupCache>(MatchingCacheItem.InstitutionGroup);

            List<NeoCampus> neoCampuses = new List<NeoCampus>();
            List<MatchItem> matchResults = new List<MatchItem>();

            //filter out products not in request group
            foreach (var mi in matchResult.MatchItemList)
            {
                if (isWT && Product.IsWarmTransferProduct(mi.Match.ProductId))
                    matchResults.Add(mi);
                else if (!isWT && !Product.IsWarmTransferProduct(mi.Match.ProductId))
                    matchResults.Add(mi);
            }

            //then remove dupes for the same program/campus but different product/client (also ranked in order to pick best one)
            matchResults = RemoveDuplicates(matchResults, MatchItemSortType.RankScore, distinct => new { distinct.Match.ProgramId, distinct.Match.ProgramName, distinct.Match.CampusId });

            var groupedByInstitution = matchResults.GroupBy(g => new { g.Match.InstitutionId, g.Match.InstitutionName });

            foreach (var institutionItem in groupedByInstitution)
            {
                //Group Matches by Canpus and Campus Type
                var groupedByCampusType = institutionItem.GroupBy(ct => new { ct.Match.CampusCampusTypeId, ct.Match.CampusId }) ;

                foreach (var campusTypeItem in groupedByCampusType)
                {
                    var groupedByCampus = campusTypeItem.GroupBy(c => c.Match.CampusId);

                    #region Create ApolloCampus Object
                    var firstCampusInGroup = groupedByCampus.First();
                    MatchItem firstCampus = firstCampusInGroup.First();
                    NeoCampus campus = MatchItemMapper.MapToNeoCampus(firstCampus);

                    //Ground
                    if (request.ProspectInput != null && firstCampus.Match.CampusCampusTypeId == 2)
                        campus.DistanceFromProspect = gcp.GetDistanceBetweenZipCodes(firstCampus.Match.CampusPostalCode, request.ProspectInput.PostalCode);

                    campus.ProgramCount = firstCampusInGroup.Count();
                    campus.ProgramList = new List<Program>();

                    if (campus.InstitutionId.HasValue && groupCache.InstitutionToGroups.ContainsKey(campus.InstitutionId.Value))
                    {
                        campus.InstitutionGroupId = groupCache.InstitutionToGroups[campus.InstitutionId.Value][0].Item1;

                        if (groupCache.GroupsThatAllowMultipleSelections.Contains(campus.InstitutionGroupId.Value))
                            campus.AllowMultiSelectForInstitutionGroup = true;
                        else
                            campus.AllowMultiSelectForInstitutionGroup = false;
                    }

                    int programCount = 0;
                    foreach (var programItem in firstCampusInGroup)
                    {
                        Program program = MatchItemMapper.MapToProgramNeo(programItem, isWT, request.ProspectInput);

                        if (request.MaxNestedProgramCount.HasValue && ++programCount <= request.MaxNestedProgramCount.Value)
                            campus.ProgramList.Add(program);
                        else
                            break;

                        if (programItem.Match.PrimarySubjectId != null)
                            subjects.Add(programItem.Match.PrimarySubjectId.Value);

                    }

                    #endregion
                    bool failedValidation;

                    decimal rankScore = CalculateRolledupRankScore(campus.ProgramList, matchResult.ChosenBusinessModel, out failedValidation);

                    campus.FailedValidation = failedValidation;

                    //if the lead is a edmc wt dupe then set the campus rank score to 0, goal is to put this campus at the bottom of the match list
                    if (firstCampus.Match.SetRankScoreZero || firstCampus.InstitutionLeadType == Base.InstitutionLeadTypes.ThirdPartyApiMatch) //external match ranks 0 as it should be at the end
                        campus.ProgramRankScore = 0;
                    else
                        campus.ProgramRankScore = rankScore;

                    campus.ProgramList = campus.ProgramList.OrderByDescending(pl => pl.ProgramName).ToList();

                    if (campus.CampusType == CampusType.Online || campus.DistanceFromProspect < 100)
                        neoCampuses.Add(campus);
                }
            }

            //Re-sort WT results for fixed ranks
            if(isWT)
                neoCampuses = neoCampuses.OrderByDescending(cl => cl.ProgramRankScore).ToList();

            return PaginateResults(neoCampuses, request.MaxCampusCountPerGrouping, 0);
        }

        public static NeoResponse CreateNeoResponse(MatchResult matchResult, NeoMatchRequest request, PerformanceLog pLog)
        {
            NeoResponse neoResponse = new NeoResponse();
            neoResponse.MatchResponseGuid = Guid.NewGuid();
            //TemplateCacheItem templateCache = StaticCacheProxyHost.CacheProxy.Get<TemplateCacheItem>(MatchingCacheItem.REProgramProductTemplateAssignment);
            HashSet<int> subjects = new HashSet<int>();
            
            matchResult.MatchItemList = matchResult.MatchItemList.Where(m => m.Match.PaidStatusTypeId == PaidStatusType.Paid && m.Match.InquiryDisabled == false).ToList();

            List<ProgramWithInstitutionCampus> formList = matchResult.MatchItemList.DistinctBy(mi => mi.Match.InstitutionId).Select(m => new ProgramWithInstitutionCampus() { InstitutionName = m.Match.InstitutionName, InstitutionId = m.Match.InstitutionId }).ToList();

            ThirdPartyMatchProcessor processor = new ThirdPartyMatchProcessor(request, pLog, formList);

            List<MatchItem> thirdPartyMatches = processor.GetMatchItems(LeadCreationType.Advising);

            if (thirdPartyMatches != null)
                matchResult.MatchItemList.AddRange(thirdPartyMatches);

            neoResponse.LiveTransferList = CreateNeoCampusList(matchResult, request, true, ref subjects);
            neoResponse.FormList = CreateNeoCampusList(matchResult, request, false, ref subjects);

            if (!String.IsNullOrEmpty(request.SearchTerm))
            {
                neoResponse.SubjectsFromTextSearch = new List<int>();
                neoResponse.SubjectsFromTextSearch.AddRange(subjects);
            }

            return neoResponse;
        }

        private static List<MatchItem> filterClickProgramsByCap(List<MatchItem> matchList)
        {
            var matchesToRemove = RulesEngine.ProcessClickProgramRules(matchList);

            if (matchesToRemove.Any())
                matchList = matchList.Except(matchesToRemove).ToList();

            return matchList;
        }

        public static ApiProgramResponse CreateApiProgramResponse(MatchResult mr)
        {
            ApiProgramResponse api = new ApiProgramResponse();

            if (mr.MatchItemList.Count > 0)
            {
                var brCache = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData);
                var kvDataCache = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_KVCodeDataCache>>(MatchingCacheItem.REKVCodeData);
                CountryGeoCacheItem countryCache = StaticCacheProxyHost.CacheProxy.Get<CountryGeoCacheItem>(MatchingCacheItem.RECountryGeoData);
                StateGeoCacheItem stateCache = StaticCacheProxyHost.CacheProxy.Get<StateGeoCacheItem>(MatchingCacheItem.REStateGeoData);

                Dictionary<int, VW_Matching_ClientCampusProductMappingCache> crCampusList =
                    StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_ClientCampusProductMappingCache>>(MatchingCacheItem.REClientCampusProductMapping);
                Dictionary<int, VW_Matching_ClientRelationProductMappingCache> crList =
                StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_ClientRelationProductMappingCache>>(MatchingCacheItem.REClientRelationProductMapping);

                MatchItem mi = mr.MatchItemList.FirstOrDefault();
                api.Program = new DTO.ApiProgram();
                api.Program.InstitutionId = mi.Match.InstitutionId;
                api.Program.InstitutionName = mi.Match.InstitutionName;
                api.Program.CampusId = mi.Match.CampusId;
                api.Program.CampusName = mi.Match.CampusName;
                //api.Program.HasCampusLogo = mi.Match.HasCampusLogo;
                api.Program.CampusLogoURL = mi.Match.CampusLogoURL;
                api.Program.ProgramId = mi.Match.ProgramId;
                api.Program.ProgramName = mi.Match.ProgramName;
                api.Program.ProgramDescription = mi.Match.ProgramDescription;
                api.Program.CategoryId = mi.Match.PrimaryCategoryId;
                api.Program.SubjectId = mi.Match.PrimarySubjectId;
                api.Program.ProgramLevelId = mi.Match.ProgramLevelId;
                api.Program.InstitutionLogoURL = mi.Match.InstitutionLogoURL;

                bool includeAllCountries = true;
                bool includeAllStates = true;
                HashSet<int> states = null;
                HashSet<int> countries = null;
                HashSet<ApiRule> rules = new HashSet<ApiRule>();

                foreach (var m in mr.MatchItemList)
                {
                    if (!m.FailedValidation)
                    {
                        #region Populate Rules
                        //Temporarily Commented out till I can talk to Erick about updating it
                        //string k = m.Match.ProgramId + "_" + m.Match.ProductId;

                        int k = m.Match.ProgramProductId;
                        foreach (var key in brCache.Keys)
                        {
                            if (brCache[key].ProgramProductAssignments.ContainsKey(k))
                            {
                                foreach (var rule in brCache[key].ProgramProductAssignments[k])
                                {
                                    ApiRule rl = new ApiRule();
                                    rl.EntityValue = rule.EntityValue;
                                    rl.RuleId = rule.RuleId;
                                    rl.RuleName = rule.RuleName;
                                    rl.StandardControlCode = rule.StandardControlCode;

                                    rl.ValidKeyValueCodeData = kvDataCache.Where(kvp => rule.KeyValueCodeIds.Contains(kvp.Value.KVCodeDataId)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value);

                                    rules.Add(rl);
                                }
                            }
                        }

                        #endregion

                        if (includeAllCountries)
                        {
                            if (crList.ContainsKey(m.Match.ClientRelationProductMappingId) && crList[m.Match.ClientRelationProductMappingId].IncludeAllCountries == false)
                                includeAllCountries = false;
                            else if (crCampusList.ContainsKey(m.Match.ClientCampusProductMappingId) && crCampusList[m.Match.ClientCampusProductMappingId].IncludeAllCountries == false)
                                includeAllCountries = false;
                        }

                        if (includeAllStates)
                        {
                            if (crList.ContainsKey(m.Match.ClientRelationProductMappingId) && crList[m.Match.ClientRelationProductMappingId].IncludeAllStates == false)
                                includeAllStates = false;
                            else if (crCampusList.ContainsKey(m.Match.ClientCampusProductMappingId) && crCampusList[m.Match.ClientCampusProductMappingId].IncludeAllStates == false)
                                includeAllStates = false;
                        }

                        if (!includeAllCountries)
                        {
                            if (countryCache.ClientRelationProductMappingList.ContainsKey(m.Match.ClientRelationProductMappingId))
                            {
                                if (countries == null)
                                    countries = new HashSet<int>(countryCache.ClientRelationProductMappingList[m.Match.ClientRelationProductMappingId]);
                                else
                                    countries.IntersectWith(countryCache.ClientRelationProductMappingList[m.Match.ClientRelationProductMappingId]);
                            }

                            if (countryCache.ClientCampusProductMappingList.ContainsKey(m.Match.ClientCampusProductMappingId))
                            {
                                if (countries == null)
                                    countries = new HashSet<int>(countryCache.ClientCampusProductMappingList[m.Match.ClientCampusProductMappingId]);
                                else
                                    countries.IntersectWith(countryCache.ClientCampusProductMappingList[m.Match.ClientCampusProductMappingId]);
                            }

                            if (countryCache.ProgramProductMappingList.ContainsKey(m.Match.ProgramProductId))
                            {
                                if (countries == null)
                                    countries = new HashSet<int>(countryCache.ProgramProductMappingList[m.Match.ProgramProductId]);
                                else
                                    countries.IntersectWith(countryCache.ProgramProductMappingList[m.Match.ProgramProductId]);
                            }
                        }

                        if (!includeAllStates)
                        {
                            if (stateCache.ClientRelationProductMappingList.ContainsKey(m.Match.ClientRelationProductMappingId))
                            {
                                if (states == null)
                                    states = new HashSet<int>(stateCache.ClientRelationProductMappingList[m.Match.ClientRelationProductMappingId]);
                                else
                                    states.IntersectWith(stateCache.ClientRelationProductMappingList[m.Match.ClientRelationProductMappingId]);
                            }

                            if (stateCache.ClientCampusProductMappingList.ContainsKey(m.Match.ClientCampusProductMappingId))
                            {
                                if (states == null)
                                    states = new HashSet<int>(stateCache.ClientCampusProductMappingList[m.Match.ClientCampusProductMappingId]);
                                else
                                    states.IntersectWith(stateCache.ClientCampusProductMappingList[m.Match.ClientCampusProductMappingId]);
                            }

                            if (stateCache.ProgramProductMappingList.ContainsKey(m.Match.ProgramProductId))
                            {
                                if (states == null)
                                    states = new HashSet<int>(stateCache.ProgramProductMappingList[m.Match.ProgramProductId]);
                                else
                                    states.IntersectWith(stateCache.ProgramProductMappingList[m.Match.ProgramProductId]);
                            }
                        }
                    }
                }

                api.Countries = countries == null ? null : countries.ToList();
                api.IncludesAllCountries = includeAllCountries;
                api.IncludesAllStates = includeAllStates;
                api.States = states == null ? null : states.ToList();
                api.Rules = rules.ToList();
            }
            return api;
        }

        public static List<ProgramRuleDefinition> GetRulesForProgramProduct(int programProductId)
        {
            List<RuleDefinition> rules = new List<RuleDefinition>();
            List<ProgramRuleDefinition> ret = new List<ProgramRuleDefinition>();
            Dictionary<BaseRuleDefinitionType, BaseRuleDefinition> br = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData);
            foreach (var brd in br)
            {
                if (brd.Value.ProgramProductAssignments.ContainsKey(programProductId))
                    rules.AddRange(brd.Value.ProgramProductAssignments[programProductId].Distinct());
            }
            //we have the list of rules now we need to translate to a collection of program rule definitions
            for (int i = 0; i < rules.Count; i++)
            {
                ProgramRuleDefinition prd = new ProgramRuleDefinition();
                prd.BaseRuleType = rules[i].BaseRuleType;
                prd.EntityValue = rules[i].EntityValue;
                prd.KeyValueCodeIds = rules[i].KeyValueCodeIds;
                prd.RuleName = rules[i].RuleName;
                prd.StandardControlCode = rules[i].StandardControlCode;
                prd.BaseRuleType = rules[i].BaseRuleType;
                if (rules[i].IsLowerBound)
                {
                    prd.LowerBound = rules[i].EntityValue;
                }
                else if (rules[i].IsUpperBound)
                {
                    prd.UpperBound = rules[i].EntityValue;
                }

                //look ahead until we finish the standard control codes definitions and use it to set upper and lower
                int k = i + 1;
                string currentScc = rules[i].StandardControlCode;
                while (k < rules.Count && rules[k].StandardControlCode == currentScc)
                {
                    if (k < rules.Count && rules[k].StandardControlCode == currentScc)
                    {
                        //we have another rule for this control
                        if (rules[k].IsLowerBound)
                        {
                            prd.LowerBound = rules[k].EntityValue;
                        }
                        else if (rules[k].IsUpperBound)
                        {
                            prd.UpperBound = rules[k].EntityValue;
                        }
                        if (prd.EntityValue == null && rules[k].EntityValue != null)
                        {
                            prd.EntityValue = rules[k].EntityValue;
                        }
                    }
                    else
                    {
                        currentScc = "";
                    }
                    k++;
                }
                i = k - 1;
                ret.Add(prd);
            }

            return ret;
        }
    }
}
